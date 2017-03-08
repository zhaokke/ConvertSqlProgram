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
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select t1.province,
               t1.city,
               t1.relation_site,
               t1.bill_code,
               tso.same_site，
               tso.same_code,
               tb.send_site,
               tb.send_date,
               t1.scan_site,
               t1.scan_date,
               tp.register_site,
               tp.register_date,
               tp.problem_cause 
         from (select 
               tsc.bill_code,
               tsc.scan_site,
               tsc.scan_date,
               ts.province, 
               ts.city,
               ts.relation_site, 
          row_number()over(partition by tsc.bill_code order by tsc.scan_site desc )as aa
          from tab_scan_come tsc
          left join tab_site ts
            on tsc.scan_site = ts.site_name
          left join tab_sign si
            on si.bill_code = tsc.bill_code
         where ts.type <> '中心'
         and tsc.bill_type='主单'
         and (tsc.scan_site='@tsc.scan_site')
         and (ts.province='@ts.province')
         and (ts.city='@ts.city')
         and si.bill_code is not null) t1
         left join tab_problem tp on t1.bill_code=tp.bill_code
         left join tab_bill tb on tb.bill_code=t1.bill_code
         left join tab_scan_other tso on tso.bill_code=t1.bill_code
         where aa=1 ");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("TSC.SCAN_SITE", "东莞分公司");
            dic.Add("TS.PROVINCE", "广东省");
            dic.Add("TS.CITY", "东莞");
            ConvertSql convertSql = new ConvertSql(sb.ToString());
            string resSql = convertSql.DynamicStitchingSql(dic);
            Console.WriteLine(resSql);
            Console.ReadKey();
        }
    }
}
