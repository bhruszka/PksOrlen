using OrlenDriver.CustomRenderers;
using OrlenDriver.Interface;
using OrlenDriver.Services;
using OrlenDriver.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace OrlenDriver.ViewModel
{
    internal class MapPageViewModel : BaseViewModel
    {
        public List<Position> RoutePoints { get => routePoints; set => routePoints = value; }
        public string KeepTrack {
            get { return keepTrack; }
            set { keepTrack = value;
                OnPropertyChanged();
            }
        }
        string keepTrack;
        private bool track = false;
        WebService service;
        private List<Position> routePoints;
        private CancellationTokenSource _tokenSource = null;
        private CancellationToken _token;
        private Task _executionTask = null;

        public RouteMap Map { get; set; }
        Command _trackCommand;
        public Command TrackCommand => _trackCommand ??
            (_trackCommand = new Command(async () => await AddPointAsync()));

        private async Task AddPointAsync()
        {
            if (!LocationService.IsLocationAvailable())
            {
                await App.Current.MainPage.DisplayAlert("Brak lokalizacji", "Proszę włączyć lokalizację GPS", "OK");
                return;
            }
            track = !track;
            if (track)
            {
                KeepTrack = " Nie podązaj";
                _executionTask = Task.Factory.StartNew(this.KeepTrackTask, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else
            {
                KeepTrack = "Podązaj";
                _tokenSource.Cancel();
            }

        }
        private Task KeepTrackTask()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        var pos = await LocationService.GetCurrentPosition();
                        var MapPosition = new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                           Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapPosition, Distance.FromMiles(0.5)));
                        });
                        await Task.Delay(3000);
                    }
                    catch (OperationCanceledException ex)
                    {
                        break;
                    }

                }

            });
        }
        public MapPageViewModel(INavService navService) : base(navService)
        {
            service = new WebService();
        }
        public override async Task InitAsync()
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            KeepTrack = "Podążaj";
            await Task.Delay(1000);
           var data=  await service.GetBusStopsAsync(AppProperties.GetToken());
            var positions = new List<Position>();
            foreach(var item in data)
            {
                positions.Add(new Position(item.Latitude, item.Longitude));
            }
            if (positions.Count == 0) return;
            var pos = await LocationService.GetCurrentPosition();
            var MapPosition = new Xamarin.Forms.Maps.Position(pos.Latitude, pos.Longitude);
            
            Device.BeginInvokeOnMainThread(() =>
            {
                Map.RouteCoordinates = positions;
                var pinList = new List<Pin>();
                pinList.Add(new Pin
                {
                    Label = "Koniec trasy",
                    Position = new Position(positions[positions.Count - 1].Latitude, positions[positions.Count - 1].Longitude)
                });
                Map.Pins = pinList;
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(MapPosition, Distance.FromMiles(0.5)));
            });


        }
    }
}
