using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class Position
{
    public int Id { get; set; }

    public string? PositionName { get; set; }

    public virtual ICollection<Official> Officials { get; set; } = new List<Official>();
}
