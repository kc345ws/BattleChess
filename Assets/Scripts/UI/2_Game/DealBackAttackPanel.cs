using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 出反击面板
/// </summary>
public class DealBackAttackPanel : UIBase
{
    private Button Button_Yes;
    private Button Button_No;
    private Text Text_Info;
    //private string defaultInfo;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_DEAL_BACKATTACK_PANEL);
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
            case UIEvent.SHOW_DEAL_BACKATTACK_PANEL:
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
        Dispatch(AreoCode.CHARACTER, CharacterEvent.RETURN_DEAL_BACKATTACK_RESULT, true);
        SetPanelActive(false);
    }

    private void noBtnClicker()
    {
        Dispatch(AreoCode.CHARACTER, CharacterEvent.RETURN_DEAL_BACKATTACK_RESULT, false);
        SetPanelActive(false);
    }

    private void ChangeInfo(string str)
    {
        Text_Info.text = str;
    }
}
