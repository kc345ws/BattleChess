using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Protocol.Dto.Fight;
using Protocol.Constants;
using Protocol.Code;
using System.Collections.Generic;

/// <summary>
/// 地图建造类
/// </summary>
public class MapBuilder : MapBase
{
    private CardDto selectArmyCard;//选择的兵种牌

    private GameObject armyPrefab;//兵种预设体

    private MyCharacterCtrl myCharacterCtrl;//卡牌控制

    private MyArmyCtrlManager myArmyCtrls;//兵种控制

    private SocketMsg socketMsg;//套接字消息封装

    private MapPointDto pointDto;//地图点传输类

    private MapMoveDto mapMoveDto;//地图移动信息传输类

    public static MapBuilder Instance;//实例单例

    public delegate void TurnDelegate(ref int turncount);//回合委托
    public TurnDelegate turnDelegate;
    private int TurnCount = 0;

    private MapBuilder() { }

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        Bind(MapEvent.SELECT_ARMYCARD);
        Bind(MapEvent.CANCEL_SELECT_ARMYCARD);
        Bind(MapEvent.SET_OTHER_ARMY);
        Bind(MapEvent.MOVE_MY_ARMY);
        Bind(MapEvent.MOVE_OTHER_ARMY);

        myCharacterCtrl = GetComponent<MyCharacterCtrl>();
        myArmyCtrls = GetComponent<MyArmyCtrlManager>();

