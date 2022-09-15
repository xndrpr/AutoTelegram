using AutoTelegram.Services;
using AutoTelegram.Views.Pages;
using DevExpress.Mvvm;
using System.Windows.Controls;

namespace AutoTelegram.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public static MainPage mainPage = new MainPage();
        public static LoginPage loginPage = new LoginPage();
        private readonly PageService _pageService;

        public Page CurrentPage { get; set; }

        public MainViewModel(PageService pageService)
        {
            _pageService = pageService;
            _pageService.OnPageChanged += page => CurrentPage = page;

            CurrentPage = loginPage;
        }
    }
}
