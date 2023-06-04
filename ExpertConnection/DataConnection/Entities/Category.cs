using System;
using System.Collections.Generic;

namespace DataConnection.Entities;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<CategoryMapping> CategoryMappings { get; set; } = new List<CategoryMapping>();
}
