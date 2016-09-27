using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using InspurOA.Models;
using InspurOA.DAL;
using InspurOA.Identity.Core;
using InspurOA.Identity.EntityFramework;
using InspurOA.Identity.Owin;
//using InspurOA.Identity.Owin.Extensions;

namespace InspurOA
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }



    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : InspurSignInManager<InspurUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(InspurUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)InspurUserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : InspurUserManager<InspurUser>
    {
        public ApplicationUserManager(IInspurUserStore<InspurUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new InspurUserStore<InspurUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.InspurUserValidator = new InspurUserValidator<InspurUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                //RequireNonLetterOrDigit = true,
                RequireDigit = true,
                //RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("手机验证码", new InspurPhoneNumberTokenProvider<InspurUser>
            {
                MessageFormat = "你的安全码是: {0}"
            });
            manager.RegisterTwoFactorProvider("邮箱验证码", new InspurEmailTokenProvider<InspurUser>
            {
                Subject = "安全码",
                BodyFormat = "你的安全码是： {0}"
            });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new InspurDataProtectorTokenProvider<InspurUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationRoleManager : InspurRoleManager<InspurIdentityRole>
    {
        public ApplicationRoleManager(IInspurRoleStore<InspurIdentityRole> store)
            : base(store)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new InspurRoleStore(context.Get<ApplicationDbContext>()));
        }
    }


    public class ApplicationPermissionManager : InspurPermissionManager<InspurIdentityPermission>
    {
        public ApplicationPermissionManager(IInspurPermissionStore<InspurIdentityPermission> store)
            : base(store)
        {

        }

        public static ApplicationPermissionManager Create(IdentityFactoryOptions<ApplicationPermissionManager> options, IOwinContext context)
        {
            return new ApplicationPermissionManager(new InspurPermissionStore(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationUserRoleManager : InspurUserRoleManager<InspurUser, InspurIdentityRole, InspurIdentityUserRole>
    {
        public ApplicationUserRoleManager(
            IInspurUserStore<InspurUser> userStore,
            IInspurRoleStore<InspurIdentityRole> roleStore,
            IInspurUserRoleStore<InspurUser, InspurIdentityUserRole> store
            ) : base(userStore, roleStore, store)
        {
        }

        public static ApplicationUserRoleManager Create(IdentityFactoryOptions<ApplicationUserRoleManager> options, IOwinContext context)
        {
            return new ApplicationUserRoleManager(
                new InspurUserStore<InspurUser>(context.Get<ApplicationDbContext>()),
                new InspurRoleStore(context.Get<ApplicationDbContext>()),
                new InspurUserRoleStore<InspurUser>(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationRolePermissionManager : InspurRolePermissionManager<InspurUser, InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission>
    {
        public ApplicationRolePermissionManager(
            IInspurRoleStore<InspurIdentityRole> roleStore,
            IInspurRolePermissionStore<InspurIdentityRole, InspurIdentityPermission, InspurIdentityRolePermission> store)
            : base(roleStore, store)
        {
        }

        public static ApplicationRolePermissionManager Create(IdentityFactoryOptions<ApplicationRolePermissionManager> options, IOwinContext context)
        {
            return new ApplicationRolePermissionManager(
                new InspurRoleStore(context.Get<ApplicationDbContext>()),
                new InspurRolePermissionStore<InspurUser>(context.Get<ApplicationDbContext>()));
        }
    }
}
