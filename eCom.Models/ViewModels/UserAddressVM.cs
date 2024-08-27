using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCom.Models.ViewModels
{
    public class UserAddressVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string NearestLandMark { get; set; }
        public string AdditionalInfo { get; set; }
        public int CityId { get; set; }
        public bool IsDefault {  get; set; }
    }
}
