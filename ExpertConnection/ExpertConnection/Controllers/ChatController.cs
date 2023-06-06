using DataConnection.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AdviseService;
using Service.AuthService;
using Service.CategoryMappingService;
using Service.CategoryService;
using Service.ChatService;
using Service.ExpertService;
using System.ComponentModel.DataAnnotations;
using ViewModel.Chat.Request;
using ViewModel.ErrorDTO;

namespace ExpertConnection.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private static string AUTHORIZATION = "Authorization";
        private IAdviseService _adviseService;
        private IExpertService _expertService;
        private IAuthService _authService;
        private IChatService _chatService;
        private static AuthError error = new AuthError();

        public ChatController(IChatService chatService, IAuthService authService, IExpertService expertService, IAdviseService adviseService)
        {
            _adviseService = adviseService;
            _expertService = expertService;
            _authService = authService;
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> InsertChat(ChatInsertViewModel cvm)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EXPERT) && !token.RoleName.Equals(USER))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var advise = await _adviseService.GetAdviseActive(Guid.Parse(cvm.AdviseId));
                if (advise is null)
                {
                    return BadRequest(new { Message = "Your action not support" });
                }
                var rs = await _chatService.CreateChat(cvm);
                if (rs is JsonResult json)
                {
                    if (json != null)
                    {
                        return StatusCode(StatusCodes.Status201Created, json.Value);
                    }
                }
                if (rs is StatusCodeResult status)
                {
                    if (status.StatusCode == 500) { return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Can't insert chat" }); }
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }

        [HttpGet]
        public async Task<IActionResult> GetChats([Required] Guid adviseId, [Required] int index)
        {
            error.Message = NOT_ACCESS;
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EXPERT) && !token.RoleName.Equals(USER))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var advise = await _adviseService.GetAdviseActive(adviseId);
                if (advise is null)
                {
                    return BadRequest(new { Message = "Your action not support" });
                }
                var rs = await _chatService.GetChats(adviseId, index);
                if (rs is JsonResult json)
                {
                    return Ok(json.Value);
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }
    }
}
