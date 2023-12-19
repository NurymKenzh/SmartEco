namespace ServiceManager.Options
{
    public class ServicesOptions
    {
        public required ReporterOption Reporter { get; set; }
    }

    public class ReporterOption : BaseOptions;
}
