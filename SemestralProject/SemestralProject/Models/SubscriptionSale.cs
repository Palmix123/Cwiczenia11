using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class SubscriptionSale
{
    public int IdSubSale { get; set; }

    public int IdSoftware { get; set; }

    public int IdClient { get; set; }

    public int Installment { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Software IdSoftwareNavigation { get; set; } = null!;
}
