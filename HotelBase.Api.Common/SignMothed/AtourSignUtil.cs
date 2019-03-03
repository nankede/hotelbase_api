﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HotelBase.Api.Common.SignMothed
{
    /// <summary>
    /// 亚朵加密规则
    /// </summary>
    public class AtourSignUtil
    {

        public static string AtourAuth_URL = ConfigurationManager.AppSettings["OpenApi"];
        public static string AtourAuth_APPID = ConfigurationManager.AppSettings["appid"];
        private static string AtourAuth_APPKEY = ConfigurationManager.AppSettings["key"];


        /// <summary>
        /// 获取亚朵签名
        /// </summary>
        /// <param name="dicparams"></param>
        /// <returns></returns>
        public static string GetSignUtil(Dictionary<string, string> dicparams)
        {
            var vDic = (from objDic in dicparams orderby objDic.Key ascending select objDic);
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                if (!string.IsNullOrWhiteSpace(pvalue))
                {
                    str.Append(pkey + pvalue);
                }
            }

            var result = MD5Helper.GetMD5(str.ToString().Substring(0, str.ToString().Length - 1) + AtourAuth_APPKEY);
            return result;

        }
    }
}
