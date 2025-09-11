using OsuScoreCheck.Classes.PropGive;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace OsuScoreCheck.ViewModels
{
    public class Manual3ViewModel : ViewModelBase
    {

        private string _clientId;
        public string ClientId
        {
            get => _clientId;
            set => this.RaiseAndSetIfChanged(ref _clientId, value);
        }

        private string _clientSecret;
        public string ClientSecret
        {
            get => _clientSecret;
            set => this.RaiseAndSetIfChanged(ref _clientSecret, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isButtonEnabled;
        public bool IsButtonEnabled => _isButtonEnabled.Value;

        public ReactiveCommand<Unit, Unit> GetAccessTokenCommand { get; }

        public Manual3ViewModel()
        {

            GetAccessTokenCommand = ReactiveCommand.CreateFromTask(GetAccessTokenAsync);

            this.WhenAnyValue(x => x.ClientId, x => x.ClientSecret,
                         (clientId, clientSecret) =>
                             !string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
           .ToProperty(this, x => x.IsButtonEnabled, out _isButtonEnabled);

        }

        private async Task GetAccessTokenAsync()
        {
            MessageBus.Current.SendMessage(new LeftMenuControlMessage(true, false, false));
            NavigateTo<ManualChekingViewModel>(false, ClientId, ClientSecret);
        }
    }
}
