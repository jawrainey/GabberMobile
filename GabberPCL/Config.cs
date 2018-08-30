namespace GabberPCL
{
    public static class Config
    {
        private static string base_url = "dev.gabber.audio";

        public static string WEB_URL = "https://" + base_url;
        public static string PRINT_URL = "www." + base_url;
        public static string ABOUT_DATA_PAGE = WEB_URL + "/research";

        public static string API_ENDPOINT = "https://9899a954.ngrok.io";//"https://api." + base_url;

        public static string DATABASE_NAME = "gabber.db3";
    }
}