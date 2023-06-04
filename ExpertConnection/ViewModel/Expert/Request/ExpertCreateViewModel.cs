using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Expert.Request
{
    public class ExpertCreateViewModel
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public string CertificateLink { get; set; } = null!;

        public string Introduction { get; set; } = null!;

        public double SummaryRating { get; set; }

        public string WorkRole { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;
    }
}
