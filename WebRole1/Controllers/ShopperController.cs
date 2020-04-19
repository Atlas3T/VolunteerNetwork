using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebRole1.Models;

namespace WebRole1.Controllers
{
    [Authorize(Roles = "Shopper")]
    public class ShopperController : Controller
    {
        public static List<SelectListItem> countryList = null;
        public static List<SelectListItem> countyList = null;

        public ActionResult MyProfile()
        {
            ShopperModel shopperModel = new ShopperModel(); ;
            string strCurrentUserId = User.Identity.GetUserId();

            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    ShopperController.SetCountryList(db);
                    ShopperController.SetCountyList(db);

                    var users = from s in db.Users
                                where s.AspNetUsersId == strCurrentUserId
                                select s;

                    if (users.Count() == 0)
                    {
                        shopperModel.address = new Address
                        {
                            Countries = countryList,
                            CountryId = 235,
                            States = countyList
                        };
                    }
                    else
                    {
                        User thisUser = users.FirstOrDefault();
                        shopperModel.firstName = thisUser.Forename;
                        shopperModel.surname = thisUser.Forename;
                        shopperModel.address = new Address();

                        ShopperAddress thisAddress = (from s in db.ShopperAddresses
                                                      where s.UserId == thisUser.Id
                                                      select s).FirstOrDefault();

                        shopperModel.address.AddressLine1 = thisAddress.AddressLine1;
                        shopperModel.address.AddressLine2 = thisAddress.AddressLine2;
                        shopperModel.address.AddressLine3 = thisAddress.AddressLine3;
                        shopperModel.address.AddressLine4 = thisAddress.AddressLine4;
                        shopperModel.address.CountryId = thisAddress.Country;
                        shopperModel.address.CityId = thisAddress.Locality;
                        shopperModel.address.Postcode = thisAddress.Postcode;
                        shopperModel.address.StateId = thisAddress.Region;
                        shopperModel.address.Countries = countryList;

                        foreach (var country in db.Countries)
                        {
                            shopperModel.address.Countries.Add(new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });
                        }

                        if (shopperModel.address.Countries != null)
                        {
                            var states = (from state in db.States
                                          where state.CountryId == shopperModel.address.CountryId
                                          select state).ToList();
                            foreach (var state in states)
                            {
                                shopperModel.address.States.Add(new SelectListItem { Text = state.StateName, Value = state.StateId.ToString() });
                            }

                            if (shopperModel.address.States != null)
                            {
                                var cities = (from city in db.Cities
                                              where city.StateId == shopperModel.address.StateId
                                              select city).ToList();
                                foreach (var city in cities)
                                {
                                    shopperModel.address.Cities.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(shopperModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(ShopperModel model)
        {
            string strCurrentUserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid &&
                    model.address.StateId > 0 &&
                    model.address.CityId > 0)
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var users = from s in db.Users
                                    where s.AspNetUsersId == strCurrentUserId
                                    select s;

                        if (users.Count() == 0)
                        {
                            User newUser = new User()
                            {
                                Forename = model.firstName,
                                Surname = model.surname,
                                AspNetUsersId = strCurrentUserId
                            };

                            db.Users.Add(newUser);
                            db.SaveChanges();

                            ShopperAddress newAddress = new ShopperAddress()
                            {
                                AddressLine1 = model.address.AddressLine1,
                                AddressLine2 = model.address.AddressLine2,
                                AddressLine3 = model.address.AddressLine3,
                                AddressLine4 = model.address.AddressLine4,
                                Locality = model.address.CityId,
                                Region = model.address.StateId,
                                Postcode = model.address.Postcode,
                                Country = 235, //model.address.CountryId,
                                UserId = newUser.Id
                            };

                            db.ShopperAddresses.Add(newAddress);
                            db.SaveChanges();
                        }
                        else
                        {
                            User thisUser = users.FirstOrDefault();

                            thisUser.Forename = model.firstName;
                            thisUser.Surname = model.surname;

                            ShopperAddress thisAddress = (from s in db.ShopperAddresses
                                                          where s.UserId == thisUser.Id
                                                          select s).FirstOrDefault();

                            thisAddress.AddressLine1 = model.address.AddressLine1;
                            thisAddress.AddressLine2 = model.address.AddressLine2;
                            thisAddress.AddressLine3 = model.address.AddressLine3;
                            thisAddress.AddressLine4 = model.address.AddressLine4;
                            thisAddress.Country = 235; //model.address.CountryId;
                            thisAddress.Locality = model.address.CityId;
                            thisAddress.Postcode = model.address.Postcode;
                            thisAddress.Region = model.address.StateId;

                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        model.address.CountryId = 235;

                        foreach (var country in db.Countries)
                        {
                            model.address.Countries.Add(new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });
                        }

                        if (model.address.Countries != null)
                        {
                            var states = (from state in db.States
                                          where state.CountryId == 235
                                          //                                      where state.CountryId == model.address.CountryId
                                          select state).ToList();
                            foreach (var state in states)
                            {
                                model.address.States.Add(new SelectListItem { Text = state.StateName, Value = state.StateId.ToString() });
                            }

                            if (model.address.States != null)
                            {
                                var cities = (from city in db.Cities
                                              where city.StateId == model.address.StateId
                                              select city).ToList();
                                foreach (var city in cities)
                                {
                                    model.address.Cities.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                }
                            }
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
            }

            return RedirectToAction("Index", "Shopper");
        }

        // GET: Volunteer
        public ActionResult Index()
        {
            TicketListModel ticketListModel = new TicketListModel();
            string strCurrentUserId = User.Identity.GetUserId();

            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    

                    var users = from s in db.Users
                                where s.AspNetUsersId == strCurrentUserId
                                select s;

                    if (users.Count() == 0)
                    {
                        return RedirectToAction("MyProfile", "Shopper");
                    }
                    else
                    {
                        User thisUser = users.FirstOrDefault();

                        var supportTasks = from s in db.SupportTasks
                                           where s.RaisedBy == thisUser.Id
                                           select s;

                        ticketListModel.tickets = new List<Ticket>();
                        foreach (SupportTask s in supportTasks)
                        {
                            var ticket = new Ticket
                            {
                                TicketNumber = s.Id,
                                Title = s.Title,
                                Description = s.Description,
                                Type = s.Type,
                                RaisedBy = s.RaisedBy,
                                ClosedBy = s.ClosedBy,
                                Status = s.Status,
                                DateRaised = s.DateRaised,
                                DateClosed = s.DateClosed,
                                AssignedTo = s.AssignedTo,
                                Severity = s.severity,
                            };

                            switch (s.Status)
                            {
                                case (int)TicketStatus.Assigned:
                                    ticket.StatusText = "Assigned";
                                    break;
                                case (int)TicketStatus.Closed:
                                    ticket.StatusText = "Closed";
                                    break;
                                case (int)TicketStatus.Unassigned:
                                default:
                                    ticket.StatusText = "Unassigned";
                                    break;
                            }

                            ticketListModel.tickets.Add(ticket);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(ticketListModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TicketListModel model)
        {
            if (ModelState.IsValid)
            {
                string strCurrentUserId = User.Identity.GetUserId();

                try
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var thisUser = (from s in db.Users
                                        where s.AspNetUsersId == strCurrentUserId
                                        select s).FirstOrDefault();

                        var supportTasks = from s in db.SupportTasks
                                           where s.RaisedBy == thisUser.Id
                                           select s;

                        model.tickets = new List<Ticket>();
                        foreach (SupportTask s in supportTasks)
                        {
                            model.tickets.Add(new Ticket
                            {
                                TicketNumber = s.Id,
                                Title = s.Title,
                                Description = s.Description,
                                Type = s.Type,
                                RaisedBy = s.RaisedBy,
                                ClosedBy = s.ClosedBy,
                                Status = s.Status,
                                DateRaised = s.DateRaised,
                                DateClosed = s.DateClosed,
                                AssignedTo = s.AssignedTo,
                                Severity = s.severity
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return View(model);
        }

        public ActionResult Create()
        {
            Ticket ticketModel = new Ticket(); ;

            return View(ticketModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket model)
        {
            if (ModelState.IsValid)
            {
                string strCurrentUserId = User.Identity.GetUserId();

                try
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var users = from s in db.Users
                                    where s.AspNetUsersId == strCurrentUserId
                                    select s;

                        if (users.Count() > 0)
                        {
                            var thisUser = users.FirstOrDefault();

                            SupportTask newTicket = new SupportTask()
                            {
                                Title = model.Title,
                                Description = model.Description,
                                Type = model.Type,
                                RaisedBy = thisUser.Id,
                                Status = (int)TicketStatus.Unassigned,
                                DateRaised = DateTime.Now,
                                severity = model.Severity,
                            };

                            db.SupportTasks.Add(newTicket);
                            db.SaveChanges();

                            AuditTable newAuditEvent = new AuditTable()
                            {
                                UserId = thisUser.Id,
                                EventType = (int)AuditEventType.CreateTask,
                                TaskId = newTicket.Id
                            };

                            db.AuditTables.Add(newAuditEvent);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return RedirectToAction("Index", "Shopper");
        }

        public ActionResult Details(int id)
        {
            Ticket thisTicket = null;
            
            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    var tasks = from s in db.SupportTasks
                                where s.Id == id
                                select s;

                    if (tasks.Count() > 0)
                    {
                        var thisTask = tasks.FirstOrDefault();

                        thisTicket = new Ticket
                        {
                            Title = thisTask.Title,
                            Description = thisTask.Description,
                            TicketNumber = thisTask.Id,
                            Type = thisTask.Type,
                            RaisedBy = thisTask.RaisedBy,
                            ClosedBy = thisTask.ClosedBy,
                            Status = thisTask.Status,
                            DateRaised = thisTask.DateRaised,
                            DateClosed = thisTask.DateClosed,
                            AssignedTo = thisTask.AssignedTo,
                            Severity = thisTask.severity
                        };

                        switch (thisTask.Status)
                        {
                            case (int)TicketStatus.Assigned:
                                thisTicket.StatusText = "Assigned";
                                break;
                            case (int)TicketStatus.Closed:
                                thisTicket.StatusText = "Closed";
                                break;
                            case (int)TicketStatus.Unassigned:
                            default:
                                thisTicket.StatusText = "Unassigned";
                                break;
                        }

                        switch (thisTask.severity)
                        {
                            case (int)TicketSeverity.Medium:
                                thisTicket.SeverityText = "Medium";
                                break;
                            case (int)TicketSeverity.High:
                                thisTicket.SeverityText = "High";
                                break;
                            case (int)TicketSeverity.Low:
                            default:
                                thisTicket.SeverityText = "Low";
                                break;
                        }

                        switch (thisTask.Type)
                        {
                            case (int)TicketType.FoodDelivery:
                                thisTicket.TypeText = "Food delivery";
                                break;
                            case (int)TicketType.DogWalking:
                                thisTicket.TypeText = "Dog Walking";
                                break;
                            case (int)TicketType.General:
                            default:
                                thisTicket.TypeText = "General";
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(User.Identity.GetUserId(), ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(thisTicket);
        }

        public ActionResult Edit(int id)
        {
            Ticket thisTicket = null;
            
            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    var tasks = from s in db.SupportTasks
                                where s.Id == id
                                select s;

                    if (tasks.Count() > 0)
                    {
                        var thisTask = tasks.FirstOrDefault();

                        thisTicket = new Ticket
                        {
                            Title = thisTask.Title,
                            Description = thisTask.Description,
                            TicketNumber = thisTask.Id,
                            Type = thisTask.Type,
                            RaisedBy = thisTask.RaisedBy,
                            ClosedBy = thisTask.ClosedBy,
                            Status = thisTask.Status,
                            DateRaised = thisTask.DateRaised,
                            DateClosed = thisTask.DateClosed,
                            AssignedTo = thisTask.AssignedTo,
                            Severity = thisTask.severity
                        };

                        switch (thisTask.Status)
                        {
                            case (int)TicketStatus.Unassigned:
                                thisTicket.StatusText = "Unassigned";
                                break;
                            case (int)TicketStatus.Assigned:
                                thisTicket.StatusText = "Assigned";
                                break;
                            case (int)TicketStatus.Closed:
                                thisTicket.StatusText = "Closed";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(User.Identity.GetUserId(), ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(thisTicket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ticket model, int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var tasks = from s in db.SupportTasks
                                    where s.Id == id
                                    select s;

                        if (tasks.Count() > 0)
                        {
                            var thisTask = tasks.FirstOrDefault();

                            thisTask.Title = model.Title;
                            thisTask.Description = model.Description;
                            thisTask.severity = model.Severity;
                            thisTask.Type = model.Type;

                            AuditTable newAuditEvent = new AuditTable()
                            {
                                UserId = thisTask.User.Id,
                                EventType = (int)AuditEventType.EditTask,
                                TaskId = thisTask.Id
                            };

                            db.AuditTables.Add(newAuditEvent);
                        }

                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(User.Identity.GetUserId(), ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return RedirectToAction("Index", "Shopper");
        }

        public ActionResult Unassign(int id)
        {
            if (ModelState.IsValid)
            {
                string strCurrentUserId = User.Identity.GetUserId();

                try
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var thisTask = (from s in db.SupportTasks
                                        where s.Id == id
                                        select s).FirstOrDefault();

                        thisTask.AssignedTo = null;
                        thisTask.Status = (int)TicketStatus.Unassigned;

                        AuditTable newAuditEvent = new AuditTable()
                        {
                            UserId = thisTask.User.Id,
                            EventType = (int)AuditEventType.UnAssignTask,
                            TaskId = thisTask.Id
                        };

                        db.AuditTables.Add(newAuditEvent);

                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return RedirectToAction("Edit", "Shopper", new { id = id });
        }

        public ActionResult Close(int id)
        {
            if (ModelState.IsValid)
            {
                string strCurrentUserId = User.Identity.GetUserId();

                try
                {
                    using (var db = new VolunteerNetworkEntities())
                    {
                        var thisTask = (from s in db.SupportTasks
                                        where s.Id == id
                                        select s).FirstOrDefault();

                        thisTask.Status = (int)TicketStatus.Closed;

                        AuditTable newAuditEvent = new AuditTable()
                        {
                            UserId = thisTask.User.Id,
                            EventType = (int)AuditEventType.CloseTask,
                            TaskId = thisTask.Id
                        };

                        db.AuditTables.Add(newAuditEvent);

                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return RedirectToAction("Edit", "Shopper", new { id = id });
        }

        private static void SetCountryList(VolunteerNetworkEntities db)
        {
            if (countryList == null)
            {
                ShopperController.countryList = new List<SelectListItem>();

                var countries = from s in db.Countries
                                select s;

                foreach (Country thiscountry in countries)
                {
                    countryList.Add(new SelectListItem { Text = thiscountry.CountryName, Value = thiscountry.CountryID.ToString() });
                }
            }
        }

        private static void SetCountyList(VolunteerNetworkEntities db)
        {
            if (countyList == null)
            {
                ShopperController.countyList = new List<SelectListItem>();

                var counties = from s in db.States
                                select s;

                foreach (State thiscounty in counties)
                {
                    countyList.Add(new SelectListItem { Text = thiscounty.StateName, Value = thiscounty.StateId.ToString() });
                }
            }
        }
    }
}