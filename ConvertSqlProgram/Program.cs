using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertSqlProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string strSql = "select * from tab_userinfo where {username=@username} and [pwd=@pwd] and ({age>@age} or {gender=@gender}) and [hobby like '%@hobby%']";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("USERNAME", "小瀚");
            dic.Add("PWD", "qwe123456");
            dic.Add("AGE", "20");
            dic.Add("GENDER", "男");
            dic.Add("HOBBY", "Code");
            ConvertSql convertSql = new ConvertSql(strSql);
            string resSql = convertSql.DynamicStitchingSql(dic);
            Console.WriteLine(resSql);
            Console.ReadKey();
        }
    }
}
