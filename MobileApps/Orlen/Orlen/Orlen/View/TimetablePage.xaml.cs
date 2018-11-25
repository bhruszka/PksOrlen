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
	public partial class TimetablePage : ContentPage
	{
        TimetablePageViewModel _viewModel
        {
            get { return BindingContext as TimetablePageViewModel; }
        }
        public TimetablePage()
        {
            InitializeComponent();
            BindingContext = new TimetablePageViewModel(DependencyService.Get<INavService>());
            listView.ItemSelected += (s, e) => listView.SelectedItem = null;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null)
                await _viewModel.InitAsync();
        }
    }
}