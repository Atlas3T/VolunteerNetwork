using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class ShopperModel
    {
        [Display(Name = "Forename")]
        public string firstName { get; set; }

        [Display(Name = "Surname")]
        public string surname { get; set; }

        [Display(Name = "Address")]
        public Address address { get; set; }
    }
}