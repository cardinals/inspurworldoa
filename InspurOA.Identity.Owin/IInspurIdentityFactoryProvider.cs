using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Owin
{
    /// <summary>
    ///     Interface used to create objects per request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInspurIdentityFactoryProvider<T> where T : IDisposable
    {
        /// <summary>
        ///     Called once per request to create an object
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        T Create(InspurIdentityFactoryOptions<T> options, IOwinContext context);

        /// <summary>
        ///     Called at the end of the request to dispose the object created
        /// </summary>
        /// <param name="options"></param>
        /// <param name="instance"></param>
        void Dispose(InspurIdentityFactoryOptions<T> options, T instance);
    }
}
