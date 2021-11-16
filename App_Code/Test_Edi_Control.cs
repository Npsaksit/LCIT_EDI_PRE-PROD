using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;

 namespace edi_system
 {

 	public class Test_edi_control_system : System.Web.Services.WebService
 	{
 		sqlcon.Connection_String constr = new sqlcon.Connection_String();
 		keepfile.flepath pathfile = new keepfile.flepath();
 		liner_profile.checkprofile LinerPF = new liner_profile.checkprofile();
 		sender_profile.senderprofile SendPF = new sender_profile.senderprofile();
 		OleDbConnection con = new OleDbConnection();
 		char[] cut_string = {'[',']'};
 		string QRY;
 		string QRY2;



 		public Test_edi_control_system()
 		{

 		}



 		[WebMethod]
 		public string TestIVK(String QueryString,String LineOper,String Movement,DateTime CurrentTM, String TerArea)
 		{

 			string edi_status="";
			DataTable dt = new DataTable();
			
				con.ConnectionString =constr.connectionTMS;				 
				con.Open();

				OleDbDataAdapter sda = new OleDbDataAdapter(QueryString,con); 
		        dt.TableName = "EDI-INFO";
		        sda.Fill(dt);

				con.Close();

				if(dt.Rows.Count >0)
			        	{
		                   // CMA clc = new CMA();
		                    // clc.create_file(LineOper,Movement);
		                    Type type = Type.GetType(LineOper);
		                    MethodInfo method = type.GetMethod("create_file");
		                    method.Invoke(LineOper,new object[] {LineOper, Movement, CurrentTM, dt,TerArea });

			        		// DataSet dataSet = new DataSet();
			        		// dataSet.Tables.Add(dt);
			        		// dataSet.WriteXml(@pathfile.SaveXML+LineOper+"\\"+Movement+"\\"+LineOper+Movement+CurrentTM.ToString("yyyyMMddHHmmss")+".XML");
		                    edi_status= "Created EDI file";
			        	}
			        	else {

			        		edi_status= "Empty Data";
			        	}
			        // }

					return edi_status;


		        

 		}

 		public string ivk_method(String QueryString,String LineOper,String Movement,DateTime CurrentTM, String TerArea)
		{
			string edi_status="";
			DataTable dt = new DataTable();
			
				con.ConnectionString =constr.connectionTMS;				 
				con.Open();

				OleDbDataAdapter sda = new OleDbDataAdapter(QueryString,con); 
		        dt.TableName = "EDI-INFO";
		        sda.Fill(dt);

				con.Close();

				//  if(SenderStatus == "N")
			    //     {
			        // CHECK VALID DATA IN DATATABLE
			        	if(dt.Rows.Count >0)
			        	{
		                   // CMA clc = new CMA();
		                    // clc.create_file(LineOper,Movement);
		                    Type type = Type.GetType(LineOper);
		                    MethodInfo method = type.GetMethod("create_file");
		                    edi_status = (string) method.Invoke(LineOper,new object[] {LineOper, Movement, CurrentTM, dt,TerArea });

			        		// DataSet dataSet = new DataSet();
			        		// dataSet.Tables.Add(dt);
			        		// dataSet.WriteXml(@pathfile.SaveXML+LineOper+"\\"+Movement+"\\"+LineOper+Movement+CurrentTM.ToString("yyyyMMddHHmmss")+".XML");
		                    // edi_status= "Created EDI file";
			        	}
			        	else {

			        		// edi_status= "Empty Data";
			        	}
			        // }

					return edi_status;
		} 

		[WebMethod]
		public DataTable fromMaingate()
		{

		 	SqlConnection conn = new SqlConnection();
		    	conn.ConnectionString = constr.CheckTruckCus;
		    	conn.Open();

		    	SqlDataAdapter sda_checklogin = new SqlDataAdapter("SELECT ContainerNo, SUBSTRING(CONVERT(VARCHAR(20),CreateDate,108),1,2) HR FROM V_ReportExportRelease WHERE (CreateDate BETWEEN '2021-MAR-19 08:00' AND '2021-MAR-20 07:59') AND (LCIT_INCOME IS NULL OR LCIT_INCOME = '')",conn);

		     	DataTable dt_checkAuthen = new DataTable();
		    	dt_checkAuthen.TableName= "CheckTruck";
		    	sda_checklogin.Fill(dt_checkAuthen);

				DataTable dt = new DataTable();
				con.ConnectionString =constr.connectionString;				 
				con.Open();

				// OleDbDataAdapter sda = new OleDbDataAdapter("SELECT CNTR_NUM, GATE_IN_LANE, GATE_OUT_LANE, TRUCK_LICENCE_NUMBER, CNTR_BOOKING_NUMBER, TO_CHAR(YARD_GROUND_TM, 'YYYY-MM-DD HH24:MI') YARD_GROUND_TM , TO_CHAR(YARD_PICK_TM, 'YYYY-MM-DD HH24:MI') YARD_PICK_TM, TO_CHAR(CNTR_ARRIVE_TM, 'YYYY-MM-DD HH24:MI') CNTR_ARRIVE_TM, TO_CHAR(CNTR_DEPART_TM, 'YYYY-MM-DD HH24:MI') CNTR_DEPART_TM , CNTR_ARRIVE_MODE, TO_CHAR(TRACTOR_ARRIVE_DATE, 'YYYY-MM-DD HH24:MI') TRACTOR_ARRIVE_DATE, TO_CHAR(TRACTOR_DEPART_DATE, 'YYYY-MM-DD HH24:MI') TRACTOR_DEPART_DATE FROM TMV_CNTR_GATE_INOUT WHERE TRACTOR_ARRIVE_DATE BETWEEN TO_DATE('2021-MAR-19 08:00', 'YYYY-MM-DD HH24:MI') AND TO_DATE('2021-MAR-20 07:59', 'YYYY-MM-DD HH24:MI') AND CNTR_ENTRY_MODE = 'Road'",con);


				OleDbDataAdapter sda = new OleDbDataAdapter("SELECT TCGI.CNTR_NUM , TO_CHAR(TCGI.TRACTOR_ARRIVE_DATE,'HH24') HR FROM MIS_OWNER.TMV_CNTR_GATE_INOUT TCGI WHERE TCGI.TRACTOR_ARRIVE_DATE BETWEEN TO_DATE('2021-MAR-19 08:00', 'YYYY-MM-DD HH24:MI') AND TO_DATE('2021-MAR-20 07:59', 'YYYY-MM-DD HH24:MI') AND TCGI.CNTR_ENTRY_MODE = 'Road'GROUP BY CNTR_NUM,TO_CHAR(TCGI.TRACTOR_ARRIVE_DATE,'HH24') ORDER BY TO_CHAR(TCGI.TRACTOR_ARRIVE_DATE,'HH24') ASC",con); 



	     	dt.TableName = "GateArrive";
	     	sda.Fill(dt);

				con.Close();

				int queue =0;
				int Arrival = 0;

				string cntrNo ="";
				string cntrNoWait ="";

				queue = dt_checkAuthen.Rows.Count;

				DataTable dataTruck = new DataTable();
				dataTruck.TableName="Truck_Queue";

				dataTruck.Columns.Add("CONTAINER");
				dataTruck.Columns.Add("HOUR");
				dataTruck.Columns.Add("All");


				

				foreach (DataRow rowMasterItems in dt.Rows)
        {
            int lintSumOfItems = 0;
            foreach (DataRow rowItems in dt_checkAuthen.Rows)
            {
                if ((rowMasterItems["CNTR_NUM"].ToString().Equals(rowItems["ContainerNo"].ToString())) && (rowMasterItems["HR"].ToString().Equals(rowItems["HR"].ToString())))
                {
                    Arrival = Arrival+1;
                    cntrNo = cntrNo + " "+ rowMasterItems["CNTR_NUM"].ToString();

                    dataTruck.Rows.Add();
                    dataTruck.Rows[Arrival-1]["CONTAINER"] = rowMasterItems["CNTR_NUM"].ToString();
                    dataTruck.Rows[Arrival-1]["HOUR"] = rowMasterItems["HR"].ToString();
                    dataTruck.Rows[Arrival-1]["All"] = queue.ToString();
                    
                }
                else
                {

                	
                }
               
            }
            
        }


			// int Arrival =0;
			// bool anyFieldContainsPepsi;

			 // for (int countTruck =0 ; countTruck>=dt_checkAuthen.Rows.Count;countTruck++)
			 // {

			

			// DataColumn[] columns = dt.Columns.Cast<DataColumn>().ToArray();
			// anyFieldContainsPepsi = dt.AsEnumerable().Any(row => columns.Any(col => row[col].ToString() == dt_checkAuthen.Rows[156]["CNTR_NUM"].ToString()));
		   //   anyFieldContainsPepsi = dt.AsEnumerable().Any(row => columns.Any(col => row[col].ToString() == "EGHU9254570"));
						
					// if(anyFieldContainsPepsi == true)
					// {
					// 		Arrival = Arrival+1;
					// }

					 
			// }

			  
				 // return "Arrival =" + Arrival.ToString() + cntrNo + " " +" Queue"+ (queue-Arrival).ToString() + " ALL " + queue;
				  return dataTruck;


		}



 		[WebMethod]
 		public DataTable sender_profile_check()
 		{
 			DataTable dt_sender_profile = new DataTable();
 			dt_sender_profile = SendPF.getSender_profile_email();

 			return dt_sender_profile;
 		}

		[WebMethod]
		public DataTable Timecheck()
		{
			DataTable dtTime = new DataTable();

			SqlConnection conn = new SqlConnection();
			    	conn.ConnectionString = constr.edidbconnection;
			    	conn.Open();
			SqlDataAdapter sda_TimeSending = new SqlDataAdapter("SELECT * FROM LCIT_EDI.DBO.EDI_TIMESENDING",conn);
			dtTime.TableName = "TimeSend";
			sda_TimeSending.Fill(dtTime);


			return dtTime;
			


		}

 		[WebMethod]
 		public DataTable Update_email_profile(String To, String CC, String Line_oper)
 		{

 			if(To.ToString() !="")
 			{
 					To = To.TrimEnd(',').Replace(",",">,<");
 					To = "<"+To+">";
 			} 
 		
 			if(CC.ToString() !="")
 			{
		 			CC = CC.Replace(",",">,<");
		 			CC = "<"+CC+">";
 			}
 			
 			DataTable dt_update_sender_profile = new DataTable();
 			dt_update_sender_profile = SendPF.UpdateSender_profile_email(To,CC,Line_oper);

 			return dt_update_sender_profile;

 		}

 		[WebMethod]
 		public string Adding_email_profile(String To, String CC, String Line_oper)
 		{
			

			if(To.ToString() !="")
 			{
 					To = To.TrimEnd(',').Replace(",",">,<");
 					To = "<"+To+">";
 			} 
 		
 			if(CC.ToString() !="")
 			{
		 			CC = CC.Replace(",",">,<");
		 			CC = "<"+CC+">";
 			}

 		 SendPF.AddingSender_profile_email(To,CC,Line_oper);

 			return "Completed Adding Email";

 		}

 		[WebMethod]
 		public DataTable Update_ftp_profile(String URL, String Username, String Password, String Line_oper)
 		{
 			DataTable dt_update_ftp_sender_profile = new DataTable();
 			dt_update_ftp_sender_profile = SendPF.UpdateSender_profile_ftp(URL,Username,Password,Line_oper);

 			return dt_update_ftp_sender_profile;
 		}

 		[WebMethod]
 		public string Adding_ftp_profile(String URL, String Username, String Password, String Line_oper)
 		{
 			
 			SendPF.AddingSender_profile_FTP(URL,Username,Password,Line_oper);

 		    return "Completed Adding Email";

 		}

 		[WebMethod]
 		public DataTable ftp_sender_profile_check()
 		{
 			DataTable dt_sender_profile = new DataTable();
 			dt_sender_profile = SendPF.getSender_profile_ftp();

 			return dt_sender_profile;
 		}

 		[WebMethod]
 		public DataTable liner_profile_for_check()
 		{
 			 DataTable dt_profile_check = new DataTable();
 			 dt_profile_check = LinerPF.getLiner_profile_operator_code();

 			 return dt_profile_check;
 		}

// Basic container search
 		[WebMethod]
 		public DataTable Search_edi_info(String container_no)
 		{
 			container_no = container_no.Trim(cut_string).Replace("\"","'").Replace("-","");
 					
            con.ConnectionString = constr.connectionTMS;
            con.Open();
            OleDbCommand comd = new OleDbCommand("SELECT to_char(ECM.ACTIVITY_TM,'DD-MON-YYYY HH24:MI') AS  ACTIVITY_TM,ECM.CNTR_AN,ECM.CNTR_OPERATOR_CODE,ECM.MOVE_TYPE_AN,ECM.LADEN_INDICATOR_AN ,ECM.TRGT_VVD_N,ECM.VESSEL_NM_AN,ECM.SHIPPING_STATUS_CODE,CASE WHEN ECM.SHIPPING_STATUS_CODE='BI' OR ECM.SHIPPING_STATUS_CODE = 'BO' THEN (SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN) ELSE ECM.BOOKING_NO_AN END AS BOOKING_NO_AN ,ECM.AREA_C,to_char(ECM.CREATE_TM,'DD-MON-YYYY HH24:MI') AS  CREATE_TM,ECM.ID,ECM.MASTER_BOL_AN FROM TMS_OWNER.EDI_CNTR_MOVE ECM WHERE ECM.CNTR_AN IN ("+container_no.ToString()+")  ORDER BY ECM.ACTIVITY_TM DESC", con);
            OleDbDataAdapter sda = new OleDbDataAdapter(comd);

 			DataTable dt_information = new DataTable();
 			dt_information.TableName = "EDI_INFO";
 			sda.Fill(dt_information);
 			con.Close();

 			return dt_information;
 		}
// Advance container search
 		[WebMethod]
 		public DataTable Search_edi_info_byDateTime(String QueryScript,String container_no)
 		{
 			string qy = "SELECT to_char(ECM.ACTIVITY_TM,'DD-MON-YYYY HH24:MI') AS  ACTIVITY_TM,ECM.CNTR_AN,ECM.CNTR_OPERATOR_CODE,ECM.MOVE_TYPE_AN,ECM.LADEN_INDICATOR_AN ,ECM.TRGT_VVD_N,ECM.VESSEL_NM_AN,ECM.SHIPPING_STATUS_CODE,CASE WHEN ECM.SHIPPING_STATUS_CODE='BI' OR ECM.SHIPPING_STATUS_CODE = 'BO' THEN (SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN) ELSE ECM.BOOKING_NO_AN END AS BOOKING_NO_AN ,ECM.AREA_C,to_char(ECM.CREATE_TM,'DD-MON-YYYY HH24:MI') AS  CREATE_TM,ECM.ID,ECM.MASTER_BOL_AN FROM TMS_OWNER.EDI_CNTR_MOVE ECM WHERE ";

 			string CNTR ="";
 			if(container_no.ToString() == "no")
 			{

 			}
 			else
 			{
        container_no = container_no.Trim(cut_string).Replace("\"","'").Replace("-","");
 				CNTR = " AND ECM.CNTR_AN IN ("+container_no+")";
 			}
		            con.ConnectionString = constr.connectionTMS;
		            con.Open();
		            OleDbCommand comd = new OleDbCommand(qy+QueryScript+CNTR+" ORDER BY ECM.ACTIVITY_TM DESC", con);
		            OleDbDataAdapter sda = new OleDbDataAdapter(comd);
		 			DataTable dt_information = new DataTable();
		 			dt_information.TableName = "EDI_INFO";
		 			sda.Fill(dt_information);
		 			con.Close();
 			return dt_information;
 		}
// Aythendication EDI system
		[WebMethod]
		    public DataTable tb_Userauthen(string UserN, string PassW){

			    	SqlConnection conn = new SqlConnection();
			    	conn.ConnectionString = constr.edidbconnection;
			    	conn.Open();
			    	SqlDataAdapter sda_checklogin = new SqlDataAdapter("SELECT COUNT(*) AS CHECKTB FROM LCIT_EDI.DBO.TB_USERLOGIN WHERE Username='"+UserN+"' and Password='"+PassW+"'",conn);

			    	DataTable dt_checkAuthen = new DataTable();
			    	dt_checkAuthen.TableName= "CheckUser";
			    	sda_checklogin.Fill(dt_checkAuthen);



			    	if(dt_checkAuthen.Rows[0]["CHECKTB"].ToString() == "1")
			    	{
			    		dt_checkAuthen.Columns.Clear();
			    		SqlDataAdapter sda_checklogin_getid = new SqlDataAdapter("SELECT Emp_id AS CHECKTB,Username,Name,LName FROM LCIT_EDI.DBO.TB_USERLOGIN WHERE Username='"+UserN+"' and Password='"+PassW+"'",conn);
			    		sda_checklogin_getid.Fill(dt_checkAuthen);
			    	}
		    	
		    		conn.Close();
		     return dt_checkAuthen;
		 }
//Query data before create EDI		 

    [WebMethod]
    public string sql( String LineOper, String Movement,String profile_query)
    {
    	
    	QRY = pathfile.QuerySQL+LineOper+"\\";
    	QRY2 = pathfile.QuerySQL_Resend+LineOper+"\\";

    	string QueryString ="";

    	if(profile_query == "P") // production query
    	{

			string[] linest =  File.ReadAllLines(QRY+"\\"+LineOper+Movement+".SQL", Encoding.UTF8);
	
			for(int i=0 ; i< linest.Count();i++)
			{
				QueryString = QueryString + linest[i].ToString()+"\r\n";
			}
		}
		if(profile_query == "R") // production query
    	{
    		string[] linest =  File.ReadAllLines(QRY2+"\\"+LineOper+Movement+".SQL", Encoding.UTF8);
	
			for(int i=0 ; i< linest.Count();i++)
			{
				QueryString = QueryString + linest[i].ToString()+"\r\n";
			}
    	}

    	//return QueryString;
    	 return QueryString.Replace(":START_DATE",DateTime.Now.AddMinutes(-15).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",DateTime.Now.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'%'");
    }

    [WebMethod]
    public string csharp( String LineOper)
    {
    	string dtr= pathfile.GenerateFile.ToString()+LineOper+".cs";
    	string[] csharp_read = File.ReadAllLines(dtr,Encoding.UTF8);

    	string csharp_string = "";

    	for(int i=0 ; i< csharp_read.Count();i++)
			{
				csharp_string = csharp_string + csharp_read[i].ToString()+"\r\n";
			}

    	// dtr = pathfile.GenerateFile+Line_oper+
    	 // string[] r_csharp =
    	return csharp_string;

    }

// Update Customer Profile 

    	[WebMethod]
 		public DataTable Update_CustomerProfile(String CusPfrofile, String SenderB5,String SenderC3,String Receriver,String SenderTP)
 		{

 					SqlConnection conn = new SqlConnection();
			    	conn.ConnectionString = constr.edidbconnection;
			    	conn.Open();
			    	SqlDataAdapter sda_UpdateCusPF = new SqlDataAdapter("UPDATE LCIT_EDI.DBO.LINER_PROFILE SET SENDERID_B5='"+SenderB5+"',SENDERID_C3='"+SenderC3+"',RECEIVERID='"+Receriver+"',SENDER_TYPE='"+SenderTP+"' WHERE LINE_ID='"+CusPfrofile+"'",conn);

			    	DataTable dt_UpdateCusPF = new DataTable();
			    	dt_UpdateCusPF.TableName= "UpdateCusPf";
			    	sda_UpdateCusPF.Fill(dt_UpdateCusPF);

			    	return dt_UpdateCusPF;

 		}

    
    [WebMethod]
    public string Modify_SQL_Query( String LineOper, String Movement, String Querydata, String profile_query )
    {
    	string QRY_modify = "";
    	DateTime CurrentTM = DateTime.Now;

    	if(profile_query == "P") // production query
    	{
    		QRY_modify = pathfile.QuerySQL+LineOper+"\\"+LineOper+Movement+".sql";

    		using(var production_query = new StreamWriter(QRY_modify, false))
				 {
				 	  production_query.WriteLine(Querydata);
				 	  //production_query.WriteLine(Querydata.Replace(",  ",","+Environment.NewLine+" ").Replace("  FROM",Environment.NewLine+"  FROM"));
				 }
    	}

    	if(profile_query == "R")
    	{
    		QRY_modify = pathfile.QuerySQL_Resend+LineOper+"\\"+LineOper+Movement+".sql";

    		using(var production_query_resend = new StreamWriter(QRY_modify, false))
				 {
				 	  production_query_resend.WriteLine(Querydata);
				 }
    	}

    	return "Successfully";
    	//return QueryString.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'%'");
    	//return QueryString.Replace(":START_DATE",DateTime.Now.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",DateTime.Now.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'%'");
    }

     [WebMethod]
    public DataTable TestQuery(String TestingQuery)
    {
    	string query = TestingQuery.Replace(":START_DATE",DateTime.Now.AddMinutes(-1024).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",DateTime.Now.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'%'");
        con.ConnectionString = constr.connectionTMS;
            con.Open();
            OleDbCommand comd = new OleDbCommand(query.ToString(), con);
            OleDbDataAdapter sda = new OleDbDataAdapter(comd);
    	DataTable dt = new DataTable();
    	dt.TableName = "TestQuery";
    	sda.Fill(dt);

    	return dt;
    }

		[WebMethod]
		public string QueryString(DateTime CurrentTM,String LineOper,String Movement,int diftime, String SenderStatus,String TerArea)
		{

				// DateTime CurrentTM = DateTime.Now;
// CHECK PATH QUERY STRING FROM OPERATOR CODE 
				QRY =  pathfile.QuerySQL.ToString()+LineOper+"\\";

				
				string QueryString ="";
				string QueryString2 ="";
				string edi_status="";

				
				

				// READ QUERY STRING FROM SQL FILE
				string[] linest =  File.ReadAllLines(QRY+"\\"+LineOper+Movement+".SQL", Encoding.UTF8);

				foreach(string fi in linest)
				 {
				 	QueryString = QueryString+fi;
				 }

				 // This element use for grup by vessel visit
				if(Movement == "LD" || Movement == "DG")
				{
					string query_checkVessel = "SELECT TRGT_VVD_N FROM TMS_OWNER.EDI_CNTR_MOVE ECM WHERE ECM.CREATE_TM BETWEEN to_date('"+CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")+"', 'DD-MON-YYYY HH24:MI') AND to_date('"+CurrentTM.ToString("dd-MMM-yyyy HH:mm")+"', 'DD-MON-YYYY HH24:MI') AND ECM.CNTR_OPERATOR_CODE = '"+LineOper+"' AND ECM.MOVE_TYPE_AN ='"+Movement.ToString()+"' GROUP BY TRGT_VVD_N";
				
					  if(LineOper == "TSL" || LineOper == "ZIM")
					  {
						  con.ConnectionString = constr.connectionTMS;
						  con.Open();

							OleDbDataAdapter checkVSL = new OleDbDataAdapter(query_checkVessel,con);
							DataTable dtCheckVSL = new DataTable();
							checkVSL.Fill(dtCheckVSL);
						 con.Close();
							if(dtCheckVSL.Rows.Count > 1)
							{
								

								for(int i = 0; i<dtCheckVSL.Rows.Count;i++)
								{
									try
									{
										QueryString2 = "";
										QueryString2 = QueryString;
										QueryString2 = QueryString2.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+TerArea+"'").Replace(":VSL","'"+dtCheckVSL.Rows[i][0].ToString()+"'");

										edi_status = ivk_method(QueryString2,LineOper,Movement,CurrentTM,TerArea);
									}
									catch (System.Exception)
									{
											QueryString = QueryString.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+TerArea+"'").Replace(":VSL","'%'");
											edi_status = ivk_method(QueryString,LineOper,Movement,CurrentTM,TerArea);
										throw;
									}
									
								}
								
							}
							else
							{

									QueryString = QueryString.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+TerArea+"'").Replace(":VSL","'%'");
									edi_status = ivk_method(QueryString,LineOper,Movement,CurrentTM,TerArea);
							}

						 

					  }
					  else
						{
							QueryString = QueryString.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+TerArea+"'");
							edi_status = ivk_method(QueryString,LineOper,Movement,CurrentTM,TerArea);
						}
				}
				else
				{
									QueryString = QueryString.Replace(":START_DATE",CurrentTM.AddMinutes(diftime).ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",CurrentTM.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+TerArea+"'");
								 	edi_status = ivk_method(QueryString,LineOper,Movement,CurrentTM,TerArea);
				}

			return edi_status;
		}
