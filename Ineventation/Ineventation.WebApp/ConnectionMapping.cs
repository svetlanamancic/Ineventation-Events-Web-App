using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ineventation.WebApp
{
    public class ConnectionMapping
    {
        public Dictionary<string, string> Connections { get; set; }

        private static object locker = true;
        private static ConnectionMapping _instance = null;
        public static ConnectionMapping Instance
        {
            get
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ConnectionMapping();
                        return _instance;
                    }
                    return _instance;
                }
            }
        }

        private ConnectionMapping()
        {
            Connections = new Dictionary<string, string>();
        }
    }
}