        socketMsg = new SocketMsg();
        pointDto = new MapPointDto();
        mapMoveDto = new MapMoveDto();
    }

    // Update is called once per frame
    void Update()
    {
        Build();
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case MapEvent.SELECT_ARMYCARD:
                selectArmyCard = message as CardDto;
                setArmyPrefab();
                break;

            case MapEvent.CANCEL_SELECT_ARMYCARD:
                selectArmyCard = null;
                break;

            case MapEvent.SET_OTHER_ARMY:
                processSetOtherArmy(message as MapPointDto);
                break;

            case MapEvent.MOVE_MY_ARMY:
                MapMoveMessage moveMessage = message as MapMoveMessage;
                moveArmy(ref moveMessage.OriginalMappointCtral,ref moveMessage.mapPointCtrl, moveMessage.cardDto,ref moveMessage.armyPrefab);
                break;

            case MapEvent.MOVE_OTHER_ARMY:
                processMoveOtherArmy(message as MapMoveDto);
                break;
        }
    }

    /// <summary>
    /// 处理其他人的兵种移动
    /// </summary>
    /// <param name="mapMoveDto"></param>
    private void processMoveOtherArmy(MapMoveDto mapMoveDto)
    {
        //镜像对称
        int totalX = 12;
        int totalZ = 8;

        int Originalx = mapMoveDto.OriginalMapPoint.X;
        int Originalz = mapMoveDto.OriginalMapPoint.Z;

        int MoveX = mapMoveDto.MoveMapPoint.X;
        int MoveZ = mapMoveDto.MoveMapPoint.Z;

        int OtherOriginalx = totalX - Originalx;
        int OtherOriginalz = totalZ - Originalz;//对方兵种真实位置
        int OtherMoveX = totalX - MoveX;
        int OtherMoveZ = totalZ - MoveZ;

        MapPointCtrl OriginalPointCtrl = null;
        MapPointCtrl MovePointCtrl = null;
        GameObject Army = null;
        OtherArmyCtrl otherArmyCtrl = null;
        foreach (var item in MapManager.mapPointCtrls)
        {
            if (item.mapPoint.X == OtherOriginalx && item.mapPoint.Z == OtherOriginalz)
            {
                OriginalPointCtrl = item;         
            }
            else if(item.mapPoint.X == OtherMoveX && item.mapPoint.Z == OtherMoveZ)
            {
                MovePointCtrl = item;
            }

            if(OriginalPointCtrl !=null && MovePointCtrl != null)
            {
                break;
            }
        }


        //fixbug已解决 只通过坐标选择了敌方单位
        foreach (var item in OtherArmyCtrlManager.OtherArmyCtrlList)
        {
            if(item.transform.position.x == OtherOriginalx && item.transform.position.z == OtherOriginalz
                &&item.armyState.MoveType == mapMoveDto.MoveType)
            {
                Army = item.gameObject;
                otherArmyCtrl = Army.GetComponent<OtherArmyCtrl>();
            }
        }

        if (mapMoveDto.MoveType == ArmyMoveType.LAND)
        {
            //如果是陆地单位
            OriginalPointCtrl.RemoveLandArmy();
            MovePointCtrl.MoveLandArmy(ref Army,otherArmyCtrl.armyState.Race,otherArmyCtrl.armyState.Name ,false);
            otherArmyCtrl.Move(MovePointCtrl.mapPoint, MovePointCtrl);
        }
        else if(mapMoveDto.MoveType == ArmyMoveType.SKY)
        {
            OriginalPointCtrl.RemoveSkyArmy();
            MovePointCtrl.MoveSkyArmy(ref Army, otherArmyCtrl.armyState.Race, otherArmyCtrl.armyState.Name ,false);
            otherArmyCtrl.Move(MovePointCtrl.mapPoint, MovePointCtrl);
        }
    }

    /// <summary>
    /// 处理设置其他人放置兵种
    /// </summary>
    /// <param name="mapPointDto"></param>
    private void processSetOtherArmy(MapPointDto mapPointDto)
    {
        
        //镜像对称
        int totalX = 12;
        int totalZ = 8;

        int x = mapPointDto.mapPoint.X;
        int z = mapPointDto.mapPoint.Z;

        int otherx = totalX - x;
        int otherz = totalZ - z;//对方兵种真实位置

        MapPointCtrl mapPointCtrl = null;
        GameObject prefab;
        foreach (var item in MapManager.mapPointCtrls)
        {
            if(item.mapPoint.X == otherx && item.mapPoint.Z == otherz)
            {
                mapPointCtrl = item;
                break;
            }
        }

        //fixbug 放置第二个单位时会额外放置第一个单位
        if (mapPointCtrl)
        {
            //传送的地图数据陆地单位不为空，且该地块上单位为空
            if(mapPointDto.LandArmyRace != -1 && mapPointCtrl.LandArmyRace==-1)
            {
                //陆地单位
                mapPointCtrl.mapPoint.X = otherx;
                mapPointCtrl.mapPoint.Z = otherz;
                mapPointCtrl.LandArmyRace = mapPointDto.LandArmyRace;
                mapPointCtrl.LandArmyName = mapPointDto.LandArmyName;
                setArmyPrefab(mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, out prefab);

                GameObject army= mapPointCtrl.SetLandArmy(prefab ,false);
                //初始化敌方兵种信息
                army.gameObject.GetComponent<OtherArmyCtrl>().Init(mapPointDto.LandArmyRace, mapPointDto.LandArmyName,mapPointCtrl) ;
                army.gameObject.GetComponent<OtherArmyCtrl>().armyState.Position = new Protocol.Constants.Map.MapPoint(otherx, otherz);
                //army.gameObject.GetComponent<OtherArmyCtrl>().armyState.CanFly = false;
                army.gameObject.GetComponent<OtherArmyCtrl>().armyState.MoveType = ArmyMoveType.LAND;
                mapPointCtrl.UpdateLandArmy(army);
                //向其他人添加兵种
                Dispatch(AreoCode.ARMY, ArmyEvent.ADD_OTHER_ARMY, army);
            }
            if(mapPointDto.SkyArmyRace != -1 && mapPointCtrl.SkyArmyRace==-1)
            {
                //飞行单位
                mapPointCtrl.mapPoint.X = otherx;
                mapPointCtrl.mapPoint.Z = otherz;
                mapPointCtrl.SkyArmyRace = mapPointDto.SkyArmyRace;
                mapPointCtrl.SkyArmyName = mapPointDto.SkyArmyName;
                setArmyPrefab(mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName, out prefab);

                GameObject army = mapPointCtrl.SetSkyArmy(prefab,false);

                //初始化敌方兵种信息
                army.gameObject.GetComponent<OtherArmyCtrl>().Init(mapPointDto.SkyArmyRace, mapPointDto.SkyArmyName,mapPointCtrl);
                army.gameObject.GetComponent<OtherArmyCtrl>().armyState.Position = new Protocol.Constants.Map.MapPoint(otherx, otherz);
                army.gameObject.GetComponent<OtherArmyCtrl>().armyState.MoveType = ArmyMoveType.SKY;
                mapPointCtrl.UpdateSkyArmy(army);
                Dispatch(AreoCode.ARMY, ArmyEvent.ADD_OTHER_ARMY, army);
            }
        }
    }

    /// <summary>
    /// 地图上设置兵种
    /// </summary>
    private void Build()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //没有点在UI上
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                /*bool hasArmy = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MyArmy"));
                Queue<Collider> colliders = new Queue<Collider>();
                while (hasArmy)
                {
                    Collider collider = hit.collider.GetComponent<Collider>();
                    collider.enabled = false;
                    colliders.Enqueue(collider);
                    hasArmy = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapPoint"));
                }*/

                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapPoint"));
                if (isCollider)
                {
                    turnDelegate.Invoke(ref TurnCount);//获取回合数
                    MapPointCtrl mapPointCtrl = hit.collider.GetComponent<MapPointCtrl>();
                    if(selectArmyCard!=null&&armyPrefab != null)
                    {
                        /*if(selectArmyCard.Class == ArmyClassType.Hero && hit.collider.tag != "BossStart")
                         {
                             //如果是英雄单位只能箭头处
                             Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "英雄单位只能箭头处");
                             return;
                         }
                         else if(selectArmyCard.Class != ArmyClassType.Hero && hit.collider.tag == "BossStart")
                         {
                             Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "非英雄单位不能箭头处");
                             return;
                         }

                         if(TurnCount <= 1)
                         {
                             if(hit.collider.tag != "StartAreo" && hit.collider.tag != "Winline" && hit.collider.tag != "BossStart")
                             {
                                 //如果第一回合不在屯兵区放置
                                 Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "初始阶段只能后三排");
                                 return;
                             }
                        }
                        else
                        {
                            if (hit.collider.tag != "StartAreo" && hit.collider.tag != "Winline" && hit.collider.tag != "BossStart" && hit.collider.tag != "FeatureArea")
                            {

                                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "只能放置在后五排");
                                return;
                            }
                        }*/
                        
                        switch (selectArmyCard.MoveType)
                        {
                            case ArmyMoveType.LAND://陆地单位
                                if (mapPointCtrl.HasLandArmy())
                                {
                                    Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "此处已有陆地单位");
                                    return;
                                }
                                //放置陆地单位
                                GameObject army;
                                army = mapPointCtrl.SetLandArmy(armyPrefab,true);
                                mapPointCtrl.LandArmyRace = selectArmyCard.Race;
                                mapPointCtrl.LandArmyName = selectArmyCard.Name;
                                //向我的兵种控制器集合添加兵种管理器
                                ArmyCtrl armyctrl = army.GetComponent<ArmyCtrl>();

                                armyctrl.Init(selectArmyCard, mapPointCtrl, army);
                                armyctrl.armyState.MoveType = (ushort)selectArmyCard.MoveType;
                                //更新地图点控制器
                                mapPointCtrl.UpdateLandArmy(army);
                                myArmyCtrls.Add(armyctrl);

                                //向发送发送出牌消息，让其他人消除卡牌
                                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_ARMYCARD_CREQ, selectArmyCard);
                                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                                //移除卡牌
                                Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectArmyCard);
                                //向服务器发送消息
                                pointDto.Change(mapPointCtrl.mapPoint, mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName);
                                socketMsg.Change(OpCode.FIGHT, FightCode.MAP_SET_ARMY_CREQ, pointDto);
                                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                                //向发送发送出牌消息，让其他人消除卡牌
                                //socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_CARD_CREQ, "出牌请求");
                                //socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_ARMYCARD_CREQ, selectArmyCard);
                                //Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                                break;

                            case ArmyMoveType.SKY:
                                if (mapPointCtrl.HasSkyArmy())
                                {
                                    Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "此处已有飞行单位");
                                    return;
                                }
                                //放置飞行单位
                                army = mapPointCtrl.SetSkyArmy(armyPrefab,true);
                                //mapPointCtrl.SkyArmyCard = selectArmyCard;
                                mapPointCtrl.SkyArmyRace = selectArmyCard.Race;
                                mapPointCtrl.SkyArmyName = selectArmyCard.Name;

                                //向我的兵种控制器集合添加兵种管理器
                                armyctrl = army.GetComponent<ArmyCtrl>();

                                armyctrl.Init(selectArmyCard, mapPointCtrl, army);
                                armyctrl.armyState.MoveType = (ushort)selectArmyCard.MoveType;
                                //更新地图点控制器
                                mapPointCtrl.UpdateSkyArmy(army);
                                myArmyCtrls.Add(armyctrl);

                                //向发送发送出牌消息，让其他人消除卡牌
                                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_ARMYCARD_CREQ, selectArmyCard);
                                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                                //移除卡牌
                                Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectArmyCard);
                                //向服务器发送消息
                                pointDto.Change(mapPointCtrl.mapPoint, mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName);
                                socketMsg.Change(OpCode.FIGHT, FightCode.MAP_SET_ARMY_CREQ, pointDto);
                                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                                /*//向发送发送出牌消息，让其他人消除卡牌
                                socketMsg.Change(OpCode.FIGHT, FightCode.DEAL_ARMYCARD_CREQ, selectArmyCard);
                                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);*/

                                break;
                        }

                        /*while(colliders.Count != 0)
                        {
                            colliders.Dequeue().enabled = true;
                        }*/
                        foreach (var item in MyArmyCtrlManager.Instance.CardCtrllist)
                        {
                            item.canBeSeletced = true;//所有单位可交互
                        }
                    }
                    
                }

            }
        }
    }

    /// <summary>
    /// 移动自己的单位
    /// </summary>
    /// <param name="originalCtral"></param>
    /// <param name="movepointctrl"></param>
    /// <param name="armyCard"></param>
    /// <param name="armyPrefab"></param>
    private void moveArmy(ref MapPointCtrl originalCtral,ref MapPointCtrl movepointctrl , CardDto armyCard , ref GameObject armyPrefab)
    {
        if (armyCard.MoveType == ArmyMoveType.LAND)
        {
            //如果是陆地单位
            //设置陆地单位
            //movepointctrl.SetLandArmy(armyPrefab);
            movepointctrl.MoveLandArmy(ref armyPrefab , armyCard.Race, armyCard.Name ,true);
            //movepointctrl.LandArmyRace = armyCard.Race;
            //movepointctrl.LandArmyName = armyCard.Name;
            //移除原来地块上的兵种
            removeArmy(ref originalCtral, armyCard);
            //向服务器发送消息
            mapMoveDto.Change(originalCtral.mapPoint, movepointctrl.mapPoint, armyCard.MoveType);
            socketMsg.Change(OpCode.FIGHT, FightCode.MAP_ARMY_MOVE_CREQ, mapMoveDto);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
        else if(armyCard.MoveType == ArmyMoveType.SKY)
        {
            //如果是飞行单位
            //设置飞行单位
            //movepointctrl.SetSkyArmy(armyPrefab);
            movepointctrl.MoveSkyArmy(ref armyPrefab, armyCard.Race, armyCard.Name ,true);
            //movepointctrl.SkyArmyRace = armyCard.Race;
            //movepointctrl.SkyArmyName = armyCard.Name;
            //移除原来地块上的兵种
            removeArmy(ref originalCtral, armyCard);
            //向服务器发送消息
            mapMoveDto.Change(originalCtral.mapPoint, movepointctrl.mapPoint, armyCard.MoveType);
            socketMsg.Change(OpCode.FIGHT, FightCode.MAP_ARMY_MOVE_CREQ, mapMoveDto);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
        }
    }

    private void removeArmy(ref MapPointCtrl Originalpointctrl, CardDto armyCard)
    {
        if (armyCard.MoveType == ArmyMoveType.LAND)
        {
            //陆地单位
            Originalpointctrl.RemoveLandArmy();
        }
        else if(armyCard.MoveType == ArmyMoveType.SKY)
        {
            Originalpointctrl.RemoveSkyArmy();
        }
    }

    private void setArmyPrefab()
    {
        string path = "";
        
        if (selectArmyCard != null)
        {
            int armyname = selectArmyCard.Name;
            switch (selectArmyCard.Race)
            {
                case RaceType.ORC:
                    path = "Prefabs/ArmyIcon/Orc/My/" + armyname;
                    break;
            }
            armyPrefab = Resources.Load<GameObject>(path);
        }
    }

    private void setArmyPrefab(int Race , int Name,out GameObject prefab)
    {
        string path = "";


        switch (Race)
            {
                case RaceType.ORC:
                    path = "Prefabs/ArmyIcon/Orc/Other/" + Name;
                    break;
            }
        prefab = Resources.Load<GameObject>(path);    

    }

    /* private void MonitorSelectArmy()
     {
         if(myCharacterCtrl.LastSelectCard != null)
         {
             CardCtrl cardCtrl = myCharacterCtrl.LastSelectCard;
             if(cardCtrl.cardDto.Type == CardType.ARMYCARD)
             {
                 //如果选中牌为兵种牌

             }
         }
     }*/
}
