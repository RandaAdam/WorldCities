using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Tests
{
    public class CitesController_Test
    {
        /// <summary>
        /// test getcity method
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCity()
        {
            // Arrange
            // define the required assets
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;
            var context = new ApplicationDbContext(options);
            context.Add(new City()
            {
                Id = 1,
                CountryId = 1,
                Lat = 1,
                Lon = 1,
                Name = "TestCity1"
            });
            context.SaveChanges();

            var controller = new CitiesController(context);
            City? city_existing=null;
            City? city_notExisting =null;

            // Act
            // invoke the test
            city_existing = (await controller.GetCity(1)).Value;
            city_notExisting = (await controller.GetCity(2)).Value;

            // Assert
            // verify that conditions are met
            Assert.NotNull(city_existing);
            Assert.Null(city_notExisting);
        }
    }
}
