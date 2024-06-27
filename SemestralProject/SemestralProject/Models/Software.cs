using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Software
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string SoftwareVersion { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<SingleSale> SingleSales { get; set; } = new List<SingleSale>();

    public virtual ICollection<SubscriptionSale> SubscriptionSales { get; set; } = new List<SubscriptionSale>();

    public virtual ICollection<Discount> IdDiscounts { get; set; } = new List<Discount>();
}
