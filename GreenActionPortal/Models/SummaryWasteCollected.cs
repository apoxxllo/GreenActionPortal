using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class SummaryWasteCollected
{
    public int Id { get; set; }

    public string? Month { get; set; }

    public int? YearId { get; set; }

    public int? Cartons { get; set; }

    public int? Plastics { get; set; }

    public int? Can { get; set; }
}
