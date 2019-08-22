using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyManager : ManagerBase
{ 
    public static ArmyManager Instance = null;

    /*/// <summary>
    /// 地图点控制类集合
    /// </summary>
    public static List<MapPointCtrl> mapPointCtrls;*/

    void Awake()
    {
        Instance = this;
        //mapPointCtrls = new List<MapPointCtrl>();
    }
}
