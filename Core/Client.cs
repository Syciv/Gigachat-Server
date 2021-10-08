using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public string ClientName;
        public Socket connection { get; set; }

        public Client(Socket conn)
        {
            connection = conn;
        }
    }
}
