using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : ManagerBase
{
    public static MapManager Instance = null;

    /// <summary>
    /// 地图点控制类集合
    /// </summary>
    public static List<MapPointCtrl> mapPointCtrls;

    void Awake()
    {
        Instance = this;
        mapPointCtrls = new List<MapPointCtrl>();
    }
}
