using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIBase {
    private Button Button_Setting;
    private Button Button_Close;
    private Button Button_Quit;
    private Image Image_BG;
    private Text Text_Setting;
    private Text Text_isAudio;
    private Text Text_Audio;
    private Toggle Toggle_isAudio;
    private Slider Slider_Audio;

    private bool isShow = false;
    private bool isBgmopen = true;
    // Use this for initialization
    void Start () {
        Button_Setting = transform.Find("Button_Setting").GetComponent<Button>();
        Button_Close = transform.Find("Button_Close").GetComponent<Button>();
        Button_Quit = transform.Find("Button_Quit").GetComponent<Button>();
        Image_BG = transform.Find("Image_BG").GetComponent<Image>();
        Text_Setting = transform.Find("Text_Setting").GetComponent<Text>();
        Text_isAudio = transform.Find("Text_isAudio").GetComponent<Text>();
        Text_Audio = transform.Find("Text_Audio").GetComponent<Text>();
        Toggle_isAudio = transform.Find("Toggle_isAudio").GetComponent<Toggle>();
        Slider_Audio = transform.Find("Slider_Audio").GetComponent<Slider>();

        Button_Setting.onClick.AddListener(settingBtnClicker);
        Button_Close.onClick.AddListener(closeBtnClicker);
        Button_Quit.onClick.AddListener(quitBtnClcker);

        Slider_Audio.onValueChanged.AddListener(changeBgmValue);
        Toggle_isAudio.onValueChanged.AddListener(isBgm);
        Toggle_isAudio.isOn = true;

        setObjectActive(isShow);
        Button_Setting.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void isBgm(bool active)
    {
        if (active)
        {
            isBgmopen = true;
            Dispatch(AreoCode.AUDIO, AudioEvent.PLAY_BG_AUDIO, "打开背景音乐");
        }
        else
        {
            isBgmopen = false;
            Dispatch(AreoCode.AUDIO, AudioEvent.STOP_BGM, "关闭背景音乐");
        }
    }

    private void changeBgmValue(float value)
    {
        if (isBgmopen)
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.CHANGE_BGM_VOLUME, value);
        }
        else
        {
            Dispatch(AreoCode.AUDIO, AudioEvent.STOP_BGM, "关闭背景音乐");
        }
    }

    private void setObjectActive(bool active)
    {
        Image_BG.gameObject.SetActive(active);
        Text_Setting.gameObject.SetActive(active);
        Text_isAudio.gameObject.SetActive(active);
        Text_Audio.gameObject.SetActive(active);
        Toggle_isAudio.gameObject.SetActive(active);
        Slider_Audio.gameObject.SetActive(active);
        Button_Close.gameObject.SetActive(active);
        Button_Quit.gameObject.SetActive(active);
    }

    private void settingBtnClicker()
    {
        isShow = !isShow;
        setObjectActive(isShow);
    }

    private void closeBtnClicker()
    {
        isShow = !isShow;
        setObjectActive(false);
    }

    private void quitBtnClcker()
    {
        Application.Quit();
    }
}
