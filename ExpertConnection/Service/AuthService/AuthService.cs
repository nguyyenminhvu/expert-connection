using AdminService.HashService;
using Azure.Core;
using DataConnection.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Auth;

namespace Service.AuthService
{
    public interface IAuthService
    {
        Task<CheckTokenResultViewModel> CheckTokenAsync(string token);
        Task<LoginResultViewModel> LoginAsync(LoginViewModel loginView, string roleName);
    }

    public class AuthService : IAuthService
    {
        private IHashService _hashService;
        private ExpertConnectionContext _context;
        private readonly string Key = "k1H25jDl1cxKMWOz1tte5I961K5oivZgJ4xdYF1hBAt85Tt1jqGwwpFvqmbToCBL";

        public AuthService(ExpertConnectionContext expertConnection, IHashService hashService)
        {
            _hashService = hashService;
            _context = expertConnection;
        }

        public async Task<CheckTokenResultViewModel> CheckTokenAsync(string token)
        {
            var checkToken = await _context.Tokens.Where(x => x.IsActive && x.AccessToken == token && (DateTime.Now.Day - x.CreatedDate.Day) <= 2).Include(x => x.Acc).FirstOrDefaultAsync();
            if (checkToken != null)
            {
                var role = await _context.Roles.Where(x => x.Id.Equals(checkToken.Acc.Role)).FirstOrDefaultAsync();
                return new CheckTokenResultViewModel
                {
                    AccId = checkToken.AccId,
                    RoleId = checkToken.Acc.Id,
                    RoleName = role.Name,
                    Username = checkToken.Acc.Username
                };
            }
            return null;
        }

        public async Task<LoginResultViewModel> LoginAsync(LoginViewModel loginView, string roleName)
        {
            var account = await _context.Accounts.Where(x => x.IsActive && x.Username.Equals(loginView.Username) && x.Password.Equals(_hashService.SHA256(Key + loginView.Password))).FirstOrDefaultAsync();
            var role = await _context.Roles.Where(x => x.Name.Equals(roleName)).FirstOrDefaultAsync();
            if (account != null && role != null)
            {
                if (!role.Id.Equals(account.Role))
                {
                    return null;
                }
                var tokenDb = await _context.Tokens.Where(x => x.IsActive && x.AccId.Equals(account.Id)).FirstOrDefaultAsync();
                var newTokenAccess = Guid.NewGuid().ToString();

                if (tokenDb != null)
                {
                    tokenDb.AccessToken = newTokenAccess;
                    tokenDb.CreatedDate = DateTime.Now;
                    tokenDb.IsActive = true;
                    await _context.SaveChangesAsync();
                    return new LoginResultViewModel { AccessToken = newTokenAccess };
                }
                else
                {
                    Token newToken = new Token
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccessToken = newTokenAccess,
                        AccId = account.Id.ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                    };
                    await _context.Tokens.AddAsync(newToken);
                    await _context.SaveChangesAsync();
                    return new LoginResultViewModel { AccessToken = newTokenAccess };
                }
            }
            return null;
        }

    }
}
