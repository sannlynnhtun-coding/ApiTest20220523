using ApiTest20220523.Context;
using ApiTest20220523.Models;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest20220523.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext db;
        public UserController()
        {
            db = new AppDbContext();
        }

        // CRUD
        // get => read
        // post => create
        // put => update
        // delete => delete

        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            var item = new Faker<UserModel>()
                .RuleFor(u => u.userCode, f => Guid.NewGuid().ToString())
                  .RuleFor(u => u.userName, (f, u) => f.Name.FirstName(Bogus.DataSets.Name.Gender.Male) + " " + f.Name.LastName(Bogus.DataSets.Name.Gender.Male));
            var newList = Enumerable.Range(1, 10).Select(x => item.Generate());
            await db.Users.AddRangeAsync(newList);
            await db.SaveChangesAsync();

            var lst = await db.Users.Where(x => x.delFlag == false).ToListAsync();
            return Ok(lst);
        }

        [HttpGet("{pageNo:int}/{rowCount:int}")]
        public async Task<IActionResult> GetUserList(int pageNo = 1, int rowCount = 10)
        {
            var lst = await db.Users.Where(x => x.delFlag == false).OrderByDescending(x => x.userId)
                .Skip(pageNo * rowCount - rowCount).Take(rowCount).ToListAsync();
            return Ok(lst);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var item = await db.Users.Where(x => x.userId == id).FirstOrDefaultAsync();
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserModel item)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                await db.Users.AddAsync(item);
                var count = await db.SaveChangesAsync();
                model.respCode = count == 1 ? "000" : "999";
                model.respDesp = count == 1 ? "saving successful!" : "saving failed!";
            }
            catch (Exception ex)
            {
                model.respCode = "999";
                model.respDesp = ex.Message + ex.StackTrace;
            }
            return Ok(model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UserModel item)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                item.userId = id;
                db.Users.Update(item);
                var count = await db.SaveChangesAsync();
                model.respCode = count == 1 ? "000" : "999";
                model.respDesp = count == 1 ? "updating successful!" : "updating failed!";
            }
            catch (Exception ex)
            {
                model.respCode = "999";
                model.respDesp = ex.Message + ex.StackTrace;
            }
            return Ok(model);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var item = await db.Users.Where(x => x.userId == id).FirstOrDefaultAsync();
                item.delFlag = true;
                db.Users.Update(item);
                var count = await db.SaveChangesAsync();
                model.respCode = count == 1 ? "000" : "999";
                model.respDesp = count == 1 ? "deleting successful!" : "deleting failed!";
            }
            catch (Exception ex)
            {
                model.respCode = "999";
                model.respDesp = ex.Message + ex.StackTrace;
            }
            return Ok(model);
        }
    }
}
