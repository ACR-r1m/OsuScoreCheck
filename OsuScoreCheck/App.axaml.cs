using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using OsuScoreCheck.Classes.Images;
using OsuScoreCheck.Data;
using OsuScoreCheck.ViewModels;
using OsuScoreCheck.Views;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.Controls.Styles")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.Controls.Components")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.Controls.ControlsClasses")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.Controls.InputEvents")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.Service")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "OsuScoreCheck.ViewModels")]

namespace OsuScoreCheck
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            using (var context = new ApplicationContext())
            {
                context.EnsureDatabaseCreated();
            }

            AsyncImageLoader.ImageLoader.AsyncImageLoader = new CustomRamCachedWebImageLoader();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}