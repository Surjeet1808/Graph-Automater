using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace GraphSimulator.Models
{
    /// <summary>
    /// Represents a directed link/edge between two nodes
    /// </summary>
    public partial class Link : ObservableObject
    {
        [ObservableProperty]
        private Guid id;

        [ObservableProperty]
        private Guid sourceNodeId;

        [ObservableProperty]
        private Guid sourcePortId;

        [ObservableProperty]
        private Guid targetNodeId;

        [ObservableProperty]
        private Guid targetPortId;

        [ObservableProperty]
        private string label = "";

        [ObservableProperty]
        private double weight = 1.0;

        [ObservableProperty]
        private string color = "#666666";

        [ObservableProperty]
        private double thickness = 2.0;

        [ObservableProperty]
        private string lineStyle = "Solid"; // Solid, Dashed, Dotted

        [ObservableProperty]
        private string arrowStyle = "Filled"; // Filled, Open, Simple

        [ObservableProperty]
        private bool isSelected = false;

        [ObservableProperty]
        private DateTime createdAt = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime modifiedAt = DateTime.UtcNow;

        [ObservableProperty]
        private string jsonData = "{}";

        public Link()
        {
            Id = Guid.NewGuid();
        }

        public Link(Guid sourceId, Guid targetId)
        {
            Id = Guid.NewGuid();
            SourceNodeId = sourceId;
            TargetNodeId = targetId;
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Validates the link
        /// </summary>
        public bool Validate()
        {
            // Check for self-loops
            if (SourceNodeId == TargetNodeId)
                return false;

            if (SourceNodeId == Guid.Empty || TargetNodeId == Guid.Empty)
                return false;

            // Validate JSON if present
            if (!string.IsNullOrEmpty(JsonData))
            {
                try
                {
                    Newtonsoft.Json.Linq.JObject.Parse(JsonData);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a deep copy of this link
        /// </summary>
        public Link Clone()
        {
            return new Link
            {
                Id = Guid.NewGuid(),
                SourceNodeId = SourceNodeId,
                TargetNodeId = TargetNodeId,
                Label = Label,
                Weight = Weight,
                Color = Color,
                Thickness = Thickness,
                LineStyle = LineStyle,
                ArrowStyle = ArrowStyle,
                JsonData = JsonData,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
        }
    }
}
