using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.AdviseService;
using Service.AuthService;
using Service.CategoryMappingService;
using Service.ExpertService;
using Service.RatingService;
using Service.UserService;
using ViewModel.ErrorDTO;
using ViewModel.Rating;

namespace ExpertConnection.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private static string EMPLOYEE = "Employee";
        private static string EXPERT = "Expert";
        private static string USER = "User";
        private static string AUTHORIZATION = "Authorization";
        private static string NOT_ACCESS = "Forbidden";
        private static string UNAUTHORIED = "Unauthorized";
        private ICategoryMappingService _categoryMappingService;
        private IRatingService _ratingService;
        private IAdviseService _adviseService;
        private IAuthService _authService;
        private IExpertService _expertService;
        private IUserService _userService;
        private static AuthError error = new AuthError();

        public RatingController(IAdviseService adviseService, IAuthService authService, IUserService userService, IExpertService expertService, IRatingService ratingService, ICategoryMappingService categoryMappingService)
        {
            _categoryMappingService = categoryMappingService;
            _ratingService = ratingService;
            _adviseService = adviseService;
            _authService = authService;
            _expertService = expertService;
            _userService = userService;
        }



        [HttpPost]
        [Route("rate")]
        public async Task<IActionResult> CreateRating(RatingCreateViewModel rcvm)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                var advise = await _adviseService.GetAdviseActive(Guid.Parse(rcvm.AdviseId));
                if (advise == null)
                {
                    return BadRequest(new { Message = "AdviseId invalid" });
                }
                if (!advise.UserId.Equals(rcvm.UserId))
                {
                    return BadRequest(new { Message = "Your action not support" });
                }
                if (await _ratingService.CheckRating(rcvm.AdviseId, rcvm.UserId) is false)
                {
                    return BadRequest(new { Message = "Your action not support" });
                }
                var rs = await _ratingService.CreateRating(rcvm);
                if (rs is JsonResult jsonResult)
                {
                    var adviseUpdate = await _adviseService.GetAdviseActive(Guid.Parse(rcvm.AdviseId));
                    int adviseCount = await _adviseService.GetAdviseCountByCategoryMappingId(Guid.Parse(adviseUpdate.CategoryMappingId));

                    var updateRatingCategoryMapping = await _categoryMappingService.UpdateRatingCategoryMapping(
                        Guid.Parse(adviseUpdate.CategoryMappingId),
                        adviseCount,
                        rcvm.Ratings);

                    var updateRatingExpert = await _expertService.UpdateRatingExpert(
                        Guid.Parse(updateRatingCategoryMapping.ExpertId),
                        await _categoryMappingService.GetCategoryMappingCountByExpertId(Guid.Parse(updateRatingCategoryMapping.ExpertId)),
                        updateRatingCategoryMapping.SummaryRating);

                    return updateRatingExpert ? Ok(jsonResult.Value) : StatusCode(StatusCodes.Status500InternalServerError);
                }
                if (rs is StatusCodeResult status)
                {
                    if (status.StatusCode == 400) return BadRequest(new { Message = "Rating[0-5]" });
                    if (status.StatusCode == 500) return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status401Unauthorized, error);
        }



        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> AcceptRatings(Guid id)
        {
            var header = Request.Headers[AUTHORIZATION].ToString();
            if (!string.IsNullOrEmpty(header))
            {
                error.Message = NOT_ACCESS;
                var token = await _authService.CheckTokenAsync(header);
                if (token == null || !token.RoleName.Equals(EMPLOYEE))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, error);
                }
                return await _ratingService.AcceptRating(id) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
            }
            error.Message = UNAUTHORIED;
            return StatusCode(StatusCodes.Status403Forbidden, error);
        }
    }
}
