﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class SpeciallyProtectedNaturalTerritory
    {
        public int Id { get; set; }
        
        public string NameKK { get; set; }
        
        public string NameRU { get; set; }
        
        public string NameEN { get; set; }
        
        public AuthorizedAuthority AuthorizedAuthority { get; set; }
        public int AuthorizedAuthorityId { get; set; }
        
        public decimal Areahectares { get; set; }
    }
}
