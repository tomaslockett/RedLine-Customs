using System;
using System.Collections.Generic;

namespace RedLine.BLL
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
