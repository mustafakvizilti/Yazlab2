using System.Drawing; // Point için þart
using System.Collections.Generic;
using Yazlab2WinForms.Models; // Modeller

namespace Yazlab2WinForms.DataAccess
{
    public class GraphData
    {
        public Graph GetGraph()
        {
            Graph graph = new Graph();

            // (Ýsim, Point)
            Node a = new Node("Ahmet", new Point(100, 100));
            Node b = new Node("Mehmet", new Point(300, 150));
            Node c = new Node("Celil", new Point(200, 300));

            graph.AddNode(a);
            graph.AddNode(b);
            graph.AddNode(c);

            // kenar
            graph.AddEdge(a, b, RelationType.Arkadas, 8);
            graph.AddEdge(b, c, RelationType.Is, 5);
            graph.AddEdge(c, a, RelationType.Akraba, 10);

            return graph;
        }
    }
}