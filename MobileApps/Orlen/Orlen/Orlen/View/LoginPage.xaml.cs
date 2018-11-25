using Orlen.Interfaces;
using Orlen.ViewModel;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Orlen.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        LoginPageViewModel _viewModel
        {
            get { return BindingContext as LoginPageViewModel; }
        }


        double promptX = 0;
        double promptY = 0;

        public LoginPage()
        {
            InitializeComponent();
            promptX = PromptFrame.X;
            promptY = PromptFrame.Y;
            NavigationPage.SetHasNavigationBar(this, false);
            MainStack.RaiseChild(MainFrame);
            BindingContext = new LoginPageViewModel(DependencyService.Get<INavService>());

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
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<string>("OrlenLogin", "animateStart", async (sender) => {
                System.Diagnostics.Debug.WriteLine("animation start message");
                await AnimateLoginStart();
            });
            MessagingCenter.Subscribe<string>("OrlenLogin", "animateStop", async (sender) => {
                await AnimateLoginStop();
            });
            MessagingCenter.Subscribe<string, string>("OrlenLogin", "scalePromptUp", async (sender, arg) => {
                await AnimatePrompt(arg);
            });

            MessagingCenter.Subscribe<string>("OrlenLogin", "wrongLogin", async (sender) => {
                await AnimateLoginAsync();
            });
            MessagingCenter.Subscribe<string>("OrlenLogin", "wrongPassword", async (sender) => {
                await AnimatePasswordAsync();
            });
            if (_viewModel != null)
                await _viewModel.InitAsync();

        }
        private async Task AnimateLoginStop()
        {

        }
        private async Task AnimateLoginStart()
        {
            await Task.Run(() =>
            {
                LoginEntry.FadeTo(0, 500, Easing.Linear);
                PasswordEntry.FadeTo(0, 500, Easing.Linear);
                LoginButton.FadeTo(0, 500, Easing.Linear);
            });


            await LogoImage.TranslateTo(0, 50, 1000, Easing.Linear);
            while ((BindingContext as LoginPageViewModel).Busy)
            {
                await LogoImage.FadeTo(1, 600);
                await LogoImage.FadeTo(0, 600);

            }
        }
        private async Task AnimateLoginAsync()
        {

            LoginEntry.TextColor = Color.Red;
            LoginEntry.PlaceholderColor = Color.Red;

            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(0, 0, 100, Easing.Linear);
            LoginEntry.PlaceholderColor = Color.LightGray;

            LoginEntry.TextColor = Color.LightGray;

        }
        private async Task AnimatePasswordAsync()
        {
            PasswordEntry.TextColor = Color.Red;
            PasswordEntry.PlaceholderColor = Color.Red;

            await PasswordEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await PasswordEntry.TranslateTo(0, 0, 100, Easing.Linear);
            PasswordEntry.PlaceholderColor = Color.LightGray;
            PasswordEntry.TextColor = Color.LightGray;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>("OrlenLogin", "animateStart");
            MessagingCenter.Unsubscribe<string>("OrlenLogin", "animateStop");
            MessagingCenter.Unsubscribe<string>("OrlenLogin", "scalePromptUp");
            MessagingCenter.Unsubscribe<string>("OrlenLogin", "screenAppearStart");
            MainFrame.TranslateTo(0, -2000, 1000, Easing.Linear);

        }

        private async Task AnimatePrompt(string arg)
        {
            await AnimateLoginStop();
            PromptLabel.Text = arg;
            LoginButton.IsEnabled = false;
            LoginEntry.TextColor = Color.Red;
            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(-10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(10, 0, 100, Easing.Linear);
            await LoginEntry.TranslateTo(0, 0, 100, Easing.Linear);
            await ArrowFrame.TranslateTo(0, 0, 1000, Easing.Linear);
            await PromptFrame.ScaleTo(1, 500, Easing.Linear);

            await Task.Delay(5000);

            await PromptFrame.ScaleTo(0, 500, Easing.Linear);
            await ArrowFrame.TranslateTo(0, 2000, 1000, Easing.Linear);
            LoginEntry.TextColor = Color.Black;
            LoginButton.IsEnabled = true;

        }
    }
}