using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Discount
{
    public int IdDiscount { get; set; }

    public string Name { get; set; } = null!;

    public decimal Value { get; set; }

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Software> IdSoftwares { get; set; } = new List<Software>();
}
