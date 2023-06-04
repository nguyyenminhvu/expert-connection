using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AuthService;
using Service.CategoryService;
using ViewModel.Category.Request;
using ViewModel.ErrorDTO;

namespace ExpertConnection.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string AUTHORIZATION = "Authorization";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private ICategoryService _categoryService;
        private IAuthService _authService;
        private static AuthError error = new AuthError();


        public CategoryController(ICategoryService categoryService, IAuthService authService)
        {
            _categoryService = categoryService;
            _authService = authService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rs = await _categoryService.GetAll();
            if (rs is JsonResult json)
            {
                return Ok(json.Value);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token != null && token.RoleName.Equals(EXPERT))
                {
                    var rs = await _categoryService.CreateCategory(name);
                    if (rs is JsonResult jsonResult)
                    {
                        return rs != null ? StatusCode(StatusCodes.Status201Created, jsonResult.Value) : StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                error.Message = NOT_ACCESS;
                return StatusCode(StatusCodes.Status403Forbidden, error);
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, CategoryUpdateViewModel viewModel)
        {
            var header = Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EMPLOYEE))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _categoryService.UpdateCategory(id, viewModel);
                if (rs is StatusCodeResult status)
                {
                    switch (status.StatusCode)
                    {
                        case 200: return Ok();
                        case 500: return StatusCode(StatusCodes.Status500InternalServerError);
                        case 400: return BadRequest(new { Message = "CategoryId invalid" });
                    }
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCategory([FromRoute] Guid id)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || token.RoleName != EMPLOYEE)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _categoryService.RemoveCategory(id);
                if (rs is StatusCodeResult status)
                {
                    if (status.StatusCode == 200) return StatusCode(StatusCodes.Status204NoContent);
                    if (status.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError);
                    if (status.StatusCode == 400) return BadRequest(new { Message = "CategoryId invalid" });
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }
    }
}
