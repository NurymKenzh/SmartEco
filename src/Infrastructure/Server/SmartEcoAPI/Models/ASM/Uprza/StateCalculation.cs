using System;
using System.Collections.Generic;

namespace SmartEcoAPI.Models.ASM.Uprza
{
    public class StateCalculation
    {
        public StateCalculation()
        {
            DiagnosticInfo = new DiagnosticInfo();
        }

        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int JobId { get; set; }

        public DateTime InitializedOn { get; set; }
        public List<string> Description { get; set; }


        //ComplexType
        public DiagnosticInfo DiagnosticInfo { get; set; }
    }

    public class DiagnosticInfo
    {
        public int Progress { get; set; }
        public double CalculationTime { get; set; }
        public double AverageTime { get; set; }
        public int NumberOfPoints { get; set; }
        public int NumberOfThreads { get; set; }
        public int NumberOfIterations { get; set; }
    }
}
