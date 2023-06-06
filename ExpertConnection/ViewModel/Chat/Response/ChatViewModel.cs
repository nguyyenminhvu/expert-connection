using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Chat.Response
{
    public class ChatViewModel
    {
        public string Id { get; set; } = null!;

        public string AdviseId { get; set; } = null!;

        public string FromAcc { get; set; } = null!;

        public string ToAcc { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? ImageUrl { get; set; }

        public string Contents { get; set; } = null!;


    }
}
