
/* - Description -
 * Klasa koja vodi evidenciju o tablicama.
 * Od nje se zahteva tablica i ona izbacuje tablicu iz excel baze.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Tank
{
    public enum Status
    {
        sUnPrepared, // Instanca jos nije pripremljena za rad.
        sError,      // Pokusaj da se pripremi klasa za rad je propao. Razlog moze biti do nedostaka fajla ili neuspela interne validacije
        sReady       // Instanca je spremna za rad samo u ovom slucaju.
    }

    /* - Description -
     * Clasa koja sadrzi sve u vezi materijala. Load iz excel fajla, lista odabranih materijala,
     * custom materijali, i dijalog za odabir materijala.
     * How to use:
     *  1. Kreira se klasa
     *  2. Dodaju se requestovi kroz AddNewRequest.
     *  3. Pokrene se Prepare() sa parametrom staze do excela
     *  4. Proveri se status
     */
    /// <summary>
    /// Contain list of requests and provide data from them.
    /// </summary>
    class Provider
    {
        // Lista svih requestova
        private List<DatabaseRequest> list = new List<DatabaseRequest>();

        // Status providera
        public Status status { get; set; }

        /* - Description -
         * Proverava i priprema excel za loading.
         */
        /// <summary>
        /// Main methos that check is everthing prepare for reading data from excel.
        /// </summary>
        /// <param name="path">Path to excel file</param>
        /// <returns>Return true if no errors and everithing is ready for data loading.</returns>
        public bool Prepare(string path)
        {
            // Algorithm
            // 1. Provera da li path file uopste postoji
            // 2. Provera da li moze da se loadira u interop
            // 3. Validate format
            // 4. Ocitavanje tablica
            // 5. Zatvaranje interopa

            /*  // TODO ovo ispratiti
                1. Never use two dots with an Excel object. 
                2. Always call Marshal.ReleaseComObject on all your objects.
                3. Always dereference your objects when you're through.
            */

            // 1. Check does path file exists
            if (File.Exists(path) == false) return false;

            // 2. Load to interop. If errror occure silent return false  
            // 2.1 Prepare variables. Excel application, workbook and sheet in workbook.
            Excel._Application excel = new Excel.Application();
            Excel.Workbooks ws = null;
            Excel.Workbook wb = null;
            try
            {
                // 2.2 Load file to excel
                ws = excel.Workbooks;
                wb = ws.Open(path, false, true);

                // 3.1. Format validation. Svaka tablica mora prvo da se cekira
                bool AllOk = true;
                for (int i = 0; i < list.Count; i++)
                {  // Basic check
                    AllOk &= TableCheck(list[i], wb);
                }
                if (AllOk == false) return false; // Exit silently if any table is not valid

                // 3.2 Personal validator
                for (int i = 0; i < list.Count; i++)
                {
                    Excel.Worksheet wss = null;
                    try
                    {
                        wss = EX_Worksheet(list[i], wb);
                        AllOk &= list[i].CheckMe(wss);
                    }
                    finally
                    {
                        if (wss != null) Marshal.ReleaseComObject(wss);
                    }
                }
                if (AllOk == false) return false; // Exit silently if any table is not valid

                // 4. Ocitavanje tablica. Svaka tablica se loadira posebno              
                for (int i = 0; i < list.Count; i++)
                {
                    Excel.Worksheet wss = null;
                    try
                    {
                        wss = EX_Worksheet(list[i], wb);
                        AllOk &= list[i].DoMe(wss);
                    }
                    finally
                    {
                        if (wss != null) Marshal.ReleaseComObject(wss);
                    }
                }
                if (AllOk == false) return false;

                // 5. final check            
                for (int i = 0; i < list.Count; i++)
                {
                    AllOk &= list[i].status == Status.sReady;
                }
                if (AllOk == false) return false;

                return AllOk;  // Should be always true at this point
            }
            catch
            {
                Trace.WriteLine("Provider.Prepare error");
                return false;
            } // If error occure silent return false
            finally
            {   // 5. Zatvaranje interopa
                if (wb != null)
                {
                    wb.Close();
                    Marshal.ReleaseComObject(wb);
                }
                if (ws != null)
                {
                    ws.Close();
                    Marshal.ReleaseComObject(ws);
                }
                if (excel != null)
                {
                    excel.Quit();
                    Marshal.ReleaseComObject(excel);
                }
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
        }

        /* - Description -
         * Dodaje novi request u listu zahteva.
         */
        /// <summary>
        /// Add new request for table.
        /// </summary>
        /// <param name="r">Request to be added.</param>
        /// <returns>Return true if no error occur.</returns>
        public bool AddNewRequest(DatabaseRequest r)
        {
            if (r != null)
            {
                r.status = Status.sUnPrepared;
                r.C = 0;
                r.R = 0;
                //r.Progress += ??  // Placeholder to attach progress monitoring method
                list.Add(r);
                return true;
            }
            else return false;
        }

        /* - Description -
         * Proverava postojanje dodatnih podataka pored hedera.
         * To su svi parametri posle 4 glavna.
         * Proverava i donji limit za vrednosti.
         */
        /// <summary>
        /// Check additional header data with lower limit check.
        /// </summary>
        /// <param name="ws">Excel worksheet.</param>
        /// <param name="Row">Header row.</param>
        /// <param name="Col">Header column.</param>
        /// <param name="V">Return true if "header" value is 1.</param>
        /// <param name="limit">lower limit of cell data</param>
        /// <returns>Return true if check is done without error.</returns>
        public static bool CheckCell(Excel.Worksheet ws, int Row, int Col, out int V, int limit)
        {
            V = 0;
            Excel.Range rng = null;
            Excel.Range c = null;
            try // check column offset, must be > 0
            {
                c = ws.Cells;
                rng = c[Row, Col];
                if (rng != null)
                {
                    V = (int)rng.Value2;  // todo fix null
                    if (V < limit) return false;  // uperr limit not checked
                }
                else return false;
            }
            catch
            {
                Trace.WriteLine("Provider.CheckCell error");
                return false;
            }
            finally
            {
                if (rng != null) Marshal.ReleaseComObject(rng);
                if (c != null) Marshal.ReleaseComObject(c);
            }
            return true;
        }

        /* - Description -
         * Proverava postojanje dodatnih podataka pored hedera.
         * To su svi parametri posle 4 glavna.
         * Ne proverava i donji limit za vrednosti.
         */
        /// <summary>
        /// Check additional header data without lower limit check.
        /// </summary>
        /// <param name="ws">Excel worksheet.</param>
        /// <param name="Row">Header row.</param>
        /// <param name="Col">Header column.</param>
        /// <param name="V">Return true if "header" value is 1.</param>
        /// <returns>Return true if check is done without error.</returns>
        public static bool CheckCell(Excel.Worksheet ws, int Row, int Col, out bool V)
        {
            V = false;
            Excel.Range rng = null;
            Excel.Range c = null;
            try // check column offset, must be > 0
            {
                c = ws.Cells;
                rng = c[Row, Col];
                if (rng != null)
                {
                    if ((int)rng.Value2 == 1)
                    {
                        V = true;
                    };
                }
                else return false;
            }
            catch
            {
                Trace.WriteLine("Provider.CheckCell error");
                return false;
            }
            finally
            {
                if (rng != null) Marshal.ReleaseComObject(rng);
                if (c != null) Marshal.ReleaseComObject(c);
            }
            return true;
        }

        /* - Description -
         * Trivijalna provera tablice.
         * Da li posle name celije idu 4 broja i da li su dozvoljenim granicama
         * Ta 4 broja su ofseti po x i y pravcu i velicina tablice po x i y pravcu.
         */
        /// <summary>
        /// Check is table basic matrix allright.
        /// </summary>
        /// <param name="Table">Table request data.</param>
        /// <param name="wb">Excel workbook.</param>
        /// <returns>If true table matrix is allright.</returns>
        private bool TableCheck(DatabaseRequest Table, Excel.Workbook wb)
        {
            Table.status = Status.sError;
            // check if sheet of table can be found, Is table header ok?
            //int sheet = EX_WorksheetIndex(Table, wb);
            //if (sheet == -1) return false;

            Excel.Range range = null;
            Excel.Worksheet ws = null;
            int X = 0, Y = 0, CC = 0, RR = 0, C = 0, R = 0;
            try
            {
                // check is there 4 integers right of origin cell
                range = EX_RefersToRange(Table, wb);
                if (range == null) return false;
                C = range.Column;
                R = range.Row;

                ws = EX_Worksheet(Table, wb);
                if (CheckCell(ws, R, C + 1, out X, 0) == false) return false;
                if (CheckCell(ws, R, C + 2, out Y, 1) == false) return false;
                if (CheckCell(ws, R, C + 3, out CC, 1) == false) return false;
                if (CheckCell(ws, R, C + 4, out RR, 1) == false) return false;
                Table.status = Status.sUnPrepared;
                return true;
            }
            catch
            {
                Trace.WriteLine("Provider.TableCheck error");
                return false;
            }
            finally
            {
                if (range != null) Marshal.ReleaseComObject(range);
                if (ws != null) Marshal.ReleaseComObject(ws);
                Table.C = CC;
                Table.R = RR;
                Table.X = C + X;
                Table.Y = R + Y;
                Table.OriginRow = R;
            }
        }

        /* - Description -
         * Vraca lokaciju prve celije u tablici.
         * Prva celija levo od sebe sadrzi heder tablice koji je dalje definise.
         */
        /// <summary>
        /// Return location of table name cell.
        /// </summary>
        /// <param name="Table">Table request data.</param>
        /// <param name="wb">Excel workbook.</param>
        /// <returns>Return location of table name cell as range.</returns>
        private Excel.Range EX_RefersToRange(DatabaseRequest Table, Excel.Workbook wb)
        {
            if (wb != null)
            {
                Excel.Names names = null;
                Excel.Name name = null;
                try
                {
                    names = wb.Names;
                    name = names.Item(Table.table);
                    return name.RefersToRange;
                }
                catch
                {
                    Trace.WriteLine("Provider.EX_RefersToRange error");
                    return null;
                }
                finally
                {
                    if (name != null) Marshal.ReleaseComObject(name);
                    if (names != null) Marshal.ReleaseComObject(names);
                }
            }
            else return null;
        }

        /* - Description -
         * Vraca excel sheet na kojem se nalazi tablica.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Table">Request data.</param>
        /// <param name="wb">Excel workbook.</param>
        /// <returns>Return excel sheet or null if not found</returns>
        private Excel.Worksheet EX_Worksheet(DatabaseRequest Table, Excel.Workbook wb)
        {
            Excel.Range range = null;
            try
            {
                range = EX_RefersToRange(Table, wb);
                if (range != null)
                {
                    return range.Worksheet;
                }
                else return null;
            }
            catch
            {
                Trace.WriteLine("Provider.EX_Worksheet error");
                return null;
            }
            finally
            {
                if (range != null) Marshal.ReleaseComObject(range);
            }
        }

        /* - Description -
         * Vraca index excel sheeta na kojem se nalazi tablica.
         */
        /// <summary>
        /// Get sheet index from table request.
        /// </summary>
        /// <param name="Table">Request data.</param>
        /// <param name="wb">Excel workbook.</param>
        /// <returns>Return excel sheet index or -1 if not found</returns>
        private int EX_WorksheetIndex(DatabaseRequest Table, Excel.Workbook wb)
        {
            Excel.Worksheet ws = null;
            try
            {
                ws = EX_Worksheet(Table, wb);
                if (ws != null)
                {
                    return ws.Index;
                }
                else return -1;
            }
            catch
            {
                Trace.WriteLine("Provider.EX_WorksheetIndex error");
                return -1;
            }
            finally
            {
                if (ws != null) Marshal.ReleaseComObject(ws);
            }
        }
    }

    /* - Description -
     * Sadrzi podatke potrebne za ocitavanje i lociranje tablice u excelu.
     */
    /// <summary>
    /// Loacate and read table from excel data.
    /// </summary>
    abstract class DatabaseRequest
    {
        public Status status;   // Request status
        public int C;           // Number of columns
        public int R;           // Number of rows
        public int X;           // Column offset
        public int Y;           // Row offset
        public int OriginRow;   // First row in table
        public string table;    // Ima tablice koja se trazi. Excel mora da sadrzi named celiju sa ovim nazivom.
        public abstract bool CheckMe(Excel.Worksheet ws);
        public abstract bool DoMe(Excel.Worksheet ws);
        // Progress report stuff
        public delegate void ThresholdReachedEventHandler(Object sender, int e);
        public event ThresholdReachedEventHandler Progress;

        /* - Description -
         * Vraca matricu podataka iz excela u string formatu
         */
        /// <summary>
        /// Return matrix of string from excel sheet.
        /// Note: All input is 1 based not zero based.
        /// </summary>
        /// <param name="aX">Column offset.</param>
        /// <param name="aY">Row offset.</param>
        /// <param name="aW">Number of columns.</param>
        /// <param name="aH">Number of rows.</param>
        /// <param name="ws">Excel sheet.</param>
        /// <returns>Return 1-based index array of string. See <see cref="string"/>[].</returns>
        public string[,] GetRange(int aX, int aY, int aW, int aH, Excel.Worksheet ws)
        {
            // Initialization
            Excel.Range r = null;
            Excel.Range c = null;
            Excel.Range c1 = null;
            Excel.Range c2 = null;

            // ToDo: Revision needed.
            try
            {
                c = ws.Cells;
                c1 = c[aY, aX];
                c2 = c[aY + aH, aX + aW];
                r = ws.Range[c1, c2];
                object[,] oo = r.Value2;
                string[,] res = new string[aH + 1, aW + 1];
                for (int i = 0; i <= aH; i++)
                {
                    for (int j = 0; j <= aW; j++)
                    {
                        if (oo != null)
                        {
                            if (oo[i + 1, j + 1] != null)
                            {
                                res[i, j] = Helpers.ObjToStr(oo[i + 1, j + 1]);
                            }
                            else
                            {
                                //Trace.WriteLine("Provider.GetRange null detected");
                                res[i, j] = null;
                            }
                        }
                        else
                        {
                            Trace.WriteLine("Provider.GetRange null detected");
                            res[i, j] = null;
                        }
                    }
                }
                return res;
            }
            catch
            {
                Trace.WriteLine("Provider.GetRange error");
                return null;
            }
            finally
            {
                if (c2 != null) Marshal.ReleaseComObject(c2);
                if (c1 != null) Marshal.ReleaseComObject(c1);
                if (c != null) Marshal.ReleaseComObject(c);
                if (r != null) Marshal.ReleaseComObject(r);
            }
        }

        /* - Description -
         * Vraca red podataka iz excela u string formatu
         */
        /// <summary>
        /// Return array of string from excel sheet.
        /// </summary>
        /// <param name="aX">Column offset.</param>
        /// <param name="aY">Row offset.</param>
        /// <param name="aW">Number of columns.</param>
        /// <param name="ws">Excel sheet.</param>
        /// <returns>Return 1-based row of strings.</returns>
        public string[] GetRow(int aX, int aY, int aW, Excel.Worksheet ws)
        {
            // Initialization
            Excel.Range r = null;
            Excel.Range c = null;
            Excel.Range c1 = null;
            Excel.Range c2 = null;

            // ToDo: Revision needed.
            try
            {
                c = ws.Cells;
                c1 = c[aY, aX];
                c2 = c[aY, aX + aW];
                r = ws.Range[c1, c2];
                object[,] oo = r.Value2;
                string[] res = new string[aW + 1];
                for (int j = 0; j <= aW; j++)
                {
                    if (oo != null)
                    {
                        res[j] = Helpers.ObjToStr(oo[1, j + 1]);
                    }
                    else
                    {
                        Trace.WriteLine("Provider.GetRow null read detected");
                        res[j] = null;
                    }
                }
                return res;
            }
            catch
            {
                Trace.WriteLine("Provider.GetRow error");
                return null;
            }
            finally
            {
                if (c2 != null) Marshal.ReleaseComObject(c2);
                if (c1 != null) Marshal.ReleaseComObject(c1);
                if (c != null) Marshal.ReleaseComObject(c);
                if (r != null) Marshal.ReleaseComObject(r);
            }
        }

        /* - Description -
         * Trenutno se nekoristi.
         * Trebalo bi da reportira progres loadiranja materijala za prikazivanje u progress baru.
         */
        /// <summary>
        /// Display material load progress.
        /// </summary>
        /// <param name="e"></param>
        public void DoProgress(int e)
        {
            ThresholdReachedEventHandler handler = Progress;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}