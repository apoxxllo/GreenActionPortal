using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public string? ProfilePicPath { get; set; }

    public virtual ICollection<Official> Officials { get; set; } = new List<Official>();
}
