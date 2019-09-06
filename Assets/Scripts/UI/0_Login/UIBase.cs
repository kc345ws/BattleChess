using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBase {
    /// <summary>
    /// 自身消息集合
    /// </summary>
    private List<int> EventcodeList = null;

    public UIBase()
    {
        EventcodeList = new List<int>();
    }

    /// <summary>
    /// 将一个或多个事件码与脚本关联
    /// </summary>
    /// <param name="eventcodes"></param>
    protected void Bind(params int[] eventcodes)
    {
        EventcodeList.AddRange(eventcodes);
        //一次性将集合中的元素全部添加到List中

        //fixbug
        //UIManager.Instance.Add(EventcodeList.ToArray(), this);
        UIManager.Instance.Add(eventcodes, this);
    }

    /// <summary>
    /// 将脚本与所有事件码解除关联
    /// </summary>
    protected void UnBind()
    {
        UIManager.Instance.Remove(EventcodeList.ToArray(), this);
        EventcodeList.Clear();
    }

    /// <summary>
    /// 当脚本销毁后将自动解除与事件码的关联
    /// </summary>
    public virtual void OnDestroy()
    {
        if (EventcodeList != null)
        {
            UnBind();
        }
    }

    /// <summary>
    /// 向其他模块发送消息
    /// </summary>
    /// <param name="areocode"></param>
    /// <param name="eventcode"></param>
    /// <param name="message"></param>
    public void Dispatch(int areocode , int eventcode , object message)
    {
        MsgCenter.Instance.Dispatch(areocode, eventcode, message);
    }

    

    /// <summary>
    /// 设置脚本关联游戏物体的Active
    /// </summary>
    /// <param name="active"></param>
    public void SetPanelActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
