using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string AccId { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string Address { get; set; } = null!;

    public string Introduction { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool EmailActivated { get; set; }

    public bool UserConfirmed { get; set; }

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Advise> Advises { get; set; } = new List<Advise>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
