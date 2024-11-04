using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class CompetitorIndexViewModel
  {
    public int Id { get; set; }
    public Nullable<decimal> Serial { get; set; }
    public string Name { get; set; }
    public Nullable<decimal> Age { get; set; }
    public Nullable<decimal> Weight { get; set; }
    public Nullable<bool> Gender { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Zip { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string School { get; set; }
    public string Belt { get; set; }
    public Dictionary<decimal,string> Event { get; set; }
    public System.DateTime RegistrationDate { get; set; }
    public string RegistrationDates { get; set; }
  }
}
