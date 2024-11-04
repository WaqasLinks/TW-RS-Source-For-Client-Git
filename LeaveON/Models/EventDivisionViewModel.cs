using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class EventDivisionViewModel
  {
    public int Id { get; set; }
    public string TournamentName { get; set; }
    public string TournamentEventName { get; set; }
    public string Name { get; set; }
    public string AgeRange { get; set; }
    public string BeltRange { get; set; }
    public string GenderRange { get; set; }
    public string WeightRange { get; set; }
    public int TotalCompetitors { get; set; }
    public int? TournamentEventId { get; set; }
    public bool? StrictCriteria { get; set; }
    public bool? IsCustom { get; set; }
    public int? GroupLimit { get; set; }
    public string Combinations { get; set; }
    public string CreatedBy { get; set; }
  }
}
