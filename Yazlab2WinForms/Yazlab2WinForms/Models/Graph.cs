using System.Collections.Generic;
using System.Linq;

namespace Yazlab2WinForms.Models
{
    public class Graph
    {
        public List<Node> Nodes { get; } = new List<Node>();
        public List<Edge> Edges { get; } = new List<Edge>();

        public void AddNode(Node node)
        {
            if (!Nodes.Any(n => n.Name == node.Name))
                Nodes.Add(node);
        }

        public void AddEdge(Node from, Node to, RelationType type, int weight)
        {
            if (Nodes.Contains(from) && Nodes.Contains(to))
                Edges.Add(new Edge(from, to, type, weight));

            
            UpdateNodeStatistics();
        }

        // --- BAĞLANTI TOPLAMA MANTIĞI ---
        public void UpdateNodeStatistics()
        {
            foreach (var node in Nodes)
            {             
                node.IliskiGucu = Edges.Where(e => e.From == node || e.To == node).Sum(e => e.Weight);

                node.BaglantiSayisi = Edges.Count(e => e.From == node || e.To == node);
            }
        }

        public int[,] GetAdjacencyMatrix()
        {
          
            int n = Nodes.Count;
            int[,] matrix = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
 
                    var edge = Edges.FirstOrDefault(e =>
                        (e.From == Nodes[i] && e.To == Nodes[j]) ||
                        (e.To == Nodes[i] && e.From == Nodes[j]));

                    if (edge != null)
                    {
                        matrix[i, j] = edge.Weight;
                    }
                    else
                    {
                        matrix[i, j] = 0;
                    }
                }
            }
            return matrix;
        }
    }
}