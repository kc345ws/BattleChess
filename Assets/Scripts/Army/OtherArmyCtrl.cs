using Protocol.Constants;
using Protocol.Constants.Map;
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

    public MapPointCtrl OtherMapPintctrl { get; private set; }//敌方兵种所在地图点控制器

    private int SelectArmyType = -1;//兵种重合时选择的兵种类型

    // Start is called before the first frame update
    void Start()
    {
        Bind(ArmyEvent.SET_OTHER_LAND_SKY);
    }

    public override void Execute(int eventcode, object message)
    {
        //base.Execute(eventcode, message);
        switch (eventcode)
        {
            case ArmyEvent.SET_OTHER_LAND_SKY:
                SelectArmyType = (int)message;
                break;
        }
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
    public void Init(int race , int name , MapPointCtrl mapPointCtrl)
    {
        isSelect = false;
        setArmyState(race, name);
        OtherMapPintctrl = mapPointCtrl;
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
        
    }

    private void OnMouseDrag()
    {
        if (!iscanShowStatePanel)
        {
            return;
        }
        if (OtherMapPintctrl.LandArmy != null && OtherMapPintctrl.SkyArmy != null)
        {
            //如果陆地和飞行单位重合
            Dispatch(AreoCode.UI, UIEvent.SELECT_OTHER_LAND_SKY, false);
            StartCoroutine(selectArmy());
        }
        else
        {
            //如果只有一个单位
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

    /// <summary>
    /// 选择兵种协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator selectArmy()
    {
        yield return new WaitUntil(isSelectArmyType);
        if (SelectArmyType == ArmyMoveType.LAND)
        {
            //选择了陆地兵种
            if(OtherMapPintctrl.LandArmy == null)
            {
                yield break;
            }
            OtherArmyCtrl armyCtrl = OtherMapPintctrl.LandArmy.GetComponent<OtherArmyCtrl>();

            if (ArmySelectDelegate.Invoke(armyCtrl) && iscanShowStatePanel)
            {
                //第一次或和上一次不一样
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
            }
            else if (iscanShowStatePanel)
            {
                //和上次一样
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
            }
            SelectArmyType = -1;
        }
        else
        {
            //如果选择了飞行单位
            if(OtherMapPintctrl.SkyArmy == null)
            {
                yield break;
            }
            OtherArmyCtrl armyCtrl = OtherMapPintctrl.SkyArmy.GetComponent<OtherArmyCtrl>();
            if (ArmySelectDelegate.Invoke(armyCtrl) && iscanShowStatePanel)
            {
                //第一次或和上一次不一样
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
            }
            else if(iscanShowStatePanel)
            {
                //和上次一样
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
            }
            SelectArmyType = -1;
        }
        SelectArmyType = -1;
    }

    /// <summary>
    /// 是否选择兵种类型
    /// </summary>
    /// <returns></returns>
    private bool isSelectArmyType()
    {
        if (SelectArmyType != -1)
        {
            return true;
        }
        return false;
    }

    public void Move(MapPoint mapPoint , MapPointCtrl mapPointCtrl)
    {
        armyState.Position = mapPoint;
        OtherMapPintctrl = mapPointCtrl;
    }
}
