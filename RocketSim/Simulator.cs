using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace RocketSim {

    public class Simulator : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera2D cam;
        WindowViewportAdapter view;
        SpriteFont font;

        DateTime worldTime; // Time of the simulated world
        int simTicks; // How many simulation ticks per logic frame at 1x time acceleration.
        int timeAcc; // Time acceleration multiplier
        int[] speeds = { 1, 5, 10, 50, 100, 500, 1000 }; //timeAcc presets
        int speedindex;

        Texture2D f9s1, f9s2dragon;

        Earth earth;
        List<Vehicle> vehicles;
        int focusedIndex;
        int camFollowIndex;

        public Simulator() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            worldTime = new DateTime(1, 1, 1, 0, 0, 0, 0);
            simTicks = 10;
            timeAcc = 1;
            speedindex = 0;

            vehicles = new List<Vehicle>();
            focusedIndex = 0;
            camFollowIndex = 0; // -1 for follow nothing
        }

        protected override void Initialize() {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 640;
            Window.Title = "Rocket Simulator";

            view = new WindowViewportAdapter(Window, GraphicsDevice);
            cam = new Camera2D(view);

            earth = new Earth(new Vector2D(500, 6371300), Color.ForestGreen, 8192);

            Vehicle temp = new F9S1("Booster", null, new Vector2D(500, 255), new Vector2D(0, 0), earth, Color.White);
            vehicles.Add(new F9S2Dragon("S2Dragon", temp, new Vector2D(500, 215), new Vector2D(0, 0), earth, Color.White));
            vehicles.Add(temp);
            vehicles[1].child = vehicles[0];
            vehicles[0].offset = new Vector2D(0, -16);

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            f9s1 = Content.Load<Texture2D>("f9s1");
            vehicles[1].SetSprite(f9s1, 4, 45);
            f9s2dragon = Content.Load<Texture2D>("f9s2dragon");
            vehicles[0].SetSprite(f9s2dragon, 4, 20);

            font = Content.Load<SpriteFont>("Verdana");
        }

        protected override void UnloadContent() {
        }

        
        protected override void Update(GameTime gameTime) {

            // HANDLE INPUT:
            KeyboardControl(gameTime);
            if (camFollowIndex < 0)
                MoveCamControl(gameTime);


            // DO SIMULATIONS:
            double dt = TargetElapsedTime.TotalSeconds / simTicks;
            for (int i = 0; i < (simTicks * timeAcc); i++)
                foreach (SimObject obj in vehicles)
                    obj.Simulate(dt);

            //UPDATE CAMERA:
            if (camFollowIndex >= 0)
                cam.LookAt(vehicles[camFollowIndex].pos.ToFloatVec());

            // HANDLE WORLD TIME:
            long worldticks = gameTime.ElapsedGameTime.Ticks * timeAcc;
            worldTime = worldTime.AddTicks(worldticks);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // RENDER TO WORLDSPACE:
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix(), blendState: BlendState.Opaque);
            earth.Render(gameTime, spriteBatch);
            foreach (SimObject obj in vehicles)
                obj.Render(cam, gameTime, spriteBatch, Vector2.Zero);
            spriteBatch.End();

            // RENDER TO SCREENSPACE:
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "T: " + worldTime.Day + "d" + worldTime.Hour + "h" + worldTime.Minute + "m" + worldTime.Second + "s", new Vector2(10, 10), Color.White);
            if (gameTime.IsRunningSlowly)
                spriteBatch.DrawString(font, "LAGGING", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(font, "Sim Speed: " + timeAcc + "x", new Vector2(200, 10), Color.White);
            spriteBatch.DrawString(font, "Precision: " + simTicks + "x", new Vector2(400, 10), Color.White);
            spriteBatch.DrawString(font, "Zoom: " + cam.Zoom, new Vector2(850, 10), Color.White);
            spriteBatch.DrawString(font, "Fuel: " + (vehicles[focusedIndex].GetFuelPercentage()*100).ToString("0.##") + "%", new Vector2(10, 40), Color.White);
            spriteBatch.DrawString(font, "Altitude: " + (vehicles[focusedIndex].GetAltitude(vehicles[focusedIndex].pos) / 1000).ToString("0.###") + " km", new Vector2(10, 70), Color.White);
            spriteBatch.DrawString(font, "Vehicle Speed: " + vehicles[focusedIndex].vel.Length().ToString("0.#") + " m/s", new Vector2(10, 100), Color.White);
            spriteBatch.DrawString(font, "Drag Force: " + (vehicles[focusedIndex].lastDrag/1000).ToString("0.#") + " kN", new Vector2(10, 130), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private KeyboardState kbs;
        private MouseState ms;
        private bool up, down, left, right;
        protected void KeyboardControl(GameTime gameTime) {
            kbs = Keyboard.GetState();
            ms = Mouse.GetState();

            if (kbs.IsKeyDown(Keys.PageUp))
                cam.Zoom *= 1.05f;
            else if (kbs.IsKeyDown(Keys.PageDown))
                cam.Zoom /= 1.05f;

            if (cam.Zoom > 5f)
                cam.Zoom = 5f;
            else if (cam.Zoom < 0.00001f)
                cam.Zoom = 0.00001f;

            if (kbs.IsKeyDown(Keys.Up) && !up) {
                up = true;
                speedindex++;
                if (speedindex >= speeds.Length)
                    speedindex = speeds.Length - 1;
                timeAcc = speeds[speedindex];
            } else if (kbs.IsKeyDown(Keys.Down) && !down) {
                down = true;
                speedindex--;
                if (speedindex < 0)
                    speedindex = 0;
                timeAcc = speeds[speedindex];
            }

            if (kbs.IsKeyDown(Keys.Right) && !right) {
                right = true;
                simTicks++;
            } else if (kbs.IsKeyDown(Keys.Left) && !left) {
                left = true;
                simTicks--;
                if (simTicks < 1)
                    simTicks = 1;
            }

            if (kbs.IsKeyUp(Keys.Up))
                up = false;
            if (kbs.IsKeyUp(Keys.Down))
                down = false;
            if (kbs.IsKeyUp(Keys.Right))
                right = false;
            if (kbs.IsKeyUp(Keys.Left))
                left = false;
        }

        protected void MoveCamControl(GameTime gameTime) {
            kbs = Keyboard.GetState();
            ms = Mouse.GetState();

            if (kbs.IsKeyDown(Keys.A))
                cam.Move(new Vector2(-10f / cam.Zoom, 0));
            else if (kbs.IsKeyDown(Keys.D))
                cam.Move(new Vector2(10f / cam.Zoom, 0));

            if (kbs.IsKeyDown(Keys.W))
                cam.Move(new Vector2(0, -10f / cam.Zoom));
            else if (kbs.IsKeyDown(Keys.S))
                cam.Move(new Vector2(0, 10f / cam.Zoom));            
        }
    }
}
