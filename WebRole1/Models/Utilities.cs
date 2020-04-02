using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Models
{
    public class Address
    {
        [Display(Name = "Address Line 1")]
        [Required]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }

        [Display(Name = "Address Line 4")]
        public string AddressLine4 { get; set; }

        public Address()
        {
            this.Countries = new List<SelectListItem>();
            this.States = new List<SelectListItem>();
            this.Cities = new List<SelectListItem>();
        }

        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SelectListItem> Cities { get; set; }

        [Display(Name = "Cities")]
        [Required]
        public int CountryId { get; set; }

        [Display(Name = "States")]
        [Required]
        public int StateId { get; set; }

        [Display(Name = "Cities")]
        [Required]
        public int CityId { get; set; }

        [Display(Name = "Postcode")]
        [Required]
        public string Postcode { get; set; }
    }

    enum TicketStatus
    {
        Unassigned = 1,
        Assigned = 2,
        Closed = 3
    }

    enum TicketSeverity
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    enum TicketType
    {
        General,
        FoodDelivery
    }
}