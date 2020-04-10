using aweb.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Xml;

namespace aweb.ui
{
    public partial class AddApacheWeb : Form
    {
        public static AddApacheWeb addApacheWeb;
        public int tomcat_port = 12345;
        public AddApacheWeb()
        {
            addApacheWeb = this;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            this.textBox3.Text = path.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                string domain = this.textBox1.Text.Trim();
                string domain2 = this.textBox4.Text.Trim();
                string webindex = this.textBox7.Text;
                string port = this.textBox2.Text.Trim();
                string rewrite = this.textBox5.Text;
                string path = this.textBox3.Text.Trim();
                string ca = this.textBox6.Text;
                string key = this.textBox8.Text;
                string webtype = this.comboBox2.Text;
                string proxy_path = this.textBox10.Text;
                string proxy_url = this.textBox9.Text;

                int gzip = 0;
                int https = 0;
                int proxy = 0;
                string str_add = "";

                if (domain == null || domain.Equals(""))
                {
                    MessageBox.Show("主域名必须填写", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (port == null || port.Equals(""))
                {
                    MessageBox.Show("端口不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (path == null || path.Equals(""))
                {
                    MessageBox.Show("根目录不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (webindex == null || webindex.Equals(""))
                {
                    MessageBox.Show("默认首页不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.checkBox3.Checked)
                {
                    https = 1;
                    if (ca == null || ca.Equals(""))
                    {
                        MessageBox.Show("CA证书不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (key == null || key.Equals(""))
                    {
                        MessageBox.Show("证书私钥不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                if (this.checkBox4.Checked)
                {
                    proxy = 1;
                    if (proxy_path == null || proxy_path.Equals(""))
                    {
                        MessageBox.Show("反代配置目录不能为空,根目录请填写/", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (proxy_url == null || proxy_url.Equals(""))
                    {
                        MessageBox.Show("反代URL不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }



                //--start 判断库里是否已经存在此网站信息
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select count(1) from apacheweblist");
                strSql.Append(" where domain=@domain ");
                SQLiteParameter[] parameters = {
                                                            new SQLiteParameter("@domain")          };
                parameters[0].Value = domain;
                if (DbHelperSQLite.Exists(strSql.ToString(), parameters))
                {
                    MessageBox.Show("此域名网站已经存在，请勿重复添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //--end 判断库里是否已经存在此网站信息

                if (checkBox1.Checked)
                {
                    //--start 判断库里是否已经存在同名数据库信息
                    StringBuilder strSql2 = new StringBuilder();
                    strSql2.Append("select count(1) from mysql");
                    strSql2.Append(" where dbname=@dbname ");
                    SQLiteParameter[] parameters2 = {
                                                                new SQLiteParameter("@dbname")          };
                    parameters2[0].Value = domain.Replace(".", "_");
                    if (DbHelperSQLite.Exists(strSql2.ToString(), parameters2))
                    {
                        MessageBox.Show("此同名数据库已存在，请删除或手动添加其他名称数据库", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //--end 判断库里是否已经存在同名数据库信息
                }

                if (checkBox2.Checked)
                {
                    gzip = 1;
                }
                
                //--start 插入一条网站信息入软件Sqlite数据库
                string sql = "insert into apacheweblist(domain,domain2,webindex,port,path,rewrite,gzip,https,https_ca,https_key,proxy,proxy_path,proxy_url,type) values(@domain,@domain2,@webindex,@port,@path,@rewrite,@gzip,@https,@https_ca,@https_key,@proxy,@proxy_path,@proxy_url,@type)";
                SQLiteParameter[] paras = new SQLiteParameter[] { new SQLiteParameter("@domain", domain), new SQLiteParameter("@domain2", domain2), new SQLiteParameter("@webindex", webindex), new SQLiteParameter("@port", port), new SQLiteParameter("@path", path), new SQLiteParameter("@rewrite", rewrite), new SQLiteParameter("@gzip", gzip), new SQLiteParameter("@https", https), new SQLiteParameter("@https_ca", ca), new SQLiteParameter("@https_key", key), new SQLiteParameter("@proxy", proxy), new SQLiteParameter("@proxy_path", proxy_path), new SQLiteParameter("@proxy_url", proxy_url), new SQLiteParameter("@type", webtype) };


                int res = DbHelperSQLite.ExecuteSql(sql, paras);


                //--end 插入一条网站信息入软件Sqlite数据库

                if (res == 1)
                {
                    //--start 选中了创建mysql数据库，执行数据库创建操作
                    if (checkBox1.Checked)
                    {
                        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                        Random rd = new Random();
                        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        string nowRand = Convert.ToInt64(ts.TotalSeconds).ToString()+ "qwe@123@asd@456@zxc@789" + rd.Next().ToString();
                         
                        string dbpass = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(nowRand), 4, 8));
                        dbpass = dbpass.Replace("-", "");
                        dbpass = dbpass.ToLower();
                        int mysqlCreate = this.CreateMysqlDataBase(domain.Replace(".", "_"), domain.Replace(".", "_"), dbpass);
                        if (mysqlCreate == 1)
                        {
                            str_add = "";
                            string sql_insertmysql = "insert into mysql(dbname,dbpass,dbuser) values(@dbname,@dbpass,@dbuser)";
                            SQLiteParameter[] paras_insertmysql = new SQLiteParameter[] { new SQLiteParameter("@dbname", domain.Replace(".", "_")), new SQLiteParameter("@dbpass", dbpass), new SQLiteParameter("@dbuser", domain.Replace(".", "_")) };

                            DbHelperSQLite.ExecuteSql(sql_insertmysql, paras_insertmysql);
                            Form1.form1.load_mysql_list();
                        }
                        else
                        {
                            str_add = "mysql 创建失败!";
                        }
                    }
                    //--end 选中了创建mysql数据库，执行数据库创建操作

                    //
                    if (this.comboBox2.Text.Equals("JAVA"))
                    {
                        webindex = "";
                        int createTomcatConf = this.add_tomcat_web_xml(domain, path, tomcat_port.ToString());
                        if (createTomcatConf != 1)
                        {

                            str_add += "Tomcat配置文件写入失败!";
                        }
                        else
                        {
                            Form1.form1.button13_Click(null, null);
                        }
                     }
                    //

                    Form1.form1.writeLog("网站添加成功 " + str_add);
                    MessageBox.Show("网站添加成功\n" + str_add);
                    this.setApacheConf(domain, domain2, webindex, port, path,https);
                    Form1.form1.load_apache_web();
                    Form1.form1.button3_Click(null,null);
                    this.Hide();

                }

            }
            catch (Exception ep)
            {
                MessageBox.Show("新增网站失败：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Form1.form1.writeLog("新增网站失败");
            }


        }

        public void setApacheConf(string domain, string domain2, string webindex, string port, string path,int https)
        {
            string rewrite = this.textBox5.Text;
            string gzip = checkBox2.Checked ? @"
<IfModule deflate_module>
    SetOutputFilter DEFLATE
    DeflateCompressionLevel 6
    AddOutputFilterByType DEFLATE text/html text/plain text/xml text/css text/javascript application/javascript
    AddOutputFilter DEFLATE css|js|txt|xml|rss|html|htm
    Header append Vary User-Agent env=!dont-vary
    SetEnvIfNoCase Request_URI .(?:gif|jpe?g|png|bmp|tif|ico|eot|svg|ttf|woff)$ no-gzip dont-vary
    SetEnvIfNoCase Request_URI .(?:exe|t?gz|zip|7z|bz2|sit|rar|bin|iso)$ no-gzip dont-vary
    SetEnvIfNoCase Request_URI .(?:pdf|doc|docx|xls|xlsx|ppt|pptx)$ no-gzip dont-vary
    SetEnvIfNoCase Request_URI .(?:mov|flv|avi|mp3|mp4|rm|webm|ogv)$ no-gzip dont-vary
</IfModule>" : "";

            string proxy = checkBox4.Checked ? @"
    <Proxy *>
    Order deny,allow
    Allow from all
    </Proxy>
    ProxyPass "+this.textBox10.Text+ @" " + this.textBox9.Text + @"
    ProxyPassReverse " + this.textBox10.Text + @" " + this.textBox9.Text + @"" : "";

            string php_cgi = this.comboBox2.Text.Equals("PHP") ? @"
    FcgidInitialEnv PHPRC """+ Application.StartupPath.Replace("\\","/") + @"/soft/php/php7.0.9nts/""
    AddHandler fcgid-script .php
    FcgidWrapper """ + Application.StartupPath.Replace("\\", "/") + @"/soft/php/php7.0.9nts/php-cgi.exe"" .php
" : "";


            string conf = @"
<VirtualHost *:" + port + @">
    DocumentRoot """ + path + @"""
    ServerName " + domain + @" " + domain2 + @"
    ServerAlias
    "+ php_cgi + @"
" + gzip + @"
" + proxy + @"
  <Directory """ + path + @""">
      Options FollowSymLinks ExecCGI
      AllowOverride All
      Order allow,deny
      Allow from all
      Require all granted
      DirectoryIndex " + webindex + @"
  </Directory>
</VirtualHost>";


            string httpsconf = @"
<VirtualHost *:443>
    DocumentRoot """ + path + @"""
    ServerName " + domain + @" " + domain2 + @"
    ServerAlias
    " + php_cgi + @"
    SSLEngine on
    SSLProtocol TLSv1 TLSv1.1 TLSv1.2
    SSLCipherSuite HIGH:MEDIUM:!aNULL:!MD5
    SSLCertificateFile """ + Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\"+ domain + @"\ca.pem" + @"""
    SSLCertificateKeyFile """ + Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + domain + @"\private.key" + @"""
" + proxy + @"
  <Directory """ + path + @""">
      Options FollowSymLinks ExecCGI
      AllowOverride All
      Order allow,deny
      Allow from all
      Require all granted
      DirectoryIndex " + webindex + @"
  </Directory>
</VirtualHost>
";
            if (this.checkBox3.Checked)
            {
                conf = conf + httpsconf;

                try
                {
                    System.IO.Directory.CreateDirectory(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + domain);
                    System.IO.File.WriteAllText(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + domain + @"\ca.pem", this.textBox6.Text, new System.Text.UTF8Encoding(false));
                    System.IO.File.WriteAllText(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + domain + @"\private.key", this.textBox8.Text, new System.Text.UTF8Encoding(false));
                }
                catch(Exception ep)
                {
                    Form1.form1.writeLog("创建HTTPS证书文件失败");
                    MessageBox.Show("创建HTTPS证书文件失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            try
            {
                System.IO.File.WriteAllText(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\vhosts\\" + domain + "_" + port + ".conf", conf, new System.Text.UTF8Encoding(false));


                string listen = "Listen 443\n";
                string sql = "select * from apacheweblist group by port";
                SQLiteDataReader reader = DbHelperSQLite.ExecuteReader(sql);
                while (reader.Read())
                {
                    listen += "listen " + reader["port"] + "\n";
                }

                System.IO.File.WriteAllText(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\vhosts\\Listen.conf", listen, new System.Text.UTF8Encoding(false));


            }
            catch (Exception e)
            {
                Form1.form1.writeLog("创建网站配置文件失败");
                MessageBox.Show("创建网站配置文件失败！请删除后重试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string subPath = this.textBox3.Text.Trim();
            if (false == System.IO.Directory.Exists(subPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(subPath);
                }
                catch (Exception e)
                {
                    Form1.form1.writeLog("创建根目录失败");
                    MessageBox.Show("创建根目录失败，请自行创建", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     
                }
            }

            if (!rewrite.Equals(""))
            {

                try
                {
                    
                    if (false == System.IO.Directory.Exists(subPath))
                    {
                        System.IO.Directory.CreateDirectory(subPath);
                    }

                    System.IO.File.WriteAllText(this.textBox3.Text.Trim() + "\\.htaccess", rewrite, new System.Text.UTF8Encoding(false));
                }
                catch (Exception e)
                {
                    Form1.form1.writeLog("生成网站伪静态文件失败");
                    MessageBox.Show("生成网站伪静态文件失败！请自行创建", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
                 
            }






        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                textBox3.Text = Application.StartupPath + "\\www\\"+ textBox1.Text;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
 
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            var serviceControllers = ServiceController.GetServices();
            var MySQLService = serviceControllers.FirstOrDefault(service => service.ServiceName == Form1.form1.mysql);
            if (MySQLService == null)
            {
                MessageBox.Show(Form1.form1.mysql + "服务未安装", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkBox1.Checked = false;
                return;
            }
            else
            {
                if (MySQLService.Status != ServiceControllerStatus.Running)
                {

                    MessageBox.Show(Form1.form1.mysql + "服务未启动", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox1.Checked = false;
                    return;

                }
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            this.textBox5.Text = rewriteValue(this.comboBox1.Text);
           
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.Text.Equals("JAVA"))
            {
                var serviceControllers = ServiceController.GetServices();

                var TomcatService = serviceControllers.FirstOrDefault(service => service.ServiceName == Form1.form1.tomcat);
                if (TomcatService == null)
                {
                    this.comboBox2.Text = "静态网站";
                    MessageBox.Show(Form1.form1.tomcat + "服务不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;

                }
                else
                {
                    if (TomcatService.Status != ServiceControllerStatus.Running)
                    {
                        this.comboBox2.Text = "静态网站";
                        MessageBox.Show(Form1.form1.tomcat + "未启动！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                XmlDocument xmlDocument = new XmlDocument();
                string xmlfile = Form1.form1.rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\conf\\server.xml";
                xmlDocument.Load(xmlfile);


                XmlNode xmlServer = xmlDocument.SelectSingleNode("Server");
                XmlNodeList xmlServiceList = xmlServer.SelectNodes("Service");
                int[] ports = new int[xmlServiceList.Count];
                int i = 0;
                foreach (XmlElement element in xmlServiceList)
                {
                     
                    XmlNode con = element.SelectSingleNode("Connector");
                    ports[i] = int.Parse(con.Attributes["port"].Value);
                    i++;
                }
                int nowPort = ports.Max() + 1;
                tomcat_port = nowPort;
                this.checkBox4.Checked = true;
                this.textBox10.Text = "/";
                this.textBox9.Text = "http://127.0.0.1:" + nowPort + "/";

            }
            else
            {
                this.checkBox4.Checked = false;
                this.textBox10.Text = "/";
                this.textBox9.Text = "";
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox4_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text.Equals("JAVA"))
            {
                this.checkBox4.Checked = true;
                MessageBox.Show("网站程序类型为JAVA，必须选中Apache反向代理", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddApacheWeb_Load(object sender, EventArgs e)
        {

        }
    }
}
