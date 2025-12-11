using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace GraphSimulator.Models
{
    /// <summary>
    /// Represents a node in the graph with properties and data
    /// </summary>
    public partial class Node : ObservableObject
    {
        [ObservableProperty]
        private Guid id;

        [ObservableProperty]
        private string name = "New Node";

        [ObservableProperty]
        private string type = "mouse_left_click";

        [ObservableProperty]
        private double x = 0;

        [ObservableProperty]
        private double y = 0;

        [ObservableProperty]
        private double width = 120;

        [ObservableProperty]
        private double height = 80;

        [ObservableProperty]
        private string jsonData = "{}";

        [ObservableProperty]
        private string color = "#1E3A5F"; // Default color for mouse_left_click

        [ObservableProperty]
        private bool isSelected = false;

        [ObservableProperty]
        private bool isLocked = false;

        [ObservableProperty]
        private DateTime createdAt = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime modifiedAt = DateTime.UtcNow;

        /// <summary>
        /// Collection of link IDs connected to this node (both incoming and outgoing)
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<Guid> ConnectedLinks { get; } = new();

        /// <summary>
        /// Collection of ports on this node (4 ports: top, right, bottom, left)
        /// </summary>
        public ObservableCollection<Port> Ports { get; set; } = new();

        public Node()
        {
            Id = Guid.NewGuid();
            InitializePorts();
        }

        /// <summary>
        /// Initialize the four ports for this node
        /// </summary>
        private void InitializePorts()
        {
            Ports.Clear();
            Ports.Add(new Port { NodeId = Id, Position = PortPosition.Top });
            Ports.Add(new Port { NodeId = Id, Position = PortPosition.Right });
            Ports.Add(new Port { NodeId = Id, Position = PortPosition.Bottom });
            Ports.Add(new Port { NodeId = Id, Position = PortPosition.Left });
        }

        /// <summary>
        /// Validates the node data
        /// </summary>
        public bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;

            // Validate JSON structure
            try
            {
                Newtonsoft.Json.Linq.JObject.Parse(JsonData);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a deep copy of this node
        /// </summary>
        public Node Clone()
        {
            var cloned = new Node
            {
                Id = Guid.NewGuid(),
                Name = Name + " (Copy)",
                Type = Type,
                X = X + 20,
                Y = Y + 20,
                Width = Width,
                Height = Height,
                JsonData = JsonData,
                Color = Color,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            // Ports are automatically initialized in Node constructor
            return cloned;
        }
    }
}
