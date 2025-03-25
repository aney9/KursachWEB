using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;

namespace P50_4_22.Controllers
{
	public class CRUDController : Controller
	{

		public PetStoreRpmContext db;

		public CRUDController(PetStoreRpmContext context)
		{
			db = context;
		}

		public async Task<IActionResult> AddProduct()
		{
			var products = db.CatalogProducts.ToList();
			var brand = db.Brands.ToList();
			var model = new Tuple<IEnumerable<CatalogProduct>, IEnumerable<Brand>>(products, brand);
			return View(model);
		}

		public IActionResult Create()
		{
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(CatalogProduct cp)
		{
			db.CatalogProducts.Add(cp);
			await db.SaveChangesAsync();
			return RedirectToAction("AddProduct");
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
	}
}
