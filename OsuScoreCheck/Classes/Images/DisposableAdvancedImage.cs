using AsyncImageLoader;
using Avalonia;
using Avalonia.Media.Imaging;
using System;

namespace OsuScoreCheck.Classes.Images
{
    public class DisposableAdvancedImage : AdvancedImage, IDisposable
    {
        public DisposableAdvancedImage() : base((Uri?)null) { }

        public void Dispose()
        {
            if (CurrentImage is ImageWrapper wrapper && wrapper.ImageImplementation is Bitmap bitmap)
            {
                bitmap.Dispose();
            }
            CurrentImage = null;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            Dispose();
        }
    }
}
