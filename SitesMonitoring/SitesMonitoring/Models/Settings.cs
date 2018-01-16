using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SitesMonitoring.Models
{
    public class Settings
    {
        public int ID { get; set; }
        [Display(Name = "Период ожидания таймера (секунды)")]
        public int WaitSecond { get; set; }
    }
}
