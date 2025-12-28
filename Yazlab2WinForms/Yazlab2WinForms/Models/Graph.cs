using System;
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
        }

        // --- YENİ SOSYAL MALİYET HESABI ---
        // İlişki şiddeti arttıkça maliyet (mesafe) azalır           Maliyet= 100 / Toplam İlişki Şiddeti
        public double GetSocialDistance(Node u, Node v)
        {
            var matchingEdges = this.Edges.Where(e =>
                (e.From == u && e.To == v) || (e.From == v && e.To == u)).ToList();

            if (!matchingEdges.Any()) return double.MaxValue;

            // Tüm paralel bağların ağırlıklarını topla
            double totalStrength = matchingEdges.Sum(e => e.Weight);

            // Formül: Güç ne kadar yüksekse maliyet o kadar düşük (100 / Toplam Güç)
            return 100.0 / totalStrength;
        }

        // --- TÜM EŞİT EN KISA YOLLARI BULAN ALGORİTMA ---
        public List<List<Node>> FindAllShortestPaths(Node start, Node end)
        {
            var distances = new Dictionary<Node, double>();
            var parents = new Dictionary<Node, List<Node>>();
            var nodes = this.Nodes.ToList();

            foreach (var node in nodes)
            {
                distances[node] = double.MaxValue;
                parents[node] = new List<Node>();
            }

            distances[start] = 0;
            var queue = new List<Node>(nodes);

            while (queue.Any())
            {
                var u = queue.OrderBy(n => distances[n]).First();
                queue.Remove(u);

                if (u == end) break;

                var neighbors = this.Edges.Where(e => e.From == u || e.To == u)
                                          .Select(e => e.From == u ? e.To : e.From).Distinct();

                foreach (var v in neighbors)
                {
                    double weight = GetSocialDistance(u, v);
                    double alt = distances[u] + weight;

                    // Daha kısa bir yol bulundu
                    if (alt < distances[v] - 0.001)
                    {
                        distances[v] = alt;
                        parents[v] = new List<Node> { u };
                    }
                    // Eşit maliyetli başka bir yol bulundu
                    else if (Math.Abs(alt - distances[v]) < 0.001)
                    {
                        parents[v].Add(u);
                    }
                }
            }
            return ReconstructAllPaths(end, parents);
        }

        private List<List<Node>> ReconstructAllPaths(Node current, Dictionary<Node, List<Node>> parents)
        {
            if (!parents.ContainsKey(current) || !parents[current].Any())
                return new List<List<Node>> { new List<Node> { current } };

            var allPaths = new List<List<Node>>();
            foreach (var parent in parents[current])
            {
                var pathsFromParent = ReconstructAllPaths(parent, parents);
                foreach (var path in pathsFromParent)
                {
                    path.Add(current);
                    allPaths.Add(path);
                }
            }
            return allPaths;
        }
    }
}
