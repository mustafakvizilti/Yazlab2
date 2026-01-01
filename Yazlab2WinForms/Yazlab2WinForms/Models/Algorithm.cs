using System;
using System.Collections.Generic;
using System.Linq;
using Yazlab2WinForms.Abstract;

namespace Yazlab2WinForms.Models
{
    public class Algorithm : IAlgorithmService
    {
        
        public double GetDynamicWeight(Node i, Node j)
        {
            double dA = Math.Pow(i.Aktiflik - j.Aktiflik, 2);
            double dI = Math.Pow(i.IliskiGucu - j.IliskiGucu, 2);
            double dB = Math.Pow(i.BaglantiSayisi - j.BaglantiSayisi, 2);

            
            return 1 + Math.Sqrt(dA + dI + dB);
        }

        public List<List<Node>> FindShortestPaths(Graph graph, Node start, Node end)
        {
            var distances = new Dictionary<Node, double>();
            var parents = new Dictionary<Node, List<Node>>();

            foreach (var n in graph.Nodes) { distances[n] = double.MaxValue; parents[n] = new List<Node>(); }
            distances[start] = 0;
            var queue = new List<Node>(graph.Nodes);

            while (queue.Any())
            {
                var u = queue.OrderBy(n => distances[n]).First();
                queue.Remove(u);
                if (u == end) break;

                var neighbors = graph.Edges.Where(e => e.From == u || e.To == u)
                                           .Select(e => e.From == u ? e.To : e.From).Distinct();

                foreach (var v in neighbors)
                {
                    
                    double weight = GetDynamicWeight(u, v);
                    double alt = distances[u] + weight;

                    if (alt < distances[v] - 0.001)
                    {
                        distances[v] = alt;
                        parents[v] = new List<Node> { u };
                    }
                    else if (Math.Abs(alt - distances[v]) < 0.001)
                    {
                        parents[v].Add(u);
                    }
                }
            }
            return Reconstruct(end, parents);
        }

        private List<List<Node>> Reconstruct(Node n, Dictionary<Node, List<Node>> p)
        {
            if (!p.ContainsKey(n) || !p[n].Any()) return new List<List<Node>> { new List<Node> { n } };
            var paths = new List<List<Node>>();
            foreach (var parent in p[n])
            {
                foreach (var path in Reconstruct(parent, p)) { path.Add(n); paths.Add(path); }
            }
            return paths;
        }

        // BFS
        public List<Node> GetReachableNodesBFS(Graph graph, Node startNode)
        {
            List<Node> visited = new List<Node>();
            Queue<Node> queue = new Queue<Node>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();
                var neighbors = graph.Edges
                    .Where(e => e.From == current || e.To == current)
                    .Select(e => e.From == current ? e.To : e.From);

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return visited;
        }

        // DFS
        public void GetReachableNodesDFS(Graph graph, Node current, List<Node> visited)
        {
            visited.Add(current);
            var neighbors = graph.Edges
                .Where(e => e.From == current || e.To == current)
                .Select(e => e.From == current ? e.To : e.From);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    GetReachableNodesDFS(graph, neighbor, visited);
                }
            }
        }

        // A* (A-Star) Algoritması
        public List<Node> FindShortestPathAStar(Graph graph, Node start, Node end)
        {
            var openSet = new List<Node> { start };
            var cameFrom = new Dictionary<Node, Node>();

            var gScore = new Dictionary<Node, double>(); 
            var fScore = new Dictionary<Node, double>();

            foreach (var n in graph.Nodes)
            {
                gScore[n] = double.MaxValue;
                fScore[n] = double.MaxValue;
            }

            gScore[start] = 0;
            fScore[start] = GetHeuristic(start, end);

            while (openSet.Any())
            {
                
                var current = openSet.OrderBy(n => fScore[n]).First();

                if (current == end) return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                var neighbors = graph.Edges
                    .Where(e => e.From == current || e.To == current)
                    .Select(e => e.From == current ? e.To : e.From).Distinct();

                foreach (var neighbor in neighbors)
                {
                    
                    double tentativeGScore = gScore[current] + GetDynamicWeight(current, neighbor);

                    if (tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor, end);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
            return new List<Node>(); 
        }

        // Heuristic (Sezgisel) Fonksiyon
        private double GetHeuristic(Node n1, Node n2)
        {
            return Math.Sqrt(Math.Pow(n1.Position.X - n2.Position.X, 2) +
                             Math.Pow(n1.Position.Y - n2.Position.Y, 2)) / 100.0;
        }

        private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var path = new List<Node> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }
            return path;
        }
    }
}
