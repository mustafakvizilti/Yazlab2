using System.Drawing;

namespace Yazlab2WinForms.Models
{
    public class Node
    {
        public string Name { get; set; }
        public Point Position { get; set; }

        public double Aktiflik { get; set; }    // Özellik I
        public double IliskiGucu { get; set; }  // Özellik II (Etkileşim görevi görecek)
        public int BaglantiSayisi { get; set; } // Özellik III

        public Node(string name, Point position, double aktiflik = 0.5)
        {
            Name = name;
            Position = position;
            Aktiflik = aktiflik;
            IliskiGucu = 0; // Başlangıçta 0, hesaplamayla dolacak
            BaglantiSayisi = 0;
        }
    }
}