using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class EventDivisionInfoViewModel
  {
    public int Id { get; set; }
    public int? TournamentEventId { get; set; }
    public int FromAge { get; set; }
    public int ToAge { get; set; }
    public int FromWeight { get; set; }
    public int ToWeight { get; set; }
    public int FromBelt { get; set; }
    public int ToBelt { get; set; }
    public int Gender { get; set; }
  }
}
