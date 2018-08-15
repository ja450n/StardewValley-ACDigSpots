using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;


namespace ACDigSpots
{
    public class ModEntry : Mod, IAssetEditor
    {

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            LocationEvents.LocationsChanged += LocationEvents_LocationsChanged;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }

        /// <summary>The method invoked when the location is changed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void LocationEvents_LocationsChanged(object sender, EventArgsLocationsChanged e)
        {
            getArtifactSpots();
        }

        /// <summary>Private dictionary of all artifact spots found during location change.</summary>
        private Dictionary<Vector2, StardewValley.Object> artifactSpots;

        /// <summary>The method invoked when the game runs an update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (artifactSpots != null)
            {
                foreach (var locationObject in artifactSpots)
                {
                    if (locationObject.Value.parentSheetIndex == 590)
                    {
                        // ArtifactSpot (worms) jiggles based on shakeTimer
                        // setting this on Tick means the timer is never above zero
                        // and cannot shake the object
                        locationObject.Value.shakeTimer = -1;
                    }
                }
            }
        }


        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // change crop seasons
            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                return true;
            }
            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("LooseSprites/Cursors"))
            {
                //asset.ReplaceWith(this.Helper.Content.Load<Texture2D>("TerrainFeatures/hoeDirt", ContentSource.GameContent));
                Texture2D customTexture = this.Helper.Content.Load<Texture2D>("assets/ACDigSpots.png", ContentSource.ModFolder);
                asset
                    .AsImage()
                    .PatchImage(customTexture, targetArea: new Rectangle(368, 32, 64, 16));
            }
        }


        /// <summary>Method to populate itterate through locations and populate dictionary of worm locations.</summary>
        private void getArtifactSpots()
        {
            artifactSpots = new Dictionary<Vector2, StardewValley.Object>();
            foreach (GameLocation location in Game1.locations)
            {
                foreach (var locationObject in location.objects.Pairs)
                {
                    if (locationObject.Value.parentSheetIndex == 590)
                    {
                        artifactSpots.Add(locationObject.Key, locationObject.Value);
                    }
                }
            }
        }
    }
}
