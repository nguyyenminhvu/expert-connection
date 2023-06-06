using DataConnection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Advise.Response
{
    public class AdviseViewModel
    {
        public string Id { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string CategoryMappingId { get; set; } = null!;

        public virtual DataConnection.Entities.CategoryMapping CategoryMapping { get; set; } = null!;

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        public virtual DataConnection.Entities.User User { get; set; } = null!;
    }
}
