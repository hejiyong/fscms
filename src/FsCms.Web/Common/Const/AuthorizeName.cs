using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FsCms.Web
{
    public class AuthorizeName
    {
        public const string PermissionButton = "PermissionButton";

        public const string Items = "Admin";

        public static string AutoSet(string t)
        {
            return "admin,system,SuperUser";
        }
    }
}
