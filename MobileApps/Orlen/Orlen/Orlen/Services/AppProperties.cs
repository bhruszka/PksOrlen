using Orlen.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Orlen.Services
{
    public class AppProperties : IAppProperties
    {
        private const string PROPERTIES_TOKEN = "TOKEN";

       
        public string GetToken()
        {
            return Application.Current.Properties[PROPERTIES_TOKEN].ToString();

        }
        public void SetToken(string token)
        {
            Application.Current.Properties[PROPERTIES_TOKEN] = token;

        }
    }
}
