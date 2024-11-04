using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class PrintEventDivisionInfoViewModel
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
    public string Name { get; set; }
    public string Description { get; set; }
    public string AgeRange { get; set; }
    public string BeltRange { get; set; }
    public string GenderRange { get; set; }
    public string WeightRange { get; set; }
    public int? TournamentId { get; set; }
    public bool? StrictCriteria { get; set; }
    public int? GroupLimit { get; set; }
    public string Combinations { get; set; }
    public string RingAssignment { get; set; }
    public bool? IsCustom { get; set; }
  }
}
