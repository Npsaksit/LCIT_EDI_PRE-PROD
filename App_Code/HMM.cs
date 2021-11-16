using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
	public class HMM
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
				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+CODHMMIN++CODECO'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
							"BGM+34+"+dt.ToString("yyyyMMddHH")+"+9'\r\n"+
							// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
							// "LOC+15+LCB05'\r\n"+
							// "LOC+9+THLCH:139:6'\r\n"+
							"NAD+MS+THLCH:160:184'\r\n";

							countSegment = countSegment+4;

							

// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
// -----------------For Content Query Data form EdiTB.Rows[checkdtb]["COLUMNS_NAME"]  -----------------------------

							ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"IN"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
							writer = new StreamWriter (ostrm);
							Console.SetOut (writer);
							Console.Write(EDIHeader);
							for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
							{
								Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
								// Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"++++"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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

								Console.WriteLine("LOC+165+THLCH:139:6'");
								countSegment++;

								if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
								{
									Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString().Replace("-","")+"'");
									countSegment++;
								}
								
								if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
								{
								Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
								countSegment++;
								}

								Console.WriteLine("NAD+CF+HMM:160:20'");
								countSegment++;
								
							}
	// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
				Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				countSegment++;

				Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				Console.WriteLine("UNZ+1+CODHMMIN'");
				Console.SetOut (oldOut);
				writer.Close();
				ostrm.Close(); 
				
				countSegment =0;
// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------


				if(SenderType.ToString() == "EMAIL")
				{
					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
					MoveFile(SaveEDIfile);
				}
				else if(SenderType.ToString() == "FTP")
				{
					MoveFile(SaveEDIfile);
				}
				break;

			case "OT":

				EDIHeader=	"UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+CODHMMOT++CODECO'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
							"BGM+36+"+dt.ToString("yyyyMMddHH").ToString()+"+9'\r\n"+
							"NAD+MS+THLCH:160:184'\r\n";
							// "LOC+15+LCB05'\r\n"+
							// "LOC+11+THLCH:139:6'\r\n"+
							// "NAD+CA+ZIM'\r\n";

							 countSegment = countSegment+4;

				ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"OT"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);

				Console.Write(EDIHeader);

				for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				{
					Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
					// Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"++++"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
					countSegment++;
					
					if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
					{
					Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					countSegment++;
					}
					// if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
					// {
					// 	Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
					// 	countSegment++;
					// }
					Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					countSegment++;
					Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+"'");
					countSegment++;
					
					Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+"'");
					countSegment++;

					Console.WriteLine("LOC+165+THLCH:139:6'");
					countSegment++;
					
					if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
					{
						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString().Replace("-","")+"'");
						countSegment++;
					}

					if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
					{
						Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
						countSegment++;
					}
							
					Console.WriteLine("NAD+CF+HMM:160:20'");
				    countSegment++;		
					
				}
		Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
		countSegment++;
		Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
		Console.WriteLine("UNZ+1+CODHMMOT'");
	//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;


				if(SenderType.ToString() == "EMAIL")
				{
					send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
					MoveFile(SaveEDIfile);
				}
				else if(SenderType.ToString() == "FTP")
				{
					MoveFile(SaveEDIfile);
				}
		break;
	
			case "LD":
				EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmm")+"DI++  COARRI'\r\n"+
							  "UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN:ITG12'\r\n"+
							  "BGM+46+"+dt.ToString("yyyyMMddHH")+"+9'\r\n"+
							  "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++HMM:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
							  "LOC+165+THLMC:139:6+LCIT'\r\n"+
							  "NAD+CA+HMM:172:166'\r\n"+
							  "NAD+MS+LCIT:160:184'\r\n";

							  countSegment = countSegment+7;

							  ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"LD"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
							  writer = new StreamWriter (ostrm);
							  Console.SetOut (writer);

							  Console.Write(EDIHeader);

							  for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
							  {
							    Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5+2+"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
								// Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+"+++"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
								countSegment++;
								if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
								{
									Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
								    countSegment++;
								}
								Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
								countSegment++;

								


								Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"::5'");
								countSegment++;

								Console.WriteLine("LOC+9+THLMC:139:6'");
								countSegment++;

								Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
								countSegment++;


								if(EdiTB.Rows[checkdtb]["VGM"].ToString() == "NOVGM")
								{
									Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
									countSegment++;
								}
								else
								{
									Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["VGM"].ToString()+"'");
									countSegment++;
								}


								// if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
								// {
								// 	Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString().Replace("-","")+"'");
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

									Console.WriteLine("NAD+CF+HMM:160:184'");
				    				countSegment++;	

							  }
								Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
								countSegment++;
								Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
								Console.WriteLine("UNZ+1+"+dt.ToString("yyyyMMddHHmm")+"'");
				//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
					Console.SetOut (oldOut);
					writer.Close();
					ostrm.Close(); 
					countSegment =0;

					if(SenderType.ToString() == "EMAIL")
				   {
						send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
						MoveFile(SaveEDIfile);
					 }
					else if(SenderType.ToString() == "FTP")
					{
						MoveFile(SaveEDIfile);
					}
							break;
			case "DG":
				EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+dt.ToString("yyyyMMddHHmm")+"DI++  COARRI'\r\n"+
							"UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN:ITG12'\r\n"+
						    "BGM+44+"+dt.ToString("yyyyMMddHH")+"+9'\r\n"+
							"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++HMM:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
							"LOC+165+THLMC:139:6+LCIT'\r\n"+
							"NAD+CA+HMM:172:166'\r\n"+
							"NAD+MS+LCIT:160:184'\r\n";
						

				countSegment = countSegment+7;
				ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"DG"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
				writer = new StreamWriter (ostrm);
				Console.SetOut (writer);
				Console.Write(EDIHeader);

			for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
			{
				 Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5+3+"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
				countSegment++;
				if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
				{
					Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					countSegment++;
				}
				if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
				{
					Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
					countSegment++;
				}
				Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
				countSegment++;

				Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"::5'");
				countSegment++;

				Console.WriteLine("LOC+11+THLMC:139:6'");
				countSegment++;

				Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
				countSegment++;
				
				if(EdiTB.Rows[checkdtb]["VGM"].ToString() == "NOVGM")
				{
					Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					countSegment++;
				}
				else
				{
					Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["VGM"].ToString()+"'");
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

				Console.WriteLine("NAD+CF+HMM:160:184'");
				countSegment++;	
			}
		Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
		countSegment++;
		Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
		Console.WriteLine("UNZ+1+"+dt.ToString("yyyyMMddHHmm")+"'");
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		countSegment =0;
	if(SenderType.ToString() == "EMAIL")
   {
		send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
		MoveFile(SaveEDIfile);
	 }
	else if(SenderType.ToString() == "FTP")
	{
		MoveFile(SaveEDIfile);
	}
			break;
		}
	}
		public static void MoveFile(string PathFileMove)
		{
			DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
			DirectoryInfo to = new DirectoryInfo(@""+PathFileMove+"backup\\");
				foreach(FileInfo fi in from.GetFiles("*.EDI"))
				{
					 fi.MoveTo(Path.Combine(to.ToString(),fi.Name));
				}
		}

	}
