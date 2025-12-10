using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GraphSimulator.Models;

namespace GraphSimulator.Services
{
    /// <summary>
    /// Service for saving and loading graph files
    /// </summary>
    public class FileService
    {
        public const string DefaultExtension = ".graph";
        private readonly string _applicationDataPath;

        public FileService()
        {
            _applicationDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GraphSimulator"
            );
            Directory.CreateDirectory(_applicationDataPath);
        }

        /// <summary>
        /// Saves a graph to file
        /// </summary>
        public async Task<bool> SaveGraphAsync(Graph graph, string filePath)
        {
            try
            {
                var graphData = new GraphFileData
                {
                    Version = "1.0",
                    Graph = graph
                };

                var json = JsonConvert.SerializeObject(graphData, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving graph: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads a graph from file
        /// </summary>
        public async Task<Graph?> LoadGraphAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var graphData = JsonConvert.DeserializeObject<GraphFileData>(json);

                return graphData?.Graph;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading graph: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Exports graph as JSON
        /// </summary>
        public async Task<bool> ExportAsJsonAsync(Graph graph, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(graph, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting graph as JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports graph as CSV (node and edge lists)
        /// </summary>
        public async Task<bool> ExportAsCsvAsync(Graph graph, string directoryPath)
        {
            try
            {
                Directory.CreateDirectory(directoryPath);

                // Export nodes
                var nodesCsv = "ID,Name,Type,X,Y,Color,JSONData\n";
                foreach (var node in graph.Nodes)
                {
                    var jsonEscaped = node.JsonData.Replace("\"", "\"\"");
                    nodesCsv += $"{node.Id},\"{node.Name}\",{node.Type},{node.X},{node.Y},{node.Color},\"{jsonEscaped}\"\n";
                }
                await File.WriteAllTextAsync(Path.Combine(directoryPath, "nodes.csv"), nodesCsv);

                // Export links
                var linksCsv = "ID,SourceNodeID,TargetNodeID,Label,Weight,Color,LineStyle,ArrowStyle\n";
                foreach (var link in graph.Links)
                {
                    linksCsv += $"{link.Id},{link.SourceNodeId},{link.TargetNodeId},\"{link.Label}\",{link.Weight},{link.Color},{link.LineStyle},{link.ArrowStyle}\n";
                }
                await File.WriteAllTextAsync(Path.Combine(directoryPath, "links.csv"), linksCsv);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting graph as CSV: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports graph as DOT format (Graphviz)
        /// </summary>
        public async Task<bool> ExportAsDotAsync(Graph graph, string filePath)
        {
            try
            {
                var dot = "digraph G {\n";
                dot += "    rankdir=LR;\n";
                dot += "    node [shape=box];\n";

                // Add nodes
                foreach (var node in graph.Nodes)
                {
                    var label = node.Name.Replace("\"", "\\\"");
                    dot += $"    \"{node.Id}\" [label=\"{label}\"];\n";
                }

                // Add edges
                foreach (var link in graph.Links)
                {
                    var linkLabel = string.IsNullOrEmpty(link.Label) ? "" : $" [{link.Label}]";
                    dot += $"    \"{link.SourceNodeId}\" -> \"{link.TargetNodeId}\"{linkLabel};\n";
                }

                dot += "}\n";

                await File.WriteAllTextAsync(filePath, dot);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting graph as DOT: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets recent file paths
        /// </summary>
        public List<string> GetRecentFiles()
        {
            var recentFilesPath = Path.Combine(_applicationDataPath, "recent.json");
            if (!File.Exists(recentFilesPath))
                return new List<string>();

            try
            {
                var json = File.ReadAllText(recentFilesPath);
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Saves recent file path
        /// </summary>
        public void SaveRecentFile(string filePath)
        {
            try
            {
                var recentFiles = GetRecentFiles();
                recentFiles.Remove(filePath);
                recentFiles.Insert(0, filePath);

                // Keep only last 10 recent files
                if (recentFiles.Count > 10)
                    recentFiles = recentFiles.GetRange(0, 10);

                var json = JsonConvert.SerializeObject(recentFiles);
                var recentFilesPath = Path.Combine(_applicationDataPath, "recent.json");
                File.WriteAllText(recentFilesPath, json);
            }
            catch
            {
                // Silently fail
            }
        }
    }

    /// <summary>
    /// Data structure for saving graph to file
    /// </summary>
    public class GraphFileData
    {
        public string Version { get; set; } = "1.0";
        public Graph? Graph { get; set; }
    }
}
