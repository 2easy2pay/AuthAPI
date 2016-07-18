﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_FormsAuth.Entities
{
    public static partial class SystemCustomerRoleNames
    {
        public static string Administrators { get { return "Administrators"; } }

        public static string ForumModerators { get { return "ForumModerators"; } }

        public static string Registered { get { return "Registered"; } }

        public static string Guests { get { return "Guests"; } }

        public static string Vendors { get { return "Vendors"; } }
    }
}
