using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiNing.UI.Controls
{
    [Serializable]
    public class DarkStepViewerItem
    {
        public string Id { get; set; }
        public string StepName { get; set; }
        public int StepOrder { get; set; }
        public string StepDesc { get; set; }
        public object StepTag { get; set; }

        //public DarkStepViewerItem() { }
        //public Image StepCompletedImage { get; set; }
        //public Image StepDoingImage { get; set; }
        public DarkStepViewerItem() { }
        public DarkStepViewerItem(string id, string stepname, int steporder, string stepdesc, object tag)
        {
            this.Id = id;
            this.StepName = stepname;
            this.StepOrder = steporder;
            this.StepDesc = stepdesc;
            this.StepTag = tag;
        }
    }
}
