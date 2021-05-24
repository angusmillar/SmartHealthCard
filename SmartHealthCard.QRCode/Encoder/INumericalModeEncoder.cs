namespace SmartHealthCard.QRCode.Encoder
{
  public interface INumericalModeEncoder
  {
    string Encode(string JWSToken);
  }
}