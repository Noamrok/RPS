using RPS_WebAPI.DTO;
using RPS_WebAPI.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace RPS_WebAPI.Controllers
{
    public class PlayerController : ApiController
    {
        [HttpGet]
        [Route("api/Player/GetPlayerStats/{username}")]
        public PlayerStats GetPlayerStats(string username)
        {
            RPSDbContext db = new RPSDbContext();
            return db.Players.Select(x => new PlayerStats()
            {
                Username = x.Username,
                Password = x.Password,
                Email = x.Email,
                Wins = x.Wins ?? 0,
                Loses = x.Loses ?? 0,
                NumOfGames = x.NumOfGames ?? 0,
            }).Single(x => x.Username == username);
        }

        [Route("api/Player/GetPlayerLoginInfo/{username}")]
        // GET api/<controller>/5
        public HttpResponseMessage GetPlayerLoginInfo(string username)
        {
            RPSDbContext db = new RPSDbContext();
            Player p = db.Players.SingleOrDefault(x => x.Username == username);
            if (p == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, new PlayerLoginInfo() 
            {
                Username = p.Username,
                Password = p.Password
            });
        }

        //[HttpPost]
        [Route("api/Player/PostNewPlayer")]
        public HttpResponseMessage PostNewPlayer()
        {
            RPSDbContext db = new RPSDbContext();
            string newUsername = HttpContext.Current.Request.Params["username"];
            Player p = db.Players.SingleOrDefault(x => x.Username == newUsername);
            if (p == null)
            {
                Player player = new Player()
                {
                    Username = newUsername,
                    Password = HttpContext.Current.Request.Params["password"],
                    Email = HttpContext.Current.Request.Params["email"],
                    Wins = 0,
                    Loses = 0,
                    NumOfGames = 0,
                };
                db.Players.Add(player);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, false);
        }
        
        ////[HttpPost]
        //[Route("api/Player/PostNewState")]
        //public HttpResponseMessage PostNewState()
        //{
        //    RPSDbContext db = new RPSDbContext();
        //    string newUsername = HttpContext.Current.Request.Params["username"];
        //    string state = HttpContext.Current.Request.Params["state"];
        //    Player p = db.Players.Single(x => x.Username == newUsername);
        //    if (p.isActive == true)
        //    {
        //        if (state == "False")
        //            p.isActive = false;
        //        else
        //            return Request.CreateResponse(HttpStatusCode.NotModified, "Player state unchanged");
        //    }
        //    else 
        //    {
        //        if (state == "True")
        //            p.isActive = true;
        //        else
        //            return Request.CreateResponse(HttpStatusCode.NotModified, "Player state unchanged");
        //    }
        //    db.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.OK, "Player state changed");
        //}

        [Route("api/Player/GetFriend/{username}")]
        // GET api/<controller>/5
        public HttpResponseMessage GetFriend(string username)
        {
            RPSDbContext db = new RPSDbContext();
            Player p = db.Players.SingleOrDefault(x => x.Username == username);
            if (p == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        //[Route("api/Player/GetPlayerState/{username}/{playerNum}")]
        //// GET api/<controller>/5
        //public HttpResponseMessage GetPlayerState(string username, int playerNum)
        //{
        //    RPSDbContext db = new RPSDbContext();
        //    Room room;
        //    bool state;
        //    if (playerNum == 1)
        //    {
        //        room = db.Rooms.OrderByDescending(x => x.RoomId).First(x => x.Player1 == username);
        //        state = db.Players.Single(x => x.Username == room.Player2).isActive;
        //        if(state == false)
        //            return Request.CreateResponse(HttpStatusCode.OK, "Other Player went inactive");
        //    }
        //    else
        //    {
        //        room = db.Rooms.OrderByDescending(x => x.RoomId).First(x => x.Player2 == username);
        //        state = db.Players.Single(x => x.Username == room.Player1).isActive;
        //        if (state == false)
        //            return Request.CreateResponse(HttpStatusCode.OK, "Other Player went inactive");
        //    }
        //    return Request.CreateResponse(HttpStatusCode.NotModified, "Player is active");
        //}
    }
}