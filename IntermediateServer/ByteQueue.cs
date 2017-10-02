using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateServer
{
    /// <summary>
    /// 字节缓冲器
    /// </summary>
   public  class ByteQueue
    {
        private List<byte> m_buffer = new List<byte>();
        public bool Find()
        {
            if (m_buffer.Count == 0)
                return false;
            int HeadIndex = m_buffer.FindIndex(o => o == 0xAA);

            if (HeadIndex == -1)
            {
                m_buffer.Clear();
                return false; //没找到AA  
            }

            else if (HeadIndex != 0) //不为开头移掉之前的字节  
            {
                if (HeadIndex > 1)
                    m_buffer.RemoveRange(0, HeadIndex);
            }

            int length = GetLength();

            if (m_buffer.Count < length)
            {
                return false;
            }

            int TailIndex = m_buffer.FindIndex(o => o == 0x55); //查找55的位置  

            if (TailIndex == -1)
            {
                //这一步为防止连发一个AA开头的包后，没发55，而又发了一个AA  
                int head = m_buffer.FindLastIndex(o => o == 0xAA);
                if (head > -1)
                {
                    m_buffer.RemoveRange(0, head);
                }
                return false;
            }
            else if (TailIndex + 1 != length) //计算包尾是否与包长度相等  
            {
                m_buffer.RemoveRange(0, TailIndex);
                return false;
            }

            return true;
        }

        /// <summary>  
        /// 命令类型  
        /// </summary>  
        /// <returns></returns>  
        public byte Cmd()
        {
            if (m_buffer.Count >= 2)
            {
                return m_buffer[1];
            }
            return 0;
        }

        /// <summary>  
        /// 序号  
        /// </summary>  
        /// <returns></returns>  
        public byte Number()
        {
            if (m_buffer.Count >= 3)
            {
                return m_buffer[2];
            }
            return 0;
        }

        /// <summary>  
        /// 包长度  
        /// </summary>  
        /// <returns></returns>  
        public int GetLength()
        {
            int len = 5;//AA 命令类型 序号 校验和 55  
            if (m_buffer.Count >= 3)
            {
                switch (m_buffer[2]) //第三字节为序号  
                {
                    case 0x00: //序号  
                        return len + 16;
                    case 0x01: //序号  
                        return len + 10;
                    case 0x02: //序号  
                        return len + 12;
                }
            }
            return 0;
        }
        /// <summary>  
        /// 提取数据  
        /// </summary>  
        public void Dequeue(byte[] buffer, int offset, int size)
        {
            m_buffer.CopyTo(0, buffer, offset, size);
            m_buffer.RemoveRange(0, size);
        }

        /// <summary>  
        /// 队列数据  
        /// </summary>  
        /// <param name="buffer"></param>  
        public void Enqueue(byte[] buffer)
        {
            m_buffer.AddRange(buffer);
        }
        //16进制字符串转字节数组 
        private static byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }
        // 字节数组转16进制字符串   
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");//ToString("X2") 为C#中的字符串格式控制符
                }
            }
            return returnStr;
        }
        //将byte数组转换成字符串
        public static string ChangBytesToStr(byte[] data, int dataLen)
        {
            string Str = "";

            for (int i = 0; i < dataLen; i++)
            {
                Str = Str + String.Format("0x{0:x2}", data[i]) + " ";
            }
            Str.Trim();
            return Str;
        }
    }
}
