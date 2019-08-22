using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 地图移动消息类
/// </summary>
class MapMoveMessage:MapBase
{
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

    public void Change(MapPointCtrl mapPointCtrl, CardDto cardDto, GameObject armyPrefab)
    {
        this.mapPointCtrl = mapPointCtrl;
        this.cardDto = cardDto;
        this.armyPrefab = armyPrefab;
    }
}

