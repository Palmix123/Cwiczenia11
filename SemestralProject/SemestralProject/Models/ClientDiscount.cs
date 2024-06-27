using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class ClientDiscount
{
    public int IdDiscount { get; set; }

    public decimal Value { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
