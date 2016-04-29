using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class MailSetting
    {
        public int Id { get; set; }
        [Required]
        public string FromAddress { get; set; }
        [Required]
        public string FromPass { get; set; }
        [Required]
        public string MailHost { get; set; }
        public int MailPort { get; set; }
    }
}