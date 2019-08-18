using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIBase {
    private Button startbtn;
    private Button registerbtn;
	// Use this for initialization
	void Start () {
        
        startbtn = transform.Find("Button_Login").GetComponent<Button>();
        registerbtn = transform.Find("Button_Register").GetComponent<Button>();

        startbtn.onClick.AddListener(startBtnClicker);
        registerbtn.onClick.AddListener(registerBtnClicker);
	}

    

    public override void OnDestroy()
    {
        base.OnDestroy();

        startbtn.onClick.RemoveAllListeners();
        registerbtn.onClick.RemoveAllListeners();
    }

    private void startBtnClicker()
    {
        MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.LOGIN_PANEL_EVENTCODE, true);
    }

    private void registerBtnClicker()
    {
        MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.REGISTER_PANEL_EVENTCODE, true);
    }
	
}
