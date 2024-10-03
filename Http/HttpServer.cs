using Http.HTTP;
using Http.Routing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Http
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;
        private readonly RoutingTable routingTable;
        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            serverListener=new TcpListener(this.ipAddress,port);
            routingTableConfiguration(this.routingTable=new RoutingTable());
        }

        public HttpServer(int port,Action<IRoutingTable> routingTable):this("127.0.0.1",port,routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable):this(8080,routingTable) { }

        public async Task Start()
        {
            serverListener.Start();

            Console.WriteLine("Bachkam Brat");

            while (true)
            {
                var connection = await serverListener.AcceptTcpClientAsync();
                _ = Task.Run(async () =>
                {
                    var networkStream = connection.GetStream();
                    var RequestText = await this.ReadRequest(networkStream);

                    Console.WriteLine(RequestText);

                    var request = Request.Parse(RequestText);
                    var response = this.routingTable.MatchRequest(request);

                    
                    AddSession(request,response);

                    await WriteResponse(networkStream, response);
                    connection.Close();
                });
               
            }

        }

        private async Task WriteResponse(NetworkStream networkStream, Response response)
        {
            var responseBytes=Encoding.UTF8.GetBytes(response.ToString());

            await networkStream.WriteAsync(responseBytes);
        }

     
        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            byte[] buffer=new byte[1024];
            StringBuilder Request=new StringBuilder();
            int totalBytes = 0;
            do
            {
               int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                totalBytes += bytesRead;

                if (totalBytes>10*1024)
                {
                    throw new InvalidOperationException("Request is too large");
                }

                Request.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            } while (networkStream.DataAvailable);

            return Request.ToString();
        }

        private static void AddSession(Request Request, Response response)
        {
            var sessionExists = Request.Session
                .ContainsKey(Session.SessionCurrentDateKey);
            if (!sessionExists) 
            {
                Request.Session[Session.SessionCurrentDateKey]=DateTime.Now.ToString();
                response.Cookies
                    .Add(Session.SessionCookieName, Request.Session.Id);
            }
        }
    }
}
