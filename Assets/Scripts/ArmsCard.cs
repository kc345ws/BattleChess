using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsCard : Card {//兵种卡
    public int MaxHp { get; set; }//最大血量
    public int Damage { get; set; }//伤害值
    public int Speed { get; set; }//速度
    public int AttackDistance { get; set; }//攻击距离
    public GameObject[] Range { get; set; }//攻击范围以及移动范围
    public GameObject ArmsPrefab;//兵种模型
    public bool isFly { get; set; }//是否是飞行单位
    public int Level { get; set; }//阶级

    
}

