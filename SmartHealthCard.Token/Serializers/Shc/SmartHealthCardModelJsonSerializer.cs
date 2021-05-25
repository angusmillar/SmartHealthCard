using Newtonsoft.Json;
using SmartHealthCard.Token.Model.Shc;

namespace SmartHealthCard.Token.Serializers.Shc
{
  public static class SmartHealthCardModelJsonSerializer
  {   
    public static string ToJson(this SmartHealthCardModel SmartHealthCardModel, bool Minified = true)
    {
      Json.JsonSerializer JsonSerializer = new Json.JsonSerializer(Minified);
      return JsonSerializer.ToJson(SmartHealthCardModel);     
    }

    public static SmartHealthCardModel FromJson(this string SmartHealthCardJson)
    {
      Json.JsonSerializer JsonSerializer = new Json.JsonSerializer();
      return JsonSerializer.FromJson<SmartHealthCardModel>(SmartHealthCardJson);
    }
  }
}
