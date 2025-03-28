using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;
using System.Security.Claims;
using System.Text.Json;

namespace P50_4_22.Controllers
{
    public class UserController : Controller
    {
        private readonly PetStoreRpmContext db;

        public UserController(PetStoreRpmContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Catalog(string[] brand)
        {
            IQueryable<CatalogProduct> products = db.CatalogProducts
                .Include(p => p.Brands)
                .Include(p => p.Categories);
            if (brand?.Any() == true)
            {
                products = products.Where(p => brand.Contains(p.Brands.Brand1));
            }

            // Получаем список избранных товаров из сессии
            var favoriteProductIds = GetFavoriteProductIds();
            ViewBag.FavoriteProductIds = favoriteProductIds;

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await db.CatalogProducts
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Users)
                .FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
            return View(product);
        }

        public async Task<IActionResult> AddToCart(int catalogId, int quantity)
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return RedirectToAction("Profile", "Profile");
            }
            var catalog = await db.CatalogProducts.FindAsync(catalogId);
            if (catalog == null)
            {
                return NotFound("Такого товара нет");
            }
            if (catalog.Quantity < quantity)
            {
                return BadRequest("Столько нет, кыш");
            }
            var cart = await db.Carts.FirstOrDefaultAsync(c => c.CatalogId == catalogId && c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Quantity = quantity,
                    Price = catalog.PriceOfProduct * quantity,
                    CatalogId = catalogId,
                    UserId = userId
                };
                db.Carts.Add(cart);
            }
            else
            {
                cart.Quantity += quantity;
                if (cart.Quantity > catalog.Quantity)
                {
                    return BadRequest("Мало товара эген");
                }
                cart.Price = cart.Quantity * catalog.PriceOfProduct;
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Review review)
        {
            review.CreatedAt = DateTime.Now;
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            review.UsersId = userId;
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = review.CatalogroductId });
        }

        public IActionResult Cart()
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return RedirectToAction("Profile", "Profile");
            }
            var cart = db.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Catalog)
                .ToList();
            return View(cart);
        }

        [HttpPost]
        public IActionResult EditQuantityCart(int CatalogId, int quantity)
        {
            var cart = db.Carts.FirstOrDefault(c => c.CatalogId == CatalogId);
            var catalog = db.CatalogProducts.Find(CatalogId);
            if (cart != null)
            {
                cart.Quantity = quantity;
                cart.Price = quantity * catalog.PriceOfProduct;
                db.SaveChanges();
            }
            return RedirectToAction("Cart");
        }

        // Метод для добавления/удаления товара в избранное
        [Authorize]
        [HttpPost]
        public IActionResult ToggleFavorite(int productId)
        {
            var favoriteProductIds = GetFavoriteProductIds();

            if (favoriteProductIds.Contains(productId))
            {
                // Удаляем из избранного
                favoriteProductIds.Remove(productId);
            }
            else
            {
                // Добавляем в избранное
                favoriteProductIds.Add(productId);
            }

            // Сохраняем обновлённый список в сессии
            HttpContext.Session.SetString("Favorites", JsonSerializer.Serialize(favoriteProductIds));

            return Json(new { success = true, isFavorite = favoriteProductIds.Contains(productId) });
        }

        // Метод для отображения страницы избранного
        [Authorize]
        public IActionResult Favorites()
        {
            var favoriteProductIds = GetFavoriteProductIds();
            var favoriteProducts = db.CatalogProducts
                .Where(p => favoriteProductIds.Contains(p.IdCatalogproducts))
                .ToList();

            return View(favoriteProducts);
        }

        // Метод для добавления товара из избранного в корзину
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCartFromFavorites(int productId)
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return Json(new { success = false, message = "Необходимо авторизоваться" });
            }

            var product = await db.CatalogProducts.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Товар не найден" });
            }

            var cartItem = await db.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.CatalogId == productId);
            if (cartItem == null)
            {
                db.Carts.Add(new Cart
                {
                    UserId = userId,
                    CatalogId = productId,
                    Quantity = 1,
                    Price = product.PriceOfProduct
                });
            }
            else
            {
                cartItem.Quantity++;
                cartItem.Price = cartItem.Quantity * product.PriceOfProduct;
                db.Carts.Update(cartItem);
            }

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // Метод для удаления товара из избранного
        [Authorize]
        [HttpPost]
        public IActionResult RemoveFromFavorites(int productId)
        {
            var favoriteProductIds = GetFavoriteProductIds();
            if (favoriteProductIds.Contains(productId))
            {
                favoriteProductIds.Remove(productId);
                HttpContext.Session.SetString("Favorites", JsonSerializer.Serialize(favoriteProductIds));
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Товар не найден в избранном" });
        }

        // Вспомогательный метод для получения списка избранных товаров из сессии
        private List<int> GetFavoriteProductIds()
        {
            var favoritesJson = HttpContext.Session.GetString("Favorites");
            if (string.IsNullOrEmpty(favoritesJson))
            {
                return new List<int>();
            }

            return JsonSerializer.Deserialize<List<int>>(favoritesJson) ?? new List<int>();
        }
    }
}