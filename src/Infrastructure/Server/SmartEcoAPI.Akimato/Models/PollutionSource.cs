using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    /// <summary>
    /// Источник выделения загрязнения.
    /// </summary>
    public class PollutionSource
    {
        /// <summary>
        /// Id источника выделения загрязнения.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название источника выделения загрязнения.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Северная широта источника выделения загрязнения.
        /// </summary>
        public decimal NorthLatitude { get; set; }
        /// <summary>
        /// Восточная долгота источника выделения загрязнения.
        /// </summary>
        public decimal EastLongitude { get; set; }
    }
}
