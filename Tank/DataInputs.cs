
/* - Description -
 * Svi polazni podatci za proracun treba da se nalaze u ovom fajlu.
 */

using System.Collections.Generic;

namespace Tank
{
    /* - Description -
     * Ova klasa sadrzi samo ulazne podatke za proracun. Podeljeni su po celinama.
     * Svi potrebni ulazni podatci za proracun treba da se nalaze ovde.  
     */
    /// <summary>
    /// All input data for calculation are located here.
    /// </summary>
    class DataInputs
    {
        public DataGlobal DataGlobal = new DataGlobal();    // All input data used for whole tank calculation
        public DataShells DataShells = new DataShells();    // Input data for each shell course
        public DataMaterials DataMat = new DataMaterials(); // Selected material data
    }

    /* - Description -
     * Ovde se nalaze svi podatci koji se koriste na vise mesta ili vaze za sve elemente tanka.
     */
    /// <summary>
    /// All input data used in whole tank calculation.
    /// </summary>
    class DataGlobal
    {
        public double Diameter;                 // Nominal tank diameter
        public double CylinderHeight;           // Total height of tank cylinder
        public double LiquidHeight;             // Design liquid height
        public int RoofType;                    // Roof type
        public double RoofSlope;                // Roof slope - angle between weld line and roof
        public int DesignStaticHead;            // Odabir tipa nivoa tecnosti
        public double LiquidOverflowHeight;     // Vrednost koja se odnosi na visinu tecnosti koja je od strane user-a
        public double BottomSlope;              // Bottom slope - optional
        public int MaterialOfConstruction;      // Izabrati osnovnu grupu materijala
        public double DesignPressureInternal;   // Design pressure - internal P
        public double DesignPressureExternal;   // Design pressure - external Pv
        public double DensityDesign;            // Density of design medium
        public double DensityTest;              // Density of test medium
        public double DesignTemperature;        // Design temperature
        public double CorrAllCourse1;           // Corrosion allowance for 1st (bottom) course
        public double CorrAllCourses;           // Corrosion allowance for all the a=other courses
        public int ToleranceClass;              // Tolerance class for plates
        public double ShellInsThk;              // Shell insulation thickness
        public double WindVelocity;             // Wind speed
        public double DensityOfAir;             // Air density
        public double ShellSurfaceCoeff;        // Wind shape coefficient
        public double DensityOfSteel;           // Steel density
    }

    /* - Desciption -
     * Lista svih selova. Koristi se da drzi podatke za svaki shell course. 
     */
    /// <summary>
    /// Contains list of all shell course input data.
    /// </summary>
    class DataShells
    {
        public readonly List<ShellCourse> Shells = new List<ShellCourse>();
    }

    /* - Description -
     * Sadrzi sve podatke potrebne da se opise jedan shell cousea.
     */
    /// <summary>
    /// Input data for one shell course.
    /// </summary>
    class ShellCourse
    {
        public string Tag;                  // Course tag
        public double PlateHeight;          // Plate height
        // public double CA;                // Plate corrosion. Make diferent for first course and other courses
        // public double PlateTolerance;    // Tolerance of plate
        // public double MinPlateThk;       // Should be derived from Table 16, just an input for now
        public double ProvidedCourseThk;    // Entered by user for every course
        public DataMaterial Material;      // Shell course material
    }

    /* - Description -
     * Lista odabranih materijala koji se koriste u proracunu.
     * Sadrzi ID materijala iz baze ili podateke za custom materijal
     */
    /// <summary>
    /// Dictionary of materials selected for use in calculation. <br/>
    /// Materials are added by ID from Tables.xlsx or without ID for custom materials.
    /// </summary>
    class DataMaterials
    {
        public readonly Dictionary<int, DataMaterial> Materials = new Dictionary<int, DataMaterial>();

