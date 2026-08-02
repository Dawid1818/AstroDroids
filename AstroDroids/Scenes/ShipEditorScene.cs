using AstroDroids.Data;
using AstroDroids.Drawables;
using AstroDroids.Entities.Friendly;
using AstroDroids.Gameplay;
using AstroDroids.Graphics;
using AstroDroids.Input;
using AstroDroids.Managers;
using AstroDroids.Screens;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AstroDroids.Scenes
{
    public class ShipEditorScene : Scene
    {
        ShipCustomizationScreenGum ui;

        CoroutineManager coroutineManager = new CoroutineManager();

        ShipCustomization customization = new ShipCustomization();

        int curPart = 0;

        private const int TrackWidth = 256;
        private const int TrackHeight = 16;

        Texture2D saturationTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] satColors = new Color[TrackWidth * TrackHeight];

        Texture2D hueTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] hueColors = new Color[TrackWidth * TrackHeight];

        Texture2D valueTrack = new Texture2D(Screen.GetGraphicsManager().GraphicsDevice, TrackWidth, TrackHeight);
        Color[] valueColors = new Color[TrackWidth * TrackHeight];

        public ShipEditorScene()
        {

        }

        void UpdateTracks(float currentHue, float currentSat, float currentValue)
        {
            for (int x = 0; x < TrackWidth; x++)
            {
                float t = x / (float)(TrackWidth - 1);

                float hueAtX = t * 360f;
                Color hueCol = Color.FromHSV(hueAtX, currentSat, currentValue);

                Color satCol = Color.FromHSV(currentHue, t, currentValue);

                Color valCol = Color.FromHSV(currentHue, currentSat, t);

                for (int y = 0; y < TrackHeight; y++)
                {
                    int index = x + (y * TrackWidth);
                    hueColors[index] = hueCol;
                    satColors[index] = satCol;
                    valueColors[index] = valCol;
                }
            }

            hueTrack.SetData(hueColors);
            saturationTrack.SetData(satColors);
            valueTrack.SetData(valueColors);
        }

        public override void Set()
        {
            Screen.GumUI.Root.Children.Clear();

            ui = new ShipCustomizationScreenGum();
            ui.AddToRoot();

            //ui.PartChanged += Ui_PartChanged;
            //ui.ColorChanged += Ui_ColorChanged;

            ui.SatTrack.Texture = saturationTrack;
            ui.ValTrack.Texture = valueTrack;
            ui.HueTrack.Texture = hueTrack;

            if (World == null)
                World = new GameWorld();

            //GameStateManager.NewState();

            List<Texture2D> starfields = TextureManager.GetStarfields();
            World.Starfield = new ImageStarfield(starfields[0]);

            World.AddPlayer(new Player(0, new Vector2(World.Bounds.Width / 2 - 16, World.Bounds.Height / 2 - 16)));

            Screen.ResetCamera();

            ShipColor targetColor = customization.GetColorByPart(0);
            UpdateTracks(targetColor.Hue, targetColor.Saturation, targetColor.Value);
            ui.SetRGB(targetColor);
        }

        private void Ui_ColorChanged(float hue, float sat, float val)
        {
            customization.SetColorByPart(curPart, new ShipColor() { Hue = hue, Saturation = sat, Value = val });

            World.GetPlayers()[0].ApplyCustomization(customization);

            UpdateTracks(hue, sat, val);
        }

        private void Ui_PartChanged(int part)
        {
            curPart = part;
            ShipColor targetColor = customization.GetColorByPart(part);
            UpdateTracks(targetColor.Hue, targetColor.Saturation, targetColor.Value);
            ui.SetRGB(targetColor);
        }

        public override void Update(GameTime gameTime)
        {
            if(InputSystem.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
            {
                if (saturationTrack != null)
                    saturationTrack.Dispose();

                if (valueTrack != null)
                    valueTrack.Dispose();

                if (hueTrack != null)
                    hueTrack.Dispose();
                SceneManager.SetScene(new ShipEditorScene());
            }

            coroutineManager.Update(gameTime);

            World.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.ScreenWidth, Screen.ScreenHeight, 0, 0, 1);
            Matrix uv_transform = Screen.GetUVTransform(TextureManager.GetStarfield(), new Vector2(0, 0), 1f, Screen.Viewport);

            Screen.Infinite.Parameters["view_projection"].SetValue(projection);
            Screen.Infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));

            World.Draw(gameTime);
        }

        public override void DrawDebug(GameTime gameTime)
        {
            if (World != null)
                World.DrawDebug();
        }
    }
}
