using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class ActivityLogViewModel
  {
    public string Barcode { get; set; }
    public string Serial { get; set; }
    public string ActivityByUser { get; set; }
    public string DeviceDesc { get; set; }

    
    public DateTime ActivityDateTime { get; set; }
    public string ActivityDesc { get; set; }
  }
}
