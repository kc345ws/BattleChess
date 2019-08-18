using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景管理类
/// </summary>
public class SceneMgr : ManagerBase {
    public static SceneMgr Instance = null;

    private SceneLoadMsg loadMsg;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        Add(SceneCode.LOAD_SCENE, this);
    }

    private void Start()
    {
        //loadMsg = new SceneLoadMsg();
    }

    public override void Execute(int eventcode, object message)
    {
        //string str;
        switch (eventcode)
        {
            case SceneCode.LOAD_SCENE:
                //str = message as string;
                //loadMsg.Change(1, "02_Main", SceneManager.sceneLoaded);
                loadMsg = message as SceneLoadMsg;
                loadMsg.Change(loadMsg.Index, loadMsg.Name, loadMsg.LoadAction);
                StartCoroutine(loadScene(loadMsg));
                //MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "切换场景成功");
                break;
        }
    }

    private IEnumerator loadScene(SceneLoadMsg loadMsg)
    {
        //登陆成功切换场景
        if(loadMsg.Index != -1)
        {

            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(loadMsg.Index);    
        }else if (loadMsg.Name != null)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(loadMsg.Name);            
        }
        
    }

    //当场景加载成功后触发的方法
    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode1)
    {
        if (loadMsg != null && loadMsg.LoadAction != null)
        {
            loadMsg.LoadAction();
        }
    }
}
