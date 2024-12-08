
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;


namespace MultiFlavor.Utils
{
    public class TagUtils
    {

        internal static IMonitor Monitor { get; set; }
        internal static IModHelper Helper { get; set; }
        public static readonly string selphExtraContextTags = "selph.ExtraMachineConfig.ExtraContextTags";
        public static readonly string BzpAllergies_MadeWith = "BarleyZP.BzpAllergies_MadeWith";
        public static string CachedDescription = "dream.CachedDescription";
        public static string CachedDisplayName = "dream.CachedDisplayName";
        public static string CachedDynamicTags = "dream.CachedDynamicTags";
        public static string ErrorMessage = "ERROR";

        public TagUtils(IModHelper helper, IMonitor monitor)
        {
            Monitor = monitor;
            Helper = helper;
        }

        public static List<string> GetPermittedContextTags(string ContextTags)
        {
            return ContextTags.Split(',').Where(tag => !tag.StartsWith("!")).ToList();
        }

        public static List<string> GetBannedContextTags(string ContextTags)
        {
            return ContextTags.Split(',').Where(tag => tag.StartsWith("!")).ToList();
        }

        public static bool TagsMatch(string ContextTags, HashSet<string> itemContextTags)
        {
            List<string> includes = ContextTags.Split(',').Where(tag => !tag.StartsWith("!")).ToList();
            List<string> excludes = ContextTags.Split(',').Where(tag => tag.StartsWith("!")).ToList();

            return includes.All(itemContextTags.Contains) && !excludes.Any(itemContextTags.Contains);
        }

        public static StardewValley.Object AssignNewContextTags(StardewValley.Object item, HashSet<string> newcontexttags)
        {
            foreach (string tag in newcontexttags)
            {
                item.MarkContextTagsDirty();
                if (!item.GetContextTags().Contains(tag))
                {
                    if (item.modData.ContainsKey(CachedDynamicTags))
                    { 
                        if(!item.modData[CachedDynamicTags].Contains(tag))
                            item.modData[CachedDynamicTags] += "," + tag;
                    }
                    else item.modData[CachedDynamicTags] += string.IsNullOrEmpty(item.modData[CachedDynamicTags]) ? tag : "," + tag;
                    item.MarkContextTagsDirty();
                }
            }
            return item;
        }
        public static HashSet<string> CollectMTagsFromCTags(StardewValley.Object itemWithMtags)
        {
            itemWithMtags.MarkContextTagsDirty();
            return itemWithMtags.GetContextTags().Where(tag => tag.StartsWith("_mtag.")).ToHashSet();
        }

        public static HashSet<string> CollectMTagsFromModData(StardewValley.Object itemWithMtags)
        {
            HashSet<string> mtag = new HashSet<string>();
            if (itemWithMtags.modData is not null)
            {
                foreach (var modKey in itemWithMtags.modData.Keys) 
                {
                    if (modKey.StartsWith("_mtag."))
                        mtag.Add(modKey);
                }
            }
            return mtag;
        }

        public static HashSet<string> CollectPersistentContextTags(StardewValley.Object itemWithMtags)
        {
            itemWithMtags.MarkContextTagsDirty();
            return itemWithMtags.GetContextTags()
                .Where(tag => AssetHandler.ConditionalTags
                   .Where(ct => ct.Value.CopyToOutputObject.Value)
                   .Select(ct => ct.Key)
               .Contains(tag)).ToHashSet();
        }
        public static Dictionary<string, string> GenerateMtags(Item item, string machineID)
        {
            //Monitor.Log("inside GenerateMtags for object: " + item.ItemId, (LogLevel)2);
            Dictionary<string, string> mtags = new Dictionary<string, string>();
            HashSet<string> itemContextTags = CollectMTagsFromCTags((StardewValley.Object)item);

            foreach (string tag in itemContextTags)
            {
                if (AssetHandler.Recipes[machineID].ContainsKey(item.ItemId)) continue;

                var obj = item as StardewValley.Object;
                string itemData = $"{item.ItemId}//{getColorTagFromItem(item)}//{obj.preservedParentSheetIndex}";

                if (item.modData.ContainsKey(tag))
                {
                   if (!item.modData[tag].Contains(itemData))
                       item.modData[tag] = item.modData[tag] + "||" + itemData;
                }
                else
                   item.modData[tag] = itemData;
            }

            foreach (string modTag in item.modData.Keys.Where(key => key.StartsWith("_mtag.")))
                mtags[modTag] = item.modData[modTag];

            return mtags;
        }

