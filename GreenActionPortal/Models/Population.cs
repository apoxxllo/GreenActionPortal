using System;
using System.Collections.Generic;

namespace GreenActionPortal.Models;

public partial class Population
{
    public int Id { get; set; }

    public int? PopulationCensus { get; set; }

    public DateTime? Date { get; set; }
}
