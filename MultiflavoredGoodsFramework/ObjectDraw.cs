
using StardewValley;
using System.Collections.Generic;
using System;
using System.Linq;
using StardewValley.Objects;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Data;
using Utils = MultiFlavor.Utils;
using xTile.Dimensions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using StardewValley.Extensions;
using StardewModdingAPI;
using System.Threading;
using MultiFlavor.Utils;

namespace MultiFlavor;


/// <summary>controls the display of sprites and their coloring</summary>
public class ObjectDraw
{
    internal static IMonitor Monitor { get; set; }
    public ObjectDraw(IMonitor monitor)
    {
        Monitor = monitor;
    }
    public static (int r, int g, int b) MixColors(string color1, string color2)
    {
        // Retrieve RGB values for both layers
        (int r1, int g1, int b1) = GetRGB(color1);
        (int r2, int g2, int b2) = GetRGB(color2);

        // Calculate the average of RGB values
        int rMixed = (r1 + r2) / 2;
        int gMixed = (g1 + g2) / 2;
        int bMixed = (b1 + b2) / 2;

        return (rMixed, gMixed, bMixed);
    }

    public static Color GetColorFromContextTag(string colorTag)
    {
        switch (colorTag)
        {
            case "color_black":
                return new Color(45, 45, 45);
            case "color_gray":
                return Color.Gray;
            case "color_white":
                return Color.White;
            case "color_pink":
                return new Color(255, 163, 186);
            case "color_red":
                return new Color(220, 0, 0);
            case "color_orange":
                return new Color(255, 128, 0);
            case "color_yellow":
                return new Color(255, 230, 0);
            case "color_green":
                return new Color(10, 143, 0);
            case "color_blue":
                return new Color(46, 85, 183);
            case "color_purple":
                return new Color(115, 41, 181);
            case "color_brown":
                return new Color(130, 73, 37);
            case "color_light_cyan":
                return new Color(180, 255, 255);
            case "color_cyan":
                return Color.Cyan;
            case "color_aquamarine":
                return Color.Aquamarine;
            case "color_sea_green":
                return Color.SeaGreen;
            case "color_lime":
                return Color.Lime;
            case "color_yellow_green":
                return Color.GreenYellow;
            case "color_pale_violet_red":
                return Color.PaleVioletRed;
            case "color_salmon":
                return new Color(255, 85, 95);
            case "color_jade":
                return new Color(130, 158, 93);
            case "color_sand":
                return Color.NavajoWhite;
            case "color_poppyseed":
                return new Color(82, 47, 153);
            case "color_dark_red":
                return Color.DarkRed;
            case "color_dark_orange":
                return Color.DarkOrange;
            case "color_dark_yellow":
                return Color.DarkGoldenrod;
            case "color_dark_green":
                return Color.DarkGreen;
            case "color_dark_blue":
                return Color.DarkBlue;
            case "color_dark_purple":
                return Color.DarkViolet;
            case "color_dark_pink":
                return Color.DeepPink;
            case "color_dark_cyan":
                return Color.DarkCyan;
            case "color_dark_gray":
                return Color.DarkGray;
            case "color_dark_brown":
                return Color.SaddleBrown;
            case "color_gold":
                return Color.Gold;
            case "color_copper":
                return new Color(179, 85, 0);
            case "color_iron":
                return new Color(197, 213, 224);
            case "color_iridium":
                return new Color(105, 15, 255);
        }
        return Color.White;
    }

