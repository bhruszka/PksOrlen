using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Orlen.CustomRenderers
{
    public class RouteMap : Map
    {
        public static readonly BindableProperty RouteCoordinatesProperty = BindableProperty.Create(nameof(RouteCoordinates), typeof(List<Position>),
            typeof(RouteMap), new List<Position>(), BindingMode.TwoWay);
        public static readonly BindableProperty PinsCoordinatesProperty = BindableProperty.Create(nameof(Pins), typeof(List<Pin>),
            typeof(RouteMap), new List<Pin>(), BindingMode.TwoWay);

        public List<Position> RouteCoordinates
        {
            get { return (List<Position>)GetValue(RouteCoordinatesProperty); }
            set { SetValue(RouteCoordinatesProperty, value); }
        }
        public List<Pin> Pins
        {
            get { return (List<Pin>)GetValue(PinsCoordinatesProperty); }
            set { SetValue(PinsCoordinatesProperty, value); }
        }
        public RouteMap()
        {
            RouteCoordinates = new List<Position>();
            Pins = new List<Pin>();
        }
    }
}
