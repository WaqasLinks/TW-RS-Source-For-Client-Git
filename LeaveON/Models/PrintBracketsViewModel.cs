using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourneyRepo.Models;

namespace LeaveON.Models
{
  public class PrintBracketsViewModel
  {
    public List<Competitor> Competitors { get; set; }
    public List<EventDivision> EventDivisions { get; set; }
    public List<DivisionGroup> DivisionGroups { get; set; }
    public string Age { get; set; }
    public string Belt { get; set; }
    public string Weight { get; set; }
    public string Gender { get; set; }
    public string Event { get; set; }
    public int id { get; set; }
    public string Combination { get; set; }
    public int Group { get; set; }
    public int TotalGroup { get; set; }
    public decimal EventId { get; set; }
    public bool IsCustom { get; set; }
    public bool IsParticipantLimited { get; set; }
  }
}
