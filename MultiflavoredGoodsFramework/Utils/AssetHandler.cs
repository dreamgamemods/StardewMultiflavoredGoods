using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiFlavor.Utils
{
    public class AssetHandler
    {
        internal static IMonitor monitor;

        private static Dictionary<string, ConditionalTagData>? PrivateConditionalTags = null;
        private static Dictionary<string, Dictionary<string, DisplayNameData>>? PrivateDisplayNames = null;
        private static Dictionary<string, Dictionary<string, OutputObjectData>>? PrivateRecipes = null;
        public static Dictionary<string, ConditionalTagData> ConditionalTags
        {
            get
            {
                if (PrivateConditionalTags == null)
                {
                    PrivateConditionalTags = Game1.content.Load<Dictionary<string, ConditionalTagData>>("dream.Multiflavor/ConditionalContextTags");
                    monitor.Log($"Loaded asset dream.Multiflavor/ConditionalContextTags with {PrivateConditionalTags.Count} entries.");
                }
                return PrivateConditionalTags!;
            }

        }
        public static Dictionary<string, Dictionary<string, DisplayNameData>> ConditionalDisplayNames
        {
            get
            {
                if (PrivateDisplayNames == null)
                {
                    PrivateDisplayNames = Game1.content.Load<Dictionary<string, Dictionary<string, DisplayNameData>>>("dream.Multiflavor/ConditionalDisplayNames");
                    monitor.Log($"Loaded asset dream.Multiflavor/ConditionalDisplayNames with {PrivateDisplayNames.Count} entries.");
                }
                return PrivateDisplayNames!;
            }

        }

        public static Dictionary<string, Dictionary<string, OutputObjectData>> Recipes
        {
            get
            {
                if (PrivateRecipes == null)
                {
                    PrivateRecipes = Game1.content.Load<Dictionary<string, Dictionary<string, OutputObjectData>>>("dream.Multiflavor/Recipes");
                    monitor.Log($"Loaded asset dream.Multiflavor/ConditionalDisplayNames with {PrivateRecipes.Count} entries.");
                }
                return PrivateRecipes!;
            }

        }

        public AssetHandler (IMonitor Monitor)
        {
            monitor = Monitor;
        }
        public void RegisterEvents(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetsInvalidated += this.OnAssetsInvalidated;
        }

        public void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("dream.Multiflavor/ConditionalContextTags"))
                e.LoadFrom(() => new Dictionary<string, ConditionalTagData>(), AssetLoadPriority.Exclusive);

            if (e.NameWithoutLocale.IsEquivalentTo("dream.Multiflavor/Recipes"))
                e.LoadFrom(() => new Dictionary<string, Dictionary<string, OutputObjectData>>(), AssetLoadPriority.Exclusive);

            if (e.NameWithoutLocale.IsEquivalentTo("dream.Multiflavor/ConditionalDisplayNames"))
                e.LoadFrom(() => new Dictionary<string, Dictionary<string, DisplayNameData>>(), AssetLoadPriority.Exclusive);
        }

    public void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            foreach (var name in e.NamesWithoutLocale)
            {
                if (name.IsEquivalentTo("dream.Multiflavor/ConditionalContextTags"))
                {
                    monitor.Log($"Asset dream.Multiflavor/ConditionalContextTags invalidated, reloading.");
                    PrivateConditionalTags = null;
                }
                if (name.IsEquivalentTo("dream.Multiflavor/ConditionalDisplayNames"))
                {
                    monitor.Log($"Asset dream.Multiflavor/ConditionalDisplayNames invalidated, reloading.");
                    PrivateDisplayNames = null;
                }
                if (name.IsEquivalentTo("dream.Multiflavor/Recipes"))
                {
                    monitor.Log($"Asset dream.Multiflavor/Recipes invalidated, reloading.");
                    PrivateDisplayNames = null;
                }
            }
        }
    }

}
