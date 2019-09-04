using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制指向被攻击兵种的箭头
/// </summary>
public class AttackArrowCtrl : UIBase
{
    private GameObject arrow;

    //private float Timer = 0;

    private Vector3 arrowPos;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_ATTACK_ARROW);
        Bind(UIEvent.CLOSE_ATTACK_ARROW);

        arrow = transform.Find("Arrow").gameObject;

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
;
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.SHOW_ATTACK_ARROW:
                SetPanelActive(true);
                ChangePos((Vector3)message);
                break;

            case UIEvent.CLOSE_ATTACK_ARROW:
                SetPanelActive(false);
                break;
        }
    }

    /// <summary>
    /// 改变出现位置
    /// </summary>
    private void ChangePos(Vector3 pos)
    {
        transform.position = pos;
    }
}
