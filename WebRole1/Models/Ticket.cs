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
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public List<SelectListItem> TypeList
        {
            get
            {
                List<SelectListItem> myList = new List<SelectListItem>();
                var data = new[]{
                                     new SelectListItem{ Value="1",Text="Food Delivery"},
                                     new SelectListItem{ Value="2",Text="General"}
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
                                     new SelectListItem{ Value="1",Text="Unassiged"},
                                     new SelectListItem{ Value="2",Text="Assigned"},
                                     new SelectListItem{ Value="3",Text="Closed"}
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
                                     new SelectListItem{ Value="1",Text="Low"},
                                     new SelectListItem{ Value="2",Text="Medium"},
                                     new SelectListItem{ Value="3",Text="High"}
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
    }
}