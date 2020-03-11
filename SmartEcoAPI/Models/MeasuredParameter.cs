using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Измеряемый параметр.
    /// </summary>
    public class MeasuredParameter
    {
        /// <summary>
        /// Id измеряемого параметра.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Единица измерения.
        /// </summary>
        public int? MeasuredParameterUnitId { get; set; }
        public MeasuredParameterUnit MeasuredParameterUnit { get; set; }
        /// <summary>
        /// Название измеряемого параметра на казахском языке.
        /// </summary>
        public string NameKK { get; set; }
        /// <summary>
        /// Название измеряемого параметра на русском языке.
        /// </summary>
        public string NameRU { get; set; }
        /// <summary>
        /// Название измеряемого параметра на английском языке.
        /// </summary>
        public string NameEN { get; set; }
        /// <summary>
        /// Ecomon код измеряемого параметра.
        /// </summary>
        public int? EcomonCode { get; set; }
        /// <summary>
        /// Oceanus код измеряемого параметра.
        /// </summary>
        public string OceanusCode { get; set; }
        /// <summary>
        /// Kazhydromet код измеряемого параметра.
        /// </summary>
        public string KazhydrometCode { get; set; }
        /// <summary>
        ///Среднесуточное ПДК измеряемого параметра.
        /// </summary>
        public decimal? MPCDailyAverage { get; set; } // maximum permissible concentration
        /// <summary>
        /// Максимально разовое ПДК измеряемого параметра.
        /// </summary>
        public decimal? MPCMaxSingle { get; set; }
    }
}
