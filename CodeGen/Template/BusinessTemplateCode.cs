namespace CodeGen.Template
{
    public partial class BusinessTemplate
    {
        public string TableName { get; set; }
        public List<ColumnModel> Columns { get; set; }
    }

    public class ColumnModel
    {
        public string COLUMN_NAME { get; set; }
        public string IS_NULLABLE { get; set; }
        public string DATA_TYPE { get; set; }
        public string CONSTRAINT_NAME { get; set; }
    }
}
