using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AdviseService;
using Service.AuthService;
using Service.CategoryService;
using Service.ExpertService;
using Service.UserService;
using System.ComponentModel.DataAnnotations;
using ViewModel.Advise.Request;
using ViewModel.ErrorDTO;

namespace ExpertConnection.Controllers
{
    [Route("api/advise")]
    [ApiController]
    public class AdviseController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string AUTHORIZATION = "Authorization";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private IAdviseService _adviseService;
        private IAuthService _authService;
        private IExpertService _expertService;
        private IUserService _userService;
        private static AuthError error = new AuthError();

        public AdviseController(IAdviseService adviseService, IAuthService authService, IUserService userService, IExpertService expertService)
        {
            _adviseService = adviseService;
            _authService = authService;
            _expertService = expertService;
            _userService = userService;
        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAdvise(AdviseCreateViewModel acv)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(USER))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _adviseService.CreateAdvise(acv);
                if (rs is JsonResult jsonResult)
                {
                    return StatusCode(StatusCodes.Status201Created, jsonResult.Value);
                }
                if (rs is StatusCodeResult status)
                {
                    if (status.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError); }
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }

        [HttpPut]
        public async Task<IActionResult> ConfirmAdvise([Required] Guid adviseId,[Required] Guid id)
            {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                var token = await _authService.CheckTokenAsync(header);
                error.Message = NOT_ACCESS;
                if (token == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                bool check = true;
                var user = await _userService.GetUserByAccId(token.AccId);
                var expert = await _expertService.GetExpertByAccId(token.AccId);
                if (user != null && !user.Id.Equals(id.ToString()))
                {
                    check = false;
                }
                else if (expert != null && !expert.Id.Equals(id.ToString()))
                {
                    check = false;
                }
                if (!check)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var rs = await _adviseService.Confirm(adviseId, id);
                if (rs is StatusCodeResult status)
                {
                    switch (status.StatusCode)
                    {
                        case 200: return Ok();
                        case 500: return StatusCode(StatusCodes.Status500InternalServerError);
                        case 400: return BadRequest(new { Message = "AdviseId invalid" });
                    }
                }
            }
            error.Message = UNAUTHORIED;
            return BadRequest(error);
        }
    }
}
