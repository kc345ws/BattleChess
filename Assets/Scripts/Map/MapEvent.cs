using UnityEngine;
using System.Collections;

public class MapEvent : MonoBehaviour
{
    /// <summary>
    /// 选中兵种牌
    /// </summary>
    public const int SELECT_ARMYCARD = 0;

    /// <summary>
    /// 取消选中兵种牌
    /// </summary>
    public const int CANCEL_SELECT_ARMYCARD = 1;

    /// <summary>
    /// 设置别人的兵种
    /// </summary>
    public const int SET_OTHER_ARMY = 2;
}
