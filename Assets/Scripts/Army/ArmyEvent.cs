using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ArmyEvent
{
    /// <summary>
    /// 给其他人添加兵种
    /// </summary>
    public const int ADD_OTHER_ARMY = 0;

    /// <summary>
    /// 多个兵种重叠时的选择结果
    /// </summary>
    public const int SET_MY_LAND_SKY = 1;
    public const int SET_OTHER_LAND_SKY = 2;

    /// <summary>
    /// 别人使用修养卡
    /// </summary>
    public const ushort OTHER_USE_REST = 3;

    /// <summary>
    /// 别人使用非指令卡
    /// </summary>
    public const ushort OTHER_USE_OTHERCARD = 4;

    /// <summary>
    /// 回合开始时刷新单位状态
    /// </summary>
    public const ushort TURN_REFRESH_ARMYSTATE = 5;


    /// <summary>
    /// 回合开始刷新单位技能
    /// </summary>
    public const ushort TURN_REFRESH_ARMYSKILL = 6;

    /// <summary>
    /// 单位死亡
    /// </summary>
    public const ushort ARMY_DEAD = 7;
}

