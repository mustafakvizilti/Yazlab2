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

        public int[,] GetAdjacencyMatrix()
        {
            // 1. Düğüm sayısına göre kare bir matris (tablo) oluşturuyoruz.
            int n = Nodes.Count;
            int[,] matrix = new int[n, n];

            // 2. Tablonun her satırı ve sütunu için tek tek kontrol yapıyoruz.
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    // Eğer i. düğüm ile j. düğüm arasında bir bağlantı varsa bul.
                    var edge = Edges.FirstOrDefault(e =>
                        (e.From == Nodes[i] && e.To == Nodes[j]) ||
                        (e.To == Nodes[i] && e.From == Nodes[j]));

                    if (edge != null)
                    {
                        // Bağlantı varsa, o kutucuğa bağlantının ağırlığını (Weight) yaz.
                        matrix[i, j] = edge.Weight;
                    }
                    else
                    {
                        // Bağlantı yoksa kutucuk 0 kalır.
                        matrix[i, j] = 0;
                    }
                }
            }
            return matrix;
        }
    }
}