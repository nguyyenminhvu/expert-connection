using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Chat
{
    public string Id { get; set; } = null!;

    public string AdviseId { get; set; } = null!;

    public string FromAcc { get; set; } = null!;

    public string ToAcc { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Advise Advise { get; set; } = null!;
}
