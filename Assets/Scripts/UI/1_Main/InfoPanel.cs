using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Dto;

public class InfoPanel : UIBase {
    private Image Image_Head;//头像图片
    private Text Text_Name;//角色名称
    private Text Text_Level;//角色等级
    private Slider Slider_Exp;//角色经验滑动条
    private Text Text_ExpValue;//角色经验值
    private Text Text_Money;//角色金币

    // Use this for initialization
    void Start () {
        Bind(UIEvent.INFO_PANEL_EVENTCODE);
        Image_Head = transform.Find("Image_Head").GetComponent<Image>();
        Text_Name = transform.Find("Text_Name").GetComponent<Text>();
        Text_Level = transform.Find("Text_Level").GetComponent<Text>();
        Slider_Exp = transform.Find("Slider_Exp").GetComponent<Slider>();
        Text_ExpValue = transform.Find("Text_ExpValue").GetComponent<Text>();
        Text_Money = transform.Find("Text_Money").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Execute(int eventcode, object message)
    {
        switch (eventcode)
        {
            case UIEvent.INFO_PANEL_EVENTCODE:
                UserDto user = message as UserDto;
                refreshView(user.Name, user.Lv, user.Exp, user.Been);
                break;
        }
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    private void refreshView(string name,int level,int exp,int money)
    {
        //从服务器获取玩家信息
        int Maxexp = level * 100;
        Text_Name.text = name;
        Text_Level.text = "Lv."+level.ToString();
        //经验公式 经验值 = 等级 * 100;
        Text_ExpValue.text = exp.ToString() + "/"+ Maxexp;
        Text_Money.text = "×"+money.ToString();
        Slider_Exp.value = (float)(exp / Maxexp);
    }
}
