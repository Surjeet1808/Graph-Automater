using System.Windows.Input;

namespace GraphSimulator.Services
{
    /// <summary>
    /// Custom application commands
    /// </summary>
    public static class ApplicationCommands
    {
        public static readonly RoutedCommand NewGraph = new(
            nameof(NewGraph), 
            typeof(ApplicationCommands), 
            new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand OpenGraph = new(
            nameof(OpenGraph),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand SaveGraph = new(
            nameof(SaveGraph),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand SaveGraphAs = new(
            nameof(SaveGraphAs),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) }
        );

        public static readonly RoutedCommand Undo = new(
            nameof(Undo),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Z, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand Redo = new(
            nameof(Redo),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Y, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand AddNode = new(
            nameof(AddNode),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Alt) }
        );

        public static readonly RoutedCommand DeleteSelected = new(
            nameof(DeleteSelected),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Delete) }
        );

        public static readonly RoutedCommand SelectAll = new(
            nameof(SelectAll),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.A, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand Find = new(
            nameof(Find),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.F, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand ZoomIn = new(
            nameof(ZoomIn),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Add, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand ZoomOut = new(
            nameof(ZoomOut),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.Subtract, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand FitToScreen = new(
            nameof(FitToScreen),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.D0, ModifierKeys.Control) }
        );

        public static readonly RoutedCommand ToggleGrid = new(
            nameof(ToggleGrid),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.G, ModifierKeys.Control | ModifierKeys.Shift) }
        );

        public static readonly RoutedCommand ToggleDarkMode = new(
            nameof(ToggleDarkMode),
            typeof(ApplicationCommands),
            new InputGestureCollection { new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift) }
        );
    }
}
