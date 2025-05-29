using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly CancellationTokenPool pool;
        public TestController(CancellationTokenPool pool)
        {
            this.pool = pool;
        }
        [HttpGet]
        public IActionResult Get()
        {
            string filePath = @"C:\Users\Lenovo\Pictures\1.jpg";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // 文件不存在时返回 404
            }
            FileStream f = new FileStream(@"C:\Users\Lenovo\Pictures\1.jpg",FileMode.Open,FileAccess.Read,FileShare.Read);
            return File(f,"image/jpeg");
        }
        [HttpGet]
        public async Task<IActionResult> GetFanRen(int chapter)
        {
            string filePath = @"C:\Users\Lenovo\Downloads\凡人修仙传.txt";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // 文件不存在时返回 404
            }
        
            string allText="";
            try
            {
                allText= await System.IO.File.ReadAllTextAsync(filePath, pool.GetToken("小说加载"));

                string zhang_jie = ChineseNumberConverter.ToChinese(chapter);
                string zhang_jie2 = ChineseNumberConverter.ToChinese(chapter+1);
                string pattern = $@"第{zhang_jie}章[\s\S]*?(?=第{zhang_jie2}章|$)";
                var match = Regex.Match(allText, pattern);
                Console.WriteLine(match.Value);
                if (!match.Success)
                    return NotFound("未找到该章节");
                return StatusCode(200, match.Value.Trim());
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499,"取消加载");
            }
            
            //return NotFound();
        }
    }
}
