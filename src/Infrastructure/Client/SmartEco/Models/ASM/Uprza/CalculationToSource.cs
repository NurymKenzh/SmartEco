using SmartEco.Models.ASM.Filsters;
using SmartEco.Models.ASM.PollutionSources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM.Uprza
{
    public class CalculationToSource
    {
        public int CalculationId { get; set; }
        public Calculation Calculation { get; set; }

        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }
    }

    public class PollutionSourceInvolved : AirPollutionSource
    {
        [Display(Name = "Участвует в расчёте рассеивания")]
        public bool IsInvolved { get; set; }
    }

    public class CalculationToSourcesInvolvedViewModel
    {
        public CalculationToSourcesFilter Filter { get; set; } = new CalculationToSourcesFilter();
        public List<PollutionSourceInvolved> Items { get; set; } = new List<PollutionSourceInvolved>();
        public Pager Pager { get; set; } = new Pager(null);

        public bool IsInvolvedAllSources { get; set; }
    }
}
