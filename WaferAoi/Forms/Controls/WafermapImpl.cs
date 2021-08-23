using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiNing.Fsm;
using YiNing.WafermapDisplay;
using YiNing.WafermapDisplay.WafermapControl;

namespace WaferAoi
{
    public class WafermapImpl : Wafermap
    {

        public override void dieEntered(Die previousDie, Die currentDie)
        {
            if (previousDie == null || currentDie == null) return;
            if (previousDie.XIndex == currentDie.XIndex && previousDie.YIndex == currentDie.YIndex) updateDie(currentDie);
            else updateDie(previousDie, currentDie);
            //Console.WriteLine("previousX:{0}, previousY:{1}, x:{2},y:{3},bincode:{4}", previousX, previousY, x, y, bincode);
            // Do nothing
        }

        public override void dieMouseDown(Die die, System.Windows.Forms.MouseButtons btn)
        {
            // Cast

            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                // Update dataset
                //Dataset[x, y] = 3;
                // Show the changes
                MouseDownDie(die);
            }
            else
            {
                // Rotation test
                //int rot = Rotation + 90;
                //if (rot > 270)
                //    rot = rot - 360;
                //Rotation = rot;
                //Invalidate();
            }
        }

        public override void dieMouseUp(Die die, System.Windows.Forms.MouseButtons btn)
        {
            // Cast
            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                // Update dataset
                //Dataset[x, y] = 3;
                // Show the changes
                MouseUpDie(die);
            }
            else
            {
                // Rotation test
                //int rot = Rotation + 90;
                //if (rot > 270)
                //    rot = rot - 360;
                //Rotation = rot;
                //Invalidate();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WafermapImpl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "WafermapImpl";
            this.Size = new System.Drawing.Size(1002, 555);
            this.ResumeLayout(false);

        }
    }
}
