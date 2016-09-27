using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Owin
{
     /// <summary>
     ///     Configuration options for a IdentityFactoryMiddleware
     /// </summary>
     /// <typeparam name="T"></typeparam>
    public class InspurIdentityFactoryOptions<T> where T : IDisposable
    {
        /// <summary>
        ///     Used to configure the data protection provider
        /// </summary>
        public IDataProtectionProvider DataProtectionProvider { get; set; }

        /// <summary>
        ///     Provider used to Create and Dispose objects
        /// </summary>
        public IInspurIdentityFactoryProvider<T> Provider { get; set; }
    }
}
