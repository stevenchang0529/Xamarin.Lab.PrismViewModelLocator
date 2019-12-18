using Prism;
using Prism.Ioc;
using Xamarin.Lab.PrismViewModelLocator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Mvvm;
using System;
using Xamarin.Lab.PrismViewModelLocator.ViewModel;
using System.Linq;
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Xamarin.Lab.PrismViewModelLocator
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.Name;//View的名稱
                var viewModelAssemblyName = "Xamarin.Lab.PrismViewModelLocator.ViewModel";//ViewModel的組件名稱
                var viewModelType = $"{viewModelAssemblyName}.{viewName}ViewModel, {viewModelAssemblyName}";//組合建立Type的字串
                var typ = Type.GetType(viewModelType);
                return typ;
            });
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            ViewModelBase.InitViewModel();
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }




        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

            var pageTypes=AppDomain.CurrentDomain.GetAssemblies()//所有組件
                .FirstOrDefault(x => x.FullName == this.GetType().Assembly.FullName)//與目前App.cs相同的組件
                .GetTypes()//所有型別
                .Where(x =>x.IsClass&&x.Namespace == "Xamarin.Lab.PrismViewModelLocator.Views");
            foreach(var type in pageTypes)
                containerRegistry.RegisterForNavigation(type, type.Name);
        }
    }
}
