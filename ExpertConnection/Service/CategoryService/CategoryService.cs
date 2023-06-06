using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataConnection.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using ViewModel.Category;
using ViewModel.Category.Request;
using ViewModel.Category.View;

namespace Service.CategoryService
{
    public interface ICategoryService
    {
        public Task<IActionResult> CreateCategory(string name);
        public Task<IActionResult> UpdateCategory(Guid categoryId, CategoryUpdateViewModel categoryVM);
        public Task<IActionResult> RemoveCategory(Guid idCategory);
        public Task<IActionResult> GetAll();

    }
    public class CategoryService : ICategoryService
    {
        private IMapper _mapper;
        private ExpertConnectionContext _context;

        public CategoryService(ExpertConnectionContext expertConnection, IMapper mapper)
        {
            _mapper = mapper;
            _context = expertConnection;
        }
        public async Task<IActionResult> GetAll()
        {
            return new JsonResult(await _context.Categories.Where(x=>x.IsActive).ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider).ToListAsync());
        }
        public async Task<IActionResult> CreateCategory(string name)
        {
            Category category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                IsActive = true
            };
            _context.Categories.Add(category);
            return await _context.SaveChangesAsync() > 0 ? new JsonResult(new CategoryViewModel { Id = category.Id, Name = category.Name }) : new StatusCodeResult(500); ;
        }
        public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryUpdateViewModel categoryVM)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(categoryId.ToString()) && x.IsActive);
            if (category != null)
            {
                category.Name = categoryVM.Name ?? category.Name;
                return await _context.SaveChangesAsync() > 0 ? new StatusCodeResult(200) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);

        }
        public async Task<IActionResult> RemoveCategory(Guid categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(categoryId.ToString()) && x.IsActive);
            if (category != null)
            {
                category.IsActive = false;
                return await _context.SaveChangesAsync() > 0 ? new StatusCodeResult(200) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }


    }
}
