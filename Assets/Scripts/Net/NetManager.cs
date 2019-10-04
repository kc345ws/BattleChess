using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Code;
using Assets.Scripts.Net.implement;


/// <summary>
/// 网络消息转发中心
/// </summary>
public class NetManager : ManagerBase {
    public static NetManager Instance = null;

    public static ClientPeer Client {get;private set; }//服务器套接字

    /*override是指“覆盖”，是指子类覆盖了父类的方法。子类的对象无法再访问父类中的该方法。
new是指“隐藏”，是指子类隐藏了父类的方法，当然，通过一定的转换，可以在子类的对象中访问父类的方法。*/
    public override void Execute(int eventcode, object message)
    {
        
        switch (eventcode)
        {
            //发送消息
            case NetEvent.SENDMSG:
                Client.SendMessage(message as SocketMsg);
                break;

            case NetEvent.RECEIVEMSG:
                break;

            default:
                break;
        }
    }

    public NetManager()
    {
        Client = new ClientPeer("127.0.0.1", 59800);  
       // Client = new ClientPeer("47.93.8.200", 59800);
    }

    void Awake()
    {
        Instance = this;

        //Add(NetEvent.SENDMSG, this);
        //Add(NetEvent.RECEIVEMSG, this);
    }
    // Use this for initialization
    void Start () {
        Client.Connect();
	}

    public static void Connect()
    {
        if(Client.clientSocket.Connected == false)
        {
            Client.Connect();
        }      
    }
	
	// Update is called once per frame
	void Update () {
        receiveMessage();
    }

    #region 分别处理客户端从服务器接受到的消息

    private void receiveMessage()
    {
        if (Client.msgQueue.Count <= 0)
        {
            return;
        }

        while (Client.msgQueue.Count > 0)
        {
            SocketMsg msg = Client.msgQueue.Dequeue();

            //处理数据
            processMessage(msg);
        }
    }

    public void processMessage(SocketMsg socketMsg)
    {
        switch (socketMsg.OpCode)
        {
            case OpCode.ACCOUNT:

                //AccountHandler.Instance.Dispatch(AreoCode.UI, socketMsg.SubCode, socketMsg.Value);
                //应该接收消息
                AccountHandler.Instance.OnReceive(socketMsg.SubCode, socketMsg.Value);
                break;

            /*case AreoCode.SCENE:
                AccountHandler.Instance.Dispatch(AreoCode.SCENE, socketMsg.SubCode, socketMsg.Value);
                break;*/

            case OpCode.USER:
                //UserHandler.Instance.Dispatch(AreoCode.UI, socketMsg.SubCode, socketMsg.Value);
                UserHandler.Instance.OnReceive(socketMsg.SubCode, socketMsg.Value);
                break;

            case OpCode.MATCH:
                MatchHandler.Instance.OnReceive(socketMsg.SubCode, socketMsg.Value);
                break;

            case OpCode.CHAT:
                ChatHandler.Instance.OnReceive(socketMsg.SubCode, socketMsg.Value);
                break;

            case OpCode.FIGHT:
                FightHandler.Instance.OnReceive(socketMsg.SubCode, socketMsg.Value);
                break;





            default:
                break;
        }
    }

    #endregion

}
