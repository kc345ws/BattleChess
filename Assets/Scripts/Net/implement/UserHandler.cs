using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Code;
using Protocol.Dto;
using UnityEngine;

namespace Assets.Scripts.Net.implement
{
    /// <summary>
    /// 角色信息处理类
    /// </summary>
    public class UserHandler : HandlerBase
    {

        private static UserHandler instance = null;

        public static UserHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserHandler();
                }
                return instance;
            }
        }

        private UserHandler() { }

        private UserDto userDto = null;
        private SocketMsg socketMsg = new SocketMsg();

        public override void OnReceive(int subcode, object message)
        {
            switch (subcode)
            {
                case UserCode.CREATE_USER_SRES:
                    processCreate((int)message);
                    break;
                case UserCode.GET_USER_SRES:
                    userDto = message as UserDto;
                    processGetuser(userDto);
                    break;

                case UserCode.LOGIN_USER_SRES:
                    processlogin((int)(message));
                    break;
            }
        }

        private void processGetuser(UserDto userDto)
        {
            if(userDto == null)
            {
                //如果角色为空显示创建面板
                Dispatch(AreoCode.UI, UIEvent.SHOW_CREATE_PANEL, true);
                return;
            }

            //如果角色不为空隐藏创建面板
            Dispatch(AreoCode.UI, UIEvent.SHOW_CREATE_PANEL, false);

            /*//角色上线
            socketMsg.Change(OpCode.USER, UserCode.LOGIN_USER_CREQ, null);
            Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);*/
            //优化：当服务器接收到获取数据的请求成功后自动在服务器进行上线

            //在本地保存角色信息
            GameModles.Instance.userDto = userDto;
            Dispatch(AreoCode.UI, UIEvent.INFO_PANEL_EVENTCODE, userDto);
        }

        /// <summary>
        /// 处理上线
        /// </summary>
        private void processlogin(int result)
        {
            if(result == 0)
            {
                //上线成功
                Debug.Log("角色上线成功");
            }else if(result == -2)
            {
                Debug.LogError("没有角色 不能上线");
                return;
            }else if(result == -1)
            {
                Debug.LogError("客户端非法登录");
                return;
            }
        }

        private void processCreate(int result)
        {
            if(result == 0)
            {
                //创建成功
                //隐藏创建面板
                Dispatch(AreoCode.UI, UIEvent.SHOW_CREATE_PANEL, false);
                //获取角色信息
                socketMsg.Change(OpCode.USER, UserCode.GET_USER_CREQ, "0");
                Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                //提示信息
                Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "角色创建成功");
            }else if(result == -1)
            {
                Debug.LogError("客户端非法登录");
            }else if(result == -2)
            {
                Debug.LogError("已经有角色 不能重复创建");
            }
        }
    }
}
