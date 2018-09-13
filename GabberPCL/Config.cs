namespace GabberPCL
{
    public static class Config
    {
        static readonly string BASE_URL = "dev.gabber.audio";

        public static string WEB_URL = "https://" + BASE_URL;
        public static string PRINT_URL = "www." + BASE_URL;
        public static string ABOUT_URL = WEB_URL + "/about/";
        public static string ABOUT_DATA_PAGE = WEB_URL + "/research";

        public static string API_ENDPOINT = "https://api." + BASE_URL;

        public static string DATABASE_NAME = "gabber.db3";
    }
}
