using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    public class Vehicle : SimObject {

        public Vehicle parent;
        public Vehicle child;
        public Vector2D offset;

        protected double Mass { get { return emptyMass + FuelMass; } set { } }
        protected double TotalMass {
            get {
                if (child == null)
                    return Mass;
                else
                    return Mass + child.TotalMass; }
            set { } }
        protected double emptyMass;
        protected double fuelCapacity;
        protected double _fuelMass;
        protected double FuelMass {
            get { return _fuelMass; }
            set {
                if (value < 0)
                    _fuelMass = 0;
                else
                    _fuelMass = value;
            }
        }

        protected Vector2D cogOffsetEmptyMass;
        protected Vector2D cogOffsetPropFull;
        protected Vector2D cogOffsetPropEmpty;

        protected Vector2D CoGProp {
            get {
                Vector2D diff = cogOffsetPropFull - cogOffsetPropEmpty;
                return diff * GetFuelPercentage() + cogOffsetPropEmpty;
            }
            private set { }
        }

        protected Vector2D CoGTotal {
            get {
                return (emptyMass * cogOffsetEmptyMass + FuelMass * CoGProp) / Mass;
            }
            private set { }
        }

        protected Vector2D CoGStack {
            get {
                if (child == null)
                    return CoGTotal;
                else
                    return (child.TotalMass * child.CoGStack + Mass * CoGTotal) / TotalMass;
            }
            private set { }
        }

        public double Pitch {
            get {
                Vector2D rel = pos - planet.pos;
                Vector2D upvec = new Vector2D(0, -1);
                return ang - rel.Angle(upvec);
            } protected set {
                Vector2D rel = pos - planet.pos;
                Vector2D upvec = new Vector2D(0, -1);
                ang = value + rel.Angle(upvec);
            }
        }

        protected double baseCd; //coefficient of drag for subsonic flight
        protected double dragArea; //Reference area for drag calc, square meters

        bool grounded;

        protected Dictionary<Engine, int> engines; //engine type, number of type
        public double throttle;

        public Vehicle(string name, Vehicle parent, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) : base(name, position, velocity, planet, dotcolor) {
            this.parent = parent;
            offset = Vector2D.Zero;
            grounded = false;
        }

        public override void Simulate(double dt) {
            if (parent == null) {
                if (!grounded) {
                    base.Simulate(dt);
                    ConsumeFuel(dt);
                    TestForGround();
                }
            } else {
                ang = parent.ang;
                pos = parent.pos + offset.Rotate(ang);
                vel = parent.vel;
            }
        }

        public double GetThrust(Vector2D p) {
            if (FuelMass <= 0)
                return 0;
            double fulltotal = 0;
            double pressure = planet.GetPressureAtAltitude(GetAltitude(p));
            foreach (Engine e in engines.Keys) {
                fulltotal += engines[e] * e.getThrustAtPressure(pressure);
            }
            return fulltotal * throttle;
        }

        protected virtual Vector2D GetThrustAcc(Vector2D p) {
            Vector2D angvec = new Vector2D(Math.Sin(ang), -Math.Cos(ang));
            double a = GetThrust(p) / TotalMass;
            return angvec * a;
        }

        protected override Vector2D GetAcc(Vector2D p, Vector2D v) {
            return base.GetAcc(p, v) + GetThrustAcc(p) + GetDragAcc(p, v);
        }

        public double lastDrag; //public record of drag force for display purposes
        protected double GetDragForce(Vector2D p, Vector2D v) {
            double speed2 = v.LengthSquared();
            lastDrag = 0.5 * speed2 * GetCd(GetMach(p, v)) * dragArea * planet.GetDensityAtAltitude(GetAltitude(p));
            return lastDrag;
        }
     
        protected Vector2D GetDragAcc(Vector2D p, Vector2D v) {
            double force;
            if (child != null)
                force = child.GetDragForce(p, v);
            else
                force = GetDragForce(p, v);
            Vector2D reverseNorm = v.Normalized() * -1;
            double dragAcc = force / TotalMass;
            return reverseNorm * dragAcc;
        }

        protected double GetCd(double mach) {
            //Naive estimation of Cd variation by Mach number
            if (mach < 0.9)
                return baseCd;
            else if (mach >= 0.9 && mach < 1.0)
                return baseCd + (mach - 0.9) * baseCd * 30;
            else if (mach >= 1.0 && mach < 1.1)
                return baseCd * 4;
            else if (mach >= 1.1 && mach < 1.5)
                return (baseCd * 4) + (mach - 1.1) * baseCd * -5;
            else if (mach >= 1.5 && mach < 2.5)
                return (baseCd * 2) + (mach - 1.5) * baseCd * -0.5;
            else
                return baseCd * 1.5;
        }

        protected double GetMach(Vector2D p, Vector2D v) {
            double soundSpeed = 331.3 + 0.606 * planet.GetTempAtAltitude(GetAltitude(p));
            return v.Length() / soundSpeed;
        }

        protected void ConsumeFuel(double dt) {
            double fulltotal = 0;
            foreach (Engine e in engines.Keys) {
                fulltotal += engines[e] * e.massFlow;
            }
            FuelMass -= fulltotal * throttle * dt;
        }

        protected void TestForGround() {
            if(GetAltitude(pos) < 0) {
                grounded = true;
                vel = Vector2D.Zero;
            }
        }

        public double GetFuelPercentage() {
            return FuelMass / fuelCapacity;
        }

        public override void Render(Camera2D cam, GameTime gt, SpriteBatch sb, Vector2 rotOrig) {
            if (parent != null && cam.Zoom < 0.1)
                return;
            else
                base.Render(cam, gt, sb, CoGStack.ToFloatVec());
        }

    }
}
