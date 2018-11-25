using Orlen.CustomRenderers;
using Orlen.Interfaces;
using Orlen.Model;
using Orlen.Services;
using Orlen.ViewModel.Base;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Position = Plugin.Geolocator.Abstractions.Position;

namespace Orlen.ViewModel
{
    public class LinePageViewModel : BaseViewModel<Line>
    {
        public RouteMap Map { get; set; }
        public List<TimeTableLineItem> LineList { get { return lineList; } set { lineList = value; OnPropertyChanged(); } }
        private List<TimeTableLineItem> lineList;
        public LinePageViewModel(INavService navService) : base(navService)
        {
            LineList = new List<TimeTableLineItem>();
        }
        TimeTableLineItem selectedItem;
        public TimeTableLineItem SelectedItem { get { return selectedItem; }
            set {
                selectedItem = value;
                OnPropertyChanged();
                OnLineItemSelected();
            } }

        private void OnLineItemSelected()
        {
            if(SelectedItem!= null)
            {
                var MapPosition = new Xamarin.Forms.Maps.Position(SelectedItem.Lat, SelectedItem.Long);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapPosition, Distance.FromMiles(1.0)));
                });
            }
        }

        public override async Task InitAsync(Line param)
        {
            if (param != null)
            {
                LineList = param.lineItems;
                int i = 1;
                foreach (var item in LineList)
                {
                    item.BusStopName = "Przystanek " + i.ToString();
                    i++;

                }
                await Task.Factory.StartNew(async () =>
                {
                    Position pos;
                    if (!LocationService.IsLocationAvailable())
                    {
                        await App.Current.MainPage.DisplayAlert("Brak lokalizacji", "Proszę włączyć lokalizację GPS", "OK");
                        pos = new Position(52.588262, 19.67104);
                    }
                    else
                    {
                        pos = await LocationService.GetCurrentPosition();
                    }
                    var MapPosition = new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var list = new List<Pin>();
                        foreach (var item in LineList)
                        {
                            var position = new Xamarin.Forms.Maps.Position(item.Lat, item.Long);
                            int k = 1;
                            list.Add(new Pin
                            {
                                Label = "Przystanek " + k.ToString(),
                                Position = position,
                                Address = item.TimeIN

                            });
                            k++;
                        }
                        Map.Pins = list;
                        Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapPosition, Distance.FromMiles(1.0)));
                    });

                });
            }
        }
    }
}
