using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

public class Node
{
    public string Name { get; set; }
    public Point Position { get; set; } // Düğümün ekranda nerede görüneceği

    public Node(string name, Point position)
    {
        Name = name;
        Position = position;
    }
}
