using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
  public class ONE
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
      //string[] MyVessel = EdiTB.Rows[0]["VESSEL_NM_AN"].ToString().Split(new char[0]);

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
        EDIHeader=  "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"CODECO'\r\n";
              

              countSegment = countSegment+1;


// ----------------------------------- CREATE BODY EDI CODECO BY CUSTOMER FORMAT -----------------------------
// -----------------For Content Query Data form EdiTB.Rows[checkdtb]["COLUMNS_NAME"]  -----------------------------

              ostrm = new FileStream (SaveEDIfile.ToString()+"ONE_CODECO_"+dt.ToString("yyyyMMddHHmmss")+"."+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
              writer = new StreamWriter (ostrm);
              Console.SetOut (writer);
              Console.Write(EDIHeader);
              for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
              {

                countSegment=1;
                
                Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN:ITG14'");
                countSegment++;

                Console.WriteLine("BGM+34+"+dt.ToString("yyyyMMddHH")+"+9'");
                countSegment++;

                Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++ONE:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
                countSegment++;

                Console.WriteLine("LOC+9+THLCH:139:6+LCIT:BER:ZZZ'");
                countSegment++;

                Console.WriteLine("NAD+CF+ONEY:172:166'");
                countSegment++;

                if(TerArea.ToString() == "B5")
                  {
                    Console.WriteLine("NAD+MS+LCIT1::ZZZ'");
                    countSegment++;
                  }
                  
                  if(TerArea.ToString() == "C3")
                  {
                    Console.WriteLine("NAD+MS+LCIT2::ZZZ'");
                    countSegment++;
                  }

                Console.WriteLine("GID+1'");
                countSegment++;

                Console.Write("SGP+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+":23'\r\n");
                            countSegment++;

                // Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
                Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5+2+"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
                countSegment++;
                if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
                {
                Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
                countSegment++;
                }
                Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
                countSegment++;


                Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
                countSegment++;

                Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
                countSegment++;

                Console.WriteLine("LOC+165+"+EdiTB.Rows[checkdtb]["ORG"].ToString()+":139:6+LCIT:TER:ZZZ'");
                countSegment++;

                Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
                countSegment++;
                if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
                {
                  Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
                  countSegment++;
                }
                
                // if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
                // {
                // Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
                // countSegment++;
                // }
                
                Console.WriteLine("TDT+1+"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"+"+EdiTB.Rows[checkdtb]["IN_TRANSPORT_MODE_CODE"].ToString()+"'");
                countSegment++;
                

                Console.WriteLine("CNT+16:1'");
                countSegment++;

                Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
                
                
                // if(EdiTB.Rows[checkdtb]["SHIPPER"].ToString() != "NO")
                // {
                // Console.WriteLine("NAD+CN+"+EdiTB.Rows[checkdtb]["SHIPPER"].ToString().Replace("'","")+"'");
                // countSegment++;
                // }
                
              }
  // ------------------------------------ CREATE FOOTER IN EDI FILE ----------------------------------------------------
        
        Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+CODECO'");
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

        EDIHeader=  "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"CODECO'\r\n";
              

               countSegment = countSegment+1;

        ostrm = new FileStream (SaveEDIfile.ToString()+"ONE_CODECO_"+dt.ToString("yyyyMMddHHmmss")+"."+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
        writer = new StreamWriter (ostrm);
        Console.SetOut (writer);

        Console.Write(EDIHeader);

        for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
        {
          countSegment=1;

          Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN:ITG14'");
          countSegment++;

          Console.WriteLine("BGM+36+"+dt.ToString("yyyyMMddHH")+"+9'");
          countSegment++;

          Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++ONE:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
          countSegment++;

          Console.WriteLine("LOC+11+THLCH:139:6+LCIT:BER:ZZZ'");
          countSegment++;

          
          
          Console.WriteLine("NAD+CF+ONEY:172:166'");
          countSegment++;

          if(TerArea.ToString() == "B5")
          {
            Console.WriteLine("NAD+MS+LCIT1::ZZZ'");
            countSegment++;
          }
          
          if(TerArea.ToString() == "C3")
          {
            Console.WriteLine("NAD+MS+LCIT2::ZZZ'");
            countSegment++;
          }

          // Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
          // countSegment++;

          Console.WriteLine("GID+1'");
          countSegment++;

          Console.WriteLine("SGP+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+":23'");
                countSegment++;

          // Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+++"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
          Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5+2+"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
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

          Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
          countSegment++;

          Console.WriteLine("LOC+165+:139:6+LCIT:TER:ZZZ'");
          countSegment++;

          if(EdiTB.Rows[checkdtb]["VGM_GROSS_WEIGHT"].ToString() != "0")
          {
            Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["VGM_GROSS_WEIGHT"].ToString()+"'");
            countSegment++;
          }

          // Console.WriteLine("MEA+AAE+VGM+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
          // countSegment++;
          
          if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
          {
            Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
            countSegment++;
          }
          
          // if(EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()!=" ")
          // {
          //  Console.WriteLine("DAM+"+EdiTB.Rows[checkdtb]["DAMAGE_SEGMENT_AN"].ToString()+"'");
          //  countSegment++;
          // }
          
          Console.WriteLine("TDT+1+"+EdiTB.Rows[checkdtb]["INLAND_CARR_TP_MEAN_CODE"]+"+"+EdiTB.Rows[checkdtb]["OUT_TRANSPORT_MODE_CODE"].ToString()+"'");
          countSegment++;
          
          
          Console.WriteLine("CNT+16:1'");
          countSegment++;

          Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");

        }
    
    Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+CODECO'");
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
  
      case "LD":
EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"COARRI'\r\n";
        

        countSegment = countSegment+1;

        ostrm = new FileStream (SaveEDIfile.ToString()+"ONE_COARRI_"+dt.ToString("yyyyMMddHHmmss")+"."+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
        writer = new StreamWriter (ostrm);
        Console.SetOut (writer);

        Console.Write(EDIHeader);

        for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
        {
          countSegment=1;

          Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN:ITG12'");
          countSegment++;

          Console.WriteLine("BGM+46+"+dt.ToString("yyyyMMddHH")+"+9'");
          countSegment++;

          Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++ONE:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
          countSegment++;

        Console.WriteLine("LOC+9+THLCH:139'");
          countSegment++;

          Console.WriteLine("NAD+CF+ONEY:172:166'");
          countSegment++;

          if(TerArea.ToString() == "B5")
          {
            Console.WriteLine("NAD+MS+LCIT1::ZZZ'");
            countSegment++;
          }
          
          if(TerArea.ToString() == "C3")
          {
            Console.WriteLine("NAD+MS+LCIT2::ZZZ'");
            countSegment++;
          }

        Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5+2+"+EdiTB.Rows[checkdtb]["EQP_STATUS_CODE"].ToString()+"+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
        countSegment++;

        if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
        {
          Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
            countSegment++;
        }
        Console.WriteLine("DTM+203:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
        countSegment++;

        Console.WriteLine("LOC+165+THLCH:139:6+LCIT:BER:ZZZ'");
        countSegment++;

        Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
        countSegment++;

        Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
        countSegment++;

        Console.WriteLine("LOC+147+"+EdiTB.Rows[checkdtb]["BAY_POSN_AN"].ToString()+"::5'");
        countSegment++;
        
        Console.WriteLine("MEA+AAE+EGW+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
        countSegment++;

        // if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
        // {
        //  Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
        //  countSegment++;
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

        Console.WriteLine("CNT+16:1'");
        countSegment++;
        Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
        }
        
        
        Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+COARRI'");
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
        EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"COARRI'\r\n";
              
        countSegment = countSegment+1;

        ostrm = new FileStream (SaveEDIfile.ToString()+"ONE_COARRI_"+dt.ToString("yyyyMMddHHmmss")+"."+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
        writer = new StreamWriter (ostrm);
        Console.SetOut (writer);
        Console.Write(EDIHeader);

      for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
      {
        countSegment=1;

        Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+COARRI:D:95B:UN:ITG12'");
        countSegment++;

          Console.WriteLine("BGM+44+"+dt.ToString("yyyyMMddHH")+"+9'");
          countSegment++;

        Console.WriteLine("TDT+20+"+EdiTB.Rows[0]["VOYAGE_AN"].ToString()+"+1++ONE:172:166+++"+EdiTB.Rows[0]["VISIT_VSL_CALL_SIGN_C"].ToString()+":103::"+EdiTB.Rows[0]["VESSEL_NM_AN"].ToString()+"'");
        countSegment++;
          
        Console.WriteLine("LOC+11+THLCH:139'");
        countSegment++;

        Console.WriteLine("NAD+CF+ONEY:172:166'");
        countSegment++;

        if(TerArea.ToString() == "B5")
        {
          Console.WriteLine("NAD+MS+LCIT1::ZZZ'");
          countSegment++;
        }
        
        if(TerArea.ToString() == "C3")
        {
          Console.WriteLine("NAD+MS+LCIT2::ZZZ'");
          countSegment++;
        }


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

        Console.WriteLine("LOC+165+THLCH:139:6+LCIT:BER:ZZZ'");
        countSegment++;

        Console.WriteLine("LOC+9+"+EdiTB.Rows[checkdtb]["POL"].ToString()+":139:6'");
        countSegment++;

        Console.WriteLine("LOC+11+"+EdiTB.Rows[checkdtb]["POD"].ToString()+":139:6'");
        countSegment++;

        

        Console.WriteLine("MEA+AAE+EGW+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
        countSegment++;

        // if(EdiTB.Rows[checkdtb]["SEAL"].ToString()!="NOSEAL")
        // {
        //  Console.WriteLine("SEL+"+EdiTB.Rows[checkdtb]["SEAL"].ToString()+"'");
        //  countSegment++;
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

        Console.WriteLine("CNT+16:1'");
        countSegment++;
        Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");

      }
    
    Console.WriteLine("UNZ+"+EdiTB.Rows.Count+"+COARRI'");
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

       string NADseg = "";

       if(TerArea.ToString() == "B5")
       {
         NADseg = "LCIT1";

       }
        else if(TerArea.ToString() == "C3")
       {
         NADseg = "LCIT2";
         
       }

                EDIHeader = "UNB+UNOA:1+"+SenderID+"+"+ReceiveID+"+"+dt.ToString("yyMMdd")+":"+dt.ToString("HHmm")+"+"+"ST++CODECO'\r\n";
                  

                  countSegment = countSegment+1;
            
                ostrm = new FileStream (SaveEDIfile.ToString()+"ONE_CODECO_"+Line+"ST_"+TerArea+FileName.ToString()+".EDI", FileMode.Create, FileAccess.Write);
                writer = new StreamWriter (ostrm);
                  Console.SetOut (writer);

                  Console.Write(EDIHeader);

                    for(int checkdtb =0 ;checkdtb < EdiTB.Rows.Count; checkdtb++)
                  {
                    Console.WriteLine("UNH+"+dt.ToString("yyyyMMddHH")+"+CODECO:D:95B:UN'");
                     countSegment++;

                    Console.WriteLine("BGM+34+CONTAINER UNSUFFING+9'");
                    countSegment++;

                    Console.WriteLine("TDT+20++1+++++:103'");
                    countSegment++;

                    Console.WriteLine("NAD+MS+"+NADseg+"'");
                    countSegment++;

                      Console.WriteLine("EQD+CN+"+EdiTB.Rows[checkdtb]["CNTR_AN"].ToString()+"+"+EdiTB.Rows[checkdtb]["CONTAINER_TYPE_CODE"].ToString()+":102:5++3+"+EdiTB.Rows[checkdtb]["LADEN_INDICATOR_AN"].ToString()+"'");
                      countSegment++;

                      if(EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString() != "NOBOOKING")
                      {
                            Console.WriteLine("RFF+BN:"+EdiTB.Rows[checkdtb]["BOOKING_NO_AN"].ToString()+"'");
                      countSegment++;

                      }     

                      Console.WriteLine("DTM+7:"+EdiTB.Rows[checkdtb]["ACTIVITY_TM"].ToString()+":203'");
                      countSegment++;

                      Console.WriteLine("LOC+165+THLCH:139:6+LCIT:TER:ZZZ'");
                      countSegment++;
                      
                      Console.WriteLine("MEA+AAE+G+KGM:"+EdiTB.Rows[checkdtb]["GWEIGHT"].ToString()+"'");
                      countSegment++;

                      Console.WriteLine("CNT+16:"+EdiTB.Rows.Count+"'");
                      countSegment++;

                      Console.WriteLine("UNT+"+countSegment.ToString()+"+"+dt.ToString("yyyyMMddHH")+"'");
                          
                  }
              
              Console.WriteLine("UNZ+1+CODECO'");
                      
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
    }
  }
    public static void MoveFile(string PathFileMove, string sender)
    {
          if(sender.ToString() == "FTP")
          {
            DirectoryInfo from = new DirectoryInfo(@""+PathFileMove);
            DirectoryInfo to = new DirectoryInfo(@""+pathfile.FTP.ToString()+"\\ONE\\IO");
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
