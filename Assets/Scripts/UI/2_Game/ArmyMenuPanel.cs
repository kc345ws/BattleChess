using Protocol.Constants;
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
    private ArmyCtrl armyCtrl;//兵种控制器

    private MapPointCtrl attackMapPointCtrl;//要攻击的地图点控制器

    private MapPointCtrl clickMapPointCtrl;//点击的地图点控制器
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
            Button_Attack.enabled = false ;
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
            Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_ATTACK, "设置攻击光标");
            //选择要攻击的地图点
            if(clickMapPointCtrl == null)
            {
                //如果选择的地图点为空则递归调用
                attackBtnClicker();
            }
            //TODO 调用Armyctrl攻击   
            if(!armyCtrl.Attack(clickMapPointCtrl, clickMapPointCtrl.gameObject.GetComponent<OtherArmyCtrl>()))
            {
                //如果不能攻击则递归调用
                attackBtnClicker();
            }
            else
            {
                Dispatch(AreoCode.UI, UIEvent.CURSOR_SET_NORMAL, "设置普通光标");
            }
        }
    }

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
