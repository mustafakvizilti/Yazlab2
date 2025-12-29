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

            // Her kenar eklendiğinde düğüm puanlarını güncelle
            UpdateNodeStatistics();
        }

        // --- BAĞLANTI TOPLAMA MANTIĞI ---
        public void UpdateNodeStatistics()
        {
            foreach (var node in Nodes)
            {
                // Düğümün toplam ilişki gücü (bağlantıların weight toplamı)
                node.IliskiGucu = Edges.Where(e => e.From == node || e.To == node).Sum(e => e.Weight);

                // Düğümün toplam bağlantı sayısı
                node.BaglantiSayisi = Edges.Count(e => e.From == node || e.To == node);
            }
        }
    }
}