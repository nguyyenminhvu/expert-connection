using AdminService.HashService;
using DataConnection.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmployeeService
{
    public interface IEmployeeService
    {
        Task<bool> CreateEmployee(string accId, string fullname);
        Task<bool> Accept(Guid Id);
    }
    public class EmployeeService : IEmployeeService
    {

        private ExpertConnectionContext _context;
        private IHashService _hashService;

        public EmployeeService(ExpertConnectionContext expertConnection, IHashService hashService)
        {
            _context = expertConnection;
            _hashService = hashService;
        }

        public async Task<bool> CreateEmployee(string accId, string fullname)
        {
            try
            {
                Employee employee = new Employee
                {
                    AccId = accId,
                    Fullname = fullname,
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                };
                await _context.Employees.AddAsync(employee);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;

            }
        }

        public async Task<bool> Accept(Guid Id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(Id.ToString()));
            if (user != null && user.UserConfirmed == false)
            {
                user.UserConfirmed = true;
                return await _context.SaveChangesAsync() > 0 ? true : false;
            }
            else
            {
                var expert = await _context.Experts.FirstOrDefaultAsync(x => x.Id.Equals(Id.ToString()));
                if (expert != null && expert.ExpertConfirmed == false)
                {
                    expert.ExpertConfirmed = true;
                    return await _context.SaveChangesAsync() > 0 ? true : false;
                }
            }
            return false;
        }

    }
}
