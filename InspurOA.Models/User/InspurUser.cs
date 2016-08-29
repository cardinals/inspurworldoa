using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Models
{
    public class InspurUser : IUser<string>
    {
        string IUser<string>.Id { get; }

        string IUser<string>.UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
