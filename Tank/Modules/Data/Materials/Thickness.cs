
/* - Description -
 * Sadrzi klase za jedan tiknes i listu tiknesa
 * Koristi se kao deo tablice svakog matreijala
 */

using System.Collections.Generic;

namespace Tank
{
    /* - Description -
     * Lista koja sadrzi vise tablica podeljenih po tiknesima.
     * Lista treba da je sortirana od najmanjeg tiknesa ka najvecem.
     */
    /// <summary>
    /// List of all thickness tables.
    /// </summary>
    class ThicknessList
    {
        public List<Thickness> Thk;  // List of all thicknesses

        /* - Description -
         * Construktor
         */
        /// <summary>
        /// Constructor
        /// </summary>
        public ThicknessList()
        {
            Thk = new List<Thickness>();
        }

        /* - Description -
         * Vraca kopiju liste sa svim tiknesima
         */
        /// <summary>
        /// Make deep copy of thickness list
        /// </summary>
        /// <returns>Copy of list</returns>
        public ThicknessList DeepCopy()
        {
            ThicknessList result = new ThicknessList();
            result.Thk = new List<Thickness>();
            for (int i = 0; i < Thk.Count; i++)
            {
                result.Thk.Add(Thk[i].DeepCopy());
            }
            return result;
        }

        /* - Description -
         * Vraca tablicu sa tiknesima.
         * Tablica sa tiknesom 0 je default vrednost kada nema vise tiknesa.
         * Ukoliko ima vise tiknesa koji su manji od zadataog vratice null.
         */
        // ToDo: Proveriti da li se koristi >= ili samo >. Staraditi kad nema nadjenog tiknesa?
        /// <summary>
        /// Return thickness table
        /// If table has thickness 0 first table is returned.
        /// If thickness is > then in existing tables it will return null 
        /// </summary>
        /// <param name="Thick">Requested thickness</param>
        /// <returns>Thickness table</returns>
        public Thickness GetThk(double Thick)
        {
            for (int i = 0; i < Thk.Count; i++)
            {
                if (Thk[i].MaxThk == 0)
                {
                    // 0 is default value and it is taken immidiately
                    return Thk[i];
                }
                else if (Thk[i].MaxThk > Thick)
                {   // Assum that table is sorted ascending
                    return Thk[i];
                }
            }
            return null;
        }
    }

    class Thickness
    {
        public double MaxThk;           // Inclusive add as properti if needed
        public TemperatureData TD;  // Stress-Temp tablica

        /* - Description -
         * Obican konstruktor koji nepopunjava klasu.
         */
        /// <summary>
        /// Default empty constructor
        /// </summary>
        public Thickness()
        {
        }

        /* - Description -
         * Konstruktor koji popunjava klasu na osovu podataka jednog reda.
         * Ovim se dodaju samo tablice Re02, Re10, Rm stoga data mora da ima vrednost 2.
         */
        /// <summary>
        /// Advance constructor.
        /// </summary>
        /// <param name="typ">Table type to read</param>
        /// <param name="data">Row data</param>
        /// <param name="hdr">Row header data</param>
        public Thickness(TabType typ, string[] data, HeaderHolder hdr)
        {
            // Data is zero based here   
            //  Initialization
            MaxThk = 0;
            TD = null;
            // Exit if "Data" column not 2
            if (data[1] != "2")
            {
                throw new Helpers.MyException("Thickness Table read error. Unexpected additional row. Not type 2.");
            }

            // Load table based on typ.
            switch (typ)
            {
                case TabType.Re02:
                case TabType.Re10:
                case TabType.Rm:
                    MaxThk = Helpers.ObjToDouble(data[hdr.Thick - 1]);
                    TD = new TemperatureData(typ, hdr, data);
                    break;
                default:
                    throw new Helpers.MyException("Thickness Table read error");
            }
        }

        /* - Description -
         * Vraca kopiju tiknes tablice
         */
        /// <summary>
        /// Make deep copy of thickness 
        /// </summary>
        /// <returns>Copy thickness</returns>
        public Thickness DeepCopy()
        {
            Thickness result = new Thickness();
            result.MaxThk = MaxThk;
            result.TD = TD.DeepCopy();
            return result;
        }
    }
}
