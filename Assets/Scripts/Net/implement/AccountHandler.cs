using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Code;
using UnityEngine;

namespace Assets.Scripts.Net.implement
{
    /// <summary>
    /// 帐号网络数据处理
    /// </summary>
    public class AccountHandler : HandlerBase
    {
        private static AccountHandler instance = null;
        public static AccountHandler Instance
        {
       
     get
            {
                if (instance == null)
                {
                    instance = new AccountHandler();
                }
                return instance;
            }
        }
        private SceneLoadMsg sceneloadMsg = new SceneLoadMsg();
        private SocketMsg socketMsg = new SocketMsg();

        private AccountHandler() { }

        public override void OnReceive(int subcode , object message)
        {
            switch (subcode)
            {
                //服务器登陆回复
                case AccountCode.LOGIN_SRES:
                    loginResponse(message);
                    break;

                //服务器注册回复
                case AccountCode.REGISTER_SRES:
                    registerResponse(message);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 服务器登陆回复
        /// </summary>
        /// <param name="result"></param>
        private void loginResponse(object result)
        {
            string str = result.ToString();
            MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, str);
            if (str == "登陆成功")
            {
                
                sceneloadMsg.Change(1, "1_Main", () =>
                 {
                     //向服务器获取角色信息
                     socketMsg.Change(OpCode.USER, UserCode.GET_USER_CREQ, "0");
                     Dispatch(AreoCode.NET, NetEvent.SENDMSG, socketMsg);
                     Debug.Log("登陆成功，切换场景成功");
                 });
                MsgCenter.Instance.Dispatch(AreoCode.SCENE, SceneCode.LOAD_SCENE, sceneloadMsg);
            }
        }

        private void registerResponse(object result)
        {
            string str = result.ToString();
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, str);
            if(str == "注册成功")
            {
                Dispatch(AreoCode.UI, UIEvent.REGISTER_PANEL_EVENTCODE, false);
            }

            /*else
            {
                MsgCenter.Instance.Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, str);
            }*/
        }
    }
}
