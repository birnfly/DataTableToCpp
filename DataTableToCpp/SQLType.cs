using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTableToCpp
{
    public class SQLType
    {
        //日期时间
        public const string DATE = "DATE";
        public const string DATETIME = "DATETIME";
        public const string TIME = "TIME";
        public const string TIMESTAMP = "TIMESTAMP";
        public const string YEAR = "YEAR";
        public const string SMALLDATETIME = "SMALLDATETIME";

        //数字
        public const string BIGINT = "BIGINT";
        public const string DECIMAL = "DECIMAL";//double
        public const string DOUBLE = "DOUBLE";
        public const string FLOAT = "FLOAT";
        public const string INT = "INT";
        public const string MEDIUMINT = "MEDIUMINT";
        public const string SMALLINT = "SMALLINT";
        public const string TINYINT = "TINYINT";
        public const string MONEY = "MONEY";
        public const string SMALLMONEY = "SMALLMONEY";
        public const string INTEGER = "INTEGER";
        public const string REAL = "REAL";
        
        //字符串
        public const string CHAR = "CHAR";
        public const string VARCHAR = "VARCHAR";
        public const string LONGTEXT = "LONGTEXT";
        public const string MEDIUMTEXT = "MEDIUMTEXT";
        public const string TEXT = "TEXT";
        public const string NCHAR = "NCHAR";
        public const string NVARCHAR = "NVARCHAR";
        public const string NTEXT = "NTEXT";
        public const string GRAPHIC = "GRAPHIC";
        public const string VARGRAPHIC = "VARGRAPHIC";

        //其他
        public const string BIT = "BIT";
        public const string ENUM = "ENUM";
    }
}
