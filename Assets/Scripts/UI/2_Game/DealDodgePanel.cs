using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择闪避面板
/// </summary>
public class DealDodgePanel : UIBase
{
    private Button Button_Yes;
    private Button Button_No;
    private Text Text_Info;
    //private string defaultInfo;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_DEAL_DODGE_PANEL);
        Button_Yes = transform.Find("Button_Yes").GetComponent<Button>();
        Button_No = transform.Find("Button_No").GetComponent<Button>();
        Text_Info = transform.Find("Text_Info").GetComponent<Text>();
        //defaultInfo = Text_Info.text;

        Button_Yes.onClick.AddListener(yesBtnClicker);
        Button_No.onClick.AddListener(noBtnClicker);

        SetPanelActive(false);
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
            case UIEvent.SHOW_DEAL_DODGE_PANEL:
                Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示隐藏面板");
                Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭等待面板");
                ChangeInfo((string)message);
                SetPanelActive(true);
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Yes.onClick.RemoveAllListeners();
        Button_No.onClick.RemoveAllListeners();
    }

    private void yesBtnClicker()
    {
        //发送消息
        Dispatch(AreoCode.CHARACTER, CharacterEvent.RETURN_DEAL_DODGE_RESULT, true);
        SetPanelActive(false);
        Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "");
        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
    }

    private void noBtnClicker()
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.RETURN_DEAL_DODGE_RESULT, false);
        SetPanelActive(false);
        Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "");
        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
    }

    private void ChangeInfo(string str)
    {
        Text_Info.text = str;
    }
}
