using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.VisualTree;
using OsuScoreCheck.Controls.Components;
using OsuScoreCheck.Models.Enums;
using ReactiveUI;
using System;
using System.Diagnostics;

namespace OsuScoreCheck.Controls.Components
{
    public class ComboFilter : TemplatedControl
    {
        #region prop
        public static readonly StyledProperty<string> FromValueProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(FromValue), defaultBindingMode: BindingMode.TwoWay);

        public string FromValue
        {
            get => GetValue(FromValueProperty);
            set => SetValue(FromValueProperty, value);
        }

        public static readonly StyledProperty<string> ToValueProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(ToValue), defaultBindingMode: BindingMode.TwoWay);

        public string ToValue
        {
            get => GetValue(ToValueProperty);
            set => SetValue(ToValueProperty, value);
        }

        public static readonly StyledProperty<bool> IsPopupOpenProperty =
            AvaloniaProperty.Register<ComboFilter, bool>(nameof(IsPopupOpen));

        public bool IsPopupOpen
        {
            get => GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        public static readonly StyledProperty<string> OneIconProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(OneIcon));

        public string OneIcon
        {
            get => GetValue(OneIconProperty);
            set => SetValue(OneIconProperty, value);
        }

        public static readonly StyledProperty<string> HeaderTextProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(HeaderText));

        public string HeaderText
        {
            get => GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly StyledProperty<string> GroupNameProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(GroupName), defaultBindingMode: BindingMode.OneWay);

        public string GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        public static readonly StyledProperty<object> PopupContentProperty =
            AvaloniaProperty.Register<ComboFilter, object>(nameof(PopupContent));

        public object PopupContent
        {
            get => GetValue(PopupContentProperty);
            set => SetValue(PopupContentProperty, value);
        }
        #endregion


        public static readonly StyledProperty<SortDirection> SortDirectionProperty =
              AvaloniaProperty.Register<ComboFilter, SortDirection>(nameof(SortDirection), defaultValue: SortDirection.None, defaultBindingMode: BindingMode.TwoWay);

        public SortDirection SortDirection
        {
            get => GetValue(SortDirectionProperty);
            set => SetValue(SortDirectionProperty, value);
        }

        public static readonly StyledProperty<string> SortGroupNameProperty =
             AvaloniaProperty.Register<ComboFilter, string>(nameof(SortGroupName), defaultBindingMode: BindingMode.OneWay);

        public string SortGroupName
        {
            get => GetValue(SortGroupNameProperty);
            set => SetValue(SortGroupNameProperty, value);
        }

        public static readonly StyledProperty<bool> IsSortButtonVisibleProperty =
             AvaloniaProperty.Register<ComboFilter, bool>(nameof(IsSortButtonVisible), defaultValue: false);

        public bool IsSortButtonVisible
        {
            get => GetValue(IsSortButtonVisibleProperty);
            set => SetValue(IsSortButtonVisibleProperty, value);
        }

        public static readonly StyledProperty<string> SortNoneIconPathProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(SortNoneIconPath), defaultValue: "\\Assets\\Svg\\Components\\SortNone24.svg");

        public string SortNoneIconPath
        {
            get => GetValue(SortNoneIconPathProperty);
            set => SetValue(SortNoneIconPathProperty, value);
        }

        public static readonly StyledProperty<string> SortAscendingIconPathProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(SortAscendingIconPath), defaultValue: "\\Assets\\Svg\\Components\\SortDescending24.svg");

        public string SortAscendingIconPath
        {
            get => GetValue(SortAscendingIconPathProperty);
            set => SetValue(SortAscendingIconPathProperty, value);
        }
        
        public static readonly StyledProperty<string> SortDescendingIconPathProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(SortDescendingIconPath), defaultValue: "\\Assets\\Svg\\Components\\SortAscending24.svg");

        public string SortDescendingIconPath
        {
            get => GetValue(SortDescendingIconPathProperty);
            set => SetValue(SortDescendingIconPathProperty, value);
        }

        public static readonly StyledProperty<string> SortFieldProperty =
            AvaloniaProperty.Register<ComboFilter, string>(nameof(SortField), defaultBindingMode: BindingMode.TwoWay);

        public string SortField
        {
            get => GetValue(SortFieldProperty);
            set => SetValue(SortFieldProperty, value);
        }

        private Button _toggleButton;
        private Button _sortButton;
        private IDisposable _sortDirectionChangedSubscription;

        public ComboFilter()
        {
            this.WhenAnyValue(x => x.SortDirection)
                  .Subscribe(direction =>
                  {
                      if (!string.IsNullOrEmpty(SortGroupName))
                      {
                          MessageBus.Current.SendMessage(new SortDirectionChangedMessage(SortGroupName, this, SortField, direction));
                      }
                  });
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _toggleButton = e.NameScope.Find<Button>("PART_ToggleButton");
            _sortButton = e.NameScope.Find<Button>("PART_SortButton");
            var filterPopup = e.NameScope.Find<Popup>("PART_FilterPopup");

            if (_toggleButton != null && filterPopup != null)
            {
                _toggleButton.Click += (sender, args) =>
                {
                    IsPopupOpen = !IsPopupOpen;

                    if (IsPopupOpen && !string.IsNullOrEmpty(GroupName))
                    {
                        CloseOtherComboFiltersInGroup();
                    }

                    args.Handled = true;
                };
            }

            if (_sortButton != null)
            {
                _sortButton.Click += (sender, args) =>
                {
                    if (_sortButton.Tag is string tagValue)
                    {
                        SortField = tagValue;
                    }
                    else
                    {
                        Debug.WriteLine("ComboFilter: SortButton Tag is not a string or is null");
                    }

                    // Затем переключаем SortDirection
                    SortDirection = SortDirection switch
                    {
                        SortDirection.None => SortDirection.Descending,
                        SortDirection.Descending => SortDirection.Ascending,
                        SortDirection.Ascending => SortDirection.None,
                        _ => SortDirection.None
                    };

                    args.Handled = true;
                };
            }

            var window = this.GetVisualRoot() as Window;
            if (window != null)
            {
                window.Deactivated += OnWindowDeactivated;
            }

            _sortDirectionChangedSubscription?.Dispose();
            _sortDirectionChangedSubscription = MessageBus.Current.Listen<SortDirectionChangedMessage>()
                .Subscribe(message =>
                {
                    if (message.SortGroupName == SortGroupName && message.Sender != this && message.SortDirection != SortDirection.None)
                    {
                        SortDirection = SortDirection.None;
                    }
                });
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            var window = this.GetVisualRoot() as Window;
            if (window != null)
            {
                window.Deactivated -= OnWindowDeactivated;
            }

            _sortDirectionChangedSubscription?.Dispose();
        }

        private void OnWindowDeactivated(object? sender, EventArgs e)
        {
            IsPopupOpen = false;
        }

        private void CloseOtherComboFiltersInGroup()
        {
            var parent = this.GetVisualParent();
            if (parent == null) return;

            foreach (var child in parent.GetVisualDescendants())
            {
                if (child is ComboFilter comboFilter &&
                    comboFilter != this &&
                    comboFilter.GroupName == GroupName)
                {
                    comboFilter.IsPopupOpen = false;
                }
            }
        }
    }
}

public class SortDirectionChangedMessage
{
    public string SortGroupName { get; }
    public ComboFilter Sender { get; }
    public string SortField { get; }
    public SortDirection SortDirection { get; }

    public SortDirectionChangedMessage(string sortGroupName, ComboFilter sender, string sortField, SortDirection sortDirection)
    {
        SortGroupName = sortGroupName;
        Sender = sender;
        SortField = sortField;
        SortDirection = sortDirection;
    }
}



