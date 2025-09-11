using Avalonia;
using Avalonia.Controls;

namespace OsuScoreCheck.Controls.ControlsClasses
{
    public class SvgTextBox : TextBox
    {
        public static readonly StyledProperty<string> SvgPathProperty =
            AvaloniaProperty.Register<SvgButton, string>(nameof(SvgPath));

        public string SvgPath
        {
            get => GetValue(SvgPathProperty);
            set => SetValue(SvgPathProperty, value);
        }


        public static readonly StyledProperty<string> StaticTextProperty =
        AvaloniaProperty.Register<SvgTextBox, string>(nameof(StaticText));

        public string StaticText
        {
            get => GetValue(StaticTextProperty);
            set => SetValue(StaticTextProperty, value);
        }

        public SvgTextBox()
        {
            //this.GetObservable(TextProperty).Subscribe(_ => UpdateWidth());
        }

        //protected override void OnInitialized()
        //{
        //    base.OnInitialized();
        //    UpdateWidth();
        //}

        //private void UpdateWidth()
        //{
        //    const int charWidth = 14;   // Ширина одного символа, подберите оптимальное значение

        //    // Учитываем длину watermark и текста
        //    var watermarkLength = Watermark?.Length ?? 0;
        //    var textLength = Text?.Length ?? 0;

        //    // Берём максимальную длину между watermark и текстом
        //    var maxLength = Math.Max(watermarkLength, textLength);

        //    Width = maxLength * charWidth;
        //}

    }
}
