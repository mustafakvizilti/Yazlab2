using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Yazlab2WinForms.Business.Concrete; // Manager
using Yazlab2WinForms.Models;           // Graph, Node
using System.Web.Script.Serialization; // JSON için şart 
using System.IO;

namespace Yazlab2WinForms
{
    public partial class Form1 : Form
    {

        GraphManager _manager = new GraphManager(); 
        Graph _currentGraph;
        private Node draggingNode = null; 
        private Point dragOffset;         
        private Graph graph = new Graph();
        private List<List<Node>> pathToHighlight = null;
        private Dictionary<string, Color> nodeColors = new Dictionary<string, Color>();

        public Form1()
        {
            InitializeComponent();
        }
            
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog { Filter = "JSON Dosyası|*.json" };
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                
                Yazlab2WinForms.DataAccess.GraphData dataAccess = new Yazlab2WinForms.DataAccess.GraphData();
                dataAccess.SaveToJson(this.graph, saveFile.FileName);
                MessageBox.Show("Graf, Aktiflikler ve Matris kaydedildi!");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog { Filter = "JSON Dosyası|*.json" };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Yazlab2WinForms.DataAccess.GraphData dataAccess = new Yazlab2WinForms.DataAccess.GraphData();
                Graph loadedGraph = dataAccess.LoadFromJson(openFile.FileName);

