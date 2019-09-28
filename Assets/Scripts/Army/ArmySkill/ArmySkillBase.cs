using UnityEngine;
using System.Collections;

/// <summary>
/// 兵种技能基类
/// </summary>
public class ArmySkillBase :ArmyBase
{


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

    public ArmySkillBase()
    {
        canPerTurn = false;
        isPassive = false;
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

}
