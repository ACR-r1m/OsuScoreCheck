using Avalonia;
using Avalonia.Controls;

namespace OsuScoreCheck.Controls.ControlsClasses
{
    public class SvgRadioButton : RadioButton
    {
        public static readonly StyledProperty<string> ActiveSvgPathProperty =
            AvaloniaProperty.Register<SvgRadioButton, string>(nameof(ActiveSvgPath));

        public string ActiveSvgPath
        {
            get => GetValue(ActiveSvgPathProperty);
            set => SetValue(ActiveSvgPathProperty, value);
        }

        public static readonly StyledProperty<string> InactiveSvgPathProperty =
            AvaloniaProperty.Register<SvgRadioButton, string>(nameof(InactiveSvgPath));

        public string InactiveSvgPath
        {
            get => GetValue(InactiveSvgPathProperty);
            set => SetValue(InactiveSvgPathProperty, value);
        }
    }
}
