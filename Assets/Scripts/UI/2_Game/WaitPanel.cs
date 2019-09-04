using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 等待对方操作结束
/// </summary>
public class WaitPanel : UIBase
{
    private Text information;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.SHOW_WAIT_PANEL);
        Bind(UIEvent.CLOSE_WAIT_PANEL);
        information = transform.Find("information").GetComponent<Text>();

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
            case UIEvent.SHOW_WAIT_PANEL:
                processShowWait((string)message);
                break;

            case UIEvent.CLOSE_WAIT_PANEL:
                SetPanelActive(false);
                break;
        }
    }

    private void processShowWait(string str)
    {
        ChangeInfo(str);
        SetPanelActive(true);
    }

    private void ChangeInfo(string str)
    {
        information.text = str;
    }
}
