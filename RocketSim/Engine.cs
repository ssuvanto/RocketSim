using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketSim {
    public class Engine {

        public static Engine Merlin1D = new Engine("Merlin1D", 305.449, 282, 311);
        public static Engine MerlinVacD = new Engine("MerlinVacD", 275.968, 300, 345); //vac engine sl isp mostly irrelevant but guesstimated anyway

        public static Engine RaptorSL = new Engine("RaptorSL", 931.178, 334, 361);
        public static Engine RaptorVac = new Engine("RaptorVac", 934.295, 350, 382); //sl isp once again irrelevant

        string name;

        public double massFlow;
        double slIsp;
        double vacIsp;

        double slThrust;
        double vacThrust;
        double dtbyslp; //delta thrust divided by sea level pressure

        // F = g0 * Isp * massflow
        public Engine(string name, double massFlow, double sealevelIsp, double vacuumIsp) {
            this.name = name;
            this.massFlow = massFlow;
            slIsp = sealevelIsp;
            vacIsp = vacuumIsp;
            slThrust = 9.80665 * slIsp * this.massFlow;
            vacThrust = 9.80665 * vacIsp * this.massFlow;
            dtbyslp = (slThrust - vacThrust)/101.401;
        }

        public double getThrustAtPressure(double pressure) {
            return vacThrust + pressure * dtbyslp;
        }

    }
}
