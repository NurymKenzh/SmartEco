using SmartEco.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Models.Filters.Directories
{
    public interface IPersonFilter : ISorter
    {
        public string Email { get; set; }
        public int? RoleId { get; set; }
    }
}
