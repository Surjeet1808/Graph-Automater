using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphSimulator.Models;

namespace GraphSimulator.ViewModels
{
    /// <summary>
    /// ViewModel for graph search and navigation
    /// </summary>
    public partial class SearchViewModel : ObservableObject
    {
        private readonly Graph _graph;

        [ObservableProperty]
        private string searchQuery = "";

        [ObservableProperty]
        private ObservableCollection<Node> searchResults = new();

        public SearchViewModel(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Searches for nodes by name, type, or ID
        /// </summary>
        [RelayCommand]
        public void Search()
        {
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            var query = SearchQuery.ToLower();

            var results = _graph.Nodes.Where(n =>
                n.Name.ToLower().Contains(query) ||
                n.Type.ToLower().Contains(query) ||
                n.Id.ToString().Contains(query)
            ).ToList();

            foreach (var node in results)
            {
                SearchResults.Add(node);
            }
        }

        /// <summary>
        /// Clears search results
        /// </summary>
        [RelayCommand]
        public void ClearSearch()
        {
            SearchQuery = "";
            SearchResults.Clear();
        }
    }

    /// <summary>
    /// ViewModel for graph validation
    /// </summary>
    public partial class ValidationViewModel : ObservableObject
    {
        private readonly Graph _graph;

        [ObservableProperty]
        private ObservableCollection<string> validationErrors = new();

        [ObservableProperty]
        private ObservableCollection<string> validationWarnings = new();

        [ObservableProperty]
        private bool isGraphValid = true;

        [ObservableProperty]
        private string validationSummary = "Click 'Validate' to check graph integrity";

        public ValidationViewModel(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Validates the entire graph
        /// </summary>
        [RelayCommand]
        public void ValidateGraph()
        {
            ValidationErrors.Clear();
            ValidationWarnings.Clear();

            var result = _graph.Validate();

            foreach (var error in result.Errors)
            {
                ValidationErrors.Add(error);
            }

            foreach (var warning in result.Warnings)
            {
                ValidationWarnings.Add(warning);
            }

            IsGraphValid = result.IsValid;

            var summary = $"Validation complete: {ValidationErrors.Count} errors, {ValidationWarnings.Count} warnings";
            if (!IsGraphValid)
                summary += " - Graph is INVALID";
            else
                summary += " - Graph is VALID";

            ValidationSummary = summary;
        }
    }

    /// <summary>
    /// ViewModel for JSON editor
    /// </summary>
    public partial class JsonEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string jsonContent = "{}";

        [ObservableProperty]
        private string errorMessage = "";

        [ObservableProperty]
        private bool hasError = false;

        /// <summary>
        /// Validates JSON syntax
        /// </summary>
        [RelayCommand]
        public void ValidateJson()
        {
            try
            {
                Newtonsoft.Json.Linq.JObject.Parse(JsonContent);
                HasError = false;
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"JSON Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Formats JSON with proper indentation
        /// </summary>
        [RelayCommand]
        public void FormatJson()
        {
            try
            {
                var parsed = Newtonsoft.Json.Linq.JObject.Parse(JsonContent);
                JsonContent = parsed.ToString(Newtonsoft.Json.Formatting.Indented);
                HasError = false;
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Format Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Minifies JSON
        /// </summary>
        [RelayCommand]
        public void MinifyJson()
        {
            try
            {
                var parsed = Newtonsoft.Json.Linq.JObject.Parse(JsonContent);
                JsonContent = parsed.ToString(Newtonsoft.Json.Formatting.None);
                HasError = false;
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Minify Error: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// ViewModel for layout algorithms
    /// </summary>
    public partial class LayoutViewModel : ObservableObject
    {
        private readonly Graph _graph;

        public LayoutViewModel(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Applies hierarchical layout
        /// </summary>
        [RelayCommand]
        public void ApplyHierarchicalLayout()
        {
            var nodes = _graph.Nodes.ToList();
            if (nodes.Count == 0)
                return;

            const double horizontalSpacing = 200;
            const double verticalSpacing = 150;

            // Simple hierarchical layout: arrange by depth
            var nodesByDepth = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Node>>();

            foreach (var node in nodes)
            {
                var depth = CalculateNodeDepth(node.Id);
                if (!nodesByDepth.ContainsKey(depth))
                    nodesByDepth[depth] = new System.Collections.Generic.List<Node>();
                nodesByDepth[depth].Add(node);
            }

            foreach (var (depth, depthNodes) in nodesByDepth)
            {
                for (int i = 0; i < depthNodes.Count; i++)
                {
                    depthNodes[i].X = i * horizontalSpacing;
                    depthNodes[i].Y = depth * verticalSpacing;
                }
            }
        }

        /// <summary>
        /// Applies circular layout
        /// </summary>
        [RelayCommand]
        public void ApplyCircularLayout()
        {
            var nodes = _graph.Nodes.ToList();
            if (nodes.Count == 0)
                return;

            const double radius = 300;
            const double centerX = 500;
            const double centerY = 400;

            for (int i = 0; i < nodes.Count; i++)
            {
                var angle = (2 * Math.PI * i) / nodes.Count;
                nodes[i].X = centerX + radius * Math.Cos(angle);
                nodes[i].Y = centerY + radius * Math.Sin(angle);
            }
        }

        /// <summary>
        /// Calculates the depth of a node
        /// </summary>
        private int CalculateNodeDepth(Guid nodeId, System.Collections.Generic.HashSet<Guid>? visited = null)
        {
            visited ??= new System.Collections.Generic.HashSet<Guid>();

            if (visited.Contains(nodeId))
                return 0;

            visited.Add(nodeId);

            var incomingLinks = _graph.Links.Where(l => l.TargetNodeId == nodeId);
            if (!incomingLinks.Any())
                return 0;

            int maxParentDepth = 0;
            foreach (var link in incomingLinks)
            {
                var parentDepth = CalculateNodeDepth(link.SourceNodeId, new System.Collections.Generic.HashSet<Guid>(visited));
                maxParentDepth = Math.Max(maxParentDepth, parentDepth);
            }

            return maxParentDepth + 1;
        }
    }
}
