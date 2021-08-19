using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiNing.Tools
{
    public class WaferMappingHelper
    {
        public static string[] GetLatestFiles(string Path)
        {
            var query = from f in Directory.GetFiles(Path, "*.txt")
                        let fi = new FileInfo(f)
                        orderby fi.CreationTime descending
                        select fi.Name;
            return query.ToArray();
        }
    }
}
