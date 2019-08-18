using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : UIBase {
    private Button Button_Create;
    private InputField InputField_Name;
    private SocketMsg socketMsg;
    // Use this for initialization
    void Start () {
        Button_Create = transform.Find("Button_Create").GetComponent<Button>();
        InputField_Name = transform.Find("InputField_Name").GetComponent<InputField>();


        Button_Create.onClick.AddListener(createBtnClicker);
        socketMsg = new SocketMsg();
    }

    private void Awake()
    {
        Bind(UIEvent.SHOW_CREATE_PANEL);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.SHOW_CREATE_PANEL:
                gameObject.SetActive((bool)message);
                break;
        }
    }



    private void createBtnClicker()
    {
        string name = InputField_Name.text;
        if (string.IsNullOrEmpty(name))
        {
            MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "名字不能为空");
            return;
        }
        //TODO向服务器发送请求
        socketMsg.Change(OpCode.USER, UserCode.CREATE_USER_CREQ, name);
        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
    }
}
