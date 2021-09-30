using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebEnvironmentDataCollector.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser() : base()
        {
            Active = true;
        }

        public AppUser(bool active) : base()
        {
            Active = active;
        }

        public AppUser(string userName) : base(userName)
        {
            Active = true;
        }

        public AppUser(string userName, bool active) : base(userName)
        {
            Active = active;
        }


        public bool Active { get; set; }

        public string Motivo { get; set; }
    }
}
