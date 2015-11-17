using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataTableToCpp
{
    class Helper
    {

        public static T RowToVO<T>(DataRow row)
        {
            T vo = System.Activator.CreateInstance<T>();

            Type voT = vo.GetType();
            Type rowT = row.GetType();
            foreach (FieldInfo f in voT.GetFields())
            {
                PropertyInfo pro = rowT.GetProperty(f.Name);
                if (pro == null)
                    continue;

                MethodInfo met = rowT.GetMethod("Is" + f.Name + "Null");
                if (met != null && (bool)(met.Invoke(row, null)) == true)
                    continue;

                f.SetValue(vo, pro.GetValue(row, null));
            }

            return vo;
        }

        public static List<T> DateTableToList<T>(DataTable dt)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Helper.RowToVO<T>(dt.Rows[i]));
            }

            return list;
        }



        public static string TableDescribeToFile(string headName,List<List<TableDescribeVO>> table)
        {
            headName = headName.ToUpper().Replace(".", "_");

            string fileTmp = getTemplete("File");

            string classCode = "";

            for (int i = 0; i < table.Count; i++)
            {
                string tableCode=TableDescribeToCppClass(table[i]);

                classCode += tableCode;
            }

            fileTmp = fileTmp.Replace("/*HEAD*/", headName);
            fileTmp = fileTmp.Replace("/*CLASS*/", classCode);

            return fileTmp;
        }


        // 表描述转换成类
        public static string TableDescribeToCppClass(List<TableDescribeVO> describe)
        {
            string classTmp = getTemplete("Class");



            //生成属性代码
            Regex e=new Regex("[\\t ]*\\/\\*PROPERTY\\*\\/");

            //取得缩进
            string indentation = e.Match(classTmp).Value.Replace("/*PROPERTY*/", "");

            string property="";

            for (int i = 0; i < describe.Count; i++)
            {
                property+=indentation + TableDescribeToCppProperty(describe[i])+"\r\n";
            }
            classTmp = classTmp.Replace(e.Match(classTmp).Value, property);

            //取得初始化数据
            e = new Regex("[\\t ]*\\/\\*FIELDINIT\\*\\/");
            string sIndent = e.Match(classTmp).Value.Replace("/*FIELDINIT*/", "");
            classTmp = classTmp.Replace(e.Match(classTmp).Value, sIndent+TableDescribeToFieldInit(describe[0]));

            //获取整数主键
            e = new Regex("[\\t ]*\\/\\*GETINTKEY\\*\\/");
            string sIndentIntKey = e.Match(classTmp).Value.Replace("/*GETINTKEY*/", "");
            classTmp = classTmp.Replace(e.Match(classTmp).Value, sIndentIntKey + TableDescribeToIntKey(describe[0]));


            //生成函数代码
            string functionCode = TableDescribeToCreateFunction(describe);
            e = new Regex("[\\t ]*\\/\\*CREATEROWCODE\\*\\/");
            Match m = e.Match(classTmp);
            while (m.Success){
                indentation = m.Value.Replace("/*CREATEROWCODE*/", "");

                string newCode = indentation+functionCode.Replace("\r\n", "\r\n" + indentation);

                classTmp = classTmp.Remove(m.Index, m.Length);
                classTmp=classTmp.Insert(m.Index, newCode);

                m = e.Match(classTmp);
            }

            //替换模板变量
            classTmp=classTmp.Replace("/*CLASSNAME*/", describe[0].TABLE_NAME);
            classTmp=classTmp.Replace("/*ROW*/","row");
            classTmp = classTmp.Replace("/*RESULT*/", "r");
            return classTmp;
        }

        // 表描述转换属性
        public static string TableDescribeToCppProperty(TableDescribeVO describe)
        {
            string cppType = getCPPType(describe);

            return cppType + " " + describe.COLUMN_NAME + "; //" + describe.COLUMN_COMMENT.Replace("\r","").Replace("\n","");

        }

        // 表描述转换字段初始化

        public static string TableDescribeToFieldInit(TableDescribeVO describe)
        {
            string cppType = getCPPType(describe);
            string sRet=describe.COLUMN_NAME+" = ";
            switch (cppType)
            {
                case CPPType.STRING:
                    sRet += "\"\"";
                    break;
                case CPPType.BOOL:
                    sRet += "false";
                    break;
                case CPPType.CHAR:
                    sRet += "\'0\'";
                    break;
                case CPPType.DOUBLE:
                case CPPType.FLOAT:
                case CPPType.INT:
                case CPPType.LONGLONG:
                default:
                    sRet += "0";
                    break;
            }

            sRet += ";";

            return sRet;

        }


        // 获取整数键值

        public static string TableDescribeToIntKey(TableDescribeVO describe)
        {
            string cppType = getCPPType(describe);

            string sRet =  "return ";

            if (cppType==CPPType.INT)
            {
                sRet += describe.COLUMN_NAME;
            }
            else
            {
                sRet += " -1";
            }
            
            sRet += ";";

            return sRet;

        } 
        
        // 表描述转换成功能代码
        public static string TableDescribeToCreateCode(TableDescribeVO describe)
        {
            string cppType = getCPPType(describe);
            string toCodeTemplate;
            switch (cppType)
            {
                case CPPType.STRING:
                    toCodeTemplate = getTemplete("ToString");
                    break;
                case CPPType.BOOL:
                    toCodeTemplate = getTemplete("ToBool");
                    break;
                case CPPType.CHAR:
                    toCodeTemplate = getTemplete("ToChar");
                    break;
                case CPPType.DOUBLE:
                    toCodeTemplate = getTemplete("ToDouble");
                    break;
                case CPPType.FLOAT:
                    toCodeTemplate = getTemplete("ToFloat");
                    break;
                case CPPType.INT:
                    toCodeTemplate = getTemplete("ToInt");
                    break;
                case CPPType.LONGLONG:
                    toCodeTemplate = getTemplete("ToLongLong");
                    break;
                default:
                    toCodeTemplate = getTemplete("ToString");
                    break;
            }

            toCodeTemplate=toCodeTemplate.Replace("/*P*/", describe.COLUMN_NAME);
            toCodeTemplate=toCodeTemplate.Replace("/*ROW*/", "row");

            return toCodeTemplate;
        }

        // 表描述转换成创建函数代码
        public static string TableDescribeToCreateFunction(List<TableDescribeVO> describe)
        {
            string code = describe[0].TABLE_NAME + " r;";

            for (int i = 0; i < describe.Count; i++)
            {
                code += "\r\n" + "r." + TableDescribeToCreateCode(describe[i]);
            }

            return code;
        }

     

        public static string getTemplete(string name)
        {
            System.IO.TextReader reader=new System.IO.StreamReader(Environment.CurrentDirectory+"\\template\\"+name+".h");
            return reader.ReadToEnd();
        }

        public static string getCPPType(TableDescribeVO describe)
        {
            string cppType;

            switch (describe.DATA_TYPE.ToUpper())
            {
                case SQLType.DATE:
                case SQLType.DATETIME:
                case SQLType.TIME:
                case SQLType.TIMESTAMP:
                case SQLType.YEAR:
                case SQLType.SMALLDATETIME:
                    cppType = CPPType.STRING;
                    break;
                case SQLType.BIGINT:
                    cppType = CPPType.LONGLONG;
                    break;
                case SQLType.DECIMAL:
                case SQLType.DOUBLE:
                case SQLType.MONEY:
                case SQLType.SMALLMONEY:
                case SQLType.REAL:
                    cppType = CPPType.DOUBLE;
                    break;
                case SQLType.FLOAT:
                    cppType = CPPType.FLOAT;
                    break;
                case SQLType.INT:
                case SQLType.MEDIUMINT:
                case SQLType.SMALLINT:
                case SQLType.INTEGER:
                case SQLType.ENUM:
                    cppType = CPPType.INT;
                    break;
                case SQLType.TINYINT:
                    cppType = CPPType.CHAR;
                    break;
                case SQLType.CHAR:
                case SQLType.VARCHAR:
                case SQLType.LONGTEXT:
                case SQLType.MEDIUMTEXT:
                case SQLType.TEXT:
                case SQLType.NCHAR:
                case SQLType.NVARCHAR:
                case SQLType.NTEXT:
                case SQLType.GRAPHIC:
                case SQLType.VARGRAPHIC:
                    cppType = CPPType.STRING;
                    break;
                case SQLType.BIT:
                    cppType = CPPType.BOOL;
                    break;
                default:
                    cppType = CPPType.STRING;
                    break;
            }

            return cppType;
        }
    }
}
