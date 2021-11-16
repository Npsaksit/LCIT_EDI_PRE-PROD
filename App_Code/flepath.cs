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

namespace keepfile
{
	public class flepath
	{
		public DirectoryInfo QuerySQL = new DirectoryInfo(@"D:\LCIT_EDI_TEST\QueryScript\");
		public DirectoryInfo QuerySQL_Resend = new DirectoryInfo(@"D:\LCIT_EDI_TEST\QueryScript _ManualCNTR\");
		public DirectoryInfo SaveEDI = new DirectoryInfo(@"D:\LCIT_EDI_TEST\FLEEDI\");
		public DirectoryInfo MainDirectory = new DirectoryInfo(@"D:\LCIT_EDI_TEST\");
		public DirectoryInfo FTP = new DirectoryInfo(@"D:\LCIT_EDI_TEST\FTP\");
		public DirectoryInfo GenerateFile = new DirectoryInfo(@"D:\LCIT_EDI_TEST\App_Code\");


	}

}