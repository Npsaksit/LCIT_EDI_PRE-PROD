using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
	public class YML
	{
		static keepfile.flepath pathfile = new keepfile.flepath();
		static customer_mail send_mail = new customer_mail();
		static liner_profile.checkprofile LinerPF = new liner_profile.checkprofile();

		public static void  create_file(string Line, string Move, DateTime dt, DataTable EdiTB,String TerArea)
		{
			FileStream ostrm;
			StreamWriter writer;
			TextWriter oldOut = Console.Out;
			string  SaveEDIfile =pathfile.SaveEDI.ToString()+Line+"\\"+Move+"\\"+TerArea+"\\";
			string  di = pathfile.MainDirectory.ToString();
			string SenderID = "";
			string ReceiveID ="";
			string EDIHeader ="";
			int countSegment =0;
			string FileName= "";
			string SenderType = "";
			string[] MyVessel = EdiTB.Rows[0]["VESSEL_NM_AN"].ToString().Split(new char[0]);

		//-------------------------------- CHECK DOCUMENT CUSTOMER PROFILR IN XML -----------------------------

			DataTable dt_profile = new DataTable();
			dt_profile = LinerPF.getLiner_profile(Line);
			FileName = String.Format("{0:D10}",Int32.Parse(dt_profile.Rows[0]["RUNNING_NUMBER"].ToString()));

			if(TerArea.ToString() == "B5")
			 {

			 SenderID = dt_profile.Rows[0]["SENDERID_B5"].ToString();

			 }
			if(TerArea.ToString() == "C3")
			 {

			 SenderID = dt_profile.Rows[0]["SENDERID_C3"].ToString();

			 }

			 ReceiveID = dt_profile.Rows[0]["RECEIVERID"].ToString();
			 SenderType = dt_profile.Rows[0]["SENDER_TYPE"].ToString();

			switch(Move)
			{
			case "IN":

//-----------------------------------CREATE HEADER EDI FILE -----------------------------------------------
//-------------- For Header Table Query data from : EdiTB.Rows[0]["COLUMNS_NAME"].ToString()-------------
				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmmss")+"++CODECO'\r\n";
							

							countSegment = countSegment+1;


// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
// -----------------For Content Query Data form EdiTB.Rows[checkdtb]["COLUMNS_NAME"]  -----------------------------

							ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"IN"+TerArea+FileName.ToString()+".TO_ITBX", FileMode.Create, FileAccess.Write);
							writer = new StreamWriter (ostrm);
							Console.SetOut (writer); 
							Console.Write(EDIHeader);
							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
							{
								countSegment = 1;

								Console.WriteLine("UNH+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+CODECO:D:95B:UN'");
								countSegment++;
								
								Console.WriteLine("BGM+34+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+9'");
								countSegment++;
								
								Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++YML:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
								countSegment++;
								
								Console.WriteLine("LOC+9+THLCH:139:6'");
								countSegment++;
								
								Console.WriteLine("NAD+CA+YML'");
								countSegment++;

								Console.WriteLine("EQD+CN+" + EdiTB.Rows[checkdtb]["CNTR_AN"].ToString() + "+" + EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString() + ":102:5++" + EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString() + "+" + EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() + "'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
								{
								Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
								countSegment++;
								}
								Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
								countSegment++;


								Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
								countSegment++;

								Console.WriteLine("LOC+165+THLCH:139:6'");
								countSegment++;

								Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
								{
									Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
									countSegment++;
								}
								 								
								Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["IN_TRANSPORT_MODE_CODE"].ToString()+"+31+:172+++"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"'");
								countSegment++;
								
								Console.WriteLine("CNT+16:"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");
								countSegment++;

								Console.WriteLine("UNT+"+countSegment.ToString()+"+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");

								
								
							}
	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
				
				Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+"+dt.ToString("yyyyMMddHHmmss")+"'");
				Console.SetOut (oldOut);
				writer.Close();
				ostrm.Close(); 

				countSegment =0;
// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------


				// if(SenderType.ToString() == "EMAIL")
				// {
				// 	send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
				// 	MoveFile(SaveEDIfile,SenderType.ToString());
				// }
				// else if(SenderType.ToString() == "FTP")
				// {
				// 	MoveFile(SaveEDIfile,SenderType.ToString());
				// }
				break;


			case "OT":

				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmmss")+"++CODECO'\r\n";
							

							 countSegment = countSegment+1;

				ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"OT"+TerArea+FileName.ToString()+".TO_ITBX", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);

				Console.Write(EDIHeader);

				for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				{   
					countSegment=1;

					Console.WriteLine("UNH+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+CODECO:D:95B:UN'");
					countSegment++;

					Console.WriteLine("BGM+36+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+9'");
					countSegment++;

					Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++YML:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
					countSegment++;

					Console.WriteLine("LOC+9+THLCH:139:6'");
					countSegment++;

					
					Console.WriteLine("NAD+CA+YML'");
					countSegment++;

					Console.WriteLine("EQD+CN+" + EdiTB.Rows[checkdtb]["CNTR_AN"].ToString() + "+" + EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString() + ":102:5++" + EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString() + "+" + EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() + "'");
					countSegment++;
					
					if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
					{
					Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					countSegment++;
					}
					if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
					{
						Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
						countSegment++;
					}
					Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					countSegment++;

					Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
					countSegment++;

					Console.WriteLine("LOC+165+THLCH:139:6'");
					countSegment++;

					Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					countSegment++;
					
					if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
					{
						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
						countSegment++;
					}
					
					Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["OUT_TRANSPORT_MODE_CODE"].ToString()+"+31+:172+++"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"'");
					countSegment++;
					
					Console.WriteLine("CNT+16:"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");
					countSegment++;

					Console.WriteLine("UNT+"+countSegment.ToString()+"+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");


				
				}
		
		Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+"+dt.ToString("yyyyMMddHHmmss")+"'");
	//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;


				// if(SenderType.ToString() == "EMAIL")
				// {
				// 	send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
				// 	MoveFile(SaveEDIfile,SenderType.ToString());
				// }
				// else if(SenderType.ToString() == "FTP")
				// {
				// 	MoveFile(SaveEDIfile,SenderType.ToString());
				// }
		break;
	
			case "LD":
EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmmss")+"++COARRI'\r\n";
			  
			  countSegment = countSegment+1;

			  ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"LD"+TerArea+FileName.ToString()+".TO_ITBX", FileMode.Create, FileAccess.Write);
			  writer = new StreamWriter (ostrm);
			  Console.SetOut (writer);

			  Console.Write(EDIHeader);

			  for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			  {
			  	countSegment=1;

			  	Console.WriteLine("UNH+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+COARRI:D:95B:UN'");
				countSegment++;

			  	Console.WriteLine("BGM+46+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+9'");
				countSegment++;

				Console.WriteLine("FTX+OSI++L'");
				countSegment++;

			  	Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++YML:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
				countSegment++;

			  	Console.WriteLine("LOC+9+THLCH:139:6'");
				countSegment++;

			  	Console.WriteLine("NAD+CA+YML'");
				countSegment++;

				Console.WriteLine("EQD+CN+" + EdiTB.Rows[checkdtb]["CNTR_AN"].ToString() + "+" + EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString() + ":102:5++" + EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString() + "+" + EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() + "'");
				countSegment++;


				Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
				countSegment++;

				Console.WriteLine("LOC+9+THLCH:139:6'");
				countSegment++;
				
				Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
				countSegment++;
  

				Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"'");
				countSegment++;

				Console.WriteLine("LOC+5+THLCH:139:6'");
				countSegment++;

				Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
				countSegment++;
				
				if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				{
					Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					countSegment++;
				}
				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"'");
					countSegment++;
				}
				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("EQA+RG+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+"'");
					countSegment++;
				}

				Console.WriteLine("CNT+16:"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");
				countSegment++;
				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");

				
			  }
				
				Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+"+dt.ToString("yyyyMMddHHmmss")+"'");
