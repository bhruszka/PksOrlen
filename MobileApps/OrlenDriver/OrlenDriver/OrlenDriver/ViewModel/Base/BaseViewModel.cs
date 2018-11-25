using OrlenDriver.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OrlenDriver.ViewModel.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected INavService NavService { get; private set; }



        protected BaseViewModel(INavService navService)
        {
            NavService = navService;

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
