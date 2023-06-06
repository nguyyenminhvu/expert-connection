using AdminService.HashService;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataConnection.Entities;
using Microsoft.EntityFrameworkCore;
using Service.AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Expert.Request;
using ViewModel.Expert;
using Microsoft.AspNetCore.Mvc;

namespace Service.ExpertService
{
    public interface IExpertService
    {
        public Task<List<ExpertViewModel>> GetAll();
        public Task<bool> CreateExpert(ExpertCreateViewModel expertCreate);
        public Task<Expert> CheckExist(string accId);
        public Task<IActionResult> GetExpert(Guid expertId);
        public Task<bool> UpdateExpert(Guid expertId, ExpertUpdate expertUpdate);
        public Task<bool> Accept(Guid Id);
        public Task<ExpertViewModel> GetExpertByAccId(string accId);
    }

    public class ExpertService : IExpertService
    {
        private IHashService _hashService;
        private IMapper _mapper;
        private ExpertConnectionContext _context;
        private IAuthService _authService;
        private readonly string Key = "k1H25jDl1cxKMWOz1tte5I961K5oivZgJ4xdYF1hBAt85Tt1jqGwwpFvqmbToCBL";

        public ExpertService(ExpertConnectionContext expertConnection, IAuthService authService, IMapper mapper, IHashService hashService)
        {
            _hashService = hashService;
            _mapper = mapper;
            _context = expertConnection;
            _authService = authService;
        }
        public async Task<List<ExpertViewModel>> GetAll()
        {
            return await _context.Experts.Include(x => x.Acc).Where(x => x.Acc.IsActive).ProjectTo<ExpertViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
        public async Task<bool> CreateExpert(ExpertCreateViewModel expertCreate)
        {
            var checkAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.Username.Equals(expertCreate.Username));
            if (checkAccount != null)
            {
                return false;
            }
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name.Equals("Expert"));
            Account account = new Account
            {
                Id = Guid.NewGuid().ToString(),
                Username = expertCreate.Username,
                Password = _hashService.SHA256(Key + expertCreate.Password),
                Role = role.Id,
                IsActive = true
            };
            _context.Accounts.Add(account);

            Expert expert = new Expert
            {
                Id = Guid.NewGuid().ToString(),
                Fullname = expertCreate.Username,
                CertificateLink = expertCreate.CertificateLink,
                Introduction = expertCreate.Introduction,
                SummaryRating = expertCreate.SummaryRating,
                WorkRole = expertCreate.WorkRole,
                Email = expertCreate.Email,
                Phone = expertCreate.Phone,
                EmailConfirmed = false,
                ExpertConfirmed = false,
                AccId = account.Id,
                Acc = account
            };
            _context.Experts.Add(expert);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Expert> CheckExist(string accId)
        {
            return await _context.Experts.FirstOrDefaultAsync(x => x.AccId.Equals(accId));
        }
        public async Task<bool> UpdateExpert(Guid expertId, ExpertUpdate expertUpdate)
        {
            var expert = await _context.Experts.Where(x => x.Id.Equals(expertId.ToString())).Include(x => x.CategoryMappings).FirstOrDefaultAsync();
            if (expert != null)
            {
                expert.Fullname = expertUpdate.Fullname ?? expert.Fullname;
                expert.CertificateLink = expertUpdate.CertificateLink ?? expert.CertificateLink;
                expert.Introduction = expertUpdate.Introduction ?? expert.Introduction;
                expert.SummaryRating = expertUpdate.SummaryRating ?? expert.SummaryRating;
                expert.WorkRole = expertUpdate.WorkRole ?? expert.WorkRole;
                expert.Email = expertUpdate.Email ?? expert.Email;
                expert.Phone = expertUpdate.Phone ?? expert.Phone;
                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<IActionResult> GetExpert(Guid expertId)
        {
            return new JsonResult(await _context.Experts.ProjectTo<ExpertViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id.Equals(expertId.ToString())));
        }
        public async Task<bool> Accept(Guid Id)
        {
            var expert = await _context.Experts.FirstOrDefaultAsync(x => x.Id.Equals(Id.ToString()));
            if (expert != null && expert.ExpertConfirmed == false)
            {
                expert.ExpertConfirmed = true;
                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<ExpertViewModel> GetExpertByAccId(string accId)
        {
            var rs = await _context.Experts.FirstOrDefaultAsync(x => x.AccId.Equals(accId));
            return _mapper.Map<ExpertViewModel>(rs);
        }
    }
}
