using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIBase
{
    private Button Button_Back;
    private Text Text_Result;
    private Text Text_ExpResult;
    private Text Text_MoneyResult;

    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_WIN_OVER_PANEL);
        Bind(UIEvent.SHOW_LOSE_OVER_PANEL);

        Button_Back = transform.Find("Button_Back").GetComponent<Button>();
        Text_Result = transform.Find("Text_Result").GetComponent<Text>();
        Text_ExpResult = transform.Find("Text_ExpResult").GetComponent<Text>();
        Text_MoneyResult = transform.Find("Text_MoneyResult").GetComponent<Text>();

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
            case UIEvent.SHOW_WIN_OVER_PANEL:
                Text_Result.text = "你胜利了";
                Text_ExpResult.text = "经验:+20";
                Text_MoneyResult.text = "金币:+100";
                SetPanelActive(true);

                break;

            case UIEvent.SHOW_LOSE_OVER_PANEL:
                Text_Result.text = "你失败了";
                Text_ExpResult.text = "经验:+5";
                Text_MoneyResult.text = "金币:+30";
                SetPanelActive(true);

                break;
        }
    }
}
