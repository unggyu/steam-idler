using Prism.Events;
using SteamIdler.Events;
using SteamIdler.Infrastructure.Helpers;
using SteamIdler.Services;

namespace SteamIdler.ViewModels
{
    public class MySplashViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly BotService _botService;
        private string _taskContent;

        public MySplashViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _botService = BotService.Instance;

            Initialize();
        }

        public string TaskContent
        {
            get => _taskContent;
            set => SetValue(ref _taskContent, value);
        }

        public async void Initialize()
        {
            TaskContent = Properties.Resources.Loading;

            await DbInitializer.EnsureInitializeAsync();

            var result = await _botService.ConnectAsync();
            _eventAggregator.GetEvent<InitializedEvent>().Publish(result);
        }
    }
}
