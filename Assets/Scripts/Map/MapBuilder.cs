using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Protocol.Dto.Fight;
using Protocol.Constants;
using Protocol.Code;

/// <summary>
/// 地图建造类
/// </summary>
public class MapBuilder : MapBase
{
    private CardDto selectArmyCard;//选择的兵种牌

    private GameObject armyPrefab;//兵种预设体

    private MyCharacterCtrl myCharacterCtrl;

    private SocketMsg socketMsg;

    private MapPointDto pointDto;

    //private int OtherCardId = 0;

    // Use this for initialization
    void Start()
    {
        Bind(MapEvent.SELECT_ARMYCARD);
        Bind(MapEvent.CANCEL_SELECT_ARMYCARD);
        Bind(MapEvent.SET_OTHER_ARMY);

        myCharacterCtrl = GetComponent<MyCharacterCtrl>();
        socketMsg = new SocketMsg();
        pointDto = new MapPointDto();
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
        }
    }

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

        if (mapPointCtrl)
        {
            if(mapPointDto.LandArmyRace != -1)
            {
                mapPointCtrl.mapPoint.X = otherx;
                mapPointCtrl.mapPoint.Z = otherz;
                mapPointCtrl.LandArmyRace = mapPointDto.LandArmyRace;
                mapPointCtrl.LandArmyName = mapPointDto.LandArmyName;
                setArmyPrefab(mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, out prefab);

                mapPointCtrl.SetLandArmy(prefab);
            }
            if(mapPointDto.SkyArmyRace != -1)
            {
                mapPointCtrl.mapPoint.X = otherx;
                mapPointCtrl.mapPoint.Z = otherz;
                mapPointCtrl.SkyArmyRace = mapPointDto.SkyArmyRace;
                mapPointCtrl.SkyArmyName = mapPointDto.SkyArmyName;
                setArmyPrefab(mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName, out prefab);

                mapPointCtrl.SetSkyArmy(prefab);
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
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapPoint"));
                if (isCollider)
                {
                    MapPointCtrl mapPointCtrl = hit.collider.GetComponent<MapPointCtrl>();
                    if(!mapPointCtrl.HasLandArmy() && selectArmyCard!=null&&armyPrefab != null && !selectArmyCard.CanFly)
                    {
                        //放置陆地单位
                        mapPointCtrl.SetLandArmy(armyPrefab);
                        //mapPointCtrl.LandArmyCard = selectArmyCard;
                        mapPointCtrl.LandArmyRace = selectArmyCard.Race;
                        mapPointCtrl.LandArmyName = selectArmyCard.Name;
                        //移除卡牌
                        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectArmyCard);
                        //向服务器发送消息
                        pointDto.Change(mapPointCtrl.mapPoint, mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName);
                        socketMsg.Change(OpCode.FIGHT, FightCode.MAP_SET_ARMY_CREQ, pointDto);
                        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                    }
                    else if(!mapPointCtrl.HasSkyArmy() && selectArmyCard != null&&armyPrefab != null && selectArmyCard.CanFly)
                    {
                        //放置飞行单位
                        mapPointCtrl.SetSkyArmy(armyPrefab);
                        //mapPointCtrl.SkyArmyCard = selectArmyCard;
                        mapPointCtrl.SkyArmyRace = selectArmyCard.Race;
                        mapPointCtrl.SkyArmyName = selectArmyCard.Name;
                        //移除卡牌
                        Dispatch(AreoCode.CHARACTER, CharacterEvent.REMOVE_MY_CARDS, selectArmyCard);
                        //向服务器发送消息
                        pointDto.Change(mapPointCtrl.mapPoint, mapPointCtrl.LandArmyRace, mapPointCtrl.LandArmyName, mapPointCtrl.SkyArmyRace, mapPointCtrl.SkyArmyName);
                        socketMsg.Change(OpCode.FIGHT, FightCode.MAP_SET_ARMY_CREQ, pointDto);
                        Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                    }
                }
            }
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
