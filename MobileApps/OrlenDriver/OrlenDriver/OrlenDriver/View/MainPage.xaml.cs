using OrlenDriver.Interface;
using OrlenDriver.View;
using OrlenDriver.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OrlenDriver
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel _viewModel
        {
            get { return BindingContext as MainPageViewModel; }
        }
        Page page;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(DependencyService.Get<INavService>());
            NavigationPage.SetHasNavigationBar(this, false);





        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
                await _viewModel.InitAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert(
                                       "Uwaga",
                                        "Czy chcesz zamknąć aplikację",
                                        "OK", "ANULUJ");


                if (result)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            });
            return true;
        }
    }
}
