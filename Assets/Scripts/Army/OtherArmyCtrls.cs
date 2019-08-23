using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 其他人的兵种管理器集合
/// </summary>
public class OtherArmyCtrls : ArmyBase
{
    public static OtherArmyCtrls Instance = null;

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 兵种集合
    /// </summary>
    public static List<GameObject> ArmyList { get; private set; }
    // Use this for initialization
    void Start()
    {
        ArmyList = new List<GameObject>();

        Bind(ArmyEvent.ADD_OTHER_ARMY);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case ArmyEvent.ADD_OTHER_ARMY:
                ArmyList.Add(message as GameObject);
                break;
        }
    }
}
