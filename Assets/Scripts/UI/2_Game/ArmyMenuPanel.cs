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
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ARMY_MENU_PANEL);
        Bind(UIEvent.CLOSE_ARMY_MENU_PANEL);

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
            Button_Attack.enabled = true;
        }
        else
        {
            Button_Attack.enabled = false;
        }
        SetPanelActive(true);
    }

    private void closeBtnClicker()
    {
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
        //选择要攻击的地图点
        if (clickMapPointCtrl == null)
        {
            yield return new WaitUntil(isclickMapPointCtrlnull);
        }
        //调用Armyctrl攻击   
        if (!armyCtrl.Attack(clickMapPointCtrl, clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>()))
        {
            //如果不能攻击
            yield return new WaitUntil(isArmycanAttack);

            //屏蔽查看属性
            clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>().iscanShowStatePanel = false;

            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "设置普通光标");
            isAttack = false;
        }
    }

    private bool isclickMapPointCtrlnull()
    {
        if (clickMapPointCtrl != null)
        {
            return true;
        }
        return false;
    }

    private bool isArmycanAttack()
    {
        if(clickMapPointCtrl == null)
        {
            return false;
        }

        else if (armyCtrl.Attack(clickMapPointCtrl, clickMapPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>()))
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
}

    

