
/* - Description -
 * Klasa koja sadrzi sve tablice. Kroz nju se pristupa svim podatcima iz tablica.
 * Ujedno je zaduzena za organizaciju potrebnih tablica.
 */

namespace Tank
{
    /// <summary>
    /// Main resource class.<br/>
    /// Provide access to all tables from base.
    /// </summary>
    class DataResources
    {
        // How to add new table:
        // 1. Create new class
        // 2. Make property of class type as public
        // 3. Register class in LoadAll method
        
        // ToDo: Remove path from here and use Options direct!
        private string path;                                    // Internal path of data file
        public Profile_UnL UnL = new Profile_UnL("Table050");   // Table of standard unequal profiles
        public Materials Mat = new Materials("Table100");       // Table of materials

        /* - Description -
         * Sve tablice treba da su ovde registrovane.
         * Metoda Prepare() ce sve registrovane tablice loadirati.
         */
        /// <summary>
        /// Register and load all tables.
        /// </summary>
        /// <returns></returns>
        public bool LoadAll()
        {
            Provider provider = new Provider();
            provider.AddNewRequest(UnL);
            provider.AddNewRequest(Mat);

            return provider.Prepare(path);
        }

        /* - Description -
         * Konstruktor. Pri kreiranju attachuje delegate na staticku klasu.
         * Setuje i lokaciju data fajla.
         */
        /// <summary>
        /// Constructor with initial setup.
        /// </summary>
        /// <param name="filepath"></param>
        public DataResources(string filepath)
        {
            this.path = filepath;
            CodeStuff.AllowStress = AllowStressDelegat;
        }

        // Delegat for calculating allowable stress
        // ToDo: Try to avoid static delegat !
        /// <summary>
        /// Return allowable stress calculated per code formula for given material<br/>
        /// Allowable stress can be calculated for test condition seting Test to true.
        /// </summary>
        /// <param name="Re">Material yield stress</param>
        /// <param name="Rm">Material tensile stress</param>
        /// <param name="mat">Material</param>
        /// <param name="Test">Calculate for test condition</param>
        /// <returns>Allowable stress per code formula.</returns>
        private double AllowStressDelegat(double Re, double Rm, Material mat, bool Test)
        {
            // Calculation of allowable stress accoring to code rules
            if (Test == true)
            {
                // Allowble stress in test condition
                return Re * 3 / 4;
            }
            else
            {
                // Allowable stress in design conditions
                return Re * 2 / 3;
            }
        }
    }
}
