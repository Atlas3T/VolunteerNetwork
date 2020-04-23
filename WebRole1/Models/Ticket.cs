using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole1.Models
{
    public class Ticket
    {
        [Display(Name = "Ticket number")]
        public int TicketNumber { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        public List<SelectListItem> TypeList
        {
            get
            {
                List<SelectListItem> myList = new List<SelectListItem>();
                var data = new[]{
                                     new SelectListItem{ Value=((int)TicketType.FoodDelivery).ToString(),Text="Food Delivery"}, 
                                     new SelectListItem{ Value=((int)TicketType.DogWalking).ToString(),Text="Dog Walking"},
                                     new SelectListItem{ Value=((int)TicketType.General).ToString(),Text="General"}
                                };
                myList = data.ToList();
                return myList;
            }
        }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Display(Name = "Raised By")]
        public int RaisedBy { get; set; }

        [Display(Name = "Closed By")]
        public int? ClosedBy { get; set; }

        public List<SelectListItem> StatusList
        {
            get
            {
                List<SelectListItem> myList = new List<SelectListItem>();
                var data = new[]{
                                     new SelectListItem{ Value=((int)TicketStatus.Unassigned).ToString(),Text="Unassiged"},
                                     new SelectListItem{ Value=((int)TicketStatus.Assigned).ToString(),Text="Assigned"},
                                     new SelectListItem{ Value=((int)TicketStatus.Closed).ToString(),Text="Closed"}
                                };
                myList = data.ToList();
                return myList;
            }
        }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Date Raised")]
        public DateTime DateRaised { get; set; }

        [Display(Name = "Date Closed")]
        public DateTime? DateClosed { get; set; }

        [Display(Name = "AssignedTo")]
        public int? AssignedTo { get; set; }

        public List<SelectListItem> SeverityList {
            get {

                List<SelectListItem> myList = new List<SelectListItem>();
                var data = new[]{
                                     new SelectListItem{ Value=((int)TicketSeverity.Low).ToString(),Text="Low"},
                                     new SelectListItem{ Value=((int)TicketSeverity.Medium).ToString(),Text="Medium"},
                                     new SelectListItem{ Value=((int)TicketSeverity.High).ToString(),Text="High"}
                                };
                myList = data.ToList();
                return myList;
            }
        }

        [Display(Name = "Severity")]
        public int Severity { get; set; }

        [Display(Name = "Status")]
        public string StatusText { get; set; }

        [Display(Name = "Severity")]
        public string SeverityText { get; set; }

        [Display(Name = "Type")]
        public string TypeText { get; set; }

        [Display(Name = "Phone (optional)")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email (optional)")]
        public string Email { get; set; }
    }
}