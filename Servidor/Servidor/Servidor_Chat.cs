using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Servidor
{
    class Servidor_Chat
    {
        /*        
            TcpListener--------> Espera la conexion del Cliente.        
            TcpClient----------> Proporciona la Conexion entre el Servidor y el Cliente.        
            NetworkStream------> Se encarga de enviar mensajes atravez de los sockets.        
        */

        private TcpListener servidor;
        private TcpClient client = new TcpClient();

        private IPEndPoint ipAndPort = new IPEndPoint(IPAddress.Any, 8000);

        private List<Connection> list = new List<Connection>();

        Connection con;


        private struct Connection
        {
            public NetworkStream stream;
            public StreamWriter streamw;
            public StreamReader streamr;
            public string nickname;
        }

        public Servidor_Chat()
        {
            Inicio();
        }

        public void Inicio()
        {
            servidor = new TcpListener(ipAndPort);
            servidor.Start();

            Console.WriteLine("Servidor Encendido");
            while (true)
            {
                client = servidor.AcceptTcpClient();

                con = new Connection();
                //Aqui usamos la estructura de los datos recibidos con ayuda del stream
                con.stream = client.GetStream();
                con.streamr = new StreamReader(con.stream);
                con.streamw = new StreamWriter(con.stream);

                con.nickname = con.streamr.ReadLine();

                list.Add(con);
                Console.WriteLine(con.nickname + " se a conectado");



                Thread t = new Thread(Escuchar_conexion);

                t.Start();
            }
        }

        void Escuchar_conexion()
        {
            Connection hilcon = con;

            do
            {
                try
                {
                    string tmp = hilcon.streamr.ReadLine();
                    Console.WriteLine(hilcon.nickname + ": " + tmp);
                    foreach (Connection c in list)
                    {
                        try
                        {
                            c.streamw.WriteLine(hilcon.nickname + ": " + tmp);
                            c.streamw.Flush();
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                    list.Remove(hilcon);
                    Console.WriteLine(con.nickname + " se a desconectado.");
                    break;
                }
            } while (true);
        }

    }
}
