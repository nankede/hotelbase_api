using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Common
{
    public class MD5Helper
    {
        /// <summary>
        /// 获取字符数组的MD5加密值
        /// </summary>
        /// <param name="sortedArray">待计算MD5哈希值的输入字符数组</param>
        /// <param name="key">密钥</param>
        /// <param name="charset"></param>
        /// <returns>输入字符数组的MD5哈希值</returns>
        public static string GetMD5ByArray(string[] sortedArray, string key, string charset)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < sortedArray.Length; i++)
            {
                if (i == sortedArray.Length - 1)
                {
                    builder.Append(sortedArray[i]);
                }
                else
                {
                    builder.Append(sortedArray[i] + "&");
                }
            }
            builder.Append(key);
            return GetMD5(builder.ToString(), charset);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="digitLength">指定待返回的MD5哈希值的数位长度，有效取值为16或32，若输入无效值，则自动以32取代无效值</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, int digitLength)
        {
            return GetMD5(input, "utf-8", digitLength);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <param name="digitLength">指定待返回的MD5哈希值的数位长度，有效取值为16或32，若输入无效值，则自动以32取代无效值</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset, int digitLength)
        {
            if (digitLength != 16 && digitLength != 32)
            {
                digitLength = 32;
            }

            if (digitLength == 32)
            {
                return GetMD5(input, charset);
            }
            else
            {
                return GetMD5(input, charset).Substring(8, 16);
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input)
        {
            return GetMD5(input, "utf-8");
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset)
        {
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMD5(string input, string charset, string key)
        {
            input += key;
            System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
