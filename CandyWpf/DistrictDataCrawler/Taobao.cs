using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace DistrictDataCrawler
{
    public class Taobao
    {
        public List<TaobaoArea> GetAllAreas()
        {
            var list = new List<TaobaoArea>();
            using (var ms = Assembly
                                .GetExecutingAssembly()
                                .GetManifestResourceStream("ChinaArea.Resources.taobao.xml") ??
                            new MemoryStream())
            {
                var xml = new XmlDocument();
                xml.Load(ms);
                var areas = xml.SelectNodes("//area");
                if (areas != null)
                {
                    foreach (XmlNode area in areas)
                    {
                        var item = new TaobaoArea
                        {
                            Id = Convert.ToInt32(area.SelectSingleNode("id")?.InnerText),
                            Name = area.SelectSingleNode("name")?.InnerText,
                            ParentId = Convert.ToInt32(area.SelectSingleNode("parent_id")?.InnerText),
                            Type = Convert.ToInt32(area.SelectSingleNode("type")?.InnerText),
                            Zip = area.SelectSingleNode("zip")?.InnerText
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public class TaobaoArea
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int ParentId { get; set; }
            public int Type { get; set; }
            public string Zip { get; set; }
        }
    }
}
