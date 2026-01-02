***Proje Raporu: Sosyal Ağ Analizi Uygulaması***
**1. Proje Bilgileri**

Proje Adı: Dinamik Ağırlıklandırmalı Sosyal Ağ Analiz Sistemi

Grup: 48

Ekip Üyeleri: Furkan Kerim Ocak(231307030), Mustafa Küçükvızıltı(231307098)

Kurum: Kocaeli Üniversitesi Teknoloji Fakültesi Bilişim Sistemleri Mühendisliği

Tarih: 1 Ocak 2026
**2. Giriş: Problemin Tanımı ve Amaç**

Günümüz sosyal ağları, kullanıcı etkileşimlerinin anlık olarak değiştiği son derece dinamik yapılardır. Geleneksel graf algoritmaları genellikle statik ağırlıklar üzerinden çalışırken, bu projede kullanıcıların aktiflik düzeyleri ve ağdaki popülerliklerine göre değişen bir maliyet hesaplama sistemi hedeflenmiştir. Temel amaç, bir sosyal ağdaki kullanıcılar arasındaki en kısa yolu, kullanıcıların anlık verilerini (aktiflik, ilişki gücü ve bağlantı sayısı) temsil eden dinamik bir formül aracılığıyla analiz etmek ve bu verileri profesyonel bir mimari yapıda görselleştirmektir.

**3. Gerçekleştirilen Algoritmalar**

**3.1. Dijkstra Algoritması**

Dijkstra algoritması, sosyal ağdaki bir kullanıcıdan diğerine olan tüm alternatif rotaları tarayarak en düşük maliyetli yolu bulmaktadır. Algoritma her adımda komşu düğümleri ziyaret ederken, kenar maliyetini 1+(Ai​−Aj​)^2+(Ii​−Ij​)^2+(Bi​−Bj​)^2​ formülünün karekökünü alarak anlık olarak hesaplar. Literatürde "Greedy" (açgözlü) bir yaklaşım olarak bilinen bu yöntem, negatif ağırlık içermeyen sosyal ağ yapılarında kesin en kısa yolu garanti etmesi nedeniyle tercih edilmiştir. Zaman karmaşıklığı, öncelikli kuyruk kullanımıyla beraber O((V+E)logV) olarak analiz edilmiştir.


<img width="399" height="700" alt="resim" src="https://github.com/user-attachments/assets/eb2eb370-13de-4e00-81ba-18d3a93d8eb6" />

**3.2. A* (A-Star) Algoritması**

A* algoritması, Dijkstra'nın en kısa yol mantığını hedefe olan kuş uçuşu mesafeyi (Öklid uzaklığı) temsil eden bir sezgisel (heuristic) fonksiyonla birleştirir. Bu yöntem, arama uzayını sadece hedef yönünde daraltarak işlem süresini ciddi oranda azaltmaktadır. Literatürde yol bulma problemlerinde en verimli yöntemlerden biri olarak kabul edilen A* algoritmasının performansı, seçilen sezgisel fonksiyonun kalitesine doğrudan bağlıdır. Karmaşıklığı, sezgisel değerin kalitesine göre değişmekle birlikte genellikle Dijkstra ile benzer seviyededir.


<img width="492" height="775" alt="resim" src="https://github.com/user-attachments/assets/f6eb4b0b-6940-4181-84d5-d4c0a372dacc" />

**3.3. BFS ve DFS Algoritmaları**

Genişlik Öncelikli Arama (BFS) katman katman ilerleyerek bir kullanıcıdan ulaşılabilecek en yakın kullanıcıları tespit ederken; Derinlik Öncelikli Arama (DFS) bir daldan gidebildiği en uzak noktaya kadar ilerler. Sosyal ağlarda erişilebilirlik analizi yapmak ve ağın bağlantı bileşenlerini bulmak için kullanılan bu algoritmalar O(V+E) karmaşıklığı ile çalışır.

**3.4. Welsh-Powell Algoritması**

Graf renklendirme problemi için kullanılan bu algoritma, düğümleri bağlantı derecelerine göre büyükten küçüğe sıralar ve komşu düğümlerin aynı renge boyanmasını engelleyerek ağın görsel haritasını çıkarır. Karmaşıklığı O(V2+VE) olan bu yöntem, ağdaki popüler grupların görsel olarak ayrıştırılmasında etkin bir rol oynamaktadır.

**4. Sınıf Yapısı ve Modüller**

Uygulama, "Sorumlulukların Ayrılması" (Separation of Concerns) ilkesine dayalı dört temel modülden oluşmaktadır:

   Modeller (Models): Node, Edge ve Graph sınıfları ağın temel veri yapılarını ve düğüm özelliklerini tanımlar.

   İş Mantığı (Business): Algorithm ve GraphManager sınıfları, arayüzler (IAlgorithmService, IGraphService) üzerinden en kısa yol ve analiz işlemlerini yürütür.

   Veri Erişimi (DataAccess): GraphData sınıfı, ağ yapısını JSON formatında kalıcı hale getirir ve akademik analizler için Komşuluk Matrisini üretir.

   Arayüz (UI): Form1 sınıfı, kullanıcının grafı yönettiği ve sonuçları dinamik bir tabloda (DataGridView) izlediği katmandır.

**5. Uygulama Açıklamaları ve Testler**

Uygulama başlatıldığında kullanıcıya etkileşimli bir çizim alanı sunar. Kullanıcılar düğüm ekleyebilir, bu düğümlere 0.1 ile 1.0 arasında aktiflik değerleri atayabilir ve düğümler arası ilişki türlerini (Akraba, İş vb.) belirleyebilirler.

<img width="1694" height="855" alt="resim" src="https://github.com/user-attachments/assets/0f9411b2-d513-4c87-9fd9-a28c9acc9c77" />


Test Senaryosu 1: Beş düğümlü bir ağda A düğümünden E düğümüne Dijkstra ve A* karşılaştırması yapılmıştır. Dijkstra tüm alternatif en kısa yolları bulurken, A* algoritmasının sezgisel fonksiyonu sayesinde hedef odaklı daha hızlı sonuç verdiği gözlemlenmiştir. Sonuçlar süre ve maliyet bazında tabloda raporlanmıştır.

<img width="249" height="93" alt="resim" src="https://github.com/user-attachments/assets/86b4c33e-b423-4721-b498-15b57828b69f" />


Test Senaryosu 2: Kaydet/Yükle fonksiyonu test edilmiş; JSON dosyasının içerisinde hem düğüm özelliklerinin hem de komşuluk matrisinin doğru şekilde oluştuğu teyit edilmiştir.

<img width="1413" height="815" alt="resim" src="https://github.com/user-attachments/assets/d968a07a-4065-431c-b5e0-1a48a3d9b536" />

**6. Sonuç ve Tartışma**

Geliştirilen sistem, sosyal ağ analizlerinde dinamik verilerin yol bulma süreçlerini nasıl etkilediğini başarılı bir şekilde kanıtlamıştır. Projenin en büyük başarısı, ayrık mimari sayesinde modüler bir yapı sunması ve karmaşık dinamik formülleri gerçek zamanlı olarak işleyebilmesidir. Sistemin en temel sınırlılığı, çok büyük düğüm sayılarında görselleştirme performansının düşmesidir. Gelecekte bu sistem, gerçek sosyal medya API'ları ile entegre edilerek gerçek zamanlı veri akışıyla çalışacak şekilde geliştirilebilir.

