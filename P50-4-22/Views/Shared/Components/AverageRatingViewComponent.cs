using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;

namespace P50_4_22.Views.Shared.Components
{
    public class AverageRatingViewComponent : ViewComponent
    {
        private readonly PetStoreRpmContext _context;

        public AverageRatingViewComponent(PetStoreRpmContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int CatalogroductId)
        {
            var avgRating = await _context.Reviews
                .Where(r => r.CatalogroductId == CatalogroductId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
            return View(avgRating);
        }
    }
}
