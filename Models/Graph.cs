using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace GraphSimulator.Models
{
    /// <summary>
    /// Represents a graph containing nodes and links
    /// </summary>
    public partial class Graph : ObservableObject
    {
        [ObservableProperty]
        private string name = "Untitled Graph";

        [ObservableProperty]
        private string description = "";

        [ObservableProperty]
        private double canvasZoom = 1.0;

        [ObservableProperty]
        private double canvasPanX = 0;

        [ObservableProperty]
        private double canvasPanY = 0;

        [ObservableProperty]
        private DateTime createdAt = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime modifiedAt = DateTime.UtcNow;

        [ObservableProperty]
        private string version = "1.0";

        [ObservableProperty]
        private string author = "Unknown";

        /// <summary>
        /// Collection of all nodes in the graph
        /// </summary>
        public ObservableCollection<Node> Nodes { get; } = new();

        /// <summary>
        /// Collection of all links in the graph
        /// </summary>
        public ObservableCollection<Link> Links { get; } = new();

        /// <summary>
        /// Predefined node types
        /// </summary>
        public static readonly string[] DefaultNodeTypes = 
        {
            "Process",
            "Data",
            "Decision",
            "Start",
            "End",
            "Input",
            "Output",
            "Database",
            "Document",
            "Custom"
        };

        /// <summary>
        /// Predefined colors for node types
        /// </summary>
    private static readonly System.Collections.Generic.Dictionary<string, string> NodeTypeColors = new()
{
    { "Process",  "#3A76C3" }, // deep blue
    { "Data",     "#1F5A8B" }, // darker steel-blue
    { "Decision", "#C15A4A" }, // muted red / brick
    { "Start",    "#2AA878" }, // teal-green
    { "End",      "#B03E3E" }, // darker red
    { "Input",    "#B37E29" }, // warm golden / amber-brown
    { "Output",   "#7D5FB5" }, // medium desaturated purple
    { "Database", "#2E3B46" }, // slate-grey / almost muted navy
    { "Document", "#3F6FA8" }, // slightly desaturated blue
    { "Custom",   "#6E777E" }  // neutral grey-blue
};


        public Graph()
        {
        }

        /// <summary>
        /// Gets the color for a node type
        /// </summary>
        public static string GetColorForNodeType(string nodeType)
        {
            return NodeTypeColors.TryGetValue(nodeType, out var color) ? color : "#95A5A6";
        }

        /// <summary>
        /// Adds a node to the graph
        /// </summary>
        public void AddNode(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (Nodes.Any(n => n.Id == node.Id))
                throw new InvalidOperationException("Node with this ID already exists");

            Nodes.Add(node);
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes a node from the graph and all connected links
        /// </summary>
        public void RemoveNode(Guid nodeId)
        {
            var node = Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
                return;

            // Remove all connected links
            var connectedLinks = Links.Where(l => l.SourceNodeId == nodeId || l.TargetNodeId == nodeId).ToList();
            foreach (var link in connectedLinks)
            {
                Links.Remove(link);
            }

            Nodes.Remove(node);
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a link to the graph
        /// </summary>
        public void AddLink(Link link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            if (!link.Validate())
                throw new InvalidOperationException("Link validation failed");

            // Check if link already exists
            if (Links.Any(l => l.SourceNodeId == link.SourceNodeId && l.TargetNodeId == link.TargetNodeId))
                throw new InvalidOperationException("Link already exists between these nodes");

            Links.Add(link);

            // Update node connections
            var sourceNode = Nodes.FirstOrDefault(n => n.Id == link.SourceNodeId);
            var targetNode = Nodes.FirstOrDefault(n => n.Id == link.TargetNodeId);

            if (sourceNode != null && !sourceNode.ConnectedLinks.Contains(link.Id))
                sourceNode.ConnectedLinks.Add(link.Id);

            if (targetNode != null && !targetNode.ConnectedLinks.Contains(link.Id))
                targetNode.ConnectedLinks.Add(link.Id);

            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes a link from the graph
        /// </summary>
        public void RemoveLink(Guid linkId)
        {
            var link = Links.FirstOrDefault(l => l.Id == linkId);
            if (link == null)
                return;

            Links.Remove(link);

            // Update node connections
            var sourceNode = Nodes.FirstOrDefault(n => n.Id == link.SourceNodeId);
            var targetNode = Nodes.FirstOrDefault(n => n.Id == link.TargetNodeId);

            if (sourceNode != null)
                sourceNode.ConnectedLinks.Remove(link.Id);

            if (targetNode != null)
                targetNode.ConnectedLinks.Remove(link.Id);

            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets statistics about the graph
        /// </summary>
        public GraphStatistics GetStatistics()
        {
            var nodeTypeDistribution = Nodes.GroupBy(n => n.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            return new GraphStatistics
            {
                TotalNodes = Nodes.Count,
                TotalLinks = Links.Count,
                NodeTypeDistribution = nodeTypeDistribution,
                OrphanedNodes = Nodes.Count(n => n.ConnectedLinks.Count == 0),
                MaxDepth = CalculateMaxDepth()
            };
        }

        /// <summary>
        /// Calculates the maximum depth of the graph
        /// </summary>
        private int CalculateMaxDepth()
        {
            if (Nodes.Count == 0)
                return 0;

            // Find all root nodes (no incoming links)
            var roots = Nodes.Where(n => !Links.Any(l => l.TargetNodeId == n.Id)).ToList();
            if (roots.Count == 0)
                return 0;

            int maxDepth = 0;
            foreach (var root in roots)
            {
                var depth = CalculateNodeDepth(root.Id, new System.Collections.Generic.HashSet<Guid>());
                maxDepth = Math.Max(maxDepth, depth);
            }

            return maxDepth;
        }

        /// <summary>
        /// Calculates the depth of a node in the graph
        /// </summary>
        private int CalculateNodeDepth(Guid nodeId, System.Collections.Generic.HashSet<Guid> visited)
        {
            if (visited.Contains(nodeId))
                return 0;

            visited.Add(nodeId);

            var outgoingLinks = Links.Where(l => l.SourceNodeId == nodeId);
            if (!outgoingLinks.Any())
                return 1;

            int maxChildDepth = 0;
            foreach (var link in outgoingLinks)
            {
                var childDepth = CalculateNodeDepth(link.TargetNodeId, visited);
                maxChildDepth = Math.Max(maxChildDepth, childDepth);
            }

            return maxChildDepth + 1;
        }

        /// <summary>
        /// Validates the entire graph
        /// </summary>
        public GraphValidationResult Validate()
        {
            var result = new GraphValidationResult { IsValid = true };

            // Check for orphaned nodes
            var orphanedNodes = Nodes.Where(n => n.ConnectedLinks.Count == 0).Select(n => n.Id).ToList();
            if (orphanedNodes.Any())
            {
                result.Warnings.Add($"Found {orphanedNodes.Count} orphaned node(s)");
            }

            // Check for invalid links
            foreach (var link in Links)
            {
                if (!link.Validate())
                {
                    result.IsValid = false;
                    result.Errors.Add($"Link {link.Id} is invalid");
                }

                if (!Nodes.Any(n => n.Id == link.SourceNodeId))
                {
                    result.IsValid = false;
                    result.Errors.Add($"Link {link.Id} references missing source node {link.SourceNodeId}");
                }

                if (!Nodes.Any(n => n.Id == link.TargetNodeId))
                {
                    result.IsValid = false;
                    result.Errors.Add($"Link {link.Id} references missing target node {link.TargetNodeId}");
                }
            }

            // Check for cycles
            if (HasCycles())
            {
                result.Warnings.Add("Graph contains cycles");
            }

            return result;
        }

        /// <summary>
        /// Checks if the graph contains cycles
        /// </summary>
        private bool HasCycles()
        {
            var visited = new System.Collections.Generic.HashSet<Guid>();
            var recursionStack = new System.Collections.Generic.HashSet<Guid>();

            foreach (var node in Nodes)
            {
                if (!visited.Contains(node.Id))
                {
                    if (HasCyclesUtil(node.Id, visited, recursionStack))
                        return true;
                }
            }

            return false;
        }

        private bool HasCyclesUtil(Guid nodeId, System.Collections.Generic.HashSet<Guid> visited, System.Collections.Generic.HashSet<Guid> recursionStack)
        {
            visited.Add(nodeId);
            recursionStack.Add(nodeId);

            var neighbors = Links.Where(l => l.SourceNodeId == nodeId).Select(l => l.TargetNodeId);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    if (HasCyclesUtil(neighbor, visited, recursionStack))
                        return true;
                }
                else if (recursionStack.Contains(neighbor))
                {
                    return true;
                }
            }

            recursionStack.Remove(nodeId);
            return false;
        }

        /// <summary>
        /// Clears the entire graph
        /// </summary>
        public void Clear()
        {
            Nodes.Clear();
            Links.Clear();
            ModifiedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Statistics about a graph
    /// </summary>
    public class GraphStatistics
    {
        public int TotalNodes { get; set; }
        public int TotalLinks { get; set; }
        public System.Collections.Generic.Dictionary<string, int> NodeTypeDistribution { get; set; } = new();
        public int OrphanedNodes { get; set; }
        public int MaxDepth { get; set; }
    }

    /// <summary>
    /// Result of graph validation
    /// </summary>
    public class GraphValidationResult
    {
        public bool IsValid { get; set; }
        public System.Collections.Generic.List<string> Errors { get; } = new();
        public System.Collections.Generic.List<string> Warnings { get; } = new();
    }
}
