using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SceneLoadMsg
{
    /// <summary>
    /// 场景索引
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 场景名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 当场景加载成功的委托
    /// </summary>
    public Action LoadAction { get; set; }

    public SceneLoadMsg()
    {
        Index = -1;
        Name = null;
        LoadAction = null;
    }

    public SceneLoadMsg(int index , string name , Action action)
    {
        Index = index;
        Name = name;
        LoadAction = action;
    }

    public void Change(int index, string name, Action action)
    {
        Index = index;
        Name = name;
        LoadAction = action;
    }
}

