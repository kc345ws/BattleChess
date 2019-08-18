using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol.Dto;

/// <summary>
/// 游戏数据模型的储存类
/// </summary>
public class GameModles
{
    private static GameModles instance = new GameModles();
    public static GameModles Instance
    {
        get
        {
            lock (instance)
            {
                if(instance == null)
                {
                    instance = new GameModles();
                }
                return instance;
            }       
        }
    }

    public UserDto userDto { get; set; }

    public MatchRoomDto matchRoomDto { get; set; }

    private GameModles()
    {

    }
}
