using System.Linq;
using System.Threading;
using HRtoVRChat_OSC_SDK;
using SuperSimpleTcp;

namespace HRtoVRChat.HRManagers;

public class SDKManager : HRManager
{
    private Thread _worker;
    private CancellationTokenSource token;

    private SimpleTcpServer server;

    private int HR;
    private bool isActive;
    private bool isOpen;
    
    public bool Init(string d1)
    {
        if (_worker != null)
        {
            token.Cancel();
        }

        token = new CancellationTokenSource();
        _worker = new Thread(() =>
        {
            server = new SimpleTcpServer(d1);
            server.Events.ClientConnected += (sender, args) => LogHelper.Log("SDKManager", "SDK Connected!");
            server.Events.ClientDisconnected += (sender, args) => LogHelper.Log("SDKManager", "SDK Disconnected!");
            server.Events.DataReceived += (sender, args) =>
            {
                byte[] data = args.Data;
                object? fakeDeserialize = Messages.DeserializeMessage(data);
                string messageType = Messages.GetMessageType(fakeDeserialize);
                switch (messageType)
                {
                    case "HRMessage":
                        Messages.HRMessage hrm = Messages.DeserializeMessage<Messages.HRMessage>(data);
                        HR = hrm.HR;
                        isActive = hrm.IsActive;
                        isOpen = hrm.IsOpen;
                        break;
                    case "GetHRData":
                        Messages.HRMessage hrm_ghrd = new Messages.HRMessage
                        {
                            HR = HR,
                            IsActive = isActive,
                            IsOpen = isOpen
                        };
                        server.Send(args.IpPort, hrm_ghrd.Serialize());
                        break;
                    default:
                        LogHelper.Warn("SDKManager", "Unknown Debug Message: " + messageType);
                        break;
                }
            };
            server.Start();
            while (!token.IsCancellationRequested)
            {
                int c = server.GetClients().ToList().Count;
                if (c <= 0)
                {
                    // reset all data
                    HR = 0;
                    isOpen = false;
                    isActive = false;
                }
                Thread.Sleep(10);
            }
            server?.Stop();
        });
        _worker.Start();
        return true;
    }

    public int GetHR() => HR;

    public void Stop() => token.Cancel();

    public bool IsOpen() => isOpen;

    public bool IsActive() => isActive;
}