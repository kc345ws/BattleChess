using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Code;

namespace Assets.Scripts.Net
{
    /// <summary>
    /// 网络消息处理基类
    /// </summary>
    public abstract class HandlerBase
    {
        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="subcode"></param>
        /// <param name="message"></param>
        public abstract void OnReceive(int subcode, object message);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="subcode"></param>
        /// <param name="message"></param>
        public void Dispatch(int areocode , int eventcode , object message)
        {
            //向消息中心发送消息
            MsgCenter.Instance.Dispatch(areocode, eventcode, message);
        }
    }
}
