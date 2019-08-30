using Protocol.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmySelectAttackPanel : UIBase
{
    private Button Button_Land;
    private Button Button_Sky;

    private bool isAttack = false;//是否是攻击请求
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_SELECT_ATTACK_PANEL);
        Bind(UIEvent.SELECT_LAND_SKY);

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
                processShowPanel((bool)message);
                break;

            case UIEvent.SELECT_LAND_SKY:
                processShowPanel((bool)message);
                break;
        }
    }

    private void processShowPanel(bool flag)
    {
        isAttack = flag;
        SetPanelActive(true);
    }

    /// <summary>
    /// 选择陆地单位
    /// </summary>
    private void landBtnClicker()
    {
        if (isAttack)
        {
            Dispatch(AreoCode.UI, UIEvent.SET_SELECK_ATTACK, ArmyMoveType.LAND);
        }
        else
        {
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_LAND_SKY, ArmyMoveType.LAND);
        }
        SetPanelActive(false);
    }

    /// <summary>
    /// 选择飞行单位
    /// </summary>
    private void skyBtnClicker()
    {
        if (isAttack)
        {
            Dispatch(AreoCode.UI, UIEvent.SET_SELECK_ATTACK, ArmyMoveType.SKY);
        }
        else
        {
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_LAND_SKY, ArmyMoveType.SKY);
        }
        SetPanelActive(false);
    }
}
