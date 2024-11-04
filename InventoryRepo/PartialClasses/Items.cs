using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryRepo.Models
{


    public interface IItem_MetadataType
    {
        //[Required(AllowEmptyStrings = false)]
        //[DisplayName("Items Name")]
        //string Name { get; set; }

        //[Range(typeof(Decimal), "0", "9999999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        //[DisplayName("Sale Price")]
        //decimal SalePrice { get; set; }

        //[Range(typeof(Decimal), "0", "9999999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        //[DisplayName("Purchase Price")]
        //decimal PurchasePrice { get; set; }

        //[DisplayName("Per Pack")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value more than 0")]
        //Nullable<int> PerPack { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        Nullable<System.DateTime> ReceivingDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        Nullable<System.DateTime> WarrantyExpiryDate { get; set; }

    }

    [MetadataType(typeof(IItem_MetadataType))]
    public partial class Item : IItem_MetadataType
    {
        /* Id property has already existed in the mapped class */
     
    }


}
