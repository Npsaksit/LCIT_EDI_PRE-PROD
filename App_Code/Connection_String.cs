using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;  
using System.IO;
using System.Web.Script.Serialization;   
using System.Reflection; 

namespace sqlcon
{

		public class Connection_String
		{
			
			 public string connectionString = "Provider=OraOLEDB.Oracle.1;Password=lcit001$lcit;Persist Security Info=True;User ID=ops$lcit001;Data Source=MISPROD";
		     public string connectionCustoms = "Provider=SQLOLEDB;Data Source=lcitdbsrv.lcit.com;Database=Customs_Interchange;User ID=sa;Password=p@ssw0rd;";
		     public string connectionEbs = "Provider=OraOLEDB.Oracle.1;Password=lcitprodebsreport;Persist Security Info=True;User ID=OPS$EBS_REPORT;Data Source=TMSPROD";
		     public string connectionTMS = "Provider=OraOLEDB.Oracle.1;Password=lcit001$lcit;Persist Security Info=True;User ID=ops$lcit001;Data Source=TMSPROD";
		     public string edidbconnection = "Data Source=lcitedisrv.lcit.com;Initial Catalog=LCIT_EDI;User ID=sa;Password=p@ssw0rd";
		     public string checkTuckmaingate = "Data Source=lcitdbsrv.lcit.com;Initial Catalog=DB_TLC;User ID=sa;Password=p@ssw0rd";
		     public string CheckTruckCus = "Data Source=lcitdbsrv.lcit.com;Initial Catalog=Customs_Interchange;User ID=sa;Password=p@ssw0rd";
			
		}

}
