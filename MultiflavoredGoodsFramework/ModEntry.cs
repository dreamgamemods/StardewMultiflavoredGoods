
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

using StardewValley.Delegates;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using HarmonyLib;
using MultiFlavor.Utils;

namespace MultiFlavor;

/// <summary>The mod entry point.</summary>
public sealed class ModEntry : Mod
{
    internal new static IModHelper Helper { get; set; }
    internal static IMonitor monitor { get; set; }

    private HarmonyPatches harmonyPatches;

    TagUtils ContextTagUtils;
    AssetHandler AssetHandler;
    ObjectDraw ObjectDraw;


    public static readonly string UniqueModId = "Dream.MultiFlavor";

    public override void Entry(IModHelper helper)
    {
        Helper = helper;
        monitor = this.Monitor;

        AssetHandler = new AssetHandler(monitor);
        ObjectDraw = new ObjectDraw(monitor);
        AssetHandler.RegisterEvents(Helper);

        var harmony = new Harmony(ModManifest.UniqueID);

        harmonyPatches = new HarmonyPatches(harmony, monitor);

        GameStateQuery.Register($"dream.multiflavor_CheckIfPermitted", CheckIfPermitted);

        ContextTagUtils = new Utils.TagUtils(helper, monitor);
    }



    //"dream_CheckIfMaking <machineID>" checks if the input tags are permitted
    public static bool CheckIfPermitted(string[] query, GameStateQueryContext context)
    {
        if (!ArgUtility.TryGet(query, 1, out var machineId, out var error))
            return false;

        HashSet<string> contextTags = new HashSet<string>();

        foreach (StardewValley.Object obj in context.Location.objects.Values)
        {
            if (!obj.ItemId.Equals(machineId))
                continue;

            Vector2 tile = context.Player.Tile;

            foreach (Vector2 offset in Character.AdjacentTilesOffsets)
            {
                Vector2 adjacentTile = tile + offset;
                if (obj.TileLocation == adjacentTile && obj.heldObject.Value != null)
                    return IsValidInput(context.InputItem, obj);
            }
        }

        return true;
    }

    private static bool IsValidInput(Item inputItem, StardewValley.Object machine)
    {
        if (!AssetHandler.Recipes.ContainsKey(machine.ItemId))
            return false;

        if (!AssetHandler.Recipes[machine.ItemId].ContainsKey(machine.heldObject.Value.ItemId))
            return true;

        var permitConditions = AssetHandler.Recipes[machine.ItemId][machine.heldObject.Value.ItemId].PermitNextInput;
        
        HashSet<string> contexttags = inputItem.GetContextTags();
        contexttags.UnionWith(TagUtils.CollectMTagsFromModData((StardewValley.Object)inputItem));
        if (TagUtils.ValidateRuleWithPassedTags(permitConditions, machine.heldObject.Value, contexttags).Count < 1)
             return false;
       
       return true; 
    }
}

