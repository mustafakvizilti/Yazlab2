using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Yazlab2WinForms.Business.Concrete; // Manager
using Yazlab2WinForms.Models;           // Graph, Node
using System.Web.Script.Serialization; // JSON için şart 
using Yazlab2WinForms.Business.Concrete;
using Yazlab2WinForms.Models;
using System.IO;

namespace Yazlab2WinForms
{
    public partial class Form1 : Form
    {

        GraphManager _manager = new GraphManager(); // GraphManager nesnesi
        Graph _currentGraph;

        private Graph graph = new Graph();
        private List<List<Node>> pathToHighlight = null;
        private Dictionary<string, Color> nodeColors = new Dictionary<string, Color>();

        public Form1()
        {
            InitializeComponent();
        }
            
        private void ShowTop5Influencers()
        {
            if (graph.Nodes.Count == 0)
            {
                MessageBox.Show("Sistemde henüz düğüm bulunmuyor.");
                return;
            }

            // 1. Derece Merkeziliği Hesaplama (LINQ kullanarak)
            var degreeCentralityList = graph.Nodes.Select(node => new
            {
                NodeName = node.Name,
                // Düğümün hem kaynak hem de hedef olduğu tüm kenarları sayıyoruz
                Degree = graph.Edges.Count(e => e.From == node || e.To == node)
            })
            .OrderByDescending(x => x.Degree) // Dereceye göre azalan sıralama
            .Take(5) // En yüksek 5 taneyi al
            .ToList();

            // 2. Tablo Görünümü Oluşturma
            string tableHeader = string.Format("{0,-15} | {1,-10}\n", "Kullanıcı Adı", "Derece");
            string separator = new string('-', 30) + "\n";
            string tableRows = "";

            foreach (var item in degreeCentralityList)
            {
                tableRows += string.Format("{0,-15} | {1,-10}\n", item.NodeName, item.Degree);
            }

            string finalMessage = "En Etkili 5 Kullanıcı (Degree Centrality):\n\n" + tableHeader + separator + tableRows;

            // 3. Sonucu Mesaj Kutusu ile Göster
            MessageBox.Show(finalMessage, "Merkezilik Analizi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "JSON Dosyası|*.json";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                SaveGraphToJson(saveFile.FileName);
            }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "JSON Dosyası|*.json";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                LoadGraphFromJson(openFile.FileName);
            }
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
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Tıklanan noktayı alıyoruz
            Point mouseLocation = e.Location;
            Node clickedNode = null;

            // Her düğümü kontrol et: Fare koordinatı düğüm dairesinin (50x50) içinde mi?
            foreach (var node in graph.Nodes)
            {
                // Düğümler 50x50 boyutunda çiziliyor
                Rectangle nodeRect = new Rectangle(node.Position.X, node.Position.Y, 50, 50);

                if (nodeRect.Contains(mouseLocation))
                {
                    clickedNode = node;
                    break;
                }
            }

