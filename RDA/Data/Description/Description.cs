﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace RDA.Data {
  public class Description : IEquatable<Description> {
    #region Properties

    public String ID { get; set; }

    public Dictionary<Languages, string> Languages { get; set; } = new Dictionary<Languages, string>();

    public Icon Icon { get; set; }
    public DescriptionFontStyle FontStyle { get; set; }
    public Description AdditionalInformation { get; set; }

    #endregion Properties

    #region Fields

    public static readonly Dictionary<string, Description> GlobalDescriptions = new Dictionary<string, Description>();

    #endregion Fields

    #region Constructors

    public Description(String id, DescriptionFontStyle fontStyle = default) {
      this.ID = id;
      if (Assets.Descriptions.TryGetValue(id, out var languages)) {
        foreach (var item in languages) {
          Languages.Add(item.Key, item.Value);
        }
      }
      else if (Assets.CustomDescriptions.TryGetValue(id, out languages)) {
        foreach (var item in languages) {
          Languages.Add(item.Key, item.Value);
        }
      }
      else {
        Languages.Add(Data.Languages.English, id);
      }
      if (Assets.Icons.ContainsKey(id)) {
        this.Icon = new Icon(Assets.Icons[id]);
      }
      this.FontStyle = fontStyle;
    }

    #endregion Constructors

    #region Methods

    public Description InsertBefore(Description description) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{(description.Languages.TryGetValue(item.Key, out var value) ? value : description.Languages.First().Value)} {item.Value}";
      }
      SetNewId();
      return this;
    }

    public Description InsertBefore(string value) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{value} {item.Value}";
      }
      SetNewId();
      return this;
    }

    public Description Append(string value) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{item.Value}{value}";
      }
      SetNewId();
      return this;
    }

    public Description Append(Description description) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{item.Value}{(description.Languages.TryGetValue(item.Key, out var value) ? value : description.Languages.First().Value)}";
      }
      SetNewId();
      return this;
    }

    public Description AppendWithSpace(string value) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{item.Value} {value}";
      }
      SetNewId();
      return this;
    }

    public Description AppendWithSpace(Description description) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{item.Value} {(description.Languages.TryGetValue(item.Key, out var value) ? value : description.Languages.First().Value)}";
      }
      SetNewId();
      return this;
    }

    public Description AppendInBraces(Description description) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = $"{item.Value} ({(description.Languages.TryGetValue(item.Key, out var value) ? value : description.Languages.First().Value)})";
      }
      SetNewId();
      return this;
    }

    public Description Remove(String value) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = item.Value.Replace(HttpUtility.HtmlDecode(value), "");
      }
      SetNewId();
      return this;
    }

    public Description Replace(String oldValue, Description newValue) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = item.Value.Replace(HttpUtility.HtmlDecode(oldValue), newValue.Languages.TryGetValue(item.Key, out var value) ? value : newValue.Languages.First().Value);
      }
      SetNewId();
      return this;
    }

    public Description Replace(String oldValue, IEnumerable<Description> newValue, Func<IEnumerable<string>, string> format) {
      foreach (var item in Languages.ToArray()) {
        Languages[item.Key] = item.Value.Replace(HttpUtility.HtmlDecode(oldValue), format(newValue.Select(v => v.Languages.TryGetValue(item.Key, out var value) ? value : v.Languages.First().Value)));
      }
      SetNewId();
      return this;
    }

    public XElement ToXml(String name) {
      this.ID = GetOrCheckExistenz();
      var result = new XElement(name);
      result.Add(new XAttribute("ID", this.ID));
      if (this.Icon?.Filename is string icon) {
        result.Add(new XAttribute("I", icon));
      }
      if (FontStyle != default) {
        result.Add(new XAttribute("FS", (int)FontStyle));
      }
      if (AdditionalInformation != null) {
        result.Add(AdditionalInformation.ToXml("AI"));
      }

      return result;
    }

    public void SetNewId() {
      ID = this.Languages.First().Value.GetHashCode().ToString();
    }

    public override String ToString() {
      return this.Languages.First().Value;
    }

    public bool Equals(Description other) {
      return ID == other.ID && this.Languages.First().Value == other.Languages.First().Value && Icon.Filename == other.Icon.Filename;
    }

    internal static Description Join(IEnumerable<Description> regions, string seperator) {
      if (regions?.Any() == true) {
        var desc = new Description(regions.First().ID);
        foreach (var item in regions.Skip(1)) {
          desc.Append(seperator).Append(item);
        }
        return desc;
      }
      return null;
    }

    private string GetOrCheckExistenz() {
      if (GlobalDescriptions.TryGetValue(this.Languages.First().Value, out var value)) {
        return value.ID;
      }
      else {
        GlobalDescriptions.Add(this.Languages.First().Value, this);

        return ID;
      }
    }

    #endregion Methods
  }
}