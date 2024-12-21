using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class GarbageCollectionDatum
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Day { get; set; }

    public int? FirstTrip { get; set; }

    public int? SecondTrip { get; set; }

    public int? Cartons { get; set; }

    public int? Plastics { get; set; }

    public int? Can { get; set; }
}
