using Protocol.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyStatePanel : UIBase
{
    private Text Text_NameValue;
    private Text Text_RaceValue;
    private Text Text_CanFlyValue;
    private Text Text_MaxHpValue;
    private Text Text_HpValue;
    private Button Button_ShowSkill;
    private Button Button_Close;

    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ARMY_STATE_PANEL);

        Text_NameValue = transform.Find("Text_NameValue").GetComponent<Text>();
        Text_RaceValue = transform.Find("Text_RaceValue").GetComponent<Text>();
        Text_CanFlyValue = transform.Find("Text_CanFlyValue").GetComponent<Text>();
        Text_MaxHpValue = transform.Find("Text_MaxHpValue").GetComponent<Text>();
        Text_HpValue = transform.Find("Text_HpValue").GetComponent<Text>();
        Button_ShowSkill = transform.Find("Button_ShowSkill").GetComponent<Button>();
        Button_Close = transform.Find("Button_Close").GetComponent<Button>();

        Button_Close.onClick.AddListener(closeBtnClicker);

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
            case UIEvent.SHOW_ARMY_STATE_PANEL:
                setState(message as ArmyCardBase);
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_Close.onClick.RemoveAllListeners();
    }

    private void setState(ArmyCardBase armystate)
    {
        //Text_RaceValue.text = armystate.Race.ToString();
        //Text_NameValue.text = armystate.Name.ToString();
        setName(armystate);
        Text_MaxHpValue.text = armystate.MaxHp.ToString();
        Text_HpValue.text = armystate.Hp.ToString();

        SetPanelActive(true);
    }

    private void setName(ArmyCardBase armystate)
    {
        switch (armystate.Race)
        {
            case RaceType.ORC:
                Text_RaceValue.text = "兽族";

                switch (armystate.Name)
                {
                    case OrcArmyCardType.Infantry:
                        Text_NameValue.text = "兽族步兵";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Eagle_Riders:
                        Text_NameValue.text = "鹰骑士";
                        Text_CanFlyValue.text = "是";
                        break;

                    case OrcArmyCardType.Black_Rats_Boomer:
                        Text_NameValue.text = "黑鼠爆破手";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Giant_mouthed_Frog:
                        Text_NameValue.text = "巨口蛙";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Forest_Shooter:
                        Text_NameValue.text = "巫林射手";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Pangolin:
                        Text_NameValue.text = "尖刺穿山甲";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Raven_Shaman:
                        Text_NameValue.text = "乌鸦萨满";
                        Text_CanFlyValue.text = "否";
                        break;

                    case OrcArmyCardType.Hero:
                        Text_NameValue.text = "兽族英雄";
                        Text_CanFlyValue.text = "否";
                        break;
                }
                break;
        }
    }

    private void closeBtnClicker()
    {
        SetPanelActive(false);
    }
}
