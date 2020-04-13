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

            this.searchCitiesListOne = new List<SelectListItem>();
            this.searchCitiesListTwo = new List<SelectListItem>();
            this.searchCitiesListThree = new List<SelectListItem>();
            this.searchCitiesListFour = new List<SelectListItem>();
        }

        public List<SelectListItem> Countries { get; set; }

        public List<SelectListItem> States { get; set; }

        public List<SelectListItem> Cities { get; set; }

        [Display(Name = "Country")]
        [Required]
        public int CountryId { get; set; }

        [Display(Name = "State or County")]
        [Required]
        public int StateId { get; set; }

        [Display(Name = "City")]
        [Required]
        public int CityId { get; set; }

        [Display(Name = "Postcode or Zipcode")]
        [Required]
        public string Postcode { get; set; }

        [Display(Name = "volunteer area 1")]
        [Required]
        public int VolunteerAreaOneId { get; set; }
        public List<SelectListItem> searchCitiesListOne { get; set; }

        [Display(Name = "volunteer area 2")]
        public int? VolunteerAreaTwoId { get; set; }
        public List<SelectListItem> searchCitiesListTwo { get; set; }

        [Display(Name = "volunteer area 3")]
        public int? VolunteerAreaThreeId { get; set; }
        public List<SelectListItem> searchCitiesListThree { get; set; }


        [Display(Name = "volunteer area 4")]
        public int? VolunteerAreaFourId { get; set; }
        public List<SelectListItem> searchCitiesListFour { get; set; }
    }

    public class ErrorClass
    {
        public static void LogError(string userId, string errorType, string errorMessage)
        {
            using (var db = new VolunteerNetworkEntities())
            {
                ErrorLog newError = new ErrorLog()
                {
                    UserId = userId,
                    ErrorType = errorType,
                    ErrorMessage = errorMessage
                };

                db.ErrorLogs.Add(newError);
                db.SaveChanges();
            }
        }
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

    enum ErrorMessageType
    {
        Exception = 1,
        Warning = 2,
        Message = 3
    }
}