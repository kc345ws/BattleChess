using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyInfoPanel : UIBase
{
    private Button Button_Head;
    private Button Button_Ready;
    private Text Text_Ready;
    private Text Text_Name;

    private SocketMsg socketMsg;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.PLAYER_READY);
        Bind(UIEvent.PLAYER_HIDE_STATE);

        socketMsg = new SocketMsg();
        Button_Head = transform.Find("Button_Head").GetComponent<Button>();
        Button_Ready = transform.Find("Button_Ready").GetComponent<Button>();
        Text_Ready = transform.Find("Text_Ready").GetComponent<Text>();
        Text_Name = transform.Find("Text_Name").GetComponent<Text>();

        Text_Name.text = GameModles.Instance.userDto.Name;
        Text_Ready.gameObject.SetActive(false);

        Button_Ready.onClick.AddListener(readyBtnClicker);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Ready.onClick.RemoveAllListeners();
    }

    private void readyBtnClicker()
    {
        socketMsg.Change(OpCode.MATCH, MatchCode.READY_CREQ, "玩家准备");
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

        Button_Ready.gameObject.SetActive(false);
        Text_Ready.gameObject.SetActive(true);
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.PLAYER_READY:
                int uid = (int)message;
                if(uid == GameModles.Instance.userDto.ID)
                {
                    Text_Ready.gameObject.SetActive(true);
                }
                break;

            case UIEvent.PLAYER_HIDE_STATE:
                Text_Ready.gameObject.SetActive(false);
                break;
        }
    }
}
