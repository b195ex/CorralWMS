using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorralWMS.Tools
{
    public class SAPSingleton
    {
        private static volatile Company myVar;
        private static object syncRoot = new Object();

        public Company oCompany
        {
            get 
            {
                if (myVar == null)
                {
                    SapSetting connsett;
                    using(var ctx=new LWMS_Context())
                    {
                        connsett = ctx.SapSettings.Find(1);
                        ctx.Entry(connsett).State = System.Data.Entity.EntityState.Detached;
                    }
                    lock (syncRoot)
                    {
                        if (myVar == null)
                        {
                            myVar = new Company();
                            myVar.CompanyDB = connsett.CompanyDB;
                            myVar.DbPassword = connsett.DbPassword;
                            myVar.DbServerType = connsett.DbServerType;
                            myVar.DbUserName = connsett.DbUserName;
                            myVar.language = connsett.language;
                            myVar.Password = connsett.Password;
                            myVar.Server = connsett.Server;
                            myVar.UserName = connsett.UserName;
                            myVar.UseTrusted = connsett.UseTrusted;
                        }
                    }
                }
                if (!myVar.Connected)
                {
                    lock (syncRoot)
                    {
                        if (!myVar.Connected)
                        {
                            if (myVar.Connect() != 0)
                            {
                                int errCode;
                                string errMsg;
                                myVar.GetLastError(out errCode, out errMsg);
                                throw new Exception(string.Format("Error al conectar con SAP, {0}:{1}", errCode, errMsg));
                            }
                        }
                    }
                }
                return myVar;
            }
        }
    }
}