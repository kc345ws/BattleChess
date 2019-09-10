using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CharacterEvent
{
    /// <summary>
    /// 初始化自己角色的手牌
    /// </summary>
    public const int INIT_MY_CARDLIST = 1000;
    public const int INIT_OTHER_CARDLIST = 1001;
    //public const int INIT_RIGHT_CARDLIST = 1002;

    //给自己的角色增加手牌牌
    public const int ADD_MY_CARDS = 1003;
    public const int ADD_OTHERT_CARDS = 1004;
    //public const int ADD_RIGHT_TABLECARDS = 1005;

    public const int DEAL_CARD = 1006;//角色出牌


    //移除手牌
    public const int REMOVE_MY_CARDS = 1007;
    public const int REMOVE_OTHER_CARDS = 1008;
    //public const int REMOVE_RIGHT_CARDS = 1009;

    //更新桌面的牌
    public const int UPDATE_SHOW_dESK = 1010;

    /// <summary>
    /// 选择指令卡
    /// </summary>
    public const int SELECT_ORDERCARD = 1011;

    /// <summary>
    /// 询问是否出闪避
    /// </summary>
    public const int INQUIRY_DEAL_DODGE = 1012;
    public const ushort RETURN_DEAL_DODGE_RESULT = 1013;

    /// <summary>
    /// 面板返回是否反击的结果
    /// </summary>
    public const ushort RETURN_DEAL_BACKATTACK_RESULT = 1014;

    /// <summary>
    /// 设置自己的兵种重叠选择结果
    /// </summary>
    public const ushort SET_MY_LAND_SKY = 1015;

    /// <summary>
    /// 选择非指令卡
    /// </summary>
    public const ushort SELECT_OTHERCARD = 1016;

    /// <summary>
    /// 对方使用攻击指令卡
    /// </summary>
    public const ushort OTHER_DEAL_ATTACK = 1017;
}

