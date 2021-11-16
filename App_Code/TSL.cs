using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
	public class TSL
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
				EDIHeader=	"UNB+UNOA:2+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"COD"+Line+"IN"+TerArea+FileName.ToString()+"'\r\n";

							countSegment = countSegment+1;

						
// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
// -----------------For Content Query Data form EdiTB.Rows[checkdtb]["COLUMNS_NAME"]  -----------------------------

							ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"IN"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
							writer = new StreamWriter (ostrm);
							Console.SetOut (writer);
							Console.Write(EDIHeader);
							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
							{
								countSegment = 1;
								Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN:ITG12'");
								countSegment++;

								if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() == "4")
								{
									Console.WriteLine("BGM+M+CONTAINER GATE IN/OUT+9'");
									countSegment++;
								}

								if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() == "5")
								{
									Console.WriteLine("BGM+34+CONTAINER GATE IN/OUT+9'");
									countSegment++;
								}

								Console.WriteLine("NAD+CF+TSL'");
								countSegment++;

								Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING"  && EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString() != "4")
								{
								Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
								countSegment++;
								}

							

								Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
								countSegment++;

								Console.WriteLine("LOC+165+THLCB:139:6+"+SenderID+":TER:ZZZ'");
								countSegment++;

								Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
								countSegment++;

								Console.WriteLine("MEA+AAE+T+KGM:"+EdiTB.Rows[checkdtb]["TARE_WEIGHT"].ToString()+"'");
								countSegment++;

								if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
								{
									Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
									countSegment++;
								}
								
								if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
								{
								Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
								countSegment++;
								}
								
								Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["IN_TRANSPORT_MODE_CODE"].ToString()+"+++++"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"'");
								countSegment++;

		 						if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != "NO")
		 						{
		 							// Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString()+"'");
		 							Console.WriteLine("FTX+AAI+++"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
		 							countSegment++;
		 						}

		 						Console.WriteLine("CNT+16:1'");
								countSegment++;

								Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
	
							}
	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
				
				Console.WriteLine("UNZ+1+"+"COD"+Line+"IN"+TerArea+FileName.ToString()+"'");
				Console.SetOut (oldOut);
				writer.Close();
				ostrm.Close(); 

				countSegment =0;
// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------


				if(SenderType.ToString() == "EMAIL")
				{
					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
					MoveFile(SaveEDIfile,SenderType.ToString());
				}
				else if(SenderType.ToString() == "FTP")
				{

					MoveFile(SaveEDIfile,SenderType.ToString());

				}
				break;

			case "OT":

				string checkPort = "";

					ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"OT"+TerArea+FileName.ToString()+".CSV", FileMode.Create, FileAccess.Write);
					writer = new StreamWriter (ostrm);
					Console.SetOut (writer);
					for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
					{

						if(EdiTB.Rows[checkdtb]["STATUS"].ToString() == "FG" || EdiTB.Rows[checkdtb]["STATUS"].ToString() == "MS")
						{

						Console.WriteLine(EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+","+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+","+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+","+EdiTB.Rows[checkdtb]["STATUS"].ToString()+","+EdiTB.Rows[checkdtb]["TERMIANL"].ToString()+","+EdiTB.Rows[checkdtb]["PORT"].ToString()+","+EdiTB.Rows[checkdtb]["TERMIANL"].ToString()+",,,"+EdiTB.Rows[checkdtb]["PORT"].ToString()+",,,,,,,"+EdiTB.Rows[checkdtb]["OWNER_STATUS"].ToString());
						
						}
						else
						{

						 Console.WriteLine(EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+","+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+","+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+","+EdiTB.Rows[checkdtb]["STATUS"].ToString()+","+EdiTB.Rows[checkdtb]["TERMIANL"].ToString()+","+EdiTB.Rows[checkdtb]["PORT"].ToString()+","+EdiTB.Rows[checkdtb]["DESTINATION"].ToString()+",,,"+EdiTB.Rows[checkdtb]["PORT"].ToString()+",,,,,,,"+EdiTB.Rows[checkdtb]["OWNER_STATUS"].ToString());
						
						}

					}
					
					Console.SetOut (oldOut);
					writer.Close();
					ostrm.Close();

					
			if(SenderType.ToString() == "EMAIL")
		     {
				send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
				MoveFile(SaveEDIfile,SenderType.ToString());
			 }
			else if(SenderType.ToString() == "FTP")
			{
				MoveFile(SaveEDIfile,SenderType.ToString());
			}


		break;
	
			case "LD":
EDIHeader =   "UNB+UNOA:2+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"COR"+Line+"LD"+TerArea+FileName.ToString()+"'\r\n"+
			  "UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN:ITG12'\r\n"+
			  "BGM+46+Discharge/Loading report+9'\r\n"+
			  "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+13+TSL:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
			  "LOC+9+THLCB:139:6+"+SenderID+":TER:ZZZ'\r\n"+
			  "NAD+CF+TSL'\r\n";

			  countSegment = countSegment+6;

			  ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"LD"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
			  writer = new StreamWriter (ostrm);
			  Console.SetOut (writer);

			  Console.Write(EDIHeader);

			  for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			  {
				
				Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
				countSegment++;

				Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
				countSegment++;
				
				Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+"'");
				countSegment++;

				Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"::5'");
				countSegment++;

				Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
				countSegment++;

				Console.WriteLine("MEA+AAE+T+KGM:"+EdiTB.Rows[checkdtb]["TARE_WEIGHT"].ToString()+"'");
				countSegment++;

				if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				{
					Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					countSegment++;
				}
				
				if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
				{
					Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
				    countSegment++;
				}

				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"+"+EdiTB.Rows[checkdtb]["UNDG"].ToString()+"'");
					countSegment++;
				}
				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("EQA+RG+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+"'");
					countSegment++;
				}
			  }
			  
				Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				countSegment++;
				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				Console.WriteLine("UNZ+1+"+"COR"+Line+"LD"+TerArea+FileName.ToString()+"'");
