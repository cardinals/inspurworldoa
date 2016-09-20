using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public interface IInspurUser : IInspurUser<string>
    {
    }

    public interface IInspurUser<out TKey>
    {
        TKey Id { get; }

        string UserName { get; set; }
    }
}
