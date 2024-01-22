using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.Uprza
{
    public class StateCalculation
    {
        public StateCalculation() 
        {
            DiagnosticInfo = new DiagnosticInfo();
        }

        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        [Display(Name = "№ п/п")]
        public int JobId { get; set; }

        [Display(Name = "Время инициализации")]
        public DateTime InitializedOn { get; set; }
        public List<string> Description { get; set; }


        //ComplexType
        public DiagnosticInfo DiagnosticInfo { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class DiagnosticInfo
    {
        [Display(Name = "Прогресс")]
        public int Progress { get; set; }
        [Display(Name = "Время расчёта, с")]
        public double CalculationTime { get; set; }
        [Display(Name = "Среднее время для расчёта одной точки, с")]
        public double AverageTime { get; set; }
        [Display(Name = "Количество точек")]
        public int NumberOfPoints { get; set; }
        [Display(Name = "Количество потоков")]
        public int NumberOfThreads { get; set; }
        [Display(Name = "Количество переборов")]
        public int NumberOfIterations { get; set; }
    }
}
