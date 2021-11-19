using System;
using System.Net.Mail;
using System.IO;
using System.Data.SqlClient;
using System.Data;


	public class customer_mail_by_type
	{

		static keepfile.flepath pathfile = new keepfile.flepath();
		sqlcon.Connection_String constr = new sqlcon.Connection_String();
		
		public void send_mail_control(string LineOper, string Movement, string AreaCode,string editype)
		{
				// Console.WriteLine("------------------------------------------------------");
				// Console.WriteLine("----------------SEND EMAIL TO CUSTOMER----------------");
				// Console.WriteLine("------------------------------------------------------");

				DirectoryInfo FinalPath = new DirectoryInfo(@""+pathfile.SaveEDI+LineOper+"\\"+Movement+"\\"+AreaCode+"\\");
				DirectoryInfo FinalPathBack = new DirectoryInfo(@""+pathfile.SaveEDI+LineOper+"\\"+Movement+"\\"+AreaCode+"\\"+"backup");
				if(editype.ToString() == "IN" || editype.ToString() == "OT")
				{
					editype = "CODECO";
				}
				else if(editype.ToString() == "LD" || editype.ToString() == "DG")
				{
					editype = "COARRI";
				}

// CHECK EMAIL ADDRESS
				SqlConnection conn = new SqlConnection();
			    	conn.ConnectionString = constr.edidbconnection;
			    	conn.Open();
			    SqlDataAdapter sda_checkemail = new SqlDataAdapter("SELECT SEND_TO,CC FROM LCIT_EDI.DBO.EMAIL_SENDER WHERE LINE_ID = '"+LineOper+"' AND  EDI_TYPE='"+editype.ToString()+"'",conn);

			    DataTable dt_checkemail = new DataTable();
			    	 dt_checkemail.TableName ="EMAIL_ADDRESS";
			    	 sda_checkemail.Fill(dt_checkemail);
					 conn.Close();


// SEARCH FILE IN DIRECTORY
		string EDI_search = ".EDI";

					 if(LineOper == "SKR")
					 {
					 	EDI_search = ".TXT";
					 }

					  if(dt_checkemail.Rows.Count > 0)
					  {
					  	foreach (FileInfo fi in FinalPath.GetFiles("*"+EDI_search+""))
							{
								MailMessage mail = new MailMessage();
								SmtpClient SmtpServer = new SmtpClient("172.19.240.77");
					
								mail.From = new MailAddress("Operations@lcit.com");
								mail.IsBodyHtml = true;
								mail.To.Add(dt_checkemail.Rows[0]["SEND_TO"].ToString());

								
								if(dt_checkemail.Rows[0]["CC"].ToString() != "")
								{
									mail.CC.Add(dt_checkemail.Rows[0]["CC"].ToString());
								}
								
								
								
								mail.Subject = LineOper+" EDI :" +Movement+"MOVEMENT FROM LCIT " + AreaCode ;
								mail.Body = "<b>Dear Sir,</b> <br /><br />"+
									"This is EDI from LCIT if any concern please direct to it@lcit.com.<br /><br />"+
									"Best Regards,";		   

								System.Net.Mail.Attachment attachment;
								attachment = new System.Net.Mail.Attachment(FinalPath.ToString() + fi.Name.ToString());
								mail.Attachments.Add(attachment);

								Console.WriteLine("Sent file : " + fi.Name.ToString());
						   
								SmtpServer.Port = 25;
								SmtpServer.Credentials = new System.Net.NetworkCredential("lcit\\administrator", PS("QERNNDA4TEAzbQ =="));
								SmtpServer.Send(mail);

								mail.Dispose();						
							}		
					  }
			
		}

		public string PS(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
		   
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
