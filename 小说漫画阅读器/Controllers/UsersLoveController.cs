using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace 小说漫画阅读器.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersLoveController : ControllerBase
    {
        //离线下载的漫画不需要用户，因为就是下载到本地的，难道用户从另外的电脑登录会有当初他离线下载的漫画嘛？

        /// <summary>
        /// 发送用户id,得到用户喜欢的漫画的uuid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration =10,VaryByQueryKeys = new[] {"id"})]
        public ActionResult<List<string>> GetUsersLove([FromQuery]int id)
        {
            using MyDbContext db = new MyDbContext();
            var data=db.Users.Include(e=>e.UsersLove).SingleOrDefault(e=>e.Id==id);
            if (data == null)
            {
                return NotFound("不存在此用户");
            }
            List<string> res = new List<string>();
            foreach (var item in data.UsersLove.ToList()) 
            {
                res.Add(item.MangaId);
            }
            return Ok(res);
        }
        /// <summary>
        /// 添加用户喜欢的漫画的uuid
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUsersLove(string uuid, int id)
        {
            using MyDbContext db = new MyDbContext();
            var data = db.Users.SingleOrDefault(e => e.Id == id);
            if (data == null)
            {
                return NotFound("不存在此用户");
            }

            UsersLove love=new UsersLove() { User=data,MangaId=uuid};
            db.UsersLoves.Add(love);
            db.SaveChanges();
            return Ok("成功添加");
        }
        [HttpPost]
        public ActionResult SubUsersLove(string uuid, int id)
        {
            using MyDbContext db = new MyDbContext();
            var data=db.UsersLoves.Include(e=>e.User).SingleOrDefault(e=>e.MangaId==uuid);
            if (data == null)
            {
                return NotFound("数据出错了");
            }
            if (data.User.Id != id)
            {
                return NotFound("用户id出错了");
            }
            db.UsersLoves.Remove(data);
            db.SaveChanges();
            return Ok("成功删除");
        }
    }
}
