using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消息处理中心
/// 只负责消息的转发
/// ui->msgcenter->其他模块
/// </summary>
public class MsgCenter : MonoBase {
    public static MsgCenter Instance = null;

    private void Awake()
    {
        Instance = this;

        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<NetManager>();
        gameObject.AddComponent<SceneMgr>();
        gameObject.AddComponent<AudioManager>();
        gameObject.AddComponent<CharacterManager>();
        gameObject.AddComponent<BgmManager>();

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 发送消息 系统里面所有的发消息 都通过这个方法来发
    ///   怎么转发？根据不同的模块 来发给 不同的模块 
    ///     怎么识别模块呢？通过 areaCode
    /// 
    ///     第二个参数：事件码 作用？用来区分 做什么事情的
    ///         比如说 第一个参数 识别到是角色模块 但是角色模块有很多功能 比如 移动 攻击 死亡 逃跑...
    ///             就需要第二个参数 来识别 具体是做哪一个动作
    /// </summary>
    public void Dispatch(int areocode , int eventcode , object message)
    {
        switch (areocode)
        {
            case AreoCode.UI:
                UIManager.Instance.Execute(eventcode, message);
                break;

            case AreoCode.NET:
                NetManager.Instance.Execute(eventcode, message);
                break;

            case AreoCode.SCENE:
                SceneMgr.Instance.Execute(eventcode, message);
                break;

            case AreoCode.AUDIO:
                AudioManager.Instance.Execute(eventcode, message);
                break;

            case AreoCode.CHARACTER:
                CharacterManager.Instance.Execute(eventcode, message);
                break;
        }
    }
}
