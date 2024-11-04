using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TourneyRepo.Models
{
  public class Dashboard
  {

    //[DisplayName("Emp.No.")]
    //public int EmployeeNumber { get; set; }
    //public string Country { get; set; }
    //[DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
    //public decimal MyTakenLeaves { get; set; }
    //public decimal MyBalanceLeaves { get; set; }

    [DisplayName("Competitors")]
    public int totalCompetitors { get; set; }

    [DisplayName("Tournaments")]
    public int totalTournaments { get; set; }

    [DisplayName("Events")]
    public int totalEvents { get; set; }

    [DisplayName("Events")]
    public int totalDivisions { get; set; }

    [DisplayName("Users")]
    public int totalUsers { get; set; }

  }
}
