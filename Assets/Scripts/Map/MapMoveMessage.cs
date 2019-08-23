using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 地图移动消息类
/// </summary>
public class MapMoveMessage:MapBase
{
    /// <summary>
    /// 原来
    /// </summary>
    public MapPointCtrl OriginalMappointCtral;
    /// <summary>
    /// 兵种想要移动到的地图点控制器
    /// </summary>
    public MapPointCtrl mapPointCtrl;
    public CardDto cardDto;
    public GameObject armyPrefab;

    public MapMoveMessage() { }

    public MapMoveMessage(MapPointCtrl mapPointCtrl, CardDto cardDto, GameObject armyPrefab)
    {
        this.mapPointCtrl = mapPointCtrl;
        this.cardDto = cardDto;
        this.armyPrefab = armyPrefab;
    }

    public void Change(MapPointCtrl originalPointctral, MapPointCtrl mapPointCtrl, CardDto cardDto, GameObject armyPrefab)
    {
        OriginalMappointCtral = originalPointctral;
        this.mapPointCtrl = mapPointCtrl;
        this.cardDto = cardDto;
        this.armyPrefab = armyPrefab;
    }
}

