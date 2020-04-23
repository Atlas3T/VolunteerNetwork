using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class TicketListModel
    {
        [Display(Name = "Tickets")]
        public List<Ticket> tickets { get; set; }

        public int VolunteerCount { get; set; }
    }
}