using OrlenDriver.Interface;
using OrlenDriver.Services;
using OrlenDriver.View;
using OrlenDriver.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OrlenDriver
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var mainPage = new NavigationPage(new MainPage());
            NavigationPage.SetHasNavigationBar(mainPage, false);
            var navService = DependencyService.Get<INavService>() as NavService;
            navService.navigation = mainPage.Navigation;
            navService.RegisterViewMapping(typeof(MainPageViewModel), typeof(MainPage));
            navService.RegisterViewMapping(typeof(MapPageViewModel), typeof(MapPage));
            App.Current.MainPage = mainPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
