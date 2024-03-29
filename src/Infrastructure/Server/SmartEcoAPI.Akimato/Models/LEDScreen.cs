﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class LEDScreen
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int? Number { get; set; }

        public decimal NorthLatitude { get; set; }
        
        public decimal EastLongitude { get; set; }
        
        public MonitoringPost MonitoringPost { get; set; }
        public int MonitoringPostId { get; set; }
    }
}
