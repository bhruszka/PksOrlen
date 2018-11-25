using OrlenDriver.Interface;
using OrlenDriver.Services;
using OrlenDriver.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
[assembly: Dependency(typeof(NavService))]
namespace OrlenDriver.Services
{

    public class NavService : INavService
    {

        public INavigation navigation { get; set; }
        readonly IDictionary<Type, Type> _viewMapping = new Dictionary<Type, Type>();
        public void RegisterViewMapping(Type viewModel, Type view)
        {
            _viewMapping.Add(viewModel, view);
        }

        public async Task BackToMainPageAsync()
        {
            await navigation.PopToRootAsync(true);
        }

        public void ClearAllViewsFromStack()
        {
            if (navigation.NavigationStack.Count <= 1)
                return;

            for (var i = 0; i < navigation.NavigationStack.Count - 1; i++)
                navigation.RemovePage(navigation.NavigationStack[i]);
        }

        public async Task NavigateToViewModelParm<ViewModel, TParameter>(TParameter parameter, bool showNavBar) where ViewModel : BaseViewModel
        {
            Type viewType;
            if (_viewMapping.TryGetValue(typeof(ViewModel), out viewType))
            {
                var constructor = viewType.GetTypeInfo()
                    .DeclaredConstructors
                    .FirstOrDefault(dc => dc.GetParameters().Count() <= 0);

                var view = constructor.Invoke(null) as Page;
                NavigationPage.SetHasNavigationBar(view, showNavBar);
                await navigation.PushAsync(view, true);
            }

            if (navigation.NavigationStack.Last().BindingContext is BaseViewModel<TParameter>)
                await ((BaseViewModel<TParameter>)(navigation.NavigationStack.Last().BindingContext)).InitAsync(parameter);
        }

        public async Task NavigateToViewModel<ViewModel>(bool showNavBar) where ViewModel : BaseViewModel
        {
            Type viewType;
            if (_viewMapping.TryGetValue(typeof(ViewModel), out viewType))
            {
                var constructor = viewType.GetTypeInfo()
                    .DeclaredConstructors
                    .FirstOrDefault(dc => dc.GetParameters().Count() <= 0);

                var view = constructor.Invoke(null) as Page;
                NavigationPage.SetHasNavigationBar(view, showNavBar);
                await navigation.PushAsync(view, true);
            }


        }
        public async Task NavigateToViewModelMasterDetail<ViewModel>(bool showNavBar, string param) where ViewModel : BaseViewModel
        {
            Type viewType;
            if (_viewMapping.TryGetValue(typeof(ViewModel), out viewType))
            {
                var constructor = viewType.GetTypeInfo()
                    .DeclaredConstructors
                    .FirstOrDefault();

                var view = constructor.Invoke(new object[] { new object[] { param } }) as Page;
                NavigationPage.SetHasNavigationBar(view, showNavBar);
                await navigation.PushAsync(view, true);
            }


        }

        public async Task PreviousPageAsync()
        {
            if (navigation.NavigationStack != null &&
                navigation.NavigationStack.Count > 0)
            {
                await navigation.PopAsync(true);
            }
        }

        public async Task NavigateToViewModelMasterDetail<ViewModel>(bool showNavBar) where ViewModel : BaseViewModel
        {
            Type viewType;
            if (_viewMapping.TryGetValue(typeof(ViewModel), out viewType))
            {
                var constructor = viewType.GetTypeInfo()
                    .DeclaredConstructors
                    .FirstOrDefault();

                var view = constructor.Invoke(null) as Page;
                NavigationPage.SetHasNavigationBar(view, showNavBar);
                await navigation.PushAsync(view, true);
            }
        }


    }
}
