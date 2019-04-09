using Component.Access.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Tables
{
    /// <summary>
    /// Sys_AreaMatch
    /// </summary>
    [Serializable, Table("Sys_AreaMatch")]
    public class Sys_AreaMatchModel
    {

        /// <summary>
        /// 自增主键
        /// </summary>
        [Key(KeyType.Identity)]
        [Column("Id")]
        public int Id { get; set; } = 0;

        /// <summary>
        /// 当前系统Id
        /// </summary>
        [Column("HbId")]
        public int HbId { get; set; } = 0;

        /// <summary>
        /// 外部系统类型 1 亚朵
        /// </summary>
        [Column("OutType")]
        public int OutType { get; set; } = 0;

        /// <summary>
        /// 外部城市Id
        /// </summary>
        [Column("OutCityId")]
        public int OutCityId { get; set; } = 0;

        /// <summary>
        /// 外部城市名称
        /// </summary>
        [Column("OutCityName")]
        public string OutCityName { get; set; } = string.Empty;

        /// <summary>
        /// 外部省份Id
        /// </summary>
        [Column("OutProvId")]
        public int OutProvId { get; set; } = 0;
        
        /// <summary>
        /// 外部省份名称
        /// </summary>
        [Column("OutProvName")]
        public string OutProvName { get; set; } = string.Empty;


        /// <summary>
        /// 外部城市编号
        /// </summary>
        [Column("OutCityCode")]
        public string OutCityCode { get; set; } = string.Empty;
    }
}
