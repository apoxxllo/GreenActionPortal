using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? PhotoPath { get; set; }

    public DateTime? Date { get; set; }
}
