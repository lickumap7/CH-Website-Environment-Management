using aweb.common;
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
    public partial class AddMysqlDatabase : Form
    {
        public AddMysqlDatabase()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                string dbname = this.textBox1.Text.Trim();
                string dbuser = this.textBox4.Text.Trim();
                string dbpass = this.textBox2.Text;
                if (dbname == null || dbname.Equals(""))
                {
                    MessageBox.Show("数据库名必须填写", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (dbuser == null || dbuser.Equals(""))
                {
                    MessageBox.Show("用户名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (dbpass == null || dbpass.Equals(""))
                {
                    MessageBox.Show("用户密码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //--start 判断库里是否已经存在同名数据库信息
                StringBuilder strSql2 = new StringBuilder();
                strSql2.Append("select count(1) from mysql");
                strSql2.Append(" where dbname=@dbname ");
                SQLiteParameter[] parameters2 = {
                                                                new SQLiteParameter("@dbname")          };
                parameters2[0].Value = dbname;
                if (DbHelperSQLite.Exists(strSql2.ToString(), parameters2))
                {
                    MessageBox.Show("此同名数据库已存在，请删除或手动添加其他名称数据库", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //--end 判断库里是否已经存在同名数据库信息


                //--start 插入一条数据库站信息入软件Sqlite数据库
                string sql = "insert into mysql(dbname,dbuser,dbpass) values(@dbname,@dbuser,@dbpass)";
                SQLiteParameter[] paras = new SQLiteParameter[] { new SQLiteParameter("@dbname", dbname), new SQLiteParameter("@dbuser", dbuser), new SQLiteParameter("@dbpass", dbpass) };
                int res = DbHelperSQLite.ExecuteSql(sql, paras);
                
                
                //--end 插入一条数据库信息入软件Sqlite数据库

                if(res == 1)
                {
                    AddApacheWeb apacheWeb = new AddApacheWeb();
                    int mysqlCreate = apacheWeb.CreateMysqlDataBase(dbname, dbuser, dbpass);
                    if (mysqlCreate == 1)
                    {

                        MessageBox.Show("数据库添加成功");
                        Form1.form1.load_mysql_list();
                        this.Hide();
                    }
                    else
                    {
                        StringBuilder delete_strSql = new StringBuilder();
                        delete_strSql.Append("delete from mysql ");
                        delete_strSql.Append(" where dbname=@dbname ");
                        SQLiteParameter[] delete_parameters = {
                                 new SQLiteParameter("@dbname")          };
                        delete_parameters[0].Value = dbname;
                        int rows = DbHelperSQLite.ExecuteSql(delete_strSql.ToString(), delete_parameters);
                    }

                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("新增数据库失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
