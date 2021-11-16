using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
	public class WHL
	{
		static keepfile.flepath pathfile = new keepfile.flepath();
		static customer_mail send_mail = new customer_mail();
		static customer_mail_by_type send_mail_by_type = new customer_mail_by_type();
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
				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"CODWHLI"+TerArea.ToString().Substring(1,1)+"++CODECO'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
							"BGM+34+"+dt.ToString("yyyyMMddHH")+"+9'\r\n"+
							"NAD+CF+WHL:160:184'\r\n"+
							"NAD+MS+THLCHTLCIT:160:184'\r\n";

							countSegment = countSegment+5;


// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
// -----------------For Content Query Data form EdiTB.Rows[checkdtb]["COLUMNS_NAME"]  -----------------------------

							ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"IN"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
							writer = new StreamWriter (ostrm);
							Console.SetOut (writer);
							Console.Write(EDIHeader);
							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
							{
								Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
								{
								Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
								countSegment++;
								}
								Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
								countSegment++;

								Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+"'");
								countSegment++;

								Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+"'");
								countSegment++;

								Console.WriteLine("LOC+8+"+EdiTB.Rows[checkdtb]["ORG"].ToString()+"'");
								countSegment++;

								if(EdiTB.Rows[checkdtb]["AREA_C"].ToString() == "B5")
								{
									Console.WriteLine("LOC+165+THLCH:139:6'");
									countSegment++;
								}

								if(EdiTB.Rows[checkdtb]["AREA_C"].ToString() == "C3")
								{
									Console.WriteLine("LOC+165+THLCH:139:6+LCH14:TER:ZZZ'");
									countSegment++;
								}

								Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
								{
									Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+SH'");
									countSegment++;
								}

								if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
								{
								Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
								countSegment++;
								}

							}
	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
				Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				countSegment++;

				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				Console.WriteLine("UNZ+1+CODWHLI"+TerArea.ToString().Substring(1,1)+"'");
				Console.SetOut (oldOut);
				writer.Close();
				ostrm.Close();

				countSegment =0;
// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------


				if(SenderType.ToString() == "EMAIL")
				{
					send_mail_by_type.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString(),Move.ToString());
					MoveFile(SaveEDIfile,SenderType.ToString());
				}
				else if(SenderType.ToString() == "FTP")
				{
					MoveFile(SaveEDIfile,SenderType.ToString());
				}
				break;


			case "OT":

				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"CODWHLO"+TerArea.ToString().Substring(1,1)+"++CODECO'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
							"BGM+36+"+dt.ToString("yyyyMMddHH")+"+9'\r\n"+
							"NAD+CF+WHL:160:184'\r\n"+
							"NAD+MS+THLCHTLCIT:160:184'\r\n";

							 countSegment = countSegment+5;

				ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"OT"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);

				Console.Write(EDIHeader);

				for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				{
					Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
					countSegment++;

					if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
					{
					Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					countSegment++;

					Console.WriteLine("RFF+RE:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					countSegment++;

					}
					if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
					{
						Console.WriteLine("RFF+CN:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
						countSegment++;
					}
					Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					countSegment++;
					Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+"'");
					countSegment++;

					Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+"'");
					countSegment++;

					Console.WriteLine("LOC+8'");
					countSegment++;

					if(EdiTB.Rows[checkdtb]["AREA_C"].ToString() == "B5")
					{
						Console.WriteLine("LOC+165+THLCH:139:6'");
						countSegment++;
					}

					if(EdiTB.Rows[checkdtb]["AREA_C"].ToString() == "C3")
					{
						Console.WriteLine("LOC+165+THLCH:139:6+LCH14:TER:ZZZ'");
						countSegment++;
					}

					Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					countSegment++;

					if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
					{
						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+SH'");
						countSegment++;
					}

				}
		Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
		countSegment++;
		Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
		Console.WriteLine("UNZ+1+CODWHLO"+TerArea.ToString().Substring(1,1)+"'");
	//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;


				if(SenderType.ToString() == "EMAIL")
				{
					send_mail_by_type.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString(),Move.ToString());
					MoveFile(SaveEDIfile,SenderType.ToString());
				}
				else if(SenderType.ToString() == "FTP")
				{
					MoveFile(SaveEDIfile,SenderType.ToString());
				}
		break;

			case "LD":

			string CheckLOC = "";

			if(EdiTB.Rows[0]["AREA_C"].ToString()== "B5")
			{
				CheckLOC = "LCH";
			}
			if(EdiTB.Rows[0]["AREA_C"].ToString()== "C3")
			{
				CheckLOC = "LCH14";
			}

EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"LO++COARRI'\r\n"+
			  "UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN'\r\n"+
			  "BGM+270+Discharge/Loading report+9'\r\n"+
			  "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
			  "LOC+9+THLCH:139:6+"+CheckLOC+":TER:ZZZ'\r\n"+
			  "NAD+MS+LCIT'\r\n";

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

				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("TMP+2+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+":CEL'");
					countSegment++;
				}

				if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				{
					Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					countSegment++;
				}

				// if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
				// {
				// 	Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
				//     countSegment++;
				// }

				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"+"+EdiTB.Rows[checkdtb]["UNDG"].ToString()+"'");
					countSegment++;
				}

			  }
				Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				countSegment++;
				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				Console.WriteLine("UNZ+1+COARRI'");
//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
	Console.SetOut (oldOut);
	writer.Close();
	ostrm.Close();
	countSegment =0;

	if(SenderType.ToString() == "EMAIL")
   {
		send_mail_by_type.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString(),Move.ToString());
		MoveFile(SaveEDIfile,SenderType.ToString());
	 }
	else if(SenderType.ToString() == "FTP")
	{
		MoveFile(SaveEDIfile,SenderType.ToString());
	}
			break;
			case "DG":

			string CheckLOC2 = "";

			if(EdiTB.Rows[0]["AREA_C"].ToString()== "B5")
			{
				CheckLOC2 = "LCH";
			}
			if(EdiTB.Rows[0]["AREA_C"].ToString()== "C3")
			{
				CheckLOC2 = "LCH14";
			}



				EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"DI++COARRI'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN'\r\n"+
						    "BGM+98+Discharge/Loading report+9'\r\n"+
							"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
							"LOC+11+THLCH:139:6+"+CheckLOC2+":TER:ZZZ'\r\n"+
							"NAD+MS+LCIT'\r\n";

				countSegment = countSegment+6;
				ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"DG"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);
				Console.Write(EDIHeader);

			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			{

				Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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

				if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
				{
					Console.WriteLine("TMP+2+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+":CEl'");
					countSegment++;
				}

				if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				{
					Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					countSegment++;
				}

				// if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
				// {
				// 	Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
				//     countSegment++;
				// }

				if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
				{
					Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"+"+EdiTB.Rows[checkdtb]["UNDG"].ToString()+"'");
					countSegment++;

				}

			}
		Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
		countSegment++;
		Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
		Console.WriteLine("UNZ+1+COARRI'");
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;

	if(SenderType.ToString() == "EMAIL")
   {
		send_mail_by_type.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString(),Move.ToString());
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
						DirectoryInfo to = new DirectoryInfo(@""+pathfile.FTP.ToString()+"\\WHL\\IO");
						foreach(FileInfo fi in from.GetFiles("*.EDI"))
						{
							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
						}
					}
					else
					{
						DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
						DirectoryInfo to = new DirectoryInfo(@""+PathFileMove+"backup\\");
						foreach(FileInfo fi in from.GetFiles("*.EDI"))
						{
							 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
						}
					}
		}
	}