    public static (int r, int g, int b) GetRGB(string color)
    {
        switch (color)
        {
            case "color_black":
                return (45, 45, 45);
            case "color_gray":
                return (128, 128, 128);
            case "color_white":
                return (255, 255, 255);
            case "color_pink":
                return (255, 163, 186);
            case "color_red":
                return (220, 0, 0);
            case "color_orange":
                return (255, 128, 0);
            case "color_yellow":
                return (255, 230, 0);
            case "color_green":
                return (10, 143, 0);
            case "color_blue":
                return (46, 85, 183);
            case "color_purple":
                return (115, 41, 181);
            case "color_brown":
                return (130, 73, 37);
            case "color_light_cyan":
                return (180, 255, 255);
            case "color_cyan":
                return (0, 255, 255);
            case "color_aquamarine":
                return (127, 255, 212);
            case "color_sea_green":
                return (46, 139, 87);
            case "color_lime":
                return (0, 255, 0);
            case "color_yellow_green":
                return (173, 255, 47);
            case "color_pale_violet_red":
                return (219, 112, 147);
            case "color_salmon":
                return (255, 85, 95);
            case "color_jade":
                return (130, 158, 93);
            case "color_sand":
                return (244, 164, 96);
            case "color_poppyseed":
                return (82, 47, 153);
            case "color_dark_red":
                return (139, 0, 0);
            case "color_dark_orange":
                return (255, 140, 0);
            case "color_dark_yellow":
                return (184, 134, 11);
            case "color_dark_green":
                return (0, 100, 0);
            case "color_dark_blue":
                return (0, 0, 139);
            case "color_dark_purple":
                return (148, 0, 211);
            case "color_dark_pink":
                return (255, 20, 147);
            case "color_dark_cyan":
                return (0, 139, 139);
            case "color_dark_gray":
                return (169, 169, 169);
            case "color_dark_brown":
                return (139, 69, 19);
            case "color_gold":
                return (255, 215, 0);
            case "color_copper":
                return (179, 85, 0);
            case "color_iron":
                return (197, 213, 224);
            case "color_iridium":
                return (105, 15, 255);
        }
        return (128, 128, 128);
    }

    public static string FindClosestColor((int r, int g, int b) color)
    {
        Dictionary<string, (int r, int g, int b)> colors = new Dictionary<string, (int r, int g, int b)>
        {
            {"color_black", (45, 45, 45)},
            {"color_gray", (128, 128, 128)},
            {"color_white", (255, 255, 255)},
            {"color_pink", (255, 163, 186)},
            {"color_red", (220, 0, 0)},
            {"color_orange", (255, 128, 0)},
            {"color_yellow", (255, 230, 0)},
            {"color_green", (10, 143, 0)},
            {"color_blue", (46, 85, 183)},
            {"color_purple", (115, 41, 181)},
            {"color_brown", (130, 73, 37)},
            {"color_light_cyan", (180, 255, 255)},
            {"color_cyan", (0, 255, 255)},
            {"color_aquamarine", (127, 255, 212)},
            {"color_sea_green", (46, 139, 87)},
            {"color_lime", (0, 255, 0)},
            {"color_yellow_green", (173, 255, 47)},
            {"color_pale_violet_red", (219, 112, 147)},
            {"color_salmon", (255, 85, 95)},
            {"color_jade", (130, 158, 93)},
            {"color_sand", (244, 164, 96)},
            {"color_poppyseed", (82, 47, 153)},
            {"color_dark_red", (139, 0, 0)},
            {"color_dark_orange", (255, 140, 0)},
            {"color_dark_yellow", (184, 134, 11)},
            {"color_dark_green", (0, 100, 0)},
            {"color_dark_blue", (0, 0, 139)},
            {"color_dark_purple", (148, 0, 211)},
            {"color_dark_pink", (255, 20, 147)},
            {"color_dark_cyan", (0, 139, 139)},
            {"color_dark_gray", (169, 169, 169)},
            {"color_dark_brown", (139, 69, 19)},
            {"color_gold", (255, 215, 0)},
            {"color_copper", (179, 85, 0)},
            {"color_iron", (197, 213, 224)},
            {"color_iridium", (105, 15, 255)}
        };

        double minDistanceSquared = double.MaxValue;
        string closestColor = null;

        foreach (var kvp in colors)
        {
            int rDiff = color.r - kvp.Value.r;
            int gDiff = color.g - kvp.Value.g;
            int bDiff = color.b - kvp.Value.b;

            double distanceSquared = rDiff * rDiff + gDiff * gDiff + bDiff * bDiff;

            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                closestColor = kvp.Key;
            }
        }

