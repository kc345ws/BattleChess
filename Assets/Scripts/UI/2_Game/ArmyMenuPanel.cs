using Protocol.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyMenuPanel : UIBase
{
    private Button Button_State;
    private Button Button_Close;

    private ArmyCardBase armyState;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ARMY_MENU_PANEL);
        Bind(UIEvent.CLOSE_ARMY_MENU_PANEL);

        Button_State = transform.Find("Button_State").GetComponent<Button>();
        Button_Close = transform.Find("Button_Close").GetComponent<Button>();

        Button_State.onClick.AddListener(stateBtnClicker);
        Button_Close.onClick.AddListener(closeBtnClicker);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.SHOW_ARMY_MENU_PANEL:
                armyState = message as ArmyCardBase;
                SetPanelActive(true);
                break;

            case UIEvent.CLOSE_ARMY_MENU_PANEL:
                SetPanelActive(false);
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_State.onClick.RemoveAllListeners();
        Button_Close.onClick.RemoveAllListeners();
    }

    private void closeBtnClicker()
    {
        SetPanelActive(false);
    }

    private void stateBtnClicker()
    {
        Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyState);
    }
}
