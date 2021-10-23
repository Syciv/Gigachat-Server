using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GigachatServer.Core;
using GigachatServer.Models;

namespace Server
{
    class ServerObject
    {
        protected List<Client> Clients; // Список подключённых клиентов
        protected Socket sock;
        protected List<Thread> ClientThreads; // Потоки слушания клиетов
        protected Mutex mutexObj = new Mutex();
        protected DataBaseObject DataBase;

        public ServerObject()
        {
            Clients = new List<Client>();
            ClientThreads = new List<Thread>();
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localIP = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 5555);
            sock.Bind(localIP);
            DataBase = new DataBaseObject();
            // Listen();
        }

        // Прослушивание новых подключений
        public void Listen()
        {   
            sock.Listen(1);
            Console.WriteLine("Слушаем...");
            while (true)
            {
                var connection = sock.Accept();
                Console.WriteLine("Подключился");
                Client client = new Client(connection);
                Clients.Add(client);

                // Send_To_All_Clients(new Data { NewClient = new NewClient { ClientName = "Пока так" } });

                Listen_ClientAsync(client);        
            }
        }

        protected async void Listen_ClientAsync(object obj)
        {
            await Task.Run(() => Listen_Client(obj));
        }

        // Прослушивание клиента
        protected void Listen_Client(object obj)
        {
            var client = (Client)obj;
            var connection = client.connection;
            bool connected = true;
            int bufSize = 510 * 1024;
            byte[] bytes = new byte[bufSize];
            while (connected)
            {
                try
                {
                    int len = connection.Receive(bytes);
                    string jsonObj = Encoding.Default.GetString(bytes);

                    // Получаем сообщение из буфера
                    jsonObj = jsonObj.Substring(0, len);
                    Data data = JsonSerializer.Deserialize<Data>(jsonObj);

                    if (data.Message != null)
                    {
                        Message mes = data.Message;
                        Console.WriteLine(String.Format("{0}: {1} ({2})\n", mes.Name, mes.Text, mes.Time));
                        Send_To_Other_Clients(client, data);
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!");
                    }

                    if (data.UserRegistration != null)
                    {
                        UserRegistration userReg = data.UserRegistration;

                        Console.WriteLine(userReg.UserName);
                        //Console.WriteLine(String.Format("{0}: {1}\n", userReg.UserName, userReg.Name));

                        // Критическая секция
                        mutexObj.WaitOne();

                        (int result, string message) = DataBase.AddUser(userReg.UserName, userReg.Password, userReg.Name, userReg.Surname);
                        Console.WriteLine(message);

                        mutexObj.ReleaseMutex();

                        Response response = new Response { Result = result, Message = message, Time = DateTime.Now.ToString("HH:mm") };
                        Data respdata = new Data { Response = response };

                        string respObj = JsonSerializer.Serialize<Data>(respdata);
                        byte[] respbytes = Encoding.Default.GetBytes(respObj);
                        client.connection.Send(respbytes);
                    }

                    if (data.UserAuthentification != null)
                    {
                        UserAuthentification userAuth = data.UserAuthentification;
                        Console.WriteLine(userAuth.UserName + " " + userAuth.Password);

                        mutexObj.WaitOne();

                        (int result, string message) = DataBase.Auntheficate(userAuth.UserName, userAuth.Password);
                        Console.WriteLine(message);

                        mutexObj.ReleaseMutex();

                        Response response = new Response { Result = result, Message = message, Time = DateTime.Now.ToString("HH:mm") };
                        Data respdata = new Data { Response = response };

                        string respObj = JsonSerializer.Serialize<Data>(respdata);
                        byte[] respbytes = Encoding.Default.GetBytes(respObj);
                        client.connection.Send(respbytes);
                    }

                    if(data.User != null)
                    {
                        User user = data.User;

                        Console.WriteLine("Достаём профиль " + user.UserName);
                        mutexObj.WaitOne();

                        (int result, string message, UserProfile userProfile) = DataBase.GetUserProfile(user.UserName);
                        Console.WriteLine(message);

                        mutexObj.ReleaseMutex();

                        Data respdata = new Data { UserProfile = userProfile };
                        string respObj = JsonSerializer.Serialize<Data>(respdata);
                        byte[] respbytes = Encoding.Default.GetBytes(respObj);
                        client.connection.Send(respbytes);
                    }
                   
                    if(data.ProfileImage != null)
                    {
                        ProfileImage profileImage = data.ProfileImage;
                        Console.WriteLine("Меняем фото " + profileImage.UserName);

                        mutexObj.WaitOne();

                        (int result, string message) = DataBase.ChangeUserImage(profileImage.UserName, profileImage.Image);
                        Console.WriteLine(message);

                        mutexObj.ReleaseMutex();

                        Response response = new Response { Result = result, Message = message, Time = DateTime.Now.ToString("HH:mm") };
                        Data respdata = new Data { Response = response };

                        string respObj = JsonSerializer.Serialize<Data>(respdata);
                        byte[] respbytes = Encoding.Default.GetBytes(respObj);
                        client.connection.Send(respbytes);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Соединение разорвано:" + ex.ToString());
                    connected = false;
                    Clients.Remove(client);
                }
            }
        }
        
        // Отправка сообщения всем клиентам
        protected void Send_To_All_Clients(Data data)
        {
            // Message mes = new Message { Text = message };
            string jsonObj = JsonSerializer.Serialize<Data>(data);
            byte[] bytes = Encoding.Default.GetBytes(jsonObj);
            foreach (Client i in Clients)
            {
                i.connection.Send(bytes);
            }
        }

        // Отправка сообщения всем клиентам, кроме заданного
        protected void Send_To_Other_Clients(Client client, Data data)
        {
            // Message mes = new Message { Text = message };
            
            string jsonObj = JsonSerializer.Serialize<Data>(data);
            byte[] bytes = Encoding.Default.GetBytes(jsonObj);
            foreach (Client i in Clients)
            {
                if (i != client)
                {
                    i.connection.Send(bytes);
                }
            }
        }

    }
}
