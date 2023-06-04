using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AccountService;
using Service.AuthService;
using Service.EmployeeService;
using Service.UserService;
using ViewModel.Auth;
using ViewModel.Employee;
using ViewModel.ErrorDTO;

namespace ExpertConnection.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private static string AUTHORIZATION = "Authorization";
        private IUserService _userService;
        private IAuthService _authService;
        private IAccountService _accountService;
        private IEmployeeService _employeeService;
        private static AuthError error = new AuthError();
        public EmployeeController(IAccountService accountService, IEmployeeService employeeService, IAuthService authService, IUserService userService)
        {
            _userService = userService;
            _authService = authService;
            _accountService = accountService;
            _employeeService = employeeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateViewModel employeeCreate)
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
                    var accId = Guid.NewGuid().ToString();
                    bool accCreated = await _accountService.CreateAccountAsync(employeeCreate.Usermame, employeeCreate.Password, EMPLOYEE, accId);
                    if (accCreated)
                    {
                        var created = await _employeeService.CreateEmployee(accId, employeeCreate.Fullname);
                        return created ? Ok("Create successful") : BadRequest();
                    }
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


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel emp)
        {
            if (ModelState.IsValid)
            {
                var rs = await _authService.LoginAsync(emp, "Employee");
                return rs != null ? Ok(rs) : NotFound();
            }
            else
            {
                var er = ModelState.Select(x => x.Value.Errors).Where(x => x.Count > 0).ToList();
                return BadRequest(er);
            }
        }


       


        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> Accept([FromRoute] Guid id)
        {
            var header = Request.Headers[AUTHORIZATION].FirstOrDefault()?.Split(" ").Last().ToString();
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
                    var rs = await _employeeService.Accept(id);
                    return rs ? Ok(new { Message = "success" }) : BadRequest();
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
