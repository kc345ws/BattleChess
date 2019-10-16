using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 光标管理类
/// </summary>
class CursorManager : UIBase
{
    /// <summary>
    /// 
    /// 正常形态
    /// </summary>
    private Texture2D Normal;

    /// <summary>
    /// 攻击形态
    /// </summary>
    private Texture2D Attack;

    /// <summary>
    /// 治疗形态
    /// </summary>
    private Texture2D Heart;
    // Start is called before the first frame update
    void Start()
    {
        Bind(UIEvent.CURSOR_SET_NORMAL);
        Bind(UIEvent.CURSOR_SET_ATTACK);
        Bind(UIEvent.CURSOR_SET_HEART);

        Normal = Resources.Load<Texture2D>("Texture/Cursor/Normal");
        Attack = Resources.Load<Texture2D>("Texture/Cursor/Attack");
        Heart = Resources.Load<Texture2D>("Texture/Cursor/Heart");
        //Cursor.SetCursor(Heart, Vector2.zero, CursorMode.Auto);
        Cursor.SetCursor(Normal, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Execute(int eventcode, object message)
    {
        //base.Execute(eventcode, message);
        switch (eventcode)
        {
            case UIEvent.CURSOR_SET_NORMAL:
                Cursor.SetCursor(Normal, Vector2.zero, CursorMode.Auto);
                break;

            case UIEvent.CURSOR_SET_ATTACK:
                Cursor.SetCursor(Attack, Vector2.zero, CursorMode.Auto);
                break;

            case UIEvent.CURSOR_SET_HEART:
                Cursor.SetCursor(Heart, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}
