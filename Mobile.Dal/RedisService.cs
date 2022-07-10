using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
namespace Mobile.Dal
{
  public  class RedisService
    {
        private string _host { get; set; }
        private int _port { get; set; }
        
        private ConnectionMultiplexer _ConnectionMultiplexer;


        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }
        public void Connect() => _ConnectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");


        public IDatabase GetDb() => _ConnectionMultiplexer.GetDatabase();


    }
}
