﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RDA.Data {
  public class Icon {
    #region Properties

    public static string[] IgnoredDirectorys { get; set; } = new[] {
      $@"{Program.PathRoot}\Resources\data\ui\2kimages\main\3dicons\Temporary_Ornament",
      $@"{Program.PathRoot}\Resources\data\level_editor\random_slots_icons"
    };

    public String Filename { get; set; }

    #endregion Properties

    #region Constructors

    public Icon(String filename) {
      var searchPath = Path.GetDirectoryName($@"{Program.PathRoot}\Resources\{filename}");
      var searchPattern = Path.GetFileNameWithoutExtension($@"{Program.PathRoot}\Resources\{filename}");
      if (IgnoredDirectorys.Contains(searchPath)) {
        return;
      }
      var fileNames = Directory.GetFiles(searchPath, $"{searchPattern}??.png", SearchOption.TopDirectoryOnly);
      if (fileNames.Length != 1) {
        if (searchPattern.Contains("seasonal")) {
          return;
        }
        //throw new NotImplementedException();
        Debug.WriteLine($"Picture Missing: {searchPath} {searchPattern}");
        return;
      }
      this.Filename = filename;
      var file = File.ReadAllBytes(fileNames[0]);
      // publish icon
      var targetPath = Path.GetDirectoryName($@"{Program.PathViewer}\Resources\{filename}");
      var targetFile = Path.GetFullPath($@"{Program.PathViewer}\Resources\{filename}");
      if (!Directory.Exists(targetPath))
        Directory.CreateDirectory(targetPath);
      if (!File.Exists(targetFile)) {
        try {
          File.WriteAllBytes(targetFile, file);
        }
        catch (Exception) { }
      }
    }

    #endregion Constructors

    #region Methods

    public XElement ToXml() {
      var result = new XElement("I");
      result.Add(new XAttribute("F", this.Filename));
      return result;
    }

    #endregion Methods
  }
}