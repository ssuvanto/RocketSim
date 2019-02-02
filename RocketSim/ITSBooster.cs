using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    class ITSBooster : Vehicle {

        double totalTime;

        public ITSBooster(string name, Vehicle parent, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) : base(name, parent, position, velocity, planet, dotcolor) {
            emptyMass = 275000;
            fuelCapacity = 6700000;
            _fuelMass = fuelCapacity * 1;

            engines = new Dictionary<Engine, int>();
            engines.Add(Engine.RaptorSL, 42);
            throttle = 1;

            totalTime = 0;
        }

        public override void Simulate(double dt) {
            //SIMPLISTIC TRAJECTORY FOR TESTING:
            if (totalTime < 10)
                Pitch = 0;
            else if (totalTime > 120)
                Pitch = Math.PI / 3;
            else
                Pitch = (totalTime - 10) * (Math.PI / 330);

            base.Simulate(dt);
            totalTime += dt;

            if (FuelMass <= 0 && child != null) {
                child.parent = null;
                child.throttle = 1;
                child = null;
            }
        }


    }
}
