﻿using Protocol.Constants;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌控制类
/// </summary>
public class CardCtrl : MonoBehaviour
{
    public CardDto cardDto { get; private set; }//脚本控制的卡牌
    private SpriteRenderer spriteRenderer;//卡牌的图片
    private bool IsMine;//是否是自己的牌
    public bool IsSelected { get; set; }//卡牌是否被选中

    public delegate bool CardMouseDownDelegate(CardCtrl cardCtrl);
    public CardMouseDownDelegate CardMouseDownEvent;

    private static GameObject Lock;

    private bool isPreview;//是否在预览

    private bool isEnlarge;//是否在放大

    private void Start()
    {
        Lock = new GameObject();
        isPreview = false;
        isEnlarge = false;
    }
    /// <summary>
    /// 初始化卡牌
    /// </summary>
    /// <param name="cardDto">卡牌数据</param>
    /// <param name="ismine">是否是自己的牌</param>
    /// <param name="index">叠放层次</param>
    public void Init(CardDto cardDto,bool ismine , int index)
    {
        //卡牌初始化
        this.cardDto = cardDto;
        this.IsMine = ismine;
        //IsSelected = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        //重用卡牌
        if (IsSelected)
        {
            IsSelected = false;
            transform.localPosition -= new Vector3(1f, 0, 0);
        }

        string path  ="";
        if (!ismine)
        {
            path = "Fight/CardBack";
            //不是自己的牌显示背面
        }
        else if(cardDto.Type == CardType.ORDERCARD)
        {
            //指令卡
            path = "Fight/OrderCard/" + cardDto.Name;
        }
        else if (cardDto.Type == CardType.ARMYCARD)
        {
            //兵种卡
            switch (cardDto.Race)
            {
                case RaceType.ORC:
                    path = "Fight/Orc/ArmyCard/" + cardDto.Name;
                    break;
            }
        }
        else
        {
            //非指令卡
            switch (cardDto.Race)
            {
                case RaceType.ORC:
                    path = "Fight/Orc/OtherCard/" + cardDto.Name;
                    break;
            }
        }

        Sprite sp = Resources.Load<Sprite>(path);
        spriteRenderer.sprite = sp;
        spriteRenderer.sortingOrder = index;


    }

    private void OnMouseDrag()
    {
        if (!isEnlarge)
        {
            transform.localPosition += new Vector3(6f, 0, 0);
            transform.localScale *= 3;
            isEnlarge = true;
        }
    }

    private void OnMouseUp()
    {
        transform.localPosition -= new Vector3(6f, 0, 0);
        transform.localScale /= 3;
        isEnlarge = false;
    }

    private void OnMouseEnter()
    {
        if (!IsSelected)
        {
            isPreview = true;
            transform.localPosition += new Vector3(0.5f, 0, 0);        
        }   
    }

    private void OnMouseExit()
    {
        if (!IsSelected)
        {
            isPreview = false;
            transform.localPosition -= new Vector3(0.5f, 0, 0);         
        }       
    }

    private void OnMouseDown()
    {    
        if (!IsMine)
        {
            return;
        }
        Debug.Log("被点击");


        //如果是第一次点击或者和上次点击的不一样,则将该张牌设为选中状态
        if (CardMouseDownEvent.Invoke(this))
        {
            IsSelected = true;
            //transform.localPosition += new Vector3(0, 0, 1f);
            if (isPreview)
            {
                transform.localPosition += new Vector3(0.5f, 0, 0);
            }
            else
            {
                transform.localPosition += new Vector3(1f, 0, 0);
            }
            
        }
        else
        {
            //如果和上次点击的一样,则取消该张牌的选中状态
            IsSelected = false;
            if (isPreview)
            {
                transform.localPosition -= new Vector3(0.5f, 0, 0);
            }
            else
            {
                transform.localPosition -= new Vector3(1f, 0, 0);
            }
        }

        /*
        if (!IsSelected)//如果没被选中且被点击了
        {
            IsSelected = true;
            transform.localPosition += new Vector3(0, 0, 1f);
        }
        else
        {
            //如果被选中了
            IsSelected = false;
            transform.localPosition -= new Vector3(0, 0, 1f);
        }*/
    }
}
