using System.Net;
using System.Net.Sockets;
using System.Text;
using HL7.Dotnetcore;

TcpClient ourTcpClient = null;
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
    var messageToTransmit =
        "MSH|^~\\&|EPIC|SIISCLIENT818^LINDAS TEST ORGANIZATION|^SIIS||20150202115044||VXU^V04^VXU_V04|225|P|2.5.1||||AL|\n" +
        "PID|1||E46700^^^^MR^||DOE^JOHN^C^JR^^^L|SMITH|20140515|M|SMITH^JOHN|2106-3^WHITE^HL70005|115 MAINSTREET^^GOODTOWN^KY^42010^USA^L^010||^PRN^PH^^^270^6009800||EN^ENGLISH^HL70296||||523968712|||2186-5^NOT HISPANIC OR LATINO^HL70012||||||||N|\n" +
        "PD1|||||||||||02^Reminder/recall-any method^HL70215|||||A^Active^HL70441|20150202^20150202 NK1|1|DOE^MARY|MTH^MOTHER^HL70063|\n" +
        "ORC|RE||9645^SIISCLIENT818||||||20150202111146|2001^ARVEY^MARVIN^K|RXA|0|1|20150202|20150202|20^DTaP^CVX^90700^DTAP^CPT|.5|ML^mL^ISO+ ||00^New immunization record^NIP001|JONES^MARK|^^^SIISCLIENT818||||A7894-2|20161115|PMC^SANOFI PASTEUR^MVX||||A RXR|ID^INTRADERMAL^HL70162|LD^LEFT\n" +
        "OBX|1|CE|64994-7^VACCINE FUNDING PROGRAM ELIGIBILITY CATEGORY^LN|1| V02^MEDICAID^HL70064||||||F|||20150202|||VXC40^ELIGIBILITY CAPTURED AT THE IMMUNIZATION LEVEL^CDCPHINVS\n" +
        "OBX|2|CE|30956-7^VACCINE TYPE^LN|2|88^FLU^CVX||||||F|||20150202102525 OBX|3|TS|29768-9^Date vaccine information statement published^LN|2|20120702||||||F OBX|4|TS|29769-7^Date vaccine information statement presented^LN|2|20120202||||||F\n" +
        "RXA|0|1|20141215|20141115|141^influenza, SEASONAL 36^CVX^90658^Influenza Split^CPT|999|||01^HISTORICAL INFORMATION – SOURCE UNSPECIFIED^ NIP001||||||||||||A";

    var byteBuffer = Encoding.UTF8.GetBytes(messageToTransmit);

    //send a message through this connection using the IO stream
    networkStream.Write(byteBuffer, 0, byteBuffer.Length);

    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Sending to Server");

    Console.WriteLine(messageToTransmit);
    
    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Sent to Server");


    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Receiving from Server");
    
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

    Console.WriteLine(receivedMessage);
    
    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Received from Server");


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