using SmartHealthCard.Token.Encoders;
using SmartHealthCard.Token.Exceptions;
using SmartHealthCard.Token.Model.Jwks;
using SmartHealthCard.Token.Model.Jws;
using SmartHealthCard.Token.Providers;
using SmartHealthCard.Token.Serializers.Json;
using SmartHealthCard.Token.Serializers.Jws;
using SmartHealthCard.Token.Support;
using SmartHealthCard.Token.Validators;
using System;
using System.Threading.Tasks;

namespace SmartHealthCard.Token.JwsToken
{
  public sealed class SmartHealthCardJwsDecoder : IJwsDecoder
  {
    private readonly IJsonSerializer? JsonSerializer;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private readonly IJwsSignatureValidator? JwsSignatureValidator;
    private readonly IJwsPayloadValidator? JwsPayloadValidator;
    private readonly IJwsHeaderValidator? JwsHeaderValidator;
    private readonly IJwksProvider? JwksProvider;
    public SmartHealthCardJwsDecoder(
      IJwsHeaderSerializer HeaderSerializer,
      IJwsPayloadSerializer PayloadSerializer)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }

    public SmartHealthCardJwsDecoder(
      IJsonSerializer JsonSerializer,
      IJwsHeaderSerializer HeaderSerializer,
      IJwsPayloadSerializer PayloadSerializer,
      IJwksProvider? JwksProvider,
      IJwsSignatureValidator? IJwsSignatureValidator,
      IJwsHeaderValidator? JwsHeaderValidator,
      IJwsPayloadValidator? IJwsPayloadValidator)
      : this(HeaderSerializer, PayloadSerializer)
    {      
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();

      this.JwksProvider = JwksProvider ?? new JwksProvider(this.JsonSerializer);
      this.JwsSignatureValidator = IJwsSignatureValidator ?? new JwsSignatureValidator();
      this.JwsHeaderValidator = JwsHeaderValidator ?? new SmartHealthCardHeaderValidator();
      this.JwsPayloadValidator = IJwsPayloadValidator ?? new SmartHealthCardPayloadValidator();
    }

