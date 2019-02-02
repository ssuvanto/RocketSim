using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    public class F9S1 : Vehicle{

        double totalTime;

        public F9S1(string name, Vehicle parent, Vector2D position, Vector2D velocity, Earth planet, Color dotcolor) : base(name, parent, position, velocity, planet, dotcolor) {

            emptyMass = 22200;
            fuelCapacity = 409500;
            _fuelMass = fuelCapacity * 1;

            cogOffsetEmptyMass = new Vector2D(1.83, 32.4);
            cogOffsetPropFull = new Vector2D(1.83, 24.7);
            cogOffsetPropEmpty = new Vector2D(1.83, 34.59);

            engines = new Dictionary<Engine, int>();
            engines.Add(Engine.Merlin1D, 9);
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

            if(FuelMass <= 0 && child != null) {
                child.parent = null;
                child.throttle = 1;
                child = null;
            }
        }

    }
}
