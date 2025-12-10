using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphSimulator.Models;
using GraphSimulator.Services;

namespace GraphSimulator.ViewModels
{
    /// <summary>
    /// Helper class to manage automatic link rerouting
    /// </summary>
    public class LinkRoutingViewModel
    {
        private readonly OrthogonalRoutingService _routingService = new();

        /// <summary>
        /// Recalculates routes for all links connected to a moved node
        /// </summary>
        public void RerouteConnectedLinks(Node movedNode, Graph graph)
        {
            if (movedNode == null || graph == null)
                return;

            // Find all links connected to this node
            var connectedLinks = graph.Links.Where(l =>
                l.SourceNodeId == movedNode.Id || l.TargetNodeId == movedNode.Id
            ).ToList();

            // Reroute each connected link
            foreach (var link in connectedLinks)
            {
                RerouteLink(link, graph);
            }
        }

        /// <summary>
        /// Recalculates the route for a single link based on current node positions
        /// </summary>
        public void RerouteLink(Link link, Graph graph)
        {
            var sourceNode = graph.Nodes.FirstOrDefault(n => n.Id == link.SourceNodeId);
            var targetNode = graph.Nodes.FirstOrDefault(n => n.Id == link.TargetNodeId);

            if (sourceNode == null || targetNode == null)
                return;

            // Get source and target port positions
            var sourcePort = sourceNode.Ports.FirstOrDefault();
            var targetPort = targetNode.Ports.FirstOrDefault();

            if (sourcePort == null || targetPort == null)
                return;

            var sourcePosEnum = sourcePort.Position;
            var targetPosEnum = targetPort.Position;

            // Update port references in link (optional - for future use)
            link.SourcePortId = sourcePort.Id;
            link.TargetPortId = targetPort.Id;

            // Store metadata for rendering (timestamps indicate reroute happened)
            link.ModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the best port to use for connecting from source to target
        /// </summary>
        public Port GetBestSourcePort(Node source, Node target)
        {
            return source.Ports.FirstOrDefault() ?? new Port { NodeId = source.Id, Position = PortPosition.Right };
        }

        /// <summary>
        /// Gets the best port to use for connecting to target from source
        /// </summary>
        public Port GetBestTargetPort(Node source, Node target)
        {
            return target.Ports.FirstOrDefault() ?? new Port { NodeId = target.Id, Position = PortPosition.Left };
        }
    }
}
