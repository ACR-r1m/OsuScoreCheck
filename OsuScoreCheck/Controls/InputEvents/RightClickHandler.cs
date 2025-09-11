using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using System;
using System.Windows.Input;

namespace OsuScoreCheck.Controls.InputEvents
{
    public class RightClickHandler
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<RightClickHandler, Control, bool>(
                "IsEnabled", default, false, BindingMode.OneTime);

        public static readonly AttachedProperty<ContextMenu> MenuProperty =
            AvaloniaProperty.RegisterAttached<RightClickHandler, Control, ContextMenu>(
                "Menu");

        public static readonly AttachedProperty<ICommand> CommandProperty =
            AvaloniaProperty.RegisterAttached<RightClickHandler, Control, ICommand>(
                "Command");

        public static readonly AttachedProperty<object> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<RightClickHandler, Control, object>(
                "CommandParameter");

        static RightClickHandler()
        {
            IsEnabledProperty.Changed.Subscribe(OnIsEnabledChanged);
        }

        public static void SetIsEnabled(Control element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(Control element) =>
            element.GetValue(IsEnabledProperty);

        public static void SetMenu(Control element, ContextMenu value) =>
            element.SetValue(MenuProperty, value);

        public static ContextMenu GetMenu(Control element) =>
            element.GetValue(MenuProperty);

        public static void SetCommand(Control element, ICommand value) =>
            element.SetValue(CommandProperty, value);

        public static ICommand GetCommand(Control element) =>
            element.GetValue(CommandProperty);

        public static void SetCommandParameter(Control element, object value) =>
            element.SetValue(CommandParameterProperty, value);

        public static object GetCommandParameter(Control element) =>
            element.GetValue(CommandParameterProperty);

        private static void OnIsEnabledChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            if (args.Sender is not Control control)
                return;

            control.PointerPressed -= OnControlPointerPressed;

            if (args.NewValue.Value)
            {
                control.PointerPressed += OnControlPointerPressed;
            }
        }

        private static void OnControlPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is not Control control ||
                !e.GetCurrentPoint(control).Properties.IsRightButtonPressed)
                return;

            // Обработка контекстного меню
            var menu = GetMenu(control);
            if (menu != null)
            {
                menu.PlacementTarget = control;
                menu.Open(control);
                e.Handled = true;
                return;
            }

            // Обработка команды
            var command = GetCommand(control);
            var parameter = GetCommandParameter(control);

            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
                e.Handled = true;
            }
        }
    }
}
