using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAPI_FormsAuth.Services
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
    }

    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
