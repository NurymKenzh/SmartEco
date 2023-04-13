using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Среда загрязнения.
    /// </summary>
    public class PollutionEnvironment
    {
        /// <summary>
        /// Id среды загрязнения.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название на казахском языке.
        /// </summary>
        public string NameKK { get; set; }
        /// <summary>
        /// Название на русском языке.
        /// </summary>
        public string NameRU { get; set; }
        /// <summary>
        /// Название на английском языке.
        /// </summary>
        public string NameEN { get; set; }
    }
}
