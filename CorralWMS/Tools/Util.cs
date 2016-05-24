using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorralWMS.Tools
{
    public class Util
    {
        public static string GetCtxConnStr()
        {
            using (var ctx = new LWMS_Context())
            {
                return ctx.Database.Connection.ConnectionString;
            }
        }

        public static string GetSapConnStr()
        {
            using (var ctx = new LWMS_Context())
            {
                return ctx.SapSettings.Find(1).ConnectionString;
            }
        }
    }
}