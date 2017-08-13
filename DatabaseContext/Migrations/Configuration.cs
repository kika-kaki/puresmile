using System;
using System.Collections.Generic;
using DatabaseContext;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace PureSmileUI.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //default roles
            var superUserRole = new Role
            {
                Name = "SuperUser"
            };
            var staffRole = new Role
            {
                Name = "Staff"
            };
            var userRole = new Role
            {
                Name = "Client"
            };
            var roleManager = new DatabaseContext.Models.RoleManager<Role>(new RoleStore(context));
            roleManager.Create(superUserRole);
            roleManager.Create(staffRole);
            roleManager.Create(userRole);

            //default users
            var UserManager = new UserManager<User, int>(new UserStore(context));
            var PasswordHash = new PasswordHasher();

            //SuperUser
            if (!context.Users.Any(u => u.UserName == "admin@admin.net"))
            {
                var user = new User
                {
                    UserName = "admin@admin.net",
                    Email = "admin@admin.net",
                    PasswordHash = PasswordHash.HashPassword("123456")
                };

                UserManager.Create(user);
                UserManager.AddToRole(user.Id, superUserRole.Name);
            }

            //staff Dr.Akula
            if (!context.Users.Any(u => u.UserName == "dr@acula.net"))
            {
                var user = new User
                {
                    UserName = "dr@acula.net",
                    Email = "dr@acula.net",
                    PasswordHash = PasswordHash.HashPassword("123456")
                };

                UserManager.Create(user);
                UserManager.AddToRole(user.Id, staffRole.Name);
            }

            //User Alice
            if (!context.Users.Any(u => u.UserName == "alice@alice.net"))
            {
                var user = new User
                {
                    UserName = "alice@alice.net",
                    Email = "alice@alice.net",
                    PasswordHash = PasswordHash.HashPassword("123123"),
                    LockoutEnabled = false
                };

                UserManager.Create(user);
                UserManager.AddToRole(user.Id, userRole.Name);
            }

            //User Carry
            if (!context.Users.Any(u => u.UserName == "carry@carry.net"))
            {
                var user = new User
                {
                    UserName = "carry@carry.net",
                    Email = "carry@carry.net",
                    PasswordHash = PasswordHash.HashPassword("123123"),
                    LockoutEnabled = false
                };

                UserManager.Create(user);
                UserManager.AddToRole(user.Id, userRole.Name);
            }

            //default clinic
            if (!context.Clinics.Any(n => n.BusinessName == "Miricos Aesthetics Clinic"))
            {
                var now = DateTime.Now;
                var hours = new List<ClinicHours>()
                {
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        Day = DayOfWeek.Tuesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        Day = DayOfWeek.Wednesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        Day = DayOfWeek.Thursday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        Day = DayOfWeek.Friday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 10, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0, 0),
                        Day = DayOfWeek.Saturday
                    }
                };
                var clinicManager = new ClinicManager();
                var newClinic = clinicManager.CreateOrUpdate(new Clinic
                {
                    BusinessName = "Miricos Aesthetics Clinic",
                    ShortName = "MAS",
                    BusinessAddress = "Level 2, 340 Collins Street, Melbourne VIC 3000",
                    OwnersName = "Ricky Lee",
                    OwnersEmailAddress = "info@miricos.com.au",
                    OwnersPhoneNumber = "+61435782764",
                    BusinessWebsiteAddress = "www.miricos.com.au",
                    EmailAddressForOnlineBookings = "miricos@puresmile.com.au",
                    DirectPhoneToTheLocation = "03 9913 7362",
                    ContactPerson = "Kitshi Hui",
                    StoreManagerPhoneNumber = "03 9913 7362",
                    WebsiteLocationAddress = "Level 2, 340 Collins Street, Melbourne VIC 3000",
                    IsActive = true,
                    City = "Melbourne",
                    State = "VIC 3000",
                    Address = "Level 2, 340 Collins Street, Melbourne VIC 3000",
                    Email = "miricos@puresmile.com.au",
                    Hours = hours,
                    BankAccountName = "Miricos Pty Ltd.",
                    BankBsb = "013257",
                    BankAccountNumber = "398887026"
                });
            }

            if (!context.Clinics.Any(n => n.BusinessName == "Esteem Hair Beauty Spa"))
            {
                var now = DateTime.Now;
                var hours = new List<ClinicHours>()
                {
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        Day = DayOfWeek.Monday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        Day = DayOfWeek.Tuesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0, 0),
                        Day = DayOfWeek.Wednesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0, 0),
                        Day = DayOfWeek.Thursday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 30, 0, 0),
                        Day = DayOfWeek.Friday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 8, 30, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 8, 30, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 16, 0, 0, 0),
                        Day = DayOfWeek.Saturday
                    }
                };
                var clinicManager = new ClinicManager();
                var newClinic = clinicManager.CreateOrUpdate(new Clinic
                {
                    BusinessName = "Esteem Hair Beauty Spa",
                    ShortName = "EHBS",
                    BusinessAddress = "Shop 15 Tattersalls Centre 510 - 536 High Street Penrith NSW 2750",
                    ABN = "73518997490",
                    OwnersName = "Martin Lazare",
                    OwnersEmailAddress = "martin@esteemsalon.com.au",
                    OwnersPhoneNumber = "402068707",
                    BusinessWebsiteAddress = "www.esteemhairbeautyspa.com.au",
                    EmailAddressForOnlineBookings = "bookings@esteemhairbeautyspa.com.au",
                    DirectPhoneToTheLocation = "(02) 47223250",
                    ContactPerson = "Martin Lazare",
                    StoreManagerPhoneNumber = "402068707",
                    WebsiteLocationAddress = "Shop 15 Tattersalls Centre 510 - 536 High Street Penrith NSW 2750",
                    IsActive = true,
                    City = "Penrith",
                    State = "NSW 2750",
                    Address = "Shop 15 Tattersalls Centre 510 - 536 High Street Penrith NSW 2750",
                    Email = "bookings@esteemhairbeautyspa.com.au",
                    Hours = hours,
                    BankAccountName = "Esteem Hair Beauty Spa",
                    BankBsb = "12429",
                    BankAccountNumber = "457356912",
                    HowToFind = "Centrally Located on High Street Penrtih across the road from Trade Secret and within 2 minutes walk of Westfield shopping centre. Best parking union lane carpark behind the tattersalls centre."
                });
            }

            if (!context.Clinics.Any(n => n.BusinessName == "Sunlounge tanning and beauty"))
            {
                var now = DateTime.Now;
                var hours = new List<ClinicHours>()
                {
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        Day = DayOfWeek.Tuesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        Day = DayOfWeek.Wednesday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        Day = DayOfWeek.Thursday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0, 0),
                        Day = DayOfWeek.Friday
                    },
                    new ClinicHours
                    {
                        BookingOpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        BookingCloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        OpenHour = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0, 0),
                        CloseHour = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0, 0),
                        Day = DayOfWeek.Saturday
                    }
                };
                var clinicManager = new ClinicManager();
                var newClinic = clinicManager.CreateOrUpdate(new Clinic
                {
                    BusinessName = "Sunlounge tanning and beauty",
                    ShortName = "STB",
                    BusinessAddress = "43 the parade norwood SA 5067",
                    ABN = "30605284124",
                    OwnersName = "Daniel Borg",
                    OwnersEmailAddress = "dan@dnaurbanspaces.com.au",
                    OwnersPhoneNumber = "405529989",
                    BusinessWebsiteAddress = "sunloungetanningandbeauty.com.au",
                    EmailAddressForOnlineBookings = "dan@dnaurbanspaces.com.au",
                    DirectPhoneToTheLocation = "08 83626211",
                    ContactPerson = "Amy Minervini",
                    StoreManagerPhoneNumber = "08 83626211",
                    WebsiteLocationAddress = "sunloungetanningandbeauty.com.au",
                    IsActive = true,
                    City = "Norwood",
                    State = "SA 5067",
                    Address = "43 the parade norwood SA 5067",
                    Email = "sunloungetanningandbeauty.com.au",
                    Hours = hours,
                    BankAccountName = "Rejuv cosmedical",
                    BankBsb = "065-115",
                    BankAccountNumber = "1040 7681",
                    HowToFind = "On the cnr of the Parade and Sydenham road Norwood."
                });
            }

            //Treatment category
            if (!context.TreatmentCategories.Any(n => n.Name == "Whitening"))
            {
                var manager = new TreatmentCategoryManager();
                var categoryId = manager.CreateOrUpdate(new TreatmentCategory()
                {
                    Description = "Whitening",
                    Name = "Whitening",
                    PictureUrl = "Whitening"
                });

                var treatmentManager = new TreatmentManager();
                var url = "http://drperrone.com/blog/wp-content/uploads/2012/06/smiles-300x300.jpg";
                treatmentManager.CreateOrUpdate(new Treatment()
                {
                    Name = "Whitening",
                    Description = "Whitening",
                    PictureUrl = url,
                    Price = 120,
                    TreatmentCategoryId = categoryId
                });
            }
        }
    }
}
