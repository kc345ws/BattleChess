using Protocol.Constants;
using Protocol.Constants.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmyMenuPanel : UIBase
{
    private static ArmyMenuPanel instance = null;
    public static ArmyMenuPanel Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new ArmyMenuPanel();
            }
            return instance;
        }
    }
    private ArmyMenuPanel() { }

    private Button Button_State;
    private Button Button_Close;
    private Button Button_Attack_Land;
    private Button Button_Attack_Sky;
    private Button Button_Turn;
    private Button Button_Skill;

    //private ArmyCardBase armyState;
    private static ArmyCtrl armyCtrl;//兵种控制器

    private MapPointCtrl attackMapPointCtrl;//要攻击的地图点控制器

    private static MapPointCtrl clickMapPointCtrl;//点击的地图点控制器

    private bool isAttack = false;//是否处于攻击状态

    private int defenseArmyType = -1;//进行防御的兵种类型

    private OtherArmyCtrl defenseArmy = null;//进行防御的兵种控制器

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ARMY_MENU_PANEL);
        Bind(UIEvent.CLOSE_ARMY_MENU_PANEL);
        Bind(UIEvent.IS_ATTACK_SUCCESS);
        //Bind(UIEvent.SET_SELECK_ATTACK);
        Bind(UIEvent.IS_BACKATTACK);

        Button_State = transform.Find("Button_State").GetComponent<Button>();
        Button_Close = transform.Find("Button_Close").GetComponent<Button>();
        Button_Attack_Land = transform.Find("Button_Attack_Land").GetComponent<Button>();
        Button_Attack_Sky = transform.Find("Button_Attack_Sky").GetComponent<Button>();
        Button_Turn = transform.Find("Button_Turn").GetComponent<Button>();
        Button_Skill = transform.Find("Button_Skill").GetComponent<Button>();

        Button_State.onClick.AddListener(stateBtnClicker);
        Button_Close.onClick.AddListener(closeBtnClicker);
        Button_Attack_Land.onClick.AddListener(attackLandBtnClicker);
        Button_Attack_Sky.onClick.AddListener(attackSkyBtnClicker);
        Button_Skill.onClick.AddListener(skillBtnClicker);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        clickMapPointCtrl = GetMapPointCtrl();
        //StartCoroutine(selectDenfenseArmy());
    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.SHOW_ARMY_MENU_PANEL:
                //armyState = message as ArmyCardBase;
                armyCtrl = message as ArmyCtrl;
                //给指令卡管理器传送单位控制器
                OrderCardManager.Instance.selectArmyCtrlDelegate.Invoke(armyCtrl);
                OtherCardManager.Instance.selectArmyCtrlDelegate.Invoke(armyCtrl);
                //SetPanelActive(true);
                processShowMenuPanel();
                break;

            case UIEvent.CLOSE_ARMY_MENU_PANEL:
                SetPanelActive(false);

                //解除屏蔽选择
                foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                {
                    item.canBeSeletced = true;
                }
                StopAllCoroutines();
                break;

            /*case UIEvent.SET_SELECK_ATTACK:
                defenseArmyType = (int)message;
                break;*/
            case UIEvent.IS_ATTACK_SUCCESS:
                processIsAttackSuccess((bool)message);
                break;

            case UIEvent.IS_BACKATTACK:
                processIsBackAttack((bool)message);
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_State.onClick.RemoveAllListeners();
        Button_Close.onClick.RemoveAllListeners();
        Button_Attack_Land.onClick.RemoveAllListeners();
        Button_Attack_Sky.onClick.RemoveAllListeners();
        Button_Skill.onClick.RemoveAllListeners();
    }

    private void processIsBackAttack(bool active)
    {
        if (active)//对方反击
        {
            
            //自己掉血
            armyCtrl.armyState.Hp -= defenseArmy.armyState.Damage;
            //提示消息
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方反击了");

            Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭等待面板");
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏平面");
            
            //移除卡牌
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_OTHER_CARDS, "移除手牌");

            //防御单位置空
            defenseArmy = null;
        }
        else//对方不反击
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方没有反击");

            Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭等待面板");
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏平面");
            //防御单位置空
            defenseArmy = null;
        }
    }

    /// <summary>
    /// 处理是否攻击成功
    /// </summary>
    /// <param name="active"></param>
    private void processIsAttackSuccess(bool active)
    {
        if (active)//对方不闪避
        {
            //对方减血
            defenseArmy.armyState.Hp -= armyCtrl.armyState.Damage;
            //提示消息
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方没有出闪避");

            //等待反击
            Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭等待面板");
            Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "等待对方是否反击...");

            if(defenseArmy.armyState.Hp <= 0)
            {
                //发送单位死亡消息
                Dispatch(AreoCode.ARMY, ArmyEvent.ARMY_DEAD, armyCtrl);
            }
        }
        else//对方闪避
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "对方闪避了");
            Dispatch(AreoCode.UI, UIEvent.CLOSE_WAIT_PANEL, "关闭等待面板");
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏平面");

            //防御单位置空
            //defenseArmy = null;
            //移除卡牌
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_OTHER_CARDS, "移除手牌");
        }

        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
        Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "等待对方是否反击");
    }

   

    /// <summary>
    /// 处理显示面板事件
    /// </summary>
    private void processShowMenuPanel()
    {
        if (armyCtrl.canAttack)
        {
            //如果兵种能攻击且没有攻击过
            //Button_Attack.enabled = true;
            Button_Attack_Land.interactable = true;
        }
        else
        {
            Button_Attack_Land.interactable = false;
        }

        if(armyCtrl.canAttack&& (armyCtrl.armyState.MoveType == ArmyMoveType.SKY || armyCtrl.armyState.CanSlantAttack))
        {
            //如果飞行单位或者斜射单位能攻击且没有攻击过
            Button_Attack_Sky.interactable = true;
        }
        else
        {
            Button_Attack_Sky.interactable = false;
        }

        if(!armyCtrl.armySkill.isPassive && armyCtrl.armySkill.isBind && !armyCtrl.armySkill.isUsed)
        {
            //非被动技能且已经绑定了技能
            Button_Skill.interactable = true;
        }
        else
        {
            Button_Skill.interactable = false;
        }
        SetPanelActive(true);
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    private void skillBtnClicker()
    {
        armyCtrl.armySkill.UseSkill();
    }

    private void closeBtnClicker()
    {
        refresh();//复原状态

        if (armyCtrl != null)
        {
            armyCtrl.ArmySelectEvent.Invoke(armyCtrl);
            foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
            {
                item.canBeSeletced = true;
            }
            armyCtrl.CheckIsA();
        }

        //停止所有协程
        StopAllCoroutines();
        Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "正常光标");
        SetPanelActive(false);
    }

    private void stateBtnClicker()
    {
        Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
    }

    /// <summary>
    /// 攻击陆地单位按钮
    /// </summary>
    private void attackLandBtnClicker()
    {
        attackState();
        if(armyCtrl != null)
        {
            StartCoroutine(AttackLand());
        }
    }

    /// <summary>
    /// 攻击飞行单位按钮
    /// </summary>
    private void attackSkyBtnClicker()
    {
        attackState();
        if (armyCtrl != null)
        {
            StartCoroutine(AttackSky());
        }
    }

    private void attackState()
    {
        if (armyCtrl != null)
        {
            //设置光标
            if (!isAttack)
            {
                Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_ATTACK, "设置攻击光标");
                isAttack = true;
            }
            //屏蔽移动
            Color color = new Color(150f / 255f, 47f / 255f, 127f / 255f);
            armyCtrl.setMappointCtrlColor(armyCtrl.canMovePointCtrls);
            armyCtrl.setMappointCtrlColor(armyCtrl.canAttckPointCtrls,color);
            armyCtrl.iscanMove = false; 
        }
    }

    /// <summary>
    /// 攻击陆地单位协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackLand()
    {
        //OtherArmyCtrl defenseArmy = null;//进行防御的兵种

        //选择要攻击的地图点
        if (clickMapPointCtrl == null)
        {
            //refresh();
            yield return new WaitUntil(isclickMapPointCtrlnull);
            attackState();
        }

        //屏蔽查看属性
        if (clickMapPointCtrl != null)//defenseArmy != null)
        {
            if (clickMapPointCtrl.LandArmy != null) {
                OtherArmyCtrl otherArmyCtrl = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
                if(otherArmyCtrl != null)
                {
                    otherArmyCtrl.iscanShowStatePanel = false;
                }
        }
            if(clickMapPointCtrl.SkyArmy !=null)
            {
                OtherArmyCtrl otherArmyCtrl = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
                if(otherArmyCtrl != null)
                {
                    otherArmyCtrl.iscanShowStatePanel = false;
                }
            }
            
        }

        if (clickMapPointCtrl.LandArmy != null)
        {
            //如果攻击方是陆地单位
            if (clickMapPointCtrl.LandArmy != armyCtrl.ArmyPrefab)
            {
                //如果攻击的不是自己
                defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
            }
        }

        if (defenseArmy == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "攻击目标无效");
            refresh();
            yield break;
        }       

        //调用Armyctrl攻击   
        if (!armyCtrl.Attack(defenseArmy.OtherMapPintctrl, defenseArmy))
        {
            //如果不能攻击
            refresh();
            //yield return new WaitUntil(isArmycanAttack);
            //防御单位置空
            defenseArmy = null;
            yield break;           
        }
        else//如果能攻击
        {   
            Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "等待对方是否闪避...");
            Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡平面");
        }

        Button_Attack_Land.interactable = false;
        Button_Attack_Sky.interactable = false;
        refresh();
        //防御单位置空
        //defenseArmy = null;
    }

    /// <summary>
    /// 攻击飞行单位协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackSky()
    {
        //选择要攻击的地图点
        if (clickMapPointCtrl == null)
        {
            //refresh();
            yield return new WaitUntil(isclickMapPointCtrlnull);
            attackState();
        }

        //屏蔽查看属性
        if (clickMapPointCtrl != null)
        {
            if (clickMapPointCtrl.LandArmy != null)
            {
                OtherArmyCtrl otherArmyCtrl = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
                if (otherArmyCtrl != null)
                {
                    otherArmyCtrl.iscanShowStatePanel = false;
                }
            }
             
            if (clickMapPointCtrl.SkyArmy != null)
            {
                OtherArmyCtrl otherArmyCtrl = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
                if(otherArmyCtrl != null)
                {
                    otherArmyCtrl.iscanShowStatePanel = false;
                }
            }
                
        }

        if (clickMapPointCtrl.SkyArmy != null)
        {
            //如果攻击方的是陆地单位
            if (clickMapPointCtrl.SkyArmy != armyCtrl.ArmyPrefab)
            {
                //如果攻击的不是自己
                defenseArmy = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
            }
        }      

        if (defenseArmy == null)
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "攻击目标无效");
            refresh();
            yield break;
        }

        //调用Armyctrl攻击   
        if (!armyCtrl.Attack(defenseArmy.OtherMapPintctrl, defenseArmy))
        {
            //如果不能攻击
            refresh();
            //防御单位置空
            defenseArmy = null;
            yield break;
        }      
        else
        {
            //显示等待面板
            Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "等待对方是否闪避...");
            Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡平面");
        }     

        Button_Attack_Land.interactable = false;
        Button_Attack_Sky.interactable = false;

        refresh();
        //防御单位置空
        //defenseArmy = null;
    }
    /// <summary>
    /// 判断是否选择了地图点
    /// </summary>
    /// <returns></returns>
    private bool isclickMapPointCtrlnull()
    {
        if (clickMapPointCtrl != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断兵种是否可以攻击选择的地图点
    /// </summary>
    /// <returns></returns>
    private bool isArmycanAttack()
    {
        if(clickMapPointCtrl == null)
        {
            return false;
        }

        else if (armyCtrl.Attack(clickMapPointCtrl, defenseArmy))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断是否选择了攻击目标
    /// </summary>
    /// <returns></returns>
    private bool isSetDefenseArmyType()
    {
        if(defenseArmyType != -1)
        {
            return true;
        }
        return false;
    }

    private bool isSetDefneseArmy()
    {
        StartCoroutine(selectDenfenseArmy());
        if (defenseArmy != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断选择的地图点控制器是否为空
    /// </summary>
    /*private System.Func<bool> isclickMapPointCtrlnull =
        () =>
        {
            if(clickMapPointCtrl != null)
            {
                return true;
            }
            return false;
        };

    private System.Func<bool> isArmycanAttack =
        () =>
        {
            if(armyCtrl.Attack(clickMapPointCtrl, clickMapPointCtrl.gameObject.GetComponent<OtherArmyCtrl>()))
            {
                return true;
            }
            return false;
        };*/

    /// <summary>
    /// 获取点击的地图点控制器
    /// </summary>
    /// <returns></returns>
    private MapPointCtrl GetMapPointCtrl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapPoint"));
                if (isCollider)
                {
                    MapPointCtrl mapPointCtrl = hit.collider.GetComponent<MapPointCtrl>();
                    //StartCoroutine(selectDenfenseArmy());
                    return mapPointCtrl;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 选择防御兵种
    /// </summary>
    /// <returns></returns>
    private IEnumerator selectDenfenseArmy()
    {
        if(clickMapPointCtrl == null)
        {
            yield break;
        }
        if (armyCtrl.armyState.MoveType == ArmyMoveType.LAND && clickMapPointCtrl.LandArmy != null)
        {
            //如果攻击方是陆地单位
            if(clickMapPointCtrl.LandArmy != armyCtrl.ArmyPrefab)
            {
                //如果攻击的不是自己
                defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
            }        
        }
        else if (armyCtrl.armyState.MoveType == ArmyMoveType.SKY && clickMapPointCtrl != null && clickMapPointCtrl.LandArmy != null && clickMapPointCtrl.SkyArmy != null)
        {
            if(clickMapPointCtrl.SkyArmy != armyCtrl.ArmyPrefab)
            {
                //如果攻击方是飞行单位,且地图点上同时有陆地和飞行单位
                MapPointCtrl mapPointCtrl = clickMapPointCtrl;
                Dispatch(AreoCode.UI, UIEvent.SHOW_SELECT_ATTACK_PANEL, true);
                yield return new WaitUntil(isSetDefenseArmyType);
                //打开面板后会清空选择的地图点控制器
                clickMapPointCtrl = mapPointCtrl;

                if (defenseArmyType == ArmyMoveType.LAND)
                {
                    defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
                }
                else if (defenseArmyType == ArmyMoveType.SKY)
                {
                    defenseArmy = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
                }

                defenseArmyType = -1;
            } 
        }
        else if (armyCtrl.armyState.MoveType == ArmyMoveType.SKY && clickMapPointCtrl.LandArmy != null)
        {
            //如果攻击方位飞行单位，且地图点上只有陆地单位
            if(armyCtrl.ArmyPrefab != clickMapPointCtrl.SkyArmy)
            {
                defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
            }           
        }
        else if (armyCtrl.armyState.MoveType == ArmyMoveType.SKY && clickMapPointCtrl.SkyArmy != null)
        {
            //如果攻击方位飞行单位，且地图点上只有飞行单位
            if(armyCtrl.ArmyPrefab != clickMapPointCtrl.SkyArmy)
            {
                defenseArmy = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
            }  
        }
    }

    /// <summary>
    /// 复原状态
    /// </summary>
    private void refresh()
    {
        if (isAttack)
        {
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "设置普通光标");
            isAttack = false;       
        }

        //恢复颜色
        armyCtrl.setMappointCtrlColor(armyCtrl.canAttckPointCtrls);
        //解除屏蔽移动
        armyCtrl.iscanMove = true;
        //解除屏蔽查看属性
        if (defenseArmy !=null)
        {
            defenseArmy.GetComponent<OtherArmyCtrl>().iscanShowStatePanel = true;
        }
        //防御单位置空
        //defenseArmy = null;
    }

    /// <summary>
    /// 刷新菜单
    /// </summary>
   /* public void refreshMenu()
    {
        SetPanelActive(false);
        SetPanelActive(true);
    }*/
}

    

