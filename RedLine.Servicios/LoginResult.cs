using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios
{
    public enum LoginResult
    {
        ValidUser,
        InvalidUsername,
        InvalidPassword,
        UserBlocked,
        UserInactive,
        UserAlreadyLoggedIn
    }
}
