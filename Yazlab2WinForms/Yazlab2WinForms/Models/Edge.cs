using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yazlab2WinForms.Models   
{
    public enum RelationType { Akraba, Arkadas, Is, Tanidik } // İlişki türleri (sonradan eklendi)
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }

        public RelationType Type { get; set; }
        public int Weight { get; set; } // Şiddet 1-10

        public Edge(Node from, Node to, RelationType type, int weight)
        {
            From = from;
            To = to;
            // İlişki türü ve şiddet ataması
            Type = type;
            Weight = weight;
        }
    }
}