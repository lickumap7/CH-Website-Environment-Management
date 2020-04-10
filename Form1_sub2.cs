using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.ServiceProcess;
using System.Threading;
using aweb.common;
using System.Diagnostics;
using System.IO;

namespace aweb
{
     
    public partial class Form1
    {
        
         
        TimeSpan timeout = new TimeSpan(0, 0, 15);
        private void button1_Click(object sender, EventArgs e)
        {
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在启动Apache...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Start), apache);
            }
            catch (Exception ee)
            {

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止Apache...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Stop), apache);
            }
            catch (Exception ee)
            {

            }
        }
        public void button3_Click(object sender, EventArgs e)
        {

            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止Apache...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Reload), apache);
            }
            catch (Exception ee)
            {

            }
        }


        private void button6_Click(object sender, EventArgs e)
        {


            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在启动MySQL...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Start), mysql);
            }
            catch (Exception ee)
            {

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止MySQL...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Stop), mysql);
            }
            catch (Exception ee)
            {

            }
        }

        public void button4_Click(object sender, EventArgs e)
        {
           // cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止MySQL...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Reload), mysql);
            }
            catch (Exception ee)
            {

            }

        }


        private void button15_Click(object sender, EventArgs e)
        {
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在启动Tomcat...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Start), tomcat);
            }
            catch (Exception ee)
            {

            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止Tomcat...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Stop), tomcat);
            }
            catch (Exception ee)
            {

            }
        }

        public void button13_Click(object sender, EventArgs e)
        {
            //cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
            try
            {
                this.writeLog("正在停止Tomcat...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(service_Reload), tomcat);
            }
            catch (Exception ee)
            {

            }
        }


        //安装服务
        private void button7_Click(object sender, EventArgs e)
        {

            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == apache);
            if (ApacheService == null)
            {
                string cmdline = rootPath + "\\soft\\apache\\Apache24\\bin\\httpd.exe -k install";

                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                    this.writeLog("安装Apache服务");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {
                    this.writeLog("Warning:安装" + apache + "异常!");
                }


            }
            else
            {
                MessageBox.Show(apache+"服务已存在，请勿重复安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }





        }
        private void button8_Click(object sender, EventArgs e)
        {
            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == apache);
            if (ApacheService == null)
            {

                MessageBox.Show(apache+"服务不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {

                if (ApacheService.Status == ServiceControllerStatus.Running)
                {
                    MessageBox.Show(apache + "必须停止后卸载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string cmdline = rootPath + "\\soft\\apache\\Apache24\\bin\\httpd.exe -k uninstall";


                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                    this.writeLog("卸载Apache服务");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {

                }

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == mysql);
            if (ApacheService == null)
            {
                string cmdline = rootPath + "\\soft\\mysql\\mysql-5.7.28-winx64\\bin\\mysqld.exe -install";

                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                    this.writeLog("安装MySQL服务");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {

                }
                if (!File.Exists(rootPath + @"\soft\mysql\mysql-5.7.28-winx64\my.ini"))
                {
                    MessageBox.Show("数据库配置文件不存在或路径错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                INIUtil iniUtil = new INIUtil(rootPath + @"\soft\mysql\mysql-5.7.28-winx64\my.ini");
 
                iniUtil.Write("mysqld", "basedir", "\"" + rootPath.Replace(@"\", @"/") + @"/soft/mysql/mysql-5.7.28-winx64""");
                iniUtil.Write("mysqld", "datadir", "\"" + rootPath.Replace(@"\", @"/") + @"/soft/mysql/mysql-5.7.28-winx64/data""");


            }
            else
            {
                MessageBox.Show(mysql+"服务已存在，请勿重复安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == mysql);
            if (ApacheService == null)
            {

                MessageBox.Show(mysql+"服务不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            else
            {
                if (ApacheService.Status == ServiceControllerStatus.Running)
                {
                    MessageBox.Show(mysql + "必须停止后卸载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmdline = rootPath + "\\soft\\mysql\\mysql-5.7.28-winx64\\bin\\mysqld.exe -remove";

                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                     
                    this.writeLog("卸载MySQL服务!");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {

                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {


            /* Process p = new Process();
             p.StartInfo.FileName = "java.exe"; 
             p.StartInfo.Arguments = "-version";
             p.StartInfo.RedirectStandardError = true;
             p.StartInfo.UseShellExecute = false;
             p.StartInfo.CreateNoWindow = true;
             p.Start();
             string result = p.StandardError.ReadToEnd();*/
            //具体逻辑你可以进一步完善，比如正则表达式 return result.Contains("java version");

            MessageBoxButtons mess = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("请确认已安装JDK 1.8及以上版本，否则Tomcat将无法启动", "提示", mess);
            if (dr != DialogResult.OK)
            {
                return;
            } 


            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == tomcat);
            if (ApacheService == null)
            {
                string cmdline = "";

                //e: & cd E:\project\AWEB\aweb\bin\Debug\soft\tomcat\apache - tomcat - 8.5.53\bin\ &service.bat install

                cmdline = rootPath.Substring(0, 1)+": & cd "+rootPath+ "\\soft\\tomcat\\apache-tomcat-8.5.53\\bin\\ & service.bat install";

                Console.WriteLine(cmdline);
                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                    this.writeLog("安装Tomcat服务!");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {

                }


            }
            else
            {
                MessageBox.Show(tomcat + "服务已存在，请勿重复安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == tomcat);
            if (ApacheService == null)
            {

                MessageBox.Show(tomcat + "服务不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            else
            {

                if (ApacheService.Status == ServiceControllerStatus.Running)
                {
                    MessageBox.Show(tomcat + "必须停止后卸载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                    string cmdline = "";

                //e: & cd E:\project\AWEB\aweb\bin\Debug\soft\tomcat\apache - tomcat - 8.5.53\bin\ &service.bat install

                cmdline = rootPath.Substring(0, 1) + ": & cd " + rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\bin\\ & service.bat remove";

                //string cmdline = rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\bin\\service.bat remove";

                cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
                try
                {
                     
                    this.writeLog("卸载Tomcat服务!");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ExeCmd), cmdline);
                }
                catch (Exception ee)
                {

                }
            }
        }

        private delegate void SetLoadingHide();

        void hideLoading()
        {

            cmd.HideOpaqueLayer();
        }
        void showLoading()
        {
            cmd.ShowOpaqueLayer(this.tabPage1, 125, true);
        }

        private void service_Start(object serviceName)
        {
            var serviceControllers = ServiceController.GetServices();
            var server = serviceControllers.FirstOrDefault(service => service.ServiceName == serviceName.ToString());


            if (server == null)
            {
                this.writeLog("此服务未安装");
                MessageBox.Show("此服务未安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                return;
            }



            if (server.Status != ServiceControllerStatus.Running)
            {


                SetLoadingHide showloding = new SetLoadingHide(showLoading);
                this.Invoke(showloding);
                try
                {
                    server.Start();
                    server.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                    this.Invoke(hideloding);
                    this.writeLog("启动" + serviceName.ToString() + "成功!");
                }
                catch (Exception e)
                {
                    SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                    this.Invoke(hideloding);
                    this.writeLog("Warning:启动"+ serviceName.ToString() + "异常!");
                }

            }


        }

        private void service_Stop(object serviceName)
        {
            var serviceControllers = ServiceController.GetServices();
            var server = serviceControllers.FirstOrDefault(service => service.ServiceName == serviceName.ToString());
            /* if (server != null && server.Status != ServiceControllerStatus.Running)
             {
                 server.Start();
             }*/

            if (server == null)
            {
                this.writeLog("此服务未安装");
                MessageBox.Show("此服务未安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                return;
            }


            SetLoadingHide showloding = new SetLoadingHide(showLoading);
            this.Invoke(showloding);
            try
            {

                server.Stop();
                server.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                this.writeLog("停止" + serviceName.ToString() + "成功!");
            }
            catch (Exception e)
            {
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                this.writeLog("Warning:停止" + serviceName.ToString() + "异常!");
            }



        }

        private void service_Reload(object serviceName)
        {
            var serviceControllers = ServiceController.GetServices();
            var server = serviceControllers.FirstOrDefault(service => service.ServiceName == serviceName.ToString());
            /* if (server != null && server.Status != ServiceControllerStatus.Running)
             {
                 server.Start();
             }*/

            if (server == null)
            {
                this.writeLog("此服务未安装");
                MessageBox.Show("此服务未安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                return;
            }

            SetLoadingHide showloding = new SetLoadingHide(showLoading);
            this.Invoke(showloding);
            try
            {
                if (server.Status == ServiceControllerStatus.Running)
                {
                    server.Stop();
                    server.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                this.writeLog("启动" + serviceName.ToString() + "服务中...");
                server.Start();
                
                server.WaitForStatus(ServiceControllerStatus.Running, timeout);

                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                this.writeLog("启动" + serviceName.ToString() + "成功!");


            }
            catch (Exception e)
            {
                SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
                this.Invoke(hideloding);
                this.writeLog("Warning:重启" + serviceName.ToString() + "异常!");
            }




        }
        
        private void ExeCmd(object cmdline)
        {
            /* var process = new Process();
             string content = "";
             process.StartInfo.FileName = "cmd.exe";
             process.StartInfo.UseShellExecute = false;
             process.StartInfo.RedirectStandardInput = true;
             process.StartInfo.RedirectStandardOutput = true;
             process.StartInfo.RedirectStandardError = true;
             process.StartInfo.CreateNoWindow = true;

             process.Start();
             process.BeginErrorReadLine();
             process.BeginOutputReadLine();
             process.StandardInput.AutoFlush = true;
             process.StandardInput.WriteLine(cmdline.ToString() + "&exit");


             process.StartInfo.RedirectStandardOutput = true;

             process.WaitForExit();
             process.Close();
             SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
             this.Invoke(hideloding);*/
            //this.writeLog("执行返回" + process.StandardOutput.ReadToEnd());
             
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = "cmd.exe";
            

            CmdProcess.StartInfo.CreateNoWindow = true;         // 不创建新窗口
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;  // 重定向输入
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            CmdProcess.StartInfo.RedirectStandardError = true;  // 重定向错误输出
            //CmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            CmdProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            CmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);

            CmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件
            CmdProcess.Exited += new EventHandler(CmdProcess_Exited);   // 注册进程结束事件

            CmdProcess.Start();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
            CmdProcess.StandardInput.AutoFlush = true;
            CmdProcess.StandardInput.WriteLine(cmdline.ToString() + "&exit");
            CmdProcess.StartInfo.RedirectStandardOutput = true;
            // 如果打开注释，则以同步方式执行命令，此例子中用Exited事件异步执行。
            //CmdProcess.WaitForExit();
           // CmdProcess.Close();

        }
        private void ReadStdOutputAction(string result)
        {
            this.writeLog("执行返回：" + result);
            
        }

        private void ReadErrOutputAction(string result)
        {
            this.writeLog("执行返回：" + result);
             
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // 4. 异步调用，需要invoke
                this.Invoke(ReadStdOutput, new object[] { e.Data });
            }
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Invoke(ReadErrOutput, new object[] { e.Data });
            }
        }
 

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            SetLoadingHide hideloding = new SetLoadingHide(hideLoading);
            this.Invoke(hideloding);
        }
    }
}
