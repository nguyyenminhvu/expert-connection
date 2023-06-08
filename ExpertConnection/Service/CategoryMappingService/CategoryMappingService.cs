using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataConnection.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModel.CategoryMapping.Request;
using ViewModel.CategoryMapping.View;

namespace Service.CategoryMappingService
{
    public interface ICategoryMappingService
    {
        public Task<IActionResult> RegisterCategoryMapping(CategoryMappingCreate cmc);
        public Task<IActionResult> GetCategoriesMapping(CategoryMappingSearch categoryMapping);
        public Task<IActionResult> UpdateCategoryMapping(Guid categoryMappingId, CategoryMappingUpdate cmu);
        public Task<IActionResult> GetCategoryMapping(Guid id);
        public Task<IActionResult> RemoveCategoryMapping(Guid id);
        public Task<CategoryMappingViewModel> UpdateRatingCategoryMapping(Guid id, int adviseNumber, double ratingNew);
        public Task<int> GetCategoryMappingCountByExpertId(Guid expertId);
    }
    public class CategoryMappingService : ICategoryMappingService
    {
        private IMapper _mapper;

        private ExpertConnectionContext _context;

        #region Filter-Search
        private IQueryable<CategoryMapping> FilterPrice(double? priceFrom, double? priceTo, IQueryable<CategoryMapping> queryable)
        {
            if (priceFrom != null && priceTo != null)
            {
                queryable = queryable.Where(x => x.Price >= priceFrom && x.Price <= priceTo);
            }
            else if (priceFrom != null && priceTo == null)
            {
                queryable = queryable.Where(x => x.Price >= priceFrom);
            }
            else if (priceFrom == null && priceTo != null)
            {
                queryable = queryable.Where(x => x.Price <= priceTo);
            }
            return queryable;
        }
        private IQueryable<CategoryMapping> FilterExperienceYear(double? expFrom, double? expTo, IQueryable<CategoryMapping> queryable)
        {
            if (expFrom != null && expTo != null)
            {
                queryable = queryable.Where(x => x.ExperienceYear >= expFrom && x.ExperienceYear <= expTo);
            }
            else if (expFrom != null && expTo == null)
            {
                queryable = queryable.Where(x => x.ExperienceYear >= expFrom);
            }
            else if (expFrom == null && expTo != null)
            {
                queryable = queryable.Where(x => x.ExperienceYear <= expTo);
            }
            return queryable;
        }
        private IQueryable<CategoryMapping> FilterSummaryRating(double? ratingFrom, double? ratingTo, IQueryable<CategoryMapping> queryable)
        {
            if (ratingFrom != null && ratingTo != null)
            {
                queryable = queryable.Where(x => x.SummaryRating >= ratingFrom && x.SummaryRating <= ratingTo);
            }
            else if (ratingFrom != null && ratingTo == null)
            {
                queryable = queryable.Where(x => x.SummaryRating >= ratingFrom);
            }
            else if (ratingFrom == null && ratingTo != null)
            {
                queryable = queryable.Where(x => x.SummaryRating <= ratingTo);
            }
            return queryable;
        }
        #endregion

        public CategoryMappingService(ExpertConnectionContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> RegisterCategoryMapping(CategoryMappingCreate cmc)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(cmc.CategoryId) && x.IsActive);
            var expert = await _context.Experts.FirstOrDefaultAsync(x => x.Id.Equals(cmc.ExpertId));
            if (category != null && expert != null)
            {
                CategoryMapping categoryMapping = new CategoryMapping
                {
                    Id = Guid.NewGuid().ToString(),
                    ExpertId = cmc.ExpertId,
                    CategoryId = cmc.CategoryId,
                    Price = cmc.Price,
                    ExperienceYear = cmc.ExperienceYear,
                    SummaryRating = cmc.SummaryRating,
                    Introduction = cmc.Introduction,
                    Description = cmc.Description,
                    IsActive = false,
                    IsConfirmed = false
                };
                await _context.CategoryMappings.AddAsync(categoryMapping);
                var categoryMappingViewModel = await _context.CategoryMappings.Where(x => x.Id.Equals(categoryMapping.Id)).ProjectTo<CategoryMappingViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
                return await _context.SaveChangesAsync() > 0 ? new JsonResult(categoryMappingViewModel) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }

