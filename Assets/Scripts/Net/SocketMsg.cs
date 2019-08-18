using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    /// <summary>
    /// 网络消息传递类
    /// </summary>
    /// 
    public class SocketMsg
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public int OpCode { get; set; }

        /// <summary>
        /// 子操作
        /// </summary>
        public int SubCode { get; set; }

        /// <summary>
        /// 数据值
        /// </summary>
        public object Value { get; set; }

        public SocketMsg()
        {

        }

        public SocketMsg(int opcode , int subcode , object value)
        {
            OpCode = opcode;
            SubCode = subcode;
            Value = value;
        }

        public void Change(int opcode, int subcode, object value)
        {
        OpCode = opcode;
        SubCode = subcode;
        Value = value;
    }


    }

