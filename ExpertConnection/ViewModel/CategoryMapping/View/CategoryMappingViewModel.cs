using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.CategoryMapping.View
{
    public class CategoryMappingViewModel
    {
        public string Id { get; set; } = null!;

        public string ExpertId { get; set; } = null!;

        public string CategoryId { get; set; } = null!;

        public double Price { get; set; }

        public double ExperienceYear { get; set; }

        public double SummaryRating { get; set; }

        public string Introduction { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
