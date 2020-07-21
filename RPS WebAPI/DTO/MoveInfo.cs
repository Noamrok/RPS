using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPS_WebAPI.DTO
{
    public class MoveInfo
    {
        public int RoomId;
        public string Player;
        public int? MoveIndx;
        public int SourceX;
        public int SourceY;
        public int DesX;
        public int DesY;
    }
}