        public static Dictionary<string, string> MergeMTagData(Dictionary<string, string> outputDict, Dictionary<string, string> inputDict)
        {
            foreach (var kvp in inputDict)
            {
                if (outputDict.TryGetValue(kvp.Key, out string existingValue))
                {
                    if (!existingValue.Contains(kvp.Value))
                        outputDict[kvp.Key] = existingValue + "||" + kvp.Value;
                }
                else
                    outputDict[kvp.Key] = kvp.Value;
            }
            return outputDict;
        }

        public static string getColorTagFromItem(Item item)
        {
            if (item.modData.TryGetValue("dream.multiflavor_Color", out string outputColor) && !string.IsNullOrEmpty(outputColor))
                return outputColor;
            else
            {
                string foundColor = item.GetContextTags().FirstOrDefault(tag => tag.StartsWith("color_"));
                if (!string.IsNullOrEmpty(foundColor))
                    return foundColor;
            }
            return "color_gray";
        }

        public static StardewValley.Object AddLayerData(StardewValley.Object myOutputObject, Dictionary<string, List<string>> layerData)
        {
            foreach (var layerKey in layerData.Keys)
                myOutputObject.modData[layerKey] = string.Join("||",layerData[layerKey].SelectMany(mtag => myOutputObject.modData[mtag].Split("||")).ToHashSet());
            return myOutputObject;
        }
        public static StardewValley.Object SetUpFlavoredName(StardewValley.Object myOutputObject)
        {
            string objectName = myOutputObject.Name;

            foreach (var key in myOutputObject.modData.Keys.Where(k => k.StartsWith("_mtag")))
            {
                var mtagData = getMtagData(myOutputObject.modData[key]);
                if (mtagData == null) continue;

                if (mtagData.TryGetValue("Ids", out var ids) && mtagData.TryGetValue("PreserveParentSheets", out var preserves))
                {
                    ids = ids.Distinct().ToList();
                    ids.AddRange(preserves.Where(s => !s.Equals("null")));
                    ids.Sort();
                    foreach (var id in ids)
                        objectName += "_" + id;
                }
            }
            myOutputObject.Name = objectName;
            return myOutputObject;
        }
        public static StardewValley.Object SetUpFlavoredDisplayName(StardewValley.Object myOutputObject)
        {
            if (AssetHandler.ConditionalDisplayNames.ContainsKey(myOutputObject.ItemId))
            {
                var OutputItemDisplayRules = AssetHandler.ConditionalDisplayNames[myOutputObject.ItemId];
                string flavoredNameFormat = myOutputObject.DisplayName;

                foreach (var kvp in OutputItemDisplayRules)
                {
                    HashSet<string> contexttags = myOutputObject.modData.Keys.ToHashSet();
                    contexttags.UnionWith(myOutputObject.GetContextTags());
                    if (ValidateRuleWithPassedTags(kvp.Value.Conditions, myOutputObject, contexttags).Count > 0)
                    {
                        flavoredNameFormat = kvp.Value.DisplayNameFormat;
                        foreach (var mtagToReplace in kvp.Value.ReplacementRules)
                        {
                            string[] parts = mtagToReplace.Key.Split("|=");

                            try
                            {
                                var mtagData = getMtagData(myOutputObject.modData[parts[0]]);
                                List<string> ids = mtagData["Ids"];
                                Dictionary<string, string> idsAndDisplayNames = mtagToReplace.Value;
                                if (flavoredNameFormat.Contains(mtagToReplace.Key))
                                {
                                    int index = parts.Length > 1 ? int.Parse(parts[1]) - 1 : 0;
                                    string myitemID = mtagData["Ids"][index];
                                    string replacementText = "ERRORTEXT";
                                    if (idsAndDisplayNames.ContainsKey(myitemID))
                                    {
                                        replacementText = idsAndDisplayNames[myitemID];
                                        if (replacementText.Contains("{0}"))
                                        {
                                            string preserveId = mtagData["PreserveParentSheets"][index];
                                            if (idsAndDisplayNames.ContainsKey(myitemID + "|=" + preserveId))
                                                replacementText = idsAndDisplayNames[myitemID + "|=" + preserveId];
                                            else if (Game1.objectData.TryGetValue(preserveId, out var preserveData) && preserveData is not null)
                                                 replacementText = replacementText.Replace("{0}", preserveData.DisplayName);
                                        }
                                    }
                                    else if (Game1.objectData.TryGetValue(myitemID, out var itemData) && itemData is not null)
                                        replacementText = itemData.DisplayName;
                                    else
                                        Monitor.Log("itemData is null for "+ myitemID, (LogLevel)2);

                                    flavoredNameFormat = flavoredNameFormat.Replace(mtagToReplace.Key, replacementText);
                                }
                            }
                            catch(Exception ex)
                            {
                                Monitor.Log("Exception: " + ex, LogLevel.Error);
                            }
                        }
                        break;
                    }
                }
                myOutputObject.displayNameFormat = flavoredNameFormat;
            }

            return myOutputObject;
        }

