using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Threading; 
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OscJack;

public class ESNetwork : HimeLib.SingletonMono<ESNetwork>
{
    public string LastRemoteClientIp;
    public int udpPort = 25588;
    public int tcpPort = 25544;
    public int oscPort = 15566;
    public System.Action<string> OnNewStrokeCome;

    private UdpClient udpServer;
    private Thread receiveThread;

    void Start()
    {
        StartServer(tcpPort);

        udpServer = new UdpClient(udpPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    async void StartServer(int userPort){
        await ReceiveFilesAsync(userPort);
    }

    async Task ReceiveFilesAsync(int usePort)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, usePort);
        listener.Start();

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            await HandleClientAsync(client, usePort);
        }
    }

    async Task HandleClientAsync(TcpClient client, int usePort)
    {
        try
        {
            NetworkStream stream = client.GetStream();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                // 分段接收資料
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                }

                // 將接收到的數據轉換為Texture
                byte[] receivedData = memoryStream.ToArray();
                string dataString = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

                OnNewStrokeCome?.Invoke(dataString);
            }

            stream.Close();
            client.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error handling client: {e.Message}");
        }
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] receiveBytes = udpServer.Receive(ref remoteEndPoint);
                string clientIp = remoteEndPoint.Address.ToString();
                LastRemoteClientIp = clientIp;
                Debug.Log("Received message from IP: " + clientIp);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        if (udpServer != null)
        {
            udpServer.Close();
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }

    public void SendLeftUsage(int val){
        if(string.IsNullOrEmpty(LastRemoteClientIp))
            return;

        using (var client = new OscClient(LastRemoteClientIp, oscPort))
        {
            client.Send("/usageleft", val);
        }
    }

    public void SendRightUsage(int val){
        if(string.IsNullOrEmpty(LastRemoteClientIp))
            return;

        using (var client = new OscClient(LastRemoteClientIp, oscPort))
        {
            client.Send("/usageright", val);
        }
    }
}
