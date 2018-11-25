using Orlen.Interfaces;
using Orlen.Model;
using Orlen.Services;
using Orlen.ViewModel.Base;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Orlen.ViewModel
{
    public class LoginPageViewModel : BaseViewModel
    {


        User _user;
        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        public LoginPageViewModel(INavService navService) : base(navService)
        {
            LoginService = new LoginService();
            DbService = new DatabaseRepository();
        }
        public override async Task InitAsync()
        {
            NavService.ClearAllViewsFromStack();
            DbService.ClearDatabase();

            User = new User();

#if DEBUG
            var userTest = new User
            {
                Login = "admin",
                Password = "123"
            };
            User = userTest;
#endif

        }
        Command _loginCommand;
        public Command LoginCommand => _loginCommand ??
            (_loginCommand = new Command(async () => await Login(User)));
        private async Task<bool> AskForPermissionAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await App.Current.MainPage.DisplayAlert("Kamera", "Wymagany dostęp do Kamery", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Camera))
                        status = results[Permission.Camera];
                }


                if (status != PermissionStatus.Granted)
                {
                    await App.Current.MainPage.DisplayAlert("Kamera Denied", "Can not continue, try again.", "OK");
                    return false;
                }
                

                status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await App.Current.MainPage.DisplayAlert("Lokalizacja", "Wymagany dostęp do lokalizacji", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await App.Current.MainPage.DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task Login(User user)
        {
            if (!await AskForPermissionAsync()) return;
            if (string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Password))
            {
                if (string.IsNullOrEmpty(user.Login))
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string>("OrlenLogin", "wrongLogin"));
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string>("OrlenLogin", "wrongPassword"));
                }
                return;
            }

            Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string>("OrlenLogin", "animateStart"));

            Busy = true;

            WebStatusData webstatus = new WebStatusData();
            if (user != null)
            {
                webstatus = await LoginService.LoginAsync(user);
            }
            if (webstatus != null)
            {
                if (!string.IsNullOrEmpty(webstatus.Token))
                {
                    AppPropertiesService.SetToken(webstatus.Token);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>("OrlenLogin", "scalePromptUp", "Zły login lub hasło"));
                Busy = false;
                return;
            }
            //await Task.Delay(4000);
                await FetchApplicationDataAsync();

            await NavService.NavigateToViewModel<TimetablePageViewModel>(false);

            //if (webstatus.Status.Equals(Status.OK))
            //{
            //    AppPropertiesService.SetUserProperties(webstatus.Token, webstatus.Client_id, webstatus.User_id);
            //    UserDataService.SaveUserData(user.Login, user.Password);
            //    DataService = new EkajetDataService(webstatus.Token, webstatus.User_id, webstatus.Client_id);
            //    await NavService.NavigateToViewModelMasterDetail<MainPageViewModel>(false, Variables.Variables.CALENDAR);
            //    DependencyService.Get<IMessage>().LongAlert(string.Format("Witaj {0}", UserDataService.GetUserLogin()));


            //}
            //else
            //{
            //    if (webstatus.Errcode.Equals("10008") || webstatus.Errcode.Equals("10006"))
            //    {
            //        Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>("OrlenLogin", "scalePromptUp", "Zły login lub hasło"));
            //        return;
            //    }
            //    else
            //    {
            //        Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<string, string>("OrlenLogin", "scalePromptUp", "Nieznany bład"));
            //        return;
            //    }
            //}


            //Busy = false;
            //Device.BeginInvokeOnMainThread(() => MessagingCenter.Send("OrlenLogin", "animateStop"));
        }
        private async Task FetchApplicationDataAsync()
        {
            var webService = new OrlenWebService();
            var data = await webService.GetBusStopsAsync(AppPropertiesService.GetToken());
            foreach (var item in data)
            {
                DbService.InserBusStop(item);
            }
            //var salon = await DataService.GetSalonAsync();
            //DbService.InsertSalon(salon);
            //var clients = await DataService.GetClientsAsync();
            //foreach (var item in clients)
            //{
            //    DbService.InsertClient(item);
            //}
        }
        ILoginService _loginService;
        public ILoginService LoginService
        {
            get { return _loginService; }
            set
            {
                _loginService = value;
                OnPropertyChanged();
            }
        }
    }
}
