using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Rating
{
    public class RatingCreateViewModel
    {
        public string AdviseId { get; set; } = null!;

        public double Ratings { get; set; }

        public string Comment { get; set; } = null!;

        public string UserId { get; set; } = null!;
    }
}
