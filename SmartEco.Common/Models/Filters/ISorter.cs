﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Models.Filters
{
    public interface ISorter
    {
        public string SortOrder { get; set; }
    }
}