using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.CategoryMapping.Request
{
    public class CategoryMappingSearch
    {
        public Guid? CategoryId { get; set; }
        public Guid? ExpertId { get; set; }
        public double? FromPrice { get; set; }
        public double? ToPrice { get; set; }
        public double? FromExperienceYear { get; set; }
        public double? ToExperienceYear { get; set; }
        public double? FromSummaryRating { get; set; }
        public double? ToSummaryRating { get; set; }
    }
}