// CHECK BEFORE RESUBMIT EDI
		[WebMethod]
		public DataTable QueryEDI(String EdiQuery, String EdiContainer,String SSType)
		{

			String Query_to_edi="";
			if(SSType.ToString() == "ST")
			{
				 Query_to_edi = "SELECT TO_CHAR(TTCL.CNTR_ARRIVE_TM,'YYYYMMDDHH24MI') AS ACTIVITY_TM,TTCL.CNTR_NUM AS CNTR_AN,TTCL.CNTR_OPER_CODE AS CNTR_OPERATOR_CODE, 'ST' AS MOVE_TYPE_AN,TTCL.CNTR_LADEN_STATUS  AS LADEN_INDICATOR_AN, '' AS SEAL,'' AS VESSEL_NM_AN,TTCL.SHIPPING_STATUS_CODE,nvl(decode(TTCL.CNTR_LADEN_STATUS,'E',(SELECT T.BOOKING_AN FROM TMS_CNTR_GRP1 C, TMS_TRML_DOC_REF T WHERE C.terminal_doc_ref_an= t.terminal_doc_ref_an(+) and c.CNTR_AN=TTCL.CNTR_NUM)),'NOBOOKING')AS BOOKING_NO_AN,(SELECT C.AREA_C FROM TMS_CNTR_GRP1 C WHERE c.CNTR_AN=TTCL.CNTR_NUM) AS AREA_C FROM TMS_OWNER.TMV_CNTR_CYCLE_LIST TTCL WHERE ";
			}
			else
			{
				 Query_to_edi = "SELECT to_char(ECM.ACTIVITY_TM,'DD-MON-YYYY HH24:MI') AS  ACTIVITY_TM,ECM.CNTR_AN,ECM.CNTR_OPERATOR_CODE,ECM.MOVE_TYPE_AN,ECM.LADEN_INDICATOR_AN ,(SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID) AS SEAL,ECM.TRGT_VVD_N,ECM.VESSEL_NM_AN,ECM.SHIPPING_STATUS_CODE,CASE WHEN ECM.SHIPPING_STATUS_CODE='BI' OR ECM.SHIPPING_STATUS_CODE = 'BO' THEN (SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN) ELSE ECM.BOOKING_NO_AN END AS BOOKING_NO_AN ,decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C) AS AREA_C  FROM TMS_OWNER.EDI_CNTR_MOVE ECM WHERE ";
			}
			

			string CNTR ="";
 			if(EdiContainer.ToString() == "no")
 			{

 			}
 			else
 			{
			        EdiContainer = EdiContainer.Trim(cut_string).Replace("\"","'").Replace("-","");

			        if(SSType.ToString() == "ST")
			        {
			 				CNTR = " AND TTCL.CNTR_NUM IN ("+EdiContainer+")"+" ORDER BY TTCL.CNTR_ARRIVE_TM DESC";
			 		}
			 		else
			 		{
			 			CNTR = " AND ECM.CNTR_AN IN ("+EdiContainer+")"+" ORDER BY ECM.ACTIVITY_TM DESC";
			 		}
 			}
		            con.ConnectionString = constr.connectionTMS;
		            con.Open();


		            OleDbCommand comd = new OleDbCommand(Query_to_edi+EdiQuery+CNTR, con);
		            OleDbDataAdapter sda = new OleDbDataAdapter(comd);
		 			DataTable dt_information = new DataTable();
		 			dt_information.TableName = "EDI_INFO_PRESUBMIT";
		 			sda.Fill(dt_information);
		 			con.Close();
 			return dt_information;
 		}
