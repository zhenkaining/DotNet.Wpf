using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DistrictDataCrawler
{
    [DebuggerDisplay("{ID}:{Name}")]
    public class DistrictDataItem
    {
        [JsonProperty("quHuaDaiMa")]
        //行政区划代码
        public string ID { get; set; }

        //父行政区划代码
        public string Pid { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        [JsonProperty("shengji")]
        //省
        public string Province { get; set; }

        private string _provinceName;
        //省 名称
        public string ProvinceName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_provinceName))
                {
                    if (!string.IsNullOrWhiteSpace(Province))
                    {
                        var x = Province.IndexOf("(", StringComparison.CurrentCultureIgnoreCase);
                        if (x != -1)
                        {
                            _provinceName = Province.Substring(0, x);
                        }
                        else
                        {
                            
                        }
                    }
                    else
                    {
                        
                    }
                }
                return _provinceName;
            }
            set { _provinceName = value; }
        }

        [JsonProperty("diji")]
        //城市
        public string City { get; set; }

        [JsonProperty("xianji")]
        //区县
        public string County { get; set; }


        [JsonProperty("quhao")]
        //区号
        public string Code { get; set; }

        //驻地
        public string ZhuDi { get; set; }
        //人口
        public string RenKou { get; set; }
        //面积
        public string Area { get; set; }
        //邮政编码
        public string ZipCode { get; set; }
        //级别
        public int Level { get; set; }
        //拼音
        public string PinYin { get; set; }

        /// <summary>
        /// 拼音首字母
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public double Lon { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Lat { get; set; }

        public override string ToString()
        {
            return string.Join(" ", ID, Name, Province,City,County);
        }
    }
}