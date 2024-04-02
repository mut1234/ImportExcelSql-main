using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExcelSql.Models
{
    public class StaffInfoViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<StaffInfoViewModel> StaffList { get; set; }
        public StaffInfoViewModel()
        {
            StaffList = new List<StaffInfoViewModel>();
        }
    }

    public class CarViewModel
    {
        public string carName { get; set; }

        public string doorNumber { get; set; }

        public string bodyStyle { get; set; }

        public string engineLocation { get; set; }

        public string numberOfCylinders { get; set; }

        public int horsePower { get; set; }

        public int price { get; set; }
    }

}
