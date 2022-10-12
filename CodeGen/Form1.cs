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
        /// Ū����ƪ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadTable_Click(object sender, EventArgs e)
        {
            // ��Ʈw�s�u
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = txtConn.Text;
            conn.Open();

            // Ū����ƪ�M��
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.Tables ");
            sql.Append("ORDER BY TABLE_NAME");

            // �����Ʈw�d�߰ʧ@
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Connection = conn;
            SqlDataAdapter adpt = new SqlDataAdapter();
            adpt.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adpt.Fill(ds);

            // ��X��ƪ�U��
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    cboTable.Items.Add(dataRow["TABLE_NAME"]);
                }
                cboTable.SelectedIndex = 0;
            }

            // �����s�u
            conn.Close();
        }

        /// <summary>
        /// ���͵{���X
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenCode_Click(object sender, EventArgs e)
        {
            // ��Ʈw�s�u
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = txtConn.Text;
            conn.Open();

            // ���o��ƪ����
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT   M.COLUMN_NAME, M.IS_NULLABLE, M.DATA_TYPE, R2.CONSTRAINT_NAME ");
            sql.Append("FROM     INFORMATION_SCHEMA.Columns M ");
            sql.Append("LEFT JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE R2 ON R2.TABLE_NAME = M.TABLE_NAME AND R2.COLUMN_NAME = M.COLUMN_NAME AND R2.CONSTRAINT_NAME LIKE 'PK_%' ");
            sql.Append("WHERE    M.Table_Name = '" + cboTable.Text + "' ");
            sql.Append("ORDER BY ORDINAL_POSITION");

            // �����Ʈw�d�߰ʧ@
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql.ToString();
            cmd.Connection = conn;
            SqlDataAdapter adpt = new SqlDataAdapter();
            adpt.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adpt.Fill(ds);

            // �d�����C��
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

            // �����˪O���
            BusinessTemplate template = new BusinessTemplate();
            template.TableName = cboTable.Text;
            template.Columns = Columns;

            // ���o�˪O���G
            txtResult.Text = template.TransformText();

            // �����s�u
            conn.Close();
        }
    }
}