            if (clickedNode != null)
            {
                // Düğüm bilgilerini topla
                int connectionCount = graph.Edges.Count(edge => edge.From == clickedNode || edge.To == clickedNode);

                string info = $"Düğüm Adı: {clickedNode.Name}\n" +
                              $"Toplam Bağlantı: {connectionCount}\n" +
                              $"Pozisyon: X:{clickedNode.Position.X}, Y:{clickedNode.Position.Y}";

                // Bilgiyi hem Label'da hem de Mesaj Kutusunda göster
                lblInfo.Text = $"Seçili: {clickedNode.Name}";
                MessageBox.Show(info, "Düğüm Bilgileri");
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
            var allPaths = graph.FindAllShortestPaths(start, end);

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
        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            string nodeName = txtNodeName.Text.Trim();
            var nodeToDelete = graph.Nodes.FirstOrDefault(n => n.Name == nodeName);

            if (nodeToDelete != null)
            {
                // 1. Düğümü sil
                graph.Nodes.Remove(nodeToDelete);

                // 2. Bu düğüme bağlı tüm kenarları (bağlantıları) temizle
                graph.Edges.RemoveAll(edge => edge.From == nodeToDelete || edge.To == nodeToDelete);

                // 3. Görseli ve pozisyonları güncelle
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

        // Çizim
        private void SaveGraphToJson(string filePath)
        {
            var dataToSave = new
            {
                Nodes = graph.Nodes.Select(n => new { n.Name, n.Position.X, n.Position.Y }).ToList(),
                Edges = graph.Edges.Select(e => new {
                    From = e.From.Name,
                    To = e.To.Name,
                    Type = e.Type.ToString(),
                    Weight = e.Weight
                }).ToList()
            };

            string json = new JavaScriptSerializer().Serialize(dataToSave);
            File.WriteAllText(filePath, json);
            MessageBox.Show("Veriler başarıyla kaydedildi!", "Sistem");
        }

        // Verileri JSON dosyasından geri yükleme
        private void LoadGraphFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            dynamic data = new JavaScriptSerializer().Deserialize<dynamic>(json);

            graph = new Graph(); // Mevcut grafı sıfırla
            nodeColors.Clear();

            // Düğümleri geri yükle
            foreach (var n in data["Nodes"])
            {
                Node newNode = new Node(n["Name"].ToString(), new Point((int)n["X"], (int)n["Y"]));
                graph.AddNode(newNode);
            }

            // Bağlantıları geri yükle
            foreach (var e in data["Edges"])
            {
                Node fromNode = graph.Nodes.First(n => n.Name == e["From"].ToString());
                Node toNode = graph.Nodes.First(n => n.Name == e["To"].ToString());
                RelationType type = (RelationType)Enum.Parse(typeof(RelationType), e["Type"].ToString());
                int weight = (int)e["Weight"];

                graph.AddEdge(fromNode, toNode, type, weight);
            }

            pictureBox1.Invalidate();
            MessageBox.Show("Veriler başarıyla geri yüklendi!", "Sistem");
        }
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
            // pictureBox1_Paint içindeki düğüm çizim döngüsünü bununla değiştirin:
            foreach (var node in graph.Nodes)
            {
                // Eğer bir renk ataması yapılmışsa o rengi kullan, yoksa Beyaz kullan
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
        // BFS Algoritması: Katman katman (genişlik öncelikli) arama yapar
        private List<Node> GetReachableNodesBFS(Node startNode)
        {
            List<Node> visited = new List<Node>();
            Queue<Node> queue = new Queue<Node>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                // Mevcut düğümün tüm komşularını bul
                var neighbors = graph.Edges
                    .Where(e => e.From == current || e.To == current)
                    .Select(e => e.From == current ? e.To : e.From);

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return visited;
        }

        // DFS Algoritması: Bir daldan sonuna kadar (derinlik öncelikli) gider
        private void GetReachableNodesDFS(Node current, List<Node> visited)
        {
            visited.Add(current);

            var neighbors = graph.Edges
                .Where(e => e.From == current || e.To == current)
                .Select(e => e.From == current ? e.To : e.From);

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    GetReachableNodesDFS(neighbor, visited);
                }
            }
        }
        private void ApplyWelshPowellColoring()
        {
            if (graph.Nodes.Count == 0) return;

            // HATALI SATIR: Dictionary<string, Color> nodeColors = new Dictionary<string, Color>(); 
            // DOĞRUSU: Sınıf düzeyindeki değişkeni temizleyip kullanıyoruz.
            nodeColors.Clear();

            var sortedNodes = graph.Nodes
                .Select(n => new {
                    Node = n,
                    Degree = graph.Edges.Count(e => e.From == n || e.To == n)
                })
                .OrderByDescending(x => x.Degree)
                .ToList();

            List<Color> palette = new List<Color> {
        Color.Red, Color.Blue, Color.Green, Color.Yellow,
        Color.Purple, Color.Orange, Color.Pink, Color.Cyan
    };

            int currentColorIndex = 0;
            var remainingNodes = sortedNodes.Select(x => x.Node).ToList();

            while (remainingNodes.Count > 0)
            {
                Color currentColor = palette[currentColorIndex % palette.Count];
                List<Node> coloredInThisRound = new List<Node>();

                for (int i = 0; i < remainingNodes.Count; i++)
                {
                    Node currentNode = remainingNodes[i];

                    // ÖNEMLİ: Sadece 'coloredInThisRound' listesindekilerle değil, 
                    // halihazırda O RENGE boyanmış tüm komşuları kontrol etmelisiniz.
                    bool hasNeighborWithSameColor = coloredInThisRound.Any(node => IsNeighbor(currentNode, node));

                    if (!hasNeighborWithSameColor)
                    {
                        nodeColors[currentNode.Name] = currentColor;
                        coloredInThisRound.Add(currentNode);
                    }
                }

                remainingNodes.RemoveAll(n => coloredInThisRound.Contains(n));
                currentColorIndex++;
            }

            // Tablo raporu hazırlama kısmı aynı kalabilir...
            string report = string.Format("{0,-15} | {1,-10} | {2,-10}\n", "Düğüm", "Derece", "Renk");
            report += new string('-', 40) + "\n";
            foreach (var item in sortedNodes)
            {
                string colorName = nodeColors.ContainsKey(item.Node.Name) ? nodeColors[item.Node.Name].Name : "Beyaz";
                report += string.Format("{0,-15} | {1,-10} | {2,-10}\n", item.Node.Name, item.Degree, colorName);
            }

            MessageBox.Show(report, "Welsh-Powell Boyama Tablosu");

            // ÇİZİMİ TETİKLE: Renklerin ekrana yansıması için Invalidate şarttır.
            pictureBox1.Invalidate();
        }

        // İki düğümün komşu olup olmadığını kontrol eden yardımcı fonksiyon
        private bool IsNeighbor(Node n1, Node n2)
        {
            return graph.Edges.Any(e =>
                (e.From == n1 && e.To == n2) ||
                (e.To == n1 && e.From == n2));
        }
        private void btnBFS_Click(object sender, EventArgs e)
        {
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            if (start == null) { MessageBox.Show("Başlangıç düğümünü girin!"); return; }

            List<Node> reachable = GetReachableNodesBFS(start);
            string result = string.Join(", ", reachable.Select(n => n.Name));
            MessageBox.Show($"BFS ile Erişilebilen Kullanıcılar:\n{result}", "BFS Sonucu");
        }

        private void btnDFS_Click(object sender, EventArgs e)
        {
            Node start = graph.Nodes.FirstOrDefault(n => n.Name == txtSource.Text.Trim());
            if (start == null) { MessageBox.Show("Başlangıç düğümünü girin!"); return; }

            List<Node> reachable = new List<Node>();
            GetReachableNodesDFS(start, reachable);

            string result = string.Join(", ", reachable.Select(n => n.Name));
            MessageBox.Show($"DFS ile Erişilebilen Kullanıcılar:\n{result}", "DFS Sonucu");
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ShowTop5Influencers();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnColoring_Click(object sender, EventArgs e)
        {
            ApplyWelshPowellColoring();
        }
    }
}
