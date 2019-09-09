using Protocol.Code;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyInfoPanel : UIBase
{
    private Button Button_Head;//自己的头像按钮
    private Button Button_Ready;
    private Button Button_NextTurn;//下一回合按钮
    private Button Button_Cemetery;//墓地按钮
    private Button Button_Setting;//设置按钮
    private Text Text_Ready;
    private Text Text_Name;

    private SocketMsg socketMsg;

    public ushort TurnCount = 0;//回合数

    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.PLAYER_READY);
        Bind(UIEvent.PLAYER_HIDE_STATE);
        Bind(UIEvent.NEXT_TURN);

        socketMsg = new SocketMsg();
        Button_Head = transform.Find("Button_Head").GetComponent<Button>();
        Button_Ready = transform.Find("Button_Ready").GetComponent<Button>();
        Text_Ready = transform.Find("Text_Ready").GetComponent<Text>();
        Text_Name = transform.Find("Text_Name").GetComponent<Text>();
        Button_NextTurn = transform.Find("Button_NextTurn").GetComponent<Button>();
        Button_Cemetery = transform.Find("Button_Cemetery").GetComponent<Button>();
        Button_Setting = transform.Find("Button_Setting").GetComponent<Button>();

        Text_Name.text = GameModles.Instance.userDto.Name;
        Text_Ready.gameObject.SetActive(false);

        Button_NextTurn.interactable = false;
        //Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "对方回合");
        //Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "对方回合");

        Button_Ready.onClick.AddListener(readyBtnClicker);
        Button_NextTurn.onClick.AddListener(nextTurnBtnClicker);

        MapBuilder.Instance.turnDelegate = processTurndelegate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Ready.onClick.RemoveAllListeners();
        Button_NextTurn.onClick.RemoveAllListeners();
    }

    private void processTurndelegate(ref int turncount)
    {
        turncount = TurnCount;
    }



    /// <summary>
    /// 下一回合按钮事件
    /// </summary>
    private void nextTurnBtnClicker()
    {
        if (TurnCount == 1 && MyCharacterCtrl.Instance.hasTypeCard(CardType.ARMYCARD))
        {
            //第一回合放置所有单位
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "第一回合必须放置所有单位");
            return;
        }
        socketMsg.Change(OpCode.FIGHT, FightCode.NEXT_TURN_CREQ, "下一回合");
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "对方回合");
        Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "对方回合");

        Button_NextTurn.interactable = false;
    }

    private void readyBtnClicker()
    {
       
        socketMsg.Change(OpCode.MATCH, MatchCode.READY_CREQ, "玩家准备");
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

        Button_Ready.gameObject.SetActive(false);
        Text_Ready.gameObject.SetActive(true);
    }

    /// <summary>
    /// 处理下一回合
    /// </summary>
    private void processNextTurn(int uid)
    {
        if(uid == GameModles.Instance.userDto.ID)
        {
            //如果轮到自己了
            Button_NextTurn.interactable = true;
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE,"关闭");
            Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "轮到你了");

            Dispatch(AreoCode.ARMY, ArmyEvent.TURN_REFRESH_ARMYSTATE, "刷新兵种状态");

            TurnCount++;//回合数加一
        }
        else
        {
            Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "对方回合");
            Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "对方回合");
        }
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

            case UIEvent.NEXT_TURN:
                processNextTurn((int)message);
                break;
        }
    }
}
