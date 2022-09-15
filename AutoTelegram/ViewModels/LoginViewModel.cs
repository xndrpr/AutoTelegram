using AutoTelegram.Services;
using DevExpress.Mvvm;
namespace AutoTelegram.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public LoginViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
    }
}
