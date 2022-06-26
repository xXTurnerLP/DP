using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP.Models.Auth
{
    public class Session
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public User Account { get; set; }
    }
}
