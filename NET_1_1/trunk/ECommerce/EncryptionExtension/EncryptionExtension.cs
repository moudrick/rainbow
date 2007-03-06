using System;
using System.Web.Services.Protocols;
using System.IO;
using System.Text;
using System.Xml;

namespace WebServiceHeaderExtension
{
	/// <summary>
	/// Summary description for WebServcSoapExt.
	/// </summary>
	
	public class EncryptionExtension : SoapExtension
	{
		private Stream inwardStream;
		private Stream outwardStream;
		public override Stream ChainStream(Stream stream)
		{
			outwardStream = stream;
			inwardStream = new MemoryStream();
			return inwardStream;
		}

		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		public override object GetInitializer(LogicalMethodInfo methodInfo,SoapExtensionAttribute attribute)
		{
			return null;
		}

		public override void Initialize(object initializer)
		{
			return;
		}

		public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
		{
			string soapMsg1;
			StreamReader readStr;
			StreamWriter writeStr;
			XmlDocument xDoc = new XmlDocument();
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeSerialize:
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
				case SoapMessageStage.BeforeDeserialize:
					readStr = new StreamReader(outwardStream);
					writeStr = new StreamWriter(inwardStream);
					soapMsg1 = readStr.ReadToEnd();
					//soapMsg2 = writeStr.ReadToEnd();
					if ( message is System.Web.Services.Protocols.SoapClientMessage)
					{
						// this is called at client side
						xDoc.LoadXml(soapMsg1);

						//						XmlNodeList xSiteID = xDoc.GetElementsByTagName("siteID");
						//						xSiteID[0].InnerXml = decrypt(xSiteID[0].InnerXml);
						//
						//						XmlNodeList xSitePwd = xDoc.GetElementsByTagName("sitePwd");
						//						xSitePwd[0].InnerXml = decrypt(xSitePwd[0].InnerXml);

						XmlNodeList xResult = xDoc.GetElementsByTagName("AuthenticateResult");
						xResult[0].InnerXml = decrypt(xResult[0].InnerXml);

					}
					else if( message is System.Web.Services.Protocols.SoapServerMessage)
					{
						// this is called at server side
						xDoc.LoadXml(soapMsg1);
						
						XmlNodeList xSiteID = xDoc.GetElementsByTagName("siteID");
						xSiteID[0].InnerXml = decrypt(xSiteID[0].InnerXml);

						XmlNodeList xSitePwd = xDoc.GetElementsByTagName("sitePwd");
						xSitePwd[0].InnerXml = decrypt(xSitePwd[0].InnerXml);

						XmlNodeList xUserID = xDoc.GetElementsByTagName("UserID");
						xUserID[0].InnerXml = decrypt(xUserID[0].InnerXml);

						XmlNodeList xPwd = xDoc.GetElementsByTagName("Password");
						xPwd[0].InnerXml = decrypt(xPwd[0].InnerXml);
					}
					
					soapMsg1 = xDoc.InnerXml;
					writeStr.Write(soapMsg1);
					writeStr.Flush();
					inwardStream.Position = 0;
					break;
				case SoapMessageStage.AfterSerialize:
					inwardStream.Position = 0;
					readStr = new StreamReader(inwardStream);
					writeStr = new StreamWriter(outwardStream);
					soapMsg1 = readStr.ReadToEnd();
					//soapMsg2 = writeStr.ReadToEnd();
					if ( message is System.Web.Services.Protocols.SoapClientMessage)
					{
						// this is called at client side
						xDoc.LoadXml(soapMsg1);
						
						XmlNodeList xSiteID = xDoc.GetElementsByTagName("siteID");
						xSiteID[0].InnerXml = encrypt(xSiteID[0].InnerXml);

						XmlNodeList xSitePwd = xDoc.GetElementsByTagName("sitePwd");
						xSitePwd[0].InnerXml = encrypt(xSitePwd[0].InnerXml);

						XmlNodeList xUserID = xDoc.GetElementsByTagName("UserID");
						xUserID[0].InnerXml = encrypt(xUserID[0].InnerXml);

						XmlNodeList xPwd = xDoc.GetElementsByTagName("Password");
						xPwd[0].InnerXml = encrypt(xPwd[0].InnerXml);

					}
					else if( message is System.Web.Services.Protocols.SoapServerMessage)
					{			
						// this is called at server side
						xDoc.LoadXml(soapMsg1);
												
						//						XmlNodeList xSiteID = xDoc.GetElementsByTagName("siteID");
						//						xSiteID[0].InnerXml = encrypt(xSiteID[0].InnerXml);
						//
						//						XmlNodeList xSitePwd = xDoc.GetElementsByTagName("sitePwd");
						//						xSitePwd[0].InnerXml = encrypt(xSitePwd[0].InnerXml);

						XmlNodeList xResult = xDoc.GetElementsByTagName("AuthenticateResult");
						xResult[0].InnerXml = encrypt(xResult[0].InnerXml);
					}
					
					soapMsg1 = xDoc.InnerXml;
					writeStr.Write(soapMsg1);
					writeStr.Flush();
					break;
			}
		} 

		private string encrypt(string message)
		{
			EncryptClass ec = new EncryptClass();
			string encryStr = ec.custEncrypt(message);
			return encryStr;
		}

		private string decrypt(string message)
		{
			EncryptClass ec = new EncryptClass();
			string decryptStr = message;
			return ec.custDecrypt(decryptStr);			
		}
	}

}
