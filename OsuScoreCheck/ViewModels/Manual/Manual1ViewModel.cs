using OsuScoreCheck.Service;
using ReactiveUI;
using System.Reactive;

namespace OsuScoreCheck.ViewModels
{
    public class Manual1ViewModel : ViewModelBase
    {
        #region localization

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
        }

        public ReactiveCommand<string, Unit> ChangeLanguageCommand { get; }
        #endregion

        private readonly SettingsService _settingsService = new SettingsService();

        public Manual1ViewModel()
        {
            var settings = _settingsService.LoadSettings();

            ChangeLanguageCommand = ReactiveCommand.Create<string>(languageCode =>
            {
                Localizer.Instance.LoadLanguage(languageCode);
                _selectedLanguage = languageCode;

                settings.Language = languageCode;
                _settingsService.SaveSettings(settings);
            });

            OpenLinkCommand = ReactiveCommand.Create(() =>
            {
                var url = "https://osu.ppy.sh/home/account/edit";
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            });
        }

        public ReactiveCommand<Unit, Unit> OpenLinkCommand { get; }
    }
}
