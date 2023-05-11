using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.Script.Services;

namespace WebApplicationWeb
{
    public partial class _Default : Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)  /*如果省略這句，下面的更新操作將無法完成，因为獲得的值是不變的*/
            {
                GetUserlist();
            }
            
        }
        [WebMethod] // POST
        public static string AddUser(string username,string age,string birthday)
        {
            string birthdayformat = string.Format("{0:yyyy-MM-dd HH:mm:ss}", birthday);
            SqlConnection connStr = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
            string insertTable = String.Format(@"INSERT INTO [dbo].[UserTable]
                                           ([Username],[Age],[birth])
                                     VALUES
                                           (N'{0}',
                                            '{1}',
                                            CONVERT(datetime, '{2}'))", username, age, birthdayformat); 
            SqlCommand cmd = new SqlCommand(insertTable, connStr);
            connStr.Open();
            cmd.ExecuteNonQuery();
            connStr.Close();

            return "success";
        }
        private void GetUserlist() 
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                string query = string.Format(@"select [Id],[Username],[Age],[birth]
                                   from [dbo].[UserTable]");
                using (SqlCommand cmdforAll = new SqlCommand(query))
                {
                    cmdforAll.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmdforAll))
                    {
                        sda.Fill(ds);
                    }
                }
            }
            GridView1.DataSource = ds;
            GridView1.DataBind();
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            GetUserlist();              /*再次绑定顯示編輯行的原數據,不進行绑定要點2次編輯才能跳到編輯狀態*/
        }
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            SqlConnection con = new SqlConnection(strConnString);
            String username = (GridView1.Rows[e.RowIndex].Cells[2].Controls[0] as TextBox).Text.ToString();    /*獲取要更新的數據*/
            String Age = (GridView1.Rows[e.RowIndex].Cells[3].Controls[0] as TextBox).Text.ToString();
            String birthday = (GridView1.Rows[e.RowIndex].Cells[4].Controls[0] as TextBox).Text.ToString();
            birthday = birthday.Replace("上午", "");
            string birthdayformat = string.Format("{0:yyyy-MM-dd HH:mm:ss}", birthday);

            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);/*獲取主鍵，需要設置 DataKeyNames，這裏設为 id */
            string update = string.Format
                (@"UPDATE [dbo].[UserTable] SET Username = N'{1}',Age='{2}',birth= CONVERT(datetime, '{3}')  WHERE Id = '{0}'", id,username, Age, birthdayformat);

            SqlCommand com = new SqlCommand(update, con);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            GridView1.EditIndex = -1;
            GetUserlist();
        }
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            SqlConnection con = new SqlConnection(strConnString);
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);/*獲取主鍵，需要設置 DataKeyNames，這裏設为 id */
            string delete = string.Format
                (@"DELETE FROM USERTABLE WHERE Id='{0}'", id);

            SqlCommand com = new SqlCommand(delete, con);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            GetUserlist();

        }
        protected void AddBtn_Click(Object sender, EventArgs e) 
        {
            
            string birthdayformat = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Request.Form["birthday"].ToString());
            SqlConnection connStr = new SqlConnection(ConfigurationManager.ConnectionStrings["connect"].ConnectionString);
            string insertTable = String.Format(@"INSERT INTO [dbo].[UserTable]
                                           ([Username],[Age],[birth])
                                     VALUES
                                           (N'{0}',
                                            '{1}',
                                            CONVERT(datetime, '{2}'))", Request.Form["username"].ToString(), Request.Form["age"], birthdayformat);
            SqlCommand cmd = new SqlCommand(insertTable, connStr);
            connStr.Open();
            cmd.ExecuteNonQuery();
            connStr.Close();
            GetUserlist();
        }
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;                 /*編輯索引賦值为-1，變回正常顯示狀態*/
            GetUserlist();
        }

        
    }
}