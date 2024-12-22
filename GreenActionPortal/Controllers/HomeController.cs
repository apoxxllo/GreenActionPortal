using GreenActionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GreenActionPortal.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Home()
        {
            var profilePicPath = User.Claims
                            .FirstOrDefault(c => c.Type == "ProfilePictureUrl" && c.Issuer == "greenactionportal")?.Value;

            var garbageCollectionData = _garbageCollectionDataRepo.GetAll(); // Adjust this as per your actual data fetching method

            // Prepare the monthly data for biodegradable and non-biodegradable
            var wasteData = GetWasteDataForGraph(garbageCollectionData);

            ViewBag.ChartData = wasteData;
            return View();
        }

        private dynamic GetWasteDataForGraph(IEnumerable<GarbageCollectionDatum> garbageCollectionData)
        {
            var biodegradableData = new Dictionary<string, int>();
            var nonBiodegradableData = new Dictionary<string, int>();

            // Loop through each record and classify based on the day of the week
            foreach (var data in garbageCollectionData)
            {
                var month = data.Date.Value.ToString("MMMM"); // Get the full month name (e.g., January, February)
                bool isBiodegradable = IsBiodegradable(data.Date.Value.DayOfWeek);

                // If the day is biodegradable, add to the biodegradable collection
                if (isBiodegradable)
                {
                    if (!biodegradableData.ContainsKey(month))
                        biodegradableData[month] = 0;
                    int firstTrip = data.FirstTrip ?? 0;
                    int secondTrip = data.SecondTrip ?? 0;
                    int total = firstTrip + secondTrip;
                    biodegradableData[month] += total; // Adjust the field name as per your model
                }
                else
                {
                    if (!nonBiodegradableData.ContainsKey(month))
                        nonBiodegradableData[month] = 0;

                    int firstTrip = data.FirstTrip ?? 0;
                    int secondTrip = data.SecondTrip ?? 0;
                    int total = firstTrip + secondTrip;
                    nonBiodegradableData[month] += total; // Adjust the field name as per your model
                }
            }

            // Prepare the result to be sent to the view
            var result = new
            {
                biodegradable = biodegradableData.Values.ToArray(),
                nonBiodegradable = nonBiodegradableData.Values.ToArray(),
                months = biodegradableData.Keys.ToArray() // Assuming both categories will have the same months
            };

            return result;
        }

        private bool IsBiodegradable(DayOfWeek dayOfWeek)
        {
            // Monday, Wednesday, and Friday are biodegradable days
            return dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Wednesday || dayOfWeek == DayOfWeek.Friday;
        }

        public IActionResult AboutUs()
        {
            var aboutUsDetails = _aboutUsRepo.Table.FirstOrDefault();
            return View(aboutUsDetails);
        }

        [HttpPost]
        public IActionResult EditAboutUs(string section, string content)
        {
            if (User.Identity.IsAuthenticated)
            {
                var aboutUsDetails = _aboutUsRepo.Table.FirstOrDefault();
                if (section.Equals("History"))
                {
                    aboutUsDetails.History = content;
                    _aboutUsRepo.Update(aboutUsDetails.Id, aboutUsDetails);
                    return Json(new { success = true });
                }
                else if (section.Equals("Vision"))
                {
                    aboutUsDetails.Vision = content;
                    _aboutUsRepo.Update(aboutUsDetails.Id, aboutUsDetails);
                    return Json(new { success = true });
                }
                else if (section.Equals("Mission"))
                {
                    aboutUsDetails.Mission = content;
                    _aboutUsRepo.Update(aboutUsDetails.Id, aboutUsDetails);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }



        public IActionResult CommunityInformation()
        {
            var captain = _userRepo.Table.Where(m => m.PositionId == 1).FirstOrDefault();
            var councilors = _userRepo.Table.Where(m => m.PositionId == 2).ToList();
            var skChairman = _userRepo.Table.Where(m => m.PositionId == 3).FirstOrDefault();
            var treasurer = _userRepo.Table.Where(m => m.PositionId == 4).FirstOrDefault();
            var secretary = _userRepo.Table.Where(m => m.PositionId == 5).FirstOrDefault();

            ViewBag.captain = captain;
            ViewBag.councilors = councilors;
            ViewBag.treasurer = treasurer;
            ViewBag.skChairman = skChairman;
            ViewBag.secretary = secretary;

            var modelView = new CommunityInformationViewModel();
            var populationForTable = _populationRepo.GetAll();
            modelView.Population = populationForTable;
            var population = _populationRepo.GetAll().Select(p => new
            {
                Date = p.Date.Value.ToString("yyyy"), // Format the date as year
                Population = p.PopulationCensus
            }).ToList();
            ViewBag.PopulationData = JsonConvert.SerializeObject(population);

            return View(modelView);
        }

        [HttpPost]
        public IActionResult AddPopulation(DateTime Date, int PopulationCensus)
        {
            // Create a new Population entity
            var newPopulation = new Population
            {
                Date = Date,
                PopulationCensus = PopulationCensus
            };

            // Save to the database (use your repository or DbContext)
            _populationRepo.Create(newPopulation);

            TempData["message"] = "Successfully added data!";
            // Redirect back to the view with updated data
            return RedirectToAction("CommunityInformation");
        }

        [HttpPost]
        public IActionResult EditOfficial(int Id, string firstName, string lastName, IFormFile ProfilePic)
        {
            var official = _userRepo.Get(Id);

            if (official != null)
            {
                ;
                official.FirstName = firstName;
                official.LastName = lastName;

                // Handle file upload for profile picture
                if (ProfilePic != null && ProfilePic.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                    var filePath = Path.Combine(uploads, Path.GetFileName(ProfilePic.FileName));

                    // Save file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfilePic.CopyTo(fileStream);
                    }

                    // Update profile picture path
                    official.ProfilePicPath = $"/Attachments/{Path.GetFileName(ProfilePic.FileName)}";
                }

                // Save changes to database
                _userRepo.Update(official.Id, official);
                TempData["message"] = "Successfully updated official details!";

                // Redirect or return success
                return RedirectToAction("CommunityInformation");
            }

            return View();
        }


        public IActionResult WasteManagement()
        {

            var modelView = new WasteManagementViewModel();

            var garbageCollectionDataMWF = _garbageCollectionDataRepo.Table.Where(m => m.Day.Equals("Monday") || m.Day.Equals("Wednesday") || m.Day.Equals("Friday")).ToList();
            modelView.GarbageCollectionMWF = garbageCollectionDataMWF;

            var garbageCollectionDataTTh = _garbageCollectionDataRepo.Table.Where(m => m.Day.Equals("Tuesday") || m.Day.Equals("Thursday")).ToList();
            modelView.GarbageCollectionTTh = garbageCollectionDataTTh;

            // Initialize variables for each month
            int januaryCans = 0, januaryCartons = 0, januaryPlastics = 0;
            int februaryCans = 0, februaryCartons = 0, februaryPlastics = 0;
            int marchCans = 0, marchCartons = 0, marchPlastics = 0;
            int aprilCans = 0, aprilCartons = 0, aprilPlastics = 0;
            int mayCans = 0, mayCartons = 0, mayPlastics = 0;
            int juneCans = 0, juneCartons = 0, junePlastics = 0;
            int julyCans = 0, julyCartons = 0, julyPlastics = 0;
            int augustCans = 0, augustCartons = 0, augustPlastics = 0;
            int septemberCans = 0, septemberCartons = 0, septemberPlastics = 0;
            int octoberCans = 0, octoberCartons = 0, octoberPlastics = 0;
            int novemberCans = 0, novemberCartons = 0, novemberPlastics = 0;
            int decemberCans = 0, decemberCartons = 0, decemberPlastics = 0;

            // Filter and accumulate data for each month
            for (int month = 1; month <= 12; month++)
            {
                var monthlyData = _garbageCollectionDataRepo.Table
                    .Where(g => g.Date.Value.Month == month)
                    .ToList();

                foreach (var item in monthlyData)
                {
                    if (item.Can.HasValue)
                    {
                        switch (month)
                        {
                            case 1: januaryCans += item.Can ?? 0; break;
                            case 2: februaryCans += item.Can ?? 0; break;
                            case 3: marchCans += item.Can ?? 0; break;
                            case 4: aprilCans += item.Can ?? 0; break;
                            case 5: mayCans += item.Can ?? 0; break;
                            case 6: juneCans += item.Can ?? 0; break;
                            case 7: julyCans += item.Can ?? 0; break;
                            case 8: augustCans += item.Can ?? 0; break;
                            case 9: septemberCans += item.Can ?? 0; break;
                            case 10: octoberCans += item.Can ?? 0; break;
                            case 11: novemberCans += item.Can ?? 0; break;
                            case 12: decemberCans += item.Can ?? 0; break;
                        }
                    }

                    if (item.Cartons.HasValue)
                    {
                        switch (month)
                        {
                            case 1: januaryCartons += item.Cartons ?? 0; break;
                            case 2: februaryCartons += item.Cartons ?? 0; break;
                            case 3: marchCartons += item.Cartons ?? 0; break;
                            case 4: aprilCartons += item.Cartons ?? 0; break;
                            case 5: mayCartons += item.Cartons ?? 0; break;
                            case 6: juneCartons += item.Cartons ?? 0; break;
                            case 7: julyCartons += item.Cartons ?? 0; break;
                            case 8: augustCartons += item.Cartons ?? 0; break;
                            case 9: septemberCartons += item.Cartons ?? 0; break;
                            case 10: octoberCartons += item.Cartons ?? 0; break;
                            case 11: novemberCartons += item.Cartons ?? 0; break;
                            case 12: decemberCartons += item.Cartons ?? 0; break;
                        }
                    }

                    if (item.Plastics.HasValue)
                    {
                        switch (month)
                        {
                            case 1: januaryPlastics += item.Plastics ?? 0; break;
                            case 2: februaryPlastics += item.Plastics ?? 0; break;
                            case 3: marchPlastics += item.Plastics ?? 0; break;
                            case 4: aprilPlastics += item.Plastics ?? 0; break;
                            case 5: mayPlastics += item.Plastics ?? 0; break;
                            case 6: junePlastics += item.Plastics ?? 0; break;
                            case 7: julyPlastics += item.Plastics ?? 0; break;
                            case 8: augustPlastics += item.Plastics ?? 0; break;
                            case 9: septemberPlastics += item.Plastics ?? 0; break;
                            case 10: octoberPlastics += item.Plastics ?? 0; break;
                            case 11: novemberPlastics += item.Plastics ?? 0; break;
                            case 12: decemberPlastics += item.Plastics ?? 0; break;
                        }
                    }
                }
            }

            // Store the results in ViewBag
            ViewBag.JanuaryCans = januaryCans;
            ViewBag.FebruaryCans = februaryCans;
            ViewBag.MarchCans = marchCans;
            ViewBag.AprilCans = aprilCans;
            ViewBag.MayCans = mayCans;
            ViewBag.JuneCans = juneCans;
            ViewBag.JulyCans = julyCans;
            ViewBag.AugustCans = augustCans;
            ViewBag.SeptemberCans = septemberCans;
            ViewBag.OctoberCans = octoberCans;
            ViewBag.NovemberCans = novemberCans;
            ViewBag.DecemberCans = decemberCans;

            ViewBag.JanuaryCartons = januaryCartons;
            ViewBag.FebruaryCartons = februaryCartons;
            ViewBag.MarchCartons = marchCartons;
            ViewBag.AprilCartons = aprilCartons;
            ViewBag.MayCartons = mayCartons;
            ViewBag.JuneCartons = juneCartons;
            ViewBag.JulyCartons = julyCartons;
            ViewBag.AugustCartons = augustCartons;
            ViewBag.SeptemberCartons = septemberCartons;
            ViewBag.OctoberCartons = octoberCartons;
            ViewBag.NovemberCartons = novemberCartons;
            ViewBag.DecemberCartons = decemberCartons;

            ViewBag.JanuaryPlastics = januaryPlastics;
            ViewBag.FebruaryPlastics = februaryPlastics;
            ViewBag.MarchPlastics = marchPlastics;
            ViewBag.AprilPlastics = aprilPlastics;
            ViewBag.MayPlastics = mayPlastics;
            ViewBag.JunePlastics = junePlastics;
            ViewBag.JulyPlastics = julyPlastics;
            ViewBag.AugustPlastics = augustPlastics;
            ViewBag.SeptemberPlastics = septemberPlastics;
            ViewBag.OctoberPlastics = octoberPlastics;
            ViewBag.NovemberPlastics = novemberPlastics;
            ViewBag.DecemberPlastics = decemberPlastics;

            return View(modelView);
        }

        [HttpPost]
        public IActionResult AddGarbageData(DateTime date, string day, int firstTrip, int secondTrip, int cansKg, int plasticsKg, int cartonsKg)
        {
            // Create a new record
            var newGarbageData = new GarbageCollectionDatum
            {
                Date = date,
                Day = day,
                FirstTrip = firstTrip,
                SecondTrip = secondTrip,
                Can = cansKg,
                Plastics = plasticsKg,
                Cartons = cartonsKg
            };

            // Save the new data to the database
            _garbageCollectionDataRepo.Create(newGarbageData);

            TempData["message"] = "Successfully added garbage data!";

            // Redirect or return a view (can be the same page or a different page)
            return RedirectToAction("WasteManagement", "Home"); // Or return a JSON response or partial view, as needed
        }


        public IActionResult Activities()
        {
            DateTime utcNow = DateTime.UtcNow;

            // Define the timezone offset for UTC+08:00
            TimeSpan utcOffset = TimeSpan.FromHours(8); // UTC+08:00

            // Apply the timezone offset to get the local time in UTC+08:00
            DateTime localTime = utcNow + utcOffset;

            // Get upcoming activities (those that have not happened yet)
            var upcomingActivities = _activityRepo.Table
                .Where(m => m.Date > localTime) // Activities scheduled in the future
                .ToList();

            // Get recent activities (those that have already passed)
            var recentActivities = _activityRepo.Table
                .Where(m => m.Date <= localTime) // Activities scheduled in the past
                .ToList();

            // Pass the data to the view using ViewBag or ViewModel
            ViewBag.UpcomingActivities = upcomingActivities;
            ViewBag.RecentActivities = recentActivities;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(string Title, string Description, DateTime Date, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                // Create a new activity object
                var newActivity = new GreenActionPortal.Models.Activity
                {
                    Title = Title,
                    Description = Description,
                    Date = Date
                };

                // Save the photo to the 'wwwroot/Attachments' folder
                if (Photo != null && Photo.Length > 0)
                {
                    // Get file extension
                    var extension = Path.GetExtension(Photo.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;  // Generate a unique file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments", fileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }

                    // Add the photo file path to the activity object
                    newActivity.PhotoPath = $"/Attachments/{Path.GetFileName(fileName)}";
                }

                // Save the activity to the database
                _activityRepo.Create(newActivity);

                TempData["message"] = "Successfully added new activity!";
                // Redirect to the activities page
                return RedirectToAction("Activities");
            }

            return View();  // Return the view if the form data is not valid
        }


        [Authorize]
        public IActionResult EditActivity(int id)
        {
            var activity = _activityRepo.Get(id);
            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActivity(Models.Activity model, IFormFile photo)
        {

            // Save the photo if it's uploaded
            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                var filePath = Path.Combine(uploadsFolder, photo.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Set the PhotoPath (or you can save the file name to your database)
                model.PhotoPath = "/Attachments/" + photo.FileName;
            }
            else
            {
                var oldModel = _activityRepo.Get(model.Id);
                model.PhotoPath = oldModel.PhotoPath;
            }

            // Update the activity in the database (assuming you have an update method)
            _activityRepo.Update(model.Id, model);


            TempData["message"] = "Successfully edited activity!";
            return RedirectToAction("Activities"); // Redirect to a page after saving
        }


        public PartialViewResult FilterUpcoming(string searchTerm, int? year, int? month)
        {
            DateTime utcNow = DateTime.UtcNow;

            // Define the timezone offset for UTC+08:00
            TimeSpan utcOffset = TimeSpan.FromHours(8); // UTC+08:00

            // Apply the timezone offset to get the local time in UTC+08:00
            DateTime localTime = utcNow + utcOffset;
            var activities = _activityRepo.Table
                .Where(m => m.Date > localTime) // Activities scheduled in the future
                .ToList();// Your logic to fetch upcoming activities
            if (!string.IsNullOrEmpty(searchTerm))
            {
                activities = activities.Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (year.HasValue)
            {
                activities = activities.Where(a => a.Date.Value.Year == year.Value).ToList();
            }
            if (month.HasValue)
            {
                activities = activities.Where(a => a.Date.Value.Month == month.Value).ToList();
            }
            return PartialView("_ActivityCards", activities);
        }


        // Filter for Recent Activities
        public PartialViewResult FilterRecent(string searchTerm, int? year, int? month)
        {
            DateTime utcNow = DateTime.UtcNow;

            // Define the timezone offset for UTC+08:00
            TimeSpan utcOffset = TimeSpan.FromHours(8); // UTC+08:00

            // Apply the timezone offset to get the local time in UTC+08:00
            DateTime localTime = utcNow + utcOffset;
            var activities = _activityRepo.Table
                            .Where(m => m.Date <= localTime) // Activities scheduled in the past
                            .ToList(); if (!string.IsNullOrEmpty(searchTerm))
            {
                activities = activities.Where(a => a.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (year.HasValue)
            {
                activities = activities.Where(a => a.Date.Value.Year == year.Value).ToList();
            }
            if (month.HasValue)
            {
                activities = activities.Where(a => a.Date.Value.Month == month.Value).ToList();
            }
            return PartialView("_ActivityCards", activities);
        }

    }
}
