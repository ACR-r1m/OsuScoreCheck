using Avalonia;
using Avalonia.Controls;

namespace OsuScoreCheck.Controls.ControlsClasses
{
    public class SvgButton : Button
    {
        public static readonly StyledProperty<string> SvgPathProperty =
            AvaloniaProperty.Register<SvgButton, string>(nameof(SvgPath));

        public string SvgPath
        {
            get => GetValue(SvgPathProperty);
            set => SetValue(SvgPathProperty, value);
        }
    }
}
