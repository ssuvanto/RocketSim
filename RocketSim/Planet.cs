using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace RocketSim {
    public class Planet {

        public Vector2D pos;
        public float radius;
        public long GM { get; protected set; }
        protected Color col;
        protected int sides;

        public Planet(Vector2D position, float radius, long stdgravparam, Color color, int rendersides) {
            pos = position;
            this.radius = radius;
            GM = stdgravparam;
            col = color;
            sides = rendersides;
        }

        public Vector2D GravAccAt(Vector2D position) {
            Vector2D diff = pos - position;
            double a = GM / diff.LengthSquared();
            return diff.Normalized() * a;
        }

        public virtual void Render(GameTime gt, SpriteBatch sb) {
            sb.DrawCircle(pos.ToFloatVec(), radius, sides, col, radius);
        }

    }
}
