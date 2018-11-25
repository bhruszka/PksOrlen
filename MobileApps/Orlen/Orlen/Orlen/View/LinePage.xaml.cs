using Orlen.Interfaces;
using Orlen.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Orlen.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LinePage : ContentPage
	{
        LinePageViewModel _viewModel
        {
            get { return BindingContext as LinePageViewModel; }
        }
        public LinePage()
        {
            InitializeComponent();
            BindingContext = new LinePageViewModel(DependencyService.Get<INavService>());
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