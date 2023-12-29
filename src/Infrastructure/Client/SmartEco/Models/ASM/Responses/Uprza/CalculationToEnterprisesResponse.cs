﻿using SmartEco.Models.ASM.Uprza;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Responses
{
    public class CalculationToEnterprisesResponse
    {
        public CalculationToEnterprisesResponse(List<CalculationToEnterprise> calcToEnts, int count) 
        {
            CalcToEnts = calcToEnts;
            Count = count;
        }

        public List<CalculationToEnterprise> CalcToEnts { get; set; }
        public int Count { get; set; }
    }
}
