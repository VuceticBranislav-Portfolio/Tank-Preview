
/* - Description -
 * Ovde treba da se nalaze svi rezultati spremni za prezentaciju u reportu.
 * Treba da nastanu iz metode DoCalculation() koja radi proracun i priprema rezultate za report. 
 */

using System.Collections.Generic;

namespace Tank
{
    /* - Description -
     * Ova klasa treba da sadrzi samo rezultate proracuna koji su generisani funkcijom DoCalculation(). 
     * Broj elemenata u ResultsShells mora da odgovara broju unetih shelova u inputu.
     */
    /// <summary>
    /// Contains all data needed to generate report. <br/>
    /// Data is provided by method DoCalculation().
    /// </summary>
    class DataResults
    {
        public ResultsShells ResultsShells = new ResultsShells();
        public double TankCylinderVolume;   // Volume of cylindrical part of tank
        public double TankRoofVolume;       // Volume of tank roof
        public double TankOverallVolume;    // Total volume of tank
        public double MinNomSpecShellThk;   // Value according to Table 16
        public double PlateTolerance;       // Plate fabrication tolerance

        // Weights and area related result data. Posle videti da li napraviti zasebnu klasu
        public double RoofHeightDome;
        public double RoofHeightCone;
        public double TankOverallHeight;
        public double DesignHeightOfLiquid;
        public double TestPressure;

        // CONTROLE VARIABLE
        public double x;
    }

    /* - Desciption -
     * Lista svih selova. Koristi se da drzi rezultate za svaki shell course. 
     */
    /// <summary>
    /// List of all shell results.
    /// </summary>
    class ResultsShells
    {
        public readonly List<ResultsShell> Shells = new List<ResultsShell>();
    }

    /* - Description -
     * Sadrzi sve rezultate koje jedan shell ima.
     */
    /// <summary>
    /// One shell result data.
    /// </summary>
    class ResultsShell
    {
        public double LiquidLevelDesign;  // Height of liquid for given shell course - design
        public double LiquidLevelTest;    // Height of liquid for given shell course - test
        public double CourseDesignThk;    // ed
        public double CourseTestThk;      // et
        public double CourseThkRequired;  // ereq.
        public double ShellProjArea;      // Ashell
        public double CourseDeadLoad;     // Fshell
    }
}