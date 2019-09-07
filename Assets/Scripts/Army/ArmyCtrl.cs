﻿using Protocol.Code;
using Protocol.Constants;
using Protocol.Constants.Map;
using Protocol.Constants.Orc;
using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//fixbug:兵种移动后需要更新可攻击范围
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

    public ArmyCardBase armyState;//兵种属性

    private List<MapPoint> canAttckPoint;//能够攻击到的点

    private SocketMsg socketMsg;//套接字消息封装

    private MapAttackDto attackDto;//地图攻击数据传输对象

    public bool canAttack { get; set; }//是否能攻击

    public bool isAttack { get; private set; }//是否攻击过

    //public bool isShowAttackButton { get; private set; }//是否显示攻击按钮

    public bool iscanMove = true;//是否可以移动

    private int SelectArmyType = -1;//多个兵种重叠时选择的兵种类型

    List<MapPointCtrl> canMovePointCtrls = new List<MapPointCtrl>();//可以移动到的地图点

    //private Renderer renderer;
    private bool isrefresh = true;//是否需要还原颜色

    private void Awake()
    {
        isSelect = false;
        moveMessage = new MapMoveMessage();
    }
    // Start is called before the first frame update
    void Start()
    {
        Bind(ArmyEvent.SET_MY_LAND_SKY);
        //ArmyPrefab = gameObject; 
        socketMsg = new SocketMsg();
        attackDto = new MapAttackDto();
    }

    public override void Execute(int eventcode, object message)
    {
        //base.Execute(eventcode, message);
        switch (eventcode)
        {
            case ArmyEvent.SET_MY_LAND_SKY:
                SelectArmyType = (int)message;
                break;
        }
    }

    public void Init(CardDto cardDto , MapPointCtrl mapPointCtrl ,GameObject armyPrefab)
    {
        ArmyCard = cardDto;
        ArmymapPointCtrl = mapPointCtrl;

        mapPoint = ArmymapPointCtrl.mapPoint;

        ArmyPrefab = armyPrefab;

        //renderer = ArmyPrefab.gameObject.GetComponent<Renderer>();

        setArmyState(cardDto);

        

        armyState.Position = mapPoint;
        canAttckPoint = MapAttackType.Instance.GetAttakRange(armyState);
        //canAttckPoint = GetAttakRange(armyState);

        if (armyState.Class == ArmyClassType.Ordinary)
        {
            //如果是普通兵种
            canAttack = true;
            isAttack = false;
            //isShowAttackButton = true;
            
        }
        else
        {
            //其他兵种
            canAttack = false;
            isAttack = false;
        }
    }

    /// <summary>
    /// 设置兵种属性
    /// </summary>
    /// <param name="cardDto"></param>
    private void setArmyState(CardDto cardDto)
    {
        switch (cardDto.Race)
        {
            case RaceType.ORC:

                switch (cardDto.Name)
                {
                    case OrcArmyCardType.Infantry:
                        armyState = new OrcInfantry();
                        break;

                    case OrcArmyCardType.Eagle_Riders:
                        armyState = new OrcEagleRiders();
                        break;

                    case OrcArmyCardType.Black_Rats_Boomer:
                        armyState = new OrcBlackRatsBoomer();
                        break;

                    case OrcArmyCardType.Giant_mouthed_Frog:
                        armyState = new OrcGiantmouthedFrog();
                        break;

                    case OrcArmyCardType.Forest_Shooter:
                        armyState = new OrcForestShooter();
                        break;

                    case OrcArmyCardType.Pangolin:
                        armyState = new OrcPangolin();
                        break;

                    case OrcArmyCardType.Raven_Shaman:
                        armyState = new OrcRavenShaman();
                        break;

                    case OrcArmyCardType.Hero:
                        armyState = new OrcHero();
                        break;
                }

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelect)
        {
            Timer += Time.deltaTime;
            StartCoroutine(selectState());
            //selectState();
            isrefresh = true;
        }
        else
        {
            //将地图点颜色变为原来的
            if (isrefresh)
            {
                StopAllCoroutines();
                setMappointCtrl(canMovePointCtrls);
                isrefresh = false;
            }         
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
                            //改变颜色
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
        if (Input.GetMouseButtonDown(0) && isSelect && iscanMove)
        {
            
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                //如果不在UI上
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit,1000,LayerMask.GetMask("MapPoint"));
                if (isCollider)
                {
                    //List<MapPointCtrl> canMovePointCtrls = GetCanMoveMapPoint();
                    canMovePointCtrls = GetCanMoveMapPoint();
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
                        //更新单位所在地图点
                        armyState.Position = movePointctrl.mapPoint;
                        //更新可攻击点
                        canAttckPoint = MapAttackType.Instance.GetAttakRange(armyState);
                        

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

    private void OnMouseDrag()
    {
        
    }

    private void OnMouseDown()
    {
        if(ArmymapPointCtrl.LandArmy !=null && ArmymapPointCtrl.SkyArmy != null)
        {
            //如果陆地和飞行单位重合
            Dispatch(AreoCode.UI, UIEvent.SELECT_MY_LAND_SKY, false);
            StartCoroutine(selectArmy());
        }
        else
        {
            //如果只有一个单位
            if (ArmySelectEvent.Invoke(this))
            {
                //和上次选择不一样或第一次选择
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, this);
            }
            else
            {
                //和上次选择一样
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
            }

        }

        
    }

    /// <summary>
    /// 选择兵种协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator selectArmy()
    {
        yield return new WaitUntil(isSelectArmyType);
        if (SelectArmyType == ArmyMoveType.LAND)
        {
            //选择了陆地兵种
            ArmyCtrl armyCtrl = ArmymapPointCtrl.LandArmy.GetComponent<ArmyCtrl>();

            if (ArmySelectEvent.Invoke(armyCtrl))
            {
                //和上次选择不一样或第一次选择
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, armyCtrl);
            }
            else
            {
                //和上次选择一样
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
            }
        }
        else
        {
            //如果选择了飞行单位
            ArmyCtrl armyCtrl = ArmymapPointCtrl.SkyArmy.GetComponent<ArmyCtrl>();
            if (ArmySelectEvent.Invoke(armyCtrl))
            {
                //和上次选择不一样或第一次选择
                Dispatch(AreoCode.UI, UIEvent.SHOW_ARMY_MENU_PANEL, armyCtrl);
            }
            else
            {
                //和上次选择一样
                Dispatch(AreoCode.UI, UIEvent.CLOSE_ARMY_MENU_PANEL, "关闭面板");
            }
        }

        SelectArmyType = -1;
    }

    /// <summary>
    /// 是否选择兵种类型
    /// </summary>
    /// <returns></returns>
    private bool isSelectArmyType()
    {
        if(SelectArmyType != -1)
        {
            return true;
        }
        return false;
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

    
      ///选择状态动画
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

    /// <summary>
    /// 监测兵种是否处于动画透明状态
    /// </summary>
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

    public bool Attack(MapPointCtrl defensemapPointCtrl ,OtherArmyCtrl defenseArmy)
    {

        /*if(armyState.Class == ArmyClassType.Ordinary)
        {
            //如果兵种是普通兵种
            foreach (var item in canAttckPoint)
            {
                if(item.X == mapPointCtrl.mapPoint.X && item.Z == mapPointCtrl.mapPoint.Z)
                {
                    canAttack = true;
                    break;
                   
                }
            }
            if (canAttack)
            {
                //TODO 播放攻击动画
                //向服务器发送攻击消息
                socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_ATTACK_CREQ, ArmymapPointCtrl);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
            }
       
        else
        {
            //TODO 其他兵种
        }*/

        if (defenseArmy == null)
        {
            return false;
        }

        bool canAttackPoint = false;//是否能攻击地图点上的兵种
        int attackSpace = ArmyMoveType.NONE;//攻击陆地1 攻击飞行2

        if(defenseArmy.armyState.Name == defensemapPointCtrl.LandArmyName)
        {
            attackSpace = ArmyMoveType.LAND;
        }
        else
        {
            attackSpace = ArmyMoveType.SKY;
        }

        if (canAttack && !isAttack)
        {
            //如果能攻击且没有攻击过
            foreach (var item in canAttckPoint)
            {
                if (item.X == defensemapPointCtrl.mapPoint.X && item.Z == defensemapPointCtrl.mapPoint.Z)
                {
                    //canAttack = true;
                    canAttackPoint = true;
                    break;
                }
            }

            if(canAttackPoint && attackSpace == ArmyMoveType.LAND)
            {
                //如果攻击陆地兵种

                //改变状态
                isAttack = true;
                //isShowAttackButton = false;
                //TODO 播放攻击动画
                //向服务器发送攻击消息
                attackDto.Change(mapPoint, defensemapPointCtrl.mapPoint, ArmyCard.CanFly, false);
                socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_ATTACK_CREQ, attackDto);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

                return true;
            }
            else if(canAttackPoint && attackSpace == ArmyMoveType.SKY)
            {
                //如果攻击飞行兵种
                //改变状态
                isAttack = true;
                //isShowAttackButton = false;
                //TODO 播放攻击动画
                //向服务器发送攻击消息
                attackDto.Change(mapPoint, defensemapPointCtrl.mapPoint, ArmyCard.CanFly, true);
                socketMsg.Change(OpCode.FIGHT, FightCode.ARMY_ATTACK_CREQ, attackDto);
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);

                return true;
            }     
            
        }

        return false;
    }


    /*public List<MapPoint> GetAttakRange(ArmyCardBase armyCardBase)
    {
        //可以攻击到的范围
        List<MapPoint> canAttckPoint = new List<MapPoint>();
        switch (armyCardBase.AttackRangeType)
        {
            case MapAttackType.NONE:
                return null;

            case MapAttackType.Triple_lattice:
                Triple_latticeType(armyCardBase.Position, ref canAttckPoint);
                break;

            case MapAttackType.Four_lattice:
                Four_latticeType(armyCardBase.Position, ref canAttckPoint);
                break;

            case MapAttackType.Three_Front:
                Three_FrontType(armyCardBase.Position, ref canAttckPoint);
                break;

            case MapAttackType.Four_Angle:
                Four_AngleType(armyCardBase.Position, ref canAttckPoint);
                break;

            case MapAttackType.All_Around:
                All_AroundType(armyCardBase.Position, ref canAttckPoint);
                break;
        }
        return canAttckPoint;
        //throw new Exception("没有该兵种的攻击范围类型");
    }

    /// <summary>
    /// 判断攻击点是否超出地图边界
    /// </summary>
    /// <param name="mapPoint"></param>
    /// <returns></returns>
    private bool iscanAttack(MapPoint mapPoint)
    {
        if (mapPoint.X < 0 || mapPoint.X > 12)
        {
            return false;
        }
        else if (mapPoint.Z < 0 || mapPoint.Z > 8)
        {
            return false;
        }
        return true;
    }

    private void Triple_latticeType(MapPoint mapPoint, ref List<MapPoint> canAttckPoint)
    {
        int x = mapPoint.X;
        int z = mapPoint.Z;


        if (iscanAttack(new MapPoint(x + 1, z)))
        {
            canAttckPoint.Add(new MapPoint(x + 1, z));
        }
        if (iscanAttack(new MapPoint(x, z + 1)))
        {
            canAttckPoint.Add(new MapPoint(x, z + 1));
        }
        if (iscanAttack(new MapPoint(x, z - 1)))
        {
            canAttckPoint.Add(new MapPoint(x, z - 1));
        }
    }

    private void Four_latticeType(MapPoint mapPoint, ref List<MapPoint> canAttckPoint)
    {
        int x = mapPoint.X;
        int z = mapPoint.Z;

        if (iscanAttack(new MapPoint(x + 1, z)))
        {
            canAttckPoint.Add(new MapPoint(x + 1, z));
        }
        if (iscanAttack(new MapPoint(x - 1, z)))
        {
            canAttckPoint.Add(new MapPoint(x - 1, z));
        }
        if (iscanAttack(new MapPoint(x, z + 1)))
        {
            canAttckPoint.Add(new MapPoint(x, z + 1));
        }
        if (iscanAttack(new MapPoint(x, z - 1)))
        {
            canAttckPoint.Add(new MapPoint(x, z - 1));
        }
    }

    private void Three_FrontType(MapPoint mapPoint, ref List<MapPoint> canAttckPoint)
    {
        int x = mapPoint.X;
        int z = mapPoint.Z;

        if (iscanAttack(new MapPoint(x + 1, z)))
        {
            canAttckPoint.Add(new MapPoint(x + 1, z));
        }
        if (iscanAttack(new MapPoint(x + 2, z)))
        {
            canAttckPoint.Add(new MapPoint(x + 2, z));
        }
        if (iscanAttack(new MapPoint(x + 3, z)))
        {
            canAttckPoint.Add(new MapPoint(x + 3, z));
        }

    }

    private void Four_AngleType(MapPoint mapPoint, ref List<MapPoint> canAttckPoint)
    {
        int x = mapPoint.X;
        int z = mapPoint.Z;

        if (iscanAttack(new MapPoint(x - 1, z - 1)))
        {
            canAttckPoint.Add(new MapPoint(x - 1, z - 1));
        }
        if (iscanAttack(new MapPoint(x + 1, z + 1)))
        {
            canAttckPoint.Add(new MapPoint(x + 1, z + 1));
        }
        if (iscanAttack(new MapPoint(x + 1, z - 1)))
        {
            canAttckPoint.Add(new MapPoint(x + 1, z - 1));
        }
        if (iscanAttack(new MapPoint(x - 1, z + 1)))
        {
            canAttckPoint.Add(new MapPoint(x - 1, z + 1));
        }
    }

    private void All_AroundType(MapPoint mapPoint, ref List<MapPoint> canAttckPoint)
    {
        Four_latticeType(mapPoint, ref canAttckPoint);
        Four_AngleType(mapPoint, ref canAttckPoint);
    }*/


}
