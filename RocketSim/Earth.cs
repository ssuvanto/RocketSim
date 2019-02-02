using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace RocketSim {
    public class Earth : Planet {

        static readonly float earthradius = 6371000;
        static readonly long earthGM = 398600441890000;

        static readonly double earthAtmosHeight = 200000;

        public Earth(Vector2D position, Color color, int rendersides) : base(position, earthradius, earthGM, color, rendersides) {
        }

        public double GetDensityAtAltitude(double h) {
            if (h > earthAtmosHeight)
                return 0;

            double p = GetPressureAtAltitude(h);
            double t = GetTempAtAltitude(h);
            return p / (0.2869 * (t + 273.1));

        }

        public double GetPressureAtAltitude(double h) {
            if (h > earthAtmosHeight)
                return 0;

            double t = GetTempAtAltitude(h);
            if (h > 25000)
                return 2.488 * Math.Pow(((t + 273.1)/216.6), -11.388);
            if (h >= 11000)
                return 22.65 * Math.Exp(1.73 - 0.000157 * h);
            if (h < 11000)
                return 101.29 * Math.Pow(((t + 273.1)/288.08), 5.256);

            return 0; //something went wrong
        }

        public double GetTempAtAltitude(double h) {
            //NOTE: Does not handle odd cases like very high altitude or below-zero altitude
            if (h > 25000)
                return -131.21 + 0.00299 * h;
            if (h >= 11000)
                return -56.46;
            if (h < 11000)
                return 15.04 - 0.00649 * h;

            return 0; //something went wrong
        }

        Color sky1 = Color.Multiply(Color.SkyBlue, 0.5f);
        Color sky2 = Color.Multiply(Color.SkyBlue, 0.1f);
        public override void Render(GameTime gt, SpriteBatch sb) {
            base.Render(gt, sb);
            sb.DrawCircle(pos.ToFloatVec(), radius + 11000, sides, Color.SkyBlue, 11000);
            sb.DrawCircle(pos.ToFloatVec(), radius + 25000, sides, sky1, 14000);
            sb.DrawCircle(pos.ToFloatVec(), radius + 200000, sides, sky2, 175000);
        }

    }
}
