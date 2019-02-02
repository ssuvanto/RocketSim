using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    public class F9S2Dragon : Vehicle {

        double stageTime;

        public F9S2Dragon(string name, Vehicle parent, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) : base(name, parent, position, velocity, planet, dotcolor) {

            emptyMass = 14000; //about 4000 for s2 and 10k for loaded dragon
            fuelCapacity = 107500;
            _fuelMass = fuelCapacity * 1;

            cogOffsetEmptyMass = new Vector2D(1.83, 4.87);
            cogOffsetPropFull = new Vector2D(1.83, 9.88);
            cogOffsetPropEmpty = new Vector2D(1.83,12.2);

            engines = new Dictionary<Engine, int>();
            engines.Add(Engine.MerlinVacD, 1);
            throttle = 0;

            baseCd = 0.4;
            dragArea = 10.52;

            stageTime = 0;
        }

        public override void Simulate(double dt) {
            if (parent == null) {
                if (stageTime < 60)
                    Pitch = Math.PI / 3;
                else if (stageTime > 225) {
                    Pitch = Math.PI / 2;
                    if (vel.Length() > Math.Sqrt(planet.GM / (pos - planet.pos).Length()))
                        throttle = 0;
                }
                else
                    Pitch = (Math.PI / 3) + (stageTime - 60) * (Math.PI / 990);

                stageTime += dt;
            }

            base.Simulate(dt);
        }

    }
}
