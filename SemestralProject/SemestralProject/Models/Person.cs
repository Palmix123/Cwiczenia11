using System;
using System.Collections.Generic;

namespace SemestralProject.Models;

public partial class Person
{
    public int IdClient { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Pesel { get; set; } = null!;

    public virtual Client IdClientNavigation { get; set; } = null!;
}