                if (loadedGraph != null)
                {
                    this.graph = loadedGraph;
                    nodeColors.Clear();
                    pictureBox1.Invalidate();
                    MessageBox.Show("Graf ve Matris başarıyla yüklendi!");
                }
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var node in graph.Nodes)
            {
                // Düğümler 50x50 boyutunda çiziliyor
                Rectangle nodeRect = new Rectangle(node.Position.X, node.Position.Y, 50, 50);

                if (nodeRect.Contains(e.Location))
                {
                    draggingNode = node;
                    dragOffset = new Point(e.X - node.Position.X, e.Y - node.Position.Y);
                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingNode != null)
            {
                draggingNode.Position = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            draggingNode = null;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            cmbRelType.DataSource = Enum.GetValues(typeof(RelationType));
        }

        // Düğüm ekleme
        private void btnAddNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();

            double girilenAktiflik = (double)numAktiflik.Value;

            if (string.IsNullOrEmpty(nodeName))
            {
                int nextNumber = 1;
                while (graph.Nodes.Any(node => node.Name == nextNumber.ToString()))
                    nextNumber++;
                nodeName = nextNumber.ToString();
            }

            // 2. Düğümü seçilen aktiflik değeriyle oluşturuyoruz
            Node newNode = new Node(nodeName, new Point(0, 0), girilenAktiflik);
            graph.AddNode(newNode);

            UpdateNodePositions();
            lblInfo.Text = $"Düğüm eklendi: {nodeName} (Aktiflik: {girilenAktiflik})";
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
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point mouseLocation = e.Location; 
            Node clickedNode = null;

            foreach (var node in graph.Nodes)
            {
                Rectangle nodeRect = new Rectangle(node.Position.X, node.Position.Y, 50, 50);
                if (nodeRect.Contains(mouseLocation))
                {
                    clickedNode = node;
                    break;
                }
            }

            if (clickedNode != null)
            {
                int connectionCount = graph.Edges.Count(edge => edge.From == clickedNode || edge.To == clickedNode);

                string infoText = $"--- KULLANICI DETAYLARI ---\n" +
                                  $"Adı: {clickedNode.Name}\n" +
                                  $"Aktiflik: {clickedNode.Aktiflik}\n" +
                                  $"Bağlantı Sayısı: {connectionCount}\n" +
                                  $"İlişki Gücü Toplamı: {clickedNode.IliskiGucu}";

                MessageBox.Show(infoText, "Düğüm Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // -----------------------------

                txtNodeName.Text = clickedNode.Name;
                numAktiflik.Value = (decimal)clickedNode.Aktiflik; //

                lblInfo.Text = $"Seçili: {clickedNode.Name}. Değerleri değiştirip Güncelle'ye basabilirsiniz.";
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

                    RelationType secilenTip = (RelationType)cmbRelType.SelectedItem;
                    int agirlik = (int)numWeight.Value;

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

        // Dijkstra
        private void btnShortestPath_Click(object sender, EventArgs e)
        {
            graph.UpdateNodeStatistics();
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            Node end = graph.Nodes.FirstOrDefault(n => n.Name == txtTarget.Text.Trim());

            if (start == null || end == null)
            {
                lblInfo.Text = "Geçerli düğüm isimleri girin!";
                return;
            }

            // --- SÜRE ÖLÇÜMÜ ---
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Algorithm alg = new Algorithm();
            var allPaths = alg.FindShortestPaths(graph, start, end); 

            watch.Stop(); //kronometreyi durdur
            double elapsedMs = watch.Elapsed.TotalMilliseconds;

            if (allPaths != null && allPaths.Count > 0 && allPaths[0].Count > 1)
            {
                pathToHighlight = allPaths;

                // birden çok yol varsa hepsini tabloya basar
                foreach (var tekYol in allPaths)
                {
                    AddResultToTable("Dijkstra", tekYol, elapsedMs); // Süreyi buraya gönderiyoruz
                }

                lblInfo.Text = $"Dijkstra Tamamlandı ({elapsedMs:F4} ms)";
                pictureBox1.Invalidate();
            }
            else
            {
                lblInfo.Text = "Yol bulunamadı!";
                pathToHighlight = null;
            }
        }
        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();
            var nodeToDelete = graph.Nodes.FirstOrDefault(n => n.Name == nodeName);

            if (nodeToDelete != null)
            {
                
                graph.Nodes.Remove(nodeToDelete);

                graph.Edges.RemoveAll(edge => edge.From == nodeToDelete || edge.To == nodeToDelete);

                UpdateNodePositions();
                pictureBox1.Invalidate();
                lblInfo.Text = $"Düğüm ve ilgili bağlantılar silindi: {nodeName}";
            }
            else
            {
                lblInfo.Text = "Silinecek düğüm bulunamadı!";
            }
        }
        private void btnDeleteEdge_Click(object sender, EventArgs e)
        {
            string sourceName = txtSource.Text.Trim();
            string targetName = txtTarget.Text.Trim();

            var sourceNode = graph.Nodes.FirstOrDefault(n => n.Name == sourceName);
            var targetNode = graph.Nodes.FirstOrDefault(n => n.Name == targetName);

            if (sourceNode == null || targetNode == null)
            {
                lblInfo.Text = "Kaynak veya hedef düğüm bulunamadı!";
                return;
            }

            // İki düğüm arasındaki tüm bağlantıları sil (yön fark etmeksizin)
            int removedCount = graph.Edges.RemoveAll(edge =>
                (edge.From == sourceNode && edge.To == targetNode) ||
                (edge.From == targetNode && edge.To == sourceNode));

            if (removedCount > 0)
            {
                lblInfo.Text = $"{removedCount} adet bağlantı silindi.";
                pictureBox1.Invalidate();
            }
            else
            {
                lblInfo.Text = "Belirtilen düğümler arasında bağlantı yok.";
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Dictionary<string, int> edgeCountMap = new Dictionary<string, int>();

            foreach (var edge in graph.Edges)
            {
                Color cizgiRengi;
                switch (edge.Type.ToString().ToLower())
                {
                    case "akraba": cizgiRengi = Color.Green; break;
                    case "arkadas": cizgiRengi = Color.Red; break;
                    case "is": cizgiRengi = Color.Blue; break;
                    case "tanidik": cizgiRengi = Color.Orange; break;
                    default: cizgiRengi = Color.Gray; break;
                }

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
                                thickness = 6;
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

            foreach (var node in graph.Nodes)
            {
                Brush nodeBrush = nodeColors.ContainsKey(node.Name) ?
                                  new SolidBrush(nodeColors[node.Name]) : Brushes.White;

                g.FillEllipse(nodeBrush, node.Position.X, node.Position.Y, 50, 50);
                g.DrawEllipse(Pens.Black, node.Position.X, node.Position.Y, 50, 50);

                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                g.DrawString(node.Name, this.Font, Brushes.Black, new RectangleF(node.Position.X, node.Position.Y, 50, 50), sf);
            }
        }

        private void button4_Click(object sender, EventArgs e)  // En popüler kişiyi bul
        {
            if (graph.Nodes.Count == 0)
            {
                MessageBox.Show("Önce ekrana düğüm ekle!");
                return;
            }

            List<Node> sampiyonlar = _manager.EnPopulerleriBul(graph);

            if (sampiyonlar.Count > 0)
            {
                string isimler = string.Join(", ", sampiyonlar.Select(n => n.Name));

                int baglantiSayisi = graph.Edges.Count(edge => edge.From == sampiyonlar[0] || edge.To == sampiyonlar[0]);

                MessageBox.Show($"En Güçlü Düğümler ({baglantiSayisi} Bağlantı):\n{isimler}", "Analiz Sonucu");
            }
            else
            {
                MessageBox.Show("Henüz hiç bağlantı (çizgi) yok.");
            }
        }


        private void btnBFS_Click(object sender, EventArgs e)
        {
            // Kaynak kutusundaki ismi bul
            Node startNode = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());

            if (startNode == null)
            {
                MessageBox.Show("Lütfen 'Kaynak' kutusuna geçerli bir düğüm adı girin!");
                return;
            }

            // Algorithm sınıfını çağır 
            Algorithm alg = new Algorithm();
            List<Node> reachable = alg.GetReachableNodesBFS(graph, startNode);

            string result = string.Join(", ", reachable.Select(n => n.Name));
            MessageBox.Show($"BFS ile Erişilebilen Kullanıcılar:\n{result}", "BFS Analizi");
        }

        private void btnDFS_Click(object sender, EventArgs e)
        {
            Node startNode = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());

            if (startNode == null)
            {
                MessageBox.Show("Lütfen 'Kaynak' kutusuna geçerli bir düğüm adı girin!");
                return;
            }

            Algorithm alg = new Algorithm();
            List<Node> reachable = new List<Node>();

            alg.GetReachableNodesDFS(graph, startNode, reachable);

            string result = string.Join(", ", reachable.Select(n => n.Name));
            MessageBox.Show($"DFS ile Erişilebilen Kullanıcılar:\n{result}", "DFS Analizi");
        }

        private void button5_Click(object sender, EventArgs e) // Top5Node Butonu
        {
            if (graph.Nodes.Count == 0) return;

            var top5 = _manager.GetTop5Influencers(graph);

            string tableHeader = string.Format("{0,-15} | {1,-10}\n", "Kullanıcı", "Bağlantı");
            string separator = new string('-', 30) + "\n";
            string tableRows = "";

            foreach (var item in top5)
            {
                tableRows += string.Format("{0,-15} | {1,-10}\n", item.NodeName, item.Degree);
            }

            MessageBox.Show(tableHeader + separator + tableRows, "En Yüksek Dereceli 5 Düğüm");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnColoring_Click(object sender, EventArgs e)
        {
            Coloring coloring = new Coloring();
            nodeColors = coloring.ApplyWelshPowell(graph);
            pictureBox1.Invalidate();
        }

        private void buttonUpdateNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();

            var targetNode = graph.Nodes.FirstOrDefault(n => n.Name == nodeName);

            if (targetNode != null)
            {
                double yeniAktiflik = (double)numAktiflik.Value;
                targetNode.Aktiflik = yeniAktiflik; //

                lblInfo.Text = $"{targetNode.Name} düğümü güncellendi. Yeni Aktiflik: {yeniAktiflik}";
                pictureBox1.Invalidate(); 

                MessageBox.Show($"{targetNode.Name} başarıyla güncellendi!", "Güncelleme Başarılı");
            }
            else
            {
                MessageBox.Show("Listede bu isimde bir düğüm bulunamadı. Lütfen önce graf üzerinden bir düğüm seçin.");
            }
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchNode_Click(object sender, EventArgs e)
        {
            string searchName = txtNodeName.Text.Trim();

            if (string.IsNullOrEmpty(searchName))
            {
                MessageBox.Show("Lütfen aranacak bir düğüm adı girin.");
                return;
            }

            nodeColors.Clear();

            // Arama işlemi
            var foundNode = graph.Nodes.FirstOrDefault(n => n.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));

            if (foundNode != null)
            {
                nodeColors[foundNode.Name] = Color.Yellow;

                lblInfo.Text = $"Düğüm bulundu: {foundNode.Name} (Aktiflik: {foundNode.Aktiflik})";

                // Görseli güncelle
                pictureBox1.Invalidate();

                MessageBox.Show($"'{foundNode.Name}' isimli düğüm bulundu ve vurgulandı.", "Arama Başarılı");
            }
            else
            {
                lblInfo.Text = "Düğüm bulunamadı.";
                pictureBox1.Invalidate();
                MessageBox.Show("Aranan isimde bir düğüm mevcut değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void AddResultToTable(string algorithmName, List<Node> path, double elapsedMs)
        {
            if (path == null || path.Count <= 1) return;

            string pathText = string.Join(" -> ", path.Select(n => n.Name));
            int nodeCount = path.Count;

            double totalCost = 0;
            Algorithm alg = new Algorithm();
            for (int i = 0; i < path.Count - 1; i++)
                totalCost += alg.GetDynamicWeight(path[i], path[i + 1]);

            dgvResults.Rows.Add(
                algorithmName,
                nodeCount,
                pathText,
                elapsedMs.ToString("F4"),
                totalCost.ToString("F4")
            );
        }

        private void buttonAStar_Click(object sender, EventArgs e)
        {
            graph.UpdateNodeStatistics();
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            Node end = graph.Nodes.FirstOrDefault(n => n.Name == txtTarget.Text.Trim());

            if (start == null || end == null) return;

            // Süre ölçümü başlatılıyor
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Algorithm alg = new Algorithm();
            //A* metodunu çağırıyoruz
            var path = alg.FindShortestPathAStar(graph, start, end);

            watch.Stop();
            double elapsedMs = watch.Elapsed.TotalMilliseconds;

            if (path != null && path.Count > 1)
            {
                pathToHighlight = new List<List<Node>> { path };

                AddResultToTable("A-Star (A*)", path, elapsedMs);

                pictureBox1.Invalidate();
            }
            else
            {
                lblInfo.Text = "A* ile yol bulunamadı!";
            }
        }
    }
}
