using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;  
using System.Collections.Generic;

	public class SKR 
	{
    static keepfile.flepath pathfile = new keepfile.flepath();
    static customer_mail send_mail = new customer_mail();
    static liner_profile.checkprofile LinerPF = new liner_profile.checkprofile();

    public static void create_file(string Line, string Move, DateTime dt, DataTable EdiTB,String TerArea) 
	    {
            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            // DirectoryInfo SaveXML = new DirectoryInfo(@"D:\\LCIT_EDI\FLEEDI\"+Line+"\\"+Move+"\\"+TerArea+"\\");
            string SaveEDIfile = pathfile.SaveEDI.ToString() + Line + "\\" + Move + "\\" + TerArea + "\\";
            string di = pathfile.MainDirectory.ToString();

            string SenderID = "";
            string ReceiveID = "";
            string EDIHeader = "";
            int countSegment =0;
            string FileName = "";
            string SenderType = "";

        //-------------------------------- CHECK DOCUMENT CUSTOMER PROFILR IN XML -----------------------------

            DataTable dt_profile = new DataTable();

            dt_profile = LinerPF.getLiner_profile(Line);
            FileName = String.Format("{0:D10}", Int32.Parse(dt_profile.Rows[0]["RUNNING_NUMBER"].ToString()));

            if (TerArea.ToString() == "B5")
            {
                SenderID = dt_profile.Rows[0]["SENDERID_B5"].ToString();
            }

            if (TerArea.ToString() == "C3")
            {
                SenderID = dt_profile.Rows[0]["SENDERID_C3"].ToString();
            }

            ReceiveID = dt_profile.Rows[0]["RECEIVERID"].ToString();
            SenderType = dt_profile.Rows[0]["SENDER_TYPE"].ToString();

        //--------------------------------- CHECK MOVEMENT BY SWITCH CASE ---------------------------------------------- 
        switch (Move)
			{
				//---------------------------- CREATE EDI FILE FOR GATE IN MOVEMENT ---------------------------------------
				case "IN":

				//-----------------------------------CREATE HEADER EDI FILE -----------------------------------------------
					EDIHeader = "UNB+UNOA:2+" + SenderID + "+" + ReceiveID + "+" + dt.ToString("yyMMdd") + ":" + dt.ToString("HHmm") + "+" + "CODECO" + FileName.ToString() + "'\r\n";
                        

                countSegment = countSegment+1;

	 			// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
	    
	    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"CODSKRIN"+TerArea+FileName.ToString()+".TXT", FileMode.Create,FileAccess.Write);
			    writer = new StreamWriter (ostrm);
		        Console.SetOut (writer);

		        Console.Write(EDIHeader);
		        

		        	for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
	 					{
	 							countSegment=1;
	 							Console.Write( 	"UNH+1+SKR" +  dt.ToString("yyMMdd") + ":D:95B:UN'\r\n" +
                         		"BGM+34+SKR" +  dt.ToString("yyMMdd") + "+9'\r\n" +
                         		"TDT+20+" + EdiTB.Rows[0]["VOYAGE_AN"].ToString() + "+1++:172:20+++" + EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString().Replace("-", "") + ":103::" + EdiTB.Rows[0]["VESSEL_NM_AN"].ToString() + "'\r\n" +
                         		 "REF+VON:"+ EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"'\r\nNAD+CA+SKR'\r\n");
                         		countSegment = countSegment+5;
                         		if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
                         		{

                         			 Console.Write("GID+1'\r\n");
                         			 countSegment++;
                         			 Console.Write("TMP+2+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+":CEL'\r\n");
                         			 countSegment++;
                         			 Console.Write("SGP+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"'\r\n");
                         			 countSegment++;

                         		}
                         		if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
		 						{
		 							Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"'");
			 						countSegment++;
		 						}
		 						Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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

		 						if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()=="4")
		 						{
		 								Console.WriteLine("FTX+IEP'");
		 								countSegment++;
		 						}

		 						 if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()=="5")
		 						{
		 								Console.WriteLine("FTX+IFE'");
		 								countSegment++;
		 						}

                                Console.WriteLine("LOC+11+" + EdiTB.Rows[checkdtb]["POD"].ToString() + ":139:6'");
                                countSegment++;

                                Console.WriteLine("LOC+165+THLCH:139:6'");
                                countSegment++;

                    Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
		 						countSegment++;

	 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
	 						{
								Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CU'");
		 						countSegment++;
	 						}			
		 						Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["IN_TRANSPORT_MODE_CODE"].ToString()+"+31++++:"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+":146:ZZZ'");
		 						countSegment++;
		 					

	 						if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != "NO")
	 						{
                                // Console.WriteLine("NAD+CN+" + EdiTB.Rows[checkdtb]["SHIPPER"].ToString().TrimEnd() + "'");
                                Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
                                countSegment++;
	 						}	 	

	 						Console.WriteLine("CNT+16:1'");
			                countSegment++;
			                Console.WriteLine("UNT+" + countSegment.ToString() + "+SKR" +dt.ToString("yyMMdd")+ "'");					
	 					}

                // ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
                
                Console.Write("UNZ+1+CODECO" + FileName.ToString() + "'");

                Console.SetOut (oldOut);
		    	writer.Close();
		   		ostrm.Close(); 

		   		// ---------------------------------------------- END OF CREATE EDI FILE PROCESS --------------------------------------------
		   		countSegment =0;


                if (SenderType.ToString() == "EMAIL")
                {
                    send_mail.send_mail_control(Line.ToString(), Move.ToString(), TerArea.ToString());
                    MoveFile(SaveEDIfile);
                }
                else if (SenderType.ToString() == "FTP")
                {
                    MoveFile(SaveEDIfile);
                }
                break;

				case "OT":
				//---------------------------- CREATE EDI FILE FOR GATE OUT MOVEMENT ---------------------------------------
			EDIHeader = "UNB+UNOA:2+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"CODECO"+FileName.ToString()+"'\r\n";
	 					

	 					countSegment = countSegment+1;
	    
	    	 	ostrm = new FileStream (SaveEDIfile.ToString()+"CODSKROT"+TerArea+FileName.ToString()+".TXT", FileMode.Create, FileAccess.Write);
			    writer = new StreamWriter (ostrm);
		        Console.SetOut (writer);

		        Console.Write(EDIHeader);

		        	for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
	 					{
	 							countSegment=1;
	 							Console.Write(	"UNH+1+SKR"+ dt.ToString("yyMMdd")+":D:95B:UN'\r\n"+
							 					"BGM+36+SKR"+ dt.ToString("yyMMdd")+"+9'\r\n"+
							 					"TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++:172:20+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString().Replace("-","")+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'\r\n"+
							 					 "REF+VON:"+ EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"'\r\nNAD+CA+SKR'\r\n");
	 							countSegment = countSegment+5;
	 							if(EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString() != "NOTEMP")
                         		{

                         			 Console.Write("GID+1'\r\n");
                         			 countSegment++;
                         			 Console.Write("TMP+2+"+EdiTB.Rows[checkdtb]["TEMPERATURE"].ToString()+":CEL'\r\n");
                         			 countSegment++;
                         			 Console.Write("SGP+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"'\r\n");
                         			 countSegment++;

                         		}

                         		if(EdiTB.Rows[checkdtb]["IMCO"].ToString() != "NODG")
		 						{
		 							Console.WriteLine("DGS+IMD+"+EdiTB.Rows[checkdtb]["IMCO"].ToString()+"'");
			 						countSegment++;
		 						}
		 						Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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

								if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()=="4")
		 						{
		 								Console.WriteLine("FTX+OEP'");
		 								countSegment++;
		 						}
		 						
		 						 if(EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()=="5")
		 						{
		 								Console.WriteLine("FTX+OFD'");
		 								countSegment++;
		 						}

		 						Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
		 						countSegment++;

                                  Console.WriteLine("LOC+165+THLCH:139:6'");
                                countSegment++;

                    Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
		 						countSegment++;
	 						if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
	 						{
		 						Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"+CU'");
		 						countSegment++;
	 						}	
		 						Console.WriteLine("TDT+1++"+EdiTB.Rows[checkdtb]["OUT_TRANSPORT_MODE_CODE"].ToString()+"+31++++"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+":146:ZZZ'");
		 						countSegment++;
		 						

	 						if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != " ")
	 						{
                                // Console.WriteLine("NAD+CN+" + EdiTB.Rows[checkdtb]["SHIPPER"].ToString().TrimEnd()+ "'");
                                Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
                                countSegment++;
	 						}	 

	 						Console.WriteLine("CNT+16:1'");
				 			countSegment++;
				 			Console.WriteLine("UNT+" + countSegment.ToString() + "+SKR" +dt.ToString("yyMMdd")+ "'");						
	 					}
	 			
	 			Console.Write("UNZ+1+CODECO"+FileName.ToString()+"'");
		  	    		
		  	    		//------------------------------------------ END OF CREATE EDI FILE PROCESS ---------------------------------------------------------
	    		Console.SetOut (oldOut);
		    	writer.Close();
		   		ostrm.Close(); 
		   		countSegment =0;


                if (SenderType.ToString() == "EMAIL")
                {
                    send_mail.send_mail_control(Line.ToString(), Move.ToString(), TerArea.ToString());
                    MoveFile(SaveEDIfile);
                }
                else if (SenderType.ToString() == "FTP")
                {
                    MoveFile(SaveEDIfile);
                }
                break;
			}	 
	    }
    public static void MoveFile(string PathFileMove)
    {
        DirectoryInfo from = new DirectoryInfo(@"" + PathFileMove);
        DirectoryInfo to = new DirectoryInfo(@"" + PathFileMove + "backup\\");

        foreach (FileInfo fi in from.GetFiles("*.TXT"))
        {
            fi.MoveTo(Path.Combine(to.ToString(), fi.Name));

            // System.IO.File.Move(from.ToString()+fi.Name.ToString(), to.ToString()+fi.Name.ToString());
            // System.IO.File.Copy(to.ToString(),fi.Name.ToString());
            // File.Copy(from+fi.Name, to + Path.GetFileName(fi.Name));
            // System.IO.File.Delete(from+fi.Name);
        }
    }
}
