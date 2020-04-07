﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDA.Data {

  public class Upgrade {

    #region Public Properties

    public Description Text { get; set; }
    public String Value { get; set; }
    public String Category { get; set; }
    public List<AdditionalOutput> AdditionalOutputs { get; set; }
    public List<ReplaceInput> ReplaceInputs { get; set; }
    public List<InputAmountUpgrade> InputAmountUpgrades { get; set; }
    public ReplacingWorkforce ReplacingWorkforce { get; set; }
    public List<Upgrade> Additionals { get; set; }

    #endregion Public Properties

    #region Public Constructors

    public Upgrade() {
    }

    public Upgrade(XElement element) {
      var isPercent = element.Element("Percental")?.Value == "1";
      var value = element.Element("Value") == null ? null : (Int32?)Int32.Parse(element.Element("Value").Value);
      var factor = 1;
      if (Assets.KeyToIdDict.ContainsKey(element.Name.LocalName)) {
        this.Text = new Description(Assets.KeyToIdDict[element.Name.LocalName]);
      }

      switch (element.Name.LocalName) {
        case "Action":
          switch (element.Element("Template").Value) {
            case "ActionStartTreasureMapQuest":
              this.Additionals = new List<Upgrade>();
              this.Text = new Description("2734").AppendWithSpace("-").AppendWithSpace(new Description(element.XPathSelectElement("Values/ActionStartTreasureMapQuest/TreasureSessionOrRegion").Value));
              this.Additionals.Add(new Upgrade { Text = new Description(element.XPathSelectElement("Values/ActionStartTreasureMapQuest/TreasureMapQuest").Value) });
              break;

            default:
              break;
          }
          break;

        case "PassiveTradeGoodGenUpgrade":
          this.Text.AdditionalInformation = new Description("20327", DescriptionFontStyle.Light);
          var genpool = element.Element("GenPool").Value;
          var items = Assets
            .Original
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement("Values/Standard/GUID")?.Value == genpool)?
            .XPathSelectElement("Values/RewardPool/ItemsPool")
            .Elements("Item")
            .Select(i => new Description(i.Element("ItemLink").Value));
          this.Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) GoodGenerationPoolFormatted]", items, (s) => string.Join(", ", s));
          value = Convert.ToInt32(element.Element("GenProbability").Value);
          isPercent = true;
          break;

        case "AddAssemblyOptions":
          this.Text.AdditionalInformation = new Description("20325", DescriptionFontStyle.Light);
          var descs = element.Elements("Item").Select(i => new Description(i.Element("NewOption").Value));
          this.Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", descs, (s) => string.Join(", ", s));
          break;

        case "AssemblyOptions":
          this.Text.AdditionalInformation = new Description("20325", DescriptionFontStyle.Light);
          descs = element.Descendants("Vehicle").Select(i => new Description(i.Value));
          this.Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AddAssemblyOptionsFormatted]", descs, (s) => string.Join(", ", s));
          break;

        case "MoraleDamage":
          this.Text.AdditionalInformation = new Description("21586", DescriptionFontStyle.Light);
          break;

        case "HitpointDamage":
          switch (element.Parent.Parent.Element("Item")?.Element("Allocation").Value ?? "Ship") {
            case "Ship":
            case "SailShip":
            case "Warship":
            case "SteamShip":
              this.Text.AdditionalInformation = new Description("21585", DescriptionFontStyle.Light);
              break;

            default:
              this.Text.AdditionalInformation = new Description("21589", DescriptionFontStyle.Light);
              break;
          }
          break;

        case "SpecialUnitHappinessThresholdUpgrade":
          this.Text.AdditionalInformation = new Description("21584", DescriptionFontStyle.Light);
          var target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault()?.Element("GUID").Value;
          Description unit = null;
          switch (target) {
            case "190777": //Hospital
              unit = new Description("100584");
              break;

            case "190776": //Police Station
              unit = new Description("100582");
              break;

            case "190775": //Fire Station
              unit = new Description("100580");
              break;
            //case "112669": //Polar Station
            //  unit = new Description("114896");
            //  break;

            default:
              throw new NotImplementedException(target);
          }
          this.Text.AdditionalInformation.Replace("[AssetData([ToolOneHelper IncidentResolverUnitsForTargetBuildings([RefGuid], 1) AT(0)]) Text]", unit);
          break;

        case "ItemSet":
        case "ProvidedNeed":
          this.Text = new Description(element.Value);
          break;

        case "HappinessIgnoresMorale":
          this.Text.AdditionalInformation = new Description("20326", DescriptionFontStyle.Light);
          break;

        case "ChangedSupplyValueUpgrade":
          this.Text = new Description("12649");
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item")) {
            this.Additionals.Add(new Upgrade() { Text = new Description(item.Element("Need").Value), Value = (item.Element("AmountInPercent").Value.StartsWith("-") ? "" : "+") + $"{item.Element("AmountInPercent").Value}%" });
          }
          break;

        case "ResolverUnitDecreaseUpgrade":
          target = element.Parent.Parent.Element("ItemEffect").Element("EffectTargets").Elements().FirstOrDefault()?.Element("GUID").Value;
          switch (target) {
            case "190777": //Hospital
              this.Text = new Description("12012");
              break;

            case "190776": //Police Station
              this.Text = new Description("21509");
              break;

            case "112669": //Polar Station
              this.Text = new Description("22983");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              this.Text = new Description("21508");
              break;

            default:
              throw new NotImplementedException(target);
          }
          break;

        case "ResolverUnitCountUpgrade":
          target = element
            .Parent
            .Parent
            .Element("ItemEffect")
            .Element("EffectTargets")
            .Elements()
            .FirstOrDefault()?
            .Element("GUID")
            .Value;

          switch (target) {
            case "190777": //Hospital
              this.Text = new Description("100583");
              break;

            case "190776": //Police Station
              this.Text = new Description("100581");
              break;

            case "112669": //Polar Station
              this.Text = new Description("114895");
              break;

            case "190775": //Fire Station
            case "1010463": //Fire Department
              this.Text = new Description("100579");
              break;

            default:
              throw new NotImplementedException(target);
          }
          break;

        case "AdditionalOutput":
          this.AdditionalOutputs = new List<AdditionalOutput>();
          foreach (var item in element.Elements()) {
            this.AdditionalOutputs.Add(new AdditionalOutput(item));
          }
          break;

        case "ReplaceInputs":
          this.ReplaceInputs = new List<ReplaceInput>();
          foreach (var item in element.Elements()) {
            this.ReplaceInputs.Add(new ReplaceInput(item));
          }
          break;

        case "InputAmountUpgrade":
          this.InputAmountUpgrades = new List<InputAmountUpgrade>();
          foreach (var item in element.Elements()) {
            this.InputAmountUpgrades.Add(new InputAmountUpgrade(item));
          }
          break;

        case "AddedFertility":
          this.Text = new Description("21371").Replace("[AssetData([ItemAssetData([RefGuid]) AddedFertility]) Text]", new Description(element.Value));

          break;

        case "ActiveTradePriceInPercent":
          if (value == null && !element.HasElements) {
            value = Int32.Parse(element.Value);
            if (value < 100) {
              value = -(100 - value);
            }
            else {
              value -= 100;
            }
          }
          isPercent = true;
          break;

        case "ActivateWhiteFlag":
          this.Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_claim_island.png");
          this.Text.AdditionalInformation = new Description("19487", DescriptionFontStyle.Light);
          break;

        case "ActivatePirateFlag":
          this.Text.Icon = new Icon("data/ui/2kimages/main/icons/icon_threat_melee_tint.png");
          this.Text.AdditionalInformation = new Description("17393", DescriptionFontStyle.Light);
          break;

        case "AttackSpeedUpgrade":
          if (value == null) {
            value = element.Value == null ? null : (Int32?)Int32.Parse(element.Value);
          }
          isPercent = true;
          break;

        case "SelfHealPausedTimeIfAttackedUpgrade":
          this.Text.AdditionalInformation = new Description("21590", DescriptionFontStyle.Light);
          value = value == -100 ? null : value;
          break;

        case "NeedProvideNeedUpgrade":
          var SubstituteNeeds = element.Descendants("SubstituteNeed").Select(i => new Description(i.Value));
          var ProvidedNeeds = element.Descendants("ProvidedNeed").Select(i => new Description(i.Value));
          this.Text.AdditionalInformation = new Description("20323", DescriptionFontStyle.Light);
          this.Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AllSubstituteNeedsFormatted]", SubstituteNeeds, s => String.Join(", ", s.Distinct()));
          this.Text.AdditionalInformation.Replace("[ItemAssetData([RefGuid]) AllProvidedNeedsFormatted]", ProvidedNeeds, s => String.Join(", ", s.Distinct()));
          break;

        case "GoodConsumptionUpgrade":
          this.Additionals = new List<Upgrade>();
          foreach (var item in element.Elements("Item")) {
            this.Additionals.Add(new Upgrade() { Text = new Description(item.Element("ProvidedNeed").Value), Value = (item.Element("AmountInPercent").Value.StartsWith("-") ? "" : "+") + $"{item.Element("AmountInPercent").Value}%" });
          }
          break;

        case "UseProjectile":
          var Projectile = Assets
            .Original
            .Descendants("Asset")
            .FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == element.Value);

          var infodesc = Projectile.XPathSelectElement("Values/Standard/InfoDescription")?.Value;
          if (infodesc == null) {
            this.Text = new Description(element.Parent.Parent.XPathSelectElement($"Standard/GUID").Value);
            break;
          }
          var infodescAsset = Assets.Original.Descendants("Asset").FirstOrDefault(a => a.XPathSelectElement($"Values/Standard/GUID")?.Value == infodesc);
          if (infodescAsset != null) {
            this.Text = new Description(infodescAsset.XPathSelectElement("Values/Standard/InfoDescription").Value) {
              AdditionalInformation = new Description(infodescAsset.XPathSelectElement("Values/Standard/GUID").Value, DescriptionFontStyle.Light)
            };
          }
          break;

        case "ActionDuration":
          this.Text.FontStyle = DescriptionFontStyle.Light;
          this.Text.Languages = new Description("3898").Languages;
          this.Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("hh':'mm':'ss");
          while (this.Value.StartsWith("00:00:")) {
            this.Value = this.Value.Remove(0, 3);
          }
          return;

        case "ActionCooldown":
          this.Text.FontStyle = DescriptionFontStyle.Light;
          this.Text.Languages = new Description("3899").Languages;
          this.Value = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value)).ToString("hh':'mm':'ss");
          while (this.Value.StartsWith("00:00:")) {
            this.Value = this.Value.Remove(0, 3);
          }
          return;

        case "IsDestroyedAfterCooldown":
          this.Text.FontStyle = DescriptionFontStyle.Light;
          this.Text.Languages = new Description("2421").Remove("&lt;font color='0xff817f87'&gt;").Remove("&lt;/font&gt;").Languages;
          break;

        case "Building":
          this.Text = new Description("17394");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SailShip":
          this.Text = new Description("17395");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "SteamShip":
          this.Text = new Description("17396");
          value = Convert.ToInt32((Decimal.Parse(element.Element("Factor").Value, System.Globalization.CultureInfo.InvariantCulture) * 100) - 100);
          isPercent = true;
          break;

        case "ReplacingWorkforce":
          this.ReplacingWorkforce = new ReplacingWorkforce(element.Value);
          break;

        case "BaseDamageUpgrade":
          value = value ?? 0;
          break;

        case "IncidentIllnessIncreaseUpgrade":
        case "IncidentArcticIllnessIncreaseUpgrade":
        case "IncidentFireIncreaseUpgrade":
        case "IncidentExplosionIncreaseUpgrade":
        case "ScrapAmountLevelUpgrade":
          factor = 10;
          isPercent = true;
          break;

        case "Normal":
        case "Cannon":
        case "BigBertha":
        case "Torpedo":
          value = -Convert.ToInt32(100M - (100M * Decimal.Parse(element.Element("Factor").Value, CultureInfo.InvariantCulture)));
          isPercent = true;
          break;

        case "ModuleLimitPercent":
        case "ConstructionTimeInPercent":
        case "ConstructionCostInPercent":
        case "TaxModifierInPercent":
        case "WorkforceModifierInPercent":
          value = Int32.Parse(element.Value);
          isPercent = true;
          break;

        case "IgnoreWeightFactorUpgrade":
        case "IgnoreDamageFactorUpgrade":
          value = -value;
          break;

        case "NeededAreaPercentUpgrade":
          isPercent = true;
          break;

        case "ResolverUnitMovementSpeedUpgrade":
          this.Value = null;
          break;

        case "IncidentRiotIncreaseUpgrade":
          if (element.Element("Percental")?.Value != "1") {
            factor = 10;
          }
          isPercent = true;
          break;

        case "AccuracyUpgrade":
        case "LineOfSightRangeUpgrade":
        case "LoadingSpeedUpgrade":
        case "PublicServiceFullSatisfactionDistance":
        case "HealRadiusUpgrade":
        case "HealPerMinuteUpgrade":
        case "SpawnProbabilityFactor":
        case "SelfHealUpgrade":
        case "AttackRangeUpgrade":
        case "ForwardSpeedUpgrade":
        case "MaxHitpointsUpgrade":
        case "ResidentsUpgrade":
        case "StressUpgrade":
        case "ProvideElectricity":
        case "NeedsElectricity":
        case "AttractivenessUpgrade":
        case "MaintenanceUpgrade":
        case "WorkforceAmountUpgrade":
        case "OutputAmountFactorUpgrade":
        case "ProductivityUpgrade":
        case "BlockBuyShare":
        case "BlockHostileTakeover":
        case "MaintainanceUpgrade":
        case "MoralePowerUpgrade":
        case "PierSpeedUpgrade":
        case "HeatRangeUpgrade":
        case "HasPollution":

        case "MinPickupTimeUpgrade":
        case "MaxPickupTimeUpgrade":
        case "AttractivenessPerSetUpgrade":
        case "SocketCountUpgrade":
        case "ProductivityBoostUpgrade":
          break;

        case "AdditionalHappiness":
        case "AdditionalSupply":
        case "AdditionalMoney":
        case "AdditionalHeat":
        case "Attractiveness":
        case "NumOfPiers":
        case "LoadingSpeed":
        case "MinLoadingTime":
        case "AttackRange":
        case "LineOfSightRange":
        case "ReloadTime":
        case "BaseDamage":
        case "HealRadius":
        case "HealPerMinute":
        case "MaxTrainCount":
        case "StorageCapacityModifier":
          value = Int32.Parse(element.Value);
          break;

        case "OverrideSpecialistPool":
          this.Text.AdditionalInformation = new Description("269571", DescriptionFontStyle.Light);
          break;

        case "RarityWeightUpgrade":
          this.Additionals = new List<Upgrade>();
          this.Text = new Description("22227");
          foreach (var item in element.Elements()) {
            if (item.Name.LocalName == "None") {
              //this.Additionals.Add(new Upgrade() { Text = new Description("None", "None"), Value = $"+{item.Element("AdditionalWeight").Value}" });
            }
            else {
              this.Additionals.Add(new Upgrade() { Text = new Description(Assets.KeyToIdDict[item.Name.LocalName]), Value = $"+{item.Element("AdditionalWeight").Value}" });
            }
          }
          break;

        case "ItemSetUpgrade":
          this.Additionals = new List<Upgrade>();
          this.Text = new Description("145011");
          foreach (var item in element.Elements()) {
            this.Additionals.Add(new Upgrade() { Text = new Description(item.Element("ItemSet").Value), Value = $"+{item.Element("AttractivenessUpgradePercent").Value}%" });
          }
          break;

        case "Residence7":
          this.Text = new Description("22379");
          this.Additionals = new List<Upgrade> {
            new Upgrade() { Text = new Description(element.Element("PopulationLevel7").Value), Value = element.Element("ResidentMax").Value }
          };
          break;

        default:
          throw new NotImplementedException(element.Name.LocalName);
      }
      if (value == null) {
        this.Value = String.Empty;
      }
      else {
        if (isPercent) {
          this.Value = value > 0 ? $"+{value * factor}%" : $"{value * factor}%";
        }
        else {
          this.Value = value > 0 ? $"+{value * factor}" : $"{value * factor}";
        }
      }
    }

    public Upgrade(String key, String amount) {
      var value = amount == null ? null : (Int32?)Int32.Parse(amount);
      this.Text = new Description(Assets.GetDescriptionID(key));
      switch (key) {
        case "PerkFormerPirate":
        case "PerkDiver":
        case "PerkZoologist":
        case "PerkMilitaryShip":
        case "PerkHypnotist":
        case "PerkAnthropologist":
        case "PerkPolyglot":
        case "PerkArcheologist":
        case "PerkMale":
        case "PerkFemale":
          value = null;
          this.Text = Text.InsertBefore(new Description("-1"));
          break;

        default:
          break;
      }
      if (value == null) {
        this.Value = String.Empty;
      }
      else {
        this.Value = value.ToString();
      }
    }

    #endregion Public Constructors

    #region Public Methods

    public XElement ToXml() {
      var result = new XElement("U");
      if (this.Text != null)
        result.Add(this.Text.ToXml("T"));
      if (this.Value != null)
        result.Add(new XAttribute("V", this.Value));
      if (this.Category != null)
        result.Add(new XAttribute("C", this.Category));
      if (this.AdditionalOutputs != null)
        result.Add(new XElement("AO", this.AdditionalOutputs.Select(s => s.ToXml())));
      if (this.ReplaceInputs != null)
        result.Add(new XElement("RI", this.ReplaceInputs.Select(s => s.ToXml())));
      if (this.InputAmountUpgrades != null)
        result.Add(new XElement("IAUp", this.InputAmountUpgrades.Select(s => s.ToXml())));
      if (this.ReplacingWorkforce != null)
        result.Add(new XElement("RW", this.ReplacingWorkforce.ToXml()));
      if (this.Additionals != null)
        result.Add(new XElement("A", this.Additionals.Select(s => s.ToXml())));
      return result;
    }

    #endregion Public Methods
  }
}