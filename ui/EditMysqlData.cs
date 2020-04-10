using aweb.common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace aweb.ui
{
    public partial class EditMysqlData : Form
    {
        public int id;
        public string dbuser;
         
        public EditMysqlData(int id)
        {
            this.id = id;
            InitializeComponent();
        }

        private void EditMysqlData_Load(object sender, EventArgs e)
        {

          
            
            
             

            string sql = "select * from mysql where id=" + id;
            DataSet ds = DbHelperSQLite.Query(sql);

            

            this.dbuser = ds.Tables[0].Rows[0]["dbuser"].ToString();
            this.Text = "编辑-" + ds.Tables[0].Rows[0]["dbname"].ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                string pass = this.textBox4.Text.Trim();
                string pass_ = this.textBox2.Text.Trim();

                if (pass == null || pass.Equals(""))
                {
                    MessageBox.Show("新密码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pass != pass_)
                {
                    MessageBox.Show("两次输入的密码不一致", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                StringBuilder strSql = new StringBuilder();
                SQLiteParameter[] parameters;
                int res = this.changeMysqlPass(this.dbuser, pass);
                if(res == 1)
                {
                     
                    strSql.Append("update mysql set dbpass=@dbpass where id = @id");
                    parameters = new SQLiteParameter[] { new SQLiteParameter("@dbpass", pass), new SQLiteParameter("@id", id) };
                    int rows = DbHelperSQLite.ExecuteSql(strSql.ToString(), parameters);
                    if (rows > 0)
                    {
                        MessageBox.Show("数据库密码修改成功");
                        Form1.form1.writeLog("修改Mysql数据库密码");
                        Form1.form1.load_mysql_list();
                        this.Hide();
                    }
                    else
                    {
                        Form1.form1.writeLog("MySQL数据库密码修改成功,软件程序缓存修改失败");
                        MessageBox.Show("MySQL数据库密码修改成功,软件程序缓存修改失败");
                        Form1.form1.load_mysql_list();
                        this.Hide();
                    }

                }
                else
                {

                }
 


            }
            catch(Exception ep)
            {

            }
        }

        private int changeMysqlPass(string user,string pass)
        {

            MySqlConnection mysqlcoon = new MySqlConnection();
            try
            {
                mysqlcoon = new MySqlConnection("Data Source=localhost;Persist Security Info=yes;port=" + Form1.config("mysql_port") + ";UserId=root; PWD=" + Form1.config("mysql_pass") + ";");
                mysqlcoon.Open();//必须打开通道之后才能开始事务
            }
            catch (Exception ep)
            {
                Form1.form1.writeLog("修改Mysql密码异常！");
                MessageBox.Show("MySQL数据库错误：" + ep.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            MySqlTransaction transaction = mysqlcoon.BeginTransaction();//事务必须在try外面赋值不然catch里的transaction会报错:未赋值
             
            try
            {

                string sql = "use mysql;update mysql.user set authentication_string=password('"+ pass + "') where User='"+ user + "';flush privileges;";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlcoon);
                cmd.ExecuteNonQuery();
                
                transaction.Commit();
                mysqlcoon.Close();
                return 1;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
                mysqlcoon.Close();
                MessageBox.Show("MySQL数据库错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 0;
            }
        }
    }
}