//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
	Console.SetOut (oldOut);
	writer.Close();
	ostrm.Close(); 
	countSegment =0;

	if(SenderType.ToString() == "EMAIL")
   {
		send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
		MoveFile(SaveEDIfile,SenderType.ToString());
	 }
	else if(SenderType.ToString() == "FTP")
	{
		MoveFile(SaveEDIfile,SenderType.ToString());
	}
			break;
			case "DG":
				EDIHeader = "UNB+UNOA:2+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"COR"+Line+"DG"+TerArea+FileName.ToString()+"'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN'\r\n"+
						    "BGM+44+Discharge/Loading report+9'\r\n"+
							"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+13+TSL:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
						  	"LOC+11+THLCB:139:6+"+SenderID+":TER:ZZZ'\r\n"+
						  	"NAD+CF+TSL'\r\n";

				countSegment = countSegment+6;
				ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"DG"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);
				Console.Write(EDIHeader);

			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			{
				Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
				countSegment++;
				
				if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
				{
					Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
					countSegment++;
				}

				Console.WriteLine("TMD+"+EdiTB.Rows[checkdtb]["CNTR_LOAD_STATUS_CODE"].ToString()+"'");
				countSegment++;
				
				Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
				countSegment++;
				Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
				countSegment++;
				
				Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
				countSegment++;

				Console.WriteLine("MEA+AAE+T+KGM:"+EdiTB.Rows[checkdtb]["TARE_WEIGHT"].ToString()+"'");
					countSegment++;

				if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				{
					Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					countSegment++;
				}

				if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
				{
					Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
				    countSegment++;
				}

				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"+"+EdiTB.Rows[checkdtb]["UNDG"].ToString()+"'");
					countSegment++;
					
				}
				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("EQA+RG+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+"'");
					countSegment++;
				}
			}
		Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
		countSegment++;
		Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
		Console.WriteLine("UNZ+1+"+"COR"+Line+"DG"+TerArea+FileName.ToString()+"'");
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;
	if(SenderType.ToString() == "EMAIL")
   {
		send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
		MoveFile(SaveEDIfile,SenderType.ToString());
	 }
	else if(SenderType.ToString() == "FTP")
	{
		MoveFile(SaveEDIfile,SenderType.ToString());
	}
			break;

			case "ST":


			ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"ST"+TerArea+FileName.ToString()+".CSV", FileMode.Create, FileAccess.Write);
			writer = new StreamWriter (ostrm);
			Console.SetOut (writer);
			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			{
				Console.WriteLine(EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+","+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+","+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+","+EdiTB.Rows[checkdtb]["STATUS"].ToString()+","+EdiTB.Rows[checkdtb]["TERMIANL"].ToString()+","+EdiTB.Rows[checkdtb]["PORT"].ToString()+","+EdiTB.Rows[checkdtb]["TERMIANL"].ToString()+",,,"+EdiTB.Rows[checkdtb]["PORT"].ToString()+",,,,,,,"+EdiTB.Rows[checkdtb]["OWNER_STATUS"].ToString());
			}
			
			Console.SetOut (oldOut);
			writer.Close();
			ostrm.Close();

			if(SenderType.ToString() == "EMAIL")
		     {
				send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
				MoveFile(SaveEDIfile,SenderType.ToString());
			 }
			else if(SenderType.ToString() == "FTP")
			{
				MoveFile(SaveEDIfile,SenderType.ToString());
			}

			break;


		}
	}

		public static void MoveFile(string PathFileMove, string sender)
		{

			if(sender.ToString() == "FTP")
			{
				DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
				DirectoryInfo to = new DirectoryInfo(@""+pathfile.FTP.ToString()+"\\TSL\\IO");
				DirectoryInfo to_csv = new DirectoryInfo(@""+pathfile.FTP.ToString()+"\\TSL\\CSV");
				foreach(FileInfo fi in from.GetFiles("*.EDI"))
				{
					 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
				}

				foreach(FileInfo fi in from.GetFiles("*.CSV"))
				{
					 fi.MoveTo(Path.Combine(to_csv.ToString(),fi.Name));
				}
			}
			else
			{
				DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
				DirectoryInfo to = new DirectoryInfo(@""+PathFileMove+"backup\\");
				DirectoryInfo to_csv = new DirectoryInfo(@""+PathFileMove+"backup\\");
				foreach(FileInfo fi in from.GetFiles("*.EDI"))
				{
					 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
				}

				foreach(FileInfo fi in from.GetFiles("*.CSV"))
				{
					 fi.MoveTo(Path.Combine(to_csv.ToString(),fi.Name));
				}

			}
			
		}

	}
