using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCard : Card {
    public int Type { get; set; }
}

internal enum OrderType { 攻击,闪避,反击,修养};

class AttackCard : OrderCard//攻击指令卡
{

}

class DodgeCard : OrderCard//闪避指令卡
{

}

class AttacBackCard : OrderCard//反击指令卡
{

}

class RestCard : OrderCard//修养指令卡
{

}