using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace OrlenDriver.Services
{
    public static class AppProperties
    {
        private const string PROPERTIES_TOKEN = "URL";


        public static string GetToken()
        {
            return Application.Current.Properties[PROPERTIES_TOKEN].ToString();

        }
        public static void SetToken(string token)
        {
            Application.Current.Properties[PROPERTIES_TOKEN] = token;

        }
    }
}
