using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiNing.Fsm;

namespace WaferAoi
{
    class WafermapImpl : YiNing.WafermapDisplay.Wafermap
    {

        public override void dieEntered(int previousX, int previousY, int previousBincode, int x, int y, int bincode)
        {
            if (previousX == x && previousY == y) updateDie(x, y, bincode);
            else updateDie(previousX, previousY, previousBincode, x, y, bincode);
            Console.WriteLine("previousX:{0}, previousY:{1}, x:{2},y:{3},bincode:{4}", previousX, previousY, x, y, bincode);
            // Do nothing
        }

        public override void dieMouseDown(int x, int y, int bincode, System.Windows.Forms.MouseButtons btn)
        {
            // Cast

            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                // Update dataset
                //Dataset[x, y] = 3;
                // Show the changes
                MouseDownDie(x, y, bincode);
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

        public override void dieMouseUp(int x, int y, int bincode, System.Windows.Forms.MouseButtons btn)
        {
            // Cast
            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                // Update dataset
                //Dataset[x, y] = 3;
                // Show the changes
                MouseUpDie(x, y, bincode);
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
            this.Size = new System.Drawing.Size(1153, 623);
            this.ResumeLayout(false);

        }
    }
}
