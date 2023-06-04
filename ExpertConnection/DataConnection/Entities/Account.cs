using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Expert> Experts { get; set; } = new List<Expert>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
