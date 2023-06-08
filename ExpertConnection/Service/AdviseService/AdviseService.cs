using AutoMapper;
using DataConnection.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModel.Advise.Request;
using ViewModel.Advise.Response;

namespace Service.AdviseService
{
    public interface IAdviseService
    {
        public Task<IActionResult> CreateAdvise(AdviseCreateViewModel acm);
        public Task<IActionResult> Confirm(Guid adviseId, Guid id);
        public Task<AdviseViewModel> GetAdviseActive(Guid id);
        public Task<int> GetAdviseCountByCategoryMappingId(Guid categoryMappingId);
    }
    public class AdviseService : IAdviseService
    {
        private IMapper _mapper;
        private ExpertConnectionContext _context;

        public AdviseService(IMapper mapper, ExpertConnectionContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> CreateAdvise(AdviseCreateViewModel acm)
        {
            Advise advise = new Advise
            {
                Id = Guid.NewGuid().ToString(),
                UserId = acm.UserId,
                CategoryMappingId = acm.CategoryMappingId,
                CreatedDate = DateTime.Now,
                UserConfirm = false,
                ExpertConfirm = false,
                IsDone = false
            };
            _context.Advises.Add(advise);
            return await _context.SaveChangesAsync() > 0 ? new JsonResult(acm) : new StatusCodeResult(500);
        }

        public async Task<IActionResult> Confirm(Guid adviseId, Guid id)
        {
            var advise = await _context.Advises.Where(x => x.Id.Equals(adviseId.ToString()) && !x.IsDone).Include(x => x.CategoryMapping).FirstOrDefaultAsync();
            if (advise != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()));
                if (user != null)
                {
                    if (advise.UserId.Equals(id.ToString()))
                    {
                        advise.UserConfirm = true;
                        if (advise.ExpertConfirm == true)
                        {
                            advise.IsDone = true;
                        }
                    }
                }
                else
                {
                    var expert = await _context.Experts.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()));
                    if (expert != null)
                    {
                        if (advise.CategoryMapping.ExpertId.Equals(id.ToString()))
                        {
                            advise.ExpertConfirm = true;
                            if (advise.UserConfirm == true)
                            {
                                advise.IsDone = true;
                            }
                        }
                    }
                }
                return await _context.SaveChangesAsync() > 0 ? new StatusCodeResult(200) : new StatusCodeResult(500);
            }
            return new StatusCodeResult(400);
        }

        public async Task<AdviseViewModel> GetAdviseActive(Guid id)
        {
            var rs = await _context.Advises.FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()) && !x.IsDone);
            return _mapper.Map<AdviseViewModel>(rs);
        }

        public async Task<int> GetAdviseCountByCategoryMappingId(Guid categoryMappingId)
        {
            return ( await _context.Advises.Where(x => x.CategoryMappingId.Equals(categoryMappingId.ToString())).ToListAsync()).Count();
        }
    }
}
