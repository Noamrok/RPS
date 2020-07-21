using RPS_WebAPI.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace RPS_WebAPI.Controllers
{
    public class QueueController : ApiController
    {
        //IHttpActionResult
        [Route("api/Queue/GetReadyForGame/{username}")]
        public HttpResponseMessage GetReadyForGame(string username)
        {
            RPSDbContext db = new RPSDbContext();
            Room room = db.Rooms.OrderByDescending(x => x.RoomId).FirstOrDefault();
            if (room.Player2 == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Second player is not here yet");
            return Request.CreateResponse(HttpStatusCode.OK, "Second player entered my room");
            
        }

        [Route("api/Queue/PostPlayerToQueue")]
        public HttpResponseMessage PostPlayerToQueue()
        {
            RPSDbContext db = new RPSDbContext();
            Room room = db.Rooms.FirstOrDefault(x => x.Player1 != null && x.Player2 == null);
            string username = HttpContext.Current.Request.Params["username"];
            if (room == null)
            {
                Room newRoom = new Room()
                {
                    Player1 = username,
                    Player2 = null
                };
                db.Rooms.Add(newRoom);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.Created, "Player opened a room");
            }
            if (room.Player1 != username)
            {
                room.Player2 = username;
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Player entered as second");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Same player name");
        }
        
        // DELETE api/<controller>/5
        [Route("api/Queue/DeletePlayerFromQueue/{username}")]
        public HttpResponseMessage DeletePlayerFromQueue(string username)
        {
            RPSDbContext db = new RPSDbContext();
            Room playerInQueue = db.Rooms.OrderByDescending(x => x.RoomId).FirstOrDefault(x => x.Player1 == username && x.Player2 == null);
            if (playerInQueue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Player is not in queue");
            db.Rooms.Remove(playerInQueue);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, "Player found and was deleted");
        }
    }
}