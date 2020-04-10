using aweb.ui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace aweb.common
{
    class OpaqueCommand
    {
        
        private MyOpaqueLayer.MyOpaqueLayer m_OpaqueLayer = null;//半透明蒙板层
        loading msg = new loading();
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="alpha">透明度</param>
        /// <param name="isShowLoadingImage">是否显示图标</param>
        public void ShowOpaqueLayer(Control control, int alpha, bool isShowLoadingImage)
        {
             /* try
              {
                  if (this.m_OpaqueLayer == null)
                  {
                      this.m_OpaqueLayer = new MyOpaqueLayer.MyOpaqueLayer(alpha, isShowLoadingImage);
                      control.Controls.Add(this.m_OpaqueLayer);
                      this.m_OpaqueLayer.Dock = DockStyle.Fill;
                      this.m_OpaqueLayer.BringToFront();
                  }
                  this.m_OpaqueLayer.Enabled = true;
                  this.m_OpaqueLayer.Visible = true;
              }
              catch { }*/
             

        /*    msg.StartPosition = FormStartPosition.CenterParent;

            msg.StartPosition = FormStartPosition.Manual;
            int x, y;
            x = Form1.form1.Location.X + (Form1.form1.Width / 2) - (msg.Width / 2);
            y = Form1.form1.Location.Y + (Form1.form1.Height / 2) - (msg.Height / 2);
            msg.Location = new Point(x, y);
            msg.Visible = false;
            msg.Show(Form1.form1);*/
             

        }
         
        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        public void HideOpaqueLayer()
        {
           

           // msg.Hide();
            /*try
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Visible = false;
                    this.m_OpaqueLayer.Enabled = false;
                }
                else
                {
                    Console.WriteLine("有问题");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }*/
        }
    }
}
