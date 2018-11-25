using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Orlen.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Orlen.CustomRenderers;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;

[assembly: ExportRenderer(typeof(RouteMap), typeof(CustomMapRenderer))]
namespace Orlen.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
        List<Position> routeCoordinates;
        List<Pin> customPins;
        RouteMap formsMap;
        public CustomMapRenderer(Context context) : base(context)
        {
        }


        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
            }

            if (e.NewElement != null)
            {
                formsMap = (RouteMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                customPins = formsMap.Pins;
                Control.GetMapAsync(this);
            }
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (this.Element == null || this.Control == null)
                return;

            if (e.PropertyName == RouteMap.RouteCoordinatesProperty.PropertyName)
            {
                UpdatePolyLine();
            }
            if (e.PropertyName == RouteMap.PinsCoordinatesProperty.PropertyName)
            {
             
                customPins = formsMap.Pins;
                UpdatePins();
            }
        }

        private void UpdatePins()
        {
            
            foreach(var item in ((RouteMap)this.Element).Pins)
            {
                NativeMap.AddMarker(CreateMarker(item));

            }
        }

        
        private void UpdatePolyLine()
        {
            

            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(0x66FF0000);

            foreach (var position in ((RouteMap)this.Element).RouteCoordinates)
            {
                polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
            }

            NativeMap.AddPolyline(polylineOptions);
        }
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bus_black));
            return marker;
        }

        

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

               
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        Pin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }


}
