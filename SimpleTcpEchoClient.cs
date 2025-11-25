using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClient
    {
        public class SimpleTcpEchoClient
        {
            public static void Main(string[] args)
            {
                System.Net.Sockets.TcpClient ourTcpClient = null;
                NetworkStream networkStream = null;

                try
                {
                    //initiate a TCP client connection to local loopback address at port 1080
                    ourTcpClient = new System.Net.Sockets.TcpClient();

                    ourTcpClient.Connect(new IPEndPoint(IPAddress.Loopback, 1080));

                    Console.WriteLine("Connected to server....");

                    //get the IO stream on this connection to write to
                    networkStream = ourTcpClient.GetStream();

                    //use UTF-8 and either 8-bit encoding due to MLLP-related recommendations
                    var messageToTransmit = "This was sent from the client - Hello at " + DateTime.Now.ToString("o");
                    var byteBuffer = Encoding.UTF8.GetBytes(messageToTransmit);

                    //send a message through this connection using the IO stream
                    networkStream.Write(byteBuffer, 0, byteBuffer.Length);

                    Console.WriteLine("Sent: '{0}' to the server", messageToTransmit);

                    var bytesReceivedFromServer = networkStream.Read(byteBuffer, 0, byteBuffer.Length);

                    // Our server for this example has been designed to echo back the message
                    // keep reading from this stream until the message is echoed back
                    while (bytesReceivedFromServer < byteBuffer.Length)
                    {
                        bytesReceivedFromServer = networkStream.Read(byteBuffer, 0, byteBuffer.Length);
                        if (bytesReceivedFromServer == 0)
                        {
                            //exit the reading loop since there is no more data
                            break;
                        }
                    }
                    var receivedMessage = Encoding.UTF8.GetString(byteBuffer);

                    Console.WriteLine("Received: '{0}' from the server", receivedMessage);

                    Console.WriteLine("Press any key to exit program...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    //display any exceptions that occur to console
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    //close the IO strem and the TCP connection
                    networkStream?.Close();
                    ourTcpClient?.Close();
                }
            }
        }
    }