using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataOrderDashboard.Context;
using DataOrderDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataOrderDashboard.Controllers
{
    public class MessageController : Controller
    {
        private readonly BigDataOrderContext _context;

        public MessageController(BigDataOrderContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            int pageSize = 15;
            var values = _context.Messages.OrderBy(x => x.MessageId).Skip((page - 1) * pageSize).Take(pageSize).Include(y => y.Customer).ToList();
            int totalCount = _context.Messages.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;
            return View(values);
        }
        public IActionResult GetMessageDetail(int id)
        {
            var message=_context.Messages.Include(x=>x.Customer).FirstOrDefault(x=>x.MessageId == id);
            return Json(new
            {
                success = true,
                messageId = message.MessageId,
                sender = $"{message.Customer.CustomerName}{message.Customer.CustomerSurname}",
                subject = message.MessageSubject,
                content = message.MessageText,
                date = message.CreatedDate.ToString("dd MMM yyyy HH:mm"),
                sentiment = message.SentimentLabel,
            });
        }
        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message)
        {
            var client=new HttpClient();
            var apiKey = "xxxxxxxxxxxxxxxx";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("User-Agent", "H&A App/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            try
            {
                var translateRequestBody = new
                {
                    inputs = message.MessageText
                };
                var translateJson=JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");
                var translateResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/Helsinki-NLP/opus-mt-tr-en"
                    , translateContent);
                var translateResponseString=await translateResponse.Content.ReadAsStringAsync();
                string englishText = message.MessageText;
                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc=JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }
                //Toksiklik Kontrolü
                var toxicRequestBody = new
                {
                    inputs = englishText
                };
                var toxicJson=JsonSerializer.Serialize(toxicRequestBody);
                var toxicContent=new StringContent(toxicJson, Encoding.UTF8, "application/json");
                var toxicResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", toxicContent);
                var toxicResponseString=await toxicResponse.Content.ReadAsStringAsync();
                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                   foreach(var item in toxicDoc.RootElement[0].EnumerateArray())
                    {
                        string label=item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();

                        if(score>0.5)
                        {
                            message.SentimentLabel = "Toksik İçerik";
                            break;

                        }
                        else
                        {
                            message.SentimentLabel = "Uygun İçerik";
                        }
                    } 
                }

            }
            catch(Exception ex)
            {
                message.SentimentLabel = "Hata Oluştu";

            }
            if (message.CreatedDate == default)
            {
                message.CreatedDate = DateTime.Now;
            }

            var values = _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteMessage(int id)
        {
            var values = _context.Messages.Find(id);
            _context.Messages.Remove(values);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult UpdateMessage(int id)
        {
            var values = _context.Messages.Find(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult UpdateMessage(Message Message)
        {
            var values = _context.Messages.Update(Message);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
