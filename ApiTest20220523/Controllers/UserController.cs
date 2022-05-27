using ApiTest20220523.Context;
using ApiTest20220523.Models;
using ApiTest20220523.Services;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest20220523.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJwtAuth _jwtAuth;
        private readonly AppDbContext _db;
        public UserController(IConfiguration configuration, IJwtAuth jwtAuth)
        {
            _jwtAuth = jwtAuth;
            _db = new AppDbContext();
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File(Path.Combine(configuration.GetSection("LogFolderPath").Value, "myapp.txt"), rollingInterval: RollingInterval.Day)
               .CreateLogger();
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
            await _db.Users.AddRangeAsync(newList);
            await _db.SaveChangesAsync();

            var lst = await _db.Users.Where(x => x.delFlag == false).ToListAsync();
            Log.Information(JsonConvert.SerializeObject(lst));
            return Ok(lst);
        }

        [HttpGet("{pageNo:int}/{rowCount:int}")]
        public async Task<IActionResult> GetUserList(int pageNo = 1, int rowCount = 10)
        {
            LogModel log = new LogModel();
            log.RequestDateTime = DateTime.Now;
            log.RequestData = new { pageNo = pageNo, rowCount = rowCount };
            log.RequestUrl = Request.Path;
            List<UserModel> lst = new List<UserModel>();
            try
            {
                lst = await _db.Users.Where(x => x.delFlag == false).OrderByDescending(x => x.userId)
                   .Skip(pageNo * rowCount - rowCount).Take(rowCount).ToListAsync();

                log.respCode = "000";
                log.respDesp = "Success";
                log.ResponseData = lst;
                log.ResponseDateTime = DateTime.Now;
                Log.Information(JsonConvert.SerializeObject(log, Formatting.Indented));
            }
            catch (Exception ex)
            {
                log.respCode = "999";
                log.respDesp = ex.Message + ex.StackTrace;
                log.ResponseDateTime = DateTime.Now;
                Log.Error(JsonConvert.SerializeObject(log, Formatting.Indented));
            }
            return Ok(lst);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var item = await _db.Users.Where(x => x.userId == id).FirstOrDefaultAsync();
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
                await _db.Users.AddAsync(item);
                var count = await _db.SaveChangesAsync();
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
                _db.Users.Update(item);
                var count = await _db.SaveChangesAsync();
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
                var item = await _db.Users.Where(x => x.userId == id).FirstOrDefaultAsync();
                item.delFlag = true;
                _db.Users.Update(item);
                var count = await _db.SaveChangesAsync();
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

public class LogModel : ResponseModel
{
    public DateTime RequestDateTime { get; set; }
    public DateTime ResponseDateTime { get; set; }
    public object ResponseData { get; set; }
    public string RequestUrl { get; set; }
    public object RequestData { get; set; }
}
