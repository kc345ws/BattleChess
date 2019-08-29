using Protocol.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmySelectAttackPanel : UIBase
{
    private Button Button_Land;
    private Button Button_Sky;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_SELECT_ATTACK_PANEL);

        Button_Land = transform.Find("Button_Land").GetComponent<Button>();
        Button_Sky = transform.Find("Button_Sky").GetComponent<Button>();

        Button_Land.onClick.AddListener(landBtnClicker);
        Button_Sky.onClick.AddListener(skyBtnClicker);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Land.onClick.RemoveAllListeners();
        Button_Sky.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventcode, object message)
    {
        //base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.SHOW_SELECT_ATTACK_PANEL:
                processShowPanel();
                break;
        }
    }

    private void processShowPanel()
    {
        SetPanelActive(true);
    }

    /// <summary>
    /// 选择陆地单位
    /// </summary>
    private void landBtnClicker()
    {
        Dispatch(AreoCode.UI, UIEvent.SET_SELECK_ATTACK, ArmyMoveType.LAND);
        SetPanelActive(false);
    }

    /// <summary>
    /// 选择飞行单位
    /// </summary>
    private void skyBtnClicker()
    {
        Dispatch(AreoCode.UI, UIEvent.SET_SELECK_ATTACK, ArmyMoveType.SKY);
        SetPanelActive(false);
    }
}
