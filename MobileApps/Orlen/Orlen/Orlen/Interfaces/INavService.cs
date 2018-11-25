using Orlen.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orlen.Interfaces
{
    public interface INavService
    {
        Task PreviousPageAsync();

        Task BackToMainPageAsync();

        Task NavigateToViewModelParm<ViewModel, TParameter>(TParameter parameter, bool showNavBar) where ViewModel : BaseViewModel;

        Task NavigateToViewModel<ViewModel>(bool showNavBar) where ViewModel : BaseViewModel;

        Task NavigateToViewModelMasterDetail<ViewModel>(bool showNavBar, string param) where ViewModel : BaseViewModel;
        //Task NavigateToViewModelMasterDetail<ViewModel>(bool showNavBar) where ViewModel : BaseViewModel;

        void ClearAllViewsFromStack();
    }
}