    public async Task<Result<PayloadType>> DecodePayloadAsync<HeaderType, PayloadType>(string Token, bool Verity = false)
    {
      if (string.IsNullOrEmpty(Token))
        return await Task.FromResult(Result<PayloadType>.Fail("The provided Token was found to be null or empty."));

      Result<JwsParts> JwsPartsParseResult = JwsParts.ParseToken(Token);
      if (JwsPartsParseResult.Failure)
        return await Task.FromResult(Result<PayloadType>.Fail(JwsPartsParseResult.ErrorMessage));

      byte[] DecodedPayload = Base64UrlEncoder.Decode(JwsPartsParseResult.Value.Payload);
      Result<PayloadType> PayloadDeserializedResult = await PayloadSerializer.DeserializeAsync<PayloadType>(DecodedPayload);
      if (PayloadDeserializedResult.Failure)
        return PayloadDeserializedResult;

      if (!Verity)
      {        
        return await Task.FromResult(Result<PayloadType>.Ok(PayloadDeserializedResult.Value)); 
      }
      else
      {
        //We must use the PayloadSerializer.Deserialize and not the JsonSerializer.FromJson() because for 
        //SMART Health Cards the payload is DEFLATE compressed bytes and not a JSON string
        Result<JwsBody> JwsBodyDeserializeResult = await PayloadSerializer.DeserializeAsync<JwsBody>(DecodedPayload);
        if (JwsBodyDeserializeResult.Failure)
          return Result<PayloadType>.Fail(JwsBodyDeserializeResult.ErrorMessage);
        
        if (JwsBodyDeserializeResult.Value.Iss is null)
          return await Task.FromResult(Result<PayloadType>.Fail("No Issuer (iss) claim found in JWS Token body."));        

        if (!Uri.TryCreate($"{JwsBodyDeserializeResult.Value.Iss}/.well-known/jwks.json", UriKind.Absolute, out Uri? WellKnownJwksUri))
          return await Task.FromResult(Result<PayloadType>.Fail($"Unable to parse the Issuer (iss) claim to a absolute Uri, value was {$"{JwsBodyDeserializeResult.Value.Iss}/.well-known/jwks.json"}"));

        if (JwksProvider is null)
          return await Task.FromResult(Result<PayloadType>.Fail($"When Verify is true {nameof(this.JwksProvider)} must be not null."));        
        
        Result<JsonWebKeySet> JsonWebKeySetResult = await JwksProvider.GetJwksAsync(WellKnownJwksUri);
        JsonWebKeySet JsonWebKeySet;
        if (JsonWebKeySetResult.Success)
        {
          JsonWebKeySet = JsonWebKeySetResult.Value;          
        }
        else
        {
          return await Task.FromResult(Result<PayloadType>.Fail($"Unable to obtain the JsonWebKeySet (JWKS) from : {WellKnownJwksUri.OriginalString}. ErrorMessage: {JsonWebKeySetResult.ErrorMessage}"));          
        }
       
        byte[] DecodedHeader = Base64UrlEncoder.Decode(JwsPartsParseResult.Value.Header);
        string HeaderJson = Utf8EncodingSupport.GetString(DecodedHeader);

        if (JsonSerializer is null)
          return await Task.FromResult(Result<PayloadType>.Fail($"When Verify is true {nameof(this.JsonSerializer)} must be not null."));        

        Result<JwsHeader> JwsHeaderFromJsonResult = JsonSerializer.FromJson<JwsHeader>(HeaderJson);
        if (JwsHeaderFromJsonResult.Failure)
          return Result<PayloadType>.Fail(JwsBodyDeserializeResult.ErrorMessage);

        if (JwsHeaderFromJsonResult.Value.Kid is null)
          return await Task.FromResult(Result<PayloadType>.Fail("No key JWK Thumb-print (kid) claim found in JWS Token header."));
        
        Result<Algorithms.IAlgorithm> AlgorithmResult = Algorithms.ES256Algorithm.FromJWKS(JwsHeaderFromJsonResult.Value.Kid, JsonWebKeySet, JsonSerializer);
        if (AlgorithmResult.Failure)
          return await Task.FromResult(Result<PayloadType>.Fail(AlgorithmResult.ErrorMessage));

        if (this.JwsSignatureValidator is null)
          return await Task.FromResult(Result<PayloadType>.Fail($"When Verify is true {nameof(this.JwsSignatureValidator)} must be not null."));

        Result JwsSignatureValidatorResult = JwsSignatureValidator.Validate(AlgorithmResult.Value, Token);
        if (JwsSignatureValidatorResult.Failure)
          return await Task.FromResult(Result<PayloadType>.Fail(JwsSignatureValidatorResult.ErrorMessage));

        if (this.JwsHeaderValidator is null)
          return await Task.FromResult(Result<PayloadType>.Fail($"When Verify is true {nameof(this.JwsHeaderValidator)} must be not null."));

        Result<HeaderType> DecodeHeaderResult = await this.DecodeHeaderAsync<HeaderType>(Token);
        if (DecodeHeaderResult.Failure)
          return await Task.FromResult(Result<PayloadType>.Fail(DecodeHeaderResult.ErrorMessage));

        Result JwsHeaderValidateResult = this.JwsHeaderValidator.Validate(DecodeHeaderResult.Value);
        if (JwsHeaderValidateResult.Failure)
          return await Task.FromResult(Result<PayloadType>.Fail(JwsHeaderValidateResult.ErrorMessage));

        if (this.JwsPayloadValidator is null)
          return await Task.FromResult(Result<PayloadType>.Fail($"When Verify is true {nameof(this.JwsPayloadValidator)} must be not null."));
        
        Result JwsPayloadValidatorResult = this.JwsPayloadValidator.Validate(PayloadDeserializedResult.Value);
        if (JwsPayloadValidatorResult.Failure)
          return await Task.FromResult(Result<PayloadType>.Fail(JwsPayloadValidatorResult.ErrorMessage));

        return Result<PayloadType>.Ok(PayloadDeserializedResult.Value);
      }
    }

    public async Task<Result<HeaderType>> DecodeHeaderAsync<HeaderType>(string Token)
    {
      if (string.IsNullOrEmpty(Token))
        return await Task.FromResult(Result<HeaderType>.Fail("The provided Token was found to be null or empty."));

      Result<JwsParts> JwsPartsParseTokenResult = JwsParts.ParseToken(Token);
      if (JwsPartsParseTokenResult.Failure)
        return await Task.FromResult(Result<HeaderType>.Fail(JwsPartsParseTokenResult.ErrorMessage));

      byte[] DecodedHeader = Base64UrlEncoder.Decode(JwsPartsParseTokenResult.Value.Header);      
      return await HeaderSerializer.DeserializeAsync<HeaderType>(DecodedHeader);
    }

  }
}