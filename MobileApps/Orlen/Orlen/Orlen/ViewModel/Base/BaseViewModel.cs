using Orlen.Interfaces;
using Orlen.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Orlen.ViewModel.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected INavService NavService { get; private set; }



        protected BaseViewModel(INavService navService)
        {
            NavService = navService;
            AppPropertiesService = new AppProperties();

        }
        
        bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                OnPropertyChanged();
            }
        }
        IDatabaseRepository _dbService;
        public IDatabaseRepository DbService
        {
            get { return _dbService; }
            set
            {
                _dbService = value;
                OnPropertyChanged();
            }
        }

        IAppProperties _appPropertiesService;
        public IAppProperties AppPropertiesService
        {
            get { return _appPropertiesService; }
            set
            {
                _appPropertiesService = value;
                OnPropertyChanged();
            }
        }
        

        bool _isProcessBusy;
        public bool IsProcessBusy
        {
            get { return _isProcessBusy; }
            set
            {
                _isProcessBusy = value;
                OnPropertyChanged();
            }
        }

        public abstract Task InitAsync();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region CustomRoutingRegion

        Command _showMapCommand;
        public Command ShowMapCommand => _showMapCommand ??
            (_showMapCommand = new Command(async (user) => await ShowMapAsync()));

        private async Task ShowMapAsync()
        {
            NavService.ClearAllViewsFromStack();
            await NavService.NavigateToViewModel<MapPageViewModel>(false);
        }
        Command _showTimetableCommand;
        public Command ShowTimetableCommand => _showTimetableCommand ??
            (_showTimetableCommand = new Command(async (user) => await ShowTimetableAsync()));

        private async Task ShowTimetableAsync()
        {
            NavService.ClearAllViewsFromStack();
            await NavService.NavigateToViewModel<TimetablePageViewModel>(false);
        }
        Command _returnToPreviousPageCommand;
        public Command ReturnToPreviousPageCommand => _returnToPreviousPageCommand ??
            (_returnToPreviousPageCommand = new Command(async (user) => await ReturnToPreviousPageAsync()));

        private async Task ReturnToPreviousPageAsync()
        {
            await NavService.PreviousPageAsync();

        }

        #endregion
    }
    public abstract class BaseViewModel<Param> : BaseViewModel
    {
        protected BaseViewModel(INavService navService) : base(navService)
        {
        }

        public override async Task InitAsync()
        {
            await InitAsync(default(Param));
        }

        public abstract Task InitAsync(Param param);
    }
}
