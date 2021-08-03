using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiNing.Tools
{
    public class JsonHelper
    {
        /// <summary>
        /// Json序列化+字段过滤+保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="props"> 指定字段 </param>
        /// <param name="isSave">true: 只保留这些字段 false: 排除掉这些字段</param>
        /// <param name="cla">实体类</param>
        /// <param name="filePath">如果传了就保存</param>
        /// <returns></returns>
        public static string Serialize<T>(T cla, string filePath = null, string[] props = null, bool isSave = false)
        {
            try
            {
                if (props == null) props = new string[] { };
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                jSetting.ContractResolver = new LimitPropsContractResolver(props, isSave);
                string res = JsonConvert.SerializeObject(cla, jSetting);

                if (filePath != null)
                {
                    File.WriteAllText(filePath, ConvertJsonString(res));
                }
                return res;
            }
            catch (Exception er)
            {
                LogHelper.WriteLog("序列化失败", er);
                return null;
            }
        }

        /// <summary>
        /// 通过文件路径反序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeByFile<T>(string filePath)
        {
            try
            {
                T res = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return res;
            }
            catch(Exception er)
            {
                LogHelper.WriteLog("反序列化失败", er);
                return default(T);
            }
        }

        /// <summary>
        /// 通过str反序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeByStr<T>(string str)
        {
            try
            {
                T res = JsonConvert.DeserializeObject<T>(str);
                return res;
            }
            catch (Exception er)
            {
                LogHelper.WriteLog("反序列化失败", er);
                return default(T);
            }
        }

        public class LimitPropsContractResolver : DefaultContractResolver
        {
            string[] props = null;

            bool retain;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="props">传入的属性数组</param>
            /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
            public LimitPropsContractResolver(string[] props, bool retain = true)
            {
                //指定要序列化属性的清单
                this.props = props;

                this.retain = retain;
            }

            protected override IList<JsonProperty> CreateProperties(Type type,

            MemberSerialization memberSerialization)
            {
                IList<JsonProperty> list =
                base.CreateProperties(type, memberSerialization);
                //只保留清单有列出的属性
                return list.Where(p =>
                {
                    if (retain)
                    {
                        return props.Contains(p.PropertyName);
                    }
                    else
                    {
                        return !props.Contains(p.PropertyName);
                    }
                }).ToList();
            }
        }
        /// <summary>
        /// Json格式化输出
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}