        /* - Description -
         * Dodaje materijal u listu odabranih materijala.
         * Za materijal iz baze je potrebno samo dati BaseID.
         * Ako je FromBase=true onda se smatra da je  material iz baze inace je custom material.
         */
        /// <summary>
        /// Add material to material list. <br/>
        /// BaseID should be zero for custom material. <br/>
        /// BaseID should be id from Tables.xlsx for materials from base. <br/>
        /// Can not add same material twice.
        /// </summary>
        /// <returns>Return true if material is added successfully</returns>
        public bool AddMaterial(DataMaterial m)
        {
            if (m != null)
            {
                // If input material exist...
                if (m.FromBase == false) // ... and it request for custom material ...
                {
                    // Override ID for custom material with first empty index 
                    for (int i = Const.CustomMatMinID; i < Const.CustomMatMaxID; i++)
                    {
                        if (Materials.ContainsKey(i) == false)
                        {
                            // First empty slot is found and exit function
                            m.BaseId = i;
                            Materials.Add(m.BaseId, m);
                            return true;
                        }
                    }
                }
                else if (Materials.ContainsKey(m.BaseId) == false) // If material do not exist with same BaseId..
                {
                    // Input material is ready to be added to material dictionary
                    Materials.Add(m.BaseId, m);
                    return true;
                }
            }
            return false; // If we come to this point material is not added to list and retun false.
        }

        /* - Description -
         * Get material from dictionary by ID
         */
        /// <summary>
        /// Find material in dictionary by ID.
        /// </summary>
        /// <returns>Returns material, or null if material is not found.</returns>
        public DataMaterial GetById(int ID)
        {
            DataMaterial dummy;
            if (Materials.TryGetValue(ID, out dummy))
            {
                return dummy;
            }
            else return null;
        }

        public DataMaterial GetByName(string name)
        {
            foreach (KeyValuePair<int, DataMaterial> item in Materials)
            {
                if (item.Value.Name == name)
                {
                    return item.Value;
                }
            }
            return null;
        }

        /* - Description -
         * Vraca listu selektovanih materijala 
         */
        /// <summary>
        /// Return string array of material names.
        /// </summary>
        /// <returns>Array of materials.</returns>
        public List<string> GetMaterialList()
        {
            List<string> lst = new List<string>();
            foreach (KeyValuePair<int, DataMaterial> item in Materials)
            {
                lst.Add( item.Value.Name);
            }
            return lst;
        }
    }

    /* - Description -
     * Jedan materijal. Moze biti iz baze ili custom material.
     */
    /// <summary>
    /// One selected material, for use in calculation.
    /// </summary>
    class DataMaterial
    {
        // Iz base
        public bool FromBase;               // True if material is from base
        public int BaseId;                  // ID of materilas from base is 10000 and over. ID for custom material is 1 to 100.
        // Custom materijal
        private string _name;               // Field. Material name for custom material
        public double ForTemperature;       // Temperature for which are allowable stress
        public double ForThickness;         // Thickness for which are allowable stress
        private double _allowAmbient;       // Field. Allowable stress at ambient temperature
        private double _allowTemperature;   // Allowable stress at elevated temperature
        private double _allowTest;          // Allowable stress for test

        /// <summary>
        /// Material name.
        /// </summary>
        /// <value>
        /// The Name property gets/sets the value of the field: <see cref="DataMaterial._name"/>. 
        /// </value>
        public string Name
        {
            /* - Description -
             * Get material name. 
             * If material is from base then get name from material base.
             * Otherwise get name from field.
             */
            get
            {
                // If material is from base...
                if (FromBase == true)
                {
                    // ... try to get material by BaseID...
                    Material m;
                    m = Program.Calc.Res.Mat.GetMat(BaseId);
                    if (m != null)
                    {
                        // Get name from material if it is found by BaseID...
                        return m.NameNumeric;
                    }
                    else
                    {
                        // ... or throw error. This should not happend.
                        throw new Helpers.MyException("TDataMaterial.Name.get\nInvalid material ID");
                    }
                }
                else
                {
                    // If material is not from base return internally storad name for custom material.
                    return _name;
                }
            }

            /* - Description -
             * Set material name. 
             * If material is from base set will throw exception.
             * Otherwise set internal name in field.
             */
            set
            {
                if (FromBase == true)
                {
                    // This should not happend.
                    throw new Helpers.MyException("TDataMaterial.Name.set\nCan not set name to material in base.");
                }
                else
                {
                    // Set internal name only for custom material
                    _name = value;
                }
            }
        }

