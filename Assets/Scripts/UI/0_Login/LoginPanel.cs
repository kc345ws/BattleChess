using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Dto;
using Protocol.Code;

public class LoginPanel : UIBase {
    private Button BTN_Login;
    private Button BTN_Back;
    private InputField input_Account;
    private InputField input_PWD;

    private AccountDto account = null;
    private SocketMsg msg = null;
    // Use this for initialization
    void Start () {
        Bind(UIEvent.LOGIN_PANEL_EVENTCODE);
        BTN_Login = transform.Find("Button_Login").GetComponent<Button>();
        BTN_Back = transform.Find("Button_Back").GetComponent<Button>();
        input_Account = transform.Find("InputField_Account").GetComponent<InputField>();
        input_PWD = transform.Find("InputField_PassWord").GetComponent<InputField>();

        BTN_Login.onClick.AddListener(loginBtnClicker);
        BTN_Back.onClick.AddListener(backBtnClicker);

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        //Bind(UIEvent.START_PANEL_EVENTCODE);
        account = new AccountDto();
        msg = new SocketMsg();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.LOGIN_PANEL_EVENTCODE:
                SetPanelActive((bool)message);
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        BTN_Login.onClick.RemoveAllListeners();
        BTN_Back.onClick.RemoveAllListeners();
    }

    private void loginBtnClicker()
    {
        NetManager.Connect();
        if (string.IsNullOrEmpty(input_Account.text)
            ||string.IsNullOrEmpty(input_PWD.text))
        {
            MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "帐号或密码不能为空");
            return;
        }
        /*if(input_PWD.text.Length < 6 || input_PWD.text.Length > 16)
        {
            MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "密码必须大于6位且小于16位");
            return;
        }*/


        account.Account = input_Account.text;
        account.Password = input_PWD.text;
        msg.OpCode = OpCode.ACCOUNT;
        msg.SubCode = AccountCode.LOGIN_CREQ;
        msg.Value = account;
        MsgCenter.Instance.Dispatch(AreoCode.NET, NetEvent.SENDMSG, msg);
    }

    private void backBtnClicker()
    {
        SetPanelActive(false);
    }
}
