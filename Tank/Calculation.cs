
/* - Description -
 * Ovo fajl u kome bi trebalo da se nalaze sve kalkulacije.
 * Treba da koristi inpute iz DataInputs fajla i da popuni outpute u DataResults fajlu.
 */

using System;
using System.Text;
using System.Windows.Forms;

namespace Tank
{
    /* - Description - 
     * Glavna klasa koja sadrzi sve proracune na osnovu inputa. Rezultati se smestaju u TDataResults klasu. 
     */
    /// <summary>
    /// Main calculation class. <br/>
    /// Only one instance of class should exist.
    /// </summary>
    class Calculation
    {
        public DataInputs Input = new DataInputs();         // Svi ulazni podatci su ovde
        public DataResults Results = new DataResults();     // Svi rezultati potrebni za report su ovde
        public DataResources Res;                           // Ovde se nalaze sve tablice i resursi
        public DataOptions Options = new DataOptions();     // Ovde idu sva podesavanja
        private StringBuilder R = new StringBuilder();      // U ovu promenjivu se generise ceo report pa se onda samo ubaci u RichTextBox u proceduri DoReport().

        /* - Description -
         * Sve kalkulacije idu ovde
         */
        /// <summary>
        /// All calculation go here. This should be call before report generation.
        /// </summary>
        private void DoCalculation()
        {
            // Reset rezultata pretabavanjem novom klasom            
            Results = new DataResults();

            // Zapremina cilindra. V= Di^2 * PI /4 * Hh
            Results.TankCylinderVolume = Input.DataGlobal.Diameter * Input.DataGlobal.Diameter * Math.PI / 4d * Input.DataGlobal.CylinderHeight;

            // Zapremina "dome" roof-a
            double R1, r;

            R1 = 1.1 * Input.DataGlobal.Diameter;
            r = 0.5 * Math.Sqrt(4 * R1 * R1 - Input.DataGlobal.Diameter * Input.DataGlobal.Diameter);
            Results.RoofHeightDome = R1 - r;

            // Zapremina "cone" roof-a
            Results.RoofHeightCone = Input.DataGlobal.Diameter / 2 * Math.Tan(Input.DataGlobal.RoofSlope * Math.PI / 180);

            // Zapremina roof-a.
            if (Input.DataGlobal.RoofType == 0)
            {
                Results.TankRoofVolume = Math.PI * Results.RoofHeightDome / 6 * (3 * (Input.DataGlobal.Diameter / 2) * (Input.DataGlobal.Diameter / 2) + Results.RoofHeightDome * Results.RoofHeightDome);
            }
            else
            {
                Results.TankRoofVolume = 1d / 3d * Math.PI * (Input.DataGlobal.Diameter / 2) * (Input.DataGlobal.Diameter / 2) * Results.RoofHeightCone;
            }

            // Totalna zapremina. 
            Results.TankOverallVolume = Results.TankCylinderVolume + Results.TankRoofVolume;

            // Ukupna visina tanka
            if (Input.DataGlobal.RoofType == 1)
            {
                Results.TankOverallHeight = Input.DataGlobal.CylinderHeight + Results.RoofHeightCone;
            }
            else
            {
                Results.TankOverallHeight = Input.DataGlobal.CylinderHeight + Results.RoofHeightDome;
            }

            // Visina medijuma sa kojom se ulazi u proracun
            if (Input.DataGlobal.DesignStaticHead == 0)
            {
                Results.DesignHeightOfLiquid = Input.DataGlobal.CylinderHeight;
            }
            else if (Input.DataGlobal.DesignStaticHead == 1)
            {
                Results.DesignHeightOfLiquid = Results.TankOverallHeight;
            }
            else
            {
                Results.DesignHeightOfLiquid = Input.DataGlobal.LiquidOverflowHeight;
            }

            // Racunanje visine vode u svakom shelu
            // Mora se preci svaki shell iako nema vode u njemu da bi imali isti broj reultata kao i broj unosa za shell.
            double dummy = 0; // Oduzeta visina
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {
                // Kreiranje shell klase za rezultate
                ResultsShell shell = new ResultsShell();
                // Izracunavanje visine vode iznad dna shela. Minimalno je 0.
                shell.LiquidLevelDesign = Math.Max(0, Input.DataGlobal.LiquidHeight - dummy);
                shell.LiquidLevelTest = Math.Max(0, Results.TankOverallHeight - dummy);


                // Povecavanje sume shelova ispod trenutnog
                dummy = dummy + Input.DataShells.Shells[i].PlateHeight;
                // shell.LiquidLevel := ;
                Results.ResultsShells.Shells.Add(shell);
            }

            // Test pritisak
            Results.TestPressure = 1.1 * Input.DataGlobal.DesignPressureInternal;

            // Minimum specified nominal shell thickness
            int[,] array1 = new int[8, 2]
            {
                // Values from table 16
                { 5, 2 },    // for       D < 4
                { 5, 3 },    // for  4 <= D < 10
                { 5, 4 },    // for 10 <= D < 15
                { 6, 5 },    // for 15 <= D < 30
                { 8, 6 },    // for 30 <= D < 45
                { 8, 0 },    // for 45 <= D < 60
                { 10, 0 },   // for 60 <= D < 90
                { 12, 0 }    // for 90 <= D
            };
            // Lociranje adekvatne vrednosti iz array-a
            int x, y;
            if (Input.DataGlobal.Diameter < 4) { x = 0; }
            else if (Input.DataGlobal.Diameter >= 4 && Input.DataGlobal.Diameter < 10) { x = 1; }
            else if (Input.DataGlobal.Diameter >= 10 && Input.DataGlobal.Diameter < 15) { x = 2; }
            else if (Input.DataGlobal.Diameter >= 15 && Input.DataGlobal.Diameter < 30) { x = 3; }
            else if (Input.DataGlobal.Diameter >= 30 && Input.DataGlobal.Diameter < 45) { x = 4; }
            else if (Input.DataGlobal.Diameter >= 45 && Input.DataGlobal.Diameter < 60) { x = 5; }
            else if (Input.DataGlobal.Diameter >= 60 && Input.DataGlobal.Diameter < 90) { x = 6; } else { x = 7; }
            if (Input.DataGlobal.MaterialOfConstruction == 0) { y = 0; } else { y = 1; }
            Results.MinNomSpecShellThk = array1[x, y];

            // Odredjivanje plate tolerance vrednosti iz tabele
            double[,] array2 = new double[4, 4]
            {
                // Values from EN 10029 Table 1
                { 0.3, 0.3, 0, 0.5 },       // 3 <= t < 5
                { 0.4, 0.3, 0, 0.6 },       // 5 <= t < 8
                { 0.5, 0.3, 0, 0.7 },       // 8 <= t < 15
                { 0.6, 0.3, 0, 0.8 }        // 15 <= t < 25
            };
            int p, q;
            int t = 6;       // Provided course thk. Vrednost koja se unosi na osnovu izracunatog thk. Za sad fiktivno
            if (t >= 3 && t < 5) { p = 0; } else if (t >= 5 && t < 8) { p = 1; } else if (t >= 8 && t < 15) { p = 2; } else { p = 3; }
            if (Input.DataGlobal.ToleranceClass == 0) { q = 0; } else if (Input.DataGlobal.ToleranceClass == 1) { q = 1; } else if (Input.DataGlobal.ToleranceClass == 2) { q = 2; } else { q = 3; }
            Results.PlateTolerance = array2[p, q];

            // Racunanje design thickness-a cours-a
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {
                Results.ResultsShells.Shells[i].CourseDesignThk = Input.DataGlobal.Diameter / 20 / Input.DataShells.Shells[i].Material.AllowableTemperature * (98 * Input.DataGlobal.DensityDesign / 1000 * (Results.ResultsShells.Shells[i].LiquidLevelDesign - 0.3) + Input.DataGlobal.DesignPressureInternal) + Input.DataGlobal.CorrAllCourse1 + Results.PlateTolerance;
            }

            // Racunanje test thickness-a cours-a
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {
                Results.ResultsShells.Shells[i].CourseTestThk = Input.DataGlobal.Diameter / 20 / Input.DataShells.Shells[i].Material.AllowableTest * (98 * Input.DataGlobal.DensityTest / 1000 * (Results.ResultsShells.Shells[i].LiquidLevelDesign - 0.3) + Results.TestPressure);
            }

            // Odredjivanje ereq.
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {

                Results.ResultsShells.Shells[i].CourseThkRequired = Math.Ceiling(Math.Max(Results.ResultsShells.Shells[i].CourseDesignThk, Results.ResultsShells.Shells[i].CourseTestThk));
            }

            // Shell stability
            // Racunanje shell projected area
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {
                Results.ResultsShells.Shells[i].ShellProjArea = Results.ResultsShells.Shells[i].LiquidLevelDesign * (Input.DataGlobal.Diameter + 2 * Input.DataShells.Shells[i].ProvidedCourseThk / 1000d + 2 * Input.DataGlobal.ShellInsThk / 1000d);

            }

            // Racunanje Fshell
            for (int i = 0; i < Input.DataShells.Shells.Count; i++)
            {
                Results.ResultsShells.Shells[i].CourseDeadLoad = (Math.Pow(Input.DataGlobal.Diameter + 2 * Input.DataShells.Shells[i].ProvidedCourseThk / 1000d, 2) - Math.Pow(Input.DataGlobal.Diameter, 2)) * Math.PI / 4d * Input.DataShells.Shells[i].PlateHeight * Input.DataGlobal.DensityOfSteel * 9.806;
            }
        }

