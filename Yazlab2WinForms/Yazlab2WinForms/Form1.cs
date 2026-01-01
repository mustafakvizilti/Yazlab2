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
        private Node draggingNode = null; // O an sürüklenen düğüm
        private Point dragOffset;         // Farenin düğüm içindeki konumu
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
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Tıklanan noktada bir düğüm var mı kontrol et
            foreach (var node in graph.Nodes)
            {
                // Düğümler 50x50 boyutunda çiziliyor
                Rectangle nodeRect = new Rectangle(node.Position.X, node.Position.Y, 50, 50);

                if (nodeRect.Contains(e.Location))
                {
                    draggingNode = node;
                    // Tıklanan nokta ile düğümün sol üst köşesi arasındaki farkı kaydet
                    dragOffset = new Point(e.X - node.Position.X, e.Y - node.Position.Y);
                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingNode != null)
            {
                // Düğümün yeni konumunu fareye göre güncelle
                draggingNode.Position = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);

                // Çizimi anlık olarak yenile
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Sürükleme işlemini bitir
            draggingNode = null;
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

            // 1. Yeni kutudan aktiflik değerini alıyoruz
            double girilenAktiflik = (double)numAktiflik.Value;

            if (string.IsNullOrEmpty(nodeName))
            {
                // Otomatik isim verme mantığı (mevcut kodun)
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
            Point mouseLocation = e.Location; // Tıklanan yeri al
            Node clickedNode = null;

            // 1. Tıklanan düğümü tespit et
            foreach (var node in graph.Nodes)
            {
                // Düğümler 50x50 çiziliyor
                Rectangle nodeRect = new Rectangle(node.Position.X, node.Position.Y, 50, 50);
                if (nodeRect.Contains(mouseLocation))
                {
                    clickedNode = node;
                    break;
                }
            }

            // 2. Eğer bir düğüm bulunduysa işlemleri yap
            if (clickedNode != null)
            {
                // İstatistikleri hesapla
                int connectionCount = graph.Edges.Count(edge => edge.From == clickedNode || edge.To == clickedNode);

                // --- POPUP BURADA BAŞLIYOR ---
                string infoText = $"--- KULLANICI DETAYLARI ---\n" +
                                  $"Adı: {clickedNode.Name}\n" +
                                  $"Aktiflik: {clickedNode.Aktiflik}\n" +
                                  $"Bağlantı Sayısı: {connectionCount}\n" +
                                  $"İlişki Gücü Toplamı: {clickedNode.IliskiGucu}";

                // Bilgiyi ekrana bas (Pop-up'ı geri getirdik)
                MessageBox.Show(infoText, "Düğüm Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // -----------------------------

                // Güncelleme için kutucukları doldur
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

            try
            {
                string json = File.ReadAllText(filePath);
                dynamic data = new JavaScriptSerializer().Deserialize<dynamic>(json);

                // 1. Mevcut veriyi tamamen temizle
                graph = new Graph();
                nodeColors.Clear();

                // 2. Düğümleri tek tek oku (Aktiflik dahil)
                foreach (var n in data["Nodes"])
                {
                    string name = n["Name"].ToString();
                    // JSON'dan gelen sayıları güvenli bir şekilde tam sayıya çeviriyoruz
                    int x = Convert.ToInt32(n["X"]);
                    int y = Convert.ToInt32(n["Y"]);
                    // Aktiflik değerini de geri yüklüyoruz
                    double aktif = n.ContainsKey("Aktiflik") ? Convert.ToDouble(n["Aktiflik"]) : 0.5;

                    Node newNode = new Node(name, new Point(x, y), aktif);
                    graph.AddNode(newNode);
                }

                // 3. Bağlantıları geri yükle
                foreach (var e in data["Edges"])
                {
                    Node fromNode = graph.Nodes.First(node => node.Name == e["From"].ToString());
                    Node toNode = graph.Nodes.First(node => node.Name == e["To"].ToString());
                    RelationType type = (RelationType)Enum.Parse(typeof(RelationType), e["Type"].ToString());
                    int weight = Convert.ToInt32(e["Weight"]);

                    graph.AddEdge(fromNode, toNode, type, weight);
                }

                // 4. Ekranda görünmesi için zorla tazele
                pictureBox1.Refresh();
                MessageBox.Show("Graf verileri, Aktiflikler ve Matris başarıyla yüklendi!", "Sistem");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Yükleme sırasında bir hata oluştu: " + ex.Message);
            }
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
            List<Node> reachable = alg.GetReachableNodesBFS(graph, startNode); // [cite: 34]

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

            // DFS metodunu Algorithm sınıfından çağırıyoruz [cite: 34, 45]
            alg.GetReachableNodesDFS(graph, startNode, reachable);

            string result = string.Join(", ", reachable.Select(n => n.Name));
            MessageBox.Show($"DFS ile Erişilebilen Kullanıcılar:\n{result}", "DFS Analizi");
        }

        private void button5_Click(object sender, EventArgs e) // Top5Node Butonu
        {
            if (graph.Nodes.Count == 0) return;

            // Analizi Manager üzerinden yapıyoruz [cite: 37]
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

            // 1. İsme göre ilgili düğümü bul
            var targetNode = graph.Nodes.FirstOrDefault(n => n.Name == nodeName);

            if (targetNode != null)
            {
                // 2. Yeni aktiflik değerini ata
                double yeniAktiflik = (double)numAktiflik.Value;
                targetNode.Aktiflik = yeniAktiflik; //

                // 3. Bilgi ver ve görseli yenile
                lblInfo.Text = $"{targetNode.Name} düğümü güncellendi. Yeni Aktiflik: {yeniAktiflik}";
                pictureBox1.Invalidate(); // Renkler veya yollar değişebileceği için grafı yeniden çizdir

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

            // Mevcut tüm renkleri temizle (isteğe bağlı, her aramada sıfırlamak için)
            nodeColors.Clear();

            // Düğümü bul
            var foundNode = graph.Nodes.FirstOrDefault(n => n.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));

            if (foundNode != null)
            {
                // Bulunan düğümü vurgula (Örn: Sarı renk)
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
    }
}