//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
	Console.SetOut (oldOut);
	writer.Close();
	ostrm.Close(); 
	countSegment =0;

	// if(SenderType.ToString() == "EMAIL")
 //   {
	// 	send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
	// 	MoveFile(SaveEDIfile,SenderType.ToString());
	//  }
	// else if(SenderType.ToString() == "FTP")
	// {
	// 	MoveFile(SaveEDIfile,SenderType.ToString());
	// }
			break;
			case "DG":
				EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmmss")+"++COARRI'\r\n";			

				countSegment = countSegment+6;
				ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"DG"+TerArea+FileName.ToString()+".TO_ITBX", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);
				Console.Write(EDIHeader);

			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			{
				countSegment=1;

				Console.WriteLine("UNH+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+COARRI:D:95B:UN'");
				countSegment++;

				Console.WriteLine("BGM+44+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"+9'");
				countSegment++;

				Console.WriteLine("FTX+OSI++D'");
				countSegment++;

				Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++YML:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
				countSegment++;

				Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
				countSegment++;

				Console.WriteLine("NAD+CA+YML'");
				countSegment++;

				Console.WriteLine("EQD+CN+" + EdiTB.Rows[checkdtb]["CNTR_AN"].ToString() + "+" + EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString() + ":102:5++" + EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString() + "+" + EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() + "'");
				countSegment++;

				// if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
				// {
				// 	Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
				// 	countSegment++;
				// }
				
				if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
				{
					Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
					countSegment++;
				}

				Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
				countSegment++;
				Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
				countSegment++;
				
				Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
				countSegment++;

				// Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"'");
				// countSegment++;

				Console.WriteLine("LOC+5+THLCH:139:6'");
				countSegment++;

				Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
				countSegment++;
				// if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				// {
				// 	Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
				// 	countSegment++;
				// }
				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"'");
					countSegment++;
				}
				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("EQA+RG+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+"'");
					countSegment++;
				}

				Console.WriteLine("CNT+16:"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");
				countSegment++;
				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+(Int32.Parse(checkdtb.ToString())+1).ToString()+"'");

				
			}
	
		Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+"+dt.ToString("yyyyMMddHHmmss")+"'");
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;
	// if(SenderType.ToString() == "EMAIL")
 //   {
	// 	send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
	// 	MoveFile(SaveEDIfile,SenderType.ToString());
	//  }
	// else if(SenderType.ToString() == "FTP")
	// {
	// 	MoveFile(SaveEDIfile,SenderType.ToString());
	// }
			break;
		}
	}
		public static void MoveFile(string PathFileMove, string sender)
		{
					if(sender.ToString() == "FTP")
					{
						DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
						DirectoryInfo to = new DirectoryInfo(@""+pathfile.FTP.ToString()+"\\YML\\IO");
						foreach(FileInfo fi in from.GetFiles("*.TO_ITBX"))
						{
							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
						}
					}
					else
					{
						DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
						DirectoryInfo to = new DirectoryInfo(@""+PathFileMove+"backup\\");
						foreach(FileInfo fi in from.GetFiles("*.TO_ITBX"))
						{
							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
						}
					}
		}
	}
