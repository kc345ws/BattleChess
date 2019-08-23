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

    /// <summary>
    /// 移动地图上我的的兵种
    /// </summary>
    public const int MOVE_MY_ARMY = 3;

    /// <summary>
    /// 移动地图上别人的兵种
    /// </summary>
    public const int MOVE_OTHER_ARMY = 4;
}
