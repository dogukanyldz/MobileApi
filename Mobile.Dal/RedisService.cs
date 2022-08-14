using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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
        public void Connect() => _ConnectionMultiplexer = ConnectionMultiplexer.Connect("172.17.0.3");


        public IDatabase GetDb() => _ConnectionMultiplexer.GetDatabase();


    }
    //public class ConnectionFactory
    //{
    //    private Lazy<ConnectionMultiplexer> _cnn1 { get; set; }
    //    public ConnectionFactory(string cnn1)
    //    {
    //        var options = ConfigurationOptions.Parse(cnn1); // host1:port1, host2:port2, ...
    //        options.Password = "0302199762Dd!";
    //        options.Ssl = false;
    //        options.AbortOnConnectFail = false;
    //        options.SslProtocols = SslProtocols.Tls12;
    //        _cnn1 = new Lazy<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
    //    }

    //    public IDatabase GetConnection()
    //    {
    //        return _cnn1.Value.GetDatabase();
    //    }
    //}



    public class ConnectionFactory
    {
        private Lazy<ConnectionMultiplexer> _cnn1 { get; set; }
        private ConnectionMultiplexer _ConnectionMultiplexer;
        public ConnectionFactory(string cnn1)
        {
            _ConnectionMultiplexer = ConnectionMultiplexer.Connect(cnn1);
        }

        public IDatabase GetConnection()
        {
            return _ConnectionMultiplexer.GetDatabase();
        }
    }
}
