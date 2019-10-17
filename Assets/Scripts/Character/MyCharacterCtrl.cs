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

    private CardCtrl dodgeCardctrl;//闪避手牌
    private OtherArmyCtrl attackCtrl;//进行攻击的兵种
    private ArmyCtrl defenseCtrl;//需要闪避的兵种
    private CardCtrl backAttackCardCtrl;//反击手牌

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.INIT_MY_CARDLIST);
        Bind(CharacterEvent.ADD_MY_CARDS);
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

            case CharacterEvent.ADD_MY_CARDS:
                addCard(message as List<CardDto>);
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
    private CardCtrl hasCardType(ushort cardtype , ushort cardname)
    {
        foreach (var item in CardCtrllist)
        {
            if(item.cardDto.Type == cardtype && item.cardDto.Name == cardname)
            {
                return item;
            }
        }
        return null;
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
            //移除反击手牌
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, backAttackCardCtrl.cardDto);
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

        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
        Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "");//显示等待面板
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
            Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, dodgeCardctrl.cardDto);
            //defenseCtrl = null;
            //attackCtrl = null;
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, true);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            //关闭箭头
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
            //关闭隐藏面板
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
            //发送反击消息
            //socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
            //Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        else//不出闪
        {
            //减血
            defenseCtrl.armyState.Hp -= attackCtrl.armyState.Damage;   
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);


            if(defenseCtrl.armyState.Race == RaceType.ORC &&
                defenseCtrl.armyState.Name == OrcArmyCardType.Pangolin)
            {
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "穿山甲发动反击背刺");
                //如果是兽族穿山甲
                attackCtrl.armyState.Hp --;//受到技能伤害
                SkillDto skillDto = new SkillDto(defenseCtrl.armyState.Race, defenseCtrl.armyState.Name
                    , OrcArmyCardType.Pangolin, attackCtrl.armyState.Position);

                socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_USE_SKILL_CREQ, skillDto);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            }
        }

        
        //是否反击
        backAttackCardCtrl = hasCardType(CardType.ORDERCARD, OrderCardType.BACKATTACK);
        if (backAttackCardCtrl!=null && defenseCtrl.armyState.Hp > 0)
        {
            //反击
            Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BACKATTACK_PANEL, "你的单位在敌人的攻击中存活下来了,是否进行反击?");
        }
        else//不反击
        {
            //关闭箭头
            Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
            //关闭隐藏面板
            Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
            //发送反击消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

            defenseCtrl = null;
            attackCtrl = null;

            Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
            Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "对方回合");
        }
        //defenseCtrl = null;
        //attackCtrl = null;
    }

    /// <summary>
    /// 询问是我方否出闪避
    /// </summary>
    private void pcoessdealDodge(MapAttackDto mapAttackDto)
    {
        if(mapAttackDto == null)
        {
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            return;
        }

        bool hasDodge = false;//手牌中是否有闪避

        dodgeCardctrl = hasCardType(CardType.ORDERCARD, OrderCardType.DODGE);
        if(dodgeCardctrl != null)
        {
            hasDodge = true;
        }
        else
        {
            hasDodge = false;
        }

        //获取攻击和防御的兵种
        getArmy(out defenseCtrl, out attackCtrl, mapAttackDto);

        if (defenseCtrl == null || attackCtrl == null)
        {
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            return;
        }


        //如果没有闪避或是普通兵种直接减血
        if (!hasDodge || defenseCtrl.armyState.Class == ArmyClassType.Ordinary)
        {
            
            defenseCtrl.armyState.Hp -= attackCtrl.armyState.Damage;
            //发送消息
            socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_DODGE_CREQ, false);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            //关闭箭头
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
            //关闭隐藏面板
            //Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
            //发送提示
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "我方(" + defenseCtrl.armyState.Position.X + "," + defenseCtrl.armyState.Position.Z + ")" + "单位被攻击");

            if (defenseCtrl.armyState.Race == RaceType.ORC &&
                defenseCtrl.armyState.Name == OrcArmyCardType.Pangolin)
            {
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "穿山甲发动反击背刺");
                //如果是兽族穿山甲
                attackCtrl.armyState.Hp--;//受到技能伤害
                SkillDto skillDto = new SkillDto(defenseCtrl.armyState.Race, defenseCtrl.armyState.Name
                    , OrcArmyCardType.Pangolin, attackCtrl.armyState.Position);

                socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_USE_SKILL_CREQ, skillDto);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            }


            if (defenseCtrl.armyState.Class == ArmyClassType.Ordinary)
            {
                //发送反击消息
                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                return;//普通兵种不能反击
            }

            //其他阶级兵种可以反击
            backAttackCardCtrl = hasCardType(CardType.ORDERCARD, OrderCardType.BACKATTACK);
            if (backAttackCardCtrl !=null && defenseCtrl.armyState.Hp > 0)
            {
                //反击
                Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡平面");
                Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_BACKATTACK_PANEL, "你的单位在敌人的攻击中存活下来了,是否进行反击?");
            }
            else//不反击
            {
                //关闭箭头
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ATTACK_ARROW, "关闭箭头");
                //关闭隐藏面板
                Dispatch(AreoCode.UI, UIEvent.CLOSE_HIDE_PLANE, "关闭隐藏面板");
                //发送反击消息
                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_BACKATTACK_CREQ, false);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

                defenseCtrl = null;
                attackCtrl = null;

                Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡面板");
                Dispatch(AreoCode.UI, UIEvent.SHOW_WAIT_PANEL, "");
            }
            return;
        }


        //有闪避且被攻击单位为非普通阶级单位
        //显示箭头
        Vector3 pos = new Vector3(defenseCtrl.armyState.Position.X, 1, defenseCtrl.armyState.Position.Z);
        Dispatch(AreoCode.UI, UIEvent.SHOW_ATTACK_ARROW, pos);
        //显示出闪避面板
        string str = "";
        if (defenseCtrl.armyState.MoveType == ArmyMoveType.SKY)
        {
            str = "我方位于箭头所指处的飞行单位被攻击，是否进行闪避？";
        }
        else if(defenseCtrl.armyState.MoveType == ArmyMoveType.LAND)
        {
            str = "我方位于箭头所指处的陆地单位被攻击，是否进行闪避？";
        }

        Dispatch(AreoCode.UI, UIEvent.SHOW_DEAL_DODGE_PANEL, str);
        Dispatch(AreoCode.UI, UIEvent.SHOW_HIDE_PLANE, "显示遮挡平面");
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
        int attackmovetype = mapAttackDto.AttackMoveType;
        int defensemovetype = mapAttackDto.DefenseMoveType;
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

        if (attackmovetype == ArmyMoveType.LAND && attackPointCtrl.LandArmy != null)
        {
            attackCtrl = attackPointCtrl.LandArmy.GetComponent<OtherArmyCtrl>();
        }
        else if(attackmovetype == ArmyMoveType.SKY && attackPointCtrl.SkyArmy !=null)
        {
            attackCtrl = attackPointCtrl.SkyArmy.GetComponent<OtherArmyCtrl>();
        }
        else
        {
            attackCtrl = null;
        }

        if (defensemovetype == ArmyMoveType.LAND && defensePointCtrl.LandArmy!=null)
        {
            defenseCtrl = defensePointCtrl.LandArmy.GetComponent<ArmyCtrl>();
        }
        else if(defensemovetype == ArmyMoveType.SKY && defensePointCtrl.SkyArmy!=null)
        {
            defenseCtrl = defensePointCtrl.SkyArmy.GetComponent<ArmyCtrl>();
        }
        else
        {
            defenseCtrl = null;
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
                foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                {
                    item.canBeSeletced = false ;//屏蔽选择
                }
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

            LastSelectCard.transform.localPosition = LastSelectCard.originlocation;

            
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
                if (LastSelectCard.cardDto.Type == CardType.ARMYCARD)
                {
                    foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                    {
                        item.canBeSeletced = true;//取消屏蔽选择
                    }
                }
            }
            else
            {
                //选择非指令卡
                if (LastSelectCard.cardDto.Type == CardType.ARMYCARD)
                {
                    foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                    {
                        item.canBeSeletced = true;//取消屏蔽选择
                    }
                }
            }

            return true;
        }
        else
        {
            //和上次点击的一样，取消选中
            if(LastSelectCard.cardDto.Type == CardType.ARMYCARD)
            {
                foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                {
                    item.canBeSeletced = true;//取消屏蔽选择
                }
            }
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

    /// <summary>
    /// 初始阶段创建手牌
    /// </summary>
    /// <param name="cardDto"></param>
    /// <param name="index"></param>
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

    /// <summary>
    /// 每回合增加手牌
    /// </summary>
    /// <param name="cardlist"></param>
    private void addCard(List<CardDto> cardlist)
    {
        int index = myCardList.Count;
        foreach (var item in cardlist)
        {
            myCardList.Add(item);
        }
        //对手牌进行排序
        //CardWeight.SortCard(ref myCardList);

        //复用先前创建的牌
        for (int i = 0; i < index; i++)
        {
            CardCtrllist[i].gameObject.SetActive(true);
            CardCtrllist[i].Init(myCardList[i], true, i);
        }

        for (int i = index; i < myCardList.Count; i++)
        {
            GameObject card = GameObject.Instantiate(cardPrefab, cardTransformParent);
            card.transform.localPosition = new Vector3(0, index * 0.01f, index * -1f);
            //card.name = myCardList[i].Name;
            CardCtrl cardCtrl = card.GetComponent<CardCtrl>();

            cardCtrl.CardMouseDownEvent = IsCanSelece;
            cardCtrl.Init(myCardList[i], true, index);

            CardCtrllist.Add(cardCtrl);
            index++;
        }
    }
    #endregion

}
