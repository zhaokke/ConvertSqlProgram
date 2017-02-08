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

            //2去掉未设定值得参数 {}
            //通过正则匹配出所有的{}
            Regex passedReg = new Regex(@"{\S+}", RegexOptions.IgnoreCase);
            MatchCollection mc = passedReg.Matches(sqlStr);
            //2.2 迭代正则得到的集合，替换sql内容或删除对应脚本
            foreach (Match m in mc)
            {
                //{tbd.balance_confirm_money>0}
                string conditionStr = Convert.ToString(m.Groups[0]);
                string[] conditionArray = conditionStr.Trim('{').Trim('}').Split(new string[] { ">", ">=", "<", "<=", "like", "not like", "=", "<>" }, StringSplitOptions.None);
                bool isExistsKey = dic.Keys.Contains(Convert.ToString(conditionArray[0]));
                if (isExistsKey)
                {
                    //取出关键字
                    string key = new Regex(">|>=|<|<=|like|not like|=|<>", RegexOptions.IgnoreCase).Match(conditionStr).ToString();
                    //{tbd.balance_confirm_money<0} 替换成 tbd.balance_confirm_money<value
                    bool isNumber = Regex.IsMatch(dic[conditionArray[0].Trim()], "[0-9]+");
                    string value = isNumber ? dic[conditionArray[0].Trim()] : string.Format("\'{0}\'", dic[conditionArray[0].Trim()]);
                    string newConditionStr = conditionArray[0].ToString() + key + value;
                    //替换对应的条件
                    sqlStr = Regex.Replace(sqlStr, conditionStr, newConditionStr, RegexOptions.IgnoreCase);
                }
                //条件未传时，则将该{tbd.balance_confirm_money>0}去掉
                else
                {
                    sqlStr = Regex.Replace(sqlStr, conditionStr, "", RegexOptions.IgnoreCase);
                }
            }

            //3将必传条件加入到sql，并取出[]
            MatchCollection mustMc = new Regex(@"\[\S+\]", RegexOptions.IgnoreCase).Matches(sqlStr);
            foreach (Match m in mustMc)
            {
                string mustStr = Convert.ToString(m.Groups[0]);
                string[] mustArray = mustStr.Trim('[').Trim(']').Split(new string[] { ">", ">=", "<", "<=", "like", "not like", "=", "<>" }, StringSplitOptions.None);
                string key = new Regex(">|>=|<|<=|like|not like|=|<>", RegexOptions.IgnoreCase).Match(mustStr).ToString();
                bool isNumber = Regex.IsMatch(dic[mustArray[0].Trim()], "[0-9]+");
                string value = isNumber ? dic[mustArray[0].Trim()] : string.Format("\'{0}\'", dic[mustArray[0].Trim()]);
                string newMustStr = mustArray[0].ToString() + key + value;
                mustStr = string.Format(@"\{0}", mustStr.Insert(mustStr.IndexOf(']'), @"\"));
                sqlStr = Regex.Replace(sqlStr, mustStr,newMustStr, RegexOptions.IgnoreCase);
            }
            //4.去掉所有的{}
            sqlStr = Regex.Replace(sqlStr,"({|})","", RegexOptions.IgnoreCase);
            //5.(or 与 (and 替换成 （
            sqlStr = Regex.Replace(sqlStr, @"\(\s*(or|and)+", "(", RegexOptions.IgnoreCase);
            //6. where or 与 where and 替换成 where 
            sqlStr = Regex.Replace(sqlStr, @"(where)*(or|and)+", "where", RegexOptions.IgnoreCase);
            //7. where)  替换成 where 
            sqlStr = Regex.Replace(sqlStr, @"(where)*(or|and)+", "where", RegexOptions.IgnoreCase);
            //8 最后为where替换为''
            sqlStr = Regex.Replace(sqlStr, @"(where)*$", "", RegexOptions.IgnoreCase);
            return sqlStr;
        }
    }
}
