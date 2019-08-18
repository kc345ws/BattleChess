using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每个模块的基类
/// 保存自身注册的一些信息
/// </summary>
public class ManagerBase : MonoBase {

    /// <summary>
    /// 字典存储事件码以及相关联的脚本
    ///  角色模块   有一个动作是 移动
    ///  移动模块   需要关心这个事件 控制角色位置进行移动
    ///  动画模块   也需要关心  控制角色播放动画    
    ///  音效模块   也需要关心  控制角色移动的音效播放走路声
    /// </summary>
    private Dictionary<int, List<MonoBase>> dictionary;

    public ManagerBase()
    {
        dictionary = new Dictionary<int, List<MonoBase>>();
    }

    /// <summary>
    /// 处理自身消息
    /// </summary>
    /// <param name="eventcode">事件码</param>
    /// <param name="message">消息</param>
    public override void Execute(int eventcode, object message)
    {

        if (!dictionary.ContainsKey(eventcode))
        {
            //如果事件码没有注册过
            Debug.Log("事件码没有注册：" + eventcode);
            return;
        }

        //如果注册过这个事件 则给所有相关联的脚本发送
        List<MonoBase> monobases = dictionary[eventcode];
        for(int i = 0; i < monobases.Count; i++)
        {
            if(monobases[i] == null)
            {
                monobases.Remove(monobases[i]);
                continue;
            }
            monobases[i].Execute(eventcode, message);
        }
    }


    /// <summary>
    /// 添加事件码与脚本相关联
    /// </summary>
    /// <param name="eventcode"></param>
    /// <param name="monobase"></param>
    public void Add(int eventcode , MonoBase monobase)
    {
        if (!dictionary.ContainsKey(eventcode))
        {
            //如果事件码还没有注册过
            //则说明也没有脚本与之关联
            List<MonoBase> monoBases = new List<MonoBase>();
            monoBases.Add(monobase);
            dictionary.Add(eventcode, monoBases);
            return;
        }

        //如果事件码已经注册过 则只需要增加与事件码关联的脚本
        else
        {
            List<MonoBase> monoBases = dictionary[eventcode];
            monoBases.Add(monobase);
        }
    }

    /// <summary>
    /// 将脚本与多个事件码关联
    /// </summary>
    /// <param name="eventcodes"></param>
    /// <param name="monoBase"></param>
    public void Add(int[] eventcodes , MonoBase monoBase)
    {
        for(int i = 0; i < eventcodes.Length; i++)
        {
            Add(eventcodes[i], monoBase);
        }
    }

    /// <summary>
    /// 删除脚本与一个事件码的关联
    /// </summary>
    /// <param name="eventcode"></param>
    /// <param name="monobase"></param>
    public void Remove(int eventcode , MonoBase monobase)
    {
        if (!dictionary.ContainsKey(eventcode))
        {
            Debug.LogWarning("无法删除没有注册的事件码:" + eventcode);
            return;
        }

        else
        {
            List<MonoBase> monoBases = dictionary[eventcode];
            if(monoBases.Count == 1)
            {
                //如果与这个事件码关联的脚本数量只有一个
                //则直接从字典中删除
                dictionary.Remove(eventcode);
            }
            else
            {
                //如果与这个事件码关联的脚本数量不只有一个
                //则只将这个脚本从脚本列表中删除
                monoBases.Remove(monobase);
            }
        }
    }

    /// <summary>
    /// 删除脚本与多个事件码的关联
    /// </summary>
    /// <param name="eventcodes"></param>
    /// <param name="monoBase"></param>
    public void Remove(int[] eventcodes , MonoBase monoBase)
    {
        for(int i = 0; i < eventcodes.Length; i++)
        {
            Remove(eventcodes[i], monoBase);
        }
    }
}
