using System;
using System.Collections.Generic;
using System.Linq;

namespace Yazlab2WinForms
{
    public class Graph
    {
        // Düğümler ve kenarlar
        public List<Node> Nodes { get; } = new List<Node>();
        public List<Edge> Edges { get; } = new List<Edge>();

        // Düğüm ekleme
        public void AddNode(Node node)
        {
            if (!Nodes.Any(n => n.Name == node.Name))
                Nodes.Add(node);
        }

        // Kenar ekleme
        public void AddEdge(Node from, Node to)
        {
            if (Nodes.Contains(from) && Nodes.Contains(to))
                Edges.Add(new Edge(from, to));
        }

        // Düğümler arası Euclidean mesafe
        private double Distance(Node a, Node b)
        {
            int dx = a.Position.X - b.Position.X;
            int dy = a.Position.Y - b.Position.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Dijkstra ile en kısa yol (yönsüz)
        public List<Node> FindShortestPathDijkstra(Node start, Node end)
        {
            var dist = new Dictionary<Node, double>();
            var prev = new Dictionary<Node, Node>();
            var q = new List<Node>();

            foreach (var node in Nodes)
            {
                dist[node] = double.MaxValue;
                prev[node] = null;
                q.Add(node);
            }
            dist[start] = 0;

            while (q.Count > 0)
            {
                Node u = q.OrderBy(n => dist[n]).First();
                q.Remove(u);

                if (u == end) break;

                // Yönsüz kenar kontrolü
                foreach (var edge in Edges.Where(ed => ed.From == u || ed.To == u))
                {
                    Node v = edge.From == u ? edge.To : edge.From;
                    double alt = dist[u] + Distance(u, v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            var path = new List<Node>();
            Node curr = end;
            while (curr != null)
            {
                path.Add(curr);
                curr = prev[curr];
            }
            path.Reverse();

            if (path.Count == 1 && path[0] != start) return null;
            return path;
        }
    }
}
