using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SitesMonitoring.Models
{
    public class Site
    {
        public int ID { get; set; }
        [Display(Name = "URL для проверки")]
        public string URL { get; set; }

        [Display(Name = "Статус сайта")]     
        //0 -не доступен, 1 - доступен
        public int? Status { get; set; }

        [Display(Name = "Дата последней проверки")]        
        public DateTime ? LastCheckedTime { get; set; }

       
        public string GetStatusString
        {
          
            get
            {
                if (Status == null)
                    return "";
                if (Status == 0)
                    return "Недоступен";
                else if (Status == 1)
                    return "Доступен";
                return "";
            }
        }


    }
}