        /* - Description -
         * Generisanje reporta ide ovde
         */
        public void DoReport(RichTextBox Report)
        {
            // Prvo treba sve preracunati da moze da se prikaze report
            DoCalculation();

            // Ciscenje pa priprema texta za report
            R.Clear();

            // Konstantni elementi za report
            const string Separator = "============================================================\n";

            // 60 karaktera sirina reporta. \t - tab karakter = 8 spaces, \n - novi red = <enter>
            // {0,10:F2} - Znaci formatiraj 1. prilozeni broj na 10 mesta, poravnat na desno, kao decimalan broj sa 2 decimalna mesta
            R.Append(Separator);
            R.Append("\t\t\t    TANK REPORT\n");

            // Input echo
            R.Append(Separator);
            R.Append("  User input data:\n");
            R.AppendFormat("    Tank diameter:\t\t\t\t{0,10:F2} m\n", Input.DataGlobal.Diameter);
            R.AppendFormat("    Cylinder height:\t\t\t\t{0,10:F2} m\n", Input.DataGlobal.CylinderHeight);
            R.AppendFormat("    Maximum fill height:\t\t\t{0,10:F2} m\n", Input.DataGlobal.LiquidHeight);
            if (Input.DataGlobal.DesignStaticHead == 0) { R.Append("    Design static head:\t\t\tNominal liquid level \n"); } else if (Input.DataGlobal.DesignStaticHead == 1) { R.Append("    Design static head:\t\t\tFull of liquid \n"); } else { R.Append("    Design static head:\t\t\tLiquid overflow \n"); }
            R.AppendFormat("    Liquid overflow height:\t\t\t{0,10:F2} m\n", Input.DataGlobal.LiquidOverflowHeight);
            if (Input.DataGlobal.RoofType == 0) { R.Append("    Roof type:\t\t\t\t\tDome \n"); } else { R.Append("    Roof type:\t\t\t\t\tCone \n"); }
            R.AppendFormat("    Roof slope:\t\t\t\t{0,10:F2} deg\n", Input.DataGlobal.RoofSlope);
            if (Input.DataGlobal.MaterialOfConstruction == 0) { R.AppendFormat("    Material of construction:\t\t   Carbon steel \n"); } else { R.AppendFormat("    Material of construction:\t\t Stainless steel \n"); }
            R.AppendFormat("    Design pressure (int.):\t\t\t{0,10:F2} mbarg\n", Input.DataGlobal.DesignPressureInternal);
            R.AppendFormat("    Design pressure (ext.):\t\t\t{0,10:F2} mbarg\n", Input.DataGlobal.DesignPressureExternal);
            R.AppendFormat("    Design temperature:\t\t\t{0,10:F2} degC\n", Input.DataGlobal.DesignTemperature);

            // Results
            R.Append(Separator);
            R.Append("  Results:\n");
            R.Append("  1. Shell verification:\n");
            R.Append("  1.1 Volume:\n");
            R.AppendFormat("    Cylinder volume:\t\t\t\t{0,10:F2} m3\n", Results.TankCylinderVolume);
            R.AppendFormat("    Roof volume:\t\t\t\t{0,10:F2} m3\n", Results.TankRoofVolume);
            R.AppendFormat("    Total volume:\t\t\t\t{0,10:F2} m3\n", Results.TankOverallVolume);
            R.Append("  1.2 Design liquid level\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {
                // Shell tag ce vek biti 20 karaktera. Duzi ce biti oseceni a za kraci ce biti dopunjeni spaceovima
                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} m\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].LiquidLevelDesign);

            }
            R.Append("  1.3 Test liquid level\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {

                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} m\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].LiquidLevelTest);
            }
            R.Append("  1.4 Calculated course thickness - design\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {

                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} mm\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].CourseDesignThk);
            }
            R.Append("  1.5 Calculated course thickness - test\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {

                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} mm\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].CourseTestThk);
            }
            R.Append("  1.6 Required calculated course thickness\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {

                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} mm\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].CourseThkRequired);
            }
            // Shell stability
            R.Append("  1.7 Projected area of shell\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {
                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} m2\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].ShellProjArea);
            }

            R.Append("  1.8 Course dead load\n");
            for (int i = 0; i < Results.ResultsShells.Shells.Count; i++)
            {
                R.AppendFormat("    {0}\t\t\t\t{1,10:F2} N\n", Input.DataShells.Shells[i].Tag.ToLength(10), Results.ResultsShells.Shells[i].CourseDeadLoad);
            }

            R.Append("  CONTROLE\n");
            R.Append(Results.x);
            R.Append("  CONTROLE\n");

            R.Append("  X. Various calculations:\n");
            R.AppendFormat("    Tank overall height:\t\t\t{0,10:F2} m\n", Results.TankOverallHeight);
            R.AppendFormat("    Design height of liquid:\t\t{0,10:F2} m\n", Results.DesignHeightOfLiquid);
            R.AppendFormat("    Test pressure:\t\t\t\t{0,10:F2} barg\n", Results.TestPressure);
            R.AppendFormat("    Minimum shell course thk:\t\t{0,10:F2} mm\n", Results.MinNomSpecShellThk);
            R.AppendFormat("    Plate tolerance:\t\t\t\t{0,10:F2} mm\n", Results.PlateTolerance);

            // Ubacivanje gotovog texta u report. Pretabavanje starog texta.
            Report.Text = R.ToString();
        }
    }
}