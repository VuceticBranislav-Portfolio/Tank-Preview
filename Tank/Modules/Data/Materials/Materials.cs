
/* - Description -
 * Klasa koja sadrzi sve materijale.
 */

using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

namespace Tank
{
    /* - Description -
     * Sadrzi informacije o hederu tablice
     */
    /// <summary>
    /// Data for table header
    /// </summary>
    class HeaderHolder
    {
        public bool Header;                 // Glavni heder tablice. 1 prva celija
        public int Elas;                    // Offset modul elasticnosti hedera
        public int Therm;                   // Offset thermalnog koeficijenta hedera
        public int Thick;                   // Maximum tiknes za material. Prazno za prvi red, 0 za sve tiknese
        public int Re02;                    // Offset 0.2% yield hedera
        public int Re10;                    // Offset 1% yield hedera
        public int Rm;                      // Offset tensile hedera
        public List<double> HeaderElas;     // Heder modula elasticnosti
        public List<double> HeaderTherm;    // Heder termalnog koeficijenta
        public List<double> HeaderRe02;     // Heder yield 0.2% tablice
        public List<double> HeaderRe10;     // Heder yield 1% tablice
        public List<double> HeaderRm;       // Heder tensile tablice
    }

    /* - Description -
     * Drzi sve materijale iz baze.
     */
    /// <summary>
    /// Hold all materials from base in one place
    /// </summary>
    class Materials : DatabaseRequest
    {
        HeaderHolder hd;                            // Header for table
        public Dictionary<int, Material> materials; // Svi materiali su ovde. ID-Material dictionary

        /* - Description -
         * Konstruktor
         */
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tablename"></param>
        public Materials(string tablename)
        {
            hd = new HeaderHolder();
            materials = new Dictionary<int, Material>();
            table = tablename;
        }

        /* - Description -
         * Vraca materijal preko ID
         */
        /// <summary>
        /// Return material from ID.
        /// </summary>
        /// <param name="ID">Material ID</param>
        /// <returns>Material from base or null if not found</returns>
        public Material GetMat(int ID)
        {
            Material m;
            if (materials.TryGetValue(ID, out m))
            {
                return m;
            }
            else return null;
        }

        /* - Description -
         * Override CheckMe() metode iz provajdera
         */
        public override bool CheckMe(Worksheet ws)
        {
            bool result = true;
            result |= Provider.CheckCell(ws, OriginRow, X + 5, out hd.Header);
            result |= Provider.CheckCell(ws, OriginRow, X + 6, out hd.Elas, 0);
            result |= Provider.CheckCell(ws, OriginRow, X + 7, out hd.Therm, 0);
            result |= Provider.CheckCell(ws, OriginRow, X + 8, out hd.Thick, 0);
            result |= Provider.CheckCell(ws, OriginRow, X + 9, out hd.Re02, 0);
            result |= Provider.CheckCell(ws, OriginRow, X + 10, out hd.Re10, 0);
            result |= Provider.CheckCell(ws, OriginRow, X + 11, out hd.Rm, 0);
            return result;
        }

        /* - Description -
         * Override DoMe() metode iz provajdera
         */
        public override bool DoMe(Worksheet ws)
        {
            string[,] data = null;
            string[,] header = null;

            bool result = false;
            status = Status.sError;

            header = GetRange(X, Y - 1, C - 1, 0, ws);
            if (header == null) return result;

            data = GetRange(X, Y, C - 1, R - 1, ws);
            if (data == null) return result;

            // 2. Extract header
            string[] h = Helpers.SliceRow(header, 0).ToArray();
            hd.HeaderElas = ExtractHeader(hd.Elas - 1, hd.Therm - hd.Elas, h).ToList();
            hd.HeaderTherm = ExtractHeader(hd.Therm - 1, hd.Thick - hd.Therm, h).ToList();
            hd.HeaderRe02 = ExtractHeader(hd.Re02 - 1, hd.Re10 - hd.Re02, h).ToList();
            hd.HeaderRe10 = ExtractHeader(hd.Re10 - 1, hd.Rm - hd.Re10, h).ToList();
            hd.HeaderRm = ExtractHeader(hd.Rm - 1, C - hd.Rm + 1, h).ToList(); // avoid -1. do bether

            result |= ReadAll(data);

            if (result) 
            { 
                status = Status.sReady; 
            }
            return result;
        }

        /* - Description -
         * Ubacuje red po red u bazu.
         * Zavisi da li je glavni red ili pomocni.
         * Glavni red sadrzi sve podatke osim yilda i tensila
         * Pomocni redovi su sa yildom i tensilom po tiknesima
         */
        /// <summary>
        /// Read all data from table.
        /// </summary>
        /// <param name="data">Data from table.</param>
        /// <returns>Return true if no error during loading.</returns>
        private bool ReadAll(string[,] data)
        {
            bool result = false;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                switch (Helpers.ObjToInt(data[i, 1]))
                {
                    case 1:
                        result |= AddNewMaterial(Helpers.SliceRow(data, i).ToArray());
                        break;
                    case 2:
                        result |= AddReRm(Helpers.SliceRow(data, i).ToArray());
                        break;
                    default:
                        throw new Helpers.MyException("Error in ReadAll");
                }
            }
            return result;
        }

        /* - Description -
         * Dodaje materijal iz glavnog reda.
         */
        /// <summary>
        /// Add material to colection from base.
        /// </summary>
        /// <param name="data">Data from base.</param>
        /// <returns>Return true if no error during loading.</returns>
        private bool AddNewMaterial(string[] data)
        {
            // 1. Check format
            // 2. Read data
            bool result = false;
            try
            {
                Material NewMat = new Material(data, hd);
                materials.Add(NewMat.ID, NewMat);
                result = true;
            }
            catch (Exception)
            {
                throw new Helpers.MyException("Error in AddNewMaterial");
            }
            return result;
        }

        /* - Description -
         * Dodaje yield i tensile tablicu na postojeci materijal.
         * Ove dve tablice su odvojene od ostatka podataka o materijalu.
         */
        /// <summary>
        /// Additional material data, yieald and tensile.
        /// </summary>
        /// <param name="data">Material data</param>
        /// <returns>Return true if no error during loading</returns>
        private bool AddReRm(string[] data)
        {
            // 1. Check format
            // 2. Find last material
            // 2. Read data
            Thickness thk;
            bool result = false;
            Material LastMat = materials.Last().Value; // resovle problem with no last item to add to

            thk = new Thickness(TabType.Re02, data, hd);
            LastMat.Re02.Thk.Add(thk); // aditional checks needed here
            result = true;

            thk = new Thickness(TabType.Re10, data, hd);
            LastMat.Re10.Thk.Add(thk); // aditional checks needed here
            result = true;

            thk = new Thickness(TabType.Rm, data, hd);
            LastMat.Rm.Thk.Add(thk); // aditional checks needed here
            result = true;

            return result;   // nije zavrseno
        }

        /* - Description -
         * Izvlaci vrednosti iz hedera.
         * Najcesce se tu nalaze temperature.
         */
        /// <summary>
        /// Return data of header.<br/>
        /// Header usually contain temperatures.
        /// </summary>
        /// <param name="start">Start location of header in list</param>
        /// <param name="len">Length of header</param>
        /// <param name="header">Header data</param>
        /// <returns>Values for given header</returns>
        private double[] ExtractHeader(int start, int len, string[] header)
        {
            string[] result = new string[len];
            Array.ConstrainedCopy(header, start, result, 0, len);
            double[] L = new double[len];
            for (int i = 0; i < len; i++)
            {
                double D;
                if (double.TryParse(result[i], out D))
                {
                    L[i] = D;
                }
                else
                {
                    Trace.WriteLine("ExtractHeader null deteced");
                    L[i] = 0;
                }
            }
            return L;
        }

        /* - Description -
         * Vraca material na osnovu datog ID
         */
        /// <summary>
        /// Return material from given ID.
        /// </summary>
        /// <param name="ID">Material ID</param>
        /// <returns>Material with given ID or null</returns>
        public Material GetMaterialFromID(int ID)
        {
            Material m;
            if (materials.TryGetValue(ID, out m))
            {
                return m;
            }
            else return null;
        }
    }
}
