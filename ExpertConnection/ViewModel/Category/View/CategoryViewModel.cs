using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.CategoryMapping.View;

namespace ViewModel.Category.View
{
    public class CategoryViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public virtual ICollection<CategoryMappingViewModel> CategoryMappings { get; set; } = new List<CategoryMappingViewModel>();
    }
}
