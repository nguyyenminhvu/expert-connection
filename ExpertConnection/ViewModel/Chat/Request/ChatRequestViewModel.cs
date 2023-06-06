

namespace ViewModel.Chat.Request
{
    public class ChatRequestViewModel
    {
        public string AdviseId { get; set; } = null!;

        public string Contents { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public virtual DataConnection.Entities.Advise Advise { get; set; } = null!;
    }
}
