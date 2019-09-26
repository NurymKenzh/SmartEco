using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Пост мониторинга.
    /// </summary>
    public class MonitoringPost
    {
        /// <summary>
        /// Id поста мониторинга
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Номер поста мониторинга.
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Название поста мониторинга.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Северная широта поста мониторинга.
        /// </summary>
        public decimal NorthLatitude { get; set; }
        /// <summary>
        /// Восточная долгота поста мониторинга.
        /// </summary>
        public decimal EastLongitude { get; set; }
        /// <summary>
        /// Дополнительная информация о посте мониторинга.
        /// </summary>
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// MN код поста мониторинга.
        /// </summary>
        public string MN { get; set; }

        /// <summary>
        /// Id поставщика данных.
        /// </summary>
        public int DataProviderId { get; set; }
        public DataProvider DataProvider { get; set; }

        /// <summary>
        /// Id среды загрязнение.
        /// </summary>
        public int PollutionEnvironmentId { get; set; }
        public PollutionEnvironment PollutionEnvironment { get; set; }
    }
}
