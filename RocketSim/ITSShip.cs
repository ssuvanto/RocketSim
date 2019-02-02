using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    class ITSShip : Vehicle {

        double stageTime;

        public ITSShip(string name, Vehicle parent, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) : base(name, parent, position, velocity, planet, dotcolor) {
            emptyMass = 450000; //150t empty + 300t cargo
            fuelCapacity = 1950000;
            _fuelMass = fuelCapacity * 1;

            engines = new Dictionary<Engine, int>();
            engines.Add(Engine.RaptorVac, 6);
            throttle = 0;

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
                } else
                    Pitch = (Math.PI / 3) + (stageTime - 60) * (Math.PI / 990);

                stageTime += dt;
            }

            base.Simulate(dt);
        }

    }
}
