using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 等待时遮盖游戏界面
/// </summary>
public class HidePlane : UIBase
{
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_HIDE_PLANE);
        Bind(UIEvent.CLOSE_HIDE_PLANE);

        SetPanelActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.SHOW_HIDE_PLANE:
                SetPanelActive(true);
                break;

            case UIEvent.CLOSE_HIDE_PLANE:
                SetPanelActive(false);
                break;
        }
    }
}
