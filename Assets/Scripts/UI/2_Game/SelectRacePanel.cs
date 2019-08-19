using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 种族选择面板
/// </summary>
public class SelectRacePanel : UIBase
{
    private Button Button_Orc;

    private SocketMsg socketMsg = new SocketMsg();
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_SELECT_RACE_PANEL);

        Button_Orc = transform.Find("Button_Orc").GetComponent<Button>();
        Button_Orc.onClick.AddListener(orcBtnClicker);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Orc.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.SHOW_SELECT_RACE_PANEL:
                SetPanelActive((bool)message);
                break;
        }
    }

    /// <summary>
    /// 0兽族
    /// </summary>
    private void orcBtnClicker()
    {
        socketMsg.Change(OpCode.FIGHT, FightCode.SELECT_RACE_CREQ, 0);
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        SetPanelActive(false);
    }
}
