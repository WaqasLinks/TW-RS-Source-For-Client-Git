using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Models
{
  public class CompetitorDivisionsVM
  {
    public int EventDivisionId { get; set; }
    public int? TournamentEventId { get; set; }
    public Competitor Competitor { get; set; }
    public List<int?> SelectedDivisionIds { get; set; }
    public List<SelectListItem> Divisions { get; set; }
  }
}
