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
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using System.Management;
using System.Collections;

namespace aweb
{
    public partial class Form1
    {

        void Load_Mysql_List_Invoke()
        {
            this.load_mysql_list();
        }

        public void init()
        {
            //
            if(false == System.IO.Directory.Exists(rootPath + @"\soft"))
            {
                MessageBox.Show("未安装环境包", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    System.Environment.Exit(0);
                }
                catch (Exception ep)
                {

                }
            }
            //


            //PHP初始化START
            if (File.Exists(rootPath + @"\soft\php\php7.0.9nts\php.ini"))
            {
                INIUtil iniUtil = new INIUtil(rootPath + @"\soft\php\php7.0.9nts\php.ini");
                string php_extension_dir = "\"" + rootPath.Replace(@"\", @"/") + @"/soft/php/php7.0.9nts/ext""";
                string default_extension_dir = "";
                string php_session_save_path = "\"" + rootPath.Replace(@"\", @"/") + @"/soft/php/session_tmp""";
                string default_session_save_path = "";
                try
                {
                    default_extension_dir = iniUtil.ReadValue("extension_dir", "PHP");
                    if (!default_extension_dir.Equals(php_extension_dir))
                    {
                        iniUtil.Write("PHP", "extension_dir", php_extension_dir);
                    }

                    default_session_save_path = iniUtil.ReadValue("session.save_path", "Session");
                    if (!default_session_save_path.Equals(php_session_save_path))
                    {
                        iniUtil.Write("Session", "session.save_path", php_session_save_path);
                    }


                }
                catch (Exception ep)
                {

                }
            }
            
            //PHP初始化END


            //APACHE
            string apache_path = Application.StartupPath + @"\soft\apache\Apache24";
            string apache_conf_path = Application.StartupPath + @"\soft\apache\Apache24\conf\httpd.conf";
            if (File.Exists(apache_conf_path))
            {
                StreamReader apache_conf_string = new StreamReader(apache_conf_path, System.Text.Encoding.Default);
                string default_apache_conf_string = apache_conf_string.ReadToEnd();
                apache_conf_string.Close();
                if (!default_apache_conf_string.Contains(apache_path))
                {
                     
                    try
                    {
                        System.IO.File.WriteAllText(apache_conf_path, WFC.apache_conf, new System.Text.UTF8Encoding(false));
                    }
                    catch(Exception ep)
                    {
                        Console.WriteLine(ep.Message);
                    }
                   
                }
               
                
            }


            //APACHE
            //Console.WriteLine(WFC.apache_conf);
            string apache_phpmyadmin_conf_path = Application.StartupPath + @"\soft\apache\Apache24\conf\vhosts\demo.conf";
            try
            {
                System.IO.File.WriteAllText(apache_phpmyadmin_conf_path, WFC.apache_phpmyadmin_conf, new System.Text.UTF8Encoding(false));
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }

            //PHPMYADMIN



            //




            this.writeLog("初始化系统成功！");

        }


        /*
         * 监控各软件运行状态(Apache,MySQL,TomCat)
         */
        private void findServiceStatus(object state, bool timedout)
        {
            var serviceControllers = ServiceController.GetServices();

            var ApacheService = serviceControllers.FirstOrDefault(service => service.ServiceName == apache);
            if (ApacheService == null)
            {
                Action action = () =>
                {
                    this.label3.Text = "未安装";
                    this.label3.ForeColor = Color.Red;
                };
                Invoke(action);
            }
            else
            {
                if (ApacheService.Status != ServiceControllerStatus.Running)
                {

                    Action action = () =>
                    {
                        this.label3.Text = "未启动";
                        this.label3.ForeColor = Color.Red;
                    };
                    Invoke(action);

                }
                else
                {
                    Action action = () =>
                    {
                        this.label3.Text = "已启动";
                        this.label3.ForeColor = Color.Green;
                    };
                    Invoke(action);

                }



            }



            var MySQLService = serviceControllers.FirstOrDefault(service => service.ServiceName == mysql);
            if (MySQLService == null)
            {
                Action action = () =>
                {
                    this.label4.Text = "未安装";
                    this.label4.ForeColor = Color.Red;
                };
                Invoke(action);
            }
            else
            {
                if (MySQLService.Status != ServiceControllerStatus.Running)
                {

                    Action action = () =>
                    {
                        this.label4.Text = "未启动";
                        this.label4.ForeColor = Color.Red;
                    };
                    Invoke(action);

                }
                else
                {
                    Action action = () =>
                    {
                        this.label4.Text = "已启动";
                        this.label4.ForeColor = Color.Green;
                    };
                    Invoke(action);

                }



            }




            var TomcatService = serviceControllers.FirstOrDefault(service => service.ServiceName == tomcat);
            if (TomcatService == null)
            {
                Action action = () =>
                {
                    this.label5.Text = "未安装";
                    this.label5.ForeColor = Color.Red;
                };
                Invoke(action);
            }
            else
            {
                if (TomcatService.Status != ServiceControllerStatus.Running)
                {

                    Action action = () =>
                    {
                        this.label5.Text = "未启动";
                        this.label5.ForeColor = Color.Red;
                    };
                    Invoke(action);

                }
                else
                {
                    Action action = () =>
                    {
                        this.label5.Text = "已启动";
                        this.label5.ForeColor = Color.Green;
                    };
                    Invoke(action);

                }



            }


        }

        /*
         * 加载底部statusBar状态栏
         */
        private void statusBarEvent()
        {
            this.toolStripStatusLabel1.Text = "当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
             
            string strQuery = "select * from win32_OperatingSystem";
            SelectQuery queryOS = new SelectQuery(strQuery);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryOS);
             
            foreach (var os in searcher.Get())
            {
                this.toolStripStatusLabel2.Text = "操作系统：" + os["Caption"];
                 
            }
           


            timer1.Interval = 1000;
            timer1.Start();
             
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
             
            this.toolStripStatusLabel1.Text = "当前时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public void processInfo(object state, bool timedout)
        {
            object cpuUsage = 0;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
                var cpuTimes = searcher.Get()
                    .Cast<ManagementObject>()
                    .Select(mo => new
                    {
                        Name = mo["Name"],
                        Usage = mo["PercentProcessorTime"]
                    }
                    )
                    .ToList();

                var query = cpuTimes.Where(x => x.Name.ToString() == "_Total").Select(x => x.Usage);
                cpuUsage = query.SingleOrDefault();
            }
            catch(Exception ep)
            {

            }
           
          

            Action action = () =>
            {
                this.ucProcessWave2.Value = int.Parse(cpuUsage.ToString());
                if (this.ucProcessWave2.Value > 50)
                {
                    this.ucProcessWave2.ForeColor = Color.White;
                }
                else
                {
                    this.ucProcessWave2.ForeColor = Color.Black;
                }
            };
            Invoke(action);

            double available = 0.00;
            double capacity = 0.00;
            string used = "";
            string memory = "";
            try
            {
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                    {
                        available = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                    }
                }

                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (mo["TotalPhysicalMemory"] != null)
                    {
                        capacity = long.Parse(mo["TotalPhysicalMemory"].ToString());
                    }
                }

