
/* - Description -
 * Klasa koja sadrzi sve podatke za jedan materijal
 */

namespace Tank
{
    /* - Description -
     * Klasa koja sadrzi sve stvari vezane za kod i koriste se globalno
     */
    /// <summary>
    /// General code stuff that need to be accessed from everywhere
    /// </summary>
    static class CodeStuff
    {
        public delegate double TCalculateAllowableStress(double Re, double Rm, Material mat, bool Test);
        public static TCalculateAllowableStress AllowStress;
    }

    /// <summary>
    /// Yield stress type enumeration
    /// </summary>
    enum YieldType
    {
        /// <summary>
        /// Yield stress 0.2%
        /// </summary>
        Re02,
        /// <summary>
        /// Yield stress 1%
        /// </summary>
        Re10
    }

    /* - Description -
     * Svi podatci o jednom materijalu se nalaze ovde
     */
    /// <summary>
    /// Material class hold all data for one material.
    /// </summary>
    class Material
    {
        public int ID;                  // Index iz baze. Treba da je >= 10000
        public string Source;           // Izvorni kod
        public string NameSymbolic;     // Simbolicko ime
        public string NameNumeric;      // Numericko ime
        public double Year;             // Godina izvora
        public string Product;          // Produkt forma
        public double Density;          // Gustina
        public YieldType typ;           // Yield tablica koja se koristi
        public ThicknessList Re02;      // Yield tablica 0.2%
        public ThicknessList Re10;      // Yield tablica 1%
        public ThicknessList Rm;        // Tensile tablica
        public TemperatureData Elast;   // Modulus of elasticity table
        public TemperatureData Termo;   // Thermal coeficient table

        /* - Description -
         * Prosti Constructor
         */
        /// <summary>
        /// Simple Constructor.
        /// </summary>
        public Material()
        {
        }

        /* - Description -
         * Konstruktor koji popunjava podatke
         */
        /// <summary>
        /// Advance Constructor
        /// </summary>
        /// <param name="data">Data row</param>
        /// <param name="hd">Header data</param>
        public Material(string[] data, HeaderHolder hd)
        {
            // Data is zero based here   
            // First cell must be populated and second must be 1 for row to be valid
            if ((data[0] != "") && (data[1] != "1"))
            {
                // Throw error if first two cell are invalid
                throw new Helpers.MyException("Table read error");
            };

            // Populate all data
            ID = Helpers.ObjToInt(data[0]);
            Source = data[2];
            NameSymbolic = data[3];
            NameNumeric = data[4];
            Product = data[5];
            Year = Helpers.ObjToDouble(data[6]);
            Density = Helpers.ObjToDouble(data[7]);
            if (Helpers.ObjToInt(data[8]) == 2)
            {
                typ = YieldType.Re10;
            }
            else
            {
                typ = YieldType.Re02;
            }

            // Populate lists for modulus of elasticity and thermal coefficient
            Elast = new TemperatureData(TabType.Elas, hd, data); 
            Termo = new TemperatureData(TabType.Therm, hd, data);
            // Empty lists for yield and tensile. 
            Re02 = new ThicknessList();
            Re10 = new ThicknessList();
            Rm = new ThicknessList();
        }

        /* - Description -
         * Duboka kopija materijala
         */
        /// <summary>
        /// Material deep copy
        /// </summary>
        /// <returns>Copy of material</returns>
        public Material DeepCopy()
        {
            Material result = new Material();
            result.ID = ID;
            result.Source = string.Copy(Source);
            result.NameSymbolic = string.Copy(NameSymbolic);
            result.NameNumeric = string.Copy(NameNumeric);
            result.Year = Year;
            result.Product = string.Copy(Product);
            result.Density = Density;
            result.typ = typ;
            result.Re02 = Re02.DeepCopy();
            result.Re10 = Re10.DeepCopy();
            result.Elast = Elast.DeepCopy();
            result.Termo = Termo.DeepCopy();
            return result;
        }

        /* - Description -
         * Vraca dozvoljeni napon za materijal na temperaturi u zavisnosti od tiknesa.
         * Od Test parametra zavisi da li je allowable za test ili dizajn
         */
        /// <summary>
        /// Return allowable stress for given temperature and thickness.<br/>
        /// If Test is true allowable stress is for test condition otherwise it is for design condition.
        /// </summary>
        /// <param name="Temp">Temperature</param>
        /// <param name="Thk">Thickness</param>
        /// <param name="Test">Allowable stress is for test condition?</param>
        /// <returns>Allowable stress</returns>
        public double GetAllowable(double Temp, double Thk, bool Test)
        {
            // Initialization
            double Re; // Get yield for allowable formula
            double Rm; // Get tensile for allowable formula
            if (typ == YieldType.Re10)
            {   // Get austenitic proof stress Re 1.0%
                Re = this.Re10.GetThk(Thk).TD.GetValue(Temp);
            }
            else
            {   // In all other cases get proof stress Re 0.2%
                Re = this.Re02.GetThk(Thk).TD.GetValue(Temp); 
            }
            Rm = this.Rm.GetThk(Thk).TD.GetValue(Temp);   

            if (CodeStuff.AllowStress == null) throw new Helpers.MyException("TCalculateAllowableStress not implemented !");
            return CodeStuff.AllowStress(Re, Rm, this, Test);
        }
    }
}
