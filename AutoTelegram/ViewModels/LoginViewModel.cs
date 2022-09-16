using AutoTelegram.Services;
using DevExpress.Mvvm;
using System.Windows.Input;

namespace AutoTelegram.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public string ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
        public LoginViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
        
        public ICommand SignInCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Properties.Settings.Default.ApiId = ApiId;
                    Properties.Settings.Default.ApiHash = ApiHash;
                    Properties.Settings.Default.PhoneNumber = PhoneNumber;
                    Properties.Settings.Default.IsAuthorized = true;

                    Properties.Settings.Default.Save();

                    _pageService.Navigate(MainViewModel.mainPage);
                });
            }
        }
    }
}
