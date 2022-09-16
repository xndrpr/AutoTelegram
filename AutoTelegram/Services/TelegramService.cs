using AutoTelegram.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TL;
using WTelegram;

namespace AutoTelegram.Services
{
    enum State
    {
        Work,
        Wait
    }

    public class TelegramService
    {
        private static Client? _client;
        private static User? _user;
        private bool _isRunning = false;
        private List<Models.Message>? _messages;
        private State _state = State.Work;
        private Models.Message? _waitingMessage = null;
        private readonly Dictionary<long, User> _users = new();
        private readonly Dictionary<long, ChatBase> _chats = new();
        private readonly AuthDto? _auth;
        public TelegramService(AuthDto auth)
        {
            _auth = auth;
        } 

        public async void Setup(List<Models.Message> messages)
        {
            try
            {
                _messages = messages;

                _client = new Client(_auth!.Config);
                _client.OnUpdate += OnUpdate;

                _user = await _client.LoginUserIfNeeded();
                _users[_user.id] = _user;

                using (StreamWriter stream = new StreamWriter("data/log.txt", false))
                {
                    stream.WriteLine($"Logged-in as:\n\tUsername: {_user.username ?? _user.first_name + " " + _user.last_name}\n\tId: {_user.id}\n\tPhone number: {_user.phone}\n\tStatus: {_user.status}\n\tFirst name{_user.first_name}\n\tLast name{_user.last_name}");
                }

                var dialogs = await _client.Messages_GetAllDialogs();
                dialogs.CollectUsersChats(_users, _chats);

                Start();

                MessageBox.Show("Started", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Resume()
        {
            _isRunning = true;
        }

        private async void Start()
        {
            _isRunning = true; 

            while (_isRunning)
            {
                try
                {

                    if (_state == State.Work)
                    {
                        foreach (var message in _messages ?? new List<Models.Message>())
                        {

                            if (_state == State.Work)
                            {
                                if (message.Text != null)
                                {
                                    var resolved = await _client.Contacts_ResolveUsername(message.Username.Replace("@", string.Empty));
                                    await _client!.SendMessageAsync(resolved, message.Text);
                                }

                                if (message.Targets != null && _waitingMessage == null)
                                {
                                    _state = State.Wait;
                                    _waitingMessage = message;
                                    continue;
                                }

                                await Task.Delay(new TimeSpan(0, 0, message.SleepSeconds));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (ex.Message.Contains("FLOOD_WAIT_X"))
                    {
                        Stop();
                    }
                }

                await Task.Delay(750);
            }
        }

        private async Task OnUpdate(IObject arg)
        {
            if (arg is not UpdatesBase updates || _state == State.Work) return;

            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm:
                        await DisplayMessage(unm.message);
                        break;
                    default:
                        Console.WriteLine(update.GetType().Name);
                        break;
                }
            }
        }

        private async Task DisplayMessage(MessageBase messageBase)
        {
            switch (messageBase)
            {
                case TL.Message message:
                    using (StreamWriter stream = new StreamWriter("data/log.txt", true))
                    {
                        stream.WriteLine($"\n\n{Peer(message.peer_id)} ( {message.peer_id} ) -> {message.message}\n\n");
                    }

                    if (_state == State.Wait && message.peer_id.ID != _user?.id)
                    {
                        var targets = _waitingMessage?.Targets?.Split(";").ToList();
                        var resolved = await _client.Contacts_ResolveUsername(_waitingMessage?.Username.Replace("@", string.Empty));

                        bool isContains = targets?.Find(target => message.message.ToLower().Contains(target.ToLower())) == null ? false : true;

                        if (isContains)
                        {
                            await _client!.SendMessageAsync(resolved, _waitingMessage?.Answer, reply_to_msg_id: message.ID);
                            _state = State.Work;
                            _waitingMessage = null;
                        }
                    }

                    break;
            }
        }

        private string? User(long id) => _users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private string? Chat(long id) => _chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private string? Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
    }

    public static class TelegramServiceExtensions
    {
        public static bool IsCorrect(this AuthDto auth)
        {
            try
            {
                var client = new Client(auth.Config);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
