using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DistrictDataCrawler
{
    [DebuggerDisplay("{ID}:{Name}")]
    public class DistrictDataItem
    {
        [JsonProperty("quHuaDaiMa")]
        //������������
        public string ID { get; set; }

        //��������������
        public string Pid { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }

        [JsonProperty("shengji")]
        //ʡ
        public string Province { get; set; }

        private string _provinceName;
        //ʡ ����
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
        //����
        public string City { get; set; }

        [JsonProperty("xianji")]
        //����
        public string County { get; set; }


        [JsonProperty("quhao")]
        //����
        public string Code { get; set; }

        //פ��
        public string ZhuDi { get; set; }
        //�˿�
        public string RenKou { get; set; }
        //���
        public string Area { get; set; }
        //��������
        public string ZipCode { get; set; }
        //����
        public int Level { get; set; }
        //ƴ��
        public string PinYin { get; set; }

        /// <summary>
        /// ƴ������ĸ
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public double Lon { get; set; }
        /// <summary>
        /// ά��
        /// </summary>
        public double Lat { get; set; }

        public override string ToString()
        {
            return string.Join(" ", ID, Name, Province,City,County);
        }
    }
}