using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskCtrl : CharacterBase
{
    private List<CardDto> deskCardList;
    private List<CardCtrl> deskCardCtrllist;//卡牌控制器集合
    private Transform cardTransformParent;//卡牌的父物体
    private GameObject cardPrefab;

    private SocketMsg socketMsg;
    private int LastNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.UPDATE_SHOW_dESK);

        deskCardList = new List<CardDto>();
        deskCardCtrllist = new List<CardCtrl>();
        cardTransformParent = transform.Find("CardPoint");
        cardPrefab = Resources.Load<GameObject>("Card/DeskCard");
        socketMsg = new SocketMsg();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case CharacterEvent.UPDATE_SHOW_dESK:
                //updateShowDesk(message as List<CardDto>);

                break;
        }
    }

    private void updateShowDesk(List<CardDto> cardlist)
    {
        if(cardlist.Count > LastNum)
        {
            //现在比原来多
            int index = deskCardCtrllist.Count;

            //复用先前创建的牌
            for (int i = 0; i < index; i++)
            {
                deskCardCtrllist[i].gameObject.SetActive(true);
                deskCardCtrllist[i].Init(cardlist[i], true, i);
            }

            //创建新的牌
            for (int i = index; i < cardlist.Count; i++)
            {
                GameObject card = GameObject.Instantiate(cardPrefab, cardTransformParent);
                card.transform.localPosition = new Vector2(i * 0.08f, 0);
                //card.name = cardlist[i].Name;
                CardCtrl cardCtrl = card.GetComponent<CardCtrl>();
                cardCtrl.Init(cardlist[i], true, i);

                deskCardCtrllist.Add(cardCtrl);
                index++;
            }
        }
        else
        {
            //现在比原来少需要隐藏
            int index = 0;

            foreach (var item in deskCardCtrllist)
            {
                item.Init(cardlist[index], true, index);
                //deskCardCtrllist[index].name = cardlist[index].Name;
                index++;

                if (index == cardlist.Count)
                {
                    break;
                }
            }

            for (int i = index; i < deskCardCtrllist.Count; i++)
            {
                if (deskCardCtrllist[i]!=null && deskCardCtrllist[i].gameObject != null)
                {
                    deskCardCtrllist[i].IsSelected = false;
                    //Destroy(deskCardCtrllist[i].gameObject);//销毁剩余卡牌之后的卡牌
                    deskCardCtrllist[i].gameObject.SetActive(false);
                }
            }
        }

        LastNum = cardlist.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
