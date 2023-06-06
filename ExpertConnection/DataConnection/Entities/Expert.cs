using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Expert
{
    public string Id { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string CertificateLink { get; set; } = null!;

    public string Introduction { get; set; } = null!;

    public double SummaryRating { get; set; }

    public string WorkRole { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public bool ExpertConfirmed { get; set; }

    public string AccId { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<CategoryMapping> CategoryMappings { get; set; } = new List<CategoryMapping>();
}
