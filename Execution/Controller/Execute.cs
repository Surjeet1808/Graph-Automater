using System;
using System.Threading.Tasks;
using GraphSimulator.Execution.Model;
using GraphSimulator.Execution.Common;

namespace GraphSimulator.Execution.Controller
{
    /// <summary>
    /// Executes automation operations defined in OperationModel
    /// </summary>
    public class Execute
    {
        /// <summary>
        /// Execute a single operation
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        public async Task ExecuteOperationAsync(OperationModel operation)
        {
            // Validate operation
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (string.IsNullOrEmpty(operation.Type))
            {
                throw new ArgumentException("Operation type cannot be null or empty", nameof(operation));
            }

            // Skip if disabled
            if (!operation.Enabled)
            {
                return;
            }

            // Delay before execution
            if (operation.DelayBefore > 0)
            {
                await Task.Delay(operation.DelayBefore);
            }

            // Execute based on type
            try
            {
                switch (operation.Type.ToLower())
                {
                    case "mouse_left_click":
                        ValidateIntValues(operation, 2, "mouse_left_click requires x and y coordinates");
                        WpfInputHelper.ClickAt(operation.IntValues[0], operation.IntValues[1]);
                        break;

                    case "mouse_right_click":
                        ValidateIntValues(operation, 2, "mouse_right_click requires x and y coordinates");
                        WpfInputHelper.RightClickAt(operation.IntValues[0], operation.IntValues[1]);
                        break;

                    case "mouse_move":
                        ValidateIntValues(operation, 2, "mouse_move requires x and y coordinates");
                        WpfInputHelper.MoveTo(operation.IntValues[0], operation.IntValues[1]);
                        break;

                    case "scroll_up":
                        int upAmount = operation.IntValues.Length > 0 ? operation.IntValues[0] : 120;
                        WpfInputHelper.Scroll(upAmount);
                        break;

                    case "scroll_down":
                        int downAmount = operation.IntValues.Length > 0 ? operation.IntValues[0] : 120;
                        WpfInputHelper.Scroll(-downAmount);
                        break;

                    case "key_press":
                        ValidateIntValues(operation, 1, "key_press requires a key code");
                        WpfInputHelper.PressKey((byte)operation.IntValues[0]);
                        break;

                    case "key_down":
                        ValidateIntValues(operation, 1, "key_down requires a key code");
                        WpfInputHelper.KeyDown((byte)operation.IntValues[0]);
                        break;

                    case "key_up":
                        ValidateIntValues(operation, 1, "key_up requires a key code");
                        WpfInputHelper.KeyUp((byte)operation.IntValues[0]);
                        break;

                    case "type_text":
                        ValidateStringValues(operation, 1, "type_text requires text to type");
                        WpfInputHelper.TypeText(operation.StringValues[0]);
                        break;

                    case "wait":
                        ValidateIntValues(operation, 1, "wait requires duration in milliseconds");
                        await Task.Delay(operation.IntValues[0]);
                        break;

                    case "custom_code":
                        if (string.IsNullOrEmpty(operation.CustomCode))
                        {
                            throw new ArgumentException("CustomCode cannot be empty for custom_code operation");
                        }
                        // Execute custom code (implement as needed)
                        throw new NotImplementedException("Custom code execution not yet implemented");

                    default:
                        throw new NotSupportedException($"Operation type '{operation.Type}' is not supported.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to execute operation '{operation.Type}': {ex.Message}", 
                    ex
                );
            }

            // Delay after execution
            if (operation.DelayAfter > 0)
            {
                await Task.Delay(operation.DelayAfter);
            }
        }

        /// <summary>
        /// Execute multiple operations in sequence
        /// </summary>
        /// <param name="operations">Array of operations to execute</param>
        public async Task ExecuteOperationsAsync(params OperationModel[] operations)
        {
            if (operations == null || operations.Length == 0)
            {
                return;
            }

            // Sort by priority (lower priority value = execute first)
            var sortedOps = operations.OrderBy(op => op.Priority).ToArray();

            foreach (var operation in sortedOps)
            {
                await ExecuteOperationAsync(operation);
            }
        }

        /// <summary>
        /// Execute operations from a list
        /// </summary>
        public async Task ExecuteOperationsAsync(List<OperationModel> operations)
        {
            if (operations == null || operations.Count == 0)
            {
                return;
            }

            await ExecuteOperationsAsync(operations.ToArray());
        }

        private void ValidateIntValues(OperationModel operation, int minCount, string errorMessage)
        {
            if (operation.IntValues == null || operation.IntValues.Length < minCount)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        private void ValidateStringValues(OperationModel operation, int minCount, string errorMessage)
        {
            if (operation.StringValues == null || operation.StringValues.Length < minCount)
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}