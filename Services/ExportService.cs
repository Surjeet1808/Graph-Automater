using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GraphSimulator.Models;
using System.Threading.Tasks;
using System.IO;

namespace GraphSimulator.Services
{
    /// <summary>
    /// Service for exporting graphs as images
    /// </summary>
    public class ImageExportService
    {
        /// <summary>
        /// Exports graph canvas to PNG
        /// </summary>
        public async Task<bool> ExportToPngAsync(Visual visual, string filePath, int width = 1920, int height = 1080)
        {
            try
            {
                var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var stream = File.Create(filePath))
                {
                    encoder.Save(stream);
                }

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to PNG: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports graph canvas to JPEG
        /// </summary>
        public async Task<bool> ExportToJpegAsync(Visual visual, string filePath, int width = 1920, int height = 1080, int quality = 95)
        {
            try
            {
                var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);

                var encoder = new JpegBitmapEncoder { QualityLevel = quality };
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var stream = File.Create(filePath))
                {
                    encoder.Save(stream);
                }

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to JPEG: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports graph to SVG format
        /// </summary>
        public async Task<bool> ExportToSvgAsync(Graph graph, string filePath)
        {
            try
            {
                var svg = GenerateSvg(graph);
                await System.IO.File.WriteAllTextAsync(filePath, svg);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting to SVG: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generates SVG content from graph
        /// </summary>
        private string GenerateSvg(Graph graph)
        {
            var svg = new System.Text.StringBuilder();
            svg.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            svg.AppendLine("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"2000\" height=\"1500\">");
            svg.AppendLine("<defs><style type=\"text/css\"><![CDATA[");
            svg.AppendLine(".node { fill: white; stroke: black; stroke-width: 2; }");
            svg.AppendLine(".link { stroke: gray; stroke-width: 2; fill: none; }");
            svg.AppendLine(".text { font-family: Arial; font-size: 14px; text-anchor: middle; }");
            svg.AppendLine("]]></style></defs>");

            // Draw links first (so they appear behind nodes)
            foreach (var link in graph.Links)
            {
                var sourceNode = graph.Nodes.FirstOrDefault(n => n.Id == link.SourceNodeId);
                var targetNode = graph.Nodes.FirstOrDefault(n => n.Id == link.TargetNodeId);

                if (sourceNode != null && targetNode != null)
                {
                    var x1 = sourceNode.X + sourceNode.Width / 2;
                    var y1 = sourceNode.Y + sourceNode.Height / 2;
                    var x2 = targetNode.X + targetNode.Width / 2;
                    var y2 = targetNode.Y + targetNode.Height / 2;

                    svg.AppendLine($"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"link\" stroke=\"{link.Color}\"/>");

                    // Draw arrowhead
                    var angle = Math.Atan2(y2 - y1, x2 - x1);
                    var arrowSize = 10;
                    var arrowX = x2 - arrowSize * Math.Cos(angle);
                    var arrowY = y2 - arrowSize * Math.Sin(angle);

                    svg.AppendLine($"<polygon points=\"{x2},{y2} {arrowX - 5 * Math.Sin(angle)},{arrowY + 5 * Math.Cos(angle)} {arrowX + 5 * Math.Sin(angle)},{arrowY - 5 * Math.Cos(angle)}\" fill=\"{link.Color}\"/>");
                }
            }

            // Draw nodes
            foreach (var node in graph.Nodes)
            {
                var x = node.X;
                var y = node.Y;
                var width = node.Width;
                var height = node.Height;

                svg.AppendLine($"<rect x=\"{x}\" y=\"{y}\" width=\"{width}\" height=\"{height}\" rx=\"8\" class=\"node\" fill=\"{node.Color}\"/>");
                svg.AppendLine($"<text x=\"{x + width / 2}\" y=\"{y + height / 2 + 5}\" class=\"text\">{System.Net.WebUtility.HtmlEncode(node.Name)}</text>");
            }

            svg.AppendLine("</svg>");
            return svg.ToString();
        }
    }

    /// <summary>
    /// Service for generating PDF
    /// </summary>
    public class PdfExportService
    {
        /// <summary>
        /// Exports graph to PDF (requires additional library like iTextSharp)
        /// </summary>
        public async Task<bool> ExportToPdfAsync(Graph graph, string filePath)
        {
            // This would require additional libraries like iTextSharp or PdfSharp
            // For now, returning a placeholder implementation
            return await Task.FromResult(false);
        }
    }
}
