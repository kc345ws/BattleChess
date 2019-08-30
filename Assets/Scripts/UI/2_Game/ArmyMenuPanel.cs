using Protocol.Constants;
using Protocol.Constants.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmyMenuPanel : UIBase
{
    private Button Button_State;
    private Button Button_Close;
    private Button Button_Attack;
    private Button Button_Dodge;
    private Button Button_Turn;
    private Button Button_Skill;

    //private ArmyCardBase armyState;
    private static ArmyCtrl armyCtrl;//兵种控制器

    private MapPointCtrl attackMapPointCtrl;//要攻击的地图点控制器

    private static MapPointCtrl clickMapPointCtrl;//点击的地图点控制器

    private bool isAttack = false;//是否处于攻击状态

    private int defenseArmyType = -1;//进行防御的兵种类型

    private OtherArmyCtrl defenseArmy = null;//进行防御的兵种控制器

    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ARMY_MENU_PANEL);
        Bind(UIEvent.CLOSE_ARMY_MENU_PANEL);
        Bind(UIEvent.SET_SELECK_ATTACK);

        Button_State = transform.Find("Button_State").GetComponent<Button>();
        Button_Close = transform.Find("Button_Close").GetComponent<Button>();
        Button_Attack = transform.Find("Button_Attack").GetComponent<Button>();
        Button_Dodge = transform.Find("Button_Dodge").GetComponent<Button>();
        Button_Turn = transform.Find("Button_Turn").GetComponent<Button>();
        Button_Skill = transform.Find("Button_Skill").GetComponent<Button>();

        Button_State.onClick.AddListener(stateBtnClicker);
        Button_Close.onClick.AddListener(closeBtnClicker);
        Button_Attack.onClick.AddListener(attackBtnClicker);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        clickMapPointCtrl = GetMapPointCtrl();
    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.SHOW_ARMY_MENU_PANEL:
                //armyState = message as ArmyCardBase;
                armyCtrl = message as ArmyCtrl;
                //SetPanelActive(true);
                processShowMenuPanel();
                break;

            case UIEvent.CLOSE_ARMY_MENU_PANEL:
                SetPanelActive(false);
                break;

            case UIEvent.SET_SELECK_ATTACK:
                defenseArmyType = (int)message;
                break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Button_State.onClick.RemoveAllListeners();
        Button_Close.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 处理显示面板事件
    /// </summary>
    private void processShowMenuPanel()
    {
        if (armyCtrl.canAttack && !armyCtrl.isAttack)
        {
            //如果兵种能攻击且没有攻击过
            //Button_Attack.enabled = true;
            Button_Attack.interactable = true;
        }
        else
        {
            Button_Attack.interactable = false;
        }
        SetPanelActive(true);
    }

    private void closeBtnClicker()
    {
        refresh();//复原状态

        SetPanelActive(false);
    }

    private void stateBtnClicker()
    {
        Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_STATE_PANEL, armyCtrl.armyState);
    }

    /// <summary>
    /// 攻击按钮
    /// </summary>
    private void attackBtnClicker()
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
            armyCtrl.iscanMove = false;
            
            /*
            //选择要攻击的地图点
            if (clickMapPointCtrl == null)
            {
                //如果选择的地图点为空则递归调用
                attackBtnClicker();
            }
            //调用Armyctrl攻击   
            if (!armyCtrl.Attack(clickMapPointCtrl, clickMapPointCtrl.gameObject.GetComponent<OtherArmyCtrl>()))
            {
                //如果不能攻击则递归调用
                attackBtnClicker();
            }*/
            StartCoroutine(Attack());
           // else
            //{
                
            //}
        }
    }

    private IEnumerator Attack()
    {
        //OtherArmyCtrl defenseArmy = null;//进行防御的兵种

        //选择要攻击的地图点
        if (clickMapPointCtrl == null)
        {
            yield return new WaitUntil(isclickMapPointCtrlnull);
        }

        if (!armyCtrl.armyState.CanFly && clickMapPointCtrl.LandArmy !=null)
        {
            //如果攻击方是陆地单位
            defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
        }
        else if (armyCtrl.armyState.CanFly && clickMapPointCtrl!=null&&clickMapPointCtrl.LandArmy != null && clickMapPointCtrl.SkyArmy!=null)
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
            else if(defenseArmyType == ArmyMoveType.SKY)
            {
                defenseArmy = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
            }
        }
        else if(armyCtrl.armyState.CanFly && clickMapPointCtrl.LandArmy != null)
        {
            //如果攻击方位飞行单位，且地图点上只有陆地单位
            defenseArmy = clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
        }
        else if(armyCtrl.armyState.CanFly && clickMapPointCtrl.SkyArmy != null)
        {
            //如果攻击方位飞行单位，且地图点上只有飞行单位
            defenseArmy = clickMapPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
        }

        //屏蔽查看属性
        if (defenseArmy != null)
        {
            defenseArmy.GetComponent<OtherArmyCtrl>().iscanShowStatePanel = false;
        }

        //调用Armyctrl攻击   
        if (!armyCtrl.Attack(clickMapPointCtrl, defenseArmy))
        {
            //如果不能攻击
            yield return new WaitUntil(isArmycanAttack);

            
            //clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>().iscanShowStatePanel = false;

            

            //Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "设置普通光标");
            //isAttack = false;
            
        }

        //对方减血
        defenseArmy.armyState.Hp -= armyCtrl.armyState.Damage;

        Button_Attack.interactable = false;
        
        refresh();
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
                    return mapPointCtrl;
                }
            }
        }
        return null;
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

        //解除屏蔽移动
        armyCtrl.iscanMove = true;
        //解除屏蔽查看属性
        if (defenseArmy !=null)
        {
            defenseArmy.GetComponent<OtherArmyCtrl>().iscanShowStatePanel = true;
        }
        //防御单位置空
        defenseArmy = null;
    }
}

    

