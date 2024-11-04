using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class TournamentRegisteration
  {
    public int CompetitorId { get; set; }
    public List<int> EventIds { get; set; }
  }
}
