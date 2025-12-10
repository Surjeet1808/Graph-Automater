using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphSimulator.Models;

namespace GraphSimulator.Services
{
    /// <summary>
    /// Service for calculating orthogonal link routes with intelligent path optimization
    /// </summary>
    public class OrthogonalRoutingService
    {
        private const double GridSize = 10.0;

        /// <summary>
        /// Calculates an orthogonal L-shaped or multi-bend path between two ports
        /// </summary>
        public List<Point> CalculateRoute(Point sourcePos, Point targetPos, List<(double x, double y, double w, double h)>? obstacles = null)
        {
            obstacles ??= new List<(double, double, double, double)>();

            // Snap to grid
            sourcePos = SnapToGrid(sourcePos);
            targetPos = SnapToGrid(targetPos);

            // Try simple L-shaped path first (horizontal then vertical)
            var path1 = CalculateLShape(sourcePos, targetPos, true);
            var path2 = CalculateLShape(sourcePos, targetPos, false);

            // Choose path with fewer intersections with obstacles
            var intersections1 = CountPathObstacleIntersections(path1, obstacles);
            var intersections2 = CountPathObstacleIntersections(path2, obstacles);

            return intersections1 <= intersections2 ? path1 : path2;
        }

        /// <summary>
        /// Calculates an L-shaped path (2 segments: horizontal + vertical or vice versa)
        /// </summary>
        private List<Point> CalculateLShape(Point start, Point end, bool horizontalFirst)
        {
            var path = new List<Point> { start };

            if (horizontalFirst)
            {
                // Go horizontal first, then vertical
                var midX = start.X + (end.X - start.X) / 2;
                path.Add(new Point(midX, start.Y));
                path.Add(new Point(midX, end.Y));
            }
            else
            {
                // Go vertical first, then horizontal
                var midY = start.Y + (end.Y - start.Y) / 2;
                path.Add(new Point(start.X, midY));
                path.Add(new Point(end.X, midY));
            }

            path.Add(end);
            return path;
        }

        /// <summary>
        /// Counts how many times a path intersects with obstacles
        /// </summary>
        private int CountPathObstacleIntersections(List<Point> path, List<(double x, double y, double w, double h)> obstacles)
        {
            int intersections = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                var segmentStart = path[i];
                var segmentEnd = path[i + 1];

                foreach (var obstacle in obstacles)
                {
                    if (SegmentIntersectsRect(segmentStart, segmentEnd, obstacle.x, obstacle.y, obstacle.w, obstacle.h))
                    {
                        intersections++;
                    }
                }
            }

            return intersections;
        }

        /// <summary>
        /// Checks if a line segment intersects with a rectangle
        /// </summary>
        private bool SegmentIntersectsRect(Point p1, Point p2, double rx, double ry, double rw, double rh)
        {
            // Check if segment is within or touching rectangle bounds
            double minX = Math.Min(p1.X, p2.X);
            double maxX = Math.Max(p1.X, p2.X);
            double minY = Math.Min(p1.Y, p2.Y);
            double maxY = Math.Max(p1.Y, p2.Y);

            // Add padding to avoid touching edges
            const double padding = 10;
            rx -= padding;
            ry -= padding;
            rw += padding * 2;
            rh += padding * 2;

            return !(maxX < rx || minX > rx + rw || maxY < ry || minY > ry + rh);
        }

        /// <summary>
        /// Snaps a point to the invisible grid
        /// </summary>
        private Point SnapToGrid(Point p)
        {
            return new Point(
                Math.Round(p.X / GridSize) * GridSize,
                Math.Round(p.Y / GridSize) * GridSize
            );
        }

        /// <summary>
        /// Calculates port position on a node given the port position
        /// </summary>
        public Point CalculatePortPosition(Node node, PortPosition portPos)
        {
            double centerX = node.X + node.Width / 2;
            double centerY = node.Y + node.Height / 2;

            return portPos switch
            {
                PortPosition.Top => new Point(centerX, node.Y),
                PortPosition.Right => new Point(node.X + node.Width, centerY),
                PortPosition.Bottom => new Point(centerX, node.Y + node.Height),
                PortPosition.Left => new Point(node.X, centerY),
                _ => new Point(centerX, centerY)
            };
        }

        /// <summary>
        /// Gets the best port to use for connecting to a target node
        /// </summary>
        public PortPosition GetBestPortForTarget(Node source, Node target)
        {
            // Simple heuristic: choose port closest to target
            var targetCenter = new Point(target.X + target.Width / 2, target.Y + target.Height / 2);
            var sourceCenter = new Point(source.X + source.Width / 2, source.Y + source.Height / 2);

            double dx = targetCenter.X - sourceCenter.X;
            double dy = targetCenter.Y - sourceCenter.Y;

            // Return port closest to target direction
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                return dx > 0 ? PortPosition.Right : PortPosition.Left;
            }
            else
            {
                return dy > 0 ? PortPosition.Bottom : PortPosition.Top;
            }
        }
    }
}
