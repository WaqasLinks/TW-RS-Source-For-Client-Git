using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class CompetitorChildViewModel
  {
    public string CoachName { get; set; }
    public string SchoolName { get; set; }
    public int TotalCompititor { get; set; }

    public List<CompetitorChild> competitors { get; set; }
  }


  public class CompetitorChild
  {
    public int Id { get; set; }
    public string QRID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public bool Gender { get; set; }
    public string State { get; set; }
    public string Email { get; set; }
    public bool IsVerify { get; set; }
    public string School { get; set; }
    public string Events { get; set; }
    public string Belt { get; set; }
    public bool IsCheckin { get; set; }
    public string Remarks { get; set; }



  }


}