        /// <summary>
        /// Allowable stress at ambient temperature. 
        /// </summary>
        /// <value>
        /// The AllowableAmbient property gets/sets the value of the field: <see cref="DataMaterial._allowAmbient"/>. 
        /// </value>
        public double AllowableAmbient
        {
            /* - Description -
             * Get allowable stress at ambient temperature.
             * If material is from base, then get allowable stress from material base.
             * Otherwise get allowable stress from field.
             */
            get
            {
                // If material is from base...
                if (FromBase == true)
                {
                    // ... try to get material by BaseID...
                    Material m;
                    m = Program.Calc.Res.Mat.GetMat(BaseId);
                    if (m != null)
                    {
                        // Get allowable from material if it is found by BaseID...
                        return m.GetAllowable(Const.AmbT, ForThickness, false);
                    }
                    else
                    {
                        // ... or throw error. This should not happend.
                        throw new Helpers.MyException("TDataMaterial.AllowableAmbient.get\nInvalid material ID");
                    }
                }
                else
                {
                    // If material is not from base return internally storad material allowable stress for custom material.
                    return _allowAmbient;
                }
            }

            /* - Description -
             * Set material ambient allowable stress. 
             * If material is from base set will throw exception.
             * Otherwise set internal ambient allowable stress in field.
             */
            set
            {
                if (FromBase == true)
                {
                    // This should not happend.
                    throw new Helpers.MyException("TDataMaterial.AllowableAmbient.set\nCan not set allowable stress to material in base.");
                }
                else
                {
                    // If material is not from base return internally storad allowable stress for custom material.
                    _allowAmbient = value;
                }
            }
        }

        /// <summary>
        /// Allowable stress at elevated temperature.
        /// </summary>
        /// <value>
        /// The AllowableTemperature property gets/sets the value of the field: <see cref="DataMaterial._allowTemperature"/>. 
        /// </value>
        public double AllowableTemperature
        {
            /* - Description -
             * Get allowable stress at elevated temperature.
             * If material is from base, then get allowable stress from material base.
             * Otherwise get allowable stress from field.
             */
            get
            {
                // If material is from base...
                if (FromBase == true)
                {
                    // ... try to get material by BaseID...
                    Material m;
                    m = Program.Calc.Res.Mat.GetMat(BaseId);
                    if (m != null)
                    {
                        // Get allowable from material if it is found by BaseID...
                        return m.GetAllowable(ForTemperature, ForThickness, false);
                    }
                    else
                    {
                        // ... or throw error. This should not happend.
                        throw new Helpers.MyException("TDataMaterial.AllowableTemperature.get\nInvalid material ID");
                    }
                }
                else
                {
                    // If material is not from base return internally storad material allowable stress for custom material.
                    return _allowTemperature;
                }
            }

            /* - Description -
             * Set material allowable stress at elevated temperature. 
             * If material is from base set will throw exception.
             * Otherwise set internal allowable stress in field.
             */
            set
            {
                if (FromBase == true)
                {
                    // This should not happend.
                    throw new Helpers.MyException("TDataMaterial.AllowableTemperature.set\nCan not set allowable stress to material in base.");
                }
                else
                {
                    // If material is not from base return internally storad allowable stress for custom material.
                    _allowTemperature = value;
                }
            }
        }

        /// <summary>
        /// Allowable stress for test condition.
        /// </summary>
        /// <value>
        /// The AllowableTest property gets/sets the value of the field: <see cref="DataMaterial._allowTest"/>. 
        /// </value>
        public double AllowableTest
        {
            /* - Description -
             * Get allowable stress for test condition.
             * If material is from base, then get allowable stress from material base.
             * Otherwise get allowable stress from field.
             */
            get
            {
                // If material is from base...
                if (FromBase == true)
                {
                    // ... try to get material by BaseID...
                    Material m;
                    m = Program.Calc.Res.Mat.GetMat(BaseId);
                    if (m != null)
                    {
                        // Get allowable from material if it is found by BaseID...
                        return m.GetAllowable(Const.AmbT, ForThickness, true);
                    }
                    else
                    {
                        // ... or throw error. This should not happend.
                        throw new Helpers.MyException("TDataMaterial.AllowableTest.get\nInvalid material ID");
                    }
                }
                else
                {
                    // If material is not from base return internally storad material allowable stress for custom material.
                    return _allowTest;
                }
            }

            /* - Description -
             * Set material allowable stress for test. 
             * If material is from base set will throw exception.
             * Otherwise set internal allowable stress in field.
             */
            set
            {
                if (FromBase == true)
                {
                    // This should not happend.
                    throw new Helpers.MyException("TDataMaterial.AllowableTest.set\nCan not set allowable stress to material in base.");
                }
                else
                {
                    // If material is not from base return internally storad allowable stress for custom material.
                    _allowTest = value;
                }
            }
        }
    }
}