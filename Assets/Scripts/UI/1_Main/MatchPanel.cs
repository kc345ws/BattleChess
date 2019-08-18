using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : UIBase {
    private Button Button_Match;//快速匹配按钮
    private Button Button_Cancel;//取消匹配按钮
    private Button Button_Enter;//进入房间按钮
    private Text Text_Matching;//正在匹配文字
    private Image Image_Logo;//图片
    private bool isShow = false;
    private int Dotcount = 0;
    private string defaultstr;
    private float Timer = 0f;

    private SocketMsg socketMsg;
    private SceneLoadMsg sceneLoadMsg;
    // Use this for initialization

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.SHOW_ROOM_ENTER_BUTTON:
                Button_Enter.gameObject.SetActive(true);
                break;
        }
    }

    void Start () {
        sceneLoadMsg = new SceneLoadMsg();
        socketMsg = new SocketMsg();
        Bind(UIEvent.SHOW_ROOM_ENTER_BUTTON);
        Button_Match = transform.Find("Button_Match").GetComponent<Button>();
        Button_Cancel = transform.Find("Button_Cancel").GetComponent<Button>();
        Button_Enter = transform.Find("Button_Enter").GetComponent<Button>();
        Text_Matching = transform.Find("Text_Matching").GetComponent<Text>();
        Image_Logo = transform.Find("Image_Logo").GetComponent<Image>();

        Button_Match.onClick.AddListener(matchBtnClicker);
        Button_Cancel.onClick.AddListener(cancelBtnClicker);
        Button_Enter.onClick.AddListener(enterBtnClicker);
        defaultstr = Text_Matching.text;

        setObjectActive(false);
        Button_Enter.gameObject.SetActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Match.onClick.RemoveAllListeners();
        Button_Cancel.onClick.RemoveAllListeners();
        Button_Enter.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update () {
        if (isShow)
        {
            Timer += Time.deltaTime;
            if(Timer >= 0.5f){
                matchAnimation();
                Timer = 0f;
            }         
        }
	}

    private void setObjectActive(bool active)
    {
        Image_Logo.gameObject.SetActive(active);
        Text_Matching.gameObject.SetActive(active);
        Button_Cancel.gameObject.SetActive(active);
    }

    private void matchBtnClicker()
    {
        //向服务器发送匹配请求
        socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, "发起匹配请求");
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

        //isShow = !isShow;
        isShow = true;
        setObjectActive(true);
        if (isShow == false)
        {
            Button_Enter.gameObject.SetActive(false);
        }

        //setObjectActive(isShow);
    }

    private void matchAnimation()
    {  
        Text_Matching.text += ".";
        Dotcount++;
        if(Dotcount >= 5)
        {
            Text_Matching.text = defaultstr;
            Dotcount = 0;
        }      
    }

    private void cancelBtnClicker()
    {
        //向服务器发送取消匹配请求
        socketMsg.Change(OpCode.MATCH, MatchCode.LEAVE_CREQ, "发起离开匹配请求");
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

        //关闭面板
        isShow = false;
        setObjectActive(false);
        Button_Enter.gameObject.SetActive(false);
    }

    private void enterBtnClicker()
    {
        sceneLoadMsg.Change(2, "03_Fight", ()=>
        {
            //Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "进入房间成功");
            /*Dispatch(AreoCode.UI, UIEvent.SET_MY_PLAYER_STATE, GameModles.Instance.userDto);
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "进入房间成功");*/

            /*int myuid = GameModles.Instance.userDto.ID;
            GameModles.Instance.matchRoomDto.ResetPosition(myuid);
            MatchRoomDto RoomDto = GameModles.Instance.matchRoomDto;
            if (RoomDto.Leftid != -1)
            {
                //如果存在左边玩家
                UserDto userDto = RoomDto.UidUdtoDic[RoomDto.Leftid];
                Dispatch(AreoCode.UI, UIEvent.SET_LEFT_PLAYER, userDto);
                //更新玩家面板
                Dispatch(AreoCode.UI, UIEvent.PLAYER_ENTER, userDto.ID);
            }

            if(RoomDto.Rightid != -1)
            {
                //如果存在右边玩家
                UserDto userDto = RoomDto.UidUdtoDic[RoomDto.Rightid];
                Dispatch(AreoCode.UI, UIEvent.SET_RIGHT_PLAYER, userDto);

                Dispatch(AreoCode.UI, UIEvent.PLAYER_ENTER, userDto.ID);
            }*/
        });
        Dispatch(AreoCode.SCENE, SceneCode.LOAD_SCENE, sceneLoadMsg);

    }
}
