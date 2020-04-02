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
    [Authorize(Roles = "Volunteer")]
    public class VolunteerController : Controller
    {
        public static List<SelectListItem> countryList = null;

        public ActionResult MyProfile()
        {
            VolunteerModel volunteerModel = new VolunteerModel(); ;

            using (var db = new VolunteerNetworkEntities())
            {
                VolunteerController.SetCountryList(db);

                string strCurrentUserId = User.Identity.GetUserId();

                var users = from s in db.Users
                            where s.AspNetUsersId == strCurrentUserId
                            select s;

                if (users.Count() == 0)
                {
                    volunteerModel.address = new Address();
                    volunteerModel.address.Countries = countryList;
                }
                else
                {
                    User thisUser = users.FirstOrDefault();
                    volunteerModel.firstName = thisUser.Forename;
                    volunteerModel.surname = thisUser.Forename;
                    volunteerModel.address = new Address();

                    VolunteerAddress thisAddress = (from s in db.VolunteerAddresses
                                                    where s.UserId == thisUser.Id
                                                    select s).FirstOrDefault();

                    volunteerModel.address.AddressLine1 = thisAddress.AddressLine1;
                    volunteerModel.address.AddressLine2 = thisAddress.AddressLine2;
                    volunteerModel.address.AddressLine3 = thisAddress.AddressLine3;
                    volunteerModel.address.AddressLine4 = thisAddress.AddressLine4;
                    volunteerModel.address.CountryId = thisAddress.Country;
                    volunteerModel.address.CityId = thisAddress.Locality;
                    volunteerModel.address.Postcode = thisAddress.Postcode;
                    volunteerModel.address.StateId = thisAddress.Region;
                    volunteerModel.address.Countries = countryList;

                    foreach (var country in db.Countries)
                    {
                        volunteerModel.address.Countries.Add(new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });
                    }

                    if (volunteerModel.address.Countries != null)
                    {
                        var states = (from state in db.States
                                      where state.CountryId == volunteerModel.address.CountryId
                                      select state).ToList();
                        foreach (var state in states)
                        {
                            volunteerModel.address.States.Add(new SelectListItem { Text = state.StateName, Value = state.StateId.ToString() });
                        }

                        if (volunteerModel.address.States != null)
                        {
                            var cities = (from city in db.Cities
                                          where city.StateId == volunteerModel.address.StateId
                                          select city).ToList();
                            foreach (var city in cities)
                            {
                                volunteerModel.address.Cities.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                            }
                        }
                    }
                }
                return View(volunteerModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(VolunteerModel model, int? countryId, int? stateId, int? cityId)
        {
            if (ModelState.IsValid &&
                model.address.CountryId > 0 &&
                model.address.StateId > 0 &&
                model.address.CityId > 0)
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    string strCurrentUserId = User.Identity.GetUserId();

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

                        VolunteerAddress newAddress = new VolunteerAddress()
                        {
                            AddressLine1 = model.address.AddressLine1,
                            AddressLine2 = model.address.AddressLine2,
                            AddressLine3 = model.address.AddressLine3,
                            AddressLine4 = model.address.AddressLine4,
                            Locality = model.address.CityId,
                            Region = model.address.StateId,
                            Postcode = model.address.Postcode,
                            Country = model.address.CountryId,
                            UserId = newUser.Id
                        };

                        db.VolunteerAddresses.Add(newAddress);
                        db.SaveChanges();
                    }
                    else
                    {
                        User thisUser = users.FirstOrDefault();

                        thisUser.Forename = model.firstName;
                        thisUser.Surname = model.surname;

                        VolunteerAddress thisAddress = (from s in db.VolunteerAddresses
                                                        where s.UserId == thisUser.Id
                                                        select s).FirstOrDefault();

                        thisAddress.AddressLine1 = model.address.AddressLine1;
                        thisAddress.AddressLine2 = model.address.AddressLine2;
                        thisAddress.AddressLine3 = model.address.AddressLine3;
                        thisAddress.AddressLine4 = model.address.AddressLine4;
                        thisAddress.Country = model.address.CountryId;
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
                    foreach (var country in db.Countries)
                    {
                        model.address.Countries.Add(new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });
                    }

                    if (model.address.Countries != null)
                    {
                        var states = (from state in db.States
                                      where state.CountryId == model.address.CountryId
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

            return RedirectToAction("Index", "Volunteer");
        }

        // GET: Volunteer
        public ActionResult Index()
        {
            TicketListModel ticketListModel = new TicketListModel();

            using (var db = new VolunteerNetworkEntities())
            {
                string strCurrentUserId = User.Identity.GetUserId();

                var users = from s in db.Users
                            where s.AspNetUsersId == strCurrentUserId
                            select s;

                if (users.Count() == 0)
                {
                    return RedirectToAction("MyProfile", "Volunteer");
                }
                else
                {
                    User thisUser = users.FirstOrDefault();

                    var supportTasks = from s in db.SupportTasks
                                       where ((s.AssignedTo == thisUser.Id || s.Status == (int)TicketStatus.Unassigned) && s.User.ShopperAddress.Locality == thisUser.VolunteerAddress.Locality)
                                       select s;

                    ticketListModel.tickets = new List<Ticket>();
                    foreach (SupportTask s in supportTasks)
                    {
                        ticketListModel.tickets.Add(new Ticket
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
                return View(ticketListModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TicketListModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    string strCurrentUserId = User.Identity.GetUserId();

                    var thisUser = (from s in db.Users
                                    where s.AspNetUsersId == strCurrentUserId
                                    select s).FirstOrDefault();

                    var supportTasks = from s in db.SupportTasks
                                       where (s.AssignedTo == thisUser.Id || s.Status == (int)TicketStatus.Unassigned)
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
                            ClosedBy = s.ClosedBy != null ? (int)s.ClosedBy : -1,
                            Status = s.Status,
                            DateRaised = s.DateRaised,
                            DateClosed = s.DateClosed,
                            AssignedTo = s.AssignedTo,
                            Severity = s.severity
                        });
                    }
                }
            }

            return View(model);
        }

        public ActionResult Assign(int id)
        {
            if (ModelState.IsValid)
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    string strCurrentUserId = User.Identity.GetUserId();

                    var thisUser = (from s in db.Users
                                    where s.AspNetUsersId == strCurrentUserId
                                    select s).FirstOrDefault();

                    var thisTask = (from s in db.SupportTasks
                                    where s.Id == id
                                    select s).FirstOrDefault();

                    thisTask.AssignedTo = thisUser.Id;
                    thisTask.Status = (int)TicketStatus.Assigned;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Volunteer");
        }

        public ActionResult Details(int id)
        {
            Ticket thisTicket = null;

            using (var db = new VolunteerNetworkEntities())
            {
                var thisTask = (from s in db.SupportTasks
                                where s.Id == id
                                select s).FirstOrDefault();

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

                return View(thisTicket);
            }
        }

        private static void SetCountryList(VolunteerNetworkEntities db)
        {
            if (countryList == null)
            {
                VolunteerController.countryList = new List<SelectListItem>();

                var countries = from s in db.Countries
                                select s;

                foreach (Country thiscountry in countries)
                {
                    countryList.Add(new SelectListItem { Text = thiscountry.CountryName, Value = thiscountry.CountryID.ToString() });
                }
            }
        }
    }
}