using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UT3_Cross_Platform
{
    class Program
    {
        const int LISTENPORT=14001;

        static void Main(string[] args)
        {
            // Create a new socket
            Socket sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            
            // Create a new IP Endpoint on the port defined by LISTENPORT
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, LISTENPORT);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            // Wait for packet and output the data
            byte[] data = new byte[1024];
            int recv = sock.ReceiveFrom(data, ref ep);

            // If the packet is from a PS3 then switch the platform identifier to 0501 (PC)
            // FIXME - Find a way to identify PS3 NICs based on the OUI (first 24 bits of the MAC address)
            if (ep.ToString() == "10.0.0.112:14001")
            {
                if(data[0] == 6 && data[1] == 8)
                {
                    data[0] = 5;
                    data[1] = 1;
                }
            }

            // If the packet is from a PC then switch the platform identifier to 0608 (PS3)
            // FIXME - Find a way to identify PS3 NICs based on the OUI (first 24 bits of the MAC address)
            if (ep.ToString() == "10.0.0.109:14001")
            {
                if (data[0] == 5 && data[1] == 1)
                {
                    data[0] = 6;
                    data[1] = 8;
                }
            }

            // Convert data to readable hexadecimal for now
            string hex = BitConverter.ToString(data);
            hex = hex.Replace("-", " ");

            //string stringData = Encoding.ASCII.GetString(data, 0, recv);
            Console.WriteLine("received: {0} from {1}", 
                hex, ep.ToString());
            
            Console.WriteLine("\nPress any key to exit...");
            Console.Read();

            // Close the socket
            sock.Close();
        }
    }
}
