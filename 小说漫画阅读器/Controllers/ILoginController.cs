using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace 小说漫画阅读器.Controllers
{
    public interface ILoginController
    {
        [HttpPost]
        public IActionResult Rejister(UserData userData);
        [HttpGet]
        public ActionResult<int> Login(UserData userData);
    }
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase, ILoginController
    {

        [HttpPost]
        public ActionResult<int> Login(UserData userData)
        {
            using MyDbContext db = new MyDbContext();
            var data = db.Users.SingleOrDefault(e => e.Name == userData.UserName);
            if(data == null){
                return StatusCode(2,"不存在当前用户");
            }
            return Ok(data.Id);
        }
        [HttpPost]
        public IActionResult Rejister(UserData userData)
        {
            using MyDbContext db = new MyDbContext();
            var data = db.Users.SingleOrDefault(e=>e.Name==userData.UserName);
            if (data != null)
            {
                return StatusCode(1,"用户名已经注册");
            }
            User user=new User() { Name=userData.UserName,PassWord=userData.Password};
            db.Users.Add(user);
            db.SaveChanges();
            return StatusCode(200,"注册成功");
        }
 
    }
    public record UserData(string UserName,string Password);
}
