using OsuScoreCheck.Service;
using ReactiveUI;

namespace OsuScoreCheck.ViewModels
{
    public class ErrorViewModel : ViewModelBase
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ErrorViewModel(string errorKey)
        {
            ErrorMessage = !string.IsNullOrEmpty(errorKey)
                ? Localizer.Instance[errorKey]
                : Localizer.Instance["AccessTokenError"];
        }
    }
}
