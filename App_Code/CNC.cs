using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;  
using System.Collections.Generic;

	public class CNC 
	{
		static keepfile.flepath pathfile = new keepfile.flepath();
		static customer_mail send_mail = new customer_mail();
		static liner_profile.checkprofile LinerPF = new liner_profile.checkprofile();
		public static void  create_file(string Line, string Move, DateTime dt, DataTable EdiTB,String TerArea) 
	    {
		    	FileStream ostrm;
				 StreamWriter writer;
				 TextWriter oldOut = Console.Out;
				 // DirectoryInfo SaveXML = new DirectoryInfo(@"D:\\LCIT_EDI\FLEEDI\"+Line+"\\"+Move+"\\"+TerArea+"\\");
				 string  SaveEDIfile =pathfile.SaveEDI.ToString()+Line+"\\"+Move+"\\"+TerArea+"\\";
				 string  di = pathfile.MainDirectory.ToString();

				 string SenderID = "";
				 string ReceiveID ="";
				 string EDIHeader ="";
				 int countSegment =0;
				 string FileName= "";
				 string SenderType = "";
				 string terminal_sepa = "";

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


				  if(TerArea == "C3")
							{
								terminal_sepa = "THLCHDL"+TerArea;
							}
							else
							{
								terminal_sepa = "LCIT";
							}

			//--------------------------------- CHECK MOVEMENT BY SWITCH CASE ---------------------------------------------- 
						switch(Move)
						{
							//---------------------------- CREATE EDI FILE FOR GATE IN MOVEMENT ---------------------------------------
							case "IN":

							//-----------------------------------CREATE HEADER EDI FILE -----------------------------------------------
								EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"DI++CODECO'\r\n"+
				 					"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
				 					"BGM+34+CONTAINER GATE IN/OUT+9'\r\n"+
				 					// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
				 					"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++:103'\r\n"+
				 					"LOC+15+LCB05'\r\n"+
				 					"LOC+9+THLCH:139:6'\r\n"+
				 					"NAD+MS+LCIT'\r\n";

				 					countSegment = countSegment+7;

				 			// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
				    
				    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"IN"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
						    writer = new StreamWriter (ostrm);
					        Console.SetOut (writer);

					        Console.Write(EDIHeader);

					        	for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				 					{
					 						Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
					 						countSegment++;
					 						if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
					 						{
					 							Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
					 							countSegment++;
					 						}
					 						
					 						Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					 						countSegment++;
					 						// Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+"'");  CMA REQUEST REMOVE 2019 AUG 27
					 						// countSegment++;
					 						Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					 						countSegment++;

				 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				 						{
											Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					 						countSegment++;
				 						}		
					 						Console.WriteLine("FTX+AAI+++I'");
					 						countSegment++;

					 						if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
					 						{
					 							Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
					 						    countSegment++;
					 						}

					 						Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["IN_TRANSPORT_MODE_CODE"].ToString()+"+++++:::"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"'");
					 						countSegment++;
					 						Console.WriteLine("LOC+165+THLCH:139:6+THLCHDL"+EdiTB.Rows[checkdtb]["AREA_C"].ToString()+":TER:ZZZ'");
					 						countSegment++;

				 						if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != "NO")
				 						{
				 							// Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString()+"'");
				 							Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
				 							countSegment++;
				 						}	 						
				 					}

				 					// ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
				 			Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				 			countSegment++;
				 			Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				 			Console.WriteLine("UNZ+1+CODECO'");
					  	    		
				    		Console.SetOut (oldOut);
					    	writer.Close();
					   		ostrm.Close(); 

					   		// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------
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

							case "OT":
							//---------------------------- CREATE EDI FILE FOR GATE OUT MOVEMENT ---------------------------------------
						EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"LO++CODECO'\r\n"+
				 					"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
				 					"BGM+36+CONTAINER GATE IN/OUT+9'\r\n"+
				 					// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
				 					// "TDT+20++1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
				 					"TDT+20++1+++++:103'\r\n"+
				 					"LOC+15+LCB05'\r\n"+
				 					"LOC+11+THLCH:139:6'\r\n"+
				 					"NAD+MS+LCIT'\r\n";

				 					countSegment = countSegment+7;
				    
				    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"OT"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
						    writer = new StreamWriter (ostrm);
					        Console.SetOut (writer);

					        Console.Write(EDIHeader);

					        	for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				 					{
					 						Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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
					 						Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					 						countSegment++;
					 						Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+"'");
					 						countSegment++;
					 						Console.WriteLine("LOC+99+"+EdiTB.Rows[checkdtb]["DST_CODE"].ToString()+"'");
					 						countSegment++;
					 						Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					 						countSegment++;
				 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				 						{
					 						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CA'");
					 						countSegment++;
				 						}
					 						Console.WriteLine("FTX+AAI+++O'");
					 						countSegment++;

					 						if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
					 						{
					 							Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
					 						    countSegment++;
					 						}

					 						Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["OUT_TRANSPORT_MODE_CODE"].ToString()+"+++++:::"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"'");
					 						countSegment++;
					 						Console.WriteLine("LOC+165+THLCH:139:6+THLCHDL"+EdiTB.Rows[checkdtb]["AREA_C"].ToString()+":TER:ZZZ'");
					 						countSegment++;

				 						if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != "NO")
				 						{
				 							// Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString()+"'");
				 							Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
				 							countSegment++;
				 						}	 						
				 					}
				 			Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				 			countSegment++;
				 			Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				 			Console.WriteLine("UNZ+1+CODECO'");
					  	    		
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
							//---------------------------- CREATE EDI FILE FOR LOAD MOVEMENT ---------------------------------------
						EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"LO++COARRI'\r\n"+
				 					"UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN'\r\n"+
				 					"BGM+270+Discharge/Loading report+9'\r\n"+
				 					// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
				 					"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++:103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
				 					"LOC+9+THLCH:139:6+"+terminal_sepa+":TER:ZZZ'\r\n"+
				 					"NAD+MS+LCIT'\r\n";

				 					countSegment = countSegment+6;
				    
				    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"LD"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
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
					 						Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					 						countSegment++;
					 						// Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6+THLCHDL"+EdiTB.Rows[checkdtb]["AREA_C"].ToString()+":TER:ZZZ'");
					 						// countSegment++;
					 						Console.WriteLine("LOC+147+::5'");
					 						countSegment++;
					 						Console.WriteLine("LOC+5+THLCH::6'");
					 						countSegment++;
					 						Console.WriteLine("MEA+WT++KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					 						countSegment++;
				 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				 						{
					 						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
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
					   			send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
					   			MoveFile(SaveEDIfile);
					   		}
					   		else if(SenderType.ToString() == "FTP")
					   		{
					   			MoveFile(SaveEDIfile);
					   		}
							break;

							case "DG":
							//---------------------------- CREATE EDI FILE FOR DISCHARGE MOVEMENT ---------------------------------------
						EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"DI++COARRI'\r\n"+
				 					"UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN'\r\n"+
				 					"BGM+98+Discharge/Loading report+9'\r\n"+
				 					// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
				 					"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++:103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
				 					"LOC+11+THLCH:139:6+"+terminal_sepa+":TER:ZZZ'\r\n"+
				 					"NAD+MS+LCIT'\r\n";

				 					countSegment = countSegment+6;
				    
				    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"COR"+Line+"DG"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
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

					 							if(EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString() != "NOBL")
					 						{
					 									Console.WriteLine("RFF+BM:"+EdiTB.Rows[checkdtb]["MASTER_BOL_AN"].ToString()+"'");
						 					countSegment++;

					 						} 					
						 					
					 						Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					 						countSegment++;
					 						Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
					 						countSegment++;
					 						Console.WriteLine("LOC+147+::5'");
					 						countSegment++;
					 						Console.WriteLine("LOC+60+THLCH::6'");
					 						countSegment++;
					 						Console.WriteLine("MEA+WT++KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					 						countSegment++;
				 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
				 						{
					 						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
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
					   			send_mail.send_mail_control(Line.ToString(),Move.ToString(),TerArea.ToString());
					   			MoveFile(SaveEDIfile);
					   		}
					   		else if(SenderType.ToString() == "FTP")
					   		{
					   			MoveFile(SaveEDIfile);
					   		}
							break;


							case "ST":

								EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"ST++CODECO'\r\n"+
				 					"UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'\r\n"+
				 					"BGM+183+CONTAINER UNSUFFING+9'\r\n"+
				 					// "TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
				 					// "TDT+20++1+++++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103'\r\n"+
				 					"TDT+20++1+++++:103'\r\n"+
				 					"NAD+MS+LCIT'\r\n";

				 					countSegment = countSegment+5;
				    
				    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"COD"+Line+"ST"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
						    writer = new StreamWriter (ostrm);
					        Console.SetOut (writer);

					        Console.Write(EDIHeader);

					        	for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
				 					{
					 						Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++3+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
					 						countSegment++;

					 						if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
					 						{
					 									Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
						 					countSegment++;

					 						} 		

					 						Console.WriteLine("DTM+133:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
					 						countSegment++;

					 						Console.WriteLine("LOC+165+THLCH:139:6+THLCHDL"+EdiTB.Rows[checkdtb]["AREA_C"].ToString()+":TER:ZZZ'");
					 						countSegment++;
					 						
					 						Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
					 						countSegment++;
				 									
				 					}
				 			Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
				 			countSegment++;
				 			Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
				 			Console.WriteLine("UNZ+1+CODECO'");
					  	    		
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

						}			
	    }

	    public static void MoveFile(string PathFileMove)
	    {
	    	DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
	    	DirectoryInfo to = new DirectoryInfo(@""+PathFileMove+"backup\\");

	    		foreach(FileInfo fi in from.GetFiles("*.EDI"))
	    		{
	    			  fi.MoveTo(Path.Combine(to.ToString(),fi.Name));

	    			// System.IO.File.Move(from.ToString()+fi.Name.ToString(), to.ToString()+fi.Name.ToString());
	    			// System.IO.File.Copy(to.ToString(),fi.Name.ToString());
	    			 // File.Copy(from+fi.Name, to + Path.GetFileName(fi.Name));
	    			//  System.IO.File.Delete(from+fi.Name);
	    	} 
	    }
	}
