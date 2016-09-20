using InspurOA.Identity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Owin
{
    public class InspurExternalLoginInfo
    {
        /// <summary>
        ///     Associated login data
        /// </summary>
        public InspurUserLoginInfo Login { get; set; }

        /// <summary>
        ///     Suggested user name for a user
        /// </summary>
        public string DefaultUserName { get; set; }

        /// <summary>
        ///     Email claim from the external identity
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The external identity
        /// </summary>
        public ClaimsIdentity ExternalIdentity { get; set; }
    }
}
