using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntermediateServer
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;

            //关闭对文本框的非线程操作检查  
            TextBox.CheckForIllegalCrossThreadCalls = false;
            txtIP.Text = "127.0.0.1";
            txtPort.Text = "5566";
        }
        string RemoteEndPoint;     //客户端的网络结点  
        string InterlockingIDs = "";

        Thread threadwatch = null;//负责监听客户端的线程  
        Socket socketwatch = null;//负责监听客户端的套接字  
        //创建一个和客户端通信的套接字  
        Dictionary<string, Socket> dic = new Dictionary<string, Socket> { };   //定义一个集合，存储客户端信息  
        private void btnStartUp_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPort.Text) && !string.IsNullOrEmpty(txtIP.Text))
            {

                this.btnStartUp.Enabled = false;
                //定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）  
                socketwatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //服务端发送信息需要一个IP地址和端口号  
                IPAddress address = IPAddress.Parse(txtIP.Text.Trim());//获取文本框输入的IP地址  
                //将IP地址和端口号绑定到网络节点point上  
                IPEndPoint point = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));//获取文本框上输入的端口号  
                                                                                           //此端口专门用来监听的  
                                                                                           //监听绑定的网络节点  
                socketwatch.Bind(point);
                //将套接字的监听队列长度限制为20  
                socketwatch.Listen(20);
                //创建一个监听线程  
                threadwatch = new Thread(watchconnecting);
                //将窗体线程设置为与后台同步，随着主线程结束而结束  
                threadwatch.IsBackground = true;
                //启动线程     
                threadwatch.Start();
                //启动线程后 textBox3文本框显示相应提示  
                txtInfo.AppendText("开始监听客户端传来的信息!" + "\r\n");
            }
            else
            {
                MessageBox.Show("请务必填写端口号和IP地址");
            }
        }
        void OnlineList_Disp(string Info)
        {
            listBoxOnlineList.Items.Add(Info);   //在线列表中显示连接的客户端套接字  
        }
        //监听客户端发来的请求  
        private void watchconnecting()
        {
            Socket connection = null;
            while (true)  //持续不断监听客户端发来的请求     
            {
                try
                {
                    connection = socketwatch.Accept();
                }
                catch (Exception ex)
                {
                    txtInfo.AppendText(ex.Message); //提示套接字监听异常     
                    break;
                }
                //获取客户端的IP和端口号  
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                //让客户显示"连接成功的"的信息  
                string sendmsg = "连接服务端成功！\r\n" + "本地IP:" + clientIP + "，本地端口" + clientPort.ToString();
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);


                RemoteEndPoint = connection.RemoteEndPoint.ToString(); //客户端网络结点号  
                txtInfo.AppendText("成功与" + RemoteEndPoint + "客户端建立连接！\t\n");     //显示与客户端连接情况  
                dic.Add(RemoteEndPoint, connection);    //添加客户端信息  

                OnlineList_Disp(RemoteEndPoint);    //显示在线客户端  


                //IPEndPoint netpoint = new IPEndPoint(clientIP,clientPort);  

                IPEndPoint netpoint = connection.RemoteEndPoint as IPEndPoint;

                //创建一个通信线程      
                ParameterizedThreadStart pts = new ParameterizedThreadStart(recv);
                Thread thread = new Thread(pts);
                thread.IsBackground = true;//设置为后台线程，随着主线程退出而退出     
                //启动线程     
                thread.Start(connection);
            }
        }
        ///     
        /// 接收客户端发来的信息      
        ///     
        ///客户端套接字对象    
        private void recv(object socketclientpara)
        {
            //txtInfo.AppendText("客户端1");

            Socket socketServer = socketclientpara as Socket;
            //txtInfo.AppendText("客户端2");
            string ip = RemoteEndPoint;
            //txtInfo.AppendText("客户端3");
            while (true)
            {
                //txtInfo.AppendText("客户端4");
                if (socketServer.Poll(10, SelectMode.SelectRead) == false)
                {
                    //txtInfo.AppendText("客户端5");
                    //创建一个内存缓冲区 其大小为1024*1024字节  即1M     
                    byte[] bytes = new byte[1024 * 1024];
                    //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度    
                    try
                    {
                        int length = socketServer.Receive(bytes);

                        //将机器接受到的字节数组转换为人可以读懂的字符串     
                        string strSRecMsg = ByteQueue.ChangBytesToStr(bytes, length);

                        //将发送的字符串信息附加到文本框txtMsg上     
                        txtInfo.AppendText("客户端:" + socketServer.RemoteEndPoint + ",time:" + GetCurrentTime() + "\r\n" + strSRecMsg + "\r\n\n");

                        DataOut(bytes, ip);
                        //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                        //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        txtInfo.AppendText("已断开连接");
                        socketwatch.Listen(20);
                        Socket connection = socketwatch.Accept();
                        //Console.WriteLine(ex.ToString());
                        //txtInfo.AppendText("客户端" + socketServer.RemoteEndPoint + "已经中断连接" + "\r\n"); //提示套接字监听异常   
                        //listBoxOnlineList.Items.Remove(socketServer.RemoteEndPoint.ToString());//从listbox中移除断开连接的客户端 
                        //socketServer.Shutdown(SocketShutdown.Both);
                        //socketServer.Close();//关闭之前accept出来的和客户端进行通信的套接字  
                        break;
                    }
                }
                else
                {
                    txtInfo.AppendText("已断开连接");
                    socketwatch.Listen(20);
                    Socket connection = socketwatch.Accept();
                }
            }
        }

        private async void DataOut(byte[] bytes, string RemoteEndPoint)
        {


            if (String.Format("0x{0:x2}", bytes[2]) == "0x80")
            {
                txtInfo.AppendText("发送功能码\r\n");
                if (String.Format("0x{0:x2}", bytes[1]) == "0x01")
                {
                    txtInfo.AppendText("进站出站\r\n");

                    Info Model = getAnalysisData(bytes);

                    #region 进站出站功能实现
                    //后台client方式GET提交
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://testweb154.usmeew.com/api/BikeInfo/Update/";
                    myHttpClient.BaseAddress = new Uri(url);
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {
                                                        {"InterlockingID",Model.InterlockingID}, //设备ID
                                                        {"EntryExitState",Model.EntryExitState.ToString()},  //出站状态
                                                        {"UpdateState","1"},//执行方法类型
                                                        {"PhysicalNumbering",Model.PhysicalNumbering}
                                                    });
                        try
                        {
                            //await异步等待回应  
                            var response = await http.PutAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            //Console.WriteLine(await response.Content.ReadAsStringAsync());
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            txtInfo.AppendText(e.Message.ToString() + "\r\n");
                        }

                    }
                    #endregion

                    #region 应答
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x01,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };
                    //string selectClient = RemoteEndPoint;  //选择要发送的客户端  
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x02")
                {
                    txtInfo.AppendText("供电方式\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    #region 供电方式功能实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi      
                    string url = "http://testweb154.usmeew.com/api/BikeInfo/Update/";
                    myHttpClient.BaseAddress = new Uri(url);
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {
                                                        {"InterlockingID",info.InterlockingID}, //设备ID
                                                        {"SolarPoweredState",info.PoweredByState.ToString()}, //供电状态
                                                        {"UpdateState","2"} //执行方法类型
                                                    });
                        try
                        {
                            //await异步等待回应  
                            var response = await http.PutAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("E:" + e.Message.ToString());
                            txtInfo.AppendText("E:" + e.Message.ToString() + "\r\n");
                        }
                    }
                    #endregion

                    #region 应答
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x02,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x03")
                {
                    txtInfo.AppendText("锁开关\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);
                    #region 锁开关功能实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://testweb154.usmeew.com/api/BikeInfo/Update/";
                    myHttpClient.BaseAddress = new Uri(url);

                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {    {"InterlockingID",info.InterlockingID},//设备ID
                                                         {"UnlockState",info.UnlockState.ToString()},//锁开关状态
                                                         {"UpdateState","3"}//执行方法类型
                                                    });
                        try
                        {
                            //await异步等待回应  
                            var response = await http.PutAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            //Console.WriteLine(await response.Content.ReadAsStringAsync());
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("F11:" + e.Message.ToString());
                            txtInfo.AppendText("F11:" + e.Message.ToString() + "\r\n");
                            if (e.Message.ToString().Contains("502")) //502重新发送一次
                            {
                                #region 锁开关功能实现
                                HttpClient myHttpClients = new HttpClient();
                                //提交当前地址的webapi
                                string urls = "http://testweb154.usmeew.com/api/BikeInfo/Update/";
                                myHttpClients.BaseAddress = new Uri(url);

                                var handlers = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                                using (var https = new HttpClient(handlers))
                                {
                                    //使用FormUrlEncodedContent做HttpContent  
                                    var contents = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                                {    {"InterlockingID",info.InterlockingID},//设备ID
                                                                     {"UnlockState",info.UnlockState.ToString()},//锁开关状态
                                                                     {"UpdateState","3"}//执行方法类型
                                                                });
                                    try
                                    {
                                        //await异步等待回应  
                                        var response = await https.PutAsync(urls, contents);
                                        //确保HTTP成功状态值  
                                        response.EnsureSuccessStatusCode();
                                        //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                        //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                        txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                                    }
                                    catch (Exception ex)
                                    {
                                        //Console.WriteLine("F00:" + ex.Message.ToString());
                                        txtInfo.AppendText("F00:" + ex.Message.ToString() + "\r\n");
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region 应答

                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x03,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x04")
                {
                    txtInfo.AppendText("读卡识别\r\n");

                    // 获取数据
                    Info info = getAnalysisData(bytes);

                    #region 读卡识别功能实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://testweb154.usmeew.com/api/UserInfo/ScanCode";
                    myHttpClient.BaseAddress = new Uri(url);
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {    {"InterlockingID",info.InterlockingID}, //设备ID
                                                         {"RFCardNumber",info.CardNumber}, //卡号
                                                         {"UnlockMode","3"}//开锁方式 
                                                    });

                        try
                        {
                            //await异步等待回应  
                            var response = await http.PostAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            //Console.WriteLine(await response.Content.ReadAsStringAsync());
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("G213:" + e.Message.ToString());
                            txtInfo.AppendText("G213:" + e.Message.ToString() + "\r\n");
                            if (e.Message.ToString().Contains("502"))
                            {
                                #region 读卡识别功能实现
                                HttpClient myHttpClients = new HttpClient();
                                //提交当前地址的webapi
                                string urls = "http://testweb154.usmeew.com/api/UserInfo/ScanCode";
                                myHttpClients.BaseAddress = new Uri(url);
                                var handlers = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                                using (var https = new HttpClient(handlers))
                                {
                                    //使用FormUrlEncodedContent做HttpContent  
                                    var contents = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                                {
                                                                    {"InterlockingID",info.InterlockingID}, //设备ID
                                                                    {"RFCardNumber",info.CardNumber}, //卡号
                                                                    {"UnlockMode","3"}//开锁方式 
                                                                });
                                    try
                                    {
                                        //await异步等待回应  
                                        var response = await https.PostAsync(urls, contents);
                                        //确保HTTP成功状态值  
                                        response.EnsureSuccessStatusCode();
                                        //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                        //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                        txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                                    }
                                    catch (Exception ex)
                                    {
                                        //Console.WriteLine("G666666:" + ex.Message.ToString());
                                        txtInfo.AppendText("G666666:" + ex.Message.ToString() + "\r\n");
                                    }

                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region 应答
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x04,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion
                }
                //定位的时候获取设备id
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x05")
                {
                    txtInfo.AppendText("位置定位\r\n");

                    //获取数据
                    Info info = getLocationData(bytes);
                    InterlockingIDs = info.InterlockingID;

                    #region 位置定位功能实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://testweb154.usmeew.com/api/BikeInfo/Update/";
                    myHttpClient.BaseAddress = new Uri(url);

                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                    {
                                                        {"InterlockingID",info.InterlockingID},  //设备ID
                                                        {"PowerValue",info.PowerValue.ToString() }, //电量值
                                                        {"Longitude",info.Longitude }, //经度
                                                        {"Dimension",info.Dimension }, //纬度
                                                        {"PositioningAccuracy",info.PositioningAccuracy.ToString()}, //定位经度
                                                        {"UnlockState",info.UnlockState.ToString() }, //开锁状态
                                                        {"LastUnlockedTime",info.LastUnlockedTime.ToString() }, //上次开锁时常
                                                        {"EntryExitState",info.EntryExitState.ToString() }, //进出站状态
                                                        {"PhysicalNumbering",info.PhysicalNumbering }, //基站编号
                                                        {"UpdateState","5"}//执行方法类型
                                                    });
                        //await异步等待回应  
                        try
                        {
                            var response = await http.PutAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            //Console.WriteLine(await response.Content.ReadAsStringAsync());
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("H:" + e.Message.ToString());
                            txtInfo.AppendText("H:" + e.Message.ToString() + "\r\n");
                        }
                    }
                    #endregion

                    #region 应答
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x05,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x06")
                {
                    txtInfo.AppendText("电量警报\r\n");

                    //获取数据
                    Info info = getAnalysisData(bytes);

                    //暂时没有操作

                    #region 应答
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x06,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    #endregion

                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x07")
                {
                    txtInfo.AppendText("异常或暴力开启上报\r\n");
                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 2;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x07,0x00,
                                                    Number,
                                                    0x02,0x00,
                                                    0x00,0x00,
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x08")
                {
                    txtInfo.AppendText("请求任务命令\r\n");

                    //暂无操作

                    byte Number = (byte)bytes[3];
                    int responseVerify2 = 13;
                    responseVerify2 = ~responseVerify2;
                    byte responseVerify3 = (byte)responseVerify2;
                    byte[] dataserDown = new byte[] {
                                                    0xAA,
                                                    0x08,0x00,
                                                    Number,
                                                    0x13,0x00,
                                                    0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,//年月日时分秒
                                                    0x00,//待发命令条数
                                                    0x00,0x00,0x00,0x00,//待用
                                                    responseVerify3, //校验和
                                                    0x55  //数据帧尾
                                                    };

                    //string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    //dic[selectClient].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                    dic[RemoteEndPoint].Send(dataserDown, dataserDown.Length, SocketFlags.None);
                }
                else
                {
                    txtInfo.AppendText("命令出错\r\n");
                }
            }
            else if (String.Format("0x{0:x2}", bytes[2]) == "0x88")
            {
                txtInfo.AppendText("应答功能码\r\n");
                if (String.Format("0x{0:x2}", bytes[1]) == "0x01")
                {
                    txtInfo.AppendText("还车完成\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    #region 还车完成实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://testweb154.usmeew.com/api/Instruction/UpdateInstruction/";
                    myHttpClient.BaseAddress = new Uri(url);
                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        if (info.ResultCode == 0)
                        {
                            //使用FormUrlEncodedContent做HttpContent  
                            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                        {    {"InterlockingID",info.InterlockingID},//设备ID
                                                             {"IsSucceed","0"},     //识别成功
                                                             {"InstructionType","1"}//执行类型    
                                                        });
                            try
                            {
                                //await异步等待回应  
                                var response = await http.PutAsync(url, content);
                                //确保HTTP成功状态值  
                                response.EnsureSuccessStatusCode();
                                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("I:" + e.Message.ToString());
                                txtInfo.AppendText("I:" + e.Message.ToString() + "\r\n");
                            }

                        }
                        else if (info.ResultCode == 1)
                        {
                            //使用FormUrlEncodedContent做HttpContent  
                            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                        {
                                                            { "InterlockingID",info.InterlockingID}, //设备ID
                                                            {"IsSucceed","1"},    //识别失败
                                                            {"InstructionType","1"}//执行类型  
                                                        });
                            try
                            {
                                //await异步等待回应  
                                var response = await http.PutAsync(url, content);
                                //确保HTTP成功状态值  
                                response.EnsureSuccessStatusCode();
                                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("J:" + e.Message.ToString());
                                txtInfo.AppendText("J:" + e.Message.ToString() + "\r\n");
                            }
                        }
                    }
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x02")
                {
                    txtInfo.AppendText("开锁命令\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    #region 开锁命令使用
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://172.17.217.63/api/Instruction/UpdateInstruction/";
                    myHttpClient.BaseAddress = new Uri(url);

                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        if (info.ResultCode == 0)
                        {
                            //使用FormUrlEncodedContent做HttpContent  
                            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                        {    {"InterlockingID",info.InterlockingID}, //设备ID
                                                                {"IsSucceed","0"},     //识别成功
                                                                {"InstructionType","2"}//键名必须为空  
                                                        });
                            try
                            {
                                //await异步等待回应  
                                var response = await http.PutAsync(url, content);
                                //确保HTTP成功状态值  
                                response.EnsureSuccessStatusCode();
                                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine("K:" + e.Message.ToString());
                                txtInfo.AppendText("K:" + e.Message.ToString() + "\r\n");
                            }
                        }
                        else if (info.ResultCode == 1)
                        {
                            //使用FormUrlEncodedContent做HttpContent  
                            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                                                        {
                                                            { "InterlockingID",info.InterlockingID}, //设备ID
                                                            {"IsSucceed","1"},     //识别失败
                                                            {"InstructionType","2"}//键名必须为空  
                                                        });
                            try
                            {
                                //await异步等待回应  
                                var response = await http.PutAsync(url, content);
                                //确保HTTP成功状态值  
                                response.EnsureSuccessStatusCode();
                                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                                txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("M:" + e.Message.ToString());
                                txtInfo.AppendText("M:" + e.Message.ToString() + "\r\n");
                            }
                        }
                    }
                    #endregion

                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x03")
                {
                    txtInfo.AppendText("GPS开关指令\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    //暂无操作

                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x04")
                {
                    txtInfo.AppendText("声音控制\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    //暂无操作

                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x05")
                {
                    txtInfo.AppendText("指示灯控制\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);
                    //暂无操作
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x06")
                {
                    txtInfo.AppendText("GPS上报周期控制\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);
                    //暂无操作
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x07")
                {
                    txtInfo.AppendText("即时上报GPS\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);
                    //暂无操作
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x08")
                {
                    txtInfo.AppendText("通讯测试GPS\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);

                    #region 通讯测试实现
                    HttpClient myHttpClient = new HttpClient();
                    //提交当前地址的webapi
                    string url = "http://172.17.217.63/api/Instruction/UpdateInstruction/";
                    myHttpClient.BaseAddress = new Uri(url);

                    var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                    using (var http = new HttpClient(handler))
                    {
                        //使用FormUrlEncodedContent做HttpContent  
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                            {
                                { "InterlockingID",info.InterlockingID}, //设备ID
                                {"InstructionType","8"}//键名必须为空  
                            });
                        try
                        {
                            //await异步等待回应  
                            var response = await http.PutAsync(url, content);
                            //确保HTTP成功状态值  
                            response.EnsureSuccessStatusCode();
                            //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）  
                            //Console.WriteLine(await response.Content.ReadAsStringAsync());
                            txtInfo.AppendText(await response.Content.ReadAsStringAsync() + "\r\n");
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("L:" + e.Message.ToString());
                            txtInfo.AppendText("L:" + e.Message.ToString() + "\r\n");
                        }
                    }
                    #endregion
                }
                else if (String.Format("0x{0:x2}", bytes[1]) == "0x09")
                {
                    txtInfo.AppendText("参数配置指令\r\n");
                    //获取数据
                    Info info = getAnalysisData(bytes);
                    //暂无操作
                }
                else
                {
                    txtInfo.AppendText("命令出错\r\n");

                }
            }
            else
            {
                txtInfo.AppendText("命令出错\r\n");
            }
        }

        //解析获取数据
        public Info getAnalysisData(byte[] bytes)
        {
            Info Model = new Info();
            Model.FunctionCode = System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[2]) + String.Format("0x{0:x2}", bytes[1]), "0x", "");
            Model.FrameLength = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[5]) + String.Format("0x{0:x2}", bytes[4]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            for (int i = 13; i > 5; i--)
            {
                Model.InterlockingID += System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[i]), "0x", "").ToUpper();
            }
            Model.PowerValue = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[14]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            Model.Longitude = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[18]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[17]) + String.Format("0x{0:x2}", bytes[16]) + String.Format("0x{0:x2}", bytes[15]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "°";
            Model.Dimension = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[22]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[21]) + String.Format("0x{0:x2}", bytes[20]) + String.Format("0x{0:x2}", bytes[19]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "°";
            Model.PositioningAccuracy = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[26]) + String.Format("0x{0:x2}", bytes[25]) + String.Format("0x{0:x2}", bytes[24]) + String.Format("0x{0:x2}", bytes[23]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            Model.EntryExitState = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[27]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            for (int i = 31; i > 27; i--)
            {
                Model.PhysicalNumbering += System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[i]), "0x", "").ToUpper();
            }
            Model.Proofreading = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", bytes[36]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            return Model;
        }

        //获取定位数据
        public Info getLocationData(byte[] datas)
        {
            #region 位置定位数据
            Info Model = new Info();
            Model.FunctionCode = System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[2]) + String.Format("0x{0:x2}", datas[1]), "0x", "");
            Model.FrameLength = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[5]) + String.Format("0x{0:x2}", datas[4]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            for (int i = 13; i > 5; i--)
            {
                Model.InterlockingID += System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[i]), "0x", "").ToUpper();
            }
            Model.PowerValue = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[14]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            //Model.Longitude = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[18]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[17]) + String.Format("0x{0:x2}", datas[16]) + String.Format("0x{0:x2}", datas[15]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            //Model.Dimension = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[22]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[21]) + String.Format("0x{0:x2}", datas[20]) + String.Format("0x{0:x2}", datas[19]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            int sum1 = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[17]) + String.Format("0x{0:x2}", datas[16]) + String.Format("0x{0:x2}", datas[15]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            double sum2 = Convert.ToDouble(sum1) * Convert.ToDouble(0.0000001);
            string eme1 = sum2.ToString();
            if (eme1.Length > 3)
            {
                eme1 = eme1.Remove(0, 2);
            }
            Model.Dimension = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[18]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + eme1;
            int sum3 = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[21]) + String.Format("0x{0:x2}", datas[20]) + String.Format("0x{0:x2}", datas[19]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            double sum4 = Convert.ToDouble(sum3) * Convert.ToDouble(0.0000001);
            string eme2 = sum4.ToString();
            if (eme2.Length > 3)
            {
                eme2 = eme2.Remove(0, 2);
            }
            Model.Longitude = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[22]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier).ToString() + "." + eme2;
            Model.PositioningAccuracy = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[26]) + String.Format("0x{0:x2}", datas[25]) + String.Format("0x{0:x2}", datas[24]) + String.Format("0x{0:x2}", datas[23]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            Model.UnlockState = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[28]) + String.Format("0x{0:x2}", datas[27]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            Model.LastUnlockedTime = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[32]) + String.Format("0x{0:x2}", datas[31]) + String.Format("0x{0:x2}", datas[30]) + String.Format("0x{0:x2}", datas[29]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            Model.EntryExitState = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[33]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            for (int i = 38; i > 34; i--)
            {
                Model.PhysicalNumbering += System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[i]), "0x", "").ToUpper();
            }
            Model.Proofreading = int.Parse((System.Text.RegularExpressions.Regex.Replace(String.Format("0x{0:x2}", datas[43]), "0x", "")), System.Globalization.NumberStyles.AllowHexSpecifier);
            #endregion

            return Model;
        }

        ///      
        /// 获取当前系统时间的方法     
        ///      
        /// 当前时间     
        private DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            base.Visible = true;
            this.notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            //base.Show();  
            base.WindowState = FormWindowState.Normal;
        }
        //发送信息到客户端  
        private void btnOut_Click(object sender, EventArgs e)
        {
            string sendMsg = txtNoticias.Text.Trim();  //要发送的信息  
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);   //将要发送的信息转化为字节数组，因为Socket发送数据时是以字节的形式发送的  

            if (listBoxOnlineList.SelectedIndex == -1)
            {
                MessageBox.Show("请选择要发送的客户端！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                dic[selectClient].Send(bytes);   //发送数据  
                txtNoticias.Clear();
                txtInfo.AppendText(lblNoticias.Text + GetCurrentTime() + "\r\n" + sendMsg + "\r\n");

            }
        }
        //快捷键 Enter 发送信息   
        private void btnOut_KeyDown(object sender, KeyEventArgs e)
        {
            //如果用户按下了Enter键     
            if (e.KeyCode == Keys.Enter)
            {
                string sendMsg = txtNoticias.Text.Trim();  //要发送的信息  
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
                if (listBoxOnlineList.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择要发送的客户端！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {

                    string selectClient = listBoxOnlineList.Text;  //选择要发送的客户端  
                    dic[selectClient].Send(bytes);   //发送数据  
                    txtNoticias.Clear();
                    txtInfo.AppendText(lblNoticias.Text + GetCurrentTime() + "\r\n" + sendMsg + "\r\n");

                }
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("是否退出？选否,最小化到托盘", "操作提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Dispose();
            }
            else if (result == DialogResult.Cancel)
            {

                e.Cancel = true;

            }
            else
            {

                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
                e.Cancel = true;
            }
        }
        public ByteQueue queue = new ByteQueue();
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int len = serialPort1.BytesToRead;
            if (len > 0)
            {
                byte[] temp = new byte[len];
                serialPort1.Read(temp, 0, len);
                queue.Enqueue(temp);
                while (queue.Find()) //while可处理同时接收到多个AA ... 55 ,AA...55的包  
                {
                    int length = queue.GetLength();
                    byte[] readBuffer = new byte[len];
                    queue.Dequeue(readBuffer, 0, length);
                    OnReceiveData(readBuffer); //<这里自己写一个委托吧就OK了  
                }

            }
        }

        private void OnReceiveData(byte[] readBuffer)
        {
            txtInfo.AppendText(ByteQueue.ChangBytesToStr(readBuffer, 50));
        }

        private void 定时任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开启定时任务");
            int i = 0;
            //定时器
            var timer = new System.Timers.Timer();
            timer.Interval = 2000;
            timer.Enabled = true;
            timer.Elapsed += async (o, a) =>
            {
                try
                {
                    txtInfo.AppendText("i=" + i + "\r\n");
                    i++;
                }
                catch (Exception ex)
                {
                    txtInfo.AppendText(ex.Message.ToString());
                }
            };
            timer.Start();
            timer.Stop();
        }

        public async void Send(Socket client)
        {
            try {

            } catch(Exception ex) {
                txtInfo.AppendText(ex.Message.ToString() + "\r\n");
            }
            
        }


    }
}
