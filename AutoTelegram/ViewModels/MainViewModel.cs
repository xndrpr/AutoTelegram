using AutoTelegram.Models;
using AutoTelegram.Services;
using AutoTelegram.Views.Pages;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoTelegram.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public static MainPage mainPage = new MainPage();
        public static LoginPage loginPage = new LoginPage();
        private readonly PageService _pageService;

        public Page CurrentPage { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public MainViewModel(PageService pageService)
        {
            _pageService = pageService;
            _pageService.OnPageChanged += page => CurrentPage = page;

            CurrentPage = loginPage;
        }

        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {

                });
            }
        }
    }
}
