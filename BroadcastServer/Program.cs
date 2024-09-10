using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BroadcastServer
{
    class Program
    {
        static ConcurrentDictionary<WebSocket, Task> connectedClients = new ConcurrentDictionary<WebSocket, Task>();

        static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "start")
            {
                await StartServer();
            }
            else if (args.Length > 0 && args[0] == "connect")
            {
                await ConnectClient();
            }
            else
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  broadcast-server start   - Start the broadcast server");
                Console.WriteLine("  broadcast-server connect - Connect to the server as a client");
            }
        }

        // Starts the WebSocket server
        static async Task StartServer()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Server started on ws://localhost:5000");
            
            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = wsContext.WebSocket;

                    Console.WriteLine("Client connected.");
                    connectedClients.TryAdd(webSocket, HandleClient(webSocket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        // Handle the client connection
        static async Task HandleClient(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Message received: {message}");

                // Broadcast the message to all connected clients
                foreach (var client in connectedClients.Keys)
                {
                    if (client != webSocket)
                    {
                        await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            connectedClients.TryRemove(webSocket, out _);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine("Client disconnected.");
        }

        // Connect to the server as a client
        static async Task ConnectClient()
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            Console.WriteLine("Connected to server.");

            Task receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Broadcast: {message}");
                }
            });

            while (webSocket.State == WebSocketState.Open)
            {
                Console.Write("Enter message: ");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                if (message == "exit")
                {
                    break;
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
            await receiveTask;
            Console.WriteLine("Disconnected from server.");
        }
    }
}
