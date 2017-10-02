using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateServer
{
    public class Info
    {

        /// <summary>
        /// 功能码
        /// </summary>
        public string FunctionCode { get; set; }
        /// <summary>
        /// 帧序号
        /// </summary>
        public int FrameNumber { get; set; }
        /// <summary>
        /// 帧长度
        /// </summary>
        public int FrameLength { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        public string InterlockingID { get; set; }
        /// <summary>
        /// 电量值
        /// </summary>
        public int PowerValue { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public string Dimension { get; set; }
        /// <summary>
        /// 定位精度
        /// </summary>
        public int PositioningAccuracy { get; set; }
        /// <summary>
        /// 进出站状态
        /// </summary>
        public int EntryExitState { get; set; }
        /// <summary>
        /// 基站编号
        /// </summary>
        public string PhysicalNumbering { get; set; }
        /// <summary>
        /// 供电状态
        /// </summary>
        public int PoweredByState { get; set; }
        /// <summary>
        /// 开锁状态
        /// </summary>
        public int UnlockState { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 上次开锁时常
        /// </summary>
        public int LastUnlockedTime { get; set; }
        /// <summary>
        /// 结果码
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 开锁上报周期
        /// </summary>
        public int UnlockReportingCycle { get; set; }
        /// <summary>
        /// 关锁上报周期
        /// </summary>
        public int LockedReportingCycle { get; set; }
        /// <summary>
        /// 校对
        /// </summary>
        public int Proofreading { get; set; }

    }
}
