using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;

namespace RocketSim {
    public class SimObject {

        string name;
        public Vector2D pos { get; protected set; }
        public Vector2D vel { get; protected set; }
        protected Vector2D acc;

        protected double ang;
        protected double angvel; // rad/s

        protected Earth planet; //TODO: if going to do n-body, needs all planets or reference to acceleration calculator

        Color dotcol;
        Texture2D sprite;
        int spritex, spritey;

        public SimObject(string name, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) {
            this.name = name;
            pos = position;
            vel = velocity;
            dotcol = dotcolor;
            this.planet = planet;
            ang = 0;

            acc = Vector2D.Zero;
        }
 
        private Vector2D oldacc;
        private bool first = true;
        public virtual void Simulate(double dt) {

            Vector2D p1, p2, p3, p4, v1, v2, v3, v4, a1, a2, a3, a4;
            p1 = pos;
            v1 = vel;
            a1 = GetAcc(p1, v1);
            p2 = p1 + v1 * dt / 2;
            v2 = v1 + a1 * dt / 2;
            a2 = GetAcc(p2, v2);
            p3 = p1 + v2 * dt / 2;
            v3 = v1 + a2 * dt / 2;
            a3 = GetAcc(p3, v3);
            p4 = p1 + v3 * dt;
            v4 = v1 + a3 * dt;
            a4 = GetAcc(p4, v4);
            pos += (v1 + 2 * v2 + 2 * v3 + v4) * dt / 6;
            vel += (a1 + 2 * a2 + 2 * a3 + a4) * dt / 6;

            /*
            if (first) {
                acc = GetAcc(pos, vel);
                first = false;
            }

            pos += dt * (vel + 0.5 * acc * dt);
            oldacc = new Vector2D(acc);
            acc = GetAcc(pos, vel);
            vel += 0.5 * (oldacc + acc) * dt;
            */
        }

        protected virtual Vector2D GetAcc(Vector2D p, Vector2D v) {
            return planet.GravAccAt(p);
        }

        public double GetAltitude(Vector2D p) {
            Vector2D diff = planet.pos - p;
            return diff.Length() - planet.radius;
        }

        public void SetSprite(Texture2D sprite, int xsize, int ysize) {
            this.sprite = sprite;
            spritex = xsize;
            spritey = ysize;
        }

        public virtual void Render(Camera2D cam, GameTime gt, SpriteBatch sb, Vector2 rotOrig) {
            if (cam.Zoom >= 0.1)
                if (sprite == null)
                    sb.DrawRectangle(pos.ToFloatVec(), new Vector2(5, 70), Color.White, 3);
                else
                    sb.Draw(texture: sprite, destinationRectangle: new Rectangle((int)pos.X, (int)pos.Y, spritex, spritey), origin: rotOrig, rotation: (float)ang);
            else
                sb.DrawPoint(pos.ToFloatVec(), dotcol, 2.0f / cam.Zoom);
        }

    }
}
