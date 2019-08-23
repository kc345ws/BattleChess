﻿using Protocol.Constants;
using Protocol.Constants.Map;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArmyCtrl : ArmyBase
{
    public GameObject ArmyPrefab { get; private set; }//脚本控制的兵种

    public CardDto ArmyCard { get; private set; }//兵种所属卡牌

    public MapPointCtrl ArmymapPointCtrl { get; private set; }//兵种所在地图点控制器

    private MapPoint mapPoint;//兵种所在地图点

    public bool isSelect;//是否被选中

    private MapMoveMessage moveMessage;//地图移动消息

    public delegate bool ArmySelectDelegate(ArmyCtrl armyCtrl);//兵种选择委托
    public ArmySelectDelegate ArmySelectEvent;//兵种选择事件

    private float Timer = 0;//计时器

    //private Renderer renderer;

    private void Awake()
    {
        isSelect = false;
        moveMessage = new MapMoveMessage();
    }
    // Start is called before the first frame update
    void Start()
    {
        //ArmyPrefab = gameObject; 
    }

    public void Init(CardDto cardDto , MapPointCtrl mapPointCtrl ,GameObject armyPrefab)
    {
        ArmyCard = cardDto;
        ArmymapPointCtrl = mapPointCtrl;

        mapPoint = ArmymapPointCtrl.mapPoint;

        ArmyPrefab = armyPrefab;

        //renderer = ArmyPrefab.gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelect)
        {
            Timer += Time.deltaTime;
            StartCoroutine(selectState());
            //selectState();
        }
        Move();
    }

    private List<MapPointCtrl> GetCanMoveMapPoint()
    {
        List<MapPoint> canMoveMapPoints = new List<MapPoint>();
        List<MapPointCtrl> canMoveMapPointCtrls = new List<MapPointCtrl>();
        int x = mapPoint.X;
        int z = mapPoint.Z;

        Color bluecolor = new Color(13f / 255f, 175f / 255f, 244f / 255f);
        if (ArmyCard.Class == ArmyClassType.Ordinary)
        {
            //如果是普通兵种不能向后移动
            MapPoint mapPointFront = new MapPoint(x + 1, z);
            MapPoint mapPointRight = new MapPoint(x, z + 1);
            MapPoint mapPointLeft = new MapPoint(x, z - 1);
            canMoveMapPoints.Add(mapPointFront);
            canMoveMapPoints.Add(mapPointRight);
            canMoveMapPoints.Add(mapPointLeft);

            foreach (var item in canMoveMapPoints)
            {
                foreach (var mapPointCtrl in MapManager.mapPointCtrls)
                {
                    if(item.X == mapPointCtrl.mapPoint.X && item.Z == mapPointCtrl.mapPoint.Z)
                    {
                        if (!ArmyCard.CanFly && mapPointCtrl.LandArmy == null)
                        {
                            //如果是陆地单位,且要移动到的地图点没有陆地单位
                            canMoveMapPointCtrls.Add(mapPointCtrl);
                            mapPointCtrl.SetColor(bluecolor);
                        }
                        else if (ArmyCard.CanFly && mapPointCtrl.SkyArmy == null)
                        {
                            //如果是飞行单位，且要移动到的地图点没有飞行单位
                            canMoveMapPointCtrls.Add(mapPointCtrl);
                            mapPointCtrl.SetColor(bluecolor);
                        }                    
                        break;
                    }
                }
            }
        }
        else
        {
            //其他兵种
            MapPoint mapPointFront = new MapPoint(x + 1, z);
            MapPoint mapPointBack = new MapPoint(x - 1, z);
            MapPoint mapPointRight = new MapPoint(x, z + 1);
            MapPoint mapPointLeft = new MapPoint(x, z - 1);
            canMoveMapPoints.Add(mapPointFront);
            canMoveMapPoints.Add(mapPointBack);
            canMoveMapPoints.Add(mapPointRight);
            canMoveMapPoints.Add(mapPointLeft);

            foreach (var item in canMoveMapPoints)
            {
                foreach (var mapPointCtrl in MapManager.mapPointCtrls)
                {
                    if (item.X == mapPointCtrl.mapPoint.X && item.Z == mapPointCtrl.mapPoint.Z)
                    {
                        if (!ArmyCard.CanFly && mapPointCtrl.LandArmy == null)
                        {
                            //如果是陆地单位,且要移动到的地图点没有陆地单位
                            canMoveMapPointCtrls.Add(mapPointCtrl);
                            mapPointCtrl.SetColor(bluecolor);
                        }
                        else if (ArmyCard.CanFly && mapPointCtrl.SkyArmy == null)
                        {
                            //如果是飞行单位，且要移动到的地图点没有飞行单位
                            canMoveMapPointCtrls.Add(mapPointCtrl);
                            mapPointCtrl.SetColor(bluecolor);
                        }
                        break;
                    }
                }
            }
        }
        return canMoveMapPointCtrls;
    }

    /// <summary>
    /// 兵种移动
    /// </summary>
    private void Move()
    {
        if (Input.GetMouseButtonDown(0) && isSelect)
        {
            
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                //如果不在UI上
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit,1000,LayerMask.GetMask("MapPoint"));
                if (isCollider)
                {
                    List<MapPointCtrl> canMovePointCtrls = GetCanMoveMapPoint();
                    MapPointCtrl movePointctrl = hit.collider.gameObject.GetComponent<MapPointCtrl>();
                    if (canMove(canMovePointCtrls, movePointctrl))
                    {
                        //如果可以移动
                        //移动
                        moveMessage.Change(ArmymapPointCtrl,movePointctrl, ArmyCard, ArmyPrefab);
                        Dispatch(AreoCode.MAP, MapEvent.MOVE_MY_ARMY, ref moveMessage);
                        //将颜色变为原来的
                        setMappointCtrl(canMovePointCtrls);
                        //改变所在所在地图点控制器
                        ArmymapPointCtrl = movePointctrl;
                        mapPoint = movePointctrl.mapPoint;
                    }
                }
            }
        }     
    }

    private void setMappointCtrl(List<MapPointCtrl> canMovePointCtrls)
    {
        foreach (var item in canMovePointCtrls)
        {
            item.SetColor(item.origanColor);
        }
    }

    /// <summary>
    /// 判断兵种是否可以向指定地图点移动
    /// </summary>
    /// <param name="canMovePointCtrls"></param>
    /// <param name="movePointctrl"></param>
    /// <returns></returns>
    private bool canMove(List<MapPointCtrl> canMovePointCtrls , MapPointCtrl movePointctrl)
    {
        bool canmove = false;
        foreach (var item in canMovePointCtrls)
        {
            if(movePointctrl.mapPoint.X == item.mapPoint.X && movePointctrl.mapPoint.Z == item.mapPoint.Z)
            {
                if (!ArmyCard.CanFly && item.LandArmy == null)
                {
                    //如果是陆地单位,且要移动到的地图点没有陆地单位
                    canmove = true;
                }
                else if(ArmyCard.CanFly && item.SkyArmy == null)
                {
                    //如果是飞行单位，且要移动到的地图点没有飞行点位
                    canmove = true;
                }
                break;
            }
        }
        return canmove;
    }

    private void OnMouseDown()
    {
        //isSelect = true;
        //transform.localScale *= 2;
        if (ArmySelectEvent.Invoke(this))
        {
            //第一次选择或和上次选择不一样
            //transform.localScale *= 2;
            //StartCoroutine(selectState());
        }
        else
        {
            //StopCoroutine(selectState());
            //和上次选择一样
            //transform.localScale /= 2;
        }
    }

    /// <summary>
    /// 兵种选择状态
    /// </summary>
    /// <returns></returns>
    /*private IEnumerator selectState()
    {
        Renderer renderer = GetComponent<Renderer>();
        float r = renderer.material.color.r;
        float g = renderer.material.color.g;
        float b = renderer.material.color.b;
        renderer.material.color = new Color(r, g, b, 0);    
        yield return new WaitForSeconds(1f);
        renderer.material.color = new Color(r, g, b, 1);
    }*/

    private IEnumerator selectState()
    {
        Renderer renderer = GetComponent<Renderer>();
        float r = renderer.material.color.r;
        float g = renderer.material.color.g;
        float b = renderer.material.color.b;

        renderer.material.color = new Color(r, g, b, 1);
        if (Timer >= 0.5)
        {
            renderer.material.color = new Color(r, g, b, 0);
            yield return new WaitForSeconds(0.5f);
            Timer = 0f;
        }
        
    }

    public void CheckIsA()
    {
        Renderer renderer = GetComponent<Renderer>();
        float r = renderer.material.color.r;
        float g = renderer.material.color.g;
        float b = renderer.material.color.b;
        if (renderer.material.color.a == 0)
        {
            renderer.material.color = new Color(r, g, b, 1);
        }
    }
}
