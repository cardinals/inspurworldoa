﻿using InspurOA.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspurOA.Web.Models
{
    public class PermissionItemViewModel
    {
        public InspurIdentityPermission Permission { get; set; }

        public bool IsChecked { get; set; }
    }
}