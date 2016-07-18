using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_FormsAuth.Entities
{
    public enum PasswordFormat
    {
        Clear = 0,
        Hashed = 1,
        Encrypted = 2
    }
}