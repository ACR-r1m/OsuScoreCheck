using OsuScoreCheck.Classes.PropGive;
using OsuScoreCheck.Service;
using ReactiveUI;
using System.Threading.Tasks;

namespace OsuScoreCheck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly SettingsService _settingsService = new SettingsService();
        private readonly OsuApiService _osuApiService = new OsuApiService();
        private string _selectedLanguage;
        private string _lastErrorKey;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
        }

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

        public MainWindowViewModel()
        {
            Navigate = newPage => CurrentPage = newPage;
            _osuApiService = new OsuApiService();
            var settings = _settingsService.LoadSettings();
            _selectedLanguage = settings.Language;
            Localizer.Instance.LoadLanguage(_selectedLanguage);
            _clientId = settings.ID;
            _clientSecret = settings.ClientSecret;

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            NavigateTo<SplashScreenViewModel>();
            await CheckApiAsync();
            await Task.Delay(1000);

            if (ApiCheck)
            {
                MessageBus.Current.SendMessage(new LeftMenuControlMessage(true, false, false));
                NavigateTo<UsersViewModel>(false);
            }
            else
            {
                bool isFirstRun = string.IsNullOrEmpty(_clientId) && string.IsNullOrEmpty(_clientSecret);
                if (isFirstRun)
                {
                    MessageBus.Current.SendMessage(new LeftMenuControlMessage(false, false, true));
                    NavigateTo<Manual1ViewModel>(true);
                }
                else
                {
                    MessageBus.Current.SendMessage(new LeftMenuControlMessage(false, false, false));
                    NavigateTo<ErrorViewModel>(true, _lastErrorKey);
                }
            }
        }

        public async Task CheckApiAsync()
        {
            var (token, errorKey) = await _osuApiService.GetAccessTokenWithErrorAsync(_clientId, _clientSecret);
            ApiCheck = !string.IsNullOrEmpty(token);
            _lastErrorKey = !ApiCheck ? (errorKey ?? "AccessTokenError") : null;
        }
    }
}
