using System;
using System.Collections.Generic;

namespace RedLine.Servicios
{
    public class LoginException : Exception
    {
        public LoginResult Result { get; set; }

        public LoginException(LoginResult result, string mensaje = "") : base(mensaje)
        {
            Result = result;
        }
    }
}
