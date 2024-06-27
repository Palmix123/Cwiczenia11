using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Company
{
    public int IdClient { get; set; }

    public string Name { get; set; } = null!;

    public string Krs { get; set; } = null!;

    public virtual Client IdClientNavigation { get; set; } = null!;
}
