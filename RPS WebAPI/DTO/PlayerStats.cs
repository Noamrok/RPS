using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPS_WebAPI.DTO
{
    public class PlayerStats
    {
        public string Username;
        public string Password;
        public string Email;
        public int Wins;
        public int Loses;
        public int NumOfGames;
    }
}