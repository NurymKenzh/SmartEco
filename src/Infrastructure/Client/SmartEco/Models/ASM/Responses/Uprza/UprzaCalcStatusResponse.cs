using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace SmartEco.Models.ASM.Responses.Uprza
{
    [JsonConverter(typeof(DateParseHandlingConverter), DateParseHandling.None)]
    public class UprzaCalcStatusResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("started_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset StartedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("diagnostic_data")]
        public DiagnosticData DiagnosticData { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("description")]
        public string[] Description { get; set; }

        [JsonProperty("daemon_id")]
        public object DaemonId { get; set; }
    }

    public class DiagnosticData
    {
        [JsonProperty("count_of_points")]
        public int CountOfPoints { get; set; }

        [JsonProperty("avg_time")]
        public double AvgTime { get; set; }

        [JsonProperty("count_of_threads")]
        public int CountOfThreads { get; set; }

        [JsonProperty("count_of_busts")]
        public int CountOfBusts { get; set; }

        [JsonProperty("error_info")]
        public string ErrorInfo { get; set; }

        [JsonProperty("progress")]
        public int Progress { get; set; }

        [JsonProperty("calc_started_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset CalcStartedAt { get; set; }

        [JsonProperty("calc_finished_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset CalcFinishedAt { get; set; }

        [JsonProperty("transform_started_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset TransformStartedAt { get; set; }

        [JsonProperty("transform_finished_at")]
        [JsonConverter(typeof(FixedIsoDateTimeOffsetConverter))]
        public DateTimeOffset TransformFinishedAt { get; set; }

        [JsonProperty("timers")]
        public object Timers { get; set; }
    }

    public class DateParseHandlingConverter : JsonConverter
    {
        readonly DateParseHandling dateParseHandling;

        public DateParseHandlingConverter(DateParseHandling dateParseHandling)
        {
            this.dateParseHandling = dateParseHandling;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            var old = reader.DateParseHandling;
            try
            {
                reader.DateParseHandling = dateParseHandling;
                existingValue = existingValue ?? serializer.ContractResolver.ResolveContract(objectType).DefaultCreator();
                serializer.Populate(reader, existingValue);
                return existingValue;
            }
            finally
            {
                reader.DateParseHandling = old;
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class FixedIsoDateTimeOffsetConverter : IsoDateTimeConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
        }

        public FixedIsoDateTimeOffsetConverter() : base()
        {
            this.DateTimeStyles = DateTimeStyles.AssumeUniversal;
        }
    }
}
