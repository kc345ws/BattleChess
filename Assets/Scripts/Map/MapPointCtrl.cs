using UnityEngine;
using System.Collections;
using Protocol.Constants.Map;
using Protocol.Dto.Fight;

/// <summary>
/// 地图点控制类
/// </summary>
public class MapPointCtrl : MapBase
{
    public GameObject LandArmy { get; private set; }//陆地单位
    public GameObject SkyArmy { get; private set; }//飞行单位
    public GameObject UnderGroundArmy { get; private set; }//地下生物
    public GameObject ParsiticArmy { get; private set; }//寄生生物

    private Renderer renderer;//地图点所在地块的渲染器

    public Color origanColor { get; private set; }//原本的颜色

    public MapPoint mapPoint { get; private set; }//地图点

    private int ArmyCount = 0;//该地块上单位数量

    private bool isOtherShow = false;

    //public CardDto LandArmyCard { get; set; }
    //public CardDto SkyArmyCard { get; set; }
    public int LandArmyRace = -1;
    public int LandArmyName = -1;
    public int SkyArmyRace = -1;
    public int SkyArmyName = -1;
    public int UnderArmyRace = -1;
    public int UnderArmyName = -1;
    public int ParsiticRace = -1;
    public int ParsiticName = -1;

    // Use this for initialization
    void Start()
    {
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;
        mapPoint = new MapPoint(x, z);

        renderer = GetComponent<Renderer>();
        origanColor = renderer.material.color;

        MapManager.mapPointCtrls.Add(this);
        if(gameObject.tag == "OtherWinline")
        {
            //如果是敌方胜利线
            MapManager.OtherLineMapPointCtrls.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ArmyCount >= 2)
        {
            //如果地块上有两个以上单位，屏蔽查看对方属性
            OtherArmyCtrl otherArmyCtrl = null;
            if(LandArmy !=null)
            otherArmyCtrl = LandArmy.gameObject.GetComponent<OtherArmyCtrl>();

            if(otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = false;
            }

            if(SkyArmy !=null)
            otherArmyCtrl = SkyArmy.gameObject.GetComponent<OtherArmyCtrl>();

            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = false;
            }

            if(UnderGroundArmy !=null)
            otherArmyCtrl = UnderGroundArmy.gameObject.GetComponent<OtherArmyCtrl>();

            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = false;
            }

