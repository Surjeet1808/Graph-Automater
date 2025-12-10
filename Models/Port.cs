using System;
using System.Windows;

namespace GraphSimulator.Models
{
    /// <summary>
    /// Represents a port on a node where links can connect
    /// </summary>
    public class Port
    {
        /// <summary>
        /// Unique identifier for this port
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Parent node ID
        /// </summary>
        public Guid NodeId { get; set; }

        /// <summary>
        /// Port position relative to node (Top, Right, Bottom, Left)
        /// </summary>
        public PortPosition Position { get; set; }

        /// <summary>
        /// Absolute canvas position of the port
        /// </summary>
        public Point CanvasPosition { get; set; }

        /// <summary>
        /// Port radius for rendering and hit detection
        /// </summary>
        public double Radius { get; set; } = 5;

        public override string ToString() => $"Port_{Position}_{NodeId}";
    }

    /// <summary>
    /// Port position relative to node
    /// </summary>
    public enum PortPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }
}
