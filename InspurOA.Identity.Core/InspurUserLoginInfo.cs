using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
     /// <summary>
     ///     Represents a linked login for a user (i.e. a facebook/google account)
     /// </summary>
    public sealed class InspurUserLoginInfo
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        public InspurUserLoginInfo(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        /// <summary>
        ///     Provider for the linked login, i.e. Facebook, Google, etc.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        ///     User specific key for the login provider
        /// </summary>
        public string ProviderKey { get; set; }
    }
}
