namespace SmartEco.Web.Helpers.Constants
{
    public static class Views
    {
        private static readonly string _views = "~/Views";
        private static readonly string _extension = ".cshtml";

        public static string? Get(HttpContext httpContext, params string[] subfolders)
        {
            var controller = httpContext.Session.GetString("controller");
            var action = httpContext.Session.GetString("action");
            var pathSubfolders = string.Join("/", subfolders);
            var viewPath = $"{_views}/" +
                $"{pathSubfolders}/" +
                $"{controller}/" +
                $"{action}{_extension}";
            return viewPath;
        }
    }
}
