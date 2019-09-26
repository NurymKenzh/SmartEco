using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Измеренные данные со станции мониторинга.
    /// </summary>
    public class MeasuredData
    {
        /// <summary>
        /// Id измеренных данных.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id измеряемого параметра.
        /// </summary>
        public int MeasuredParameterId { get; set; }
        public MeasuredParameter MeasuredParameter { get; set; }

        /// <summary>
        /// Дата и время получения измеренных данных.
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// Значение измеренных данных.
        /// </summary>
        public decimal? Value { get; set; }

        /// <summary>
        /// Дата и время измеряемых данных Ecomon в формате количества секунд.
        /// </summary>
        public long? Ecomontimestamp_ms { get; set; }

        /// <summary>
        /// Год измеряемых данных. Используется для нерегулярных измеряемых данных.
        /// </summary>
        public int? Year { get; set; }
        /// <summary>
        /// Месяц измеряемых данных. Используется для нерегулярных измеряемых данных.
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Год измеряемых данных.
        /// </summary>
        public int? DateYear
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Year;
                }
                else
                {
                    return Year;
                }
            }
        }

        /// <summary>
        /// Месяц измеряемых данных.
        /// </summary>
        public int? DateMonth
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Month;
                }
                else
                {
                    return Month;
                }
            }
        }

        /// <summary>
        /// День измеряемых данных.
        /// </summary>
        public int? DateDay
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Day;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Час измеряемых данных.
        /// </summary>
        public int? DateHour
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Hour;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Минута измеряемых данных.
        /// </summary>
        public int? DateMinute
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Minute;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Секунда измеряемых данных.
        /// </summary>
        public int? DateSecond
        {
            get
            {
                if (DateTime != null)
                {
                    return DateTime.Value.Second;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Месяц максимального значения за год.
        /// </summary>
        public int? MaxValueMonth { get; set; } // per year
        /// <summary>
        /// День максимального значения за месяц.
        /// </summary>
        public int? MaxValueDay { get; set; } // per month

        /// <summary>
        /// Максимальное значение за год.
        /// </summary>
        public decimal? MaxValuePerYear { get; set; }
        /// <summary>
        /// Максимальное значение за месяц.
        /// </summary>
        public decimal? MaxValuePerMonth { get; set; }

        /// <summary>
        /// Id поста мониторинга.
        /// </summary>
        public int? MonitoringPostId { get; set; }
        public MonitoringPost MonitoringPost { get; set; }

        public int? PollutionSourceId { get; set; }
        public PollutionSource PollutionSource { get; set; }

        public bool? Averaged { get; set; }
    }
}
