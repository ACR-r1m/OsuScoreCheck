using AsyncImageLoader.Loaders;
using Avalonia.Media.Imaging;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OsuScoreCheck.Classes.Images
{
    public class CustomRamCachedWebImageLoader : RamCachedWebImageLoader
    {
        private readonly ConcurrentDictionary<string, Task<Bitmap?>> _memoryCache = new();

        public void ClearCache()
        {
            foreach (var item in _memoryCache)
            {
                if (item.Value.IsCompleted && item.Value.Result != null)
                {
                    item.Value.Result.Dispose();
                }
            }
            _memoryCache.Clear();
        }

        public override async Task<Bitmap?> ProvideImageAsync(string url)
        {
            var bitmap = await _memoryCache.GetOrAdd(url, LoadAsync).ConfigureAwait(false);
            if (bitmap == null) _memoryCache.TryRemove(url, out _);
            return bitmap;
        }
    }
}
