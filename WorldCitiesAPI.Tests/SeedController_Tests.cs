using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using WorldCitiesAPI.Controllers;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Tests
{
    public class SeedController_Tests
    {
        [Fact]
        public async Task CreateDefaultUsers()
        {
            // Arrange
            // create the option instances required by the
            // ApplicationDbContext
            var options = new
                DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;
            // create a IWebHost environment mock instance
            var mockEnv = Mock.Of<IWebHostEnvironment>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(
                x => x[It.Is<string>(s => s == "DefaultPasswords:RegisteredUser")]).
                Returns("M0ckP$$word");
            mockConfiguration.SetupGet(
                x => x[It.Is<string>(s => s == "DefaultPasswords:Adminstrator")]).
                Returns("M0ckP$$word");

            // create a ApplicationDbContext instance using the
            // in-memory DB
            using var context = new ApplicationDbContext(options);

            // create a RoleManager instance
            var roleManager = IdentityHelper.GetRoleManager(
                new RoleStore<IdentityRole>(context));

            //create userManager instance
            var userManager = IdentityHelper.GetUserManager(
                new UserStore<ApplicationUser>(context));

            //create seedController instance
            var controller = new SeedController(
                context,
                roleManager,
                userManager,
                mockEnv,
                mockConfiguration.Object);

            //definr var for required test users
            ApplicationUser user_Admin = null;
            ApplicationUser user_User = null;
            ApplicationUser user_NotExisting = null;

            //Act
            //execute seedController's CreateDefaultUsers() method
            //to create the default users and roles
            await controller.CreateDefaultUsers();

            // retrieve the users
            user_Admin = await userManager.FindByEmailAsync("admin@email.com");
            user_User = await userManager.FindByEmailAsync("user@email.com");
            user_NotExisting = await userManager.FindByEmailAsync("notexixting@email.com");

            //Assert
            Assert.NotNull(user_Admin); 
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);
        }
    }
}
