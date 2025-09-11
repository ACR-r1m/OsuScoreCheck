using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using System;
using System.Windows.Input;

namespace OsuScoreCheck.Controls.InputEvents
{
    public class LeftClickHandler
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<LeftClickHandler, Control, bool>(
                "IsEnabled", default, false, BindingMode.OneTime);

        public static readonly AttachedProperty<ICommand> CommandProperty =
            AvaloniaProperty.RegisterAttached<LeftClickHandler, Control, ICommand>(
                "Command");

        public static readonly AttachedProperty<object> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<LeftClickHandler, Control, object>(
                "CommandParameter");

        static LeftClickHandler()
        {
            IsEnabledProperty.Changed.Subscribe(OnIsEnabledChanged);
        }

        public static void SetIsEnabled(Control element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(Control element) =>
            element.GetValue(IsEnabledProperty);

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

            control.PointerReleased -= OnControlPointerReleased;

            if (args.NewValue.Value)
            {
                control.PointerReleased += OnControlPointerReleased;
            }
        }

        private static void OnControlPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (sender is not Control control)
                return;

            if (e.InitialPressMouseButton == MouseButton.Left)
            {
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
}
