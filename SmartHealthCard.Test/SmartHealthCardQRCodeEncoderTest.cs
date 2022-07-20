﻿using Hl7.Fhir.Model;
using SmartHealthCard.Test.Model;
using SmartHealthCard.Test.Serializers;
using SmartHealthCard.Test.Support;
using SmartHealthCard.Token;
using SmartHealthCard.Token.Model.Shc;
using SmartHealthCard.QRCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SmartHealthCard.Token.Providers;
using System.Runtime.Versioning;
using SkiaSharp;

namespace SmartHealthCard.Test
{
  public class SmartHealthCardQRCodeEncoderTest
  {    
    [Fact]
    public async void Encode_QRCode()
    {
      //### Prepare ######################################################

      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      //Bundle FhirBundleResource = FhirDataSupport.GetCovid19DetectedFhirBundleExample();
      //Bundle FhirBundleResource = FhirDataSupport.GetINRFhirBundleExample(ReleaseDate.Date, 12, 1.2m );
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19NotDetectedFhirBundleExample();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);
      //File.WriteAllText(@$"C:\Temp\SMARTHealthCard\Output\FHIRBundle.json", FhirBundleJson);

      //The base of the URL where a validator will retie the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new("https://localhost:44306/Smart-health-card");

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
      // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new()
      {
        VerifiableCredentialType.HealthCard,
        VerifiableCredentialType.Covid19,
        VerifiableCredentialType.Laboratory
      };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardToEncode = new(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new();      
      
      //### Act ##########################################################

      //Get the Smart Health Card retrieve Token 
      string SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardToEncode);

      //Create list of QR Codes
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeFactory = new();
      List<SKBitmap> QRCodeImageList = SmartHealthCardQRCodeFactory.GetQRCodeList(SmartHealthCardJwsToken);


      //Write out QR Code to file
      //for (int i = 0; i < QRCodeImageList.Count; i++)
      //{
      //  using SKData data = QRCodeImageList[i].Encode(SKEncodedImageFormat.Png, 90);
      //  using FileStream stream = File.OpenWrite(@$"C:\Temp\SMARTHealthCard\Output\QRCode-{i}.png");
      //  data.SaveTo(stream);
      //}

      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(SmartHealthCardJwsToken));
      Assert.NotNull(QRCodeImageList);
      Assert.Single(QRCodeImageList);

    }

    
    [Fact]
    public async void Decode_QRCodeRawData()
    {
      //### Prepare ######################################################

      //Get the ECC certificate from the Cert and Private key PEM files
      X509Certificate2 Certificate = CertificateSupport.GetCertificateFromPemFiles();
      //X509Certificate2 Certificate = CertificateSupport.GetCertificate(CertificateSupport.TestingThumbprint);

      //The Version of FHIR in use
      string FhirVersion = "4.0.1";

      //Get FHIR bundle
      //Bundle FhirBundleResource = FhirDataSupport.GetCovid19DetectedFhirBundleExample();
      Bundle FhirBundleResource = FhirDataSupport.GetCovid19NotDetectedFhirBundleExample();
      string FhirBundleJson = FhirSerializer.SerializeToJson(FhirBundleResource);
      
      //File.WriteAllText(@$"C:\Temp\SMARTHealthCard\Output\FHIRBundle.json", FhirBundleJson);

      //The base of the URL where a validator will retie the public keys from (e.g : [Issuer]/.well-known/jwks.json) 
      Uri Issuer = new("https://e1414486fce0.ngrok.io");

      //When the Smart Health Card became valid, the from date.
      DateTimeOffset IssuanceDateTimeOffset = DateTimeOffset.Now.AddMinutes(-1);

      //The Uri for the type of VerifiableCredentials
      // Uri VerifiableCredentialType = new Uri("https://smarthealth.cards#covid19");
      List<VerifiableCredentialType> VerifiableCredentialTypeList = new() 
      { 
        VerifiableCredentialType.HealthCard, 
        VerifiableCredentialType.Covid19 
      };

      //Create the SmartHealthCardModel
      SmartHealthCardModel SmartHealthCardModel = new(Issuer, IssuanceDateTimeOffset,
          new VerifiableCredential(VerifiableCredentialTypeList,
            new CredentialSubject(FhirVersion, FhirBundleJson)));

      //Instantiate the SmartHealthCard Encoder
      SmartHealthCardEncoder SmartHealthCardEncoder = new();      

      //### Act ##########################################################

      //Get the Smart Health Card retrieve Token 
      string SmartHealthCardJwsToken = await SmartHealthCardEncoder.GetTokenAsync(Certificate, SmartHealthCardModel);
      //File.WriteAllText(@$"C:\Temp\SMARTHealthCard\Output\JwsToken.txt", SmartHealthCardJwsToken);

      //Create list of QR Codes
      SmartHealthCardQRCodeEncoder SmartHealthCardQRCodeEncoder = new();
      List<string> QRCodeRawDataList = SmartHealthCardQRCodeEncoder.GetQRCodeRawDataList(SmartHealthCardJwsToken);

      //Write out Raw QR Code data to file
      //for (int i = 0; i < QRCodeRawDataList.Count; i++)
      //{
      //  File.WriteAllText(@$"C:\Temp\SMARTHealthCard\Output\RawQRCodeData-{i}.txt", QRCodeRawDataList[i]);
      //}

      SmartHealthCardQRCodeDecoder SmartHealthCardQRCodeDecoder = new();
      string JWS = SmartHealthCardQRCodeDecoder.GetToken(QRCodeRawDataList);

      //Create list of QR Codes
      
#pragma warning disable IDE0059 // Unnecessary assignment of a value
      List<SKBitmap> QRCodeImageList = SmartHealthCardQRCodeEncoder.GetQRCodeList(SmartHealthCardJwsToken);
#pragma warning restore IDE0059 // Unnecessary assignment of a value

      //Write out QR Code Image to files
      //for (int i = 0; i < QRCodeImageList.Count; i++)
      //{
      //  using SKData data = QRCodeImageList[i].Encode(SKEncodedImageFormat.Png, 90);
      //  using FileStream stream = File.OpenWrite(@$"C:\Temp\SMARTHealthCard\Output\QRCode-{i}.png");
      //  data.SaveTo(stream);
      //}
     

      //This testing JwksSupport class provides us with a mocked IJwksProvider that will inject the JWKS file
      //rather than make the HTTP call to go get it from a public endpoint.
      IJwksProvider MockedIJwksProvider = JwksSupport.GetMockedIJwksProvider(Certificate, Issuer);
      SmartHealthCardDecoder SmartHealthCardDecoder = new(MockedIJwksProvider);
      SmartHealthCardModel = await SmartHealthCardDecoder.DecodeAsync(JWS, true);
      //### Assert #######################################################

      Assert.True(!string.IsNullOrWhiteSpace(JWS));
      Assert.Equal(3, JWS.Split('.').Length);
      Assert.NotNull(SmartHealthCardModel);

    }

  }
}
