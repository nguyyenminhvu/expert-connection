using DataConnection.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.AccountService;
using Service.AuthService;
using Service.UserService;
using ViewModel.Auth;
using ViewModel.ErrorDTO;
using ViewModel.User;

namespace ExpertConnection.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IAuthService _authService;
        private IUserService _userService;
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private static string AUTHORIZATION = "Authorization";
        private static AuthError error = new AuthError();

        public UserController(IUserService userService, IAuthService authService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserCreateViewModel userCreate)
        {
            if (ModelState.IsValid)
            {
                var rs = await _userService.CreateUser(userCreate);
                return rs ? StatusCode(StatusCodes.Status201Created, userCreate) : StatusCode(StatusCodes.Status400BadRequest, new { Message = "Username already exist " });
            }
            else
            {
                var er = ModelState.Select(x => x.Value.Errors).Where(x => x.Count > 0).ToList();
                return BadRequest(er);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel loginView)
        {
            if (ModelState.IsValid)
            {
                var rs = await _authService.LoginAsync(loginView, "User");
                return rs != null ? Ok(rs) : StatusCode(StatusCodes.Status400BadRequest, new { Message = "Username or password invalid " });
            }
            else
            {
                var er = ModelState.Select(x => x.Value.Errors).Where(x => x.Count > 0).ToList();
                return BadRequest(er);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                var checkToken = await _authService.CheckTokenAsync(header);
                if (checkToken == null || !checkToken.RoleName.Equals(EMPLOYEE))
                {
                    error.Message = NOT_ACCESS;
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                if (ModelState.IsValid)
                {
                    return Ok(await _userService.GetAll());
                }
                else
                {
                    var er = ModelState.Select(x => x.Value.Errors).Where(x => x.Count > 0).ToList();
                    return BadRequest(er);
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }

    }
}
