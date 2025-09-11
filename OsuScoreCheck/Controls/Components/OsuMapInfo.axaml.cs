using Avalonia;
using Avalonia.Controls.Primitives;
using OsuScoreCheck.Classes.Images;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace OsuScoreCheck.Controls.Components
{
    public class OsuMapInfo : TemplatedControl, IDisposable
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Title), "Error");

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> DiffProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Diff), "Error");

        public string Diff
        {
            get => GetValue(DiffProperty);
            set => SetValue(DiffProperty, value);
        }

        public static readonly StyledProperty<string> TimeProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Time), "00:00");

        public string Time
        {
            get => GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly StyledProperty<string> StarProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Star), "00.00");

        public string Star
        {
            get => GetValue(StarProperty);
            set => SetValue(StarProperty, value);
        }

        public static readonly StyledProperty<int> PPProperty =
            AvaloniaProperty.Register<OsuMapInfo, int>(nameof(PP));

        public int PP
        {
            get => GetValue(PPProperty);
            set => SetValue(PPProperty, value);
        }

        public static readonly StyledProperty<string> ResultProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Result), "Error");

        public string Result
        {
            get => GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        public static readonly StyledProperty<string> ModsProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(Mods), "Error");

        public string Mods
        {
            get => GetValue(ModsProperty);
            set => SetValue(ModsProperty, value);
        }

        public static readonly StyledProperty<string> BackgroundImageProperty =
           AvaloniaProperty.Register<OsuMapInfo, string>(nameof(BackgroundImage));

        public string BackgroundImage
        {
            get => GetValue(BackgroundImageProperty);
            set => SetValue(BackgroundImageProperty, value);
        }

        public static readonly StyledProperty<string> SimplifiedCategoryProperty =
            AvaloniaProperty.Register<OsuMapInfo, string>(nameof(SimplifiedCategory));
        public string SimplifiedCategory
        {
            get => GetValue(SimplifiedCategoryProperty);
            set => SetValue(SimplifiedCategoryProperty, value);
        }

        public static readonly StyledProperty<int> ModsCountProperty =
            AvaloniaProperty.Register<OsuMapInfo, int>(nameof(ModsCount));

        public int ModsCount
        {
            get => GetValue(ModsCountProperty);
            private set => SetValue(ModsCountProperty, value);
        }

        public static readonly StyledProperty<double> ModsMaxWidthProperty =
            AvaloniaProperty.Register<OsuMapInfo, double>(nameof(ModsMaxWidth));

        public double ModsMaxWidth
        {
            get => GetValue(ModsMaxWidthProperty);
            private set => SetValue(ModsMaxWidthProperty, value);
        }

        #region visableBorder

        public static readonly StyledProperty<bool> IsResultModsBorderVisibleProperty =
            AvaloniaProperty.Register<OsuMapInfo, bool>(nameof(IsResultModsBorderVisible));
        public bool IsResultModsBorderVisible
        {
            get => GetValue(IsResultModsBorderVisibleProperty);
            set => SetValue(IsResultModsBorderVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> IsPointsBorderVisibleProperty =
            AvaloniaProperty.Register<OsuMapInfo, bool>(nameof(IsPointsBorderVisible));
        public bool IsPointsBorderVisible
        {
            get => GetValue(IsPointsBorderVisibleProperty);
            set => SetValue(IsPointsBorderVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> IsLovedVisibleProperty =
            AvaloniaProperty.Register<OsuMapInfo, bool>(nameof(IsLovedVisible));
        public bool IsLovedVisible
        {
            get => GetValue(IsLovedVisibleProperty);
            set => SetValue(IsLovedVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> IsLovedResultModsVisibleProperty =
        AvaloniaProperty.Register<OsuMapInfo, bool>(nameof(IsLovedResultModsVisible));
        public bool IsLovedResultModsVisible
        {
            get => GetValue(IsLovedResultModsVisibleProperty);
            set => SetValue(IsLovedResultModsVisibleProperty, value);
        }

        #endregion

        private readonly CompositeDisposable _subscriptions = new();
        private DisposableAdvancedImage _image;

        public OsuMapInfo()
        {
            this.WhenAnyValue(
                 x => x.SimplifiedCategory,
                 x => x.Mods,
                 x => x.Result,
                 x => x.PP,
                 x => x.BackgroundImage)
                 .Subscribe(_ => UpdateBordersVisibility());

            UpdateModsCountAndMaxWidth();

            this.GetPropertyChangedObservable(ModsProperty).Subscribe(_ => UpdateModsCountAndMaxWidth());
        }

        private void UpdateModsCountAndMaxWidth()
        {
            int newModsCount = string.IsNullOrWhiteSpace(Mods)
                ? 0
                : Mods.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            ModsCount = newModsCount;

            if (newModsCount == 1)
            {
                ModsMaxWidth = 25.0;
            }
            else
            {
                ModsMaxWidth = 78.0;
            }
        }

        private void UpdateBordersVisibility()
        {
            bool isRanked = SimplifiedCategory == "Ranked";
            bool isQualified = SimplifiedCategory == "Qualified";
            bool isLoved = SimplifiedCategory == "Loved";

            bool isLovedOrZeroPP = (isLoved || (isRanked || isQualified)) && PP <= 0;


            IsResultModsBorderVisible = (isRanked || isQualified) && !string.IsNullOrEmpty(Mods) && !string.IsNullOrEmpty(Result) && PP > 0;

            IsPointsBorderVisible = (isRanked || isQualified) && PP > 0 && string.IsNullOrEmpty(Mods) && !string.IsNullOrEmpty(Result);

            IsLovedVisible = isLovedOrZeroPP && !string.IsNullOrEmpty(Result) && string.IsNullOrEmpty(Mods);

            IsLovedResultModsVisible = isLoved && !string.IsNullOrEmpty(Result) && !string.IsNullOrEmpty(Mods) && PP <= 0;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
            _subscriptions.Clear();
            Mods = null;
            Result = null;
            PP = 0;
            DataContext = null;
            BackgroundImage = null;
            _image?.Dispose();
        }
    }
}
