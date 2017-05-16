using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;



    class Communication
    {
        Socket S;
        FinalAnswer GamePos;
        public void StartConnectionAsServer(string ipaddress)
        {
            string address = ipaddress;
            int intAddress = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
            string ip2Address = new IPAddress(BitConverter.GetBytes(intAddress)).ToString();
            IPEndPoint localEndPoint = new IPEndPoint(intAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                GamePos.SyncCommGame = false;
             //   UnityEngine.Debug.Log("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                S = handler;
                listener.Close();
           //     UnityEngine.Debug.Log("A5eran Connected...");
                GamePos.SyncCommGame = true;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        }
        public void StartConnectionAsClient(string ipaddress)
        {
            try
            {
                string address = ipaddress;
                int intAddress = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
                string ip2Address = new IPAddress(BitConverter.GetBytes(intAddress)).ToString();
                IPEndPoint remoteEP = new IPEndPoint(intAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  


                sender.Connect(remoteEP);
                UnityEngine.Debug.Log("Socket connected to {0}"+sender.RemoteEndPoint.ToString());
                //Console.WriteLine;





                S = sender;
                GamePos.SyncCommGame = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
        }
        public void send(int x,int y)
        {
            byte[] ack = new byte[1024];


            String data = x.ToString();//GamePos.PlayerPosition[0]; ;//M.GetMoveI().ToString();
            data = data + ",";
            data = data + y.ToString();
            int bytesSent = S.Send(Encoding.ASCII.GetBytes(data + "<EOF>"));

            StreamWriter file = new StreamWriter("log.txt", true);
            file.WriteLine(DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "(" + x+ "," + y + ")" + " sent");
            file.Close();
            int bytesRec = S.Receive(ack);

        }
        public void Receive()
        {
            byte[] bytes = new byte[1024];


            string data = null;

            data += Encoding.ASCII.GetString(bytes, 0, S.Receive(bytes));

            int i = Int32.Parse(data.Substring(0, data.Length - 5).Split(',')[0]);
            int j = Int32.Parse(data.Substring(0, data.Length - 5).Split(',')[1]);
            
            int bytesSend = S.Send(bytes);
            GamePos.PlayerPosition[0] = i.ToString();
            GamePos.PlayerPosition[1] = j.ToString();
           // UnityEngine.Debug.Log("Weslo i = " + GamePos.AgentPosition[0] + "Weslo j = " + GamePos.AgentPosition[1]);
            StreamWriter file = new StreamWriter("log.txt", true);
            file.WriteLine(DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "(" + i + "," + j + ")" + " Received");
            file.Close();
            return ;
        }
        public void closeconnection()
        {
            if (S != null)
                S.Close();
        }

        public  Communication(ref FinalAnswer FN)
        {
            GamePos=new FinalAnswer();
            GamePos = FN;
        }

        public void CommMain(int s)
        {
           // UnityEngine.Debug.Log("S =" + s);
            if (s == 0) //i am Host
            {
                int count = 0;
                GamePos.CommTurn = 1;
                StartConnectionAsServer("192.168.0.1");
                while (true)
                {
                    if (count % 2 == 0)
                    {
                        GamePos.AgentPlayed = false;
                        if (GamePos.PlayerPlayed)
                        {

                            //send();
                            count++;
                            GamePos.CommTurn = 2;
                            GamePos.PlayerPlayed = false;
                        }
                    }
                    else
                    {
                        Receive();
                        GamePos.AgentPlayed = true;
                        System.Threading.Thread.Sleep(35);
                        GamePos.CommTurn = 1;
                        count++;
                    }

                }
            }
            if(s==1) // i will join 
            {
                int count = 1;
                GamePos.CommTurn = 2;
                StartConnectionAsClient("192.168.0.2");
                while (true)
                {
                    if (count % 2 == 0)
                    {
                        GamePos.AgentPlayed = false;
                        if (GamePos.PlayerPlayed)
                        {
                            //send();
                            count++;
                            GamePos.CommTurn = 2;
                            GamePos.PlayerPlayed = false;
                        }
                    }
                    else
                    {
                        Receive();
                        GamePos.AgentPlayed = true;
                        System.Threading.Thread.Sleep(35);
                        GamePos.CommTurn = 3;
                        count++;
                    }

                }
            }

            if (s == 3) //i am Host Agent vs Agent
            {
                int count = 0;
                GamePos.CommTurn = 3;
                StartConnectionAsServer("192.168.0.1");
                while (GamePos.SyncCommGame)
                {
                    if (count % 2 == 0) //My Agent
                    {
                      //  UnityEngine.Debug.Log("Count = " + count);
                    //    UnityEngine.Debug.Log("Door Our Agent");
                        if (GamePos.AgentPlayed)
                        {
                         //   UnityEngine.Debug.Log("Our Agent Statred");
                        //    UnityEngine.Debug.Log("AgentBet3naE5tar = " + GamePos.AgentPosition[0].ToString() + " " + GamePos.AgentPosition[1].ToString());
                          //  UnityEngine.Debug.Log("THeir Agent E5tar = " + GamePos.PlayerPosition[0] + " " + GamePos.PlayerPosition[1]);
                            GamePos.PlayerPlayed = false;
                            //System.Threading.Thread.Sleep(10);
                            send(GamePos.AgentPosition[0], GamePos.AgentPosition[1]);
                            count++;
 
                           // System.Threading.Thread.Sleep(35);

                        }
                    }
                    else
                    {
                        GamePos.AgentPlayed = false;
                        Receive();
                        GamePos.PlayerPlayed = true;
                        count++;
                       // System.Threading.Thread.Sleep(35);
                    }

                }
            }

            if (s == 4) // Join  Agent vs Agent
            {
                int count = 0;
                GamePos.CommTurn = 2;
                StartConnectionAsServer("192.168.0.1");
                while (true)
                {
                    if (count % 2 == 0)
                    {

                        if (GamePos.AgentPlayed && (GamePos.AgentPosition[0].ToString() != GamePos.PlayerPosition[0] && GamePos.AgentPosition[1].ToString() != GamePos.PlayerPosition[1]))
                        {

                            GamePos.PlayerPlayed = false;
                            System.Threading.Thread.Sleep(10);
                            send(GamePos.AgentPosition[0], GamePos.AgentPosition[1]);
                            count++;
                            GamePos.CommTurn = 2;
                            GamePos.AgentPlayed = false;
                        }
                    }
                    else
                    {
                        Receive();
                        GamePos.PlayerPlayed = true;
                        System.Threading.Thread.Sleep(35);
                        GamePos.CommTurn = 3;
                        count++;
                    }

                }
            }

                
        }
  
    
    
    
    }


