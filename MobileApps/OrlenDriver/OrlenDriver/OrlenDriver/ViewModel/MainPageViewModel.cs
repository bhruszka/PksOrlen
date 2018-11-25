using OrlenDriver.Interface;
using OrlenDriver.Services;
using OrlenDriver.ViewModel.Base;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace OrlenDriver.ViewModel
{
    internal class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(INavService navService) : base(navService)
        {

        }


        public override async Task InitAsync()
        {
            if (!await AskForPermissionAsync())
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                await ScanAsync();
            }
        }
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
                    await App.Current.MainPage.DisplayAlert("Camera Denied", "Can not continue, try again.", "OK");
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
        private async Task ScanAsync()
        {


            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                AppProperties.SetToken(result.ToString());
                await NavService.NavigateToViewModel<MapPageViewModel>(false);
            }
                
        }
    }
   
}
