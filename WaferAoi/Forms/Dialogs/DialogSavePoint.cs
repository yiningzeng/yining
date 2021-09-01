﻿using System;
using System.IO;
using System.Windows.Forms;
using WaferAoi.Tools;
using YiNing.Tools;
using YiNing.UI.Controls;
using YiNing.UI.Forms;

namespace WaferAoi
{
    public partial class DialogSavePoint : DarkDialog
    {
        public DialogSavePoint()
        {
            InitializeComponent();
            Ini();
            this.Load += DialogSavePoint_Load;
        }

        private void DialogSavePoint_Load(object sender, EventArgs e)
        {
            int[] xyz = MotorsControl.GetXYEncPos(2, 1, 4);
            tbX.Text = xyz[0].ToString();
            tbY.Text = xyz[1].ToString();
            tbZ.Text = xyz[2].ToString();
        }

        public void Ini()
        {
            this.btnOk.Text = "确定";
            this.btnCancel.Text = "取消";

            this.btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "")
            {
                DarkMessageBox.ShowWarning("参数有误");
                return;
            }
            try
            {

                Config config = FsmHelper.GetConfig();
                if ((WaferSize)cmbWaferSize.SelectedIndex == WaferSize.INCH6)
                {
                    config.Inch6SavePoints.Add(new PointInfo() { Remark = tbName.Text, X = int.Parse(tbX.Text), Y = int.Parse(tbY.Text), Z = int.Parse(tbZ.Text) });
                }
                else
                {
                    config.Inch8SavePoints.Add(new PointInfo() { Remark = tbName.Text, X = int.Parse(tbX.Text), Y = int.Parse(tbY.Text), Z = int.Parse(tbZ.Text) });
                }
                JsonHelper.Serialize(config, "yining.config");
                this.Close();
            }
            catch (Exception er) { DarkMessageBox.ShowError(er.Message); }
        }
    }
}