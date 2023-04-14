using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class TargetTerritory
    {
        public int Id { get; set; }
        
        public TerritoryType TerritoryType { get; set; }
        public int TerritoryTypeId { get; set; }
        
        public KATO KATO { get; set; }
        public int? KATOId { get; set; }
        
        public string NameKK { get; set; }
        
        public string NameRU { get; set; }
        
        public string GISConnectionCode { get; set; }
        
        public string AdditionalInformationKK { get; set; }
        
        public string AdditionalInformationRU { get; set; }
        
        public MonitoringPost MonitoringPost { get; set; }
        public int? MonitoringPostId { get; set; }
        
        public KazHydrometSoilPost KazHydrometSoilPost { get; set; }
        public int? KazHydrometSoilPostId { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
