using System.Collections.Generic; // List için þart
using System.Linq; // Count, Where için þart
using Yazlab2WinForms.DataAccess;
using Yazlab2WinForms.Models;
using Yazlab2WinForms.Abstract;

namespace Yazlab2WinForms.Business.Concrete
{
    public class GraphManager : IGraphService
    {
        private GraphData _graphData;

        public GraphManager()
        {
            _graphData = new GraphData();
        }

        public Graph GetGraphVerisi()
        {
            return _graphData.GetGraph();
        }

        
        public List<Node> EnPopulerleriBul(Graph graph)
        {
            if (graph == null || graph.Nodes.Count == 0)
                return new List<Node>();

            
            var skorlar = new Dictionary<Node, int>();
            int maxBaglanti = 0;

            foreach (var node in graph.Nodes)
            {
                
                int sayi = graph.Edges.Count(e => e.From == node || e.To == node);
                skorlar[node] = sayi;

               
                if (sayi > maxBaglanti) maxBaglanti = sayi;
            }

           

            List<Node> kazananlar = graph.Nodes.Where(n => skorlar[n] == maxBaglanti).ToList();

            return kazananlar;
        }

        public List<dynamic> GetTop5Influencers(Graph graph)
        {
            return graph.Nodes.Select(node => new
            {
                NodeName = node.Name,
                Degree = graph.Edges.Count(e => e.From == node || e.To == node)
            })
            .OrderByDescending(x => x.Degree)
            .Take(5).Cast<dynamic>().ToList();
        }
    }
}