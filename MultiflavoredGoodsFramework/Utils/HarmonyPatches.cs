using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using StardewValley;
using System.Threading;
using StardewValley.GameData.Machines;
using System.Data;
using xTile.Layers;
using StardewValley.Extensions;
using System.Collections;
using System.Xml.Linq;
using StardewValley.SpecialOrders;
using System.Threading.Tasks.Dataflow;
using System.IO;

namespace MultiFlavor.Utils
{
    internal class HarmonyPatches
    {
        internal static IMonitor Monitor { get; set; }

        public HarmonyPatches(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;

            harmony.Patch(
                 original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.PlaceInMachine)),
                 prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaceInMachine_Prefix)),
                 postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PlaceInMachine_Postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.getDescription)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(GetDescription_Postfix)));
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(StardewValley.Object), nameof(StardewValley.Object.DisplayName)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(DisplayName_Postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(Item),"_PopulateContextTags"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PopulateContextTags_postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.draw), new[]{typeof (SpriteBatch), typeof(int), typeof(int), typeof(float)}),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ObjectDraw_prefix)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ObjectDraw_postfix)) );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(ColoredObject.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ObjectDraw_prefix)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ObjectDraw_postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object),nameof(StardewValley.Object.drawInMenu),new Type[] {typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_drawInMenu_prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object),nameof(StardewValley.Object.drawWhenHeld)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_drawWhenHeld_prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ColoredObject), nameof(ColoredObject.drawInMenu), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ColoredObject_drawInMenu_prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(ColoredObject), nameof(ColoredObject.drawWhenHeld)),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_drawWhenHeld_prefix))); 
        }


        public static void PlaceInMachine_Prefix(StardewValley.Object __instance, Farmer who, bool probe, out Dictionary<string, Item>? __state)
        {
            try
            {
                if (!probe)
                {
                    __state = InventoryUtils.GetInventoryItemLookup(StardewValley.Object.autoLoadFrom ?? who.Items);
                    __state.Add("OldHeldObject", __instance.heldObject.Value);
                }else __state =null;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(PlaceInMachine_Prefix)}:\n{ex}", LogLevel.Error);
                __state = null;
            }
        }

        [HarmonyPriority(1000)]
        public static void PlaceInMachine_Postfix(Farmer who, Dictionary<string, Item>? __state, StardewValley.Object __instance)
        {
            try
            {

                if (__state is null) return;
                Item heldObject = __state["OldHeldObject"];
                __state.Remove("OldHeldObject");
                if (__state.Count < 1) return;
                Dictionary<string, Item> afterConsume = InventoryUtils.GetInventoryItemLookup(StardewValley.Object.autoLoadFrom ?? who.Items);
                List<Item> spentItems = InventoryUtils.InventoryUsedItems(__state, afterConsume);
                StardewValley.Object placeholderObject = __instance.heldObject.Value;
                HashSet<string> copyToOutputContextTags = new HashSet<string>();

                if (placeholderObject is not null && (Game1.bigCraftableData[__instance.ItemId].CustomFields is null || !Game1.bigCraftableData[__instance.ItemId].CustomFields.ContainsKey("dream.multiflavor")))
                {
                    copyToOutputContextTags.UnionWith(spentItems.OfType<StardewValley.Object>().SelectMany(TagUtils.CollectPersistentContextTags));
                    placeholderObject = TagUtils.AssignNewContextTags(placeholderObject, copyToOutputContextTags);
                    return;
                }

                if (spentItems.Count < 1) return;

                if (placeholderObject is null || __instance.readyForHarvest.Value) return;

                Dictionary<string, string> combinedMtags = new Dictionary<string, string>();
                HashSet<string> allContextMTags = new HashSet<string>();
                int Price = 0;
                int Quality = 0;

                //collect data from all objects
                foreach (Item item in spentItems)
                {
                    if (item == null) continue;
                    StardewValley.Object myItemObject = item as StardewValley.Object;
                    allContextMTags.UnionWith(TagUtils.CollectMTagsFromCTags(myItemObject));
                    allContextMTags.UnionWith(TagUtils.CollectMTagsFromModData(myItemObject));
                    combinedMtags = TagUtils.MergeMTagData(combinedMtags, TagUtils.GenerateMtags(item, __instance.ItemId));
                    copyToOutputContextTags.UnionWith(TagUtils.CollectPersistentContextTags(myItemObject));
                    Quality += myItemObject.Quality;
                    Price += item.sellToStorePrice();
                 //   Monitor.Log("/////////////////////////////////////////// ", (LogLevel)2);
                //    Monitor.Log("input object: : " + item.ItemId, (LogLevel)2);
                //    Monitor.Log("input quality: : " + myItemObject.Quality, (LogLevel)2);
                    //Monitor.Log("input object context tags: " + string.Join(", ", item.GetContextTags()), (LogLevel)2);
                    //Monitor.Log("input object moddata tags: " + string.Join(", ", item.modData.Keys), (LogLevel)2);
                }
                Quality = Quality/spentItems.Count;
              //  Monitor.Log("input quality / count: : " + Quality, (LogLevel)2);

                if (heldObject is not null && heldObject is StardewValley.Object hobject)
                {
                    heldObject.modData["dream.placeholderOutput"] = heldObject.ItemId;
                    Dictionary<string, string> filteredDict = new Dictionary<string, string>(); 
                    foreach (var key in heldObject.modData.Keys)
                    {
                        if (key.StartsWith("_mtag."))
                            filteredDict.Add(key, heldObject.modData[key]);
                    }
                    combinedMtags = TagUtils.MergeMTagData(combinedMtags, filteredDict);
                    allContextMTags.UnionWith(TagUtils.CollectMTagsFromModData((StardewValley.Object)heldObject));
                    copyToOutputContextTags.UnionWith(TagUtils.CollectPersistentContextTags(hobject));

                    Monitor.Log("Price: : " + Price, (LogLevel)2);
                    Monitor.Log("heldObject.Price: : " + hobject.Price, (LogLevel)2);
                    Price = hobject.Price + Price;
                    Monitor.Log("Quality: : " + Quality, (LogLevel)2);
                    Monitor.Log("heldObject.Quality: : " + hobject.Quality, (LogLevel)2);
                    Quality = (heldObject.Quality + Quality) / 2;

                   // Monitor.Log("/////////////////////////////////////////// ", (LogLevel)2);
                  //  Monitor.Log("held object: : " + heldObject.ItemId, (LogLevel)2);
                  //  Monitor.Log("held object context tags: " + string.Join(", ", heldObject.GetContextTags()), (LogLevel)2);
                  //  Monitor.Log("held object moddata tags: " + string.Join(", ", heldObject.modData.Keys), (LogLevel)2);
                }
                else
                {
                  //  Monitor.Log("/////////////////////////////////////////// ", (LogLevel)2);
                  //  Monitor.Log("held object: : " + placeholderObject.ItemId, (LogLevel)2);
                  //  Monitor.Log("held object context tags: " + string.Join(", ", placeholderObject.GetContextTags()), (LogLevel)2);
                  //  Monitor.Log("held object moddata tags: " + string.Join(", ", placeholderObject.modData.Keys), (LogLevel)2);
                    heldObject = placeholderObject;
                    Price = placeholderObject.Price;
                    Quality = placeholderObject.Quality;
                }


                //find the recipe for the new output object
                Dictionary <string, OutputObjectData> outputObjectData = AssetHandler.Recipes[__instance.ItemId];

                Dictionary<string, List<string>> layersData = new Dictionary<string, List<string>>();
                string myRecipe = null;

                StardewValley.Object tmpObject = new StardewValley.Object(heldObject.ItemId, 1, false, -1, 0);
                tmpObject = TagUtils.SetUpModData(tmpObject, heldObject, combinedMtags);

             //   Monitor.Log("/////////////////////////////////////////// ", (LogLevel)2);
             //   Monitor.Log("tmp context tags: " + string.Join(", ", heldObject.GetContextTags()), (LogLevel)2);
             //   Monitor.Log("tmp moddata tags: " + string.Join(", ", heldObject.modData.Keys), (LogLevel)2);
            //    Monitor.Log("tmp allContextMTags: " + string.Join(", ", allContextMTags), (LogLevel)2);



                foreach (var outputObject in outputObjectData)
                {
                     var layers = outputObject.Value.Layers;

                    var tmpLayersData = layers.ToDictionary(
                        layer => layer.Key,
                        layer => TagUtils.ValidateRuleWithPassedTags(layer.Value.RecipeIngredients, tmpObject, allContextMTags)
                    );

                    if (tmpLayersData.Values.All(tags => tags.Count > 0))
                    {
                        myRecipe = outputObject.Key;
                        layersData = tmpLayersData;
                        break;
                    }
                }

                //creade new output object
                StardewValley.Object myOutputObject;
                if (!string.IsNullOrEmpty(myRecipe))
                {
                    myOutputObject = new StardewValley.Object(myRecipe, 1, false, -1, 0);
                    myOutputObject = TagUtils.CastColoredObject(myOutputObject);
                }
                else
                    myOutputObject = (StardewValley.Object)heldObject;


                //set up mod data
                myOutputObject = TagUtils.SetUpModData(myOutputObject, heldObject, combinedMtags);

                foreach (var layerKey in layersData.Keys)
                    myOutputObject.modData[layerKey] = string.Join("||", layersData[layerKey].SelectMany(mtag => myOutputObject.modData[mtag].Split("||")).ToHashSet());

                myOutputObject.Price = Price;
                myOutputObject.Quality = Quality;

                //assign conditional context tags
                myOutputObject = TagUtils.AssignNewContextTags(myOutputObject, copyToOutputContextTags);

                //set up objects name and displayname
                myOutputObject = TagUtils.SetUpFlavoredName(myOutputObject);
                myOutputObject = TagUtils.SetUpFlavoredDisplayName(myOutputObject);

             //   Monitor.Log("/////////////////////////////////////////// ", (LogLevel)2);
              //  Monitor.Log("myOutputObject name: " + myOutputObject.Name, (LogLevel)2);
              //  Monitor.Log("myOutputObject context tags: " + string.Join(", ", myOutputObject.GetContextTags()), (LogLevel)2);
              //  Monitor.Log("myOutputObject moddata tags: " + string.Join(", ", myOutputObject.modData.Keys), (LogLevel)2);


                //replace the heldobject with the new one
                __instance.heldObject.Value = myOutputObject;

            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(PlaceInMachine_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

       
        [HarmonyPriority(1000)]
        static void GetDescription_Postfix(StardewValley.Object __instance, ref string __result)
        {
            if (!__instance.HasTypeObject()) return;

            if (__instance.modData.TryGetValue(TagUtils.CachedDescription, out string? cachedDescription) && !string.IsNullOrWhiteSpace(cachedDescription) && cachedDescription != TagUtils.ErrorMessage)
            {
                __result = cachedDescription;
                return;
            }

            HashSet<string> contexttags = __instance.GetContextTags();

            var matchingKeys = AssetHandler.ConditionalTags.Where(kvp => contexttags.Contains(kvp.Key) && kvp.Value.VisibleInDescription == true).Select(kvp => kvp.Key).ToList();

            if (matchingKeys.Count < 1) return;

            int width = 0;
            try
            {
                width = ModEntry.Helper.Reflection.GetMethod(__instance, "getDescriptionWidth").Invoke<int>();
            }
            catch (Exception e)
            {
                Monitor.Log($"Error reflecting into getDescription: {e.Message}");
                // Stop doing it lol
                __instance.modData[TagUtils.CachedDescription] = "";
                return;
            }

            string descEnd = "";
            string descStart = "";
            string descFinal = "";

            if (__instance.modData.TryGetValue(TagUtils.CachedDescription, out string? cachedDesc))
            {
                if (cachedDesc.Equals("ERROR")) return;
            }
            else
            {
                foreach (var key in matchingKeys)
                {
                    var visibilityOptions = AssetHandler.ConditionalTags[key].VisibilityOptions;
                    switch (visibilityOptions.DescriptionPlacement.ToUpperInvariant())
                    {
                        case "REPLACE": descFinal = visibilityOptions.Description; break;
                        case "START": descStart += " " + visibilityOptions.Description; break;
                        case "END": descEnd += " " + visibilityOptions.Description; break;
                    }
                }
            }
            if (string.IsNullOrEmpty(descFinal))
                descFinal = __result;
            descFinal = Game1.parseText(descStart, Game1.smallFont, width) + Game1.parseText(descFinal, Game1.smallFont, width) + Game1.parseText(descEnd, Game1.smallFont, width);
            __instance.modData[TagUtils.CachedDescription] = descFinal;
            __result = descFinal;
        }

        static void DisplayName_Postfix(StardewValley.Object __instance, ref string __result)
        {
            if (!__instance.HasTypeObject()) return;

            if (__instance.modData.TryGetValue(TagUtils.CachedDisplayName, out string? cachedDisplayname) && !string.IsNullOrWhiteSpace(cachedDisplayname))
           {
               __result = cachedDisplayname;
                return;
            }

            HashSet<string> contexttags = __instance.GetContextTags();

            var matchingKeys = AssetHandler.ConditionalTags.Where(kvp => contexttags.Contains(kvp.Key) && kvp.Value.VisibleInDisplayName == true).Select(kvp => kvp.Key).ToList();

            if (matchingKeys.Count < 1) return;

            string descEnd = "";
            string descStart = "";
            string descFinal = "";

            foreach (var key in matchingKeys)
           {
               var visibilityOptions = AssetHandler.ConditionalTags[key].VisibilityOptions;
               switch (visibilityOptions.DisplayNamePlacement.ToUpperInvariant())
               {
                    case "REPLACE": descFinal = visibilityOptions.DisplayName;  break; 
                    case "START": descStart += " " + visibilityOptions.DisplayName; break;
                    case "END": descEnd += " " + visibilityOptions.DisplayName; break;
               }
            }

            if (string.IsNullOrEmpty(descFinal))
                descFinal = __result;
            __instance.modData[TagUtils.CachedDisplayName] = descStart + descFinal + descEnd;
            __result = descStart + descFinal + descEnd;
        }

        private static void PopulateContextTags_postfix(Item __instance, ref HashSet<string> tags)
        {
            if (!__instance.HasTypeObject()) return;

            if (__instance.modData.TryGetValue(TagUtils.CachedDynamicTags, out string dynamicTags) && !string.IsNullOrEmpty(dynamicTags))
            {
                tags.UnionWith(dynamicTags.Split(","));
                return;
            }

            HashSet<string> localTags = new HashSet<string>();

            foreach (var tag in AssetHandler.ConditionalTags)
            {
                if (TagUtils.ValidateRuleWithPassedTags(tag.Value.Conditions, (StardewValley.Object)__instance, tags).Count > 0)
                    localTags.Add(tag.Key);
            }
            __instance.modData[TagUtils.CachedDynamicTags] = string.Join(",", localTags);
            tags.UnionWith(localTags); 
        }

       
        public static bool ObjectDraw_prefix(StardewValley.Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.TypeDefinitionId.Equals("(O)") && AssetHandler.Recipes.Values.Any(innerDict => innerDict.ContainsKey(__instance.ItemId)))
            {
                Vector2 location = __instance.TileLocation;
                ObjectDraw.drawSpritesObject(__instance, spriteBatch, location, alpha, false);
                return false;
            }
            return true;
        }
        
        public static void ObjectDraw_postfix(StardewValley.Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            /*
            if (__instance.TypeDefinitionId.Equals("(BC)") && AssetHandler.Recipes.ContainsKey(__instance.ItemId))
            {
                if (__instance.heldObject.Value != null)
                {
                    Vector2 locationMachine = __instance.TileLocation;
                    ObjectDraw.drawSpritesMachine(__instance, spriteBatch, locationMachine);
                }
            }
         */
            if (__instance.TypeDefinitionId.Equals("(O)") && AssetHandler.Recipes.Values.Any(innerDict => innerDict.ContainsKey(__instance.ItemId)))
            {
                Vector2 location = __instance.TileLocation;
                ObjectDraw.drawSpritesObject(__instance, spriteBatch, location, alpha, false);
            }
        }
        
        public static bool Object_drawInMenu_prefix(StardewValley.Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.TypeDefinitionId.Equals("(O)") && AssetHandler.Recipes.Values.Any(innerDict => innerDict.ContainsKey(__instance.ItemId)))
            {
                __instance.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
                ObjectDraw.drawSpritesObject(__instance, spriteBatch, location, layerDepth, true);
                __instance.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth + 3E-05f, drawStackNumber, color);
                return false;
            }
            return true;
        }

        public static bool ColoredObject_drawInMenu_prefix(ColoredObject __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color colorOverride, bool drawShadow)
        {
            if (__instance.TypeDefinitionId.Equals("(O)") && AssetHandler.Recipes.Values.Any(innerDict => innerDict.ContainsKey(__instance.ItemId)))
            {
                __instance.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
                ObjectDraw.drawSpritesObject(__instance, spriteBatch, location, layerDepth, true);
                __instance.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth + 3E-05f, drawStackNumber, colorOverride);
                return false;
            }
            return true;
        }
        
        public static bool Object_drawWhenHeld_prefix(StardewValley.Object __instance, SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (__instance.TypeDefinitionId.Equals("(O)") && AssetHandler.Recipes.Values.Any(innerDict => innerDict.ContainsKey(__instance.ItemId)))
            {
                float layerDepth = Math.Max(0f, (f.StandingPixel.Y + 4) / 10000f);
                ObjectDraw.drawSpritesObject(__instance, spriteBatch, objectPosition, layerDepth, false);
                return false;
            }
            return true;
        }
       
    }
}
