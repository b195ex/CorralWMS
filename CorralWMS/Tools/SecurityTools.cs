using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorralWMS.Tools
{
    public class SecurityTools
    {
        public static HashSet<Permission> GetPermissions(User usuario, LWMS_Context ctx)
        {
            HashSet<Permission> permissions = new HashSet<Permission>();
            ctx.Entry(usuario).Collection("Roles").Load();
            foreach (var rol in usuario.Roles)
            {
                ctx.Entry(rol).Collection("Permissions").Load();
                permissions.UnionWith(rol.Permissions);
            }
            return permissions;
        }
    }
}