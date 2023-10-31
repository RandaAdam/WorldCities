using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using System.Security;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Adminstrator")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IConfiguration configuration) {
            _context = context;
            _env = env;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ActionResult> Import()
        {
            //only dev environmet
            if (!_env.IsDevelopment())
                throw new SecurityException("This is not the suitable env.");

            var path = Path.Combine(
                _env.ContentRootPath,
                "Data/Source/worldcities.xlsx"
                );

            using var stream = System.IO.File.OpenRead( path );
            using var excelPackage = new ExcelPackage(stream);

            //first worksheat
            var worksheet = excelPackage.Workbook.Worksheets[0];

            //num of rows to process
            var nEndRow = worksheet.Dimension.End.Row;

            //initialize rec counters
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            //new lookup dic
            // containing all the countries already existing 
            // into the Database (it will be empty on first run).

            var countriesByName = _context.Countries.AsNoTracking()
                .ToDictionary(x=>x.Name,
                StringComparer.OrdinalIgnoreCase);

            for(int nRow =2; nRow<=nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];

                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();
                // skip this country if it already exists in the database
                if (countriesByName.ContainsKey(countryName))
                    continue;

                // create the Country entity and fill it with xlsx data 
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                await _context.Countries.AddAsync(country);

                //store to lookup to retrieve its id
                countriesByName.Add(countryName, country);
                numberOfCountriesAdded++;
            }

            //save countries to db
            if(numberOfCountriesAdded > 0)
            {
                await _context.SaveChangesAsync();
            }

            // create a lookup dictionary
            // containing all the cities already existing 
            // into the Database (it will be empty on first run). 
            var cities = _context.Cities
            .AsNoTracking()
            .ToDictionary(x => (
            Name: x.Name,
            Lat: x.Lat,
            Lon: x.Lon,
            CountryId: x.CountryId));

            // iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
 nRow, 1, nRow, worksheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                // retrieve country Id by countryName
                var countryId = countriesByName[countryName].Id;

                // skip this city if it already exists in the database
                if (cities.ContainsKey((
                Name: name,
                Lat: lat,
                Lon: lon,
                CountryId: countryId)))
                    continue;

                // create the City entity and fill it with xlsx data 
                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };

                // add the new city to the DB context 
                _context.Cities.Add(city);
                // increment the counter 
                numberOfCitiesAdded++;
            }

            // save all the cities into the Database 
            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            //default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Adminstrator = "Adminstrator";

            //create the default roles if not exist
            if(await _roleManager.FindByNameAsync(role_RegisteredUser)
                == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
            }

            if(await _roleManager.FindByNameAsync(role_Adminstrator) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Adminstrator));
            }

            //list to track newly added users
            var addedUserList = new List<ApplicationUser>();

            var email_admin = "admin@email.com";
            if(await _roleManager.FindByNameAsync(email_admin) == null)
            {
                //create new admin
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_admin,
                    Email = email_admin,
                };

                //insert admin user to db
                await _userManager.CreateAsync(user_Admin,
                    _configuration["DefaultPasswords:Adminstrator"]);

                //assign the registered user and :Adminstrator" role
                await _userManager.AddToRoleAsync(user_Admin, role_Adminstrator);
                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);

                //confirm email and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                //add admin user to users list
                addedUserList.Add(user_Admin);
            }

            //check if the standard user already exists
            var email_User = "user@email.com";
            if(await _userManager.FindByNameAsync(email_User) == null)
            {
                //create new standard application user
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User,
                };
                await _userManager.CreateAsync(user_User,
                    _configuration["DefaultPasswords:RegisteredUser"]);

                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled=false;

                addedUserList.Add(user_User);
            }

            if(addedUserList.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                Count = addedUserList.Count,
                User = addedUserList
            });
        }
    }

    
}
