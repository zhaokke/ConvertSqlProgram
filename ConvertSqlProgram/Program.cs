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
            string sql = "select * from tab_example where [username='@username'] and [pwd='@pwd'] and ({age>@age} or {gender='@gender'}) and {hobby like '%@hobby%'}";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("USERNAME", "SmallHan");
            dic.Add("HOBBY", "泡妹子");
            dic.Add("AGE", "23");
            dic.Add("GENDER", "男");
            dic.Add("PWD", "qwe123");
            ConvertSql convertSql = new ConvertSql(sql);
            string resSql = convertSql.DynamicStitchingSql(dic);
            Console.WriteLine(resSql);
            Console.ReadKey();
        }
    }
}
