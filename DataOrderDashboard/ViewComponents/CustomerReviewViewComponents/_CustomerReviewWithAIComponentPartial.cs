using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataOrderDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.ViewComponents.CustomerReviewViewComponents
{
    public class _CustomerReviewWithAIComponentPartial:ViewComponent
    {
        private readonly BigDataOrderContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public _CustomerReviewWithAIComponentPartial(BigDataOrderContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            id = 175;
            var reviews = await _context.Reviews
                 .Where(r => r.CustomerId == id)
                 .OrderByDescending(r => r.ReviewDate)
                 .Take(10)
                 .Select(r => new
                 {
                     r.Rating,
                     r.Sentiment,
                     r.ReviewText,
                     r.ReviewDate
                 })
                 .ToListAsync();
            var jsonData = JsonSerializer.Serialize(reviews);
            string prompt = $@"⚠️ Çok kritik kurallar:
            - Kesinlikle kendi formatını kullanma.
            - Sadece benim verdiğim HTML şablonunu doldur.
            - HTML etiketlerini değiştirme.
            - Başlıkları değiştirme.
            - Kod bloğu verme.
            - Markdown verme.
            - Yorum yapma, açıklama ekleme.
            - Sadece saf HTML döndür.

            🔽 Veriler:
            {jsonData}

            🔽 Doldurman gereken HTML şablonu (TAM OLARAK BUNU DOLDUR):
            <h3>1. 👤 Müşteri Profili</h3>
            <p><b>Ad:</b> [AD]</p>
            <p><b>Soyad:</b> [SOYAD]</p>
            <p><b>Toplam Sipariş:</b> [TOPLAM_SIPARIS]</p>
            <p><b>Toplam Harcama:</b> [TOPLAM_HARCAMA] ₺</p>

            <h3>2. 🛍️ Ürün Tercihleri</h3>
            <ul>
              <li>🏠 Ev & Dekorasyon – [X] sipariş</li>
              <li>💄 Kozmetik – [X] sipariş</li>
            </ul>
            <p><b>Öne çıkan ürünler:</b></p>
            <ul>
              <li>[ÜRÜN ADI] ([ADET] — [FIYAT] ₺)</li>
            </ul>

            <h3>3. ⏰ Zaman Bazlı Alışveriş Davranışı</h3>
            <p>En yoğun ay: [AY]</p>
            <p>En yoğun gün: [GUN]</p>
            <p>Favori saat aralığı: [SAAT_ARALIGI]</p>

            <h3>4. 💰 Ortalama Harcama ve Sıklık</h3>
            <p>Aylık ortalama sipariş: [AYLIK]</p>
            <p>Ortalama sepet tutarı: [ORT_SEPET] ₺</p>
            <p>En yüksek sipariş: [MAX] ₺</p>
            <p>En düşük sipariş: [MIN] ₺</p>

            <h3>5. 🎯 Sadakat ve Tekrar Harcama Eğilimi</h3>
            <p>Tekrar alışveriş eğilimi: [TEKRAR]</p>
            <p>Marka sadakati: [MARKA_SADakati]</p>
            <p>Kategori sadakati: [KAT_SADakati]</p>

            <h3>6. 🚀 Pazarlama Önerileri</h3>
            <ul>
              <li>🎁 Kampanya önerisi: [KAMPANYA]</li>
              <li>✉️ Hedefli e-posta: [MAIL]</li>
              <li>🆕 Yeni ürün tanıtımı önerisi: [YENI_URUN]</li>
            </ul>

            Sadece yukarıdaki HTML’nin içindeki [KÖŞELİ ALANLARI] doldur. Başka hiçbir şey ekleme.";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://chatgpt-42.p.rapidapi.com/aitohuman");
            request.Headers.Add("x-rapidapi-key", "1b01b4abbfmsh618c142c34ac024p1583c7jsn34ff990c3e13");
            request.Headers.Add("x-rapidapi-host", "chatgpt-42.p.rapidapi.com");

            var payload = new
            {
                text = prompt,
                temperature = 0.3,
                max_tokens = 700
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            string aiResult;
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                aiResult = await response.Content.ReadAsStringAsync();
            }

            ViewBag.AIAnalysis = aiResult;

            return View(reviews);
        }
    }
}
