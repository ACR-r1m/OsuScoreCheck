using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Windows.Input;

namespace OsuScoreCheck.Controls.Components
{
    public class UserInfo : TemplatedControl
    {
        public static readonly StyledProperty<string> AvatarUrlProperty =
           AvaloniaProperty.Register<OsuMapInfo, string>(nameof(AvatarUrl));

        public string AvatarUrl
        {
            get => GetValue(AvatarUrlProperty);
            set => SetValue(AvatarUrlProperty, value);
        }

        public static readonly StyledProperty<string> UsernameProperty =
            AvaloniaProperty.Register<UserInfo, string>(nameof(Username));

        public string Username
        {
            get => GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public static readonly StyledProperty<string> FirstAnalysisTimeProperty =
            AvaloniaProperty.Register<UserInfo, string>(nameof(FirstAnalysisTime));

        public string FirstAnalysisTime
        {
            get => GetValue(FirstAnalysisTimeProperty);
            set => SetValue(FirstAnalysisTimeProperty, value);
        }

        public static readonly StyledProperty<int> TotalPlayedMapsProperty =
            AvaloniaProperty.Register<UserInfo, int>(nameof(TotalPlayedMaps));

        public int TotalPlayedMaps
        {
            get => GetValue(TotalPlayedMapsProperty);
            set => SetValue(TotalPlayedMapsProperty, value);
        }

        public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<TemplatedControl, bool>(nameof(IsLoading), false);

        public bool IsLoading
        {
            get => GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly StyledProperty<int> LastCheckedMapIdProperty =
           AvaloniaProperty.Register<UserInfo, int>(nameof(LastCheckedMapId));

        public int LastCheckedMapId
        {
            get => GetValue(LastCheckedMapIdProperty);
            set => SetValue(LastCheckedMapIdProperty, value);
        }

        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<UserInfo, ICommand?>(nameof(Command));

        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object?> CommandParameterProperty =
           AvaloniaProperty.Register<UserInfo, object?>(nameof(CommandParameter));

        public object? CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public UserInfo()
        {
            PointerPressed += OnPointerPressed;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (!e.Handled &&
                e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
                Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var loadingIndicator = e.NameScope.Find<Control>("PART_LoadingIndicator");
            var content = e.NameScope.Find<Control>("PART_Content");

            if (loadingIndicator != null && content != null)
            {
                loadingIndicator.IsVisible = IsLoading;
                content.IsVisible = !IsLoading;
            }
        }

    }
}
