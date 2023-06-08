using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Rating
{
    public string Id { get; set; } = null!;

    public string AdviseId { get; set; } = null!;

    public double Ratings { get; set; }

    public string Comment { get; set; } = null!;

    public bool IsActive { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Advise Advise { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
