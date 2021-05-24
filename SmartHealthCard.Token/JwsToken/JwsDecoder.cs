using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Model.Jws;
using SmartHealthCard.Token.Serializers.Jws;
using System;

namespace SmartHealthCard.Token.JwsToken
{

  public sealed class JwsDecoder
  {
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private readonly IJwsSignatureValidator? JwsSignatureValidator;
    private readonly IJwsPayloadValidator? JwsPayloadValidator;
    private readonly IJwsHeaderValidator? JwsHeaderValidator;
    public JwsDecoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }

    public JwsDecoder(IJwsHeaderSerializer HeaderSerializer, IJwsPayloadSerializer PayloadSerializer, IJwsSignatureValidator? IJwsSignatureValidator, IJwsHeaderValidator? JwsHeaderValidator, IJwsPayloadValidator? IJwsPayloadValidator)
      : this(HeaderSerializer, PayloadSerializer)
    {
      this.JwsSignatureValidator = IJwsSignatureValidator;
      this.JwsHeaderValidator = JwsHeaderValidator;
      this.JwsPayloadValidator = IJwsPayloadValidator;
    }

    public PayloadType DecodePayload<HeaderType, PayloadType>(string Token, bool Verity = false)
    {      
      if (Verity)
      {
        if (this.JwsHeaderValidator is null) 
          throw new NullReferenceException($"When Verify is true {nameof(this.JwsHeaderValidator)} must be not null.");
        if (this.JwsSignatureValidator is null)
          throw new NullReferenceException($"When Verify is true {nameof(this.JwsSignatureValidator)} must be not null.");
        if (this.JwsPayloadValidator is null)
          throw new NullReferenceException($"When Verify is true {nameof(this.JwsPayloadValidator)} must be not null.");

        //Validate the header first as there is no need to try and deserialze the payload if Verify is true and the header is invalid.
        if (this.JwsHeaderValidator is object)
        {
          this.JwsHeaderValidator.Validate(this.DecodeHeader<HeaderType>(Token));
        }
      }

      if (string.IsNullOrEmpty(Token))
      {
        throw new ArgumentException(nameof(Token));
      }
      string Payload = new JwsParts(Token).Payload;
      byte[] DecodedPayload = Base64UrlEncoder.Decode(Payload);
      PayloadType PayloadDeserialized = PayloadSerializer.Deserialize<PayloadType>(DecodedPayload);

      if (Verity)
      {
#pragma warning disable CS8602 // Dereference of a possibly null reference as already checked a method begining.
        this.JwsSignatureValidator.Validate(Token);
        this.JwsPayloadValidator.Validate(PayloadDeserialized);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
      }
      return PayloadDeserialized;
    }

    public HeaderType DecodeHeader<HeaderType>(string Token)
    {
      if (string.IsNullOrEmpty(Token))
      {
        throw new ArgumentException(nameof(Token));
      }
      string Header = new JwsParts(Token).Header;
      byte[] DecodedHeader = Base64UrlEncoder.Decode(Header);
      return HeaderSerializer.Deserialize<HeaderType>(DecodedHeader);
    }

  }
}