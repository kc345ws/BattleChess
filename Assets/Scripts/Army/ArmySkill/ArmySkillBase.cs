using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Protocol.Dto.Fight;

/// <summary>
/// 兵种技能基类
/// </summary>
public class ArmySkillBase :ArmyBase
{
    /// <summary>
    /// 该单位的控制器
    /// </summary>
    /// 
    public ArmyCtrl armyCtrl;
    /// <summary>
    /// 是否每局都执行
    /// </summary>
    public bool canPerTurn { get; set; }

    /// <summary>
    /// 是否是被动技能
    /// </summary>
    public bool isPassive { get; set; }

    /// <summary>
    /// 是否需要其他单位死亡
    /// </summary>
    public bool isNeedOtherDead { get; set; }

    /// <summary>
    /// 本回合是否使用过
    /// </summary>
    public bool isUsed { get; set; }

    /// <summary>
    /// 是否绑定了技能
    /// </summary>
    public bool isBind { get; set; }

    public SkillDto skillDto;
    public SocketMsg socketMsg;

    public ArmySkillBase()
    {
        canPerTurn = false;
        isPassive = false;
        isNeedOtherDead = false;
        isUsed = false;
        isBind = false;
        skillDto = new SkillDto();
        socketMsg = new SocketMsg();
    }

    public void SetArmyCtrl(ref ArmyCtrl armyCtrl)
    {
        this.armyCtrl = armyCtrl;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

     public virtual void UseSkill() { }
     public virtual void TurnRefrsh() { }
     public virtual void ProcessOtherDead() { }//处理其他单位死亡

    /// <summary>
    /// 获得鼠标左键点击兵种控制器
    /// </summary>
    public ArmyCtrl getArmyCtrlByMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MyArmy"));
                if (isCollider)
                {
                    //selectArmyCtrl = hit.collider.gameObject.GetComponent<ArmyCtrl>();

                    return hit.collider.gameObject.GetComponent<ArmyCtrl>();
                }
            }
        }
        return null;
    }

}
