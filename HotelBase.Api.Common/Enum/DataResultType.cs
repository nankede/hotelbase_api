using System.ComponentModel;

namespace HotelBase.Api.Entity.CommonModel.Enum
{
    public enum DataResultType
    {
        // <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Sucess = 0,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Fail = 1,
    }
}