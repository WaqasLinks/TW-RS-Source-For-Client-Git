using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class RoundRobinEliminationVM
  {
    public int Round { get; set; }
    public string Team1 { get; set; }
    public string Team1School { get; set; }
    public string Team2 { get; set; }
    public string Team2School { get; set; }
  }
}
