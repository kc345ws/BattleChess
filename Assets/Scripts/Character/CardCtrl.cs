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
            transform.localPosition -= new Vector3(0, 0.3f, 0);
        }

        string path;
        if (!ismine)
        {
            path = "Poker/CardBack";
            //不是自己的牌显示背面
        }
        else
        {
            path = "Poker/" + cardDto.Name;
        }

        Sprite sp = Resources.Load<Sprite>(path);
        spriteRenderer.sprite = sp;
        spriteRenderer.sortingOrder = index;
    }

    private void OnMouseDown()
    {    
        if (!IsMine)
        {
            return;
        }
        Debug.Log("被点击");

        if (!IsSelected)//如果没被选中且被点击了
        {
            IsSelected = true;
            transform.localPosition += new Vector3(0, 0.3f, 0);
        }
        else
        {
            //如果被选中了
            IsSelected = false;
            transform.localPosition -= new Vector3(0, 0.3f, 0);
        }
    }
}
