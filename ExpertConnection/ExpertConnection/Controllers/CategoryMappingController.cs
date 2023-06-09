﻿using Microsoft.AspNetCore.Mvc;
using Service.AuthService;
using Service.CategoryMappingService;
using Service.CategoryService;
using ViewModel.CategoryMapping.Request;
using ViewModel.ErrorDTO;
using ViewModel.CategoryMapping.Request;
using System.Runtime.InteropServices;
using Service.ExpertService;
using DataConnection.Entities;
using ViewModel.CategoryMapping.View;

namespace ExpertConnection.Controllers
{
    [Route("api/categories-mapping")]
    [ApiController]
    public class CategoryMappingController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private static string AUTHORIZATION = "Authorization";
        private IExpertService _expertService;
        private ICategoryMappingService _categoryMappingService;
        private ICategoryService _categoryService;
        private IAuthService _authService;
        private static AuthError error = new AuthError();

        public CategoryMappingController(ICategoryService categoryService, IAuthService authService, ICategoryMappingService categoryMappingService, IExpertService expertService)
        {
            _expertService = expertService;
            _categoryMappingService = categoryMappingService;
            _categoryService = categoryService;
            _authService = authService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategoriesMapping([FromQuery] CategoryMappingSearch cm)
        {
            var rs = await _categoryMappingService.GetCategoriesMapping(cm);
            if (rs is JsonResult json)
            {
                return Ok(json.Value);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCategoryMapping([FromRoute] Guid id)
        {
            var rs = await _categoryMappingService.GetCategoryMapping(id);
            if (rs is JsonResult json)
            {
                return Ok(json.Value);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "CategoryMappingId invalid" });

        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterCategoryMapping(CategoryMappingCreate cmc)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EXPERT))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _categoryMappingService.RegisterCategoryMapping(cmc);
                if (rs is JsonResult json)
                {
                    return Ok(json.Value);
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> CategoryMappingUpdate([FromRoute] Guid id, [FromQuery] CategoryMappingUpdate cmu)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var checkToken = await _authService.CheckTokenAsync(header);
                if (checkToken == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var auth = await _expertService.CheckExist(checkToken.AccId);
                var categoryQuery = await _categoryMappingService.GetCategoryMapping(id);
                if (categoryQuery is JsonResult json)
                {
                    if (json.Value == null) { return BadRequest(new { Message = "CategoryMappingId invalid" }); }
                    var category = json.Value as CategoryMappingViewModel;
                    if (!auth.Id.Equals(category.ExpertId))
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, error);
                    }
                }
                var rs = await _categoryMappingService.UpdateCategoryMapping(id, cmu);
                if (rs is StatusCodeResult status)
                {
                    if (status.StatusCode == 200) return Ok();
                    if (status.StatusCode == 400) return BadRequest(new { Message = "CategoryMappingId invalid" });
                    if (status.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCategoryMapping([FromRoute] Guid id)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EXPERT))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var auth = await _expertService.CheckExist(token.AccId);
                if (auth == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _categoryMappingService.RemoveCategoryMapping(id);
                if (rs is StatusCodeResult status)
                {
                    switch (status.StatusCode)
                    {
                        case 200: return StatusCode(StatusCodes.Status204NoContent);
                        case 500: return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Status500InternalServerError" });
                        case 400: return StatusCode(StatusCodes.Status400BadRequest, new { Message = "CategoryMappingId invalid" });
                    }
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }

    }
}
