using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yazlab2WinForms
{
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }

        public Edge(Node from, Node to)
        {
            From = from;
            To = to;
        }
    }
}