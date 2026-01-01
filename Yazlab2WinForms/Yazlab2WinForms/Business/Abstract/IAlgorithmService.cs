using System.Collections.Generic;
using Yazlab2WinForms.Models;

namespace Yazlab2WinForms.Abstract
{
    public interface IAlgorithmService
    {
        // Dinamik ağırlık hesaplama
        double GetDynamicWeight(Node i, Node j);

        // En kısa yol algoritmaları
        List<List<Node>> FindShortestPaths(Graph graph, Node start, Node end); // Dijkstra
        List<Node> FindShortestPathAStar(Graph graph, Node start, Node end);   // A*

        // Arama algoritmaları
        List<Node> GetReachableNodesBFS(Graph graph, Node startNode);
        void GetReachableNodesDFS(Graph graph, Node current, List<Node> visited);
    }
}