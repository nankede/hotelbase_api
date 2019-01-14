using Component.Access.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBase.Api.Entity.Tables
{
    /// <summary>
    /// H_DistributorAreaInfo表实体类
    /// </summary>
    [Serializable, Table("H_DistributorAreaInfo")]
    public class H_DistributorAreaInfoModel
    {
        /// <summary>
		/// 数据库字段：AA_Id 
		/// </summary>
		private int _aA_Id = 0;

        /// <summary>
        /// 自增主键
        /// </summary>
        [Key(KeyType.Identity)]
        [Column("AA_Id")]
        public int AA_Id
        {
            get { return _aA_Id; }
            set { _aA_Id = value; }
        }

        /// <summary>
        /// 数据库字段：AA_ProvinceId
        /// </summary>
        private int _aA_ProvinceIdd = 0;

        /// <summary>
        /// 省份id
        /// </summary>
        [Column("AA_ProvinceId")]
        public int AA_ProvinceId
        {
            get { return _aA_ProvinceIdd; }
            set { _aA_ProvinceIdd = value; }
        }

        /// <summary>
        /// 数据库字段：AA_ProvinceName
        /// </summary>
        private string _aA_ProvinceName = "";

        /// <summary>
        /// 省份名称
        /// </summary>
        [Column("AA_ProvinceName")]
        public string AA_ProvinceName
        {
            get { return _aA_ProvinceName; }
            set { _aA_ProvinceName = value; }
        }

        /// <summary>
        /// 数据库字段：AA_CityId
        /// </summary>
        private int _aA_CityIdd = 0;

        /// <summary>
        /// 城市id
        /// </summary>
        [Column("AA_CityId")]
        public int AA_CityId
        {
            get { return _aA_CityIdd; }
            set { _aA_CityIdd = value; }
        }

        /// <summary>
        /// 数据库字段：AA_CityName
        /// </summary>
        private string _aA_CityName = "";

        /// <summary>
        /// 城市名称
        /// </summary>
        [Column("AA_CityName")]
        public string AA_CityName
        {
            get { return _aA_CityName; }
            set { _aA_CityName = value; }
        }

        /// <summary>
		/// 数据库字段：AA_Type 
		/// </summary>
		private int _aA_Type = 0;

        /// <summary>
        /// 类型（1：亚朵  2：喜玩.........）
        /// </summary> 
        [Column("AA_Type")]
        public int AA_Type
        {
            get { return _aA_Type; }
            set { _aA_Type = value; }
        }
    }
}
