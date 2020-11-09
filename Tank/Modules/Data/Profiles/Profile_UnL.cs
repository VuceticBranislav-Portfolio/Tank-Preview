
/* - Description -
 * Tablica sa Nejednakim L profilima
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace Tank
{
    /* - Description -
     * Enumeracija kolona za tablicu
     */
    /// <summary>
    /// Table column enumeration
    /// </summary>
    enum Columns_UnL
    {
        /// <summary>
        /// Nominal dimensions<br/> a x b x t<br/>Units: [mm] 
        /// </summary>
        Name,
        /// <summary>
        /// Radius r1<br/>Units: [mm] 
        /// </summary>
        R1,
        /// <summary>
        /// Radius r2<br/>Units: [mm] 
        /// </summary>
        R2,
        /// <summary>
        /// Cross-section area<br/>Units: [cm2] 
        /// </summary>
        A,
        /// <summary>
        /// Nominal weight per 1m<br/>Units: [kg] 
        /// </summary>
        M,
        /// <summary>
        /// Distance to Center of Gravity on X axes<br/>Units: [mm] 
        /// </summary>
        Cx,
        /// <summary>
        /// Distance to Center of Gravity on Y axes<br/>Units: [mm] 
        /// </summary>
        Cy,
        /// <summary>
        /// Moment Of Inertia for X axes<br/>Units: [cm4] 
        /// </summary>
        XXm,
        /// <summary>
        /// Moment Of Inertia for Y axes<br/>Units: [cm4] 
        /// </summary>
        YYm,
        /// <summary>
        /// Moment Of Inertia for U axes<br/>Units: [cm4] 
        /// </summary>
        UUm,
        /// <summary>
        /// Moment Of Inertia for V axes<br/>Units: [cm4] 
        /// </summary>
        VVm,
        /// <summary>
        /// Radius Of Gyration for X axes<br/>Units: [cm] 
        /// </summary>
        XXr,
        /// <summary>
        /// Radius Of Gyration for Y axes<br/>Units: [cm] 
        /// </summary>
        YYr,
        /// <summary>
        /// Radius Of Gyration for U axes<br/>Units: [cm] 
        /// </summary>
        UUr,
        /// <summary>
        /// Radius Of Gyration for V axes<br/>Units: [cm] 
        /// </summary>
        VVr,
        /// <summary>
        /// Angle x-x to u-u<br/>Units: [rad] 
        /// </summary>
        Angle
    }
    
    class Profile_UnL : DatabaseRequest
    {
        private List<string> naziv;         // Prvi red sadrzi nazive profila
        private List<List<double>> values;  // Ostalo je tablica sa podatcima

        /* - Description -
         * Override CheckMe() metode iz provajdera
         */
        public override bool CheckMe(Excel.Worksheet ws)
        {
            // check is there 16 columns. that is constant and cann not be changed
            // At least one row must exist
            if ((C != 16) || (R <= 0))
            {
                status = Status.sError;
                return false;
            }
            status = Status.sUnPrepared;
            return true;
        }

        /* - Description -
         * Override DoMe() metode iz provajdera
         */
        public override bool DoMe(Excel.Worksheet ws)
        {
            naziv = new List<string>();
            values = new List<List<double>>();
            string[,] dummy = null;
            string[] dummy2 = null;

            // 1. Get data

            // Read table data
            // Prva kolona
            dummy = GetRange(X, Y, 0, R - 1, ws);
            if (dummy != null)
            {
                naziv.AddRange(dummy.Cast<string>());
            }

            // Ostali redovi
            for (int i = 0; i < R; i++)
            {
                dummy2 = GetRow(X + 1, Y+i, C - 2, ws);
                if (dummy2 != null)
                {
                    List<double> L = new List<double>();
                    for (int j = 0; j < dummy2.Length; j++)
                    {
                        double D;
                        if (double.TryParse(dummy2[j], out D))
                        {
                            L.Add(D);
                        }
                        else
                        {
                            Trace.WriteLine("FAIL coversion to double in DoMe");
                            L.Add(0);
                        }
                    }
                    values.Add(L);
                }
            }

            //2. Check data
            status = Status.sReady;
            return true;
        }

        /* - Description -
         * Prosti konstruktor sa imenom tablice u bazi
         */
        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="tablename">Name of table in data file</param>
        public Profile_UnL(string tablename)
        {
            table = tablename;
        }

        /* - Description -
         * Vraca vrednost iz tablice za zadati red i kolonu
         */
        /// <summary>
        /// Return value for given row and column
        /// </summary>
        /// <param name="Index">Table row</param>
        /// <param name="Column">Table column</param>
        /// <returns>Value from table cell</returns>
        public double GetValue(int Index, Columns_UnL Column)
        {
            if ((status == Status.sReady) && (naziv != null))
            {
                if ((Index >= R) || (Index < 0) || (Column == Columns_UnL.Name))
                {
                    throw new Helpers.MyException("Invalid table parameter");
                }
                return values[Index][(int)Column - 1];
            }
            else
            {
                throw new Helpers.MyException("Not redy to use");
            }
        }

        /* - Description -
         * Vraca listu svih dostupnih profila
         */
        /// <summary>
        /// Get list of all profiles
        /// </summary>
        /// <returns>List of all avalible profiles</returns>
        public List<string> GetNames()
        {
            if ((status == Status.sReady) && (naziv != null))
            {
                return naziv;
            }
            else
            {
                throw new Helpers.MyException("Not redy to use");
            }
        }

        /* - Description -
         * Vraca index profila u tablici na osnovu imena
         */
        /// <summary>
        /// Get index of given profile name
        /// </summary>
        /// <param name="Name">Name of index that should be found</param>
        /// <returns>Index of name in table</returns>
        public int GetId(string Name)
        {
            if ((status == Status.sReady) && (naziv != null))
            {
                return naziv.IndexOf(Name);
            }
            else
            {
                throw new Helpers.MyException("Not redy to use");
            }
        }
    }
}
