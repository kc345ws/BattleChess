using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Constants;
using Protocol.Code;

/// <summary>
/// 自己角色的控制器
/// </summary>
public class MyCharacterCtrl : CharacterBase
{
    private List<CardDto> myCardList;
    private List<CardCtrl> CardCtrllist;//卡牌控制器集合
    private Transform cardTransformParent;//卡牌的父物体
    private GameObject cardPrefab;//卡牌预设体

    private SocketMsg socketMsg;
    public CardCtrl LastSelectCard { get; private set; }//上一次选择的卡牌
    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.INIT_MY_CARDLIST);
        //Bind(CharacterEvent.ADD_MY_TABLECARDS);
        Bind(CharacterEvent.DEAL_CARD);
        Bind(CharacterEvent.REMOVE_MY_CARDS);

        myCardList = new List<CardDto>();
        CardCtrllist = new List<CardCtrl>();
        cardTransformParent = transform.Find("CardPoint");
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card/MyCard");
        socketMsg = new SocketMsg();
    }

    /// <summary>
    /// 每次只有一张牌被选择
    /// </summary>
    /// <param name="selectCard"></param>
    /// <returns></returns>
    public bool IsCanSelece(CardCtrl selectCard)
    {
        if(LastSelectCard == null)
        {
            //第一次点击
            LastSelectCard = selectCard;
            if(selectCard.cardDto.Type == CardType.ARMYCARD)
            {
                Dispatch(AreoCode.MAP, MapEvent.SELECT_ARMYCARD, selectCard.cardDto);
                //如果选中的是兵种开
            }
            
            return true;
        }
        else if(LastSelectCard != selectCard)
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
            return true;
        }
        else
        {
            //和上次点击的一样，取消选中
            LastSelectCard = null;
            Dispatch(AreoCode.MAP, MapEvent.CANCEL_SELECT_ARMYCARD, null);
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
        for(int i = 0; i < RestCardCtrllist.Count; i++)
        {
            if(RestCardCtrllist[i].cardDto == removeCard)
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

            if(index == RestCardCtrllist.Count)
            {
                break;
            }
        }

        for(int i = index; i < CardCtrllist.Count; i++)
        {
            CardCtrllist[i].IsSelected = false;
            Destroy(CardCtrllist[i].gameObject);
            CardCtrllist.RemoveAt(i);
        }

        //CardCtrllist = RestCardCtrllist;
        

        LastSelectCard = null;
        Dispatch(AreoCode.MAP, MapEvent.CANCEL_SELECT_ARMYCARD, null);//清除上次选择的卡牌

        //int index = 0;

        /*if(restcardList.Count == 0)
        {
            return;//如果剩余手牌为0
        }*/

        /*foreach (var item in CardCtrllist)
        {
            //CardWeight.SortCard(ref myCardList);
            item.Init(restcardList[index], true, index);
            index++;
            
            if(index == restcardList.Count)
            {
                break;
            }
        }*/


        /*for (int i = index; i < CardCtrllist.Count; i++)
        {
            //CardWeight.SortCard(ref myCardList);
            if (CardCtrllist[i]!=null && CardCtrllist[i].gameObject != null)
            {
                CardCtrllist[i].IsSelected = false;
                Destroy(CardCtrllist[i].gameObject);//销毁剩余卡牌之后的卡牌
            }           
        }*/
    }

    /// <summary>
    /// 停顿使发牌有动画感
    /// </summary>
    /// <returns></returns>
    private IEnumerator initPlayerCard(List<CardDto> cardList)
    {
        for(int i = 0; i < 14; i++)
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
        for(int i = 0; i < 17; i++)
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

    private void Awake()
    {     
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
        }
    }

    // Update is called once per frame
    void Update()
    {
         
    }
}