        return closestColor;
    }

    /*
         public static Color? getColorMaskFromTag(string tag, Item item)
     {

         string myTag = tag;
         int index = 0;
         Color? color = null;
         if (!string.IsNullOrEmpty(tag))
         {
             string mixedColor = getMixedColorTag((StardewValley.Object)item, tag);
             color = GetColorFromContextTag(mixedColor);
         }
         if (!string.IsNullOrEmpty(tag) && char.IsDigit(tag[^1]))
         {
             index = int.Parse(tag[^1].ToString());
             index -= 1;
             myTag = tag[..^1];
         }
         if (item.modData.TryGetValue($"_mtag.{myTag}", out string madeWithValue) && !string.IsNullOrEmpty(madeWithValue))
         {
             Dictionary<string, List<string>> madeWith = ProcessInput.getMadeWithForToken(madeWithValue, item.GetContextTags().ToList());
             if (madeWith.TryGetValue("Colors", out List<string> myColors))
             {
                 myColors.Sort();
                 if (myColors.Count > index)
                     color = GetColorFromContextTag(myColors[index]);
             }
         }

         return color;
     }
     /*

     public static string? getIdFromTag(string tag, Item item)
     {
         string myTag = tag;
         int index = 0;
         string? myId = null;

         if (!string.IsNullOrEmpty(tag) && char.IsDigit(tag[^1]))
         {
             index = int.Parse(tag[^1].ToString());
             index -= 1;
             myTag = tag[..^1];
         }
         if (item.modData.TryGetValue($"_mtag.{myTag}", out string madeWithValue) && !string.IsNullOrEmpty(madeWithValue))
         {
             Dictionary<string, List<string>> madeWith = ProcessInput.getMadeWithForToken(madeWithValue, item.GetContextTags().ToList());
             if (madeWith.TryGetValue("Ids", out List<string> myIds))
             {
                 myIds.Sort();
                 if (myIds.Count > index)
                     myId = myIds[index];
             }
         }

         return myId;
     }

     public static string getColorNameFromMTag(string tag, Item item)
     {
         string myTag = tag;
         int index = 0;
         string color = null;
         if (!string.IsNullOrEmpty(tag))
             color = getMixedColorTag((StardewValley.Object)item, tag);
         return color;
     }
    */

    public static string getMixedColorTag(StardewValley.Object myItemObject, string mtag)
    {
        string closestColor = null;

        string mtagValue = (myItemObject as Item)?.modData.TryGetValue($"_mtag.{mtag}", out string temp) == true ? temp : null;

        List<string> savedColors = new List<string>();
        if (!string.IsNullOrEmpty(mtagValue))
        {
            Dictionary<string, List<string>> madeWith = TagUtils.getMtagData(mtagValue);
            if (madeWith.TryGetValue("Colors", out List<string> myColors))
                savedColors = myColors;
        }

        if (savedColors.Count < 1)
        {
            closestColor = myItemObject.GetContextTags().FirstOrDefault(tag => tag.StartsWith("color_"));
        }
        else
        {
            savedColors.Sort();
            closestColor = savedColors[0];
            for (int i = 1; i < savedColors.Count; i++)
            {
                string color = savedColors[i];
                (int r, int g, int b) mixedColor = MixColors(color, closestColor);
                closestColor = FindClosestColor(mixedColor);
            }
        }

        return closestColor;
    }


    public static void drawSpritesObject(StardewValley.Object myobject, SpriteBatch spriteBatch, Vector2 location, float layerDepth, bool isMenu)
    {
        OutputObjectData outputObjectData = AssetHandler.Recipes.Values.Where(innerDict => innerDict.ContainsKey(myobject.ItemId)).Select(innerDict => innerDict[myobject.ItemId]).FirstOrDefault();

        Dictionary<string, RecipeData> visibleLayers = outputObjectData.Layers.Where(entry => entry.Value.Visible.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
        int numOfLayers = visibleLayers.Values.Count;

        RecipeData recipeData = visibleLayers.ElementAt(0).Value;

        string color = getMixedColorTag(myobject, visibleLayers.ElementAt(0).Key);

        ImageDisplayData layerRule = new ImageDisplayData();

        foreach (var ruleset in recipeData.DisplayRules)
        {
            var rule = ruleset.Value;
            HashSet<string> contextTags = myobject.GetContextTags();
            contextTags.UnionWith(TagUtils.CollectMTagsFromModData(myobject));
            if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, contextTags).Count() > 0)
            {
                string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                {
                    layerRule = rule;
                    break;
                }
            }
        }
        layerDepth = MathF.BitIncrement(layerDepth);
        //1
        DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);

        //2
        if (numOfLayers > 1)
        {
            recipeData = visibleLayers.ElementAt(1).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            DrawLayer(spriteBatch, location, MathF.BitIncrement(layerDepth), layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //3
        if (numOfLayers > 2)
        {
            recipeData = visibleLayers.ElementAt(2).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //4
        if (numOfLayers > 3)
        {
            recipeData = visibleLayers.ElementAt(3).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //5
        if (numOfLayers > 4)
        {
            recipeData = visibleLayers.ElementAt(4).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //6
        if (numOfLayers > 5)
        {
            recipeData = visibleLayers.ElementAt(5).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //7
        if (numOfLayers > 6)
        {
            recipeData = visibleLayers.ElementAt(6).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //8
        if (numOfLayers > 7)
        {
            recipeData = visibleLayers.ElementAt(7).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //9
        if (numOfLayers > 8)
        {
            recipeData = visibleLayers.ElementAt(8).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
        //10
        if (numOfLayers > 9)
        {
            recipeData = visibleLayers.ElementAt(9).Value;
            foreach (var ruleset in recipeData.DisplayRules)
            {
                var rule = ruleset.Value;
                if (rule.DisplayConditions is null || TagUtils.ValidateRuleWithPassedTags(rule.DisplayConditions, myobject, myobject.GetContextTags()).Count() > 0)
                {
                    string onlyWhenOutputColor = rule.DisplayConditions.OnlyWhenOutputColor;
                    if (string.IsNullOrEmpty(onlyWhenOutputColor) || onlyWhenOutputColor.Equals(color))
                    {
                        layerRule = rule;
                        break;
                    }
                }
            }
            layerDepth = MathF.BitIncrement(layerDepth);
            DrawLayer(spriteBatch, location, layerDepth, layerRule, color, myobject.QualifiedItemId, false, isMenu);
        }
    }

    /*
    public static void drawSpritesMachine(StardewValley.Object machine, SpriteBatch spriteBatch, Vector2 location)
    {
        float layerDepth = (float)((location.Y + 1.0) * 64.0 / 10000.0);

        StardewValley.Object heldobject = machine.heldObject.Value;
        string itemId = heldobject.ItemId;

        SpecialDisplayRule specialDisplayRule;

        if (!DataLoader.SpecialDisplayRules.ContainsKey(itemId))
            specialDisplayRule = new SpecialDisplayRule();
        else specialDisplayRule = DataLoader.SpecialDisplayRules[itemId];

         MachineDisplayRule machineDisplayRule = specialDisplayRule.MachineDisplayRule;

        Vector2 myLocation = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(location.X * 64.0 + 32.0), (float)(location.Y * 64.0 - 16.0)));
        int rectangleWidth = 16;
        //paddingX = 8.0; paddingY = 40.0; for rectangleWidth = 32
        //paddingX = 32.0; paddingY = 16.0; for rectangleWidth = 16
        float paddingX = 32.0f;
        float paddingY = 16.0f;
        float scale = 1f;

        Dictionary<string, LayerRules> LayersData = specialDisplayRule.MachineDisplayRule.LayersData;
            string[] layers = LayersData.Keys.ToArray();
            LayerRules layerRules = LayersData[layers[0]];

        scale = specialDisplayRule.MachineDisplayRule.ScaleMenu;
        rectangleWidth = machineDisplayRule.RectangleSize;
        paddingX = machineDisplayRule.PaddingX;
        paddingY = machineDisplayRule.PaddingY;
        myLocation = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(location.X * 64.0 + paddingX), (float)(location.Y * 64.0 - paddingY)));


        //1

        DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, layerDepth, layerRules, true);

        //2
        if (layers.Length > 1)
        {
            layerRules = LayersData[layers[1]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //3
        if (layers.Length > 2)
        {
            layerRules = LayersData[layers[2]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //4
        if (layers.Length > 3)
        {
            layerRules = LayersData[layers[3]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //5
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //6
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //7
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //8
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //9
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
        //10
        if (layers.Length > 4)
        {
            layerRules = LayersData[layers[4]];
            DrawLayer(spriteBatch, heldobject, rectangleWidth, myLocation, scale, MathF.BitIncrement(layerDepth), layerRules, true);
        }
    }



    private static void drawSprite(Texture2D texture2D, int spriteIndex, SpriteBatch spriteBatch, int rectangleWidth, Vector2 location, float scaleSize, Color color, float layerDepth, LayerRules rule, string baseColor, bool isMachine)
    {
        int spriteindex = rule.ColorSpriteIndex;
        float colorStrength = rule.DefaultColorIntensity;

        Rectangle rectangle = rectangleWidth > 16
            ? Game1.getSquareSourceRectForNonStandardTileSheet(texture2D, rectangleWidth, rectangleWidth, spriteindex)
            : Game1.getSourceRectForStandardTileSheet(texture2D, spriteindex, rectangleWidth, rectangleWidth);

        float num = 4f * scaleSize;
        Vector2 vector = new Vector2(8f, 8f);

        if (rule.OverrideColors != null && rule.OverrideColors.ContainsKey(baseColor))
        {
            rule.OverrideColors.TryGetValue(baseColor, out var overrideData);
            texture2D = Game1.content.Load<Texture2D>(overrideData.ColorOverrideSpritesheet);
            spriteindex = overrideData.ColorOverrideIndex;
            if(overrideData.TintStength!=-1)
                colorStrength = overrideData.TintStength;
        }
            rectangle = rectangleWidth > 16
                ? Game1.getSquareSourceRectForNonStandardTileSheet(texture2D, rectangleWidth, rectangleWidth, spriteindex)
                : Game1.getSourceRectForStandardTileSheet(texture2D, spriteindex, rectangleWidth, rectangleWidth);

            spriteBatch.Draw(texture2D, location, rectangle, Color.White * 1f, 0f, vector * scaleSize, num, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture2D, location, rectangle, color * colorStrength, 0f, vector * scaleSize, num, SpriteEffects.None, MathF.BitIncrement(layerDepth));
    }
    */
    public static void DrawLayer(SpriteBatch spriteBatch, Vector2 location, float layerDepth, ImageDisplayData rule, string color, string itemId, bool isMachine, bool isMenu)
    {
        Texture2D spritesheet = !string.IsNullOrEmpty(rule.Spritesheet) ? Game1.content.Load<Texture2D>(rule.Spritesheet) : ItemRegistry.GetDataOrErrorItem(itemId).GetTexture();

        int rectangleWidth = rule.RectangleSize.Value;
        int spriteIndex = rule.SpriteIndex.Value;
        float paddingX = rule.PaddingX.Value;
        float paddingY = rule.PaddingY.Value;
        float colorIntensity = rule.ColorOverlayIntensity.Value;
        float scale;
        Color myColor = Color.White;
        if (isMachine)
            scale = rule.ScaleInMachine.Value;
        else scale = isMenu ? rule.ScaleInMenu.Value : rule.ScaleWhenHeld.Value;


        location = isMachine ? location + new Vector2(32f + 32f + paddingX, 32f + 16f + paddingY) * scale : location + new Vector2(32f + paddingX, 32f + paddingY) * scale;


        //float num = 4f * scale;
        // Vector2 vector = new Vector2(8f, 8f);
        if (colorIntensity > 0f)
        {
            myColor = GetColorFromContextTag(color);
        }
        else
        {
            colorIntensity = 1f;
        }

        Rectangle rectangle = rectangleWidth > 16
           ? Game1.getSquareSourceRectForNonStandardTileSheet(spritesheet, rectangleWidth, rectangleWidth, spriteIndex)
           : Game1.getSourceRectForStandardTileSheet(spritesheet, spriteIndex, rectangleWidth, rectangleWidth);

        // spriteBatch.Draw(texture2D, location, rectangle, Color.White * 1f, 0f, vector * scaleSize, num, SpriteEffects.None, layerDepth);
        spriteBatch.Draw(spritesheet, location, rectangle, myColor * colorIntensity, 0f, new Vector2(8f, 8f) * scale, 4f * scale, SpriteEffects.None, layerDepth);
    }

}