            if(ParsiticArmy!=null)
            otherArmyCtrl = ParsiticArmy.gameObject.GetComponent<OtherArmyCtrl>();

            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = false;
            }
            isOtherShow = false;
        }
        else
        {
            isOtherShow = true;
        }


        if (isOtherShow)
        {
            //如果地块上有两个以上单位，屏蔽查看对方属性
            OtherArmyCtrl otherArmyCtrl = null;
            if (LandArmy!=null)
            {
                otherArmyCtrl = LandArmy.gameObject.GetComponent<OtherArmyCtrl>();
            }
            
            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = true;
            }

            if(SkyArmy !=null)
            otherArmyCtrl = SkyArmy.gameObject.GetComponent<OtherArmyCtrl>();

            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = true;
            }

            if(UnderGroundArmy!=null)
            otherArmyCtrl = UnderGroundArmy.gameObject.GetComponent<OtherArmyCtrl>();
            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = true;
            }

            if(ParsiticArmy!=null)
            otherArmyCtrl = ParsiticArmy.gameObject.GetComponent<OtherArmyCtrl>();
            if (otherArmyCtrl != null)
            {
                otherArmyCtrl.iscanShowStatePanel = true;
            }
            isOtherShow = false;
        }
    }



    /// <summary>
    /// 设置地块颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    #region 陆地单位
    /// <summary>
    /// 该地图点上是否有陆地单位
    /// </summary>
    /// <returns></returns>
    public bool HasLandArmy()
    {
        return LandArmy != null;
    }

    /// <summary>
    /// 在地图点上放置陆地单位
    /// </summary>
    /// <param name="landArmy"></param>
    public GameObject SetLandArmy(GameObject landArmy)
    {
        LandArmy = landArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        ArmyCount++;
        return GameObject.Instantiate(landArmy, position, landArmy.transform.rotation);  
    }

    /// <summary>
    /// 更新陆地单位
    /// </summary>
    /// <param name="landArmy"></param>
    /// <returns></returns>
    public void UpdateLandArmy(GameObject landArmy)
    {
        LandArmy = landArmy;
    }

    /// <summary>
    /// 移动陆地单位
    /// </summary>
    /// <param name="landArmy"></param>
    /// <param name="race"></param>
    /// <param name="name"></param>
    public void MoveLandArmy(ref GameObject landArmy, int race, int name)
    {
        LandArmy = landArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        landArmy.transform.position = position;
        LandArmyRace = race;
        LandArmyName = name;
        ArmyCount++;
    }

    /// <summary>
    /// 移除陆地单位
    /// </summary>
    public void RemoveLandArmy()
    {
        //GameObject.Destroy(LandArmy.gameObject);
        LandArmy = null;
        LandArmyRace = -1;
        LandArmyName = -1;
        ArmyCount--;
    }
    #endregion

    #region 飞行单位

    /// <summary>
    /// 该地图点上是否有飞行单位
    /// </summary>
    /// <returns></returns>
    public bool HasSkyArmy()
    {
        return SkyArmy != null;
    }

    public GameObject SetSkyArmy(GameObject skyarmy)
    {
        SkyArmy = skyarmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        ArmyCount++;
        return GameObject.Instantiate(skyarmy, position, skyarmy.transform.rotation);
    }

    public void MoveSkyArmy(ref GameObject skyArmy, int race, int name)
    {
        SkyArmy = skyArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        skyArmy.transform.position = position;
        SkyArmyRace = race;
        SkyArmyName = name;
        ArmyCount++;
    }

    public void UpdateSkyArmy(GameObject skyarmy)
    {
        SkyArmy = skyarmy;
    }

    /// <summary>
    /// 移除天空单位
    /// </summary>
    public void RemoveSkyArmy()
    {
        //GameObject.Destroy(SkyArmy.gameObject);
        SkyArmy = null;
        SkyArmyRace = -1;
        SkyArmyName = -1;
        ArmyCount--;
    }



    #endregion

    #region 地下单位

    /// <summary>
    /// 是否有地下单位
    /// </summary>
    /// <returns></returns>
    public bool HasUnderArmy()
    {
        return UnderGroundArmy != null;
    }

    /// <summary>
    /// 设置地下单位
    /// </summary>
    /// <param name="skyarmy"></param>
    /// <returns></returns>
    public GameObject SetUnderArmy(GameObject underarmy)
    {
        UnderGroundArmy = underarmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        ArmyCount++;
        return GameObject.Instantiate(underarmy, position, underarmy.transform.rotation);
    }

    /// <summary>
    /// 移动地下单位
    /// </summary>
    /// <param name="skyArmy"></param>
    /// <param name="race"></param>
    /// <param name="name"></param>
    public void MoveUnderArmy(ref GameObject underArmy, int race, int name)
    {
        UnderGroundArmy = underArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        underArmy.transform.position = position;
        UnderArmyRace = race;
        UnderArmyName = name;
        ArmyCount++;
    }

    public void UpdateUnderArmy(GameObject underarmy)
    {
        UnderGroundArmy = underarmy;
    }

    /// <summary>
    /// 移除地下单位
    /// </summary>
    public void RemoveUnderArmy()
    {
        //GameObject.Destroy(SkyArmy.gameObject);
        UnderGroundArmy = null;
        UnderArmyRace = -1;
        UnderArmyName = -1;
        ArmyCount--;
    }

    #endregion

    #region 寄生单位

    /// <summary>
    /// 是否有寄生单位
    /// </summary>
    /// <returns></returns>
    public bool HasParsiticArmy()
    {
        return ParsiticArmy != null;
    }

    /// <summary>
    /// 设置寄生单位
    /// </summary>
    /// <param name="skyarmy"></param>
    /// <returns></returns>
    public GameObject SetParsiticArmy(GameObject parsiticarmy)
    {
        ParsiticArmy = parsiticarmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        ArmyCount++;
        return GameObject.Instantiate(parsiticarmy, position, parsiticarmy.transform.rotation);
    }

    /// <summary>
    /// 移动寄生1单位
    /// </summary>
    /// <param name="skyArmy"></param>
    /// <param name="race"></param>
    /// <param name="name"></param>
    public void MoveParsiticArmy(ref GameObject parsiticArmy, int race, int name)
    {
        ParsiticArmy = parsiticArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        parsiticArmy.transform.position = position;
        UnderArmyRace = race;
        UnderArmyName = name;
        ArmyCount++;
    }

    public void UpdateParsiticArmy(GameObject parsiticarmy)
    {
       ParsiticArmy = parsiticarmy;
    }

    /// <summary>
    /// 移除地下单位
    /// </summary>
    public void RemoveParsiticArmy()
    {
        //GameObject.Destroy(SkyArmy.gameObject);
        ParsiticArmy = null;
        ParsiticRace = -1;
        ParsiticName = -1;
        ArmyCount--;
    }

    #endregion


    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }
}
