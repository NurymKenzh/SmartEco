using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Поставщик данных.
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// Id поставщика данных.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название поставщика данных.
        /// </summary>
        public string Name { get; set; }
    }
}
