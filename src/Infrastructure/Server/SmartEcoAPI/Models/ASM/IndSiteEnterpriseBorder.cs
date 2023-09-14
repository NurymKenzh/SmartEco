using SmartEcoAPI.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    public class IndSiteEnterpriseBorder
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int IndSiteEnterpriseId { get; set; }
        public IndSiteEnterprise IndSiteEnterprise { get; set; }

        public List<string> Coordinates { get; set; }
    }
}
