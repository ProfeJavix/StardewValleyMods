using System.ComponentModel;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace TreeOrganizer
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig config;
        private IModHelper helper;

        private bool movingTree = false;
        private TerrainFeature? curTree = null;

        private Rectangle greenTileSrc = new(194, 388, 16, 16);
        private Rectangle redTileSrc = new(210, 388, 16, 16);

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            this.helper = helper;

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!config.ModEnabled || !Context.IsPlayerFree || !Game1.currentLocation.IsFarm)
                return;

            if (!movingTree && config.GrabTreeKey.JustPressed())
            {
                if (Game1.currentLocation.terrainFeatures.TryGetValue(e.Cursor.GrabTile, out TerrainFeature tf) && ((tf is Tree t && t.growthStage.Value >= 5) || (tf is FruitTree ft && ft.growthStage.Value >= 4)))
                {
                    movingTree = true;
                    curTree = tf;
                }
            }
            else if (movingTree)
            {
                if (e.Pressed.Any(b => b.IsActionButton() || b.IsUseToolButton()))
                {
                    Vector2 tile = e.Cursor.Tile;

                    if (curTree != null && canPlaceCurTree(tile, true))
                    {
                        Vector2 prevTile = curTree.Tile;

                        Game1.currentLocation.terrainFeatures.Remove(curTree.Tile);
                        Game1.currentLocation.terrainFeatures.TryAdd(tile, curTree);

                        curTree.Tile = tile;

                        if(Game1.currentLocation.objects.TryGetValue(prevTile, out var obj)){
                            Game1.currentLocation.objects.Remove(prevTile);
                            Game1.currentLocation.objects.TryAdd(tile, obj);
                            obj.TileLocation = tile;
                        }

                        clearCurrentTree();
                    }

                    foreach (SButton b in e.Pressed)
                        helper.Input.Suppress(b);
                }
                else if (e.Pressed.Contains(SButton.Escape))
                {
                    clearCurrentTree();

                    foreach (SButton b in e.Pressed)
                        helper.Input.Suppress(b);
                }
            }
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (!config.ModEnabled || !Context.IsPlayerFree || !movingTree || curTree == null)
                return;

            Vector2 tile = Game1.currentCursorTile;
            bool canPlace = canPlaceCurTree(tile);

            e.SpriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(tile * 64), canPlace ? greenTileSrc : redTileSrc, Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);

            drawTreeAtTile(e.SpriteBatch, tile, canPlace);
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            clearCurrentTree();
        }
        
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if(configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => helper.WriteConfig(config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Enable Mod",
                getValue: () => config.ModEnabled,
                setValue: val => config.ModEnabled = val,
                tooltip: () => "Enable/Disable the mod"
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Grab Tree Key",
                getValue: () => config.GrabTreeKey,
                setValue: val => config.GrabTreeKey = val,
                tooltip: () => "Key to grab and move the trees."
            );
        }

        private bool canPlaceCurTree(Vector2 tile, bool showError = false)
        {
            if (curTree == null)
            {
                return false;
            }

            string id = "";
            if (curTree is FruitTree ft)
            {
                id = ft.treeId.Value;
            }
            else if (curTree is Tree t)
            {
                id = "259";
            }

            Item i = ItemRegistry.Create("(O)" + id, 1);

            return i.canBePlacedHere(Game1.currentLocation, tile, showError: showError);
        }

        private void clearCurrentTree(){
            movingTree = false;
            curTree = null;
        }

        private void drawTreeAtTile(SpriteBatch b, Vector2 tile, bool canPlace){

            Rectangle srcRect;
            Color color = canPlace? Color.LightGreen: Color.Red;


            if(curTree is Tree t){

                if(t.texture == null || !Tree.TryGetData(t.treeType.Value, out var data)){
                    return;
                }

                srcRect = Tree.stumpSourceRect;
                if (t.hasMoss.Value)
			    {
				    srcRect.X += 96;
			    }
                b.Draw(t.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64, tile.Y * 64 - 64)), srcRect, color * 0.25f, 0, Vector2.Zero, 4f, t.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);

                srcRect = Tree.treeTopSourceRect;

                if ((data.UseAlternateSpriteWhenSeedReady && t.hasSeed.Value) || (data.UseAlternateSpriteWhenNotShaken && !t.wasShakenToday.Value))
				{
					srcRect.X = 48;
				}
				else
				{
					srcRect.X = 0;
				}

                if(t.hasMoss.Value){
                    srcRect.X = 96;
                }

                b.Draw(t.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64 + 32, tile.Y * 64 + 64)), srcRect, color * 0.35f, 0, new Vector2(24, 96), 4, t.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None,  1f);

            }else if(curTree is FruitTree ft){

                int spriteRow = ft.GetSpriteRowNumber();
                Season cosmeticSeason = ft.GetCosmeticSeason();

				if (!ft.falling.Value)
				{
                    srcRect = new Rectangle((12 + (int)cosmeticSeason * 3) * 16, spriteRow * 5 * 16 + 64, 48, 16);
					b.Draw(ft.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64 + 32, tile.Y * 64 + 64)), srcRect, color * 0.35f, 0, new Vector2(24, 16), 4, ft.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
				}

                srcRect = new Rectangle((12 + (int)cosmeticSeason * 3) * 16, spriteRow * 5 * 16, 48, 64);
				b.Draw(ft.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64 + 32, tile.Y * 64 + 64)), srcRect, color * 0.35f, 0, new Vector2(24f, 80f), 4f, ft.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);

                srcRect = new Rectangle(384, spriteRow * 5 * 16 + 48, 48, 32);
                
                b.Draw(ft.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64 + 32, tile.Y * 64 + 64)), srcRect, color * 0.25f, 0, new Vector2(24, 32), 4, ft.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
            }
            
        }
    }
}
