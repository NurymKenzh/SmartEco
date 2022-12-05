namespace SmartEco.Web.Helpers.Constants
{
    public static class Views
    {
        private static string _views = "~/Views";
        private static string _extension = ".cshtml";

        public static string Get(HttpContext httpContext, params string[] subfolders)
        {
            string controller = httpContext.Session.GetString("controller");
            string action = httpContext.Session.GetString("action");
            var pathSubfolders = string.Join("/", subfolders);
            var viewPath = $"{_views}/" +
                $"{pathSubfolders}/" +
                $"{controller}/" +
                $"{action}{_extension}";
            return viewPath;
        }
    }
}
