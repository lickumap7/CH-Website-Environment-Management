using aweb.common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace aweb.ui
{
    public partial class AddApacheWeb
    {


        public int CreateMysqlDataBase(string dbname, string dbuser, string dbpass)
        {


            MySqlConnection mysqlcoon = new MySqlConnection();
            try
            {
                mysqlcoon = new MySqlConnection("Data Source=localhost;Persist Security Info=yes;port=" + Form1.config("mysql_port") + ";UserId=root; PWD=" + Form1.config("mysql_pass") + ";");
                mysqlcoon.Open();//必须打开通道之后才能开始事务
            }
            catch (Exception ep)
            {
                Form1.form1.writeLog("创建Mysql数据库异常！");
                MessageBox.Show("MySQL数据库错误：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            MySqlTransaction transaction = mysqlcoon.BeginTransaction();//事务必须在try外面赋值不然catch里的transaction会报错:未赋值
             
            try
            {
 
                string sql = "create database if not exists " + dbname + " default character set utf8mb4 collate utf8mb4_general_ci;";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlcoon);
                cmd.ExecuteNonQuery();
                this.CreateDatabaseUser(dbname, dbuser, dbpass);
                transaction.Commit();
                mysqlcoon.Close();
                return 1;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback(); 
                mysqlcoon.Close();
                MessageBox.Show("CreateMysqlDataBase数据库错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return 0;
            } 
 

        }

        public void CreateDatabaseUser(string dbname,string dbuser,string dbpass)
        {
            


            MySqlConnection mysqlcoon = new MySqlConnection();
            try
            {
                mysqlcoon = new MySqlConnection("Data Source=localhost;Persist Security Info=yes;port=" + Form1.config("mysql_port") + ";UserId=root; PWD=" + Form1.config("mysql_pass") + ";");
                mysqlcoon.Open();//必须打开通道之后才能开始事务
            }
            catch (Exception ep)
            {
                Form1.form1.writeLog("创建Mysql用户异常！");
                MessageBox.Show("MySQL数据库错误：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 
            }



            MySqlTransaction transaction = mysqlcoon.BeginTransaction();//事务必须在try外面赋值不然catch里的transaction会报错:未赋值
            Console.WriteLine("已经建立连接");
            try
            {

                /*create user 'dbuser'@'localhost' identified by 'erp_test@abc';grant select, insert, update, delete, create on *.* to erp_test;flush privileges;
                grant select, insert, update, delete, create on *.* to erp_test; 
                flush privileges; */

                string sql = "flush privileges;create user '" + dbuser + "'@'%' identified by '"+ dbpass + "';flush privileges;grant all privileges on `"+ dbname + "`.* to '"+dbuser+"'@'localhost' identified by '"+dbpass+"' with grant option;flush privileges;";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlcoon);
                cmd.ExecuteNonQuery();
                transaction.Commit();
                mysqlcoon.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                mysqlcoon.Close();
                MessageBox.Show("CreateDatabaseUser数据库错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                
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
            catch(Exception ep)
            {
                return 0;
            }
             


        }



        public static string rewriteValue(string program)
        {
            string value = "";
            switch (program)
            {
                case "ThinkPHP5":
                    value = WFC.apacheRewrite_thinkphp5;
                    break;
                case "Laraval":
                    value = WFC.apacheRewrite_laraval;
                    break;
                default:
                    value = "";
                    break;
            }
        
            return value;

        }




    }
}
