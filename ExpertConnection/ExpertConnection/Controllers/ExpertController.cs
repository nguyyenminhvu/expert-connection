using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AccountService;
using Service.AuthService;
using Service.EmployeeService;
using Service.ExpertService;
using ViewModel.ErrorDTO;
using ViewModel.Auth;
using ViewModel.Expert.Request;

namespace ExpertConnection.Controllers
{
    [Route("api/experts")]
    [ApiController]
    public class ExpertController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private static string AUTHORIZATION = "Authorization";
        private IExpertService _expertService;
        private IAuthService _authService;
        private static AuthError error = new AuthError();

        public ExpertController(IAuthService authService, IExpertService expertService)
        {
            _expertService = expertService;
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var rs = await _authService.LoginAsync(loginViewModel, "Expert");
            return rs != null ? Ok(rs) : BadRequest(new { Message = "Username or password invalid" });
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var checkToken = await _authService.CheckTokenAsync(header);
                if (checkToken == null || !checkToken.RoleName.Equals("Employee"))
                {
                    return NotFound(error);
                }
                if (ModelState.IsValid)
                {
                    return Ok(await _expertService.GetAll());
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpPost("register")]
        public async Task<IActionResult> CreateExpert(ExpertCreateViewModel expertCreate)
        {
            if (ModelState.IsValid)
            {
                var rs = await _expertService.CreateExpert(expertCreate);
                return rs ? StatusCode(StatusCodes.Status201Created, expertCreate) : StatusCode(StatusCodes.Status400BadRequest, new { Message = "Username already exist" });
            }
            else
            {
                var er = ModelState.Select(x => x.Value.Errors).Where(x => x.Count > 0).ToList();
                return BadRequest(er);
            }
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetExpert([FromRoute] Guid id)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                var token = await _authService.CheckTokenAsync(header);
                error.Message = NOT_ACCESS;
                if (token == null || !token.RoleName.Equals(EXPERT))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var auth = await _expertService.CheckExist(token.AccId);
                if (auth is null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _expertService.GetExpert(id);
                if (rs is JsonResult json)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateExpert(Guid id, ExpertUpdate expertUpdate)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {

                var token = await _authService.CheckTokenAsync(header);
                error.Message = NOT_ACCESS;
                if (token == null || !token.RoleName.Equals(EXPERT))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var auth = await _expertService.CheckExist(token.AccId);
                if (auth is null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                return await _expertService.UpdateExpert(id, expertUpdate) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }
    }
}
