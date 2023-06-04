using AdminService.HashService;
using DataConnection.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.AccountService
{
    public interface IAccountService
    {
        Task<bool> CreateAccountAsync(string username, string password, string role, string accId);
    }

    public class AccountService:IAccountService
    {
        private IHashService _hashSerrvice;
        private ExpertConnectionContext _context;
        private readonly string Key = "k1H25jDl1cxKMWOz1tte5I961K5oivZgJ4xdYF1hBAt85Tt1jqGwwpFvqmbToCBL";
        public AccountService(ExpertConnectionContext expertConnection,IHashService hashService)
        {
            _hashSerrvice = hashService;
            _context = expertConnection;
        }

        public async Task<bool> CreateAccountAsync(string username, string password, string role, string accId)
        {
            var acc = await _context.Accounts.Where(x => x.Username.Equals(username)).FirstOrDefaultAsync();
            if (acc==null)
            {
                var roledb = await _context.Roles.Where(q=> q.IsActive && q.Name == role).FirstOrDefaultAsync();
                if (roledb != null)
                {
                    Account account = new Account
                    {
                        Username = username,
                        Password = _hashSerrvice.SHA256(Key + password),
                        Id = accId,
                        Role = roledb.Id,
                         IsActive = true
                    };
                    await _context.Accounts.AddAsync(account);
                    return await _context.SaveChangesAsync() > 0;
                }
                else return false;
            }
            return false;
        }
    }
}
