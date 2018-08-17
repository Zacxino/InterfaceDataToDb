using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterfaceDataToDb
{
    public partial class Form1 : Form
    {
        ServiceLdzz.LdzzWebServiceSoapClient ldzz = new ServiceLdzz.LdzzWebServiceSoapClient();
        ServiceYzl.WebScheduleSoapClient yzl = new ServiceYzl.WebScheduleSoapClient();
        DdService.DdServiceSoapClient Dd = new DdService.DdServiceSoapClient();
        public Form1()
        {
            InitializeComponent();
            //var obj = JObject.Parse(ldzz.Ldzz_TjByXzqh("330100", Convert.ToDateTime("2011-01-01"), Convert.ToDateTime("2011-12-31")));
            //var list = JObject.Parse(ldzz.Ldzz_XmlbByXzqh("330100", Convert.ToDateTime("2011-01-01"), Convert.ToDateTime("2011-12-31")));
            //var num = list["result"].Count();
            //var num1 = obj["result"].Count();
            this.button1.Enabled = true;
            this.button2.Enabled = false;
            this.lhzl.Enabled = false;
            this.SLFY.Enabled = false; //从2016起有数据
            this.SBLH.Enabled = false;
            this.pylh.Enabled = false;
            this.XZZGS.Enabled = false;
            this.FHL.Enabled = false;
            this.DE.Enabled = false;
            this.Tjxmgs.Enabled = false;
            this.DJBDetail.Enabled = false;
            this.OldtoNew.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            DateTime starttime = Convert.ToDateTime(year + "-01-01");
            DateTime endtime = Convert.ToDateTime(year + "-12-31");

            StringBuilder strSql = new StringBuilder();
            Hashtable ht = new Hashtable();
            strSql.Append(" select DQCODE, dqgrade, dqparerent, dqname");
            strSql.Append("   from  BS_DQ ");
            strSql.AppendFormat("  where DQPARERENT = '{0}'", "330000");
            strSql.Append("  order by DQID ");
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                var dqparent = item["DQCODE"].ToString();
                strSql.Clear();
                strSql.Append(" select DQCODE, dqgrade, dqparerent, dqname");
                strSql.Append("   from  BS_DQ ");
                strSql.AppendFormat("  where DQPARERENT = '{0}'", dqparent);
                strSql.Append("  order by DQID ");
                DataTable dataTable = DbHelperSQL.Query(strSql.ToString()).Tables[0];

                var list = JObject.Parse(ldzz.Ldzz_XmlbByXzqh(dqparent, starttime, endtime));
                //var num = list["result"].Count();
                //WHERE(DQPARERENT = '330100')
                if (list["success"].ToString().ToLower() == "true")
                {
                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        var area = list["result"][i]["county"].ToString();
                        DataRow row = dataTable.Select("DQNAME like '" + "%" + area.Remove(area.Length - 1, 1) + "%" + "'").FirstOrDefault();

                        StringBuilder sb = new StringBuilder();
                        sb.Append("insert into National_Application(");
                        sb.Append("ApplicationId,AreaType,DqName,Approve,ProjectName,ApprovalAuthority,ApprovalNumber,ShouldForestVegetationRestorationCosts,DqCode,Year,DqParent)");
                        sb.Append(" values (");
                        sb.Append("@ApplicationId,@AreaType,@DqName,@Approve,@ProjectName,@ApprovalAuthority,@ApprovalNumber,@ShouldForestVegetationRestorationCosts,@DqCode,@Year,@DqParent)");
                        SqlParameter[] parameters = {
                    //new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@ApplicationId", SqlDbType.NVarChar,14),
                    new SqlParameter("@AreaType", SqlDbType.NVarChar,9),
                    new SqlParameter("@DqName", SqlDbType.NVarChar,50),
                    new SqlParameter("@Approve", SqlDbType.NVarChar,5),
                    new SqlParameter("@ProjectName", SqlDbType.NVarChar,200),
                    new SqlParameter("@ApprovalAuthority", SqlDbType.NVarChar,20),
                    new SqlParameter("@ApprovalNumber", SqlDbType.NVarChar,30),
                    new SqlParameter("@ShouldForestVegetationRestorationCosts", SqlDbType.Decimal,13),
                    new SqlParameter("@DqCode", SqlDbType.NVarChar,50),
                    new SqlParameter("@Year", SqlDbType.NVarChar,50),
                    new SqlParameter("@DqParent", SqlDbType.NVarChar,50)
                    };
                        parameters[0].Value = list["result"][i]["ApplicationId"].ToString();
                        parameters[1].Value = list["result"][i]["AreaType"].ToString();
                        parameters[2].Value = list["result"][i]["county"].ToString();
                        parameters[3].Value = list["result"][i]["Approve"].ToString();
                        parameters[4].Value = list["result"][i]["ProjectName"].ToString();
                        parameters[5].Value = list["result"][i]["ApprovalAuthority"].ToString();
                        parameters[6].Value = list["result"][i]["ApprovalNumber"].ToString();
                        parameters[7].Value = Convert.ToDecimal(list["result"][i]["ShouldForestVegetationRestorationCosts"].ToString());
                        if (row != null)
                        {
                            parameters[8].Value = row["DQCODE"].ToString();
                        }
                        parameters[9].Value = year;
                        parameters[10].Value = dqparent;
                        ht.Add(sb, parameters);
                    }
                }

            }
            //try
            //{
            //    DbHelperSQL.ExecuteSqlTran(ht);
            //    MessageBox.Show("入库成功！");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("入库失败！");
            //    //throw;
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            DateTime starttime = Convert.ToDateTime(year + "-01-01");
            DateTime endtime = Convert.ToDateTime(year + "-12-31");

            //var obj = JObject.Parse(ldzz.Ldzz_TjByLdlx("330100", Convert.ToDateTime("2011-01-01"), Convert.ToDateTime("2011-12-31")));
            StringBuilder strSql = new StringBuilder();
            Hashtable ht = new Hashtable();
            strSql.Append(" select DQCODE, dqgrade, dqparerent, dqname");
            strSql.Append("   from  BS_DQ ");
            strSql.AppendFormat("  where DQGRADE = '{0}'", "2");
            strSql.AppendFormat(" or DQGRADE = '{0}'", "3");
            strSql.AppendFormat(" or DQGRADE = '{0}'", "4");
            strSql.Append("  order by DQID ");
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                var dqparent = item["DQCODE"].ToString();
                var list = JObject.Parse(ldzz.Ldzz_TjByLdlx(dqparent, starttime, endtime));
                if (list["success"].ToString().ToLower() == "true")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into National_TjZzld(");
                    sb.Append("DqName,DqCode,DqParent,AreaTotal,AreaTotalSum1,AreaTotalSum2,AreaTotalSum3,AreaTotalSum4,AreaTotalSum5,AreaTotalSum6,AreaTotalSum7,AreaTotalSum8,AreaTotalSum9,AreaTotalSum10,AreaTotalSum11,AreaTotalSum12,Year)");
                    sb.Append(" values (");
                    sb.Append("@DqName,@DqCode,@DqParent,@AreaTotal,@AreaTotalSum1,@AreaTotalSum2,@AreaTotalSum3,@AreaTotalSum4,@AreaTotalSum5,@AreaTotalSum6,@AreaTotalSum7,@AreaTotalSum8,@AreaTotalSum9,@AreaTotalSum10,@AreaTotalSum11,@AreaTotalSum12,@Year)");
                    SqlParameter[] parameters = {
                    //new SqlParameter("@ID", SqlDbType.Int,4),
                    new SqlParameter("@DqName", SqlDbType.NVarChar,50),
                    new SqlParameter("@DqCode", SqlDbType.NVarChar,50),
                    new SqlParameter("@DqParent", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotal", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum1", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum2", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum3", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum4", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum5", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum6", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum7", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum8", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum9", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum10", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum11", SqlDbType.NVarChar,50),
                    new SqlParameter("@AreaTotalSum12", SqlDbType.NVarChar,50),
                    new SqlParameter("@Year", SqlDbType.NVarChar,20)
                    };
                    parameters[0].Value = item["dqname"].ToString();
                    parameters[1].Value = dqparent;
                    parameters[2].Value = item["dqparerent"].ToString(); ;
                    parameters[3].Value = list["result"][0]["mjhj"].ToString();
                    parameters[4].Value = list["result"][0]["AreaTotalSum1"].ToString();
                    parameters[5].Value = list["result"][0]["AreaTotalSum2"].ToString();
                    parameters[6].Value = list["result"][0]["AreaTotalSum3"].ToString();
                    parameters[7].Value = list["result"][0]["AreaTotalSum4"].ToString();
                    parameters[8].Value = list["result"][0]["AreaTotalSum5"].ToString();
                    parameters[9].Value = list["result"][0]["AreaTotalSum6"].ToString();
                    parameters[10].Value = list["result"][0]["AreaTotalSum7"].ToString();
                    parameters[11].Value = list["result"][0]["AreaTotalSum8"].ToString();
                    parameters[12].Value = list["result"][0]["AreaTotalSum9"].ToString();
                    parameters[13].Value = list["result"][0]["AreaTotalSum10"].ToString();
                    parameters[14].Value = list["result"][0]["AreaTotalSum11"].ToString();
                    parameters[15].Value = list["result"][0]["AreaTotalSum12"].ToString();
                    parameters[16].Value = year;
                    ht.Add(sb, parameters);
                }
            }

            try
            {
                DbHelperSQL.ExecuteSqlTran(ht);
                MessageBox.Show("入库成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("入库失败！");
                //throw;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ExistSqlServerService("W3SVC"))
            {
                MessageBox.Show("IIS已经存在了");
            }
            else
            {
                MessageBox.Show("IIS没有安装");
            }
        }

        /// <summary>
        /// 如果不能实现 用如下方法在试一下： if (service[i].DisplayName.ToString() == tem )改成if (service[i].ServiceName.ToString() == tem )
        /// 或者if(ExistSqlServerService("W3SVC"))改成if(ExistSqlServerService("World Wide Web Publishing"))
        /// </summary>
        /// <param name="tem"></param>
        /// <returns></returns>
        public static bool ExistSqlServerService(string tem)
        {
            bool ExistFlag = false;
            List<int> vs = new List<int> { 1, 2, 3, 4, 6 };
            var list = vs.Skip(2).Take(3);
            ServiceController[] service = ServiceController.GetServices();
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].DisplayName.ToString() == tem)
                {
                    ExistFlag = true;
                }
            }
            return ExistFlag;
        }

        private void lhzl_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_PlainGreen("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_PlainGreen("", year));
                if (list["success"].ToString().ToLower() == "true")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_PlainGreen (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,No,EndDate,Afforestation,PlainAfforestation,ReturnAfforestation,LDLW,WLDHSLDDNXF,TimberForest,BambooForest,EconomicForest,EnergyForest,ProtectionForest,SpecialPurposeAfforest,StateForrest,CollectiveForrest,OwerShipForrest,Reforestation,Fencing,FencingDNXF,WLDHXLD,WLDHXLDDNXF,GMLHYLD,GMLHYLDDNXF,Other,YLDBZZL,LMFY,YLFY,CLFY,YLFYL,DCDXLGZ,ZLDG,YCDG,DCDGOther,LDJS,YMMJ,DTYM,RQYM,RQMZLZS,RQMZLBL,LXZS,ZLGXJH,ZLGXWC,WCL,RGZLJXF,PrintState,State,InsertTime,InsertUserName,InsertRealName,CFJD,HSJD,BCHCZJD,RDGX,TRGX,RGCJGX,SLFY,YWZSRS,TDYYFY,YWZSZS,KG,GF,ZG,RGZLMJ,VerityState,FLYYDZLMJ,XTSZZLMJ,ZGSZZLMJ,SSSZZLMJ,YLDZLMJ,SPLXZS,ZYLLFYMJ,WCLFY,YMZS,ZGSCGXMJ,RGGX,THLXF,DCGZ,THGZ,CSSLJS,SZMSL,SJSL,STZS,ZSZS,SLYWZSRS,YEAR)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Afforestation"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PlainAfforestation"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ReturnAfforestation"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LDLW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WLDHSLDDNXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TimberForest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BambooForest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EconomicForest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EnergyForest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ProtectionForest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SpecialPurposeAfforest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["StateForrest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CollectiveForrest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["OwerShipForrest"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Reforestation"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Fencing"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FencingDNXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WLDHXLD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WLDHXLDDNXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GMLHYLD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GMLHYLDDNXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Other"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YLDBZZL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LMFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YLFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CLFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YLFYL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["DCDXLGZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZLDG"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YCDG"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["DCDGOther"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LDJS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YMMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["DTYM"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RQYM"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RQMZLZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RQMZLBL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LXZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZLGXJH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZLGXWC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RGZLJXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertUserName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertRealName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CFJD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["HSJD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BCHCZJD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RDGX"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TRGX"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RGCJGX"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SLFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YWZSRS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TDYYFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YWZSZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["KG"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZG"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RGZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FLYYDZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["XTSZZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZGSZZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SSSZZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YLDZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SPLXZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZYLLFYMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCLFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YMZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZGSCGXMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RGGX"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["THLXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["DCGZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["THGZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CSSLJS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SZMSL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SJSL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["STZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZSZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SLYWZSRS"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }

                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }
                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("该年份没有数据！");
            }


        }

        private void SLFY_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_GetAfforest_Foster("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_GetAfforest_Foster("", year));
                if (list["success"].ToString().ToLower() == "true")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_GetAfforest_Foster (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,IYear,No,EndDate,ZHFY,TGF,SF,SCF,WSF,BZ,GGCC,GY,JT,GYL,SPL,ZLL,YLL,FYWCMJ,CFMJ,CCL,FYSYW,QDHTFS,ZJDW,DFCZ,WCTZ,PXCS,PXRC,SWZNY,LCGYYL,SYJZP,Other,QZJYJH,LWZSR,PrintState,State,InsertTime,InsertUserName,InsertRealName,VerityState,ZYDCount,JKSL,FYBT,ZLCB,FYBTM,YEAR)");
                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["IYear"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZHFY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TGF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SCF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WSF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GGCC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GYL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SPL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZLL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YLL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FYWCMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CFMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CCL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FYSYW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QDHTFS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZJDW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["DFCZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCTZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PXCS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PXRC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SWZNY"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LCGYYL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SYJZP"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Other"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QZJYJH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["LWZSR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertUserName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertRealName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZYDCount"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JKSL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FYBT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZLCB"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FYBTM"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }
                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void SBLH_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_GetSBLH("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_GetSBLH("", year));
                if (list["success"].ToString().ToLower() == "true")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_GetSBLH (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,No,EndDate,PrintState,State,InsertTime,SLTDJSLCHJ,SLTDJSMJHJ,GSGLLC,GSGLMJ,GSDLC,GSDMJ,QTGLLC,QTGLMJ,GSTLLC,GSTLMJ,PTTLLC,PTTLMJ,ZYHDCD,ZYHDMJ,QTHDCD,QTHDMJ,SLCZJS,WCTD,VerityState,Years,Year)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SLTDJSLCHJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SLTDJSMJHJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSGLLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSGLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSDLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSDMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QTGLLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QTGLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSTLLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GSTLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PTTLLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PTTLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZYHDCD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZYHDMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QTHDCD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["QTHDMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SLCZJS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCTD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Years"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }

                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void pylh_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_GetPlainG("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_GetPlainG("", year));
                if (list["success"].ToString().ToLower() == "true")
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_GetPlainG (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,No,EndDate,CSLH,CZLH,CuZLH,GLTLLH,GLTLLHLC,JHQDLH,JHQDLHCD,NTLW,JGLD,Other,WCQKHJ,CZTR,BMTR,XZTR,OtherTR,TZQKHJ,PrintState,State,InsertTime,InsertUserName,InsertRealName,VerityState,CSLHT,CZLHT,CuZLHT,GLTLLHT,JHQDLHT,NTLWT,JGLDT,OtherT,WCQKHJT,HJ,Years,Year)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CSLH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CZLH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CuZLH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GLTLLH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GLTLLHLC"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JHQDLH"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JHQDLHCD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["NTLW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JGLD"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Other"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCQKHJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CZTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BMTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["XZTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["OtherTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TZQKHJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertUserName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertRealName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CSLHT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CZLHT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CuZLHT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["GLTLLHT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JHQDLHT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["NTLWT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JGLDT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["OtherT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCQKHJT"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["HJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Years"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }

                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void XZZGS_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_GetZGTree("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_GetZGTree("", year));
                if (list["success"].ToString().ToLower() == "true")
                {

                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_GetZGTree (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,No,EndDate,PrintState,State,InsertTime,JHRW,JZXJ,JDZLMJ,JDZLZS,BZPYMJ,BZPYZS,SPZSZS,TRXJ,CZTR,SHTR,VerityState,InsertRealName,Years,Year)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());

                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JHRW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JZXJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JDZLMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JDZLZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BZPYMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["BZPYZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SPZSZS"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TRXJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CZTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SHTR"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertRealName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Years"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }

                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void FHL_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            //var tip = yzl.YZL_GetAfforest_Majorproject("", year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(yzl.YZL_GetAfforest_Majorproject("", year));
                if (list["success"].ToString().ToLower() == "true")
                {

                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into YZL_GetAfforest_Majorproject (");
                    strSql.Append("CodeId,CodeName,TownId,TownName,No,EndDate,NZJGZL,HSLZL,NZJGLDXF,JYRGZL,JYBZGZ,YHJGXJ,PYLDLW,RGZL,FSYL,SDZLXJ,HeJi,ZJDW,XJCZDW,YWCTZ,PrintState,State,InsertTime,InsertUserName,InsertRealName,VerityState,WCYZLKCMJ,WCZLZDMJ,SHTZ,PYLHGZ,YEAR)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["TownName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["No"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["EndDate"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["NZJGZL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["HSLZL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["NZJGLDXF"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JYRGZL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["JYBZGZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YHJGXJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PYLDLW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["RGZL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["FSYL"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SDZLXJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["HeJi"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ZJDW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["XJCZDW"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YWCTZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PrintState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["State"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertTime"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertUserName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["InsertRealName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["VerityState"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCYZLKCMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["WCZLZDMJ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["SHTZ"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["PYLHGZ"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }
                }

                try
                {
                    DbHelperSQL.ExecuteSqlTran(sqllist);
                    MessageBox.Show("入库成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("入库失败！");
                    //throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void DE_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            var tip = ldzz.Ldzz_de(year);
            List<string> sqllist = new List<string>();
            try
            {
                var list = JObject.Parse(ldzz.Ldzz_de(year));
                if (list["success"].ToString().ToLower() == "true")
                {

                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into LDZZ_DE (");
                    strSql.Append("CodeId,CodeName,OperateSet,OperateAdd,CountrySet,ProviceAdd,Lefts,YearPlanDown,KeysSone,IsDown,IsCalBack,Years,Year)");

                    for (int i = 0; i < list["result"].Count(); i++)
                    {
                        strSql.AppendFormat(" select '{0}'", list["result"][i]["CodeId"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CodeName"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["OperateSet"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["OperateAdd"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["CountrySet"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["ProviceAdd"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Lefts"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["YearPlanDown"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["KeysSone"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["IsDown"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["IsCalBack"].ToString());
                        strSql.AppendFormat(",'{0}'", list["result"][i]["Year"].ToString());
                        strSql.AppendFormat(",'{0}'", year);
                        strSql.AppendFormat(" UNION ALL ");
                    }
                    string insertStr = strSql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }

                    try
                    {
                        DbHelperSQL.ExecuteSqlTran(sqllist);
                        MessageBox.Show("入库成功！");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("入库失败！");
                        //throw;
                    }
                }
                else
                {
                    MessageBox.Show("该年份没有数据111！");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("该年份没有数据！");
            }
        }

        private void Tjxmgs_Click(object sender, EventArgs e)
        {
            var year = this.txtYear.Text.Trim();
            string dqcode = "330000";
            List<string> sqllist = new List<string>();
            if (string.IsNullOrEmpty(year))
            {
                MessageBox.Show("请输入年份");
                return;
            }
            DateTime starttime = Convert.ToDateTime(year + "-01-01");
            DateTime endtime = Convert.ToDateTime(year + "-12-31");

            //var tip = ldzz.Ldzz_TjByXzqh("330105", starttime, endtime);

            StringBuilder strSql = new StringBuilder();
            Hashtable ht = new Hashtable();
            strSql.Append(" select DQCODE, dqgrade, dqparerent, dqname");
            strSql.Append("   from  BS_DQ ");
            strSql.AppendFormat("  where DQPARERENT = '{0}'", dqcode);
            strSql.Append("  order by DQID ");
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];

            try
            {
                var jlist = JObject.Parse(ldzz.Ldzz_TjByXzqh(dqcode, starttime, endtime));

                if (jlist["success"].ToString().ToLower() == "true")
                {
                    StringBuilder Isql = new StringBuilder();
                    Isql.Append("insert into Ldzz_XMTJ (");
                    Isql.Append("CodeId,CodeName,hj,mj,CodeParent,Year)");

                    for (int i = 0; i < jlist["result"].Count(); i++)
                    {
                        Isql.AppendFormat(" select '{0}'", jlist["result"][i]["codeid"].ToString());
                        Isql.AppendFormat(",'{0}'", jlist["result"][i]["codename"].ToString());
                        Isql.AppendFormat(",'{0}'", jlist["result"][i]["hj"].ToString());
                        Isql.AppendFormat(",'{0}'", jlist["result"][i]["mj"].ToString());
                        Isql.AppendFormat(",'{0}'", dqcode);
                        Isql.AppendFormat(",'{0}'", year);
                        Isql.AppendFormat(" UNION ALL ");
                    }

                    foreach (DataRow item in dt.Rows)
                    {
                        var dqparent = item["DQCODE"].ToString();
                        strSql.Clear();
                        strSql.Append(" select DQCODE, dqgrade, dqparerent, dqname");
                        strSql.Append("   from  BS_DQ ");
                        strSql.AppendFormat("  where DQPARERENT = '{0}'", dqparent);
                        strSql.Append("  order by DQID ");
                        DataTable dataTable = DbHelperSQL.Query(strSql.ToString()).Tables[0];

                        foreach (DataRow dtxian in dataTable.Rows)
                        {
                            var dqxian = dtxian["DQCODE"].ToString();
                            var list = JObject.Parse(ldzz.Ldzz_TjByXzqh(dqxian, starttime, endtime));

                            if (list["success"].ToString().ToLower() == "true")
                            {
                                for (int i = 0; i < list["result"].Count(); i++)
                                {
                                    Isql.AppendFormat(" select '{0}'", dqxian);
                                    Isql.AppendFormat(",'{0}'", dtxian["dqname"].ToString());
                                    Isql.AppendFormat(",'{0}'", list["result"][i]["hj"].ToString());
                                    Isql.AppendFormat(",'{0}'", list["result"][i]["mj"].ToString());
                                    Isql.AppendFormat(",'{0}'", dqparent);
                                    Isql.AppendFormat(",'{0}'", year);
                                    Isql.AppendFormat(" UNION ALL ");
                                }
                            }
                        }


                    }

                    string insertStr = Isql.ToString();
                    if (insertStr.Contains(" UNION ALL "))
                    {
                        insertStr = insertStr.Substring(0, insertStr.Length - 10);
                        sqllist.Add(insertStr);
                    }

                    //try
                    //{
                    //    DbHelperSQL.ExecuteSqlTran(sqllist);
                    //    MessageBox.Show("入库成功！");
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("入库失败！");
                    //    //throw;
                    //}
                }
                else
                {
                    MessageBox.Show("该年份没有数据111！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("该年份没有数据！");
            }



            //var list = JObject.Parse(ldzz.Ldzz_TjByXzqh("330600", starttime, endtime));
        }

        private void DJBDetail_Click(object sender, EventArgs e)
        {
            //var list = Dd.YzUP("admin", "Admin@#!");

            int min = 21000;
            int max = 21500;
            List<string> sqllist = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select ApplicationId from National_Application where ID <= {0} and ID>{1} order by id ", max, min);

            DataTable dt = DbHelperSQL.Query(sb.ToString()).Tables[0];

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LDZZ_DZB (");
            strSql.Append("ApplicationId,ProjectName,Construction,Uses,ApprovalAuthority,ApprovalNumber,ApprovalAuthorityLevel,LandUnit,LegalProof,Postcode,Address,Telephone,Contact,Total,Farmland,Woodland,OtherFarmland,ConstructionLand,UnusedFarmland,AreaType,AreaDate)");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var applicationid = dt.Rows[i]["ApplicationId"].ToString();
                var list = JObject.Parse(ldzz.Ldzz_DjbByCode(applicationid));

                if (list["success"].ToString().ToLower() == "true")
                {
                    min++;
                    strSql.AppendFormat(" select '{0}'", list["result"][0]["applicationid"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["ProjectName"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Construction"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Uses"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["ApprovalAuthority"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["ApprovalNumber"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["ApprovalAuthorityLevel"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["LandUnit"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["LegalProof"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Postcode"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Address"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Telephone"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Contact"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Total"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Farmland"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["Woodland"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["OtherFarmland"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["ConstructionLand"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["UnusedFarmland"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["AreaType"].ToString());
                    strSql.AppendFormat(",'{0}'", list["result"][0]["AreaDate"].ToString());
                    strSql.AppendFormat(" UNION ALL ");
                }
            }
            string insertStr = strSql.ToString();
            if (insertStr.Contains(" UNION ALL "))
            {
                insertStr = insertStr.Substring(0, insertStr.Length - 10);
                sqllist.Add(insertStr);
            }
            try
            {
                DbHelperSQL.ExecuteSqlTran(sqllist);
                MessageBox.Show("入库成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("入库失败！");
                //throw;
            }
        }

        private void OldtoNew_Click(object sender, EventArgs e)
        {
            StringBuilder strSql = new StringBuilder();
            List<string> sqllist = new List<string>();
            strSql.Append(" select DQCODE, DQGRADE, DQPARERENT, DQNAME");
            strSql.Append("   from  BS_DQ ");
            strSql.AppendFormat("  where DQPARERENT = '{0}'", "330000");
            strSql.Append("  order by DQID ");
            DataTable dt = DbHelperSQL.Query(strSql.ToString()).Tables[0];

            StringBuilder str = new StringBuilder();
            str.Append("insert into AdministrativeDivisions (");
            str.Append("Id,Code,Name,[Level],ValidDate,InValidDate,ParentADId,SourceDId)");

            Guid id = Guid.NewGuid();
            var ValidDate = DateTime.Parse("1900-1-1");
            var InValidDate = DateTime.Parse("9999-12-31");

            str.AppendFormat(" select '{0}'", id);
            str.AppendFormat(",'{0}'", "330000");
            str.AppendFormat(",'{0}'", "浙江省");
            str.AppendFormat(",'{0}'", "2");
            str.AppendFormat(",'{0}'", ValidDate);
            str.AppendFormat(",'{0}'", InValidDate);
            str.Append(", null");
            str.Append(", null");
            str.AppendFormat(" UNION ALL ");
            foreach (DataRow item in dt.Rows)
            {
                Guid subid = Guid.NewGuid();
                str.AppendFormat(" select '{0}'", subid);
                str.AppendFormat(",'{0}'", item["DQCODE"].ToString());
                str.AppendFormat(",'{0}'", item["DQNAME"].ToString());
                str.AppendFormat(",'{0}'", item["DQGRADE"].ToString());
                str.AppendFormat(",'{0}'", ValidDate);
                str.AppendFormat(",'{0}'", InValidDate);
                str.AppendFormat(",'{0}'", id);
                str.Append(", null");
                str.AppendFormat(" UNION ALL ");

                var dqparent = item["DQCODE"].ToString();
                strSql.Clear();
                strSql.Append(" select DQCODE, DQGRADE, DQPARERENT, DQNAME");
                strSql.Append("   from  BS_DQ ");
                strSql.AppendFormat("  where DQPARERENT = '{0}'", dqparent);
                strSql.Append("  order by DQID ");
                DataTable dataTable = DbHelperSQL.Query(strSql.ToString()).Tables[0];

                
                foreach (DataRow subitme in dataTable.Rows)
                {
                    Guid ssubid = Guid.NewGuid();
                    str.AppendFormat(" select '{0}'", ssubid);
                    str.AppendFormat(",'{0}'", subitme["DQCODE"].ToString());
                    str.AppendFormat(",'{0}'", subitme["DQNAME"].ToString());
                    str.AppendFormat(",'{0}'", subitme["DQGRADE"].ToString());
                    str.AppendFormat(",'{0}'", ValidDate);
                    str.AppendFormat(",'{0}'", InValidDate);
                    str.AppendFormat(",'{0}'", subid);
                    str.Append(", null");
                    str.AppendFormat(" UNION ALL ");
                    createsql(str, subitme["DQCODE"].ToString(), ssubid);
                }
            }

            string insertStr = str.ToString();
            if (insertStr.Contains(" UNION ALL "))
            {
                insertStr = insertStr.Substring(0, insertStr.Length - 10);
                sqllist.Add(insertStr);
            }
            try
            {
                //DbHelperSQL.ExecuteSqlTran(sqllist);
               // MessageBox.Show("入库成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("入库失败！");
                //throw;
            }
        }

        public StringBuilder createsql(StringBuilder str,string dqcode,Guid id)
        {
            string sql = string.Format("select * from BS_DQ where DQPARERENT = '{0}' order by DQID", dqcode);
            DataTable dataTable = DbHelperSQL.Query(sql.ToString()).Tables[0];
            var ValidDate = DateTime.Parse("1900-1-1");
            var InValidDate = DateTime.Parse("9999-12-31");
            foreach (DataRow item in dataTable.Rows)
            {
                Guid subid = Guid.NewGuid();
                str.AppendFormat(" select '{0}'", subid);
                str.AppendFormat(",'{0}'", item["DQCODE"].ToString());
                str.AppendFormat(",'{0}'", item["DQNAME"].ToString());
                str.AppendFormat(",'{0}'", item["DQGRADE"].ToString());
                str.AppendFormat(",'{0}'", ValidDate);
                str.AppendFormat(",'{0}'", InValidDate);
                str.AppendFormat(",'{0}'", id);
                str.Append(", null");
                str.AppendFormat(" UNION ALL ");

                string sqlsq = string.Format("select * from BS_DQ where DQPARERENT = '{0}' order by DQID", item["DQCODE"].ToString());
                DataTable dt = DbHelperSQL.Query(sqlsq.ToString()).Tables[0];
                if (dt.Rows.Count>0)
                {
                    foreach (DataRow itsq in dt.Rows)
                    {
                        Guid ssubid = Guid.NewGuid();
                        str.AppendFormat(" select '{0}'", ssubid);
                        str.AppendFormat(",'{0}'", itsq["DQCODE"].ToString());
                        str.AppendFormat(",'{0}'", itsq["DQNAME"].ToString());
                        str.AppendFormat(",'{0}'", itsq["DQGRADE"].ToString());
                        str.AppendFormat(",'{0}'", ValidDate);
                        str.AppendFormat(",'{0}'", InValidDate);
                        str.AppendFormat(",'{0}'", subid);
                        str.Append(", null");
                        str.AppendFormat(" UNION ALL ");
                    }
                }
            }
            return str;
        }
    }
}
