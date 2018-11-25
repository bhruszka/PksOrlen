using Orlen.CustomRenderers;
using Orlen.Interfaces;
using Orlen.Services;
using Orlen.ViewModel.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Orlen.ViewModel
{
    internal class MapPageViewModel : BaseViewModel
    {
        public List<Position> RoutePoints { get => routePoints; set => routePoints = value; }

        private List<Position> routePoints;

        public RouteMap Map { get; set; }
        Command _addCommand;
        public Command AddCommand => _addCommand ??
            (_addCommand = new Command(() => AddPoint()));

        private void AddPoint()
        {

            
        }

        public MapPageViewModel(INavService navService) : base(navService)
        {
            DbService = new DatabaseRepository();

        }
        public override async Task InitAsync()
        {

            await Task.Delay(1000);
            Device.BeginInvokeOnMainThread(async () =>
            {
               
                var data = DbService.GetAllBusStops();
                var list = new List<Pin>();
                int i = 1;
                foreach(var item in data)
                {
                    var position = new Position(item.Lat, item.Long);

                    list.Add(new Pin {
                        Label = "Przystanek " + i.ToString(),
                        Position = position,
                        Address = item.TimeIN

                    });
                    i++;
                }
                Map.Pins = list;
               
            });


            Plugin.Geolocator.Abstractions.Position pos;
            if (!LocationService.IsLocationAvailable())
            {
                await App.Current.MainPage.DisplayAlert("Brak lokalizacji", "Proszę włączyć lokalizację GPS", "OK");
                pos = new Plugin.Geolocator.Abstractions.Position(52.588262, 19.67104);
            }
            else
            {
                pos = await LocationService.GetCurrentPosition();
            }
            var MapPosition = new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude);
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapPosition, Distance.FromMiles(1.0)));



        }
    }
}
