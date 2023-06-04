using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Advise
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string CategoryMappingId { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool UserConfirm { get; set; }

    public bool ExpertConfirm { get; set; }

    public bool IsDone { get; set; }

    public virtual CategoryMapping CategoryMapping { get; set; } = null!;

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual User User { get; set; } = null!;
}
