using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;

namespace P50_4_22.Controllers
{
    public class CRUDController : Controller
    {
        private readonly PetStoreRpmContext db;
        private readonly IWebHostEnvironment env;

        public CRUDController(PetStoreRpmContext context, IWebHostEnvironment env)
        {
            db = context;
            this.env = env;
        }

        public async Task<IActionResult> AddProduct()
        {
            var products = await db.CatalogProducts.ToListAsync();
            var brands = await db.Brands.ToListAsync() ?? new List<Brand>();
            var categories = await db.Categories.ToListAsync() ?? new List<Category>();

            var model = new Tuple<IEnumerable<CatalogProduct>, IEnumerable<Brand>, IEnumerable<Category>>(products, brands, categories);

            ViewBag.Brands = new SelectList(brands, "IdBrands", "Brand1");
            ViewBag.Categories = new SelectList(categories, "IdCategories", "Categories");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CatalogProduct cp)
        {
            db.CatalogProducts.Add(cp);
            await db.SaveChangesAsync();
            return RedirectToAction("AddProduct");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
            if (cp == null)
            {
                return NotFound();
            }

            return View(cp);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
            if (cp == null)
            {
                return NotFound();
            }

            return View(cp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CatalogProduct cp)
        {
            db.CatalogProducts.Update(cp);
            await db.SaveChangesAsync();
            return RedirectToAction("AddProduct");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
            if (cp == null)
            {
                return NotFound();
            }

            return View(cp);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
            if (cp != null)
            {
                db.CatalogProducts.Remove(cp);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("AddProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            db.Brands.Add(brand);
            await db.SaveChangesAsync();
            return RedirectToAction("AddProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return RedirectToAction("AddProduct");
        }
    }
}