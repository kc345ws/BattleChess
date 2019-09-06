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
    private bool isSetOther = false;//是否是设置他人的重叠兵种结果
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_SELECT_ATTACK_PANEL);
        Bind(UIEvent.SELECT_MY_LAND_SKY);
        Bind(UIEvent.SELECT_OTHER_LAND_SKY);

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

            case UIEvent.SELECT_MY_LAND_SKY:
                isSetOther = false;
                processShowPanel((bool)message);
                break;

            case UIEvent.SELECT_OTHER_LAND_SKY:
                isSetOther = true;
                processShowPanel((bool)message);
                break;
        }
    }



    /// <summary>
    /// 选择面板
    /// </summary>
    /// <param name="flag">是否攻击</param>
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
            ///设置兵种重叠攻击选择结果
            Dispatch(AreoCode.UI, UIEvent.SET_SELECK_ATTACK, ArmyMoveType.LAND);
        }
        else if(!isSetOther)
        {
            //设置自己的兵种重叠结果
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_MY_LAND_SKY, ArmyMoveType.LAND);
            Dispatch(AreoCode.CHARACTER, CharacterEvent.SET_MY_LAND_SKY, ArmyMoveType.LAND);
        }
        else
        {
            //设置他人的兵种重叠结果
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_OTHER_LAND_SKY, ArmyMoveType.LAND);
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
        else if(!isSetOther)
        {
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_MY_LAND_SKY, ArmyMoveType.SKY);
            Dispatch(AreoCode.CHARACTER, CharacterEvent.SET_MY_LAND_SKY, ArmyMoveType.SKY);
        }
        else
        {
            Dispatch(AreoCode.ARMY, ArmyEvent.SET_OTHER_LAND_SKY, ArmyMoveType.SKY);
        }
        SetPanelActive(false);
    }
}
