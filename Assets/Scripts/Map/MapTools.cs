using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol.Constants.Map;

public class MapTools : MapBase
{
    public static MapTools Instance = null;
    private MapTools() { }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 设置地图块颜色
    /// </summary>
    /// <param name="canMovePointCtrls"></param>
    public static void setMappointCtrlColor(List<MapPointCtrl> canMovePointCtrls)
    {
        //Color bluecolor = new Color(13f / 255f, 175f / 255f, 244f / 255f);
        foreach (var item in canMovePointCtrls)
        {
            item.SetColor(item.origanColor);
        }
    }

    public static void setMappointCtrlColor(List<MapPointCtrl> canMovePointCtrls, Color color)
    {
        //Color bluecolor = new Color(13f / 255f, 175f / 255f, 244f / 255f);
        foreach (var item in canMovePointCtrls)
        {
            item.SetColor(color);
        }
    }

    public static List<MapPointCtrl>getMapCtrlByMapPoint(List<MapPoint> mapPoints)
    {
        List<MapPointCtrl> mapPointCtrls = new List<MapPointCtrl>();
        foreach (var point in mapPoints)
        {
            foreach (var item in MapManager.mapPointCtrls)
            {
                if(point.X == item.mapPoint.X && point.Z == item.mapPoint.Z)
                {
                    mapPointCtrls.Add(item);
                    break;
                }
            }
        }
        return mapPointCtrls;
    }
}
