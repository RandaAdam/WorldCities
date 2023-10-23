using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WorldCitiesAPI.Tests
{
    /// <summary>
    /// We created two methods—GetRoleManager and GetUserManager—which we can use to 
    /// create these providers for other tests.
    /// This basically means that these providers will perform their job for real, but 
    /// everything will be done on the in-memory database instead of the SQL Server data source,
    /// </summary>
    public static class IdentityHelper
    {
        public static RoleManager<TIdentityRole> 
            GetRoleManager<TIdentityRole>(
                IRoleStore<TIdentityRole> roleStore)
                where TIdentityRole : IdentityRole
        {
            return new RoleManager<TIdentityRole>(
                roleStore,
                Array.Empty<IRoleValidator<TIdentityRole>>(),
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        }

        public static UserManager<TIDentityUser>
            GetUserManager<TIDentityUser>(
                IUserStore<TIDentityUser> userStore
            )
            where TIDentityUser : IdentityUser
        {
            return new UserManager<TIDentityUser>(userStore,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<TIDentityUser>>().Object,
                new IUserValidator<TIDentityUser>[0],
                new IPasswordValidator<TIDentityUser>[0],
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<TIDentityUser>>>().Object);
        }

    }
}
