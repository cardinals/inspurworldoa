using InspurOA.Identity.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.EntityFramework
{
    public class InspurIdentityUser : InspurIdentityUser<string>, IInspurUser
    {
        public InspurIdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public InspurIdentityUser(string userName) : this()
        {
            UserName = userName;
        }

        public InspurIdentityUser(string userName, string email) : this(userName)
        {
            Email = email;
        }
    }

    public class InspurIdentityUser<TKey> : IInspurUser<TKey>
    {
        public TKey Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string Department { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}
