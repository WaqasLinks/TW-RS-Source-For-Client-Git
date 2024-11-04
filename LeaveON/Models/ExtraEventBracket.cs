using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class ExtraEventBracket
  {
    public int EventId { get; set; }
    public int BracketId { get; set; }
    public string[] EventDivisions { get; set; }
  }
}
