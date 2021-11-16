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

namespace sender_profile
{
	public class senderprofile
	{
				sqlcon.Connection_String constr = new sqlcon.Connection_String();
		public DataTable getSender_profile_email()
		{
			SqlConnection conn_check_profile = new SqlConnection();
			    	conn_check_profile.ConnectionString = constr.edidbconnection;
			    	conn_check_profile.Open();
			        SqlDataAdapter sda_checksender = new SqlDataAdapter("SELECT REPLACE(REPLACE(SEND_TO,'<',''),'>','') AS SEND_TO,REPLACE(REPLACE(CC,'<',''),'>','') AS CC,LINE_ID FROM LCIT_EDI.DBO.EMAIL_SENDER",conn_check_profile);

			    	 DataTable dt_checkSender = new DataTable();
			    	 dt_checkSender.TableName ="LINER_PROFILE";
			    	 sda_checksender.Fill(dt_checkSender);

			    	conn_check_profile.Close();
		     return dt_checkSender;
		}


		public DataTable getSender_profile_ftp()
		{
			SqlConnection conn_check_profile = new SqlConnection();
			    	conn_check_profile.ConnectionString = constr.edidbconnection;
			    	conn_check_profile.Open();
			        SqlDataAdapter sda_checksender = new SqlDataAdapter("SELECT * FROM LCIT_EDI.DBO.FTP_CUSTOMER",conn_check_profile);

			    	 DataTable dt_checkSender = new DataTable();
			    	 dt_checkSender.TableName ="SENDER_PROFILE_FTP";
			    	 sda_checksender.Fill(dt_checkSender);
			    	 
			    	
			    	
			    	 
			    	 // for(int check_endcode=0 ; check_endcode < dt_checkSender.Rows.Count;check_endcode++)
			    	 // {
			    	 // 	var encodeing = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes( dt_checkSender.Rows[check_endcode]["PASS_FTP"].ToString()));
			    	 // 	dt_checkSender.Rows[check_endcode]["PASS_FTP"].Item = encodeing.ToString();

			    	 // }
			    	 

			    	conn_check_profile.Close();
		     return dt_checkSender;
		}

		public DataTable UpdateSender_profile_email(String To, String CC, String Line_oper)
		{
			SqlConnection conn_update_email = new SqlConnection();
					conn_update_email.ConnectionString = constr.edidbconnection;
			        SqlDataAdapter std_update_email = new SqlDataAdapter("UPDATE LCIT_EDI.dbo.EMAIL_SENDER SET SEND_TO ='"+To+"',CC='"+CC+"' WHERE LINE_ID='"+Line_oper+"'",conn_update_email);
					DataTable dt_mail_update = new DataTable();
					dt_mail_update.TableName = "MAIL_UPDATE";
					std_update_email.Fill(dt_mail_update);

				
				dt_mail_update.Columns.Add("information",typeof(string));
				dt_mail_update.Rows.Add("Successfully");

					return dt_mail_update;
		}

		public DataTable UpdateSender_profile_ftp(String URL,String Username, String Password, String Line_oper)
		{

			SqlConnection conn_update_ftp = new SqlConnection();
			conn_update_ftp.ConnectionString = constr.edidbconnection;
			SqlDataAdapter sda_update_ftp = new SqlDataAdapter("UPDATE LCIT_EDI.DBO.TEST_FTP_CUSTOMER SET URL='"+URL+"',USER_FTP='"+Username+"',PASS_FTP='"+Password+"' WHERE OPER_CODE='"+Line_oper+"'",conn_update_ftp);

			DataTable dt_ftp_update = new DataTable();
			sda_update_ftp.Fill(dt_ftp_update);
			dt_ftp_update.TableName = "FTP_UPDATE";

			dt_ftp_update.Columns.Add("information",typeof(string));
			dt_ftp_update.Rows.Add("Successfully");

			return dt_ftp_update;
		}



		public string AddingSender_profile_email(String To, String CC, String Line_oper)
		{
					SqlConnection conn_adding_email = new SqlConnection();
					conn_adding_email.ConnectionString = constr.edidbconnection;
			        SqlDataAdapter std_adding_email = new SqlDataAdapter("INSERT INTO LCIT_EDI.dbo.EMAIL_SENDER VALUES ('"+To+"','"+CC+"','"+Line_oper+"')",conn_adding_email);
					DataTable dt_mail_adding = new DataTable();
					dt_mail_adding.TableName = "MAIL_ADDING";
					std_adding_email.Fill(dt_mail_adding);
 			

			return "Add email profile";
		}

		public string AddingSender_profile_FTP(String URL,String Username, String Password, String Line_oper)
		{
			SqlConnection conn_adding_ftp = new SqlConnection();
			conn_adding_ftp.ConnectionString = constr.edidbconnection;
			SqlDataAdapter sda_adding_ftp = new SqlDataAdapter("INSERT INTO LCIT_EDI.DBO.TEST_FTP_CUSTOMER VALUES ('"+URL+"','"+Username+"''"+Password+"','"+Line_oper+"')",conn_adding_ftp);

			DataTable dt_ftp_adding= new DataTable();
			sda_adding_ftp.Fill(dt_ftp_adding);
			dt_ftp_adding.TableName = "FTP_ADDING";


			return "Add FTP profile";
		}
	}
}