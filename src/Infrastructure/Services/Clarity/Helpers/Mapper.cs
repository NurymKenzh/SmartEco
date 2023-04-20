using Clarity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clarity.Helpers
{
    public static class Mapper
    {
        private static readonly Dictionary<Type, int> TypeParameters = new Dictionary<Type, int>
        {
            {typeof(Pm25ConcMass), 2},
            {typeof(Pm10ConcMass), 3},
            {typeof(Temperature), 4},
            {typeof(No2Conc), 13},
            {typeof(RelHumid), 19}
        };

        private static readonly Dictionary<Type, bool> IsPollution = new Dictionary<Type, bool>
        {
            {typeof(Pm25ConcMass), true},
            {typeof(Pm10ConcMass), true},
            {typeof(Temperature), false},
            {typeof(No2Conc), true},
            {typeof(RelHumid), false}
        };

        public static List<MeasuredData> DataClarityToSmartEco(
            List<ClarityMeasurement> clarityMeasurements, 
            List<MonitoringPost> monitoringPosts)
        {
            var measuredDatas = new List<MeasuredData>();
            foreach (var clarityMeasurement in clarityMeasurements)
            {
                var monitoringPost = monitoringPosts
                    .Where(m => m.Name == clarityMeasurement.DeviceCode)
                    .FirstOrDefault();
                if (monitoringPost != null)
                {
                    if (clarityMeasurement.Characteristics?.Pm2_5ConcMass != null)
                        measuredDatas.Add(ToMeasuredData(clarityMeasurement.Characteristics.Pm2_5ConcMass, clarityMeasurement.Time, monitoringPost.Id));
                    if (clarityMeasurement.Characteristics?.Pm10ConcMass != null)
                        measuredDatas.Add(ToMeasuredData(clarityMeasurement.Characteristics.Pm10ConcMass, clarityMeasurement.Time, monitoringPost.Id));
                    if (clarityMeasurement.Characteristics?.Temperature != null)
                        measuredDatas.Add(ToMeasuredData(clarityMeasurement.Characteristics.Temperature, clarityMeasurement.Time, monitoringPost.Id));
                    if (clarityMeasurement.Characteristics?.No2Conc != null)
                        measuredDatas.Add(ToMeasuredData(clarityMeasurement.Characteristics.No2Conc, clarityMeasurement.Time, monitoringPost.Id));
                    if (clarityMeasurement.Characteristics?.RelHumid != null)
                        measuredDatas.Add(ToMeasuredData(clarityMeasurement.Characteristics.RelHumid, clarityMeasurement.Time, monitoringPost.Id));
                }
            }
            return measuredDatas;
        }

        private static MeasuredData ToMeasuredData(BaseMeasurement baseMeasurement, DateTime time, int monitoringPostId)
            => new MeasuredData
            {
                DateTime = TimeZoneConverter.ToCentralAsia(time),
                MeasuredParameterId = TypeParameters[baseMeasurement.GetType()],
                MonitoringPostId = monitoringPostId,
                Value = IsPollution[baseMeasurement.GetType()] ? baseMeasurement.Value * 0.001m : baseMeasurement.Value,
                Averaged = true
            };
    }
}
