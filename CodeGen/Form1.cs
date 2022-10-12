using CodeGen.Template;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CodeGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 讀取資料表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadTable_Click(object sender, EventArgs e)
        {
            // 資料庫連線
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = txtConn.Text;
            conn.Open();

            // 讀取資料表清單
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.Tables ");
            sql.Append("ORDER BY TABLE_NAME");

            // 執行資料庫查詢動作
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Connection = conn;
            SqlDataAdapter adpt = new SqlDataAdapter();
            adpt.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adpt.Fill(ds);

            // 輸出資料表下拉
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    cboTable.Items.Add(dataRow["TABLE_NAME"]);
                }
                cboTable.SelectedIndex = 0;
            }

            // 關閉連線
            conn.Close();
        }

        /// <summary>
        /// 產生程式碼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenCode_Click(object sender, EventArgs e)
        {
            // 資料庫連線
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = txtConn.Text;
            conn.Open();

            // 取得資料表欄位
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT   M.COLUMN_NAME, M.IS_NULLABLE, M.DATA_TYPE, R2.CONSTRAINT_NAME ");
            sql.Append("FROM     INFORMATION_SCHEMA.Columns M ");
            sql.Append("LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE R2 ON R2.TABLE_NAME = M.TABLE_NAME AND R2.COLUMN_NAME = M.COLUMN_NAME AND R2.CONSTRAINT_NAME LIKE 'PK_%' ");
            sql.Append("WHERE    M.Table_Name = '" + cboTable.Text + "' ");
            sql.Append("ORDER BY ORDINAL_POSITION");

            // 執行資料庫查詢動作
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Connection = conn;
            SqlDataAdapter adpt = new SqlDataAdapter();
            adpt.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adpt.Fill(ds);

            // 範本欄位列表
            List<ColumnModel> Columns = new List<ColumnModel>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ColumnModel col = new ColumnModel();
                col.COLUMN_NAME = row["COLUMN_NAME"].ToString();
                col.IS_NULLABLE = row["IS_NULLABLE"].ToString();
                col.DATA_TYPE = row["DATA_TYPE"].ToString();
                col.CONSTRAINT_NAME = row["CONSTRAINT_NAME"].ToString();
                Columns.Add(col);
            }

            // 給予樣板資料
            BusinessTemplate template = new BusinessTemplate();
            template.TableName = cboTable.Text;
            template.Columns = Columns;

            // 取得樣板結果
            txtResult.Text = template.TransformText();

            // 關閉連線
            conn.Close();
        }
    }
}