using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using 小说漫画阅读器.JsonClass;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MangadexController : ControllerBase
    {
        private readonly HttpClient httpClient;


        public MangadexController(IHttpClientFactory factory)
        {
            httpClient = factory.CreateClient("MangaClient");
        }


        [HttpGet]
        //根据标题得到漫画id，模糊匹配
        public async Task<ActionResult<List<Guid>>> GetManga([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("title 不能为空");
            //string title = "Kanojyo to Himitsu to Koimoyou";
            string url = $"manga?title={Uri.EscapeDataString(title)}&limit=100&includes[]=cover_art";
            //string url = $"manga?title={title}& limit=100";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            var ids = doc.RootElement.GetProperty("data")
                                 .EnumerateArray()
                                 .Select(m => m.GetProperty("id").GetGuid()) // GetGuid() 把 UUID 字符串转 Guid
                                 .ToList();

            return ids; // 会自动序列化为 JSON
        }
        [HttpGet]
        //根据标签得到漫画id,limit=30
        public async Task<ActionResult<List<string>>> SearchMangaByTags()
        {
            // Step 1: 标签名称
            var includedTagNames = new[] { "Action", "Romance" };
            var excludedTagNames = new[] { "Harem" };

            // Step 2: 获取所有标签
            var tagResp = await httpClient.GetAsync("manga/tag");
            tagResp.EnsureSuccessStatusCode();
            var tagJson = await tagResp.Content.ReadAsStringAsync();


            using var tagDoc = JsonDocument.Parse(tagJson);
            var tagList = tagDoc.RootElement.GetProperty("data").EnumerateArray();

            // Step 3: 找到 UUID
            var includedTagIDs = tagList
                .Where(tag => includedTagNames.Contains(tag.GetProperty("attributes").GetProperty("name").GetProperty("en").GetString()))
                .Select(tag => tag.GetProperty("id").GetString())
                .ToList();

            var excludedTagIDs = tagList
                .Where(tag => excludedTagNames.Contains(tag.GetProperty("attributes").GetProperty("name").GetProperty("en").GetString()))
                .Select(tag => tag.GetProperty("id").GetString())
                .ToList();

            // Step 4: 构造查询字符串
            var queryParams = new List<string>();

            foreach (var id in includedTagIDs)
                queryParams.Add($"includedTags[]={id}");

            foreach (var id in excludedTagIDs)
                queryParams.Add($"excludedTags[]={id}");

            // limit 可选
            queryParams.Add("limit=30");

            var queryString = string.Join("&", queryParams);
            var searchUrl = $"manga?{queryString}";

            // Step 5: 搜索漫画
            var searchResp = await httpClient.GetAsync(searchUrl);
            searchResp.EnsureSuccessStatusCode();
            var searchJson = await searchResp.Content.ReadAsStringAsync();

            // Step 6: 取 manga ID 列表
            using var searchDoc = JsonDocument.Parse(searchJson);
            var mangas = searchDoc.RootElement.GetProperty("data").EnumerateArray()
                .Select(m => m.GetProperty("id").GetString())
                .ToList();

            return mangas;
        }
        [HttpGet]
        //得到漫画封面的图片的方法,uuid+文件名,可以在.jpg前面加.256或者.512
        public async Task<ActionResult> GetCoverImg([FromQuery] string uuid, string fileName)
        {
            fileName = "0f411c81-14a6-4d59-9e57-869d839c4972.jpg";
            uuid = "831b12b8-2d0e-4397-8719-1efee4c32f40";
            var response = await httpClient.GetAsync($"https://uploads.mangadex.org/covers/{uuid}/{fileName}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取封面失败");
            }
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            return File(imageBytes, contentType);
        }

        [HttpGet]
        //根据漫画的uuid得到漫画的评分
        public async Task<IActionResult> GetCommetnById([FromQuery] string uuid)
        {
            uuid = "0301208d-258a-444a-8ef7-66e433d801b1";

            var response = await httpClient.GetAsync($"/statistics/manga/{uuid}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "评分获取失败");
            }

            using var sJson = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<MangaStatisticsResponse>(sJson);
            if (json != null && json.statistics.TryGetValue(uuid, out var stat))
            {
                return Ok(json);
            }

            return NotFound("Manga statistics not found");
        }

        [HttpGet]
        //根据标题得到漫画的的uuid和封面文件名
        public async Task<ActionResult<CoverResponse>> GetManga1([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("title 不能为空");
            //string title = "Kanojyo to Himitsu to Koimoyou";
            string url = $"manga?title={Uri.EscapeDataString(title)}&limit=100&includes[]=cover_art";
            //string url = $"manga?title={title}& limit=100";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }

            using var json1 = await response.Content.ReadAsStreamAsync();

            var json = JsonSerializer.Deserialize<CoverResponse>(json1);

            if (json != null)
            {
                return Ok(json);
            }
            return NotFound("Manga statistics not found");
        }
        [HttpGet]
        //根据uuid来查找对应的所有章节信息，里面有最基本的章节id
        public async Task<ActionResult<ChapterResponse>> GetFeed(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return BadRequest("uuid 不能为空");
            string url = $"/manga/{uuid}/feed";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "获取失败");
            }

            using var sJson = await response.Content.ReadAsStreamAsync();
            //var sJson = await response.Content.ReadAsStringAsync();

            //if (sJson != null)
            //{
            //    return Ok(sJson);
            //}

            var json = JsonSerializer.Deserialize<ChapterResponse>(sJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (json != null)
            {
                return Ok(json);
            }
            return NotFound("没有你想要的内容");
        }
        [HttpGet]
        //根据章节id得到对应的图片内容
        public async Task<ActionResult<ChapterInfo>> GetChapterImg(string chapterId)
        {

            if (string.IsNullOrWhiteSpace(chapterId))
                return BadRequest("参数空缺");

            string GetChapterInfo = $"https://api.mangadex.org/at-home/server/{chapterId}";

            var response = await httpClient.GetAsync(GetChapterInfo);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound("没找到这个章节的信息");
            }
            using var sJson = await response.Content.ReadAsStreamAsync();
            var json = JsonSerializer.Deserialize<ChapterInfo>(sJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            var base64Images = new List<string>();
            if (json != null)
            {
                string baseUrl = json.baseUrl;
                string hash = json.chapter.hash;
                foreach (var fileName in json.chapter.data)
                {
                    string imageUrl = $"{baseUrl}/data/{hash}/{fileName}";
                    var imgResponse = await httpClient.GetAsync(imageUrl);
                    if (!imgResponse.IsSuccessStatusCode) continue;

                    var bytes = await imgResponse.Content.ReadAsByteArrayAsync();
                    string base64 = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
                    base64Images.Add(base64);
                }
                return Ok(base64Images);
            }
            return NotFound("json为空");
        }
    }
    public class MangaStatisticsResponse
    {

        public Dictionary<string, MangaStat> statistics { get; set; }
    }

    public class MangaStat
    {
        public Rating rating { get; set; }
        public int follows { get; set; }
    }

    public class Rating
    {
        public double average { get; set; }
        public double bayesian { get; set; }
    }
}
