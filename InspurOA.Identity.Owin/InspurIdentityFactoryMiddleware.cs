using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Owin
{
    /// <summary>
    ///     OwinMiddleware that initializes an object for use in the OwinContext via the Get/Set generic extensions method
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    public class InspurIdentityFactoryMiddleware<TResult, TOptions> : OwinMiddleware
        where TResult : IDisposable
        where TOptions : InspurIdentityFactoryOptions<TResult>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="options">Configuration options for the middleware</param>
        public InspurIdentityFactoryMiddleware(OwinMiddleware next, TOptions options)
            : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (options.Provider == null)
            {
                throw new ArgumentNullException("options.Provider");
            }
            Options = options;
        }

        /// <summary>
        ///     Configuration options
        /// </summary>
        public TOptions Options { get; private set; }

        /// <summary>
        ///     Create an object using the Options.Provider, storing it in the OwinContext and then disposes the object when finished
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            var instance = Options.Provider.Create(Options, context);
            try
            {
                context.Set(instance);
                if (Next != null)
                {
                    await Next.Invoke(context);
                }
            }
            finally
            {
                Options.Provider.Dispose(Options, instance);
            }
        }
    }
}
