using System;

namespace GraphSimulator.Services
{
    /// <summary>
    /// Base interface for undoable commands
    /// </summary>
    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
        string GetDescription();
    }

    /// <summary>
    /// Manages undo/redo operations
    /// </summary>
    public class CommandHistory
    {
        private readonly System.Collections.Generic.Stack<IUndoableCommand> _undoStack = new();
        private readonly System.Collections.Generic.Stack<IUndoableCommand> _redoStack = new();
        private readonly int _maxHistorySize;

        public CommandHistory(int maxHistorySize = 100)
        {
            _maxHistorySize = maxHistorySize;
        }

        public event EventHandler<CommandHistoryChangedEventArgs>? HistoryChanged;

        /// <summary>
        /// Executes a command and adds it to the history
        /// </summary>
        public void Execute(IUndoableCommand command)
        {
            command.Execute();
            _undoStack.Push(command);

            // Clear redo stack
            _redoStack.Clear();

            // Limit history size
            if (_undoStack.Count > _maxHistorySize)
            {
                var items = _undoStack.ToArray();
                _undoStack.Clear();
                for (int i = 0; i < items.Length - 1; i++)
                {
                    _undoStack.Push(items[i]);
                }
            }

            HistoryChanged?.Invoke(this, new CommandHistoryChangedEventArgs 
            { 
                CanUndo = CanUndo, 
                CanRedo = CanRedo,
                UndoDescription = GetUndoDescription(),
                RedoDescription = GetRedoDescription()
            });
        }

        /// <summary>
        /// Undoes the last command
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count == 0)
                return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            HistoryChanged?.Invoke(this, new CommandHistoryChangedEventArgs 
            { 
                CanUndo = CanUndo, 
                CanRedo = CanRedo,
                UndoDescription = GetUndoDescription(),
                RedoDescription = GetRedoDescription()
            });
        }

        /// <summary>
        /// Redoes the last undone command
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count == 0)
                return;

            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);

            HistoryChanged?.Invoke(this, new CommandHistoryChangedEventArgs 
            { 
                CanUndo = CanUndo, 
                CanRedo = CanRedo,
                UndoDescription = GetUndoDescription(),
                RedoDescription = GetRedoDescription()
            });
        }

        /// <summary>
        /// Clears all command history
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();

            HistoryChanged?.Invoke(this, new CommandHistoryChangedEventArgs 
            { 
                CanUndo = false, 
                CanRedo = false 
            });
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public string GetUndoDescription() => CanUndo ? $"Undo: {_undoStack.Peek().GetDescription()}" : "Undo";
        public string GetRedoDescription() => CanRedo ? $"Redo: {_redoStack.Peek().GetDescription()}" : "Redo";
    }

    public class CommandHistoryChangedEventArgs : EventArgs
    {
        public bool CanUndo { get; set; }
        public bool CanRedo { get; set; }
        public string? UndoDescription { get; set; }
        public string? RedoDescription { get; set; }
    }
}
