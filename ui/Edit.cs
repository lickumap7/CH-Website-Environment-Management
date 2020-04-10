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
    public partial class Edit : Form
    {
        public static Edit edit;

        public int id;
        public int tomcat_port = 12345;

        public string oldDomain;
        public string oldPort;

        public Edit(int id)
        {
            edit = this;
            this.id = id;
            InitializeComponent();
        }


        private void Edit_Load(object sender, EventArgs e)
        {
           
          

           
            string sql = "select * from apacheweblist where id=" + id;
            DataSet ds = DbHelperSQLite.Query(sql);
            

            this.textBox1.Text = ds.Tables[0].Rows[0]["domain"].ToString();
            oldDomain = ds.Tables[0].Rows[0]["domain"].ToString();
            oldPort = ds.Tables[0].Rows[0]["port"].ToString();
            this.textBox4.Text = ds.Tables[0].Rows[0]["domain2"].ToString();
            this.textBox7.Text = ds.Tables[0].Rows[0]["webindex"].ToString();
            this.textBox2.Text = ds.Tables[0].Rows[0]["port"].ToString();
            this.textBox3.Text = ds.Tables[0].Rows[0]["path"].ToString();
            this.comboBox2.Text = ds.Tables[0].Rows[0]["type"].ToString();

            


            string ca_path = Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + oldDomain + @"\\ca.pem";
            string key_path = Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + oldDomain + @"\\private.key";

            if(int.Parse(ds.Tables[0].Rows[0]["https"].ToString()) == 1)
            {
                this.checkBox3.Checked = true;

                if (File.Exists(ca_path))
                {
                    StreamReader ca_string = new StreamReader(ca_path, System.Text.Encoding.Default);
                    textBox6.Text = ca_string.ReadToEnd();
                    ca_string.Close();
                }

                if (File.Exists(key_path))
                {
                    StreamReader key_string = new StreamReader(key_path, System.Text.Encoding.Default);
                    textBox8.Text = key_string.ReadToEnd();
                    key_string.Close();
                }

                 

            }

            /* string filename = openFileDialog1.FileName;
            StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default);
            textBox1.Text = sr.ReadToEnd();
            sr.Close(); */

            if (File.Exists(this.textBox3.Text.Trim() + "\\.htaccess"))
            {
               
                StreamReader sr = new StreamReader(this.textBox3.Text.Trim() + "\\.htaccess", System.Text.Encoding.Default);
                textBox5.Text = sr.ReadToEnd();
                sr.Close();
            }

         
            if(int.Parse(ds.Tables[0].Rows[0]["gzip"].ToString()) == 1)
            {
                this.checkBox2.Checked = true;
            }
            if (int.Parse(ds.Tables[0].Rows[0]["proxy"].ToString()) == 1)
            {
                this.checkBox4.Checked = true;
                this.textBox10.Text = ds.Tables[0].Rows[0]["proxy_path"].ToString();
                this.textBox9.Text = ds.Tables[0].Rows[0]["proxy_url"].ToString();
            }

            if (this.comboBox2.Text.Equals("JAVA"))
            {
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
                tomcat_port = ports.Max() + 1;
                this.textBox9.Text = "http://127.0.0.1:"+ tomcat_port + "/";
            }

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
                string dbPath = "Data Source=" + Directory.GetCurrentDirectory() + @"\web.db;Version=3;Password=;";
                using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                {
                    try
                    {
                        string domain = this.textBox1.Text.Trim();
                        string domain2 = this.textBox4.Text.Trim();
                        string webindex = this.textBox7.Text;
                        string port = this.textBox2.Text.Trim();
                        string path = this.textBox3.Text.Trim();
                        string rewrite = this.textBox5.Text;
                        string ca = this.textBox6.Text;
                        string key = this.textBox8.Text;
                        string proxy_path = this.textBox10.Text;
                        string proxy_url = this.textBox9.Text;
                        string webtype = this.comboBox2.Text;
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
                        if (checkBox2.Checked)
                        {
                            gzip = 1;
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




                        //,https,https_ca,https_key
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("update apacheweblist set domain=@domain,domain2=@domain2,webindex=@webindex,port=@port,path=@path,rewrite=@rewrite,gzip=@gzip,https=@https,https_ca=@https_ca,https_key=@https_key,proxy=@proxy,proxy_path=@proxy_path,proxy_url=@proxy_url,type=@type where id = @id");

                        SQLiteParameter[] parameters = new SQLiteParameter[] { new SQLiteParameter("@domain", domain), new SQLiteParameter("@domain2", domain2), new SQLiteParameter("@webindex", webindex), new SQLiteParameter("@port", port), new SQLiteParameter("@path", path), new SQLiteParameter("@rewrite", rewrite), new SQLiteParameter("@gzip", gzip), new SQLiteParameter("@https", https), new SQLiteParameter("@https_ca", ca), new SQLiteParameter("@https_key", key), new SQLiteParameter("@proxy", proxy), new SQLiteParameter("@proxy_path", proxy_path), new SQLiteParameter("@proxy_url", proxy_url), new SQLiteParameter("@type", webtype), new SQLiteParameter("@id", id) };
                        
                        int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);



                        //
                        if (this.comboBox2.Text.Equals("JAVA"))
                        {
                            webindex = "";
                            int deleteTomcatConf = this.delete_tomcat_web_xml(domain);
                            if(deleteTomcatConf == 1)
                            {
                                int createTomcatConf = this.add_tomcat_web_xml(domain, path, tomcat_port.ToString());
                                if (createTomcatConf != 1)
                                {

                                    str_add += ",Tomcat配置文件写入失败!";
                                }
                                else
                                {
                                    Form1.form1.button13_Click(null, null);
                                }
                            }
                            else
                            {
                                str_add += ",Tomcat配置文件写入失败!";
                            }
                             
                        }
                        //



                        if (rows > 0)
                        {
                            Form1.form1.writeLog("网站修改成功"+str_add);
                            MessageBox.Show("网站修改成功"+str_add);
                            this.setApacheConf(domain, domain2, webindex, port, path, https);
                            Form1.form1.load_apache_web();
                            Form1.form1.button3_Click(null, null);
                            this.Hide();


                        }

                      
                       
                    }
                    catch(Exception ep)
                    {
                        Form1.form1.writeLog("修改网站失败");
                        MessageBox.Show("修改网站失败：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         
                    }
                    

                }
            }
            catch (Exception ex)
            {
                 
                MessageBox.Show("连接数据库失败：" + ex.Message);
            }
        }

        public void setApacheConf(string domain, string domain2, string webindex, string port, string path,int https)
        {

            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\vhosts\\" + oldDomain + "_" + oldPort + ".conf");
                if (file.Exists)
                {
                    file.Delete();
                }

            }
            catch(Exception e)
            {

            }

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
    ProxyPass " + this.textBox10.Text + @" " + this.textBox9.Text + @"
    ProxyPassReverse " + this.textBox10.Text + @" " + this.textBox9.Text + @"" : "";

            string php_cgi = this.comboBox2.Text.Equals("PHP") ? @"
    FcgidInitialEnv PHPRC """ + Application.StartupPath.Replace("\\", "/") + @"/soft/php/php7.0.9nts/""
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
    SSLCertificateFile """ + Application.StartupPath + "\\soft\\apache\\Apache24\\conf\\ssl\\" + domain + @"\ca.pem" + @"""
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
                catch (Exception ep)
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
                Form1.form1.writeLog("写入网站配置文件失败");
                MessageBox.Show("写入网站配置文件失败！请删除后重试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
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

            if (rewrite != null || !rewrite.Equals(""))
            {

                try
                {
                    System.IO.File.WriteAllText(this.textBox3.Text.Trim() + "\\.htaccess", rewrite, new System.Text.UTF8Encoding(false));
                }
                catch (Exception e)
                {
                    Form1.form1.writeLog("写入网站伪静态文件失败");
                    MessageBox.Show("写入网站伪静态文件失败！请自行创建", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
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

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            this.textBox5.Text = AddApacheWeb.rewriteValue(this.comboBox1.Text);
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
        private void checkBox4_Click(object sender, EventArgs e)
        {
            if (this.comboBox2.Text.Equals("JAVA"))
            {
                this.checkBox4.Checked = true;
                MessageBox.Show("网站程序类型为JAVA，必须选中Apache反向代理", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public int add_tomcat_web_xml(string domain, string path, string port)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                string xmlfile = Form1.form1.rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\conf\\server.xml";
                xmlDocument.Load(xmlfile);
                XmlNode xmlServer = xmlDocument.SelectSingleNode("Server");
                Console.WriteLine(xmlServer.Name.ToLower());
                if (xmlServer.Name.ToLower() == "server")
                {
                    XmlElement xmlChild_Service = xmlDocument.CreateElement("Service");

                    XmlAttribute Service_name = xmlDocument.CreateAttribute("name");
                    Service_name.Value = domain;
                    xmlChild_Service.Attributes.SetNamedItem(Service_name);


                    //创建Connector--------------------------
                    XmlElement xmlElementInner_Connector = xmlDocument.CreateElement("Connector");
                    XmlAttribute Connector_port = xmlDocument.CreateAttribute("port");
                    Connector_port.Value = port;
                    xmlElementInner_Connector.Attributes.SetNamedItem(Connector_port);

                    XmlAttribute Connector_protocol = xmlDocument.CreateAttribute("protocol");
                    Connector_protocol.Value = "HTTP/1.1";
                    xmlElementInner_Connector.Attributes.SetNamedItem(Connector_protocol);

                    XmlAttribute Connector_connectionTimeout = xmlDocument.CreateAttribute("connectionTimeout");
                    Connector_connectionTimeout.Value = "20000";
                    xmlElementInner_Connector.Attributes.SetNamedItem(Connector_connectionTimeout);

                    XmlAttribute Connector_redirectPort = xmlDocument.CreateAttribute("redirectPort");
                    Connector_redirectPort.Value = "8443";
                    xmlElementInner_Connector.Attributes.SetNamedItem(Connector_redirectPort);
                    xmlChild_Service.AppendChild(xmlElementInner_Connector);
                    //--------------------------

                    //创建Engine
                    XmlElement xmlElementInner_Engine = xmlDocument.CreateElement("Engine");

                    XmlAttribute Engine_name = xmlDocument.CreateAttribute("name");
                    Engine_name.Value = "domain5";
                    xmlElementInner_Engine.Attributes.SetNamedItem(Engine_name);

                    XmlAttribute Engine_defaultHost = xmlDocument.CreateAttribute("defaultHost");
                    Engine_defaultHost.Value = "127.0.0.1";
                    xmlElementInner_Engine.Attributes.SetNamedItem(Engine_defaultHost);

                    XmlElement xmlElementInner_Engine_Realm = xmlDocument.CreateElement("Realm");

                    XmlAttribute Realm_className = xmlDocument.CreateAttribute("className");
                    Realm_className.Value = "org.apache.catalina.realm.LockOutRealm";
                    xmlElementInner_Engine_Realm.Attributes.SetNamedItem(Realm_className);


                    XmlElement xmlElementInner_Engine_Realm_Realm = xmlDocument.CreateElement("Realm");

                    XmlAttribute Realm_Realm_className = xmlDocument.CreateAttribute("className");
                    Realm_Realm_className.Value = "org.apache.catalina.realm.UserDatabaseRealm";
                    xmlElementInner_Engine_Realm_Realm.Attributes.SetNamedItem(Realm_Realm_className);


                    XmlAttribute Realm_Realm_resourceName = xmlDocument.CreateAttribute("resourceName");
                    Realm_Realm_resourceName.Value = "UserDatabase";
                    xmlElementInner_Engine_Realm_Realm.Attributes.SetNamedItem(Realm_Realm_resourceName);

                    xmlElementInner_Engine_Realm.AppendChild(xmlElementInner_Engine_Realm_Realm);

                    xmlElementInner_Engine.AppendChild(xmlElementInner_Engine_Realm);


                    XmlElement xmlElementInner_Engine_Host = xmlDocument.CreateElement("Host");


                    XmlAttribute Host_name = xmlDocument.CreateAttribute("name");
                    Host_name.Value = "127.0.0.1";
                    xmlElementInner_Engine_Host.Attributes.SetNamedItem(Host_name);

                    XmlAttribute Host_appBase = xmlDocument.CreateAttribute("appBase");
                    Host_appBase.Value = "webapps";
                    xmlElementInner_Engine_Host.Attributes.SetNamedItem(Host_appBase);

                    XmlAttribute Host_unpackWARs = xmlDocument.CreateAttribute("unpackWARs");
                    Host_unpackWARs.Value = "true";
                    xmlElementInner_Engine_Host.Attributes.SetNamedItem(Host_unpackWARs);

                    XmlAttribute Host_autoDeploy = xmlDocument.CreateAttribute("autoDeploy");
                    Host_autoDeploy.Value = "true";
                    xmlElementInner_Engine_Host.Attributes.SetNamedItem(Host_autoDeploy);


                    XmlElement xmlElementInner_Engine_Host_Value = xmlDocument.CreateElement("Value");


                    XmlAttribute Value_className = xmlDocument.CreateAttribute("className");
                    Value_className.Value = "org.apache.catalina.valves.AccessLogValve";
                    xmlElementInner_Engine_Host_Value.Attributes.SetNamedItem(Value_className);


                    XmlAttribute Value_directory = xmlDocument.CreateAttribute("directory");
                    Value_directory.Value = "logs";
                    xmlElementInner_Engine_Host_Value.Attributes.SetNamedItem(Value_directory);

                    XmlAttribute Value_prefix = xmlDocument.CreateAttribute("prefix");
                    Value_prefix.Value = domain + "_access_log";
                    xmlElementInner_Engine_Host_Value.Attributes.SetNamedItem(Value_prefix);

                    XmlAttribute Value_suffix = xmlDocument.CreateAttribute("suffix");
                    Value_suffix.Value = ".txt";
                    xmlElementInner_Engine_Host_Value.Attributes.SetNamedItem(Value_suffix);

                    XmlAttribute Value_pattern = xmlDocument.CreateAttribute("pattern");
                    Value_pattern.Value = "%h %l %u %t &quot;%r&quot; %s %b";
                    xmlElementInner_Engine_Host_Value.Attributes.SetNamedItem(Value_pattern);


                    XmlElement xmlElementInner_Engine_Host_Context = xmlDocument.CreateElement("Context");

                    XmlAttribute Context_docBase = xmlDocument.CreateAttribute("docBase");
                    Context_docBase.Value = path;
                    xmlElementInner_Engine_Host_Context.Attributes.SetNamedItem(Context_docBase);

                    XmlAttribute Context_reloadable = xmlDocument.CreateAttribute("reloadable");
                    Context_reloadable.Value = "true";
                    xmlElementInner_Engine_Host_Context.Attributes.SetNamedItem(Context_reloadable);

                    XmlAttribute Context_path = xmlDocument.CreateAttribute("path");
                    Context_path.Value = "";
                    xmlElementInner_Engine_Host_Context.Attributes.SetNamedItem(Context_path);

                    xmlElementInner_Engine_Host.AppendChild(xmlElementInner_Engine_Host_Context);
                    xmlElementInner_Engine_Host.AppendChild(xmlElementInner_Engine_Host_Value);
                    xmlElementInner_Engine.AppendChild(xmlElementInner_Engine_Host);
                    //--------------------------------------
                    xmlChild_Service.AppendChild(xmlElementInner_Engine);
                    xmlServer.AppendChild(xmlChild_Service);
                    xmlDocument.Save(xmlfile);
                }
                return 1;
            }
            catch (Exception ep)
            {
                Console.WriteLine("add xml:"+ep.Message);
                return 0;
            }



        }
        public int delete_tomcat_web_xml(string domain)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                string xmlfile = Form1.form1.rootPath + "\\soft\\tomcat\\apache-tomcat-8.5.53\\conf\\server.xml";
                Console.WriteLine(xmlfile);
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
                return 1;
            }
            catch(Exception ep)
            {
                Console.WriteLine("delete xml:" + ep.ToString());
                return 0;
            }
            

        }
    }
}