// RESUBMIT EDI FILE
 		[WebMethod]
 		public string Submit_edi(String edi_line, String edi_move, String edi_area, DateTime edi_start_date, DateTime edi_end_time,String edi_container )
 		{
 			DateTime CurrentTM = DateTime.Now;
 			QRY =  pathfile.QuerySQL_Resend.ToString()+edi_line+"\\";

 			DataTable dt = new DataTable();
				string QueryString ="";
				string edi_status="";

				if(edi_container.ToString() == "NO")
				{
					string[] linest =  File.ReadAllLines(QRY+"\\"+edi_line+edi_move+".SQL", Encoding.UTF8);

				foreach(string fi in linest)
				 {
				 	QueryString = QueryString+fi;
				 }

				 	QueryString = QueryString.Replace(":START_DATE",edi_start_date.ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",edi_end_time.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+edi_area+"'");

				 	if(edi_move.ToString()=="ST")
				 	{
				 		QueryString = QueryString.Replace("AND TTCL.CNTR_NUM IN (:CNTR)","");
				 	}
				 	else
				 	{
				 		QueryString = QueryString.Replace("AND ECM.CNTR_AN IN (:CNTR)","");
				 	}


				 	con.ConnectionString =constr.connectionTMS;				 
					con.Open();

					OleDbDataAdapter sda = new OleDbDataAdapter(QueryString,con); 
		        	dt.TableName = "EDI-INFO";
		        	sda.Fill(dt);


		        	 Type type = Type.GetType(edi_line);
		             MethodInfo method = type.GetMethod("create_file");
		             method.Invoke(edi_line,new object[] {edi_line, edi_move, CurrentTM, dt,edi_area });

		             edi_status= "Created EDI file";

				}
				else
				{
					string[] linest =  File.ReadAllLines(QRY+"\\"+edi_line+edi_move+".SQL", Encoding.UTF8);
					edi_container = edi_container.Trim(cut_string).Replace("\"","'").Replace("-","");

				foreach(string fi in linest)
				 {
				 	QueryString = QueryString+fi;
				 }

				 	QueryString = QueryString.Replace(":START_DATE",edi_start_date.ToString("dd-MMM-yyyy HH:mm")).Replace(":END_DATE",edi_end_time.ToString("dd-MMM-yyyy HH:mm")).Replace(":BERTH","'"+edi_area+"'");
				 	// QueryString = QueryString.Replace(":CNTR",edi_container);

				 	if(edi_move.ToString()=="ST")
				 	{
				 		QueryString = QueryString.Replace(":CNTR",edi_container);
				 	}
				 	else
				 	{
				 		QueryString = QueryString.Replace(":CNTR",edi_container);
				 	}


				 	con.ConnectionString =constr.connectionTMS;				 
					con.Open();

					OleDbDataAdapter sda = new OleDbDataAdapter(QueryString,con); 
		        	dt.TableName = "EDI-INFO";
		        	sda.Fill(dt);

		        

		        	 Type type = Type.GetType(edi_line);
		             MethodInfo method = type.GetMethod("create_file");
		             method.Invoke(edi_line,new object[] {edi_line, edi_move, CurrentTM, dt,edi_area });

		             edi_status= "Created EDI file";
		        	

		        	
				
				}

				return edi_status;

				// READ QUERY STRING FROM SQL FILE
				
 		}

 		[WebMethod]
        public DataTable create_profile(String Lineoper, String ReceiveID, String SenderIDC3, String SenderIDB5, String Sendtype,String EDI_Fact)
        {
        	
        	
        	DataTable dt_profile = check_profile_db(Lineoper);
        	

			if(dt_profile.Rows.Count >0)
			{
				dt_profile.Columns.Clear();
				dt_profile.Rows.Clear();
				dt_profile.Columns.Add("information",typeof(string));
				dt_profile.Rows.Add("Data Duplicate");
			}
			else
			{
		    SqlConnection connect_profile_add = new SqlConnection();
			    	connect_profile_add.ConnectionString = constr.edidbconnection;
			    	connect_profile_add.Open();
			SqlDataAdapter sda_profile_add = new SqlDataAdapter("INSERT INTO LCIT_EDI.DBO.LINER_PROFILE VALUES('"+Lineoper+"','"+SenderIDB5+"','"+SenderIDC3+"','"+ReceiveID+"','"+Sendtype+"','"+1+"')",connect_profile_add);
			DataTable dt_profile_add = new DataTable();	
			dt_profile_add.TableName = "Profile_created";
			sda_profile_add.Fill(dt_profile_add);
			connect_profile_add.Close();

			dt_profile = check_profile_db(Lineoper);

			bool exist = System.IO.Directory.Exists(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\");

			if(!exist)
			{
				if(EDI_Fact.ToString() =="CODECO")
				{

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\C3\\backup");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\C3\\backup");

				}
				else if(EDI_Fact.ToString() =="CODECO+COARRI")
				{

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\IN\\C3\\backup");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\OT\\C3\\backup");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\LD\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\LD\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\LD\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\LD\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\LD\\C3\\backup");

					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\DG\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\DG\\B5\\");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\DG\\B5\\backup");
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\DG\\C3\\");		
					System.IO.Directory.CreateDirectory(pathfile.SaveEDI.ToString()+Lineoper.ToString()+"\\DG\\C3\\backup");

				}		

			}
// -- END CREATE FOLDER LINER IN FLEEDI FOLDER --		
// -- CHECK EDI SEND EDI FROM FTP OR NOT --
			if(Sendtype == "FTP")
			{
				System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\");
				System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\IO\\");
				System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\IO\\backup\\");

				// System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\");
				// System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\LD\\");
				// System.IO.Directory.CreateDirectory(pathfile.FTP.ToString()+Lineoper.ToString()+"\\LD\\backup\\");
			}

// -- END CHECK EDI SEND EDI FROM FTP OR NOT --
// -- CREATE FOLDER QUERY SCRIPT --

			bool QueryExist = System.IO.Directory.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\");

			if(!QueryExist)
			{
				System.IO.Directory.CreateDirectory(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\");
				System.IO.Directory.CreateDirectory(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\");

			}

// -- END CREATE FOLDER QUERY SCRIPT --	

// CREATE SQL QUERY STANDART FOR CUSTOMER
			if(EDI_Fact.ToString() =="CODECO")
			{
				// QUERY FOR CONTAINER GATE IN

				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql"))
				{
					string GI_path = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql";
					 using(var tw_GI = new StreamWriter(GI_path, true))
				    {
				    	 tw_GI.WriteLine(" (SELECT");
						 tw_GI.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GI.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GI.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GI.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GI.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER, ");
						 tw_GI.WriteLine(" ECM.CNTR_AN,");
						 tw_GI.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GI.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GI.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GI.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GI.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GI.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GI.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GI.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
						 tw_GI.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GI.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GI.WriteLine(" ECM.DST_CODE,");
						 tw_GI.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GI.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GI.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GI.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GI.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
						 tw_GI.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE, ' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GI.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
						 tw_GI.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
						 tw_GI.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GI.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GI.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GI','RI')");
						 tw_GI.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
						 tw_GI.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GI.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
						 tw_GI.WriteLine(" )");
						 tw_GI.WriteLine(" union all");
						 tw_GI.WriteLine(" (");
						 tw_GI.WriteLine(" SELECT"); 
						 tw_GI.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GI.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GI.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GI.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GI.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GI.WriteLine(" ECM.CNTR_AN,");
						 tw_GI.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GI.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GI.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GI.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GI.WriteLine(" nvl((SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN),'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GI.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GI.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GI.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
						 tw_GI.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GI.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GI.WriteLine(" ECM.DST_CODE,");
						 tw_GI.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GI.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GI.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GI.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GI.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
						 tw_GI.WriteLine(" (SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN)AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GI.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
						 tw_GI.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C"); 
						 tw_GI.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GI.WriteLine(" WHERE UPPER(ECM.CNTR_OPERATOR_CODE) IN ('"+Lineoper.ToString()+"')");
						 tw_GI.WriteLine(" AND ECM.MOVE_TYPE_AN = 'DG'");
						 tw_GI.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('BARGE')");
						 tw_GI.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GI.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
						 tw_GI.WriteLine(" )");
				    }
				}	
					// FOR CONTAINER GATE OUT
				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql"))
				{
					string GO_path = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql";
				 	using(var tw_GO = new StreamWriter(GO_path, true))
			    	{
			    		 tw_GO.WriteLine(" (SELECT");
						 tw_GO.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GO.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GO.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GO.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GO.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GO.WriteLine(" ECM.CNTR_AN,");
						 tw_GO.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GO.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GO.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GO.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GO.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GO.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GO.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GO.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
						 tw_GO.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GO.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GO.WriteLine(" ECM.DST_CODE,");
						 tw_GO.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GO.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GO.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GO.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GO.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
						 tw_GO.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_CODE, ' '),'&','AND'),'+','AND')AS INLAND_CARR_CODE,");
						 tw_GO.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GO.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
						 tw_GO.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
						 tw_GO.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
						 tw_GO.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GO.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GO.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GO','RO')");
						 tw_GO.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
						 tw_GO.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')  ");
						 tw_GO.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
						 tw_GO.WriteLine(" )");
						 tw_GO.WriteLine(" union all");
						 tw_GO.WriteLine(" (SELECT");
						 tw_GO.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GO.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GO.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GO.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GO.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GO.WriteLine(" ECM.CNTR_AN,");
						 tw_GO.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GO.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GO.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GO.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GO.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GO.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GO.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GO.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
						 tw_GO.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GO.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GO.WriteLine(" ECM.DST_CODE,");
						 tw_GO.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GO.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GO.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GO.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GO.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
						 tw_GO.WriteLine(" nvl((SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN), ' ')AS INLAND_CARR_CODE,");
						 tw_GO.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GO.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
						 tw_GO.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
						 tw_GO.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
						 tw_GO.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GO.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GO.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
						 tw_GO.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('BARGE')");
						 tw_GO.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GO.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
						 tw_GO.WriteLine(" )");
			    	}
				}
				// FOR RESEND 
					if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql"))
				{
					string GI_path_resend = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql";
						 using(var tw_GI_resend = new StreamWriter(GI_path_resend, true))
					    {
					    	 tw_GI_resend.WriteLine( "(SELECT");
							 tw_GI_resend.WriteLine( "ECM.VESSEL_NM_AN,");
							 tw_GI_resend.WriteLine( "ECM.VOYAGE_AN,");
							 tw_GI_resend.WriteLine( "substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GI_resend.WriteLine( "ECM.CNTR_OPERATOR_CODE,");
							 tw_GI_resend.WriteLine( "substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER, ");
							 tw_GI_resend.WriteLine( "ECM.CNTR_AN,");
							 tw_GI_resend.WriteLine( "ECM.CONTAINER_TYPE_CODE,");
							 tw_GI_resend.WriteLine( "nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GI_resend.WriteLine( "decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GI_resend.WriteLine( "nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GI_resend.WriteLine( "nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GI_resend.WriteLine( "to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GI_resend.WriteLine( "(select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
							 tw_GI_resend.WriteLine( "(select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GI_resend.WriteLine( "(SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GI_resend.WriteLine( "ECM.DST_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GI_resend.WriteLine( "nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GI_resend.WriteLine( "nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GI_resend.WriteLine( "(SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GI_resend.WriteLine( "decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
							 tw_GI_resend.WriteLine( "replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE, ' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_GI_resend.WriteLine( "nvl(ECM.AREA_C,'B5') AS AREA_C");
							 tw_GI_resend.WriteLine( "FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GI_resend.WriteLine( "WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GI_resend.WriteLine( "AND ECM.MOVE_TYPE_AN IN ('GI','RI')");
							 tw_GI_resend.WriteLine( "AND ECM.IN_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
							 tw_GI_resend.WriteLine( "AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GI_resend.WriteLine( "AND ECM.AREA_C LIKE :BERTH");
							 tw_GI_resend.WriteLine( "AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GI_resend.WriteLine( ")");
							 tw_GI_resend.WriteLine( "union all");
							 tw_GI_resend.WriteLine( "(");
							 tw_GI_resend.WriteLine( "SELECT"); 
							 tw_GI_resend.WriteLine( "ECM.VESSEL_NM_AN,");
							 tw_GI_resend.WriteLine( "ECM.VOYAGE_AN,");
							 tw_GI_resend.WriteLine( "substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GI_resend.WriteLine( "ECM.CNTR_OPERATOR_CODE,");
							 tw_GI_resend.WriteLine( "substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GI_resend.WriteLine( "ECM.CNTR_AN,");
							 tw_GI_resend.WriteLine( "ECM.CONTAINER_TYPE_CODE,");
							 tw_GI_resend.WriteLine( "nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GI_resend.WriteLine( "decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GI_resend.WriteLine( "nvl((SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN),'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GI_resend.WriteLine( "nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GI_resend.WriteLine( "to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GI_resend.WriteLine( "(select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
							 tw_GI_resend.WriteLine( "(select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GI_resend.WriteLine( "(SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GI_resend.WriteLine( "ECM.DST_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GI_resend.WriteLine( "nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GI_resend.WriteLine( "nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GI_resend.WriteLine( "(SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GI_resend.WriteLine( "decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
							 tw_GI_resend.WriteLine( "(SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN)AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GI_resend.WriteLine( "decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_GI_resend.WriteLine( "nvl(ECM.AREA_C,'B5') AS AREA_C"); 
							 tw_GI_resend.WriteLine( "FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GI_resend.WriteLine( "WHERE UPPER(ECM.CNTR_OPERATOR_CODE) IN ('"+Lineoper.ToString()+"')");
							 tw_GI_resend.WriteLine( "AND ECM.MOVE_TYPE_AN = 'DG'");
							 tw_GI_resend.WriteLine( "AND ECM.IN_TRANSPORT_MODE_CODE IN ('BARGE')");
							 tw_GI_resend.WriteLine( "AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GI_resend.WriteLine( "AND ECM.AREA_C LIKE :BERTH");
							 tw_GI_resend.WriteLine( "AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GI_resend.WriteLine( ")");
					    }
				}	
					// FOR CONTAINER GATE OUT
				if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql"))
				{
					string GO_path_resend = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql";
				 	using(var tw_GO_resend = new StreamWriter(GO_path_resend, true))
			    	{
				    		 tw_GO_resend.WriteLine(" (SELECT");
							 tw_GO_resend.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GO_resend.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GO_resend.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GO_resend.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GO_resend.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GO_resend.WriteLine(" ECM.CNTR_AN,");
							 tw_GO_resend.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GO_resend.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GO_resend.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GO_resend.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GO_resend.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GO_resend.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GO_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
							 tw_GO_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GO_resend.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GO_resend.WriteLine(" ECM.DST_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GO_resend.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GO_resend.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GO_resend.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GO_resend.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
							 tw_GO_resend.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_CODE, ' '),'&','AND'),'+','AND')AS INLAND_CARR_CODE,");
							 tw_GO_resend.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
							 tw_GO_resend.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
							 tw_GO_resend.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
							 tw_GO_resend.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GO_resend.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GO_resend.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GO','RO')");
							 tw_GO_resend.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
							 tw_GO_resend.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')  ");
							 tw_GO_resend.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
							 tw_GO_resend.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GO_resend.WriteLine(" )");
							 tw_GO_resend.WriteLine(" union all");
							 tw_GO_resend.WriteLine(" (SELECT");
							 tw_GO_resend.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GO_resend.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GO_resend.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GO_resend.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GO_resend.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GO_resend.WriteLine(" ECM.CNTR_AN,");
							 tw_GO_resend.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GO_resend.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GO_resend.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GO_resend.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GO_resend.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GO_resend.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GO_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");		
							 tw_GO_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GO_resend.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GO_resend.WriteLine(" ECM.DST_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GO_resend.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GO_resend.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GO_resend.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GO_resend.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
							 tw_GO_resend.WriteLine(" nvl((SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN), ' ')AS INLAND_CARR_CODE,");
							 tw_GO_resend.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GO_resend.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
							 tw_GO_resend.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
							 tw_GO_resend.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
							 tw_GO_resend.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GO_resend.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GO_resend.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
							 tw_GO_resend.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('BARGE')");
							 tw_GO_resend.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GO_resend.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
							 tw_GO_resend.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GO_resend.WriteLine(" )");
			    	}
				}
			}
			else if(EDI_Fact.ToString() =="CODECO+COARRI")
			{
				// QUERY FOR CONTAINER GATE IN
				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql"))
				{
					string GI_path2 = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql";
					 using(var tw_GI2 = new StreamWriter(GI_path2, true))
				    {
				    	 tw_GI2.WriteLine(" (SELECT");
						 tw_GI2.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GI2.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GI2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GI2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GI2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER, ");
						 tw_GI2.WriteLine(" ECM.CNTR_AN,");
						 tw_GI2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GI2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GI2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GI2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GI2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GI2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GI2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");						 
						 tw_GI2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GI2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GI2.WriteLine(" ECM.DST_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GI2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GI2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GI2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GI2.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
						 tw_GI2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE, ' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
						 tw_GI2.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
						 tw_GI2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GI2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GI2.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GI','RI')");
						 tw_GI2.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
						 tw_GI2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GI2.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
						 tw_GI2.WriteLine(" )");
						 tw_GI2.WriteLine(" union all");
						 tw_GI2.WriteLine(" (");
						 tw_GI2.WriteLine(" SELECT"); 
						 tw_GI2.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GI2.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GI2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GI2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GI2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GI2.WriteLine(" ECM.CNTR_AN,");
						 tw_GI2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GI2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GI2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GI2.WriteLine(" nvl((SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN),'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GI2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GI2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GI2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
						 tw_GI2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GI2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GI2.WriteLine(" ECM.DST_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GI2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GI2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GI2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GI2.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
						 tw_GI2.WriteLine(" (SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN)AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GI2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
						 tw_GI2.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C"); 
						 tw_GI2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GI2.WriteLine(" WHERE UPPER(ECM.CNTR_OPERATOR_CODE) IN ('"+Lineoper.ToString()+"')");
						 tw_GI2.WriteLine(" AND ECM.MOVE_TYPE_AN = 'DG'");
						 tw_GI2.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('BARGE')");
						 tw_GI2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GI2.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
						 tw_GI2.WriteLine(" )");
				    }

				}	
					// FOR CONTAINER GATE OUT
				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql"))
				{
						string GO_path2 = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql";
				 	using(var tw_GO2 = new StreamWriter(GO_path2, true))
			    	{
			    		 tw_GO2.WriteLine(" (SELECT");
						 tw_GO2.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GO2.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GO2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GO2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GO2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GO2.WriteLine(" ECM.CNTR_AN,");
						 tw_GO2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GO2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GO2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GO2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GO2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GO2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GO2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");						 
						 tw_GO2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GO2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GO2.WriteLine(" ECM.DST_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GO2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GO2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GO2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GO2.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
						 tw_GO2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_CODE, ' '),'&','AND'),'+','AND')AS INLAND_CARR_CODE,");
						 tw_GO2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
						 tw_GO2.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
						 tw_GO2.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
						 tw_GO2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GO2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GO2.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GO','RO')");
						 tw_GO2.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
						 tw_GO2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')  ");
						 tw_GO2.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
						 tw_GO2.WriteLine(" )");
						 tw_GO2.WriteLine(" union all");
						 tw_GO2.WriteLine(" (SELECT");
						 tw_GO2.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_GO2.WriteLine(" ECM.VOYAGE_AN,");
						 tw_GO2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_GO2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_GO2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
						 tw_GO2.WriteLine(" ECM.CNTR_AN,");
						 tw_GO2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_GO2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
						 tw_GO2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
						 tw_GO2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
						 tw_GO2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
						 tw_GO2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
						 tw_GO2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");						 
						 tw_GO2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
						 tw_GO2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
						 tw_GO2.WriteLine(" ECM.DST_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_GO2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
						 tw_GO2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_GO2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
						 tw_GO2.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
						 tw_GO2.WriteLine(" nvl((SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN), ' ')AS INLAND_CARR_CODE,");
						 tw_GO2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_GO2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
						 tw_GO2.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
						 tw_GO2.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
						 tw_GO2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
						 tw_GO2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
						 tw_GO2.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
						 tw_GO2.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('BARGE')");
						 tw_GO2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_GO2.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
						 tw_GO2.WriteLine(" )");
			    	}
				}
					// FOR LOAD 
				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"LD"+".sql"))
				{
					string LD_path = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"LD"+".sql";
					using(var tw_LD = new StreamWriter(LD_path, true))
				    {
				    	 tw_LD.WriteLine(" (SELECT");
						 tw_LD.WriteLine(" ECM.VESSEL_NM_AN,");
						 tw_LD.WriteLine(" ECM.VOYAGE_AN,");
						 tw_LD.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
						 tw_LD.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
						 tw_LD.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35) AS SHIPPER,");
						 tw_LD.WriteLine(" ECM.CNTR_AN,");
						 tw_LD.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
						 tw_LD.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,");
						 tw_LD.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ') AS EQP_STATUS_CODE,");
						 tw_LD.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5','L','5',' ') AS LADEN_INDICATOR_AN,");
						 tw_LD.WriteLine(" nvl(ECM.BOOKING_NO_AN,'NOBOOKING') BOOKING_NO_AN,");
						 tw_LD.WriteLine(" nvl(ECM.MASTER_BOL_AN,'NOBL') MASTER_BOL_AN,");
						 tw_LD.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI') AS ACTIVITY_TM,");
						 tw_LD.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE) AS POD,");						 
						 tw_LD.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE) AS POL ,");
						 tw_LD.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ) AS ORG,");
						 tw_LD.WriteLine(" ECM.DST_CODE,");
						 tw_LD.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
						 tw_LD.WriteLine(" NVL(TO_CHAR(ECM.TEMPERATURE), 'NOTEMP') AS TEMPERATURE,");
						 tw_LD.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
						 tw_LD.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID)) AS UNDG,");
						 tw_LD.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ') AS IN_TRANSPORT_MODE_CODE,");
						 tw_LD.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','') AS INLAND_CARR_TP_MEAN_CODE,");
						 tw_LD.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
						 tw_LD.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
						 tw_LD.WriteLine(" FROM EDI_CNTR_MOVE ECM, TMV_VESSEL_VISIT TV");
						 tw_LD.WriteLine(" WHERE  ECM.TRGT_VVD_N = TV.VESSEL_VISIT_C");
						 tw_LD.WriteLine(" AND ECM.CNTR_OPERATOR_CODE LIKE ('"+Lineoper.ToString()+"')");
						 tw_LD.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
						 tw_LD.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RORO','LOLO')");
						 tw_LD.WriteLine(" AND  ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
						 tw_LD.WriteLine(" AND ECM.AREA_C LIKE UPPER(:BERTH)");
						 tw_LD.WriteLine(" AND ECM.EQP_STATUS_CODE NOT IN ('RS')");
						 tw_LD.WriteLine(" )");
				    }
				}
					// DISCHARGE
				if(!File.Exists(pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"DG"+".sql"))
				{
					string DG_path = pathfile.QuerySQL.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"DG"+".sql";
					using(var tw_DG = new StreamWriter(DG_path, true))
					    {
					    	 tw_DG.WriteLine(" (SELECT");
							 tw_DG.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_DG.WriteLine(" ECM.VOYAGE_AN,");
							 tw_DG.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_DG.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_DG.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35) AS SHIPPER,");
							 tw_DG.WriteLine(" ECM.CNTR_AN,");
							 tw_DG.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_DG.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,");
							 tw_DG.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_DG.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5','L','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_DG.WriteLine(" nvl(ECM.BOOKING_NO_AN,'NOBOOKING') BOOKING_NO_AN,");
							 tw_DG.WriteLine(" nvl(ECM.MASTER_BOL_AN,'NOBL') MASTER_BOL_AN,");
							 tw_DG.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI') AS ACTIVITY_TM,");
							 tw_DG.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE) AS POD,");							 
							 tw_DG.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE) AS POL,");
							 tw_DG.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ) AS ORG, ");
							 tw_DG.WriteLine(" ECM.DST_CODE,");
							 tw_DG.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT) AS GWEIGHT,");
							 tw_DG.WriteLine(" NVL(TO_CHAR(ECM.TEMPERATURE), 'NOTEMP') AS TEMPERATURE,");
							 tw_DG.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_DG.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID)) AS UNDG,");
							 tw_DG.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','3') AS OUT_TRANSPORT_MODE_CODE,");
							 tw_DG.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_DG.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_DG.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
							 tw_DG.WriteLine(" FROM EDI_CNTR_MOVE ECM, TMV_VESSEL_VISIT TV");
							 tw_DG.WriteLine(" WHERE  ECM.TRGT_VVD_N = TV.VESSEL_VISIT_C");
							 tw_DG.WriteLine(" AND UPPER(ECM.CNTR_OPERATOR_CODE) LIKE UPPER('"+Lineoper.ToString()+"')");
							 tw_DG.WriteLine(" AND ECM.MOVE_TYPE_AN = 'DG'");
							 tw_DG.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('RORO','LOLO')");
							 tw_DG.WriteLine(" AND  ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_DG.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
							 tw_DG.WriteLine(" AND ECM.EQP_STATUS_CODE NOT IN ('RS')");
						     tw_DG.WriteLine(" )");
				    }
				}
				// FOR RESEND 
					if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql"))
				{
						string GI_path_resend2 = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"IN"+".sql";
						 using(var tw_GI_resend2 = new StreamWriter(GI_path_resend2, true))
					    {
					    	 tw_GI_resend2.WriteLine(" (SELECT");
							 tw_GI_resend2.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GI_resend2.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GI_resend2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GI_resend2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GI_resend2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER, ");
							 tw_GI_resend2.WriteLine(" ECM.CNTR_AN,");
							 tw_GI_resend2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GI_resend2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GI_resend2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GI_resend2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GI_resend2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GI_resend2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GI_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
							 tw_GI_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GI_resend2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GI_resend2.WriteLine(" ECM.DST_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GI_resend2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GI_resend2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GI_resend2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GI_resend2.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
							 tw_GI_resend2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE, ' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_GI_resend2.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
							 tw_GI_resend2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GI_resend2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GI_resend2.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GI','RI')");
							 tw_GI_resend2.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
							 tw_GI_resend2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GI_resend2.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
							 tw_GI_resend2.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GI_resend2.WriteLine(" )");
							 tw_GI_resend2.WriteLine(" union all");
							 tw_GI_resend2.WriteLine(" (");
							 tw_GI_resend2.WriteLine(" SELECT"); 
							 tw_GI_resend2.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GI_resend2.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GI_resend2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GI_resend2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GI_resend2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GI_resend2.WriteLine(" ECM.CNTR_AN,");
							 tw_GI_resend2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GI_resend2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GI_resend2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GI_resend2.WriteLine(" nvl((SELECT TCG.BARGE_BKG FROM TMS_CNTR_GRP1 TCG WHERE TCG.CNTR_AN=ECM.CNTR_AN),'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GI_resend2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GI_resend2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GI_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POL,");
							 tw_GI_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POD,");							 
							 tw_GI_resend2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GI_resend2.WriteLine(" ECM.DST_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GI_resend2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GI_resend2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GI_resend2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GI_resend2.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','BARGE','8','3')AS IN_TRANSPORT_MODE_CODE,");
							 tw_GI_resend2.WriteLine(" (SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN)AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GI_resend2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_GI_resend2.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C"); 
							 tw_GI_resend2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GI_resend2.WriteLine(" WHERE UPPER(ECM.CNTR_OPERATOR_CODE) IN ('"+Lineoper.ToString()+"')");
							 tw_GI_resend2.WriteLine(" AND ECM.MOVE_TYPE_AN = 'DG'");
							 tw_GI_resend2.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('BARGE')");
							 tw_GI_resend2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GI_resend2.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
							 tw_GI_resend2.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GI_resend2.WriteLine(" )");
					    }
				}	

					// FOR CONTAINER GATE OUT
				if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql"))
				{
					string GO_path_resend2 = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"OT"+".sql";
				 	using(var tw_GO_resend2 = new StreamWriter(GO_path_resend2, true))
			    	{
				    		 tw_GO_resend2.WriteLine(" (SELECT");
							 tw_GO_resend2.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GO_resend2.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GO_resend2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GO_resend2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GO_resend2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GO_resend2.WriteLine(" ECM.CNTR_AN,");
							 tw_GO_resend2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GO_resend2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GO_resend2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GO_resend2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GO_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");
							 tw_GO_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");							 
							 tw_GO_resend2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GO_resend2.WriteLine(" ECM.DST_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GO_resend2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GO_resend2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GO_resend2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GO_resend2.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
							 tw_GO_resend2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_CODE, ' '),'&','AND'),'+','AND')AS INLAND_CARR_CODE,");
							 tw_GO_resend2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
							 tw_GO_resend2.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
							 tw_GO_resend2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GO_resend2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GO_resend2.WriteLine(" AND ECM.MOVE_TYPE_AN IN ('GO','RO')");
							 tw_GO_resend2.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RAIL','GATE','LOLO')");
							 tw_GO_resend2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')  ");
							 tw_GO_resend2.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
							 tw_GO_resend2.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GO_resend2.WriteLine(" )");
							 tw_GO_resend2.WriteLine(" union all");
							 tw_GO_resend2.WriteLine(" (SELECT");
							 tw_GO_resend2.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_GO_resend2.WriteLine(" ECM.VOYAGE_AN,");
							 tw_GO_resend2.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,' '),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_GO_resend2.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_GO_resend2.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35)AS SHIPPER,");
							 tw_GO_resend2.WriteLine(" ECM.CNTR_AN,");
							 tw_GO_resend2.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_GO_resend2.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,10),'NOSEAL')AS SEAL,");
							 tw_GO_resend2.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.BOOKING_NO_AN, 'NOBOOKING')AS BOOKING_NO_AN,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.MASTER_BOL_AN, 'NOBL')AS MASTER_BOL_AN,");
							 tw_GO_resend2.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI')AS ACTIVITY_TM,");
							 tw_GO_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE)AS POD,");							 
							 tw_GO_resend2.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE)AS POL,");
							 tw_GO_resend2.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ)AS ORG,");
							 tw_GO_resend2.WriteLine(" ECM.DST_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_GO_resend2.WriteLine(" nvl(to_char(ECM.TEMPERATURE),'NOTEMP')AS TEMPERATURE,");
							 tw_GO_resend2.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_GO_resend2.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID))AS UNDG,");
							 tw_GO_resend2.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ')AS OUT_TRANSPORT_MODE_CODE,");
							 tw_GO_resend2.WriteLine(" nvl((SELECT VSL_C FROM TMV_VESSEL_VISIT WHERE EXT_VOY_C=ECM.VOYAGE_AN AND VISIT_VSL_NAME_AN =ECM.VESSEL_NM_AN), ' ')AS INLAND_CARR_CODE,");
							 tw_GO_resend2.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','AND')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_GO_resend2.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1')AS DAMAGE_SEGMENT_AN,");
							 tw_GO_resend2.WriteLine(" decode(ECM.AREA_C,'DP',(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT IN_VESSEL_VISIT_C FROM TMS_CNTR_GRP1 WHERE CNTR_AN=ECM.CNTR_AN AND ROWNUM=1)),ECM.AREA_C)AS AREA_C,");
							 tw_GO_resend2.WriteLine(" nvl(ECM.VGM_GROSS_WEIGHT,'0')AS VGM_GROSS_WEIGHT");
							 tw_GO_resend2.WriteLine(" FROM TMS_OWNER.EDI_CNTR_MOVE ECM");
							 tw_GO_resend2.WriteLine(" WHERE ECM.CNTR_OPERATOR_CODE IN ('"+Lineoper.ToString()+"')");
							 tw_GO_resend2.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
							 tw_GO_resend2.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('BARGE')");
							 tw_GO_resend2.WriteLine(" AND ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_GO_resend2.WriteLine(" AND (DECODE((SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N),'',DECODE(ECM.SHIPPING_STATUS_CODE,'ST',ECM.AREA_C,(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C =(SELECT CG.PREV_VVD_N FROM TMS_OWNER.TMV_CNTR_GRP1 CG  WHERE CG.CNTR_AN = ECM.CNTR_AN))),(SELECT AREA_C FROM TMV_VESSEL_VISIT WHERE VESSEL_VISIT_C = ECM.TRGT_VVD_N))) LIKE :BERTH");
							 tw_GO_resend2.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_GO_resend2.WriteLine(" )");
			    	}
				}

				// FOR LOAD 
				if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"LD"+".sql"))
				{
					string LD_path_resend = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"LD"+".sql";
					using(var tw_LD_resend = new StreamWriter(LD_path_resend, true))
				    {
					    	 tw_LD_resend.WriteLine(" (SELECT");
							 tw_LD_resend.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_LD_resend.WriteLine(" ECM.VOYAGE_AN,");
							 tw_LD_resend.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_LD_resend.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_LD_resend.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35) AS SHIPPER,");
							 tw_LD_resend.WriteLine(" ECM.CNTR_AN,");
							 tw_LD_resend.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_LD_resend.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,");
							 tw_LD_resend.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ') AS EQP_STATUS_CODE,");
							 tw_LD_resend.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5','L','5',' ') AS LADEN_INDICATOR_AN,");
							 tw_LD_resend.WriteLine(" nvl(ECM.BOOKING_NO_AN,'NOBOOKING') BOOKING_NO_AN,");
							 tw_LD_resend.WriteLine(" nvl(ECM.MASTER_BOL_AN,'NOBL') MASTER_BOL_AN,");
							 tw_LD_resend.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI') AS ACTIVITY_TM,");
							 tw_LD_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE) AS POD,");
							 tw_LD_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE) AS POL ,");							 
							 tw_LD_resend.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ) AS ORG,");
							 tw_LD_resend.WriteLine(" ECM.DST_CODE,");
							 tw_LD_resend.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT)AS GWEIGHT,");
							 tw_LD_resend.WriteLine(" NVL(TO_CHAR(ECM.TEMPERATURE), 'NOTEMP') AS TEMPERATURE,");
							 tw_LD_resend.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_LD_resend.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID)) AS UNDG,");
							 tw_LD_resend.WriteLine(" decode(ECM.IN_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2',' ') AS IN_TRANSPORT_MODE_CODE,");
							 tw_LD_resend.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','') AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_LD_resend.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_LD_resend.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
							 tw_LD_resend.WriteLine(" FROM EDI_CNTR_MOVE ECM, TMV_VESSEL_VISIT TV");
							 tw_LD_resend.WriteLine(" WHERE  ECM.TRGT_VVD_N = TV.VESSEL_VISIT_C");
							 tw_LD_resend.WriteLine(" AND ECM.CNTR_OPERATOR_CODE LIKE ('"+Lineoper.ToString()+"')");
							 tw_LD_resend.WriteLine(" AND ECM.MOVE_TYPE_AN = 'LD'");
							 tw_LD_resend.WriteLine(" AND ECM.OUT_TRANSPORT_MODE_CODE IN ('RORO','LOLO')");
							 tw_LD_resend.WriteLine(" AND  ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_LD_resend.WriteLine(" AND ECM.AREA_C LIKE UPPER(:BERTH)");
							 tw_LD_resend.WriteLine(" AND ECM.EQP_STATUS_CODE NOT IN ('RS')");
							 tw_LD_resend.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_LD_resend.WriteLine(" )");
				    }
				}

					// DISCHARGE
				if(!File.Exists(pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"DG"+".sql"))
				{
					string DG_path_resend = pathfile.QuerySQL_Resend.ToString()+Lineoper.ToString()+"\\"+Lineoper.ToString()+"DG"+".sql";
					using(var tw_DG_resend = new StreamWriter(DG_path_resend, true))
				    {
				    		 tw_DG_resend.WriteLine(" (SELECT");
							 tw_DG_resend.WriteLine(" ECM.VESSEL_NM_AN,");
							 tw_DG_resend.WriteLine(" ECM.VOYAGE_AN,");
							 tw_DG_resend.WriteLine(" substr(replace(nvl(ECM.VISIT_VSL_CALL_SIGN_C,''),'-',''),1,9) AS VISIT_VSL_CALL_SIGN_C,");
							 tw_DG_resend.WriteLine(" ECM.CNTR_OPERATOR_CODE,");
							 tw_DG_resend.WriteLine(" substr(translate(nvl(nvl((SELECT a.SHPR_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN),(SELECT a.CSGN_N from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ AND a.CNTR_AN=ECM.CNTR_AN)), 'NO'), '!@#$%^&*-+|\\/[]{}<>?', '                    '),1,35) AS SHIPPER,");
							 tw_DG_resend.WriteLine(" ECM.CNTR_AN,");
							 tw_DG_resend.WriteLine(" ECM.CONTAINER_TYPE_CODE,");
							 tw_DG_resend.WriteLine(" nvl(substr((SELECT stragg(a.SEAL_NO_AN) from EDI_CNTR_MOVE_SEAL a WHERE a.SEAL_NO_AN IS NOT NULL AND a.CNTR_MOVE_ID=ECM.ID),1,15),'NOSEAL')AS SEAL,");
							 tw_DG_resend.WriteLine(" decode(ECM.EQP_STATUS_CODE,'OB','2','IB','3',' ')AS EQP_STATUS_CODE,");
							 tw_DG_resend.WriteLine(" decode(ECM.LADEN_INDICATOR_AN,'E','4','F','5','L','5',' ')AS LADEN_INDICATOR_AN,");
							 tw_DG_resend.WriteLine(" nvl(ECM.BOOKING_NO_AN,'NOBOOKING') BOOKING_NO_AN,");
							 tw_DG_resend.WriteLine(" nvl(ECM.MASTER_BOL_AN,'NOBL') MASTER_BOL_AN,");
							 tw_DG_resend.WriteLine(" to_char(ECM.ACTIVITY_TM,'YYYYMMDDHH24MI') AS ACTIVITY_TM,");
							 tw_DG_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POD_CODE) AS POD,");
							 tw_DG_resend.WriteLine(" (select tl.CTRY_C || tl.LOC_C from tmt_loc tl where tl.loc_c = ECM.POL_CODE) AS POL,");							 
							 tw_DG_resend.WriteLine(" (SELECT a.ORG_I from TMS_CNTR_HIST a where a.CNTR_SEQ=ECM.CNTR_SEQ) AS ORG, ");
							 tw_DG_resend.WriteLine(" ECM.DST_CODE,");
							 tw_DG_resend.WriteLine(" decode(ECM.GROSS_WEIGHT_UOM,'MT',ECM.GROSS_WEIGHT*1000,ECM.GROSS_WEIGHT) AS GWEIGHT,");
							 tw_DG_resend.WriteLine(" NVL(TO_CHAR(ECM.TEMPERATURE), 'NOTEMP') AS TEMPERATURE,");
							 tw_DG_resend.WriteLine(" nvl((SELECT MAX(a.IMCO_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID),'NODG') AS IMCO,");
							 tw_DG_resend.WriteLine(" (SELECT MAX(a.UNDG_AN) from EDI_CNTR_MOVE_HAZ a where a.CNTR_MOVE_ID=ECM.ID and a.IMCO_AN=(SELECT MAX(b.IMCO_AN) from EDI_CNTR_MOVE_HAZ b where b.CNTR_MOVE_ID=ECM.ID)) AS UNDG,");
							 tw_DG_resend.WriteLine(" decode(ECM.OUT_TRANSPORT_MODE_CODE,'GATE','3','RAIL','2','3') AS OUT_TRANSPORT_MODE_CODE,");
							 tw_DG_resend.WriteLine(" replace(replace(nvl(ECM.INLAND_CARR_TP_MEAN_CODE,' '),'&','AND'),'+','')AS INLAND_CARR_TP_MEAN_CODE,");
							 tw_DG_resend.WriteLine(" decode(ECM.DAMAGE_SEGMENT_AN,null,' ',' ',' ','1') AS DAMAGE_SEGMENT_AN,");
							 tw_DG_resend.WriteLine(" nvl(ECM.AREA_C,'B5') AS AREA_C");
							 tw_DG_resend.WriteLine(" FROM EDI_CNTR_MOVE ECM, TMV_VESSEL_VISIT TV");
							 tw_DG_resend.WriteLine(" WHERE  ECM.TRGT_VVD_N = TV.VESSEL_VISIT_C");
							 tw_DG_resend.WriteLine(" AND UPPER(ECM.CNTR_OPERATOR_CODE) LIKE UPPER('"+Lineoper.ToString()+"')");
							 tw_DG_resend.WriteLine(" AND ECM.MOVE_TYPE_AN = 'DG'");
							 tw_DG_resend.WriteLine(" AND ECM.IN_TRANSPORT_MODE_CODE IN ('RORO','LOLO')");
							 tw_DG_resend.WriteLine(" AND  ECM.CREATE_TM between to_date(':START_DATE','DD-MON-YYYY HH24:MI') AND to_date(':END_DATE','DD-MON-YYYY HH24:MI')");
							 tw_DG_resend.WriteLine(" AND ECM.AREA_C LIKE :BERTH");
							 tw_DG_resend.WriteLine(" AND ECM.EQP_STATUS_CODE NOT IN ('RS')");
							 tw_DG_resend.WriteLine(" AND ECM.CNTR_AN IN (:CNTR)");
							 tw_DG_resend.WriteLine(" )");
				    }
				}

			}
// END OF SQL QUERY SEGMENT		

// -- CREATE CS FILE FORGENERATE EDI FILE --

			if(!File.Exists(pathfile.GenerateFile.ToString()+Lineoper.ToString()+".cs"))
			{
				string path = pathfile.GenerateFile.ToString()+Lineoper.ToString()+".cs";
			

				 using(var tw = new StreamWriter(path, true))
				    {
				        tw.WriteLine("using System;");
				        tw.WriteLine("using System.Linq;");
				        tw.WriteLine("using System.Threading;");
				        tw.WriteLine("using System.Xml.Linq;");
				        tw.WriteLine("using System.IO;");
				        tw.WriteLine("using System.Data;");
				        tw.WriteLine("using System.Configuration;");
				        tw.WriteLine("using System.Text;");
				        tw.WriteLine("using System.Collections.Generic;");


				        tw.WriteLine("	public class"+" "+Lineoper);
				        tw.WriteLine("	{");
				        tw.WriteLine("		static keepfile.flepath pathfile = new keepfile.flepath();");
				        tw.WriteLine("		static customer_mail send_mail = new customer_mail();");
				        tw.WriteLine("		static liner_profile.checkprofile LinerPF = new liner_profile.checkprofile();");
				        tw.WriteLine("");
				        tw.WriteLine("		public static void  create_file(string Line, string Move, DateTime dt, DataTable EdiTB,String TerArea)");
				        tw.WriteLine("		{");

				        tw.WriteLine("			FileStream ostrm;");
				        tw.WriteLine("			StreamWriter writer;");
				        tw.WriteLine("			TextWriter oldOut = Console.Out;");
				        tw.WriteLine("			string  SaveEDIfile =pathfile.SaveEDI.ToString()+Line+"+"\"\\\\\""+"+Move+"+"\"\\\\\""+"+TerArea+"+"\"\\\\\""+";");
				        tw.WriteLine("			string  di = pathfile.MainDirectory.ToString();");
				        tw.WriteLine("			string SenderID = \"\";");
				        tw.WriteLine("			string ReceiveID =\"\";");
				        tw.WriteLine("			string EDIHeader =\"\";");
				        tw.WriteLine("			int countSegment =0;");
				        tw.WriteLine("			string FileName= \"\";");
				        tw.WriteLine("			string SenderType = \"\";");
				        tw.WriteLine("			string[] MyVessel = EdiTB.Rows[0][\"VESSEL_NM_AN\"].ToString().Split(new char[0]);");
				        tw.WriteLine("");
				        tw.WriteLine("		//-------------------------------- CHECK DOCUMENT CUSTOMER PROFILR IN XML -----------------------------");
				        tw.WriteLine("");
				        tw.WriteLine("			DataTable dt_profile = new DataTable();");
				        tw.WriteLine("			dt_profile = LinerPF.getLiner_profile(Line);");
				        tw.WriteLine("			FileName = String.Format(\"{0:D10}\",Int32.Parse(dt_profile.Rows[0][\"RUNNING_NUMBER\"].ToString()));");
				        tw.WriteLine("");
				        tw.WriteLine("			if(TerArea.ToString() == \"B5\")");
				        tw.WriteLine("			 {");
				        tw.WriteLine("");
				        tw.WriteLine("			 SenderID = dt_profile.Rows[0][\"SENDERID_B5\"].ToString();");
				        tw.WriteLine("");
				        tw.WriteLine("			 }");
				        tw.WriteLine("			if(TerArea.ToString() == \"C3\")");
				        tw.WriteLine("			 {");
				        tw.WriteLine("");
				        tw.WriteLine("			 SenderID = dt_profile.Rows[0][\"SENDERID_C3\"].ToString();");
				        tw.WriteLine("");
				        tw.WriteLine("			 }");
				        tw.WriteLine("");

				        tw.WriteLine("			 ReceiveID = dt_profile.Rows[0][\"RECEIVERID\"].ToString();");
				        tw.WriteLine("			 SenderType = dt_profile.Rows[0][\"SENDER_TYPE\"].ToString();");
				        

				     if(EDI_Fact.ToString() =="CODECO")
				      	{
				      	tw.WriteLine("");	
				        tw.WriteLine("			switch(Move)");
				        tw.WriteLine("			{");

				        tw.WriteLine("			case \"IN\":");
				        tw.WriteLine("");
				        tw.WriteLine("//-----------------------------------CREATE HEADER EDI FILE -----------------------------------------------");
				        tw.WriteLine("//-------------- For Header Table Query data from : EdiTB.Rows[0][\"COLUMNS_NAME\"].ToString()-------------");

				        tw.WriteLine("				EDIHeader=	\"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"DI++CODECO'\\r\\n\"+");
				        tw.WriteLine("							\"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+CODECO:D:95B:UN'\\r\\n\"+");
				        tw.WriteLine("							\"BGM+34+CONTAINER GATE IN/OUT+9'\\r\\n\"+");
				        tw.WriteLine("							\"TDT+20+\"+EdiTB.Rows[0][\"VOYAGE_AN\"].ToString()+\"+1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+15+LCB05'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+9+THLCH:139:6'\\r\\n\"+");
				        tw.WriteLine("							\"NAD+MS+LCIT'\\r\\n\";");
				        tw.WriteLine("");
				        tw.WriteLine("							countSegment = countSegment+7;");
				        
				        tw.WriteLine("");

				        tw.WriteLine("// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------");
				        tw.WriteLine("// -----------------For Content Query Data form EdiTB.Rows[checkdtb][\"COLUMNS_NAME\"]  -----------------------------");
				        tw.WriteLine("");
				        tw.WriteLine("							ostrm = new FileStream (SaveEDIfile.ToString()+\"COD\"+Line+\"IN\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);");
				        tw.WriteLine("							writer = new StreamWriter (ostrm);");
				        tw.WriteLine("							Console.SetOut (writer);");
				        tw.WriteLine("							Console.Write(EDIHeader);");

				        tw.WriteLine("							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)");
				        tw.WriteLine("							{");
				        tw.WriteLine("								Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								Console.WriteLine(\"DTM+7:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								Console.WriteLine(\"MEA+AAE+VGM+KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("									Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"+CA'\");");
				        tw.WriteLine("									countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()!=\" \")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"DAM+\"+EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");
				        tw.WriteLine("								Console.WriteLine(\"TDT+1++\"+EdiTB.Rows[checkdtb][\"IN_TRANSPORT_MODE_CODE\"].ToString()+\"+++++:::\"+EdiTB.Rows[checkdtb][\"INLAND_CARR_TP_MEAN_CODE\"]+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								");
				        tw.WriteLine("								Console.WriteLine(\"LOC+165+THLCH:139:6+THLCHDL\"+EdiTB.Rows[checkdtb][\"AREA_C\"].ToString()+\":TER:ZZZ'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString() != \"NO\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"NAD+CN+\"+EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString().Replace(\"'\",\"\")+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");

				        tw.WriteLine("							}");
				        tw.WriteLine("	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------");
				        tw.WriteLine("				Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("");
				        tw.WriteLine("				Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");
				        tw.WriteLine("				Console.WriteLine(\"UNZ+1+CODECO'\");");
				        tw.WriteLine("				Console.SetOut (oldOut);");
				        tw.WriteLine("				writer.Close();");
				        tw.WriteLine("				ostrm.Close(); ");
				        tw.WriteLine("");
				        tw.WriteLine("				countSegment =0;");
				        tw.WriteLine("// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------");
				        tw.WriteLine("");
				        tw.WriteLine("");
				        tw.WriteLine("				if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				break;");
				        tw.WriteLine("");
				  
				        tw.WriteLine("");
				        tw.WriteLine("			case \"OT\":");
				        tw.WriteLine("");
				        tw.WriteLine("				EDIHeader=	\"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"DI++CODECO'\\r\\n\"+");
				        tw.WriteLine("							\"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+CODECO:D:95B:UN'\\r\\n\"+");
				        tw.WriteLine("							\"BGM+36+CONTAINER GATE IN/OUT+9'\\r\\n\"+");
				        tw.WriteLine("							\"TDT+20+\"+EdiTB.Rows[0][\"VOYAGE_AN\"].ToString()+\"+1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103'\\r\\n\"+");	        
				        tw.WriteLine("							\"LOC+15+LCB05'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+11+THLCH:139:6'\\r\\n\"+");
				        tw.WriteLine("							\"NAD+MS+LCIT'\\r\\n\";");
				        tw.WriteLine("");
				        tw.WriteLine("							 countSegment = countSegment+7;");
				        tw.WriteLine("");
				        tw.WriteLine("				ostrm = new FileStream (SaveEDIfile.ToString()+\"COD\"+Line+\"OT\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);");
				        tw.WriteLine("				writer = new StreamWriter (ostrm);");
				        tw.WriteLine("				Console.SetOut (writer);");  
				        tw.WriteLine("");
				        tw.WriteLine("				Console.Write(EDIHeader);");
				        tw.WriteLine("");
				        tw.WriteLine("				for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("					Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString() != \"NOBL\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"RFF+BM:\"+EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString()+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					Console.WriteLine(\"DTM+7:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+9+\"+EdiTB.Rows[checkdtb][\"POL\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+99+\"+EdiTB.Rows[checkdtb][\"DST_CODE\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"MEA+AAE+VGM+KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"+CA'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()!=\" \")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"DAM+\"+EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					");
				        tw.WriteLine("					Console.WriteLine(\"TDT+1++\"+EdiTB.Rows[checkdtb][\"OUT_TRANSPORT_MODE_CODE\"].ToString()+\"+++++:::\"+EdiTB.Rows[checkdtb][\"INLAND_CARR_TP_MEAN_CODE\"]+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+165+THLCH:139:6+THLCHDL\"+EdiTB.Rows[checkdtb][\"AREA_C\"].ToString()+\":TER:ZZZ'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString() != \"NO\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"NAD+CN+\"+EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString().Replace(\"'\",\"\")+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("				}");
				        tw.WriteLine("		Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");
				        tw.WriteLine("		countSegment++;");
				        tw.WriteLine("		Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");
				        tw.WriteLine("		Console.WriteLine(\"UNZ+1+CODECO'\");");
				        tw.WriteLine("	//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------");
				        tw.WriteLine("		Console.SetOut (oldOut);");
				        tw.WriteLine("		writer.Close();");
				        tw.WriteLine("		ostrm.Close();");
				        tw.WriteLine("		countSegment =0;");
				        tw.WriteLine("");
				        tw.WriteLine("");
				        tw.WriteLine("				if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("		break;");
				        tw.WriteLine("		}");
				        tw.WriteLine("	}");
				        tw.WriteLine("	");
				        }				   

				     if(EDI_Fact.ToString()=="CODECO+COARRI")
				        {
				      		tw.WriteLine("");	
				        tw.WriteLine("			switch(Move)");
				        tw.WriteLine("			{");

				        tw.WriteLine("			case \"IN\":");
				        tw.WriteLine("");
				        tw.WriteLine("//-----------------------------------CREATE HEADER EDI FILE -----------------------------------------------");
				        tw.WriteLine("//-------------- For Header Table Query data from : EdiTB.Rows[0][\"COLUMNS_NAME\"].ToString()-------------");

				        tw.WriteLine("				EDIHeader=	\"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"DI++CODECO'\\r\\n\"+");
				        tw.WriteLine("							\"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+CODECO:D:95B:UN'\\r\\n\"+");
				        tw.WriteLine("							\"BGM+34+CONTAINER GATE IN/OUT+9'\\r\\n\"+");
				        tw.WriteLine("							\"TDT+20+\"+EdiTB.Rows[0][\"VOYAGE_AN\"].ToString()+\"+1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+15+LCB05'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+9+THLCH:139:6'\\r\\n\"+");
				        tw.WriteLine("							\"NAD+MS+LCIT'\\r\\n\";");
				        tw.WriteLine("");
				        tw.WriteLine("							countSegment = countSegment+7;");
				        tw.WriteLine("");
				        
				        tw.WriteLine("");
				        tw.WriteLine("// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------");
				        tw.WriteLine("// -----------------For Content Query Data form EdiTB.Rows[checkdtb][\"COLUMNS_NAME\"]  -----------------------------");
				        tw.WriteLine("");
				        tw.WriteLine("							ostrm = new FileStream (SaveEDIfile.ToString()+\"COD\"+Line+\"IN\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);");
				        tw.WriteLine("							writer = new StreamWriter (ostrm);");
				        tw.WriteLine("							Console.SetOut (writer);");
				        tw.WriteLine("							Console.Write(EDIHeader);");
				        tw.WriteLine("							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)");
				        tw.WriteLine("							{");
				        tw.WriteLine("								Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								Console.WriteLine(\"DTM+7:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								Console.WriteLine(\"MEA+AAE+VGM+KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("									Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"+CA'\");");
				        tw.WriteLine("									countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()!=\" \")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"DAM+\"+EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");
				        tw.WriteLine("								Console.WriteLine(\"TDT+1++\"+EdiTB.Rows[checkdtb][\"IN_TRANSPORT_MODE_CODE\"].ToString()+\"+++++:::\"+EdiTB.Rows[checkdtb][\"INLAND_CARR_TP_MEAN_CODE\"]+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								");
				        tw.WriteLine("								Console.WriteLine(\"LOC+165+THLCH:139:6+THLCHDL\"+EdiTB.Rows[checkdtb][\"AREA_C\"].ToString()+\":TER:ZZZ'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								");
				        tw.WriteLine("								if(EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString() != \"NO\")");
				        tw.WriteLine("								{");
				        tw.WriteLine("								Console.WriteLine(\"NAD+CN+\"+EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString().Replace(\"'\",\"\")+\"'\");");
				        tw.WriteLine("								countSegment++;");
				        tw.WriteLine("								}");
				        tw.WriteLine("								");
				        tw.WriteLine("							}");
				        tw.WriteLine("	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------");
				        tw.WriteLine("				Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("");
				        tw.WriteLine("				Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");
				        tw.WriteLine("				Console.WriteLine(\"UNZ+1+CODECO'\");");
				        tw.WriteLine("				Console.SetOut (oldOut);");
				        tw.WriteLine("				writer.Close();");
				        tw.WriteLine("				ostrm.Close(); ");
				        tw.WriteLine("");
				        tw.WriteLine("				countSegment =0;");
				        tw.WriteLine("// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------");
				        tw.WriteLine("");
				        tw.WriteLine("");
				        tw.WriteLine("				if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				break;");
				        tw.WriteLine("");
				        tw.WriteLine("");
				        tw.WriteLine("			case \"OT\":");
				        tw.WriteLine("");
				        tw.WriteLine("				EDIHeader=	\"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"DI++CODECO'\\r\\n\"+");
				        tw.WriteLine("							\"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+CODECO:D:95B:UN'\\r\\n\"+");
				        tw.WriteLine("							\"BGM+36+CONTAINER GATE IN/OUT+9'\\r\\n\"+");
				        tw.WriteLine("							\"TDT+20+\"+EdiTB.Rows[0][\"VOYAGE_AN\"].ToString()+\"+1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103'\\r\\n\"+");	        
				        tw.WriteLine("							\"LOC+15+LCB05'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+11+THLCH:139:6'\\r\\n\"+");
				        tw.WriteLine("							\"NAD+MS+LCIT'\\r\\n\";");
				        tw.WriteLine("");
				        tw.WriteLine("							 countSegment = countSegment+7;");
				        tw.WriteLine("");
				        tw.WriteLine("				ostrm = new FileStream (SaveEDIfile.ToString()+\"COD\"+Line+\"OT\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);");
				        tw.WriteLine("				writer = new StreamWriter (ostrm);");
				        tw.WriteLine("				Console.SetOut (writer);");  
				        tw.WriteLine("");
				        tw.WriteLine("				Console.Write(EDIHeader);");
				        tw.WriteLine("");
				        tw.WriteLine("				for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("					Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString() != \"NOBL\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"RFF+BM:\"+EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString()+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					Console.WriteLine(\"DTM+7:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+9+\"+EdiTB.Rows[checkdtb][\"POL\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+99+\"+EdiTB.Rows[checkdtb][\"DST_CODE\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"MEA+AAE+VGM+KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"+CA'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()!=\" \")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"DAM+\"+EdiTB.Rows[checkdtb][\"DAMAGE_SEGMENT_AN\"].ToString()+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("					");
				        tw.WriteLine("					Console.WriteLine(\"TDT+1++\"+EdiTB.Rows[checkdtb][\"OUT_TRANSPORT_MODE_CODE\"].ToString()+\"+++++:::\"+EdiTB.Rows[checkdtb][\"INLAND_CARR_TP_MEAN_CODE\"]+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					Console.WriteLine(\"LOC+165+THLCH:139:6+THLCHDL\"+EdiTB.Rows[checkdtb][\"AREA_C\"].ToString()+\":TER:ZZZ'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("					");
				        tw.WriteLine("					if(EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString() != \"NO\")");
				        tw.WriteLine("					{");
				        tw.WriteLine("						Console.WriteLine(\"NAD+CN+\"+EdiTB.Rows[checkdtb][\"SHIPPER\"].ToString().Replace(\"'\",\"\")+\"'\");");
				        tw.WriteLine("						countSegment++;");
				        tw.WriteLine("					}");
				        tw.WriteLine("				}");
				        tw.WriteLine("		Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");
				        tw.WriteLine("		countSegment++;");
				        tw.WriteLine("		Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");
				        tw.WriteLine("		Console.WriteLine(\"UNZ+1+CODECO'\");");
				        tw.WriteLine("	//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------");
				        tw.WriteLine("		Console.SetOut (oldOut);");
				        tw.WriteLine("		writer.Close();");
				        tw.WriteLine("		ostrm.Close();");
				        tw.WriteLine("		countSegment =0;");
				        tw.WriteLine("");
				        tw.WriteLine("");
				        tw.WriteLine("				if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("				else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("				}");
				        tw.WriteLine("		break;");
				        tw.WriteLine("	");
				        tw.WriteLine("			case \"LD\":");
				        tw.WriteLine("EDIHeader = \"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"LO++COARRI'\\r\\n\"+");
				 		tw.WriteLine("			  \"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+COARRI:D:95B:UN'\\r\\n\"+");
				 		tw.WriteLine("			  \"BGM+270+Discharge/Loading report+9'\\r\\n\"+");
				 		tw.WriteLine("			  \"TDT+20++1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103::\"+EdiTB.Rows[0][\"VESSEL_NM_AN\"].ToString()+\"'\\r\\n\"+");
				 		tw.WriteLine("			  \"LOC+9+THLCH:139:6+THLCHDL\"+EdiTB.Rows[0][\"AREA_C\"].ToString()+\":TER:ZZZ'\\r\\n\"+");
				 		tw.WriteLine("			  \"NAD+MS+LCIT'\\r\\n\";");
				        tw.WriteLine("");
				        tw.WriteLine("			  countSegment = countSegment+6;");
				        tw.WriteLine("");
				        tw.WriteLine("			  ostrm = new FileStream (SaveEDIfile.ToString()+\"COR\"+Line+\"LD\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);");
				        tw.WriteLine("			  writer = new StreamWriter (ostrm);");
				        tw.WriteLine("			  Console.SetOut (writer);");
				        tw.WriteLine("");
				        tw.WriteLine("			  Console.Write(EDIHeader);");
				        tw.WriteLine("");
				        tw.WriteLine("			  for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)");
				        tw.WriteLine("			  {");
				        tw.WriteLine("				Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"CONTAINER_TYPE_CODE\"].ToString()+\"++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");");
				        tw.WriteLine("				    countSegment++;");
				        tw.WriteLine("				}");
				        tw.WriteLine("				Console.WriteLine(\"DTM+203:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				Console.WriteLine(\"LOC+147+::5'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				Console.WriteLine(\"LOC+5+THLCH::6'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				Console.WriteLine(\"MEA+WT++KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("				}");
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"IMCO\"].ToString() != \"NODG\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"DGS+IMD+\"+EdiTB.Rows[checkdtb][\"IMCO\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("				}");
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"TEMPERATURE\"].ToString() != \"NOTEMP\")");
				        tw.WriteLine("				{");
				        tw.WriteLine("					Console.WriteLine(\"EQA+RG+\"+EdiTB.Rows[checkdtb][\"TEMPERATURE\"].ToString()+\"'\");");
				        tw.WriteLine("					countSegment++;");
				        tw.WriteLine("				}");
				        tw.WriteLine("			  }");
				        tw.WriteLine("				Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");
				        tw.WriteLine("				countSegment++;");
				        tw.WriteLine("				Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");
				        tw.WriteLine("				Console.WriteLine(\"UNZ+1+COARRI'\");");
				        tw.WriteLine("//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------");
				        tw.WriteLine("	Console.SetOut (oldOut);");
				        tw.WriteLine("	writer.Close();");
				        tw.WriteLine("	ostrm.Close(); ");
				        tw.WriteLine("	countSegment =0;");
				        tw.WriteLine("");
				        tw.WriteLine("	if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("   {");
				        tw.WriteLine("		send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("		MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("	 }");
				        tw.WriteLine("	else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("	{");
				        tw.WriteLine("		MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("	}");
				        tw.WriteLine("			break;");
				        tw.WriteLine("			case \"DG\":");
				        tw.WriteLine("				EDIHeader = \"UNB+UNOA:1+\"+SenderID+\"+\"+ReceiveID+\"+\"+dt.ToString(\"yyMMdd\")+\":\"+dt.ToString(\"HHmm\")+\"+\"+\"DI++COARRI'\\r\\n\"+");
				        tw.WriteLine("							\"UNH+\"+dt.ToString(\"yyyyMMddHH\")+\"+COARRI:D:95B:UN'\\r\\n\"+");
				        tw.WriteLine("						    \"BGM+98+Discharge/Loading report+9'\\r\\n\"+");
				        tw.WriteLine("							\"TDT+20+\"+EdiTB.Rows[0][\"VOYAGE_AN\"].ToString()+\"+1+++++\"+EdiTB.Rows[0][\"VISIT_VSL_CALL_SIGN_C\"].ToString()+\":103::\"+EdiTB.Rows[0][\"VESSEL_NM_AN\"].ToString()+\"'\\r\\n\"+");
				        tw.WriteLine("							\"LOC+11+THLCH:139:6+THLCHDL\"+EdiTB.Rows[0][\"AREA_C\"].ToString()+\":TER:ZZZ'\\r\\n\"+");
				        tw.WriteLine("							\"NAD+MS+LCIT'\\r\\n\";"); 
				        tw.WriteLine(""); 
				        tw.WriteLine("				countSegment = countSegment+6;"); 
				        tw.WriteLine("				ostrm = new FileStream (SaveEDIfile.ToString()+\"COR\"+Line+\"DG\"+TerArea+FileName.ToString()+\".EDI\", FileMode.Create, FileAccess.Write);"); 
				        tw.WriteLine("				writer = new StreamWriter (ostrm);"); 
				        tw.WriteLine("				Console.SetOut (writer);"); 
				        tw.WriteLine("				Console.Write(EDIHeader);"); 
				        tw.WriteLine(""); 
				        tw.WriteLine("			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)"); 
				        tw.WriteLine("			{"); 
				        tw.WriteLine("				Console.WriteLine(\"EQD+CN+\"+EdiTB.Rows[checkdtb][\"CNTR_AN\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"CONTAINER_TYPE_CODE\"].ToString()+\"++\"+EdiTB.Rows[checkdtb][\"EQP_STATUS_CODE\"].ToString()+\"+\"+EdiTB.Rows[checkdtb][\"LADEN_INDICATOR_AN\"].ToString()+\"'\");"); 
				        tw.WriteLine("				countSegment++;"); 
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString() != \"NOBOOKING\")"); 
				        tw.WriteLine("				{"); 
				        tw.WriteLine("					Console.WriteLine(\"RFF+BN:\"+EdiTB.Rows[checkdtb][\"BOOKING_NO_AN\"].ToString()+\"'\");"); 
				        tw.WriteLine("					countSegment++;"); 
				        tw.WriteLine("				}"); 
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString() != \"NOBL\")");   
				        tw.WriteLine("				{");   
				        tw.WriteLine("					Console.WriteLine(\"RFF+BM:\"+EdiTB.Rows[checkdtb][\"MASTER_BOL_AN\"].ToString()+\"'\");");   
				        tw.WriteLine("					countSegment++;");   
				        tw.WriteLine("				}");   
				        tw.WriteLine("				Console.WriteLine(\"DTM+203:\"+EdiTB.Rows[checkdtb][\"ACTIVITY_TM\"].ToString()+\":203'\");");   
				        tw.WriteLine("				countSegment++;");   
				        tw.WriteLine("				Console.WriteLine(\"LOC+9+\"+EdiTB.Rows[checkdtb][\"POL\"].ToString()+\":139:6'\");");   
				        tw.WriteLine("				countSegment++;");   
				        tw.WriteLine("				Console.WriteLine(\"LOC+147+::5'\");");   
				        tw.WriteLine("				countSegment++;");   
				        tw.WriteLine("				Console.WriteLine(\"LOC+60+THLCH::6'\");");   
				        tw.WriteLine("				countSegment++;");   
				        tw.WriteLine("				Console.WriteLine(\"MEA+WT++KGM:\"+EdiTB.Rows[checkdtb][\"GWEIGHT\"].ToString()+\"'\");");   
				        tw.WriteLine("				countSegment++;");   
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"SEAL\"].ToString()!=\"NOSEAL\")");   
				        tw.WriteLine("				{");   
				        tw.WriteLine("					Console.WriteLine(\"SEL+\"+EdiTB.Rows[checkdtb][\"SEAL\"].ToString()+\"'\");");   
				        tw.WriteLine("					countSegment++;");   
				        tw.WriteLine("				}");   
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"IMCO\"].ToString() != \"NODG\")");   
				        tw.WriteLine("				{");   
				        tw.WriteLine("					Console.WriteLine(\"DGS+IMD+\"+EdiTB.Rows[checkdtb][\"IMCO\"].ToString()+\"'\");");   
				        tw.WriteLine("					countSegment++;");   
				        tw.WriteLine("				}");   
				        tw.WriteLine("				if(EdiTB.Rows[checkdtb][\"TEMPERATURE\"].ToString() != \"NOTEMP\")");   
				        tw.WriteLine("				{");   
				        tw.WriteLine("					Console.WriteLine(\"EQA+RG+\"+EdiTB.Rows[checkdtb][\"TEMPERATURE\"].ToString()+\"'\");");   
				        tw.WriteLine("					countSegment++;");   
				        tw.WriteLine("				}");   
				        tw.WriteLine("			}");   
				        tw.WriteLine("		Console.WriteLine(\"CNT+16:\"+EdiTB.Rows.Count+\"'\");");   
				        tw.WriteLine("		countSegment++;");   
				        tw.WriteLine("		Console.WriteLine(\"UNT+\"+countSegment.ToString()+\"+\"+dt.ToString(\"yyyyMMddHH\")+\"'\");");   
				        tw.WriteLine("		Console.WriteLine(\"UNZ+1+COARRI'\");");   
				        tw.WriteLine("		Console.SetOut (oldOut);");   
				        tw.WriteLine("		writer.Close();");   
				        tw.WriteLine("		ostrm.Close();");   
				        tw.WriteLine("		countSegment =0;");   
						tw.WriteLine("	if(SenderType.ToString() == \"EMAIL\")");
				        tw.WriteLine("   {");
				        tw.WriteLine("		send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());");
				        tw.WriteLine("		MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("	 }");
				        tw.WriteLine("	else if(SenderType.ToString() == \"FTP\")");
				        tw.WriteLine("	{");
				        tw.WriteLine("		MoveFile(SaveEDIfile,SenderType.ToString());");
				        tw.WriteLine("	}");
				        tw.WriteLine("			break;");
				        tw.WriteLine("		}");
				        tw.WriteLine("	}");
				      }
				       
				       	tw.WriteLine("		public static void MoveFile(string PathFileMove, string sender)");
						tw.WriteLine("		{");
						tw.WriteLine("					if(sender.ToString() == \"FTP\")");
						tw.WriteLine("					{");
						tw.WriteLine("						DirectoryInfo from = new DirectoryInfo(@\"\"+PathFileMove);");
						tw.WriteLine("						DirectoryInfo to = new DirectoryInfo(@\"\"+pathfile.FTP.ToString()+\"\\\\"+Lineoper+"\\\\IO\");");
						tw.WriteLine("						foreach(FileInfo fi in from.GetFiles(\"*.EDI\"))");
						tw.WriteLine("						{");
						tw.WriteLine("							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));");
						tw.WriteLine("						}");
						tw.WriteLine("					}");
						tw.WriteLine("					else");
						tw.WriteLine("					{");
						tw.WriteLine("						DirectoryInfo from = new DirectoryInfo(@\"\"+PathFileMove);");
						tw.WriteLine("						DirectoryInfo to = new DirectoryInfo(@\"\"+PathFileMove+\"backup\\\\\");");
						tw.WriteLine("						foreach(FileInfo fi in from.GetFiles(\"*.EDI\"))");
						tw.WriteLine("						{");
						tw.WriteLine("							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));");
						tw.WriteLine("						}");
						tw.WriteLine("					}");
						tw.WriteLine("		}");
						tw.WriteLine("	}");

				    }
				}
// -- END CREATE CS FILE FORGENERATE EDI FILE --




			}
			return dt_profile;
             
        }

        public DataTable check_profile_db(String pfline)
        {
        	SqlConnection connect_profile = new SqlConnection();
			    	connect_profile.ConnectionString = constr.edidbconnection;
			    	connect_profile.Open();
			SqlDataAdapter sda_profile = new SqlDataAdapter("SELECT * FROM LCIT_EDI.DBO.TEST_LINER_PROFILE WHERE LINE_ID ='"+pfline+"'",connect_profile);
			DataTable dt_profile = new DataTable();	
			dt_profile.TableName = "Profile_created";
			sda_profile.Fill(dt_profile);
			connect_profile.Close();

			return dt_profile;
        }

		
 	}

 }
