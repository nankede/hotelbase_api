using Component.Access.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Tables
{
    /// <summary>
    /// 国家地区数据
    /// </summary>
    [Serializable, Table("Sys_AreaInfo")]
    public class Sys_AreaInfoModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Key(KeyType.Identity)]
        [Column("Id")]
        public int id { get; set; } = 0;
        /// <summary>父级Id</summary>
        [Column("pid")]
        public int pid { get; set; } = 0;
        /// <summary>地区完整Id</summary>
        [Column("pid_path")]
        public string pid_path { get; set; } = string.Empty;
        /// <summary>地区完整名称</summary>
        [Column("p_cnname_path")]
        public string p_cnname_path { get; set; } = string.Empty;
        /// <summary>地区名称</summary>
        [Column("name")]
        public string name { get; set; } = string.Empty;
        /// <summary>英文名称</summary>
        [Column("en_name")]
        public string en_name { get; set; } = string.Empty;
        /// <summary>英文简称</summary>
        [Column("short_en_name")]
        public string short_en_name { get; set; } = string.Empty;
        /// <summary>地区级别：0-洲，1-国，2-省，3-市，4-县</summary>
        [Column("type")]
        public int type { get; set; } = 0;
        /// <summary>中文简称</summary>
        [Column("shortCnName")]
        public string shortCnName { get; set; } = string.Empty;
        /// <summary>首字母</summary>
        [Column("prefixLetter")]
        public string prefixLetter { get; set; } = string.Empty;
        /// <summary>全拼</summary>
        [Column("FullPinyinName")]
        public string FullPinyinName { get; set; } = string.Empty;
    }
}
