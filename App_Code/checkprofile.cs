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

namespace liner_profile
{
	public class checkprofile
	{
		sqlcon.Connection_String constr = new sqlcon.Connection_String();

		public DataTable getLiner_profile(string Line_ID)
		{
			SqlConnection conn = new SqlConnection();
			    	conn.ConnectionString = constr.edidbconnection;
			    	conn.Open();
			        SqlDataAdapter sda_checkliner = new SqlDataAdapter("SELECT LINE_ID,SENDERID_B5,SENDERID_C3,RECEIVERID,SENDER_TYPE,RUNNING_NUMBER FROM LCIT_EDI.DBO.LINER_PROFILE WHERE LINE_ID = '"+Line_ID+"'",conn);

			    	 DataTable dt_checkLiner = new DataTable();
			    	 dt_checkLiner.TableName ="LINER_PROFILE";
			    	 sda_checkliner.Fill(dt_checkLiner);

			    	 int updateRunning = Int32.Parse(dt_checkLiner.Rows[0]["RUNNING_NUMBER"].ToString())+1;
			 

			    	 SqlDataAdapter sda_updateRun = new SqlDataAdapter("UPDATE LCIT_EDI.DBO.LINER_PROFILE SET RUNNING_NUMBER='"+updateRunning+"' WHERE LINE_ID = '"+Line_ID+"'",conn);
			    	 DataTable dt_update = new DataTable();
			    	 sda_updateRun.Fill(dt_update);

			    	conn.Close();
		     return dt_checkLiner;
		}

		public DataTable getLiner_profile_operator_code()
		{
			SqlConnection conn_check_profile = new SqlConnection();
			    	conn_check_profile.ConnectionString = constr.edidbconnection;
			    	conn_check_profile.Open();
			        SqlDataAdapter sda_checkliner_2 = new SqlDataAdapter("SELECT * FROM LCIT_EDI.DBO.LINER_PROFILE",conn_check_profile);

			    	 DataTable dt_checkLiner_2 = new DataTable();
			    	 dt_checkLiner_2.TableName ="LINER_PROFILE";
			    	 sda_checkliner_2.Fill(dt_checkLiner_2);

			    	conn_check_profile.Close();
		     return dt_checkLiner_2;
		}
	}	

}