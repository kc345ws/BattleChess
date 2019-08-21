using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCharacterCtrl : CharacterBase
{
    private Transform cardTransformParent;//卡牌的父物体
    private GameObject cardPrefab;
    private int Index = 0;
    private static Object Lock = new Object();
    private List<GameObject> OtherCardList;

    private void Awake()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case CharacterEvent.INIT_OTHER_CARDLIST:
                StartCoroutine(initPlayerCard());
                break;

            case CharacterEvent.ADD_OTHERT_TABLECARDS:
                addTableCard();
                break;

            case CharacterEvent.REMOVE_OTHER_CARDS:
                removeSelectCard(message as List<CardDto>);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Bind(CharacterEvent.INIT_OTHER_CARDLIST);
        Bind(CharacterEvent.ADD_OTHERT_TABLECARDS);
        Bind(CharacterEvent.REMOVE_OTHER_CARDS);

        cardTransformParent = transform.Find("CardPoint");
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card/OtherCard");
        OtherCardList = new List<GameObject>();
    }

    /// <summary>
    /// 出牌成功时移除手牌
    /// </summary>
    /// <param name="restcardList">出牌后的剩余手牌</param>
    private void removeSelectCard(List<CardDto> restcardList)
    {
        int index = 0;
        /*if (restcardList.Count == 0)
        {
            return;//如果剩余手牌为0
        }*/

        foreach (var item in restcardList)
        {
            index++;

            if (index == restcardList.Count)
            {
                break;
            }
        }

        for (int i = index; i < OtherCardList.Count; i++)
        {
            if (OtherCardList[i].gameObject != null)
            {
                Destroy(OtherCardList[i].gameObject);//销毁剩余卡牌之后的卡牌
            }
        }
    }

        /// <summary>
        /// 停顿使发牌有动画感
        /// </summary>
        /// <returns></returns>
        private IEnumerator initPlayerCard()
    {
        for (int i = 0; i < 14; i++)
        {
            createCard(i);
            lock (Lock)
            {
                Index++;
            }
            yield return new WaitForSeconds(0.1f);       
        }
    }

    private void addTableCard()
    {
        for(int i = 0; i < 3; i++)
        {
            createCard(Index);
            Index++;
        }
    }

    private void createCard(int index)
    {
        GameObject card = GameObject.Instantiate(cardPrefab, cardTransformParent);
        //card.transform.localPosition = new Vector2(0, index * 0.8f);
        card.transform.localPosition = new Vector3(0, 0, index * 0.8f);
        card.GetComponent<SpriteRenderer>().sortingOrder = index;

        OtherCardList.Add(card);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
