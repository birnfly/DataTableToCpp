using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTableToCpp
{
    public class TableDescribeVO
    {
        public string TABLE_CATALOG;

        public string TABLE_SCHEMA;

        public string TABLE_NAME;

        public string COLUMN_NAME;

        public System.Decimal ORDINAL_POSITION;

        public string COLUMN_DEFAULT;

        public string IS_NULLABLE;

        public string DATA_TYPE;

        public System.Decimal CHARACTER_MAXIMUM_LENGTH;

        public System.Decimal CHARACTER_OCTET_LENGTH;

        public System.Decimal NUMERIC_PRECISION;

        public System.Decimal NUMERIC_SCALE;

        public string CHARACTER_SET_NAME;

        public string COLLATION_NAME;

        public string COLUMN_TYPE;

        public string COLUMN_KEY;

        public string EXTRA;

        public string PRIVILEGES;

        public string COLUMN_COMMENT; 
    }
}
