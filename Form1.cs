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
using aweb.ui;
using System.Data.SQLite;
using CCWin;
using System.Reflection;
using System.Xml;
using MySql.Data.MySqlClient;

namespace aweb
{
    public delegate void DelReadStdOutput(string result);
    public delegate void DelReadErrOutput(string result);
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static Form1 form1;
        public event DelReadStdOutput ReadStdOutput;
        public event DelReadErrOutput ReadErrOutput;


        public string apache = "Apache2.4";
        public string mysql = "MySQL";
        public string tomcat = "Tomcat8";
        public string rootPath = Application.StartupPath;

        OpaqueCommand cmd = new OpaqueCommand();
        private void Form1_Load(object sender, EventArgs e)
        {

            //this.xml();
            ReadStdOutput += new DelReadStdOutput(ReadStdOutputAction);
            ReadErrOutput += new DelReadErrOutput(ReadErrOutputAction);

            form1 = this;
            this.init();
            this.statusBarEvent();
             
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(findServiceStatus), null, 100, false);
            ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(processInfo), null, 1000, false);
           // ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(true), new WaitOrTimerCallback(getWindowsErrorLog), null, 1000, false);
            this.load_apache_web();
            this.load_mysql_list();
            this.getWindowsErrorLog();
        }

        


       public void delete_tomcat_web_xml(string domain)
        {

            XmlDocument xmlDocument = new XmlDocument();
            string xmlfile = rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\conf\\server.xml";
            xmlDocument.Load(xmlfile);

            XmlNode xmlServer = xmlDocument.SelectSingleNode("Server");
            XmlNodeList xmlServiceList = xmlServer.SelectNodes("Service");
            foreach (XmlElement element in xmlServiceList)
            {
                if (element.Attributes["name"].Value.Equals(domain))
                {
                    Console.WriteLine(element.Attributes["name"].Value);
                    xmlServer.RemoveChild(element);
                }
                
            }
            xmlDocument.Save(xmlfile);
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("你确定要退出吗！", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
               
                 
                try
                {
                    System.Environment.Exit(0);
                    e.Cancel = false;  //点击OK 
                }
                catch(Exception ep)
                {

                }
                 
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void writeLog(string msg)
        {
            this.BeginInvoke((Action)(() =>
            {
                textBox1.AppendText(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
                textBox1.AppendText(Environment.NewLine);
                textBox1.ScrollToCaret();
            }));
        }
        

        private void button16_Click(object sender, EventArgs e)
        {
            AddApacheWeb aaw = new AddApacheWeb();
            aaw.StartPosition = FormStartPosition.CenterParent;
            aaw.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            if ((dataGridView1.Columns[e.ColumnIndex].Name == "Column1" && e.RowIndex >= 0))
            {

                int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string sql = "select * from apacheweblist where id=" + id;
                DataSet ds = DbHelperSQLite.Query(sql);


                Console.WriteLine(System.IO.Directory.Exists(ds.Tables[0].Rows[0]["path"].ToString()));

                if (true == System.IO.Directory.Exists(ds.Tables[0].Rows[0]["path"].ToString()))
                {
                    try
                    {
                        
                        string v_OpenFolderPath = ds.Tables[0].Rows[0]["path"].ToString(); 
                        System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
                    }
                    catch (Exception ep)
                    {
                        MessageBox.Show("打开网站目录失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }

                 

               
            }


            if (dataGridView1.Columns[e.ColumnIndex].Name == "apache_web_button" && e.RowIndex >= 0)
            {
                //说明点击的列是DataGridViewButtonColumn列
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string domain = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                Edit edit = new Edit(id);
                edit.StartPosition = FormStartPosition.CenterParent;
                edit.ShowDialog();




            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "apache_delete" && e.RowIndex >= 0)
            {
                //说明点击的列是DataGridViewButtonColumn列
                DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
                int id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string domain = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string port = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string type = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                DialogResult result = MessageBox.Show("你确定要删除["+id+"]("+ domain + ")？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    this.deleteApache(id,domain,port, type);
                 
                   
                }


            }


        }
 

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((dataGridView2.Columns[e.ColumnIndex].Name == "mysql_name" || dataGridView2.Columns[e.ColumnIndex].Name == "mysql_user" || dataGridView2.Columns[e.ColumnIndex].Name == "mysql_pass") && e.RowIndex >= 0)
                {
                    Clipboard.SetDataObject(dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    MessageBox.Show("已复制：" + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                }


                if (dataGridView2.Columns[e.ColumnIndex].Name == "mysql_edit" && e.RowIndex >= 0)
                {
                    //说明点击的列是DataGridViewButtonColumn列
                    DataGridViewColumn column = dataGridView2.Columns[e.ColumnIndex];
                    int id = int.Parse(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
                    
                    EditMysqlData edit = new EditMysqlData(id);

                    edit.StartPosition = FormStartPosition.CenterParent;
                    edit.ShowDialog();

                }


                if (dataGridView2.Columns[e.ColumnIndex].Name == "mysql_import" && e.RowIndex >= 0)
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Multiselect = true;
                    fileDialog.Title = "请选择文件";
                    fileDialog.Filter = "SQL文件|*.sql"; //设置要选择的文件的类型
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string dbname = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                        string dbuser = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
                        string dbpass = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
                        string conn = "server=localhost;port=3306;user="+dbuser+";password="+ dbpass + "; database=" + dbname;
                        string file = fileDialog.FileName;//返回文件的完整路径     

                        string[] param = new string[] { file , conn };

                        ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteSqlFile), (object)param);
                       // this.ExecuteSqlFile(file, conn);
                    }
                     


                }


                if (dataGridView2.Columns[e.ColumnIndex].Name == "mysql_delete" && e.RowIndex >= 0)
                {
                    //说明点击的列是DataGridViewButtonColumn列
                    DataGridViewColumn column = dataGridView2.Columns[e.ColumnIndex];
                    int id = int.Parse(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
                    string dbname = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
                    
                    DialogResult result = MessageBox.Show("你确定要删除数据库[" + id + "](" + dbname + ")？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        try
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(deleteMysql), id);
                        }
                        catch (Exception ee)
                        {

                        }
                        

                    }


                }


            }
            catch(Exception ep)
            {

            }
             
        }

        private void button18_Click(object sender, EventArgs e)
        {
            AddMysqlDatabase add_mysql = new AddMysqlDatabase();
            add_mysql.StartPosition = FormStartPosition.CenterParent;
            add_mysql.ShowDialog();
        }

        public static string config(string key) {

            string sql = "select * from `default` where Emun='" + key + "'";
            DataSet ds = DbHelperSQLite.Query(sql);
            string result = ds.Tables[0].Rows[0]["Value"].ToString();
            return result;
        }
 
        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void tool3_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            Form1.form1.load_apache_web();
            Form1.form1.load_mysql_list();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //还原窗体显示    
            WindowState = FormWindowState.Normal;
            //激活窗体并给予它焦点
            this.Activate();
            //任务栏区显示图标
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出吗！", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            this.getWindowsErrorLog();
        }

        private void ExecuteSqlFile(object state)
        {
            string[] p = (string[])state;
            using (StreamReader reader = new StreamReader(p[0], System.Text.Encoding.GetEncoding("utf-8")))
            {

                MySqlCommand command;
                MySqlConnection Connection = new MySqlConnection(p[1]);
                try
                {
                  
                   
                    Connection.Open();
                    string line = "";
                    string l;
                    while (true)
                    {
                        // 如果line被使用，则设为空
                        if (line.EndsWith(";"))
                            line = "";

                        l = reader.ReadLine();

                        // 如果到了最后一行，则退出循环
                        if (l == null) break;
                        // 去除空格
                        l = l.TrimEnd();
                        // 如果是空行，则跳出循环
                        if (l == "") continue;
                        // 如果是注释，则跳出循环
                        if (l.StartsWith("--")) continue;

                        // 行数加1 
                        line += l;
                        // 如果不是完整的一条语句，则继续读取
                        if (!line.EndsWith(";")) continue;
                        if (line.StartsWith("/*!"))
                        {
                            continue;
                        }

                        //执行当前行
                        command = new MySqlCommand(line, Connection);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("导入完成");

                }catch(Exception ep)
                {
                    Console.WriteLine(ep.ToString());
                    MessageBox.Show("导入异常！");
                }
                finally
                {
                    Connection.Close();
                }
            }

            
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://127.0.0.1:16888");
            }catch(Exception ep)
            {

            }
            
        }
    }
}
 