using System;

namespace SmartHealthCard.Token.Enums
{
  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public sealed class EnumInfoAttribute : Attribute
  {
    readonly string literal;
    readonly string description;

    /// <summary>
    // This is a positional argument
    /// </summary>
    /// <param name="literal"></param>
    /// <param name="description"></param>
    public EnumInfoAttribute(string literal, string description)
    {
      this.literal = literal;
      this.description = description;
    }

    public EnumInfoAttribute(string literal)
    {
      this.literal = literal;
      this.description = "Enum description not defined";
    }

    public string Literal
    {
      get { return literal; }
    }
    public string Description
    {
      get { return description; }
    }
  }
}
