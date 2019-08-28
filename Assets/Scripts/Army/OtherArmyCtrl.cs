using Protocol.Constants;
using Protocol.Constants.Orc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherArmyCtrl : ArmyBase
{
    /// <summary>
    ///是否被选择
    /// </summary>
    public bool isSelect;

    /// <summary>
    /// 兵种状态
    /// </summary>
    public ArmyCardBase armyState { get; private set; }

    public delegate bool OtherArmySelectDelegate(OtherArmyCtrl otherArmyCtrl);
    public OtherArmySelectDelegate ArmySelectDelegate;

    public bool iscanShowStatePanel = true;//是否可以显示属性面板

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 初始化兵种属性
    /// </summary>
    /// <param name="race"></param>
    /// <param name="name"></param>
    public void Init(int race , int name)
    {
        isSelect = false;
        setArmyState(race, name);
    }

    private void setArmyState(int race , int name)
    {
        switch (race)
        {
            case RaceType.ORC:

                switch (name)
                {
                    case OrcArmyCardType.Infantry:
                        armyState = new OrcInfantry();
                        break;

                    case OrcArmyCardType.Eagle_Riders:
                        armyState = new OrcEagleRiders();
                        break;

                    case OrcArmyCardType.Black_Rats_Boomer:
                        armyState = new OrcEagleRiders();
                        break;

                    case OrcArmyCardType.Giant_mouthed_Frog:
                        armyState = new OrcGiantmouthedFrog();
                        break;

                    case OrcArmyCardType.Forest_Shooter:
                        armyState = new OrcForestShooter();
                        break;

                    case OrcArmyCardType.Pangolin:
                        armyState = new OrcPangolin();
                        break;

                    case OrcArmyCardType.Raven_Shaman:
                        armyState = new OrcRavenShaman();
                        break;

                    case OrcArmyCardType.Hero:
                        armyState = new OrcHero();
                        break;
                }

                break;
        }
    }

    private void OnMouseDown()
    {
         if (ArmySelectDelegate.Invoke(this))
            {
                //第一次或和上一次不一样
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyState);           
            }
          else  
            {
                //和上次一样
            }   
    }
}
