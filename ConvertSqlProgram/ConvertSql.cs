using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConvertSqlProgram
{
    public class ConvertSql
    {
        private string sqlStr;
        public string SqlStr
        {
            get { return sqlStr; }
            set
            {
                this.sqlStr = value.ToUpper();
            }
        }

        public ConvertSql(string _sqlSql)
        {
            SqlStr = _sqlSql;
        }
        public ConvertSql() { }

        /// <summary>
        /// 动态拼接sql
        /// </summary>
        /// <param name="dic">用户传过来的值</param>
        /// <returns></returns>
        public string DynamicStitchingSql(Dictionary<string, string> dic = null)
        {
            //1.将所有转行或空字符转换为空格
            sqlStr = Regex.Replace(sqlStr, @"\s+", " ", RegexOptions.IgnoreCase);

            //2 将Dic中key值 在sql条件中替换成 [ ]
            foreach(var itKey in dic.Keys)
            {
                string conditionReg = @"(\[|\{)" + itKey + ".+?"+@"(\}|\])";
                MatchCollection mc = new Regex(conditionReg).Matches(sqlStr);
                foreach (Match m in mc)
                {
                    string conditionStr = Convert.ToString(m.Groups[0]);
                    conditionStr = conditionStr.Replace("{", "[").Replace("}", "]");
                    sqlStr = Regex.Replace(sqlStr, conditionReg, conditionStr, RegexOptions.IgnoreCase);
                }
            }
            // 去掉包含 {}的条件
            sqlStr = Regex.Replace(sqlStr, @"(\{).+?(\})", "", RegexOptions.IgnoreCase);
            //替换值
            RelpaceValue(dic);
            //去掉所有的[]
            sqlStr = Regex.Replace(sqlStr,@"(\[|\])","", RegexOptions.IgnoreCase);
            //(or 与 (and 替换成 （ 
            sqlStr = Regex.Replace(sqlStr, @"\(\s*(or|and)+", "(", RegexOptions.IgnoreCase);
            //where)  替换成 where 
            sqlStr = Regex.Replace(sqlStr, @"where\s*\)", "where", RegexOptions.IgnoreCase);
            //where and|or () 替换为 where
            sqlStr = Regex.Replace(sqlStr, @"(where)\s+(and|or)\s*\(\s*\)", "where", RegexOptions.IgnoreCase);
            // where or 与 where and 替换成 where 
            sqlStr = Regex.Replace(sqlStr, @"(where)\s*(or|and)+", "where", RegexOptions.IgnoreCase);
            // and与or()and  替换为 and
            sqlStr = Regex.Replace(sqlStr, @"(or|and)\s*\(\s*\)\s*and\s*$", "and", RegexOptions.IgnoreCase);
            // and与or()and  替换为 or
            sqlStr = Regex.Replace(sqlStr, @"(or|and)\s*\(\s*\)\s*or\s*$", "or", RegexOptions.IgnoreCase);
            // and()and与or替换为 and 
            sqlStr = Regex.Replace(sqlStr, @"and\s*\(\s*\)\s*(and|or)", "and", RegexOptions.IgnoreCase);
            // or与and与or替换为 or 
            sqlStr = Regex.Replace(sqlStr, @"or\s*\(\s*\)\s*(and|or)", "or", RegexOptions.IgnoreCase);
            // or)与and)与or替换为 or 
            sqlStr = Regex.Replace(sqlStr, @"or\s*\(\s*\)\s*(and|or)", "or", RegexOptions.IgnoreCase);
            //or与and) 替换为 ）
            sqlStr = Regex.Replace(sqlStr, @"\s*(and|or)\s*\)", ")", RegexOptions.IgnoreCase);
            //where ()and与or
            sqlStr = Regex.Replace(sqlStr, @"where\s*\(\s*\)\s*(and|or)*", "where", RegexOptions.IgnoreCase);
            //最后为 and 或 or替换为 ''
            sqlStr = Regex.Replace(sqlStr, @"(and|or)*(\s|\(\s*\))*$", "", RegexOptions.IgnoreCase);
            // 最后为where替换为''
            sqlStr = Regex.Replace(sqlStr.Trim(), @"(where)*$", "", RegexOptions.IgnoreCase);
            return sqlStr;
        }

        /// <summary>
        /// 替换值
        /// </summary>
        /// <param name="dic"></param>
        private void RelpaceValue(Dictionary<string, string> dic)
        {
            foreach (var it in dic)
            {
                string conditionReg = @"(\[)" + it.Key + "(.+)@" + it.Key + @"(\])*";
                string conditionStr=Regex.Match(sqlStr, conditionReg).Groups.ToString();
                sqlStr = Regex.Replace(sqlStr, "@" + it.Key, it.Value);
            }
        }
    }
}
