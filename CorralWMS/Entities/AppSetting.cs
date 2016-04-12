using System.ComponentModel.DataAnnotations;

namespace CorralWMS.Entities
{
    public class AppSetting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}