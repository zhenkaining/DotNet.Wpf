using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Pinyin4net;
using Pinyin4net.Format;

namespace DistrictDataCrawler
{
    /// <summary>
    /// 行政区数据爬虫
    /// </summary>
    class Program
    {
        private static HanyuPinyinOutputFormat format;
      
        private static void Main(string[] args)
        {
            format = new HanyuPinyinOutputFormat();
            format.ToneType = HanyuPinyinToneType.WITHOUT_TONE;
            format.VCharType = HanyuPinyinVCharType.WITH_V;
            format.CaseType = HanyuPinyinCaseType.LOWERCASE;
            var home = Get("http://xzqh.mca.gov.cn/map");
            var regexJson = new Regex("var json = ([^;]+);");
            if (regexJson.IsMatch(home))
            {
                var list = new List<DataItem>();
                var encode = Encoding.GetEncoding(936);
                var json = regexJson.Match(home).Groups[1].Value;
                var categoryItems = JsonConvert.DeserializeObject<List<DataItem>>(json);
                foreach (var shengji in categoryItems)
                {
                    Console.WriteLine(shengji);
                    shengji.Level = 1;
                    shengji.PinYin = ConvertToPinYin(shengji.ShengJiName);
                    shengji.ShouZiMu = GetShouZiMu(shengji.ShengJiName);
                    list.Add(shengji);
                    var key = HttpUtility.UrlEncode(shengji.ShengJi, encode);
                    var page = Get($"http://202.108.98.30/defaultQuery?shengji={key}&diji=-1&xianji=-1");
                    var doc = new HtmlDocument();
                    doc.LoadHtml(page);
                    var trList = doc.DocumentNode.SelectNodes("//tr");
                    if (trList != null)
                    {
                        DataItem diji = null;
                        foreach (var tr in trList)
                        {
                            var tdList = tr.SelectNodes("./td");
                            if (tdList != null && tdList.Count == 7)
                            {
                                DataItem current;
                                if (diji == null || !string.IsNullOrWhiteSpace(tr.GetAttributeValue("flag", string.Empty)))
                                {
                                    diji = JsonConvert
                                        .DeserializeObject<DataItem>(JsonConvert.SerializeObject(shengji));
                                    current = diji;
                                    current.DiJi = tdList[0].InnerText.Trim('+', '☆', ' ', '\r', '\n');
                                    current.Level = 2;
                                    current.PinYin = ConvertToPinYin(current.DiJi);
                                    current.ShouZiMu = GetShouZiMu(current.DiJi);
                                }
                                else
                                {
                                    current = JsonConvert
                                        .DeserializeObject<DataItem>(JsonConvert.SerializeObject(diji));
                                    current.XianJi = tdList[0].InnerText;
                                    current.Level = 3;
                                    current.PinYin = ConvertToPinYin(current.DiJi);
                                    current.ShouZiMu = GetShouZiMu(current.XianJi);
                                }
                                current.ZhuDi = tdList[1].InnerText.Trim();
                                current.RenKou = tdList[2].InnerText.Trim();
                                current.MianJi = tdList[3].InnerText.Trim();
                                current.QuHuaDaiMa = tdList[4].InnerText.Trim();
                                current.QuHao = tdList[5].InnerText.Trim();
                                current.YouBian = tdList[6].InnerText.Trim();


                                if (!string.IsNullOrWhiteSpace(current.QuHuaDaiMa))
                                {
                                    list.Add(current);
                                }
                            }
                        }
                    }
                }

                var dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 4; i++)
                {
                    dir = Path.GetDirectoryName(dir);
                }
                dir = Path.Combine(dir, "data");
                var dt = DateTime.Now.ToString("yyyyMMdd");
                var dateDir = Path.Combine(dir, dt);
                if (!Directory.Exists(dateDir))
                {
                    Directory.CreateDirectory(dateDir);
                }
                using (var fs = File.OpenWrite(Path.Combine(dateDir, "China.csv")))
                {
                    fs.SetLength(0);
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine("f_code,f_province,f_city,f_county,f_level,f_pinyin,f_shouzimu");
                        foreach (var item in list)
                        {
                            sw.WriteLine(string.Join(",",
                                item.QuHuaDaiMa,
                                item.ShengJiName,
                                item.DiJi,
                                item.XianJi,
                                item.Level,
                                item.PinYin,
                                item.ShouZiMu
                            ));
                        }
                    }
                }
                File.WriteAllText(Path.Combine(dateDir, "China.json"), JsonConvert.SerializeObject(list, Formatting.Indented), Encoding.UTF8);

                File.Copy(Path.Combine(dateDir, "China.csv"), Path.Combine(dir, "China.csv"), true);
                File.Copy(Path.Combine(dateDir, "China.json"), Path.Combine(dir, "China.json"), true);
                Console.WriteLine("Done.");
            }
            Console.ReadLine();
        }

        public static string Get(string url)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (var web = new WebClient())
                    {
                        web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.79 Safari/537.36");
                        web.Headers.Add(HttpRequestHeader.Referer, "http://202.108.98.30");
                        var bytes = web.DownloadData(url);
                        var encode = Encoding.GetEncoding(936);
                        return encode.GetString(bytes);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            throw new Exception("获取失败");
        }

        public static string ConvertToPinYin(string name)
        {
            string pinyinStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                char[] charArray = name.ToCharArray();
                if (charArray.Length>0)
                {
                    for (int i = 0; i < charArray.Length; i++)
                    {
                        pinyinStr += ConvertToPinYin(charArray[i])+"_";
                    }
                }
            }
            if (pinyinStr.EndsWith("_"))
            {
                pinyinStr = pinyinStr.TrimEnd('_');
            }
            return pinyinStr;
        }

        public static string ConvertToPinYin(char ch)
        {
            string pinyinStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(ch.ToString()))
            {
                string[] stringArr = PinyinHelper.ToHanyuPinyinStringArray(ch, format);
                if (stringArr!=null)
                {
                    if (stringArr.Count()>1)
                    {
                        pinyinStr = stringArr[0];
                    }
                    else
                    {
                        pinyinStr = stringArr[0];

                    }
                    //foreach (var s in stringArr)
                    //{
                    //    pinyinStr += s;
                    //}
                }
            }
            return pinyinStr;

        }

        public static string GetShouZiMu(string name)
        {
            string pinyinStr = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                char[] charArray = name.ToCharArray();
                if (charArray.Length > 0)
                {
                    string tempStr = ConvertToPinYin(charArray[0]);
                    if (!string.IsNullOrWhiteSpace(tempStr)&& tempStr.Length>0)
                    {
                        pinyinStr = tempStr.Substring(0, 1);
                    }
                  
                }
            }
            return pinyinStr;
        }
    }
}
