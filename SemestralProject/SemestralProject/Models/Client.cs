using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public int? IdDiscount { get; set; }

    public string Adress { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string IsDeleted { get; set; } = null!;

    public virtual Company? Company { get; set; }

    public virtual ClientDiscount? IdDiscountNavigation { get; set; }

    public virtual Person? Person { get; set; }

    public virtual ICollection<SingleSale> SingleSales { get; set; } = new List<SingleSale>();

    public virtual ICollection<SubscriptionSale> SubscriptionSales { get; set; } = new List<SubscriptionSale>();
}
