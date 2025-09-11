using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace OsuScoreCheck.ViewModels
{
        public class ViewModelBase : ReactiveObject
        {
            #region Navigation Page

            private static readonly Dictionary<(Type, object), WeakReference<ViewModelBase>> _viewModelCache = new();
                private ViewModelBase _currentPage;
            public ViewModelBase CurrentPage
            {
                get => _currentPage;
                set => this.RaiseAndSetIfChanged(ref _currentPage, value);
            }

            public static Action<ViewModelBase> Navigate { get; set; }
            public ReactiveCommand<Type, Unit> NavigateCommand { get; }
            public ReactiveCommand<Type, Unit> NavigateToNewCommand { get; }

            public void NavigateTo<T>(bool clearOld = false, params object[] args) where T : ViewModelBase
            {
                var viewModelType = typeof(T);
                var cacheKey = (viewModelType, args.Length > 0 ? args[0] : null);

                if (_viewModelCache.TryGetValue(cacheKey, out var weakRef) && weakRef.TryGetTarget(out var existingViewModel))
                {
                    if (clearOld)
                    {
                        RemoveViewModel(cacheKey);
                    }
                    else
                    {
                        Navigate?.Invoke(existingViewModel);
                        return;
                    }
                }

                var newViewModel = (T)Activator.CreateInstance(viewModelType, args);
                _viewModelCache[cacheKey] = new WeakReference<ViewModelBase>(newViewModel);
                Navigate?.Invoke(newViewModel);
            }

            public static void RemoveViewModel((Type, object) cacheKey)
            {
                if (_viewModelCache.TryGetValue(cacheKey, out var weakRef) && weakRef.TryGetTarget(out var viewModel))
                {
                    if (viewModel is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _viewModelCache.Remove(cacheKey);
            }

            #endregion

            public ViewModelBase()
            {
                NavigateCommand = ReactiveCommand.Create<Type>(viewModelType =>
                {
                    var cacheKey = (viewModelType, (object)null); // Без параметров, как в XAML
                    if (_viewModelCache.TryGetValue(cacheKey, out var weakRef) && weakRef.TryGetTarget(out var existingViewModel))
                    {
                        Navigate?.Invoke(existingViewModel);
                    }
                    else
                    {
                        var viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                        _viewModelCache[cacheKey] = new WeakReference<ViewModelBase>(viewModel);
                        Navigate?.Invoke(viewModel);
                    }
                });

                NavigateToNewCommand = ReactiveCommand.Create<Type>(viewModelType =>
                {
                    var cacheKey = (viewModelType, (object)null);
                    if (_viewModelCache.ContainsKey(cacheKey))
                    {
                        RemoveViewModel(cacheKey);
                    }
                    var viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                    _viewModelCache[cacheKey] = new WeakReference<ViewModelBase>(viewModel);
                    Navigate?.Invoke(viewModel);
                });
            }
        }
}
