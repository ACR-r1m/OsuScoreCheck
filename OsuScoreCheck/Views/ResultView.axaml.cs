using Avalonia;
using Avalonia.Controls;
using OsuScoreCheck.ViewModels;
using System;

namespace OsuScoreCheck.Views
{
    public partial class ResultView : UserControl
    {
        public ResultView()
        {
            InitializeComponent();
        }

        private async void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (scrollViewer.Offset.Y >= (scrollViewer.Extent.Height - scrollViewer.Viewport.Height) - 50)
                {
                    if (DataContext is ResultViewModel viewModel)
                    {
                        await viewModel.LoadMoreMapsAsync();
                    }
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            MapsItemsControl.ItemsSource = null;
            MapsItemsControl.Items.Clear();
            DataContext = null;
            base.OnDetachedFromVisualTree(e);
        }
    }
}
