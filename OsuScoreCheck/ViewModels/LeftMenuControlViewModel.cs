using OsuScoreCheck.Classes.PropGive;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace OsuScoreCheck.ViewModels
{
    public class LeftMenuControlViewModel : ViewModelBase
    {
        private bool _isButton1Checked;
        private bool _isButton2Checked;
        private bool _isButton3Checked;

        public bool IsButton1Checked
        {
            get => _isButton1Checked;
            set => this.RaiseAndSetIfChanged(ref _isButton1Checked, value);
        }

        public bool IsButton2Checked
        {
            get => _isButton2Checked;
            set => this.RaiseAndSetIfChanged(ref _isButton2Checked, value);
        }

        public bool IsButton3Checked
        {
            get => _isButton3Checked;
            set => this.RaiseAndSetIfChanged(ref _isButton3Checked, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        public LeftMenuControlViewModel()
        {
            MessageBus.Current.Listen<ApiCheckMessage>()
            .Subscribe(OnApiCheckMessageReceived);

            MessageBus.Current.Listen<LeftMenuControlMessage>()
            .Subscribe(ChoisePage);

        }

        private void OnApiCheckMessageReceived(ApiCheckMessage message)
        {
            IsEnabled = message.ApiCheck;
        }

        private void ChoisePage(LeftMenuControlMessage message)
        {
            IsButton1Checked = message.IsButton1Checked;
            IsButton2Checked = message.IsButton2Checked;
            IsButton3Checked = message.IsButton3Checked;
        }
    }
}
