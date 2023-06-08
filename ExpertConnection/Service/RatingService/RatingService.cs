using AutoMapper;
using DataConnection.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModel.Rating;
namespace Service.RatingService
{
    public interface IRatingService
    {
        public Task<IActionResult> CreateRating(RatingCreateViewModel rcvm);
        public Task<bool> AcceptRating(Guid id);
        public Task<IActionResult> GetRatingAdvise(Guid adviseId);
        public Task<bool> CheckRating(string adviseId, string userId);
    }
    public class RatingService : IRatingService
    {
        private IMapper _mapper;
        private ExpertConnectionContext _context;
        public RatingService(ExpertConnectionContext expertConnection, IMapper mapper)
        {
            _mapper = mapper;
            _context = expertConnection;
        }
        public async Task<IActionResult> CreateRating(RatingCreateViewModel rcvm)
        {

            if (rcvm.Ratings > 0 && rcvm.Ratings <= 5)
            {
                Rating rating = new Rating
                {
                    Id = Guid.NewGuid().ToString(),
                    Ratings = rcvm.Ratings,
                    Comment = rcvm.Comment,
                    UserId = rcvm.UserId,
                    AdviseId = rcvm.AdviseId,
                    IsActive = false
                };
                await _context.Ratings.AddAsync(rating);
                return await _context.SaveChangesAsync() > 0 ? new JsonResult(_mapper.Map<RatingViewModel>(rating)) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }

        public async Task<bool> AcceptRating(Guid id)
        {
            var rating = await _context.Ratings.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()));
            if (rating != null)
            {
                rating.IsActive = true;
                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<bool> CheckRating(string adviseId, string userId)
        {
            return await _context.Ratings.FirstOrDefaultAsync(x => x.AdviseId.Equals(adviseId.ToString()) && x.UserId.Equals(userId.ToString())) != null ? false : true;
        }
        public async Task<IActionResult> GetRatingAdvise(Guid adviseId)
        {
            var ratings = await _context.Ratings.Where(x => x.AdviseId.Equals(adviseId.ToString())).ToListAsync();
            if (ratings != null)
            {
                double ratingNumber = 0;
                foreach (var item in ratings)
                {
                    ratingNumber += item.Ratings;
                }
                return new JsonResult(new { Ratings = ratingNumber / ratings.Count });
            }
            return null!;
        }


    }
}
