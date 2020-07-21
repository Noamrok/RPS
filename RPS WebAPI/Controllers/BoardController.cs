using RPS_WebAPI.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace RPS_WebAPI.Controllers
{
    public class BoardController : ApiController
    {
        // GET api/<controller>
        [Route("api/Board/GetLastMove/{username}/{playerNum}")]
        public string[] GetLastMove(string username, int playerNum)
        {
            RPSDbContext db = new RPSDbContext();
            Room room;
            string[] lastMove = { "" };
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                if (room.Player2LastMove == null)
                    return lastMove;
                lastMove = room.Player2LastMove.Split(',');
                room.Player2LastMove = null;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                if (room.Player1LastMove == null)
                    return lastMove;
                lastMove = room.Player1LastMove.Split(',');
                room.Player1LastMove = null;
            }
            db.SaveChanges();
            return lastMove;
        }

        [Route("api/Board/GetWinner/{username}/{playerNum}")]
        public string GetWinner(string username, int playerNum)
        {
            RPSDbContext db = new RPSDbContext();
            Room room;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                return room.Player2;
            }
            room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
            return room.Player1;
        }

        [Route("api/Board/GetInitPawnsLoc/{username}/{playerNum}")]
        public string[] GetInitPawnsLoc(string username, int playerNum)
        {
            RPSDbContext db = new RPSDbContext();
            Room room;
            string[] pawnsLoc;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                while(room.Player2InitPawns == null)
                {
                    Thread.Sleep(200);
                    db = new RPSDbContext();
                    room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                }
                pawnsLoc = room.Player2InitPawns.Split(',');
                room.Player2InitPawns = null;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                while (room.Player1InitPawns == null)
                {
                    Thread.Sleep(200);
                    db = new RPSDbContext();
                    room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                }
                pawnsLoc = room.Player1InitPawns.Split(',');
                room.Player1InitPawns = null;
            }
            db.SaveChanges();
            return pawnsLoc;
        }

        [Route("api/Board/GetDrawChoice/{username}/{playerNum}")]
        public string GetDrawChoice(string username, int playerNum)
        {
            RPSDbContext db = new RPSDbContext();
            Room room;
            string choice;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                while (room.Player2Choice == null)
                {
                    Thread.Sleep(200);
                    db = new RPSDbContext();
                    room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                }
                choice = room.Player2Choice;
                room.Player2Choice = null;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                while (room.Player1Choice == null)
                {
                    Thread.Sleep(200);
                    db = new RPSDbContext();
                    room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                }
                choice = room.Player1Choice;
                room.Player1Choice = null;
            }
            db.SaveChanges();
            return choice;
        }

        //[HttpPost]
        // POST api/<controller>/5
        [Route("api/Board/PostNewMove")]
        public HttpResponseMessage PostNewMove()
        {
            RPSDbContext db = new RPSDbContext();
            string move = HttpContext.Current.Request.Params["lastMove"];
            string username = HttpContext.Current.Request.Params["username"];
            int playerNum = Convert.ToInt32(HttpContext.Current.Request.Params["playerNum"]);
            Room room;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                room.Player1LastMove = move;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                room.Player2LastMove = move;
            }
            return db.SaveChanges() > 0 ? Request.CreateResponse(HttpStatusCode.OK, true) : Request.CreateResponse(HttpStatusCode.BadRequest, false);
        }

        [Route("api/Board/PostInitPawns")]
        public HttpResponseMessage PostInitPawns()
        {
            string pawnsLocNew = HttpContext.Current.Request.Params["initPawnsLoc"];
            string username = HttpContext.Current.Request.Params["username"];
            int playerNum = Convert.ToInt32(HttpContext.Current.Request.Params["playerNum"]);
            RPSDbContext db = new RPSDbContext();
            Room room;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                room.Player1InitPawns = pawnsLocNew;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                room.Player2InitPawns = pawnsLocNew;
            }
            return db.SaveChanges() > 0 ? Request.CreateResponse(HttpStatusCode.OK, true) : Request.CreateResponse(HttpStatusCode.BadRequest, false);
        }

        [Route("api/Board/PostDrawChoice")]
        public HttpResponseMessage PostDrawChoice()
        {
            string choice = HttpContext.Current.Request.Params["choice"];
            string username = HttpContext.Current.Request.Params["username"];
            int playerNum = Convert.ToInt32(HttpContext.Current.Request.Params["playerNum"]);
            RPSDbContext db = new RPSDbContext();
            Room room;
            if (playerNum == 1)
            {
                room = db.Rooms.Where(x => x.Player1 == username).OrderByDescending(x => x.RoomId).First();
                room.Player1Choice = choice;
            }
            else
            {
                room = db.Rooms.Where(x => x.Player2 == username).OrderByDescending(x => x.RoomId).First();
                room.Player2Choice = choice;
            }
            return db.SaveChanges() > 0 ? Request.CreateResponse(HttpStatusCode.OK, true) : Request.CreateResponse(HttpStatusCode.BadRequest, false);
        }
    }
}