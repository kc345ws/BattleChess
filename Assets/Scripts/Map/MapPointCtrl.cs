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
    private Renderer renderer;//地图点所在地块的渲染器

    public Color origanColor { get; private set; }//原本的颜色

    public MapPoint mapPoint { get; private set; }//地图点

    //public CardDto LandArmyCard { get; set; }
    //public CardDto SkyArmyCard { get; set; }
    public int LandArmyRace = -1;
    public int LandArmyName = -1;
    public int SkyArmyRace = -1;
    public int SkyArmyName = -1;

    // Use this for initialization
    void Start()
    {
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;
        mapPoint = new MapPoint(x, z);

        renderer = GetComponent<Renderer>();
        origanColor = renderer.material.color;

        MapManager.mapPointCtrls.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 设置地块颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    /// <summary>
    /// 该地图点上是否有陆地单位
    /// </summary>
    /// <returns></returns>
    public bool HasLandArmy()
    {
        return LandArmy != null;
    }

    /// <summary>
    /// 该地图点上是否有飞行单位
    /// </summary>
    /// <returns></returns>
    public bool HasSkyArmy()
    {
        return SkyArmy != null;
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
        return GameObject.Instantiate(landArmy, position, landArmy.transform.rotation);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="landArmy"></param>
    /// <returns></returns>
    public void UpdateLandArmy(GameObject landArmy)
    {
        LandArmy = landArmy;
    }

    public void MoveLandArmy(ref GameObject landArmy)
    {
        LandArmy = landArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        landArmy.transform.position = position;
    }

    public void MoveSkyArmy(ref GameObject skyArmy)
    {
        SkyArmy = skyArmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        skyArmy.transform.position = position;
    }

    /// <summary>
    /// 移除陆地单位
    /// </summary>
    public void RemoveLandArmy()
    {
        //GameObject.Destroy(LandArmy.gameObject);
        LandArmy = null;
    }

    public GameObject SetSkyArmy(GameObject skyarmy)
    {
        SkyArmy = skyarmy;
        Vector3 position = transform.transform.position;
        position.y = 1;
        return GameObject.Instantiate(skyarmy, position, skyarmy.transform.rotation);
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
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }
}
