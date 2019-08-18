using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherInfoPanel : UIBase
{
    private Button Button_Head;
    private Text Text_Ready;
    private Text Text_Name;

    private SocketMsg socketMsg;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SET_OTHER_PLAYER_INFO);
        Bind(UIEvent.PLAYER_READY);
        Bind(UIEvent.PLAYER_HIDE_STATE);

        socketMsg = new SocketMsg();
        Button_Head = transform.Find("Button_Head").GetComponent<Button>();
        Text_Ready = transform.Find("Text_Ready").GetComponent<Text>();
        Text_Name = transform.Find("Text_Name").GetComponent<Text>();
     
        Text_Name.text = "";
        Button_Head.gameObject.SetActive(false);
        Text_Ready.gameObject.SetActive(false);

        if (GameModles.Instance.matchRoomDto.OtherID !=-1)
        {
            UserDto userDto = GameModles.Instance.matchRoomDto.UidUdtoDic[GameModles.Instance.matchRoomDto.OtherID];
            Text_Name.text = userDto.Name;
            Button_Head.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.SET_OTHER_PLAYER_INFO:
                setInfo();
                break;

            case UIEvent.PLAYER_READY:
                int uid = (int)message;
                if(uid == GameModles.Instance.matchRoomDto.OtherID)
                {
                    Text_Ready.gameObject.SetActive(true);
                }
                break;

            case UIEvent.PLAYER_HIDE_STATE:
                Text_Ready.gameObject.SetActive(false);
                break;
        }
    }


    private void setInfo()
    {
        int myuid = GameModles.Instance.userDto.ID;
        MatchRoomDto matchRoomDto = GameModles.Instance.matchRoomDto;
        foreach (var item in matchRoomDto.UidUdtoDic)
        {
            if(item.Key != myuid)
            {
                Button_Head.gameObject.SetActive(true);
                Text_Name.text = item.Value.Name;
            }
        }
    }
}
