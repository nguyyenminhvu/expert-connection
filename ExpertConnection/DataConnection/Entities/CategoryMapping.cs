using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class CategoryMapping
{
    public string Id { get; set; } = null!;

    public string ExpertId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public double Price { get; set; }

    public double ExperienceYear { get; set; }

    public double SummaryRating { get; set; }

    public bool IsActive { get; set; }

    public bool IsConfirmed { get; set; }

    public string Introduction { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Advise> Advises { get; set; } = new List<Advise>();

    public virtual Category Category { get; set; } = null!;

    public virtual Expert Expert { get; set; } = null!;
}
