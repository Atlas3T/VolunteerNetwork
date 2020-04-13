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
        public static List<SelectListItem> countyList = null;

        public ActionResult MyProfile()
        {
            VolunteerModel volunteerModel = new VolunteerModel();
            string strCurrentUserId = User.Identity.GetUserId();

            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    VolunteerController.SetCountryList(db);
                    VolunteerController.SetCountyList(db);

                    var users = from s in db.Users
                                where s.AspNetUsersId == strCurrentUserId
                                select s;

                    if (users.Count() == 0)
                    {
                        volunteerModel.address = new Address
                        {
                            Countries = countryList,
                            CountryId = 235,
                            States = countyList
                        };
                    }
                    else
                    {
                        User thisUser = users.FirstOrDefault();
                        volunteerModel.firstName = thisUser.Forename;
                        volunteerModel.surname = thisUser.Surname;
                        volunteerModel.address = new Address();

                        VolunteerAddress thisAddress = (from s in db.VolunteerAddresses
                                                        where s.UserId == thisUser.Id
                                                        select s).FirstOrDefault();

                        volunteerModel.address.AddressLine1 = thisAddress.AddressLine1;
                        volunteerModel.address.AddressLine2 = thisAddress.AddressLine2;
                        volunteerModel.address.AddressLine3 = thisAddress.AddressLine3;
                        volunteerModel.address.AddressLine4 = thisAddress.AddressLine4;
                        volunteerModel.address.CountryId = 235; //thisAddress.Country;
                        volunteerModel.address.CityId = thisAddress.Locality;
                        volunteerModel.address.Postcode = thisAddress.Postcode;
                        volunteerModel.address.StateId = thisAddress.Region;
                        volunteerModel.address.Countries = countryList;

                        SearchArea thisSearchArea = (from s in db.SearchAreas
                                                     where s.UserId == thisUser.Id
                                                     select s).FirstOrDefault();

                        volunteerModel.address.VolunteerAreaOneId = (int)thisSearchArea.City1;
                        volunteerModel.address.VolunteerAreaTwoId = thisSearchArea.City2;
                        volunteerModel.address.VolunteerAreaThreeId = thisSearchArea.City3;
                        volunteerModel.address.VolunteerAreaFourId = thisSearchArea.City4;

                        foreach (var country in db.Countries)
                        {
                            volunteerModel.address.Countries.Add(new SelectListItem { Text = country.CountryName, Value = country.CountryID.ToString() });
                        }

                        if (volunteerModel.address.Countries != null)
                        {
                            var states = (from state in db.States
                                          where state.CountryId == 235 //volunteerModel.address.CountryId
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

                                    volunteerModel.address.searchCitiesListOne.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    volunteerModel.address.searchCitiesListTwo.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    volunteerModel.address.searchCitiesListThree.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    volunteerModel.address.searchCitiesListFour.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
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

            return View(volunteerModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(VolunteerModel model)
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
                                AspNetUsersId = strCurrentUserId,
                                RefId = Guid.NewGuid().ToString()
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
                                Country = 235, //model.address.CountryId,
                                UserId = newUser.Id
                            };

                            db.VolunteerAddresses.Add(newAddress);

                            SearchArea newSearchArea = new SearchArea
                            {
                                City1 = model.address.VolunteerAreaOneId,
                                UserId = newUser.Id
                            };

                            if (model.address.VolunteerAreaTwoId != null)
                            {
                                newSearchArea.City1 = model.address.VolunteerAreaTwoId;
                            }
                            if (model.address.VolunteerAreaThreeId != null)
                            {
                                newSearchArea.City1 = model.address.VolunteerAreaThreeId;
                            }
                            if (model.address.VolunteerAreaFourId != null)
                            {
                                newSearchArea.City1 = model.address.VolunteerAreaFourId;
                            }

                            db.SearchAreas.Add(newSearchArea);

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
                            thisAddress.Country = 235; // model.address.CountryId;
                            thisAddress.Locality = model.address.CityId;
                            thisAddress.Postcode = model.address.Postcode;
                            thisAddress.Region = model.address.StateId;

                            SearchArea thisSearchArea = (from s in db.SearchAreas
                                                         where s.UserId == thisUser.Id
                                                         select s).FirstOrDefault();

                            thisSearchArea.City1 = model.address.VolunteerAreaOneId;

                            if (model.address.VolunteerAreaTwoId != null)
                            {
                                thisSearchArea.City2 = model.address.VolunteerAreaTwoId;
                            }
                            if (model.address.VolunteerAreaThreeId != null)
                            {
                                thisSearchArea.City3 = model.address.VolunteerAreaThreeId;
                            }
                            if (model.address.VolunteerAreaFourId != null)
                            {
                                thisSearchArea.City4 = model.address.VolunteerAreaFourId;
                            }

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
                                    model.address.searchCitiesListOne.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    model.address.searchCitiesListTwo.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    model.address.searchCitiesListThree.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
                                    model.address.searchCitiesListFour.Add(new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() });
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

            return RedirectToAction("Index", "Volunteer");
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
                        return RedirectToAction("MyProfile", "Volunteer");
                    }
                    else
                    {
                        var thisUser = (from s in db.Users
                                        where s.AspNetUsersId == users.FirstOrDefault().AspNetUsersId
                                        select s).FirstOrDefault();

                        if (thisUser.KYCVerified == false)
                        {
                            return RedirectToAction("KYCCheck", "Volunteer");
                        }

                        var searchAreas = (from s in db.SearchAreas
                                           where s.UserId == users.FirstOrDefault().Id
                                           select s).FirstOrDefault();

                        var supportTasks = from s in db.SupportTasks
                                           where ((s.AssignedTo == users.FirstOrDefault().Id || s.Status == (int)TicketStatus.Unassigned) &&
                                                  (s.User.ShopperAddress.Locality == searchAreas.City1 ||
                                                   s.User.ShopperAddress.Locality == searchAreas.City2 ||
                                                   s.User.ShopperAddress.Locality == searchAreas.City3 ||
                                                   s.User.ShopperAddress.Locality == searchAreas.City4))
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
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return View(model);
        }

        public ActionResult Assign(int id)
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

                        var thisTask = (from s in db.SupportTasks
                                        where s.Id == id
                                        select s).FirstOrDefault();

                        thisTask.AssignedTo = thisUser.Id;
                        thisTask.Status = (int)TicketStatus.Assigned;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ErrorClass.LogError(strCurrentUserId, ErrorMessageType.Exception.ToString(), e.Message);
                }
            }

            return RedirectToAction("Index", "Volunteer");
        }

        public ActionResult Details(int id)
        {
            Ticket thisTicket = null;

            try
            {
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
                        case (int)TicketType.General:
                        default:
                            thisTicket.TypeText = "General";
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorClass.LogError(User.Identity.GetUserId(), ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(thisTicket);
        }

        public ActionResult TicketAddress(int id)
        {
            ViewBag.Id = id;
            ShopperModel shopperModel = new ShopperModel();

            try
            {
                using (var db = new VolunteerNetworkEntities())
                {
                    SetCountryList(db);

                    var thisTask = (from s in db.SupportTasks
                                    where s.Id == id
                                    select s).FirstOrDefault();

                    var thisUser = (from s in db.Users
                                    where s.Id == thisTask.RaisedBy
                                    select s).FirstOrDefault();

                    var thisUserAddress = (from s in db.ShopperAddresses
                                           where s.UserId == thisTask.RaisedBy
                                           select s).FirstOrDefault();

                    shopperModel.firstName = thisUser.Forename;
                    shopperModel.surname = thisUser.Surname;
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
            catch (Exception e)
            {
                ErrorClass.LogError(User.Identity.GetUserId(), ErrorMessageType.Exception.ToString(), e.Message);
            }

            return View(shopperModel);
        }

        public ActionResult KYCCheck()
        {
            string strCurrentUserId = User.Identity.GetUserId();

            using (var db = new VolunteerNetworkEntities())
            {
                var thisUser = (from s in db.Users
                                where s.AspNetUsersId == strCurrentUserId
                                select s).FirstOrDefault();

                ViewBag.RefId = thisUser.RefId;
            }

            return View();
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

        private static void SetCountyList(VolunteerNetworkEntities db)
        {
            if (countyList == null)
            {
                VolunteerController.countyList = new List<SelectListItem>();

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
