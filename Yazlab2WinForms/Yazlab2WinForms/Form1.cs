using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Yazlab2WinForms.Business.Concrete; // Manager
using Yazlab2WinForms.Models;           // Graph, Node

namespace Yazlab2WinForms
{
    public partial class Form1 : Form
    {
        GraphManager _manager = new GraphManager(); // GraphManager nesnesi
        Graph _currentGraph;

        private Graph graph = new Graph();
        private List<List<Node>> pathToHighlight = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbRelType.DataSource = Enum.GetValues(typeof(RelationType));
        }

        // Düğüm ekleme
        private void btnAddNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();

            // Eğer isim boşsa otomatik numara ver
            if (string.IsNullOrEmpty(nodeName))
            {
                int nextNumber = 1;
                while (graph.Nodes.Any(node => node.Name == nextNumber.ToString()))
                    nextNumber++;
                nodeName = nextNumber.ToString();
            }

            Node newNode = new Node(nodeName, new Point(0, 0));
            graph.AddNode(newNode);

            // Tüm düğümlerin pozisyonunu güncelle (daire)
            UpdateNodePositions();

            lblInfo.Text = $"Düğüm eklendi: {nodeName}";
            txtNodeName.Clear();
            pictureBox1.Invalidate();
        }

        private void UpdateNodePositions()
        {
            int count = graph.Nodes.Count;
            if (count == 0) return;

            int radius = Math.Min(pictureBox1.Width, pictureBox1.Height) / 2 - 50;
            Point center = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);

            for (int i = 0; i < count; i++)
            {
                double angle = 2 * Math.PI * i / count;
                int x = center.X + (int)(radius * Math.Cos(angle)) - 25;
                int y = center.Y + (int)(radius * Math.Sin(angle)) - 25;
                graph.Nodes[i].Position = new Point(x, y);
            }
        }

        // Kenar ekleme
        private void btnAddEdge_Click(object sender, EventArgs e)
        {
            if (graph.Nodes.Count == 0)
            {
                lblInfo.Text = "Önce düğüm ekleyin!";
                return;
            }

            string sourceName = txtSource.Text.Trim();
            string targetName = txtTarget.Text.Trim();

            List<Node> fromNodes;
            List<Node> toNodes;

            // Kaynak boşsa tüm düğümler
            if (string.IsNullOrEmpty(sourceName))
                fromNodes = new List<Node>(graph.Nodes);
            else
            {
                var n = graph.Nodes.FirstOrDefault(node => node.Name == sourceName);
                if (n == null)
                {
                    lblInfo.Text = "Kaynak düğüm bulunamadı!";
                    return;
                }
                fromNodes = new List<Node> { n };
            }

            // Hedef boşsa tüm düğümler
            if (string.IsNullOrEmpty(targetName))
                toNodes = new List<Node>(graph.Nodes);
            else
            {
                var n = graph.Nodes.FirstOrDefault(node => node.Name == targetName);
                if (n == null)
                {
                    lblInfo.Text = "Hedef düğüm bulunamadı!";
                    return;
                }
                toNodes = new List<Node> { n };
            }

            int addedEdges = 0;


            foreach (var from in fromNodes)
            {
                foreach (var to in toNodes)
                {
                    if (from == to) continue; // Kendine bağlama yapmasın

                    // 1. Kutuda seçili olan türü ve ağırlığı al
                    RelationType secilenTip = (RelationType)cmbRelType.SelectedItem;
                    int agirlik = (int)numWeight.Value;

                    // 2. KONTROL: Grafın içinde bu iki düğüm arasında AYNI TÜRDE bir bağ zaten var mı?
                    // Sadece bu tıklamaya değil, tüm geçmişe (graph.Edges) bakar.
                    bool zatenVar = graph.Edges.Any(edge =>
                        ((edge.From == from && edge.To == to) || (edge.From == to && edge.To == from))
                        && edge.Type == secilenTip);

                    if (!zatenVar)
                    {
                        // 3. Eğer aynı türde bağ yoksa ekle
                        graph.AddEdge(from, to, secilenTip, agirlik);
                        addedEdges++;
                    }
                }
            }

            lblInfo.Text = $"{addedEdges} kenar eklendi.";
            txtSource.Clear();
            txtTarget.Clear();
            pictureBox1.Invalidate();
        }

        // En kısa yol
        private void btnShortestPath_Click(object sender, EventArgs e)
        {
           
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            Node end = graph.Nodes.FirstOrDefault(n => n.Name == txtTarget.Text.Trim());

            if (start == null || end == null)
            {
                lblInfo.Text = "Geçerli düğüm isimleri girin!";
                return;
            }

            // Algoritma çalıştırılıyor
            Algorithm alg = new Algorithm();
            var allPaths = alg.FindShortestPaths(graph, start, end); // Algoritmayı bu nesne üzerinden çalıştırıyoruz

            if (allPaths.Count == 0 || (allPaths.Count == 1 && allPaths[0].Count == 1 && allPaths[0][0] != start))
            {
                lblInfo.Text = "Yol bulunamadı!";
                pathToHighlight = null;
            }
            else
            {
                // Bütün yolları metin olarak birleştirelim
                List<string> pathStrings = new List<string>();

                for (int i = 0; i < allPaths.Count; i++)
                {
                    // Örn: "Yol 1: Ahmet → Ali → Veli"
                    string pathText = $"Yol {i + 1}: " + string.Join(" → ", allPaths[i].Select(n => n.Name));
                    pathStrings.Add(pathText);
                }

                // Bütün yolları alt alta birleştir
                string finalReport = string.Join("\n", pathStrings);

                if (allPaths.Count > 1)
                {
                    lblInfo.Text = $"Eşit maliyette {allPaths.Count} yol bulundu. Detaylar mesaj kutusunda.";
                    // Büyük bir pencerede bütün seçenekleri göster
                    MessageBox.Show($"Bulunan En Kısa Sosyal Yollar:\n\n{finalReport}", "Alternatif Yollar");
                }
                else
                {
                    lblInfo.Text = "En iyi yol: " + string.Join(" → ", allPaths[0].Select(n => n.Name));
                }

                // Görsel hepsini parlatıyoruz
                if (allPaths.Count > 0)
                {
                    // Sadece ilkini değil, hepsini gönderiyoruz
                    pathToHighlight = allPaths;
                }
                else
                {
                    pathToHighlight = null;
                }
                pictureBox1.Invalidate();
            }

            pictureBox1.Invalidate();
        }


        // Çizim
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Dictionary<string, int> edgeCountMap = new Dictionary<string, int>();

            foreach (var edge in graph.Edges)
            {
                // 1. ÖNCE RENGİ BELİRLE (Normal kendi rengi)
                Color cizgiRengi;
                switch (edge.Type.ToString().ToLower())
                {
                    case "akraba": cizgiRengi = Color.Green; break;
                    case "arkadas": cizgiRengi = Color.Red; break;
                    case "is": cizgiRengi = Color.Blue; break;
                    case "tanidik": cizgiRengi = Color.Orange; break;
                    default: cizgiRengi = Color.Gray; break;
                }

                // 2. YOL KONTROLÜ VE KALINLIK BELİRLEME
                bool isPathEdge = false;
                int thickness = 2; // Standart kalınlık

                if (pathToHighlight != null)
                {
                    foreach (var tekYol in pathToHighlight)
                    {
                        for (int i = 0; i < tekYol.Count - 1; i++)
                        {
                            if ((edge.From == tekYol[i] && edge.To == tekYol[i + 1]) ||
                                (edge.To == tekYol[i] && edge.From == tekYol[i + 1]))
                            {
                                isPathEdge = true;
                                thickness = 6; // Yol bulunduysa çizgiyi büyüt
                                               // İstersen rengi de biraz daha parlak yapabilirsin ama şu an kendi renginde kalıyor
                                               // cizgiRengi = ControlPaint.Light(cizgiRengi); // Bu satır açılırsa rengi parlatır
                                break;
                            }
                        }
                        if (isPathEdge) break;
                    }
                }

                // 3. PARALEL ÇİZGİ HESABI 
                Point p1 = new Point(edge.From.Position.X + 25, edge.From.Position.Y + 25);
                Point p2 = new Point(edge.To.Position.X + 25, edge.To.Position.Y + 25);

                int h1 = edge.From.GetHashCode();
                int h2 = edge.To.GetHashCode();
                string pairKey = h1 < h2 ? $"{h1}-{h2}" : $"{h2}-{h1}";

                if (!edgeCountMap.ContainsKey(pairKey)) edgeCountMap[pairKey] = 0;
                int index = edgeCountMap[pairKey]++;

                float dx = p2.X - p1.X;
                float dy = p2.Y - p1.Y;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);

                float offset = (index == 0) ? 0 : (index % 2 == 0 ? (index / 2) * 12 : -(index / 2 + 1) * 12);
                float offsetX = length > 0 ? -dy / length * offset : 0;
                float offsetY = length > 0 ? dx / length * offset : 0;

                // 4. ÇİZİM
                using (Pen pen = new Pen(cizgiRengi, thickness))
                {
                    g.DrawLine(pen, p1.X + offsetX, p1.Y + offsetY, p2.X + offsetX, p2.Y + offsetY);
                }
            }

            // Düğümleri çiz (Bu kısım da aynı)
            foreach (var node in graph.Nodes)
            {
                g.FillEllipse(Brushes.White, node.Position.X, node.Position.Y, 50, 50);
                g.DrawEllipse(Pens.Black, node.Position.X, node.Position.Y, 50, 50);
                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                g.DrawString(node.Name, this.Font, Brushes.Black, new RectangleF(node.Position.X, node.Position.Y, 50, 50), sf);
            }
        }

        private void button4_Click(object sender, EventArgs e)  // En popüler kişiyi bul
        {
            // Boş mu kontrolü
            if (graph.Nodes.Count == 0)
            {
                MessageBox.Show("Önce ekrana düğüm ekle!");
                return;
            }

            // 1. Manager bize bir LİSTE veriyor
            List<Node> sampiyonlar = _manager.EnPopulerleriBul(graph);

            if (sampiyonlar.Count > 0)
            {
                // 2. Listedeki isimleri aralarına virgül koyarak birleştiriyoruz
                string isimler = string.Join(", ", sampiyonlar.Select(n => n.Name));

                // Bağlantı sayısını da gösterelim
                int baglantiSayisi = graph.Edges.Count(edge => edge.From == sampiyonlar[0] || edge.To == sampiyonlar[0]);

                MessageBox.Show($"En Güçlü Düğümler ({baglantiSayisi} Bağlantı):\n{isimler}", "Analiz Sonucu");
            }
            else
            {
                MessageBox.Show("Henüz hiç bağlantı (çizgi) yok.");
            }
        }
    }
}
