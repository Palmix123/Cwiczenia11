using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Payment
{
    public int IdPayment { get; set; }

    public int IdSale { get; set; }

    public DateTime? Date { get; set; }

    public decimal Value { get; set; }

    public virtual SingleSale IdSaleNavigation { get; set; } = null!;
}
