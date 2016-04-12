using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class SapSetting
    {
        public int id { get; set; }
        [Required]
        public string CompanyDB { get; set; }
        [Required]
        public string DbPassword { get; set; }
        public BoDataServerTypes DbServerType { get; set; }
        [Required]
        public string DbUserName { get; set; }
        public BoSuppLangs language { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Server { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool UseTrusted { get; set; }
    }
}