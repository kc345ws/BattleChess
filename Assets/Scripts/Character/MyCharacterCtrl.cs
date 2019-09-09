using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Constants;
using Protocol.Code;
using Protocol.Constants.Map;

/// <summary>
/// 自己角色的控制器
/// </summary>
public class MyCharacterCtrl : CharacterBase
{
    public static MyCharacterCtrl Instance;
    private MyCharacterCtrl() { }

    private List<CardDto> myCardList;//卡牌数据传输对象集合
    public List<CardCtrl> CardCtrllist { get; private set; }//卡牌控制器集合
    private Transform cardTransformParent;//卡牌的父物体
    private GameObject cardPrefab;//卡牌预设体

    private SocketMsg socketMsg;//套接字封装
    public CardCtrl LastSelectCard { get; private set; }//上一次选择的卡牌

    private CardCtrl dodgeArmyctrl;//闪避手牌
    private OtherArmyCtrl attackCtrl;//进行攻击的兵种
    private ArmyCtrl defenseCtrl;//需要闪避的兵种

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.INIT_MY_CARDLIST);
        //Bind(CharacterEvent.ADD_MY_TABLECARDS);
        Bind(CharacterEvent.DEAL_CARD);
        Bind(CharacterEvent.REMOVE_MY_CARDS);
        Bind(CharacterEvent.INQUIRY_DEAL_DODGE);
        Bind(CharacterEvent.RETURN_DEAL_DODGE_RESULT);
        Bind(CharacterEvent.RETURN_DEAL_BACKATTACK_RESULT);

        myCardList = new List<CardDto>();
        CardCtrllist = new List<CardCtrl>();
        cardTransformParent = transform.Find("CardPoint");
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card/MyCard");
        socketMsg = new SocketMsg();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case CharacterEvent.INIT_MY_CARDLIST:
                StartCoroutine(initPlayerCard(message as List<CardDto>));
                break;

            case CharacterEvent.ADD_MY_TABLECARDS:
                addTableCard(message as List<CardDto>);
                break;

            /*case CharacterEvent.DEAL_CARD:
                dealSelectedCard();
                break;*/

            case CharacterEvent.REMOVE_MY_CARDS:
                //removeSelectCard(message as List<CardDto>);
                removeSelectCard(message as CardDto);
                break;

            case CharacterEvent.INQUIRY_DEAL_DODGE:
                pcoessdealDodge(message as MapAttackDto);
                break;

            case CharacterEvent.RETURN_DEAL_DODGE_RESULT:
                processDodgeResult((bool)message);
                break;

            case CharacterEvent.RETURN_DEAL_BACKATTACK_RESULT:
                processBackAttackResult((bool)message);
                break;
        }
    }

    /// <summary>
    /// 是否有某一类型的卡牌
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool hasTypeCard(int type)
    {
        foreach (var item in CardCtrllist)
        {
            if (item.cardDto.Type == type)
            {
                return true;
            }
        }
        return false;
    }

    #region 反击
    /// <summary>
    /// 是否有某一卡牌
    /// </summary>
    /// <returns></returns>
    private bool hasCardType(ushort cardtype , ushort cardname)
    {
        foreach (var item in CardCtrllist)
        {
            if(item.cardDto.Type == cardtype && item.cardDto.Name == cardname)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 处理反击结果
    /// </summary>
    /// <param name="active"></param>
    private void processBackAttackResult(bool active)
    {
        if (active)//如果选择了反击
        {
            //减血
            attackCtrl.armyState.Hp -= defenseCtrl.armyState.Damage;
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, true);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        else
        {
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        //关闭箭头
        Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
        //关闭隐藏面板
        Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
        defenseCtrl = null;
        attackCtrl = null;
    }


    #endregion

    #region 闪避
    /// <summary>
    /// 处理出闪面板发来的消息
    /// </summary>
    /// <param name="active"></param>
    private void processDodgeResult(bool active)
    {
        if (active)//出闪
        {
            //移除手牌
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, dodgeArmyctrl.cardDto);
            defenseCtrl = null;
            attackCtrl = null;
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, true);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            //关闭箭头
            Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
            //关闭隐藏面板
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
            //发送反击消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        else//不出闪
        {
            //减血
            defenseCtrl.armyState.Hp -= attackCtrl.armyState.Damage;   
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

            if (hasCardType(CardType.ORDERCARD, OrderCardType.BACKATTACK) && defenseCtrl.armyState.Hp > 0)
            {
                //反击
                Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BACKATTACK_PANEL, "你的单位在敌人的攻击中存活下来了,是否进行反击?");
            }
            else
            {
                //关闭箭头
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
                //关闭隐藏面板
                Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
                //发送反击消息
                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            }
            defenseCtrl = null;
            attackCtrl = null;
        }
        
    }

    /// <summary>
    /// 是否出闪避
    /// </summary>
    private void pcoessdealDodge(MapAttackDto mapAttackDto)
    {
        bool hasDodge = false;//手牌中是否有闪避
        foreach (var item in CardCtrllist)
        {
            if (item.cardDto.Type == CardType.ORDERCARD && item.cardDto.Name == OrderCardType.DODGE)
            {
                hasDodge = true;
                dodgeArmyctrl = item;
                break;
            }
        }

        //ArmyCtrl defenseCtrl;
        //OtherArmyCtrl attackCtrl;
        getArmy(out defenseCtrl, out attackCtrl, mapAttackDto);

        if (defenseCtrl == null || attackCtrl == null)
        {
            return;
        }

        if (!hasDodge || defenseCtrl.armyState.Class == ArmyClassType.Ordinary)
        {
            //如果没有闪避或是普通兵种直接减血
            defenseCtrl.armyState.Hp -= attackCtrl.armyState.Damage;
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            //关闭箭头
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
            //关闭隐藏面板
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
            //发送提示
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "你方(" + defenseCtrl.armyState.Position.X + "," + defenseCtrl.armyState.Position.Z + ")" + "单位被攻击");

            //反击
            if (hasCardType(CardType.ORDERCARD, OrderCardType.BACKATTACK) && defenseCtrl.armyState.Hp > 0)
            {
                //反击
                Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示隐藏平面");
                Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BACKATTACK_PANEL, "你的单位在敌人的攻击中存活下来了,是否进行反击?");
            }
            else
            {
                //关闭箭头
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
                //关闭隐藏面板
                Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
                //发送反击消息
                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            }
            return;
        }


        //显示箭头
        Vector3 pos = new Vector3(defenseCtrl.armyState.Position.X, 1, defenseCtrl.armyState.Position.Z);
        Dispatch(AreoCode.UI, UIEvent.SHOW_ATTACK_ARROW, pos);
        //显示出闪避面板
        string str;
        if (defenseCtrl.armyState.CanFly)
        {
            str = "你方位于箭头所指处的飞行单位被攻击，是否进行闪避？";
        }
        else
        {
            str = "你方位于箭头所指处的陆地单位被攻击，是否进行闪避？";
        }
        Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_DODGE_PANEL, str);
        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示隐藏平面");
    }

    /// <summary>
    /// 获取防御和攻击兵种的信息
    /// </summary>
    private void getArmy(out ArmyCtrl defenseCtrl, out OtherArmyCtrl attackCtrl, MapAttackDto mapAttackDto)
    {
        //镜像对称
        int totalX = 12;
        int totalZ = 8;

        MapPoint attackpoint = new MapPoint(totalX - mapAttackDto.AttacklMapPoint.X, totalZ - mapAttackDto.AttacklMapPoint.Z);
        MapPoint defensepoint = new MapPoint(totalX - mapAttackDto.DefenseMapPoint.X, totalZ - mapAttackDto.DefenseMapPoint.Z); ;
        bool attackcanfly = mapAttackDto.AttackCanFly;
        bool defensecanfly = mapAttackDto.DefenseCanFly;
        MapPointCtrl attackPointCtrl = null;
        MapPointCtrl defensePointCtrl = null;
        //OtherArmyCtrl attackCtrl;
        //ArmyCtrl defenseCtrl;

        foreach (var item in MapManager.mapPointCtrls)
        {
            if (item.mapPoint.X == attackpoint.X && item.mapPoint.Z == attackpoint.Z)
            {
                attackPointCtrl = item;
            }
            else if (item.mapPoint.X == defensepoint.X && item.mapPoint.Z == defensepoint.Z)
            {
                defensePointCtrl = item;
            }

            if (attackPointCtrl != null && defensePointCtrl != null)
            {
                break;
            }
        }

        if (!attackcanfly)
        {
            attackCtrl = attackPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
        }
        else
        {
            attackCtrl = attackPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
        }

        if (!defensecanfly)
        {
            defenseCtrl = defensePointCtrl.LandArmy.GetComponent<ArmyCtrl>();
        }
        else
        {
            defenseCtrl = defensePointCtrl.SkyArmy.GetComponent<ArmyCtrl>();
        }
    }
    #endregion

    #region 卡牌方法
    /// <summary>
    /// 每次只有一张牌被选择
    /// </summary>
    /// <param name="selectCard"></param>
    /// <returns></returns>
    public bool IsCanSelece(CardCtrl selectCard)
    {
        if (LastSelectCard == null)
        {
            //第一次点击
            LastSelectCard = selectCard;
            if (selectCard.cardDto.Type == CardType.ARMYCARD)
            {
                Dispatch(AreoCode.MAP, MapEvent.SELECT_ARMYCARD, selectCard.cardDto);
                //如果选中的是兵种卡
            }
            else if (selectCard.cardDto.Type == CardType.ORDERCARD)
            {
                //如果选中指令卡
                Dispatch(AreoCode.CHARACTER, CharacterEvent.SELECT_ORDERCARD, selectCard);
            }else if(selectCard.cardDto.Type == CardType.OTHERCARD)
            {
                //如果选中非指令卡
                Dispatch(AreoCode.CHARACTER, CharacterEvent.SELECT_OTHERCARD, selectCard);
            }

            return true;
        }
        else if (LastSelectCard != selectCard)
        {
            //和上次点击的不一样,取消选中上一次选中的牌，选中当前的牌
            LastSelectCard.transform.localPosition -= new Vector3(1f, 0, 0);
            LastSelectCard.IsSelected = false;

            LastSelectCard = selectCard;
            if (selectCard.cardDto.Type == CardType.ARMYCARD)
            {
                Dispatch(AreoCode.MAP, MapEvent.SELECT_ARMYCARD, selectCard.cardDto);
                //如果选中的是兵种卡
            }
            else if (selectCard.cardDto.Type == CardType.ORDERCARD)
            {
                //如果选中指令卡
                Dispatch(AreoCode.CHARACTER, CharacterEvent.SELECT_ORDERCARD, selectCard);
            }

            return true;
        }
        else
        {
            //和上次点击的一样，取消选中
            LastSelectCard = null;
            Dispatch(AreoCode.MAP, MapEvent.CANCEL_SELECT_ARMYCARD, null);
            Dispatch(AreoCode.CHARACTER, CharacterEvent.SELECT_ORDERCARD, null);
            return false;
        }
    }

    /// <summary>
    /// 出所选中的牌
    /// </summary>
    /*private void dealSelectedCard()
    {
        DealDto dealDto = new DealDto(GameModles.Instance.userDto.ID, getSelectedCard());
        if (dealDto.isRegular)
        {          
            //socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_CREQ, dealDto);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        else
        {
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "出牌不合法");
        }
    }*/

    /// <summary>
    /// 获取所选中的牌
    /// </summary>
    /// <returns></returns>
    /*private List<CardDto> getSelectedCard()
    {
        List<CardDto> selectedCard = new List<CardDto>();
        foreach (var item in CardCtrllist)
        {
            if(item.IsSelected == true)
            {
                selectedCard.Add(item.cardDto);
            }
        }
        return selectedCard;
    }*/

    /// <summary>
    /// 出牌成功时移除手牌
    /// </summary>
    private void removeSelectCard(CardDto removeCard)
    {
        //剩余手牌
        //List<CardDto> RestmyCardList = new List<CardDto>(myCardList);
        List<CardCtrl> RestCardCtrllist = new List<CardCtrl>(CardCtrllist);

        //int removeindex = -1;
        for (int i = 0; i < RestCardCtrllist.Count; i++)
        {
            if (RestCardCtrllist[i].cardDto == removeCard)
            {
                RestCardCtrllist.RemoveAt(i);
                //removeindex = i;
                break;
            }
        }
        myCardList.Remove(removeCard);

        int index = 0;
        foreach (var item in RestCardCtrllist)
        {
            CardCtrllist[index].Init(item.cardDto, true, index);

            index++;

            if (index == RestCardCtrllist.Count)
            {
                break;
            }
        }

        for (int i = index; i < CardCtrllist.Count; i++)
        {
            CardCtrllist[i].IsSelected = false;
            Destroy(CardCtrllist[i].gameObject);
            CardCtrllist.RemoveAt(i);
        }

        //CardCtrllist = RestCardCtrllist;


        LastSelectCard = null;
        Dispatch(AreoCode.MAP, MapEvent.CANCEL_SELECT_ARMYCARD, null);//清除上次选择的卡牌
    }

    /// <summary>
    /// 停顿使发牌有动画感
    /// </summary>
    /// <returns></returns>
    private IEnumerator initPlayerCard(List<CardDto> cardList)
    {
        for (int i = 0; i < 14; i++)
        {
            createCard(cardList[i], i);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void createCard(CardDto cardDto, int index)
    {
        GameObject card = GameObject.Instantiate(cardPrefab, cardTransformParent);
        //card.transform.localPosition = new Vector2(index * 1f, 0);
        card.transform.localPosition = new Vector3(0, index * 0.01f, index * -1f);
        //card.name = cardDto.Name;
        CardCtrl cardCtrl = card.GetComponent<CardCtrl>();
        cardCtrl.CardMouseDownEvent = IsCanSelece;
        cardCtrl.Init(cardDto, true, index);

        myCardList.Add(cardDto);
        CardCtrllist.Add(cardCtrl);
    }

    private void addTableCard(List<CardDto> cardlist)
    {
        int index = myCardList.Count;
        foreach (var item in cardlist)
        {
            myCardList.Add(item);
        }
        //CardWeight.SortCard(ref myCardList);

        //复用先前创建的牌
        for (int i = 0; i < 17; i++)
        {
            CardCtrllist[i].gameObject.SetActive(true);
            CardCtrllist[i].Init(myCardList[i], true, i);
        }

        for (int i = index; i < 20; i++)
        {
            GameObject card = GameObject.Instantiate(cardPrefab, cardTransformParent);
            card.transform.localPosition = new Vector2(index * 0.2f, 0);
            //card.name = myCardList[i].Name;
            CardCtrl cardCtrl = card.GetComponent<CardCtrl>();
            cardCtrl.Init(myCardList[i], true, index);

            CardCtrllist.Add(cardCtrl);
            index++;
        }
    }
    #endregion

}
