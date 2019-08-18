using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


    /// <summary>
    /// 编码工具类
    /// 解决粘包和拆包问题
    /// 处理SocketMgr
    /// </summary>
    public static class EncodeTool
    {
        #region 粘包拆包问题解决
        /// <summary>
        /// 构造消息体 : 消息头 + 消息尾
        /// 消息头 : 数据长度(int类型4字节)  消息尾 : 数据
        /// </summary>
        /// <param name="data">原数据</param>
        /// <returns></returns>
        public static byte[] EncodeMessage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(data.Length);//先写入数据长度
                    bw.Write(data);//再写入数据

                    //byte[] buffer = new byte[ms.Length];//长度为 4+原数据长度
                    //Buffer.BlockCopy(ms.ToArray(), 0, buffer, 0, (int)ms.Length);
                    return ms.GetBuffer();
                    //return ms.ToArray();
                }
            }
            //using代码段中的代码，结束后自动释放内存
        }

        /// <summary>
        /// 解析客户端消息
        /// </summary>
        /// <returns></returns>
        public static byte[] DecodeMessage(ref List<byte> data)
        {
            //ref关键字在方法中修改后，在外部也会产生影响，相当于引用传递
            if (data.Count < 4)
            {
                //throw new Exception("数据长度不足4，无法构成完成数据");
                return null;
            }
            using (MemoryStream ms = new MemoryStream(data.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();//数据实际长度
                    int restlength = (int)(ms.Length - ms.Position);//剩余长度
                if (length > restlength)
                    {
                        //throw new Exception("剩余数据无法构成一个完成数据包");
                        return null;
                    }

                    byte[] buffer = br.ReadBytes(length);

                    //更新缓存区
                    data.Clear();
                    data.AddRange(br.ReadBytes(restlength));
                    //将这个包的信息从缓冲区中删除
                    return buffer;
                }
            }
        }
        #endregion

        #region SocketMgr与byte[]之间的转换
        /// <summary>
        /// 将SocketMgr转化为byte[]
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static byte[] EncodeSocketMgr(SocketMsg mgr)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(mgr.OpCode);
                    bw.Write(mgr.SubCode);
                    if (mgr.Value != null)
                    {
                        byte[] value = EncodeObj(mgr.Value);
                        //将object转化为byte[]
                        bw.Write(value);
                    }
                    return ms.GetBuffer();
                }
            }
        }
        /// <summary>
        /// 将byte[]转化为SocketMgr
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMsg DeCodeSocketMgr(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int OpCode = br.ReadInt32();
                    int SubCode = br.ReadInt32();
                    SocketMsg socketMgr = new SocketMsg();
                    socketMgr.OpCode = OpCode;
                    socketMgr.SubCode = SubCode;
                if (ms.Length > ms.Position)
                    {
                        //如果还有剩余数据
                        object value = DecodeObj(br.ReadBytes((int)(ms.Length - ms.Position)));

                        socketMgr.Value = value;
                }
                
                    
                    return socketMgr;
                }
            }
        }
        #endregion

        #region object与byte[]的转化

        /// <summary>
        /// 将object转化为byte[]进行序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObj(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// 将byte[]转化为object进行反序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DecodeObj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }
        #endregion
    }

