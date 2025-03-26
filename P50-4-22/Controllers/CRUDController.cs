using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;

namespace P50_4_22.Controllers
{
	public class CRUDController : Controller
	{

		public PetStoreRpmContext db;
        private readonly IWebHostEnvironment env;

        public CRUDController(PetStoreRpmContext context, IWebHostEnvironment env)
        {
            db = context;
            this.env = env;
        }

        public async Task<IActionResult> AddProduct()
		{
            var products = await db.CatalogProducts.ToListAsync();
            var brands = await db.Brands.ToListAsync();
            var categories = await db.Categories.ToListAsync(); 

            var model = new Tuple<IEnumerable<CatalogProduct>, IEnumerable<Brand>>(products, brands);

            ViewBag.Brands = new SelectList(brands, "IdBrands", "Brand1"); 
            ViewBag.Categories = new SelectList(categories, "IdCategories", "CategoryName");

            return View(model);
        }

		public IActionResult Create()
		{
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(CatalogProduct cp)
		{
            if (ModelState.IsValid)
            {
                db.CatalogProducts.Add(cp);
                await db.SaveChangesAsync();
                return RedirectToAction("AddProduct");
            }
            ViewBag.Brands = new SelectList(await db.Brands.ToListAsync(), "IdBrands", "Brand1");
            ViewBag.Categories = new SelectList(await db.Categories.ToListAsync(), "IdCategories", "CategoryName");
            return View(cp);
        }
		

		public async Task<IActionResult> Details(int? id)
		{
			if (id != null)
			{
				CatalogProduct cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
				if (cp != null)
				{
					return View(cp);
				}
			}
			return NotFound();
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id != null)
			{
				CatalogProduct cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
				if (cp != null)
				{
					return View(cp);
				}
			}
			return NotFound();
		}

		[HttpPost]
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
			if (id != null)
			{
				CatalogProduct cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
				if (cp != null)
				{
					return View(cp);
				}
			}
			return NotFound();
		}
		[HttpPost]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id != null)
			{
				CatalogProduct cp = await db.CatalogProducts.FirstOrDefaultAsync(p => p.IdCatalogproducts == id);
				if (cp != null)
				{
					db.CatalogProducts.Remove(cp);
					await db.SaveChangesAsync();
					return RedirectToAction("AddProduct");
				}
			}
			return NotFound();
		}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {
            if (db.Brands.Any(b => b.Brand1.ToLower() == brand.Brand1.ToLower()))
            {
                ModelState.AddModelError("Brand1", "Бренд с таким названием уже существует.");
            }

            if (string.IsNullOrWhiteSpace(brand.ImgBrand))
            {
                ModelState.AddModelError("ImgBrand", "Пожалуйста, укажите URL изображения.");
            }

            if (ModelState.IsValid)
            {
                db.Brands.Add(brand);
                await db.SaveChangesAsync();
                return RedirectToAction("AddProduct");
            }

            var products = await db.CatalogProducts.ToListAsync();
            var brands = await db.Brands.ToListAsync();
            var categories = await db.Categories.ToListAsync();
            var model = new Tuple<IEnumerable<CatalogProduct>, IEnumerable<Brand>>(products, brands);
            ViewBag.Brands = new SelectList(brands, "IdBrands", "Brand1");
            ViewBag.Categories = new SelectList(categories, "IdCategories", "CategoryName");
            return View("AddProduct", model);
        }
    }
}
