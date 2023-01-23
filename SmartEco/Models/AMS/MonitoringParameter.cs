using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace SmartEco.Models.AMS
{
    public class MonitoringParameter
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Unit")]
        public string Unit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCMaxSingle")]
        [DisplayFormat(DataFormatString = "{0:G}", ApplyFormatInEditMode = true)]
        public decimal? MPCMaxSingle { get; set; }
    }
}
