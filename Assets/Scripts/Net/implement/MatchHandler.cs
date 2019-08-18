using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol.Code;
using Protocol.Dto;

namespace Assets.Scripts.Net.implement
{
    /// <summary>
    /// 匹配处理类
    /// </summary>
    public class MatchHandler : HandlerBase
    {
        private static MatchHandler instance = new MatchHandler();
        public static MatchHandler Instance { get
            {
                lock (instance)
                {
                    if(instance == null)
                    {
                        instance = new MatchHandler();
                    }
                    return instance;
                }
            } }

        private MatchHandler() { }

        public override void OnReceive(int subcode, object message)
        {
            switch (subcode)
            {
                //有玩家进入的广播
                case MatchCode.ENTER_BOD:
                    processEnterBOD(message as UserDto);
                    break;

                    //服务器对进入匹配队列请求的回复
                case MatchCode.ENTER_SRES:
                    processEnterSRES(message as MatchRoomDto);
                    break;

                //有玩家离开的广播
                case MatchCode.LEAVE_BOD:
                    processLEAVE_BOD((int)message);
                    break;

                    //有玩家准备的广播
                case MatchCode.READY_BOD:
                    processREADY_BOD((int)message);
                    break;

                    //游戏开始的广播
                case MatchCode.START_GAME_BOD:
                    START_GAME_BOD();
                    break;
            }
        }

        private void processEnterSRES(MatchRoomDto RoomDto)
        {
            //将房间信息保存在本地
            GameModles.Instance.matchRoomDto = RoomDto;
            int myuid = GameModles.Instance.userDto.ID;

            GameModles.Instance.matchRoomDto.ResetPosition(myuid);

            /*if(RoomDto.OtherID != -1)
            {
                UserDto userDto = RoomDto.UidUdtoDic[RoomDto.OtherID];
                Dispatch(AreoCode.UI, UIEvent.SET_OTHER_PLAYER_INFO, userDto);

            }*/

            /*if (RoomDto.Leftid != -1)
            {
                //如果存在左边玩家
                UserDto userDto = RoomDto.UidUdtoDic[RoomDto.Leftid];
                Dispatch(AreoCode.UI, UIEvent.SET_LEFT_PLAYER, userDto);
                //更新玩家面板
                Dispatch(AreoCode.UI, UIEvent.PLAYER_ENTER, userDto.ID);
            }

            if(RoomDto.Rightid != -1)
            {
                //如果存在右边玩家
                UserDto userDto = RoomDto.UidUdtoDic[RoomDto.Rightid];
                Dispatch(AreoCode.UI, UIEvent.SET_RIGHT_PLAYER, userDto);

                Dispatch(AreoCode.UI, UIEvent.PLAYER_ENTER, userDto.ID);
            }*/

            //设置自身玩家信息
            UserDto myuserDto = RoomDto.UidUdtoDic[myuid];
            //Dispatch(AreoCode.UI, UIEvent.SET_MY_PLAYER_STATE, myuserDto);


            //显示进入房间按钮
            Dispatch(AreoCode.UI, UIEvent.SHOW_ROOM_ENTER_BUTTON, "显示进入房间按钮");
            //Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "已匹配到房间");
        }

        private void processEnterBOD(UserDto userDto)
        {
            //把新进入房间的用户信息保存在本地
            GameModles.Instance.matchRoomDto.Add(userDto.ID, userDto);

            int myuid = GameModles.Instance.userDto.ID;
            GameModles.Instance.matchRoomDto.ResetPosition(myuid);

            //当匹配完成却没有进入房间，其他玩家已经匹配到，因为场景没有加载会导致无法设置左右玩家数据
            MatchRoomDto RoomDto = GameModles.Instance.matchRoomDto;

            if (RoomDto.OtherID != -1)
            {
                UserDto otheruserDto = RoomDto.UidUdtoDic[RoomDto.OtherID];
                Dispatch(AreoCode.UI, UIEvent.SET_OTHER_PLAYER_INFO, otheruserDto);

            }

            /*if (RoomDto.Leftid != -1)
            {
                //如果存在左边玩家
                UserDto leftuserDto = RoomDto.UidUdtoDic[RoomDto.Leftid];
                Dispatch(AreoCode.UI, UIEvent.SET_LEFT_PLAYER, leftuserDto);
            }

            if (RoomDto.Rightid != -1)
            {
                //如果存在右边玩家
                UserDto rightuserDto = RoomDto.UidUdtoDic[RoomDto.Rightid];
                Dispatch(AreoCode.UI, UIEvent.SET_RIGHT_PLAYER, rightuserDto);
            }*/

            //更新场景显示玩家面板
            //Dispatch(AreoCode.UI, UIEvent.PLAYER_ENTER, userDto.ID);

            //发送有玩家进入的消息
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, userDto.Name + "进入房间");
        }

        /// <summary>
        /// 有玩家准备的广播
        /// </summary>
        /// <param name="uid"></param>
        private void processLEAVE_BOD(int uid)
        {
            UserDto userDto = GameModles.Instance.matchRoomDto.UidUdtoDic[uid];

            int myuid = GameModles.Instance.userDto.ID;
            GameModles.Instance.matchRoomDto.ResetPosition(myuid);

            

            //更新场景关闭玩家状态面板
            Dispatch(AreoCode.UI, UIEvent.PLAYER_LEAVE, userDto.ID);

            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, userDto.Name + "离开房间");
            //把存在本地的离开玩家信息删除
            //GameModles.Instance.matchRoomDto.UidUdtoDic.Remove(uid);   
            GameModles.Instance.matchRoomDto.Leave(uid);
        }

        /// <summary>
        /// 有玩家准备了的广播
        /// </summary>
        /// <param name="uid"></param>
        private void processREADY_BOD(int uid)
        {
            UserDto userDto = GameModles.Instance.matchRoomDto.UidUdtoDic[uid];
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, userDto.Name + "准备了");
            //GameModles.Instance.matchRoomDto.ReadyUidlist.Add(uid);
            GameModles.Instance.matchRoomDto.Ready(uid);

            //更新场景显示准备文字
            Dispatch(AreoCode.UI, UIEvent.PLAYER_READY, userDto.ID);
        }

        private void START_GAME_BOD()
        {
            //更新游戏场景 隐藏准备文字
            //Dispatch(AreoCode.UI, UIEvent.PLAYER_HIDE_STATE, GameModles.Instance.userDto.ID);
            Dispatch(AreoCode.UI, UIEvent.PLAYER_HIDE_STATE, "隐藏准备文字");
            Dispatch(AreoCode.UI, UIEvent.PROMPT_PANEL_EVENTCODE, "游戏即将开始");
        }
    }
}
