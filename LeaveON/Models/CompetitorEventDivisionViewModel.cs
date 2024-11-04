using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyRepo.Models;

namespace LeaveON.Models
{
  public class CompetitorEventDivisionViewModel
  {
    public List<decimal?> SelectedDivisionIds { get; set; }
    public List<SelectListItem> Divisions { get; set; }
    public List<TournamentEvent> TournamentEvents { get; set; }
  }
}
