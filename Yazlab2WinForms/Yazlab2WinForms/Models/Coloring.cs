using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Yazlab2WinForms.Abstract;

namespace Yazlab2WinForms.Models
{
    public class Coloring : IColoringService
    {
        // Welsh-Powell graf renklendirme
        public Dictionary<string, Color> ApplyWelshPowell(Graph graph)
        {
            Dictionary<string, Color> nodeColors = new Dictionary<string, Color>();
            if (graph.Nodes.Count == 0) return nodeColors;

            var sortedNodes = graph.Nodes
                .Select(n => new {
                    Node = n,
                    Degree = graph.Edges.Count(e => e.From == n || e.To == n)
                })
                .OrderByDescending(x => x.Degree).ToList();

            List<Color> palette = new List<Color> { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Purple, Color.Orange, Color.Pink, Color.Cyan };
            int currentColorIndex = 0;
            var remainingNodes = sortedNodes.Select(x => x.Node).ToList();

            while (remainingNodes.Count > 0)
            {
                Color currentColor = palette[currentColorIndex % palette.Count];
                List<Node> coloredInThisRound = new List<Node>();

                for (int i = 0; i < remainingNodes.Count; i++)
                {
                    Node currentNode = remainingNodes[i];
                    bool hasNeighborWithSameColor = coloredInThisRound.Any(node => IsNeighbor(graph, currentNode, node));

                    if (!hasNeighborWithSameColor)
                    {
                        nodeColors[currentNode.Name] = currentColor;
                        coloredInThisRound.Add(currentNode);
                    }
                }
                remainingNodes.RemoveAll(n => coloredInThisRound.Contains(n));
                currentColorIndex++;
            }
            return nodeColors;
        }

        private bool IsNeighbor(Graph graph, Node n1, Node n2)
        {
            return graph.Edges.Any(e => (e.From == n1 && e.To == n2) || (e.To == n1 && e.From == n2));
        }
    }
}