using Protocol.Dto.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyArmyCtrls : MonoBehaviour
{
    private List<ArmyCtrl> CardCtrllist;//兵种控制器集合

    // Start is called before the first frame update
    void Start()
    {
        CardCtrllist = new List<ArmyCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Add(ArmyCtrl armyCtrl , CardDto cardDto , MapPointCtrl mapPointCtrl)
    {
        armyCtrl.Init(cardDto, mapPointCtrl);
        CardCtrllist.Add(armyCtrl);      
    }
}
