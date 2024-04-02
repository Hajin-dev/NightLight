using System;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace NightLight
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        // Config fields
        private ModConfig Config = new();

        /*********
        ** Public methods
        *********/
        // <summary>The mod entry point, called after the mod is first loaded.</summary>
        // <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            // Set the mod's config
            Config = helper.ReadConfig<ModConfig>();

            // Set up events
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonsChanged += ButtonsChanged;
            helper.Events.GameLoop.UpdateTicked += GameUpdated;
        }

        private void GameUpdated(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;

            if (Config.NightLightEnabled) {
                handleLighting();
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller. This includes mouse clicks. If the player pressed/released multiple keys at once, this is only raised once.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Config.NightLightEnabled) {
                /*
                 ### Toggle NightLight if configured hotkey button is pressed ###
                */

                // Outdoors only 
                if (this.Config.NightLightToggleOutdoorsKey.JustPressed()) {
                    // Outdoors
                    if (this.Config.NightLightOutdoors == true) {
                        this.Config.NightLightOutdoors = false;
                    } else {
                        this.Config.NightLightOutdoors = true;
                    }

                    this.Helper.WriteConfig(this.Config);
                }

                // Mines only
                if (this.Config.NightLightToggleMinesKey.JustPressed()) {
                    // Mines
                    if (this.Config.NightLightMines == true) {
                        this.Config.NightLightMines = false;
                    } else {
                        this.Config.NightLightMines = true;
                    }

                    this.Helper.WriteConfig(this.Config);
                }

                // Outdoors & Mines
                if (this.Config.NightLightToggleAllKey.JustPressed()) {
                    // Outdoors
                    if (this.Config.NightLightOutdoors == true) {
                        this.Config.NightLightOutdoors = false;
                    } else {
                        this.Config.NightLightOutdoors = true;
                    }

                    // Mines
                    if (this.Config.NightLightMines == true) {
                        this.Config.NightLightMines = false;
                    } else {
                        this.Config.NightLightMines = true;
                    }

                    this.Helper.WriteConfig(this.Config);

                }
            }
        }

        private void handleLighting() {

            // Toggle outdoors lighting
            if (Config.NightLightOutdoors) {

                // Get a reference to the game's ambientLight
                IReflectedField<Color> ambientLight = this.Helper.Reflection.GetField<Color>(typeof(Game1), "ambientLight");
                
                ambientLight.SetValue(Color.Transparent);

            }

            // Toggle lighting within the mines
            if (Config.NightLightMines) {
                if (Game1.currentLocation.Name.StartsWith("UndergroundMine")) {
                    Game1.drawLighting = false;
                }
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e) {
            // Get GMCM API
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            // Make sure it was found
            if (configMenu is null) {
                return;
            }

            // Register NightLight Mod
            configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
            );

            // Config Options
            // Enable/Disable
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable NightLight",
                tooltip: () => "Enables NightLight",
                getValue: () => this.Config.NightLightEnabled,
                setValue: value => this.Config.NightLightEnabled = value
            );

            // Section Title For Areas
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Areas To Light",
                tooltip: () => "Toggle which areas you would like to light up."
            );

            // Outdoors NightLight
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Outdoors",
                tooltip: () => "Enables light while outside at all times.",
                getValue: () => this.Config.NightLightOutdoors,
                setValue: value => this.Config.NightLightOutdoors = value
            );

            // Mines NightLight
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mines",
                tooltip: () => "Enables light on all floors of the mines (most notable on floors 30-39 of the regular mines).",
                getValue: () => this.Config.NightLightMines,
                setValue: value => this.Config.NightLightMines = value
            );

            // Section Title For Hotkeys
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Hotkeys",
                tooltip: () => "View or change the hotkeys to light up one or more areas."
            );

            // Toggle All Hotkey
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle All",
                tooltip: () => "Set the keybind to toggle NightLight on/off for both outdoors and the mines.",
                getValue: () => this.Config.NightLightToggleAllKey,
                setValue: value => this.Config.NightLightToggleAllKey = value
            );

            // Toggle Outdoors Only Hotkey
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle Outdoors",
                tooltip: () => "Set the keybind to toggle NightLight on/off for outdoors only.",
                getValue: () => this.Config.NightLightToggleOutdoorsKey,
                setValue: value => this.Config.NightLightToggleOutdoorsKey = value
            );

            // Toggle Mines Only Hotkey
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle Mines",
                tooltip: () => "Set the keybind to toggle NightLight on/off for mines only.",
                getValue: () => this.Config.NightLightToggleMinesKey,
                setValue: value => this.Config.NightLightToggleMinesKey = value
            );
        }
    }
}