                /*  ManagementClass cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                  ManagementObjectCollection moc1 = cimobject1.GetInstances();
                  foreach (ManagementObject mo1 in moc1)
                  {
                      capacity += ((Math.Round(Int64.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024 / 1024.0, 1)));
                  }
                  moc1.Dispose();
                  cimobject1.Dispose();

                  ManagementClass cimobject2 = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory");
                  ManagementObjectCollection moc2 = cimobject2.GetInstances();
                  foreach (ManagementObject mo2 in moc2)
                  {
                      available += ((Math.Round(Int64.Parse(mo2.Properties["AvailableMBytes"].Value.ToString()) / 1024.0, 1)));

                  }
                  moc2.Dispose();
                  cimobject2.Dispose();*/
                used = ((capacity - available) / 1024 / 1024 / 1024).ToString("0.0");
                memory = ((capacity) / 1024 / 1024 / 1024).ToString("0.0");
            }
            catch(Exception ep)
            {

            }
            

             

            Action action_Memory = () =>
            {
                try
                {
                    this.label8.Text = "内存:" + used + "G/" + memory + "G";
                    this.ucProcessWave1.Value = int.Parse((Math.Round((capacity - available) / capacity * 100, 0)).ToString());
                    if(this.ucProcessWave1.Value > 50)
                    {
                        this.ucProcessWave1.ForeColor = Color.White;
                    }
                    else
                    {
                        this.ucProcessWave1.ForeColor = Color.Black;
                    }
                }catch(Exception ep)
                {
                    this.label8.Text = "内存获取失败";
                    this.ucProcessWave1.Value = 0;
                }
                
            };
            Invoke(action_Memory);
        }

        public void getWindowsErrorLog()
        {
             
            this.BeginInvoke((Action)(() =>
            {
                textBox2.Text = "";
            }));
            EventLog eventlog = new EventLog();
            eventlog.Log = "Application";
            List<Hashtable> logs = new List<Hashtable>();
            //"Application"应用程序, "Security"安全, "System"系统
            EventLogEntryCollection eventLogEntryCollection = eventlog.Entries;
            foreach (EventLogEntry entry in eventLogEntryCollection)
            {
                 
                if (@"Apache Service" == entry.Source.ToString() || @"MySQL" == entry.Source.ToString())
                {
                    string t = entry.TimeGenerated.ToLongDateString() + " " + entry.TimeGenerated.ToLongTimeString();
                    Hashtable hashtable = new Hashtable();
                    if (entry.EntryType.ToString().Equals("Error")) {
                        
                        hashtable.Add("Source", entry.Source.ToString());
                        hashtable.Add("time", t);
                        hashtable.Add("msg", entry.Message.ToString());


                        logs.Add(hashtable);
                    }
                }
            }
            logs.Reverse();
            int i = 0;
            foreach (Hashtable hashtable1 in logs)
            {
                
                 
                if (i<=100)
                {

                    this.BeginInvoke((Action)(() =>
                    {
                        string date = hashtable1["time"].ToString();
                        string log = @"-------------" + hashtable1["Source"].ToString() + @" Error Log----------------

时间：" + date + @"

日志：

" + hashtable1["msg"].ToString() + @"

";


                        textBox2.AppendText(log);
                        //textBox2.AppendText(Environment.NewLine);
                        textBox2.ScrollToCaret();
                    }));
                    i++;


                }
            }
        }



        /**
         * 加载apache网站列表
         */

        public void load_apache_web()
        {


            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            try
            {
                string sql = "select * from apacheweblist order by id desc";
                SQLiteDataReader reader = DbHelperSQLite.ExecuteReader(sql);
                int count = dataGridView1.RowCount;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        dataGridView1.Rows.Clear();
                    }
                }
                while (reader.Read())
                {
                    DataGridViewRow row = new DataGridViewRow();
                    int index = this.dataGridView1.Rows.Add(row);
                    this.dataGridView1.Rows[index].Cells[0].Value = reader["id"];
                    this.dataGridView1.Rows[index].Cells[1].Value = reader["domain"];
                    this.dataGridView1.Rows[index].Cells[2].Value = reader["port"];
                    this.dataGridView1.Rows[index].Cells[3].Value = reader["type"];
                    this.dataGridView1.Rows[index].Cells[4].Value = "编辑";
                    this.dataGridView1.Rows[index].Cells[5].Value = "删除";

                }

            }
            catch(Exception ep)
            {

            }
           
             

        }


        /**
        * 加载mysql数据库列表
        */

        public void load_mysql_list()
        {


            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            try
            {
                string sql = "select * from `mysql` order by id desc";
                SQLiteDataReader reader = DbHelperSQLite.ExecuteReader(sql);

                int count = dataGridView2.RowCount;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        dataGridView2.Rows.Clear();
                    }
                }
                while (reader.Read())
                {
                    Console.WriteLine(12321);
                    DataGridViewRow row = new DataGridViewRow();
                    int index = this.dataGridView2.Rows.Add(row);
                    this.dataGridView2.Rows[index].Cells[0].Value = reader["id"];
                    this.dataGridView2.Rows[index].Cells[1].Value = reader["dbname"];
                    this.dataGridView2.Rows[index].Cells[2].Value = reader["dbuser"];
                    this.dataGridView2.Rows[index].Cells[3].Value = reader["dbpass"]; ;
                    this.dataGridView2.Rows[index].Cells[4].Value = "改密";
                    this.dataGridView2.Rows[index].Cells[5].Value = "删除";
                    this.dataGridView2.Rows[index].Cells[6].Value = "导入";
                }
            }catch(Exception ep)
            {

            }



        }


        /**
         * 删除apache网站
         */

        public void deleteApache(int id,string domain,string port, string type)
        { 
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from apacheweblist ");
            strSql.Append(" where id=@ID ");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@ID", DbType.Int32,8)          };
            parameters[0].Value = id;
            int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                try
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\vhosts\\" + domain + "_" + port + ".conf");
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    if (type.Equals("JAVA"))
                    {
                        this.delete_tomcat_web_xml(domain);
                    }


                    this.load_apache_web();
                    this.writeLog("删除网站["+ domain + "]");
                    MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception e)
                {
                    MessageBox.Show("删除Apache网站配置文件失败：" + e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



            }
          
        }


        public void deleteMysql(object state)
        {
            int id = int.Parse(state.ToString());
            string sql = "select * from mysql where id=" + id;
            if (!DbHelperSQLite.Exists(sql))
            {
                MessageBox.Show("查询此数据库失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        
            
            DataSet ds = DbHelperSQLite.Query(sql);
             
            int res = this.deleteMysqlDataFromMySqlCommand(id, ds.Tables[0].Rows[0]["dbname"].ToString(), ds.Tables[0].Rows[0]["dbuser"].ToString());

            if (res == 1)
            { 
                string sql_insertmysql = "delete from mysql where id="+ id;


                DbHelperSQLite.ExecuteSql(sql_insertmysql);

                SetLoadingHide hideloding = new SetLoadingHide(Load_Mysql_List_Invoke);
                this.Invoke(hideloding);

                this.writeLog("删除数据库[" + ds.Tables[0].Rows[0]["dbname"].ToString() + "]");
                MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                MessageBox.Show("删除失败，请自行删除,code:102", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public int deleteMysqlDataFromMySqlCommand(int id,string dbname, string dbuser)
        {
            MySqlConnection mysqlcoon = new MySqlConnection();
            try
            {
                mysqlcoon = new MySqlConnection("Data Source=localhost;Persist Security Info=yes;port=" + Form1.config("mysql_port") + ";UserId=root; PWD=" + Form1.config("mysql_pass") + ";");
                mysqlcoon.Open();//必须打开通道之后才能开始事务
            }
            catch(Exception ep)
            {
                this.writeLog("操作删除Mysql数据库异常！");
                MessageBox.Show("MySQL数据库错误：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            
            

            MySqlTransaction transaction = mysqlcoon.BeginTransaction();//事务必须在try外面赋值不然catch里的transaction会报错:未赋值
            
            try
            {

                string sql = "drop database " + dbname + ";";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlcoon);
                cmd.ExecuteNonQuery();

                string sql2 = "Delete FROM mysql.user Where User ='" + dbuser + "';flush privileges;";
                MySqlCommand cmd2 = new MySqlCommand(sql2, mysqlcoon);
                cmd2.ExecuteNonQuery();

                transaction.Commit();
                mysqlcoon.Close();
                return 1;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                mysqlcoon.Close();
                this.writeLog("操作删除Mysql数据库异常！");
                MessageBox.Show("MySQL数据库错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 0;
            }
        }
    }
}
