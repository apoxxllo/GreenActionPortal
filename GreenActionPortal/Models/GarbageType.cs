using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class GarbageType
{
    public int Id { get; set; }

    public string? GargabeType { get; set; }

    public virtual ICollection<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; } = new List<GarbageCollectionSchedule>();
}