        public async Task<IActionResult> GetCategoriesMapping(CategoryMappingSearch categoryMapping)
        {
            IQueryable<CategoryMapping> query = _context.CategoryMappings.Where(x => x.IsActive);

            query = FilterPrice(categoryMapping.FromPrice, categoryMapping.ToPrice, query);
            query = FilterExperienceYear(categoryMapping.FromExperienceYear, categoryMapping.ToExperienceYear, query);
            query = FilterSummaryRating(categoryMapping.FromSummaryRating, categoryMapping.ToSummaryRating, query);

            if (categoryMapping.CategoryId != null)
            {
                query = query.Where(x => x.CategoryId.Equals(categoryMapping.CategoryId.ToString()));
            }
            if (categoryMapping.ExpertId != null)
            {
                query = query.Where(x => x.Expert.Id.Equals(categoryMapping.ExpertId.ToString()));
            }
            return new JsonResult(await query.ProjectTo<CategoryMappingViewModel>(_mapper.ConfigurationProvider).ToListAsync());
        }

        public async Task<IActionResult> UpdateCategoryMapping(Guid categoryMappingId, CategoryMappingUpdate cmu)
        {
            var categoryMapping = await _context.CategoryMappings.Where(x => x.Id.Equals(categoryMappingId.ToString()) && x.IsActive).FirstOrDefaultAsync();
            if (categoryMapping != null)
            {
                categoryMapping.ExpertId = cmu.ExpertId ?? categoryMapping.ExpertId;
                categoryMapping.CategoryId = cmu.CategoryId ?? categoryMapping.CategoryId;
                categoryMapping.Price = cmu.Price ?? categoryMapping.Price;
                categoryMapping.ExperienceYear = cmu.ExperienceYear ?? categoryMapping.ExperienceYear;
                categoryMapping.SummaryRating = cmu.SummaryRating ?? categoryMapping.SummaryRating;
                categoryMapping.Introduction = cmu.Introduction ?? categoryMapping.Introduction;
                categoryMapping.Description = cmu.Description ?? categoryMapping.Description;
                categoryMapping.IsConfirmed = false;
                return await _context.SaveChangesAsync() > 0 ? new StatusCodeResult(200) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }

        public async Task<IActionResult> GetCategoryMapping(Guid id)
        {
            var categoryMapping = await _context.CategoryMappings.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()) && x.IsActive);
            if (categoryMapping != null)
            {
                return new JsonResult(_mapper.Map<CategoryMappingViewModel>(categoryMapping));
            }
            return new JsonResult(null);
        }

        public async Task<IActionResult> RemoveCategoryMapping(Guid id)
        {
            var categoryMapping = await _context.CategoryMappings.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()));
            if (categoryMapping != null)
            {
                categoryMapping.IsActive = false;
                return await _context.SaveChangesAsync() > 0 ? new StatusCodeResult(200) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }

        public async Task<int> GetCategoryMappingCountByExpertId(Guid expertId)
        {
            return (await _context.CategoryMappings.Where(x => x.ExpertId.Equals(expertId)).ToListAsync()).Count();
        }

        public async Task<CategoryMappingViewModel> UpdateRatingCategoryMapping(Guid id, int adviseNumber, double ratingNew)
        {
            var categoryMapping = await _context.CategoryMappings.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()));
            if (categoryMapping != null)
            {
                double ratingUpdate = (categoryMapping.SummaryRating * adviseNumber + ratingNew) /( adviseNumber + 1);
                categoryMapping.SummaryRating = ratingUpdate;
                return await _context.SaveChangesAsync() > 0 ? _mapper.Map<CategoryMappingViewModel>(categoryMapping) : null!;
            }
            return null!;
        }
    }
}
