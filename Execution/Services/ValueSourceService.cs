using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GraphSimulator.Execution.Model;

namespace GraphSimulator.Execution.Services
{
    /// <summary>
    /// Service to resolve operation values based on ValueSource type
    /// </summary>
    public static class ValueSourceService
    {
        /// <summary>
        /// Resolves the values for an operation based on its ValueSource
        /// </summary>
        /// <param name="operation">The operation model</param>
        /// <param name="nodeIdentifier">Node identifier in format: NodeName-NodeID (required for date-json)</param>
        /// <returns>Resolved values containing IntValues and StringValues</returns>
        public static ResolvedValues ResolveValues(OperationModel operation, string? nodeIdentifier = null)
        {
            switch (operation.ValueSource?.ToLower())
            {
                case "node":
                    return ResolveFromNode(operation);
                
                case "date-map":
                    return ResolveFromDateMap(operation);
                
                case "date-json":
                    if (string.IsNullOrEmpty(nodeIdentifier))
                    {
                        throw new InvalidOperationException("Node identifier is required for date-json value source");
                    }
                    return ResolveFromDateJson(operation, nodeIdentifier);
                
                default:
                    // Default to node behavior
                    return ResolveFromNode(operation);
            }
        }

        /// <summary>
        /// Get values directly from node data (current behavior)
        /// </summary>
        private static ResolvedValues ResolveFromNode(OperationModel operation)
        {
            return new ResolvedValues
            {
                IntValues = operation.IntValues ?? Array.Empty<int>(),
                StringValues = operation.StringValues ?? Array.Empty<string>()
            };
        }

        /// <summary>
        /// Get values from date map based on current date
        /// </summary>
        private static ResolvedValues ResolveFromDateMap(OperationModel operation)
        {
            if (operation.DateMap == null || operation.DateMap.Count == 0)
            {
                // Fall back to default values from node
                return ResolveFromNode(operation);
            }

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Try to find exact date match
            if (operation.DateMap.TryGetValue(currentDate, out var dateValues))
            {
                return new ResolvedValues
                {
                    IntValues = dateValues.IntValues ?? Array.Empty<int>(),
                    StringValues = dateValues.StringValues ?? Array.Empty<string>()
                };
            }

            // No match found, fall back to default values from node
            return ResolveFromNode(operation);
        }

        /// <summary>
        /// Get values from external JSON file based on current date and node identifier
        /// </summary>
        private static ResolvedValues ResolveFromDateJson(OperationModel operation, string nodeIdentifier)
        {
            if (string.IsNullOrEmpty(operation.DateJsonFilePath))
            {
                throw new InvalidOperationException(
                    $"DateJsonFilePath is not set for node '{nodeIdentifier}'.\n" +
                    "Please select a JSON file in the Value Source settings.");
            }

            if (!File.Exists(operation.DateJsonFilePath))
            {
                throw new FileNotFoundException(
                    $"Date JSON file not found: {operation.DateJsonFilePath}\n" +
                    $"Node: {nodeIdentifier}");
            }

            try
            {
                string jsonContent = File.ReadAllText(operation.DateJsonFilePath);
                var dateObjects = JsonSerializer.Deserialize<List<JsonElement>>(jsonContent);

                if (dateObjects == null || dateObjects.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Date JSON file is empty or invalid: {operation.DateJsonFilePath}\n" +
                        $"Node: {nodeIdentifier}");
                }

                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Find the object for current date
                var dateObject = dateObjects.FirstOrDefault(obj =>
                {
                    if (obj.TryGetProperty("date", out var dateProperty))
                    {
                        return dateProperty.GetString() == currentDate;
                    }
                    return false;
                });

                if (dateObject.ValueKind == JsonValueKind.Undefined)
                {
                    throw new InvalidOperationException(
                        $"No data found for date: {currentDate}\n" +
                        $"File: {operation.DateJsonFilePath}\n" +
                        $"Node: {nodeIdentifier}\n\n" +
                        "Execution stopped. Please add data for this date in your JSON file.");
                }

                // Find the node's data within the date object
                if (!dateObject.TryGetProperty(nodeIdentifier, out var nodeData))
                {
                    throw new InvalidOperationException(
                        $"No data found for node identifier: '{nodeIdentifier}'\n" +
                        $"Date: {currentDate}\n" +
                        $"File: {operation.DateJsonFilePath}\n\n" +
                        "Execution stopped. Please add this node's data in your JSON file.");
                }

                // Extract IntValues and StringValues
                int[] intValues = Array.Empty<int>();
                string[] stringValues = Array.Empty<string>();

                if (nodeData.TryGetProperty("IntValues", out var intValuesElement) && 
                    intValuesElement.ValueKind == JsonValueKind.Array)
                {
                    intValues = intValuesElement.EnumerateArray()
                        .Select(e => e.GetInt32())
                        .ToArray();
                }

                if (nodeData.TryGetProperty("StringValues", out var stringValuesElement) && 
                    stringValuesElement.ValueKind == JsonValueKind.Array)
                {
                    stringValues = stringValuesElement.EnumerateArray()
                        .Select(e => e.GetString() ?? "")
                        .ToArray();
                }

                return new ResolvedValues
                {
                    IntValues = intValues,
                    StringValues = stringValues
                };
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Invalid JSON format in file: {operation.DateJsonFilePath}\n" +
                    $"Node: {nodeIdentifier}\n" +
                    $"Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Represents resolved values from a value source
    /// </summary>
    public class ResolvedValues
    {
        public int[] IntValues { get; set; } = Array.Empty<int>();
        public string[] StringValues { get; set; } = Array.Empty<string>();
    }
}
