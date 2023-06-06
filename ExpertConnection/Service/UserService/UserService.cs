using DataConnection.Entities;
using Service.AuthService;
using Microsoft.EntityFrameworkCore;
using ViewModel.User;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using AdminService.HashService;
using Microsoft.AspNetCore.Mvc;

namespace Service.UserService
{
    public interface IUserService
    {
        Task<bool> CreateUser(UserCreateViewModel userCreate);
        Task<List<UserViewModel>> GetAll();
        public Task<bool> Accept(Guid Id);
        public Task<bool> CheckExist(string accId);
        public Task<UserViewModel> GetUserByAccId(string accId);
    }
    public class UserService : IUserService
    {
        private IHashService _hashService;
        private IMapper _mapper;
        private ExpertConnectionContext _context;
        private readonly string Key = "k1H25jDl1cxKMWOz1tte5I961K5oivZgJ4xdYF1hBAt85Tt1jqGwwpFvqmbToCBL";


        public UserService(ExpertConnectionContext expertConnection, IMapper mapper, IHashService hashService)
        {
            _hashService = hashService;
            _mapper = mapper;
            _context = expertConnection;
        }

        public async Task<bool> CreateUser(UserCreateViewModel userCreate)
        {
            var user = await _context.Accounts.Where(x => x.Username.Equals(userCreate.Username)).FirstOrDefaultAsync();
            if (user == null)
            {
                var role = await _context.Roles.Where(x => x.Name.Equals("User")).FirstOrDefaultAsync();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = userCreate.Username,
                    Password = _hashService.SHA256(Key + userCreate.Password),
                    Role = role.Id,
                    IsActive = true
                };
                await _context.Accounts.AddAsync(account);

                User newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    AccId = account.Id,
                    Fullname = userCreate.Fullname,
                    Birthday = userCreate.Birthday,
                    Address = userCreate.Address,
                    Introduction = userCreate.Introduction,
                    PhoneNumber = userCreate.PhoneNumber,
                    Email = userCreate.Email,
                    EmailActivated = false,
                    UserConfirmed = false,
                    Acc = account
                };
                await _context.Users.AddAsync(newUser);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<List<UserViewModel>> GetAll()
        {
            return await _context.Users.ProjectTo<UserViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
        public async Task<bool> Accept(Guid Id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(Id.ToString()));
            if (user != null && user.UserConfirmed == false)
            {
                user.UserConfirmed = true;
                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            return false;
        }
        public async Task<UserViewModel> GetUserByAccId(string accId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.AccId.Equals(accId));
            return _mapper.Map<UserViewModel>(user);
        }
        public async Task<bool> CheckExist(string accId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.AccId.Equals(accId));
            return user != null;
        }
    }
}
