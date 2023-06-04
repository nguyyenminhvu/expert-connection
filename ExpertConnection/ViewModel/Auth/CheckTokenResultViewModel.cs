using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Auth
{
    public class CheckTokenResultViewModel
    {
        public string AccId { get; set; }
        public string Username { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
