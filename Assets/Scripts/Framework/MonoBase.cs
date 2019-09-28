using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 为什么写空脚本？
/// 扩展MonoBehaviour
/// </summary>
public class MonoBase : MonoBehaviour {
    /// <summary>
    /// 定义一个虚方法
    /// </summary>
    /// <param name="eventcode"></param>
    /// <param name="message"></param>
	public virtual void Execute(int eventcode , object message)
    {

    }

    public MonoBase() { }
}
