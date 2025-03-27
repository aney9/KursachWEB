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
            // Выполняем один запрос для получения среднего рейтинга и количества отзывов
            var reviewStats = await _context.Reviews
                .Where(r => r.CatalogroductId == CatalogroductId)
                .GroupBy(r => r.CatalogroductId)
                .Select(g => new
                {
                    avgRating = g.Average(r => (double?)r.Rating) ?? 0,
                    reviewCount = g.Count()
                })
                .FirstOrDefaultAsync();

            // Если отзывов нет, возвращаем значения по умолчанию
            var model = reviewStats ?? new { avgRating = 0.0, reviewCount = 0 };

            return View(model);
        }
    }
}