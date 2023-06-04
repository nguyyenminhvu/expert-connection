using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.User
{
    public class UserCreateViewModel
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public DateTime Birthday { get; set; }

        public string Address { get; set; } = null!;

        public string Introduction { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