        public static List<string> ValidateRuleWithPassedTags(GenericConditionalData conditions, StardewValley.Object itemObject, HashSet<string> contexttags)
        {
            var passedTags = new List<string>();
            if (!string.IsNullOrEmpty(conditions.AnyTagsPresent))
            {
                var anyTags = conditions.AnyTagsPresent.Split(",").Select(s => s.Trim()).ToList();
                var matchingTags = anyTags.Where(item => contexttags.Contains(item)).ToList();
                if (!matchingTags.Any())
                    return new List<string>();
                passedTags.AddRange(matchingTags);
            }

            if (!string.IsNullOrEmpty(conditions.AllTagsPresent))
            {
                var allTags = conditions.AllTagsPresent.Split(",").Select(s => s.Trim()).ToList();
                var matchingTags = allTags.Where(item => contexttags.Contains(item)).ToList();
                if (matchingTags.Count != allTags.Count)
                    return new List<string>();  
                else
                    passedTags.AddRange(matchingTags); 
            }


            if (!string.IsNullOrEmpty(conditions.NoTagsPresent))
            {
                var noTags = conditions.NoTagsPresent.Split(",").Select(s => s.Trim()).ToList();
                var matchingTags = noTags.Where(item => contexttags.Contains(item)).ToList();
                if (matchingTags.Any()) 
                    return new List<string>();  
            }

            if (conditions.MultipleObjectsUnderOutputMTag is not null && conditions.MultipleObjectsUnderOutputMTag.Count > 0)
            {
                Monitor.Log("MultipleObjectsUnderMTag. is not null  ", (LogLevel)2);
                foreach (var rule in conditions.MultipleObjectsUnderOutputMTag)
                {
                    Monitor.Log("itemObject.modData[rule.Key.Trim()]  "+ itemObject.modData[rule.Key.Trim()], (LogLevel)2);
                    Monitor.Log("rule.Value  " + rule.Value, (LogLevel)2);
                    string[] subItems = itemObject.modData[rule.Key.Trim()].Split("||");
                    if (subItems.Length != rule.Value)
                        return new List<string>();
                    else
                        passedTags.Add(rule.Key.Trim());
                }
            }

            if (conditions.ItemIdPresentUnderOutputMTag is not null && conditions.ItemIdPresentUnderOutputMTag.Count > 0)
            {
                foreach (var rule in conditions.ItemIdPresentUnderOutputMTag)
                {
                    if (!itemObject.modData[rule.Key.Trim()].Contains(rule.Value.Trim()))
                        return new List<string>();
                    else
                        passedTags.Add(rule.Key.Trim());
                }
            }
            return passedTags;
        }


        public static Dictionary<string, List<string>> getMtagData(string mtag)
        {
            var outputData = new Dictionary<string, List<string>>
    {
        { "Ids", new List<string>() },
        { "Colors", new List<string>() },
        { "PreserveParentSheets", new List<string>() },
    };

            foreach (var ingredient in mtag.Split("||").Select(data => data.Split("//")).Where(ingredient => ingredient.Length == 3))
            {
                outputData["Ids"].Add(ingredient[0]);
                outputData["Colors"].Add(ingredient[1]);
                outputData["PreserveParentSheets"].Add(ingredient[2]);
            }

            return outputData;
        }

        public static StardewValley.Object SetUpModData(StardewValley.Object myOutputObject, Item replacedHeldObject, Dictionary<string, string> combinedMtags)
        {
            if (replacedHeldObject.modData.TryGetValue(BzpAllergies_MadeWith, out string allergyValue))
                myOutputObject.modData[BzpAllergies_MadeWith] = allergyValue;

            foreach (var key in replacedHeldObject.modData.Keys)
            {
                if (!key.StartsWith("_mtag.") && !key.StartsWith("dream.Cached"))
                    myOutputObject.modData[key] = replacedHeldObject.modData[key];
            }

            foreach (var kvp in combinedMtags)
                myOutputObject.modData[kvp.Key] = kvp.Value;

            myOutputObject.MarkContextTagsDirty();

            return myOutputObject;
        }

        public static StardewValley.Object CastColoredObject(StardewValley.Object outputObject)
        {
            ColoredObject newColoredObject = new ColoredObject(
            outputObject.ItemId,
            outputObject.Stack,
            Color.White
            );
            Helper.Reflection.GetMethod(newColoredObject, "GetOneCopyFrom").Invoke(outputObject);
            newColoredObject.Stack = outputObject.Stack;
            return newColoredObject;
        }

    }
}
