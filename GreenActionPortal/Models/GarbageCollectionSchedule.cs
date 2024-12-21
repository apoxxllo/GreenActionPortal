using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class GarbageCollectionSchedule
{
    public int Id { get; set; }

    public int? GarbageTypeId { get; set; }

    public string? DayOfTheWeek { get; set; }

    public virtual GarbageType? GarbageType { get; set; }
}
