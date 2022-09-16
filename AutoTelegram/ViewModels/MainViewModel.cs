using AutoTelegram.Models;
using AutoTelegram.Services;
using AutoTelegram.Views.Pages;
using DevExpress.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoTelegram.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public static MainPage mainPage = new MainPage();
        public static LoginPage loginPage = new LoginPage();
        private readonly PageService _pageService;
        private TelegramService? _telegramService;

        public Page CurrentPage { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public State StartButtonState { get; set; } = State.Start;
        public MainViewModel(PageService pageService)
        {
            _pageService = pageService;
            _pageService.OnPageChanged += page => CurrentPage = page;

            if (Properties.Settings.Default.IsAuthorized)
            {
                CurrentPage = mainPage;

                Messages = GetMessages();
            }
            else
            {
                CurrentPage = loginPage;
            }
        }

        public ICommand LogOutCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _telegramService = null;
                    Properties.Settings.Default.ApiId = null;
                    Properties.Settings.Default.ApiHash = null;
                    Properties.Settings.Default.PhoneNumber = null;
                    Properties.Settings.Default.IsAuthorized = false;

                    _pageService.Navigate(loginPage);
                });
            }
        }
        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    SaveMessages();

                    if (StartButtonState == State.Start)
                    {
                        if (_telegramService == null)
                        {
                            StartButtonState = State.Stop;

                            var properties = Properties.Settings.Default;
                            AuthDto auth = new AuthDto(properties.ApiId, properties.ApiHash, properties.PhoneNumber);

                            _telegramService = new TelegramService(auth);
                            await Task.Run(() => { _telegramService.Setup(Messages); return Task.CompletedTask; });
                        }
                        else
                        {
                            _telegramService?.Resume();
                            StartButtonState = State.Stop;
                        }
                    }
                    else if (StartButtonState == State.Stop)
                    {
                        StartButtonState = State.Start;
                        _telegramService?.Stop();
                    }
                });
            }
        }

        private void SaveMessages()
        {
            string json = JsonSerializer.Serialize(Messages);

            if (Directory.Exists("data") == false)
            {
                Directory.CreateDirectory("data");
            }

            File.WriteAllText(@"data\messages.json", json);
        }

        private List<Message> GetMessages()
        {
            if (Directory.Exists("data") == false)
            {
                return new List<Message>();
            }
            string json = File.ReadAllText(@"data\messages.json");
            var messages = JsonSerializer.Deserialize<List<Message>>(json);

            return messages ?? new List<Message>();
        }

        public enum State
        {
            Start,
            Stop
        }
    }
}
