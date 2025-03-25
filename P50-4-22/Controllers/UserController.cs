using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;
using System.Drawing.Drawing2D;
using System.Security.Claims;

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
            IQueryable<CatalogProduct> products = db.CatalogProducts.Include(p => p.Brands);
            if (brand?.Any() == true)
            {
                products = products.Where(p => brand.Contains(p.Brands.Brand1));
            }
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

		

    }
}
