using System;
using System.Collections.Generic;
using System.Text;

namespace Clarity.Models
{
    public class ClarityMeasurement
    {
        public string OutputFrequency { get; set; }
        public string _Id { get; set; }
        public string Average { get; set; }
        public Characteristics Characteristics { get; set; }
        public string RecId { get; set; }
        public DateTime Time { get; set; }
        public string DeviceCode { get; set; }
        public Location Location { get; set; }
        public string DatasourceType { get; set; }
    }

    public class Characteristics
    {
        public RelHumid RelHumid { get; set; }
        public Temperature Temperature { get; set; }
        public Pm25ConcMass Pm2_5ConcMass { get; set; }
        public Pm10ConcMass Pm10ConcMass { get; set; }
        public No2Conc No2Conc { get; set; }
    }

    public class Location
    {
        public List<decimal> Coordinates { get; set; }
        public string Type { get; set; }
    }

    public class No2Conc : BaseConcMass { }

    public class Pm10ConcMass : BaseConcMass { }

    public class Pm25ConcMass : BaseConcMass { }

    public class RelHumid : BaseMeasurement { }

    public class Temperature: BaseMeasurement { }

    public class BaseConcMass : BaseMeasurement
    {
        public decimal CalibratedValue { get; set; }
    }

    public class BaseMeasurement
    {
        public decimal Value { get; set; }
        public int Weight { get; set; }
        public decimal Raw { get; set; }
    }
}
