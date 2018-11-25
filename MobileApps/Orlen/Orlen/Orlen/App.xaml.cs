using Orlen.Interfaces;
using Orlen.Services;
using Orlen.ViewModel;
using Orlen.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FirebasePushNotification;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Orlen
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var mainPage = new NavigationPage(new LoginPage());
            NavigationPage.SetHasNavigationBar(mainPage, false);
            var navService = DependencyService.Get<INavService>() as NavService;
            navService.navigation = mainPage.Navigation;
            navService.RegisterViewMapping(typeof(LoginPageViewModel), typeof(LoginPage));
            navService.RegisterViewMapping(typeof(MapPageViewModel), typeof(MapPage));
            navService.RegisterViewMapping(typeof(LinePageViewModel), typeof(LinePage));
            navService.RegisterViewMapping(typeof(TimetablePageViewModel), typeof(TimetablePage));
            App.Current.MainPage = mainPage;

        }

        protected override void OnStart()
        {
            //CrossFirebasePushNotification.Current.Subscribe("general");
            //CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            //{
             
            //};
            //System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");

            //CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            //{
            //    try
            //    {
            //        System.Diagnostics.Debug.WriteLine("Received");
                    
            //    }
            //    catch (Exception)
            //    {

            //    }

            //};

            //CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            //{

            //    System.Diagnostics.Debug.WriteLine("Opened with ID: " + p.Identifier);
            //    foreach (var data in p.Data)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            //    }
                
            //};
            //CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            //{
            //    System.Diagnostics.Debug.WriteLine("Dismissed");
            //};
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
