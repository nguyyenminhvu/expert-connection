using DataConnection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Chat.Request
{
    public class ChatInsertViewModel
    {
        public string AdviseId { get; set; } = null!;

        public string FromAcc { get; set; } = null!;

        public string ToAcc { get; set; } = null!;

        public string Contents { get; set; } = null!;

        public string? ImageUrl { get; set; }

    }
}
