using System.Collections.Generic; // List için þart
using System.Linq; // Count, Where için þart
using Yazlab2WinForms.DataAccess;
using Yazlab2WinForms.Models;

namespace Yazlab2WinForms.Business.Concrete
{
    public class GraphManager
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

        // --- GÜNCELLENEN KISIM ---
        // Artýk tek bir 'Node' deðil, 'List<Node>' (Node Listesi) döndürüyor
        public List<Node> EnPopulerleriBul(Graph graph)
        {
            if (graph == null || graph.Nodes.Count == 0)
                return new List<Node>();

            // 1. Önce herkesin kaç baðlantýsý var hesaplayalým
            var skorlar = new Dictionary<Node, int>();
            int maxBaglanti = 0;

            foreach (var node in graph.Nodes)
            {
                // Bu düðüm kaç kere geçiyor?
                int sayi = graph.Edges.Count(e => e.From == node || e.To == node);
                skorlar[node] = sayi;

                // Rekor kýrýldýysa maxBaglanti'yi güncelle
                if (sayi > maxBaglanti) maxBaglanti = sayi;
            }

            // 2. Skoru, Max skor ile ayný olan HERKESÝ bul
            // (Eðer hiç baðlantý yoksa hepsini döndürmesin diye max > 0 kontrolü yapýlabilir)
            if (maxBaglanti == 0) return new List<Node>(); // Kimsenin baðlantýsý yoksa boþ dön

            List<Node> kazananlar = graph.Nodes.Where(n => skorlar[n] == maxBaglanti).ToList();

            return kazananlar;
        }
    }
}