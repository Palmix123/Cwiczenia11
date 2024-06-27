using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class SingleSale
{
    public int IdSale { get; set; }

    public int IdClient { get; set; }

    public int IdSoftware { get; set; }

    public string SoftwareVersion { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpireDate { get; set; }

    public string IsSigned { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int ExtraYearsForUpdates { get; set; }

    public string UpdatesInfo { get; set; } = null!;

    public int NumberOfRates { get; set; }

    public DateTime EndOfSoftware { get; set; }

    public virtual Client IdClientNavigation { get; set; } = null!;

    public virtual Software IdSoftwareNavigation { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
