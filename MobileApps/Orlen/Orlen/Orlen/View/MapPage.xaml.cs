using Orlen.Interfaces;
using Orlen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Orlen.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPage : ContentPage
	{
        MapPageViewModel _viewModel
        {
            get { return BindingContext as MapPageViewModel; }
        }
        public MapPage ()
		{
			InitializeComponent ();

            BindingContext = new MapPageViewModel(DependencyService.Get<INavService>());
            
            _viewModel.Map = MyMap;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
                await _viewModel.InitAsync();
        }
    }
}