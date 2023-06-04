using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Employee
{
    public string Id { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string AccId { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
