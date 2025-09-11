using OsuScoreCheck.Classes.PropGive;
using OsuScoreCheck.Service;
using ReactiveUI;
using System.Threading.Tasks;

namespace OsuScoreCheck.ViewModels
{
    public class ManualChekingViewModel : ViewModelBase
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly OsuApiService _osuApiService = new OsuApiService();
        private readonly SettingsService _settingsService = new SettingsService();
        private bool _apiCheck;
        public bool ApiCheck
        {
            get => _apiCheck;
            set
            {
                this.RaiseAndSetIfChanged(ref _apiCheck, value);
                MessageBus.Current.SendMessage(new ApiCheckMessage(value));
            }
        }

        private bool _isLoadingStatus;

        public ManualChekingViewModel(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            IsLoadingStatus = true;
            await CheckApiAsync();
            await Task.Delay(3000);

            var settings = _settingsService.LoadSettings();
            if (ApiCheck)
            {
                settings.ID = _clientId;
                settings.ClientSecret = _clientSecret;
                NavigateTo<UsersViewModel>();
            }
            else
            {
                settings.ID = null;
                settings.ClientSecret = null;
            }
            _settingsService.SaveSettings(settings);
        }

        private async Task CheckApiAsync()
        {
            var token = await _osuApiService.GetAccessTokenAsync(_clientId, _clientSecret);
            ApiCheck = !string.IsNullOrEmpty(token);
        }

        public bool IsLoadingStatus
        {
            get => _isLoadingStatus;
            set => this.RaiseAndSetIfChanged(ref _isLoadingStatus, value);
        }
    }
}
