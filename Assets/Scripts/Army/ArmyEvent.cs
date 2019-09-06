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

    
}

