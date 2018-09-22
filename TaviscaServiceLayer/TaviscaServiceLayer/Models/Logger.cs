using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaviscaServiceLayer.Models
{
    public class Logger
    {
        private static Logger logInstance = null;
        
        
        private Logger()
        {
            
        }
        public static Logger getInstance()
        {
            if (logInstance == null)
                logInstance = new Logger();

            return logInstance;
        }
        public void LogMessage(String messages)
        {
            try
            {
                var cluster = Cluster.Builder()
                    .AddContactPoints("127.0.0.1")
                    .Build();
                var session = cluster.Connect("hotels");
                var result = session.Execute("insert into logdetails(logid,comment,logtime) values(uuid(),'" + messages + "',dateof(now()))");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}