using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartEco.Models.ASM.Requests.Uprza
{
    public class UprzaRequest
    {
        public UprzaRequest()
        {
            AirPollutionSources = new List<AirPollutionSource>();
            Buildings = new List<object>();
        }

        [JsonProperty(PropertyName = "threshold_pdk")]
        public double ThresholdPdk { get; set; }

        [JsonProperty(PropertyName = "locality", Required = Required.Always)]
        public Locality Locality { get; set; }

        [JsonProperty(PropertyName = "meteo", Required = Required.Always)]
        public Meteo Meteo { get; set; }

        [JsonProperty(PropertyName = "background", Required = Required.Always)]
        public Background Background { get; set; }

        [JsonProperty(PropertyName = "method", Required = Required.Always)]
        public int Method { get; set; }

        [JsonProperty(PropertyName = "contributor_count")]
        public int ContributorCount { get; set; }

        [JsonProperty(PropertyName = "use_summation_groups", Required = Required.Always)]
        public bool UseSummationGroups { get; set; }

        [JsonProperty(PropertyName = "air_pollution_sources")]
        public List<AirPollutionSource> AirPollutionSources { get; set; }

        [JsonProperty(PropertyName = "calculated_area", Required = Required.Always)]
        public CalculatedArea CalculatedArea { get; set; }

        [JsonProperty(PropertyName = "buildings")]
        public List<object> Buildings { get; set; }

        [JsonProperty(PropertyName = "pollutants")]
        public List<Pollutant> Pollutants { get; set; }

        [JsonProperty(PropertyName = "season")]
        public int Season { get; set; }
    }
    public class Locality
    {
        [JsonProperty(PropertyName = "square", Required = Required.Always)]
        public int Square { get; set; }

        [JsonProperty(PropertyName = "relief_coefficient", Required = Required.Always)]
        public int ReliefCoefficient { get; set; }

        [JsonProperty(PropertyName = "stratification_coefficient", Required = Required.Always)]
        public int StratificationCoefficient { get; set; }
    }

    public class WindSpeedSettings
    {
        [JsonProperty(PropertyName = "mode")]
        public int? Mode { get; set; }

        [JsonProperty(PropertyName = "speed")]
        public float? Speed { get; set; }

        [JsonProperty(PropertyName = "start_speed")]
        public float? StartSpeed { get; set; }

        [JsonProperty(PropertyName = "end_speed")]
        public float? EndSpeed { get; set; }

        [JsonProperty(PropertyName = "step_speed")]
        public float? StepSpeed { get; set; }

        [JsonProperty(PropertyName = "speeds")]
        public List<float?> Speeds { get; set; }
    }

    public class WindDirectionSettings
    {
        [JsonProperty(PropertyName = "mode")]
        public int? Mode { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public float? Direction { get; set; }

        [JsonProperty(PropertyName = "start_direction")]
        public float? StartDirection { get; set; }

        [JsonProperty(PropertyName = "end_direction")]
        public float? EndDirection { get; set; }

        [JsonProperty(PropertyName = "step_direction")]
        public float? StepDirection { get; set; }

        [JsonProperty(PropertyName = "directions")]
        public List<float?> Directions { get; set; }
    }

    public class Meteo
    {
        [JsonProperty(PropertyName = "temperature", Required = Required.Always)]
        public float Temperature { get; set; }

        [JsonProperty(PropertyName = "wind_speed_settings", Required = Required.Always)]
        public WindSpeedSettings WindSpeedSettings { get; set; }

        [JsonProperty(PropertyName = "wind_direction_settings", Required = Required.Always)]
        public WindDirectionSettings WindDirectionSettings { get; set; }

        [JsonProperty(PropertyName = "u_speed", Required = Required.Always)]
        public double USpeed { get; set; }
    }

    public class Background
    {
        [JsonProperty(PropertyName = "mode", Required = Required.Always)]
        public int Mode { get; set; }

        //[JsonProperty(PropertyName = "value_type")]
        //public int? ValueType { get; set; }
    }

    public class Point1
    {
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "z")]
        public double Z { get; set; }
    }

    public class Configuration
    {
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "height")]
        public double Height { get; set; }

        [JsonProperty(PropertyName = "diameter")]
        public double Diameter { get; set; }

        [JsonProperty(PropertyName = "flow_temperature")]
        public double FlowTemperature { get; set; }

        [JsonProperty(PropertyName = "flow_speed")]
        public double FlowSpeed { get; set; }

        [JsonProperty(PropertyName = "thermal_power")]
        public double? ThermalPower { get; set; }

        [JsonProperty(PropertyName = "radiation_power")]
        public double? RadiationPower { get; set; }

        [JsonProperty(PropertyName = "emission_density")]
        public double? EmissionDensity { get; set; }

        [JsonProperty(PropertyName = "point_1")]
        public Point1 Point1 { get; set; }

        [JsonProperty(PropertyName = "relief_coefficient")]
        public int ReliefCoefficient { get; set; }

        [JsonProperty(PropertyName = "is_covered")]
        public bool IsCovered { get; set; }

        [JsonProperty(PropertyName = "is_gas")]
        public bool IsGas { get; set; }

        [JsonProperty(PropertyName = "is_vertical_deviation")]
        public bool IsVerticalDeviation { get; set; }

        [JsonProperty(PropertyName = "is_torch")]
        public bool IsTorch { get; set; }

        [JsonProperty(PropertyName = "vertical_deviation")]
        public double? VerticalDeviation { get; set; }

        [JsonProperty(PropertyName = "rotation_angle")]
        public double? RotationAngle { get; set; }
    }

    public class Emission
    {
        [JsonProperty(PropertyName = "pollutant_code")]
        public int PollutantCode { get; set; }

        [JsonProperty(PropertyName = "power")]
        public double Power { get; set; }

        [JsonProperty(PropertyName = "coefficient")]
        public int Coefficient { get; set; }
    }

    public class AirPollutionSource
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "is_organized")]
        public bool IsOrganized { get; set; }

        [JsonProperty(PropertyName = "methodical")]
        public int Methodical { get; set; }

        [JsonProperty(PropertyName = "background_relation")]
        public int BackgroundRelation { get; set; }

        [JsonProperty(PropertyName = "configuration")]
        public Configuration Configuration { get; set; }

        [JsonProperty(PropertyName = "emissions")]
        public List<Emission> Emissions { get; set; }

        [JsonProperty(PropertyName = "humidity")]
        public double? Humidity { get; set; }

        [JsonProperty(PropertyName = "pressure")]
        public double? Pressure { get; set; }
    }

    public class CenterPoint
    {
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "z")]
        public int Z { get; set; }
    }

    public class Point
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }

        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }

        [JsonProperty(PropertyName = "z")]
        public int Z { get; set; }
    }

    public class Rectangle
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "center_point")]
        public CenterPoint CenterPoint { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "step_by_width")]
        public int StepByWidth { get; set; }

        [JsonProperty(PropertyName = "step_by_length")]
        public int StepByLength { get; set; }
    }

    public class CalculatedArea
    {
        public CalculatedArea()
        {
            Rectangles = new List<Rectangle>();
            Points = new List<Point>();
            Lines = new List<object>();
        }

        [JsonProperty(PropertyName = "rectangles")]
        public List<Rectangle> Rectangles { get; set; }

        [JsonProperty(PropertyName = "points")]
        public List<Point> Points { get; set; }

        [JsonProperty(PropertyName = "lines")]
        public List<object> Lines { get; set; }

        [JsonProperty(PropertyName = "living_area_border_step")]
        public double? LivingAreaBorderStep { get; set; }

        [JsonProperty(PropertyName = "sanitary_area_border_step")]
        public double? SanitaryAreaBorderStep { get; set; }

        [JsonProperty(PropertyName = "unit_border_step")]
        public double? UnitBorderStep { get; set; }
    }

    public class Pollutant
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "pdk")]
        public double Pdk { get; set; }

        [JsonProperty(PropertyName = "pdk_long")]
        public double? PdkLong { get; set; }
    }
}
