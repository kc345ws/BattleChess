using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol.Code;

public class PromptPanel : UIBase {
    private static PromptPanel instance = null;
    public static PromptPanel Instance { get
        {
            lock (instance)
            {
                if(instance == null)
                {
                    instance = new PromptPanel();
                }
                return instance;
            }
        } }

    private Text TXT_Prompt;

    private float Timer = 0f;

    [Range(0,3)]
    public float ShowTime = 2f;

	// Use this for initialization
	void Start () {
        Bind(UIEvent.PROMPT_PANEL_EVENTCODE, AccountCode.LOGIN_SRES, AccountCode.REGISTER_SRES);
        /*Bind(UIEvent.PROMPT_PANEL_EVENTCODE);
        Bind(AccountCode.LOGIN_SRES);
        Bind(AccountCode.REGISTER_SRES);*/

        TXT_Prompt = transform.Find("TXT_Prompt").GetComponent<Text>();

        //面板默认隐藏
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        instance = new PromptPanel();    
    }

    public override void Execute(int eventcode, object message)
    {
        string str = null;
        switch (eventcode)
        {
            case UIEvent.PROMPT_PANEL_EVENTCODE:
                str = message as string;
                SetText(str);
                break;

            case AccountCode.LOGIN_SRES:
                str = message as string;
                SetText(str);
                break;

            case AccountCode.REGISTER_SRES:
                str = message as string;
                SetText(str);
                break;

            default:
                break;
        }
    }

    public void SetText(string str)
    {
        TXT_Prompt.text = str;
        gameObject.SetActive(true);
        StartCoroutine(ShowText());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    IEnumerator ShowText()
    {    
        while (Timer <= ShowTime)
        {
            Timer += Time.deltaTime;   
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        Timer = 0f;
    }
}
