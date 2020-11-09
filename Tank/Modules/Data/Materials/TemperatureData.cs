
/* - Description -
 * Klasa koja se bavi tablicom temperatura i vrednostima za svaku temperaturu
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tank
{
    /* - Description -
     * Enumeracija za tablice koje sadrze vrednosti po temperaturi
     */
    /// <summary>
    /// Enumeration of temperature tables types
    /// </summary>
    public enum TabType
    {
        /// <summary>
        /// Elasticity
        /// </summary>
        Elas = 1,
        /// <summary>
        /// Thermal coefficient
        /// </summary>
        Therm = 2,
        /// <summary>
        /// Yeald stress 0.2%
        /// </summary>
        Re02 = 3,
        /// <summary>
        /// Yield stress 1%
        /// </summary>
        Re10 = 4,
        /// <summary>
        ///  Tensile stress
        /// </summary>
        Rm = 5
    }

    /* - Description -
     * Tablica koja sadrzi vrednosti po temperaturi
     */
    /// <summary>
    /// Temperature table with values
    /// </summary>
    class TemperatureData
    {
        /* - Description -
         * Jedan unos u tablicu. Par temperatura-vrednost.
         */
        /// <summary>
        /// Temperature-value holder
        /// </summary>
        private struct DataHolder
        {
            // Both temperature and value are nulable internally
            internal double? Temp;  // Internal temperature
            internal double? Val;   // Internal value

            /* - Description -
             * Public property for temperature. Read only.
             */
            /// <summary>
            /// Temperature
            /// </summary>
            public double Temperature
            {
                get
                {
                    // Nulable result assign
                    return Temp ?? throw new Helpers.MyException("Invalid Temp data");
                }
            }

            /* - Description -
             * Public property for temperature value. Read only.
             */
            /// <summary>
            /// Value at temperature
            /// </summary>
            public double Value
            {
                get
                {
                    // Nulable result assign
                    return Val ?? throw new Helpers.MyException("Invalid Val data");
                }
            }

            /* - Description -
             * Duboka kopija podataka
             */
            /// <summary>
            /// Deep copy of data holder
            /// </summary>
            /// <returns>Copy of data</returns>
            public DataHolder DeepCopy()
            {
                DataHolder result = new DataHolder();
                result.Temp = Temp;
                result.Val = Val;
                return result;
            }
        }

        /* - Description -
         * Lista parova temperatura-vrednost
         */
        /// <summary>
        /// List of temperature-value pairs.
        /// </summary>
        private List<DataHolder> Data = new List<DataHolder>();

        /* - Description -
         * Simple constructor
         */
        /// <summary>
        /// Simple Constructor
        /// </summary>
        public TemperatureData()
        {
        }

        /* - Description -
         * Konstruktor koji odma popunjava podatke
         */
        /// <summary>
        /// Advance constructor basd on provided data.
        /// </summary>
        /// <param name="typ">Table type</param>
        /// <param name="hdr">Header data</param>
        /// <param name="data">Row data</param>
        public TemperatureData(TabType typ, HeaderHolder hdr, string[] data)
        {
            // Initialization
            List<double> Dummy;
            int offset;

            // Prepare values for future use
            switch (typ)
            {
                case TabType.Elas:
                    Dummy = hdr.HeaderElas;
                    offset = hdr.Elas - 1;
                    break;
                case TabType.Therm:
                    Dummy = hdr.HeaderTherm;
                    offset = hdr.Therm - 1;
                    break;
                case TabType.Re02:
                    Dummy = hdr.HeaderRe02;
                    offset = hdr.Re02 - 1;
                    break;
                case TabType.Re10:
                    Dummy = hdr.HeaderRe10;
                    offset = hdr.Re10 - 1;
                    break;
                case TabType.Rm:
                    Dummy = hdr.HeaderRm;
                    offset = hdr.Rm - 1;
                    break;
                default:
                    Dummy = null;
                    offset = 0;
                    break;
            }

            // Throw error if invalid header
            if (Dummy == null)
            {
                throw new Helpers.MyException("TemperatureData constructor error. No header data. Invalid type.");
            }

            // Clear list and repopulate it
            Data.Clear();
            for (int i = 0; i < Dummy.Count; i++)
            {
                // Prepare one entry
                DataHolder dh = new DataHolder();

                // Populate temperature data from header and value from data
                dh.Temp = Dummy[i];
                dh.Val = Helpers.ToDouble(data[offset + i]);

                // Add pair only if Val has value. No need for incomplete pair.
                if (dh.Val != null)
                {
                    Data.Add(dh);
                }
            }
        }

        /* - Description -
         * Vraca interpoliranu vrednost za datu temperaturu
         */
        /// <summary>
        /// Return interpolated value for given temperature<vr/>
        /// Extrapolation is not allowed
        /// </summary>
        /// <param name="Temperature">Interpolation temperature</param>
        /// <returns>Value at given temperature</returns>
        public double GetValue(double Temperature)
        {
            // 1. Get values for interppolation
            // 2. If values invalid throw error
            // 3. Do interpolation
            if (InterpolateIndex(Temperature, out int V1, out int V2))
            {
                if ((Data[V2].Temperature - Data[V1].Temperature) == 0)
                {
                    // No interpolation needed if temperatures are same
                    return Data[V1].Value;
                }
                else if ((Data[V1].Temperature > Temperature) || (Data[V2].Temperature < Temperature))
                {
                    // ToDo: Extrapolation not allowed
                    //throw new Helpers.MyException("Extrapolation not allowed in TemperatureData.GetValue");
                    Trace.WriteLine("Extrapolation not allowed - ");
                    //return Data[V1].Value + (Temperature - Data[V1].Temperature) * (Data[V2].Value - Data[V1].Value) / (Data[V2].Temperature - Data[V1].Temperature);
                    if (Data[V2].Temperature < Temperature)
                    {
                        return Data[V2].Value;
                    }
                    if (Data[V1].Temperature > Temperature)
                    {
                        return Data[V1].Value;
                    }
                }
                // Regular interpolation
                return Data[V1].Value + (Temperature - Data[V1].Temperature) * (Data[V2].Value - Data[V1].Value) / (Data[V2].Temperature - Data[V1].Temperature);
            }
            else
            {
                throw new Helpers.MyException("Invalid interpolation table");
            }
        }

        /* - Description -
         * Vraca dva indeksa vrednosti za interpolaciju.
         * Vrednost manja i veca od trazene.
         */
        /// <summary>
        /// Return two index of temperature for interpolation 
        /// </summary>
        /// <param name="val">Requested temperature</param>
        /// <param name="V1">Lower value then requested</param>
        /// <param name="V2">Higher value then requested</param>
        /// <returns>Return true if solution is found</returns>
        private bool InterpolateIndex(double val, out int V1, out int V2)
        {
            // ToDo: Napraviti da prepoznaje iste rojeve pri sortiranju
            V1 = 0;
            V2 = 0;

            // Find index
            if (Data.First().Temperature > Data.Last().Temperature)
            {
                // Descending
                throw new Exception("InterpolateIndex not implemented - Descending");
            }
            else
            {
                // Ascending
                V1 = Data.Count() - 1;
                for (int i = 0; i < Data.Count(); i++)
                {
                    if (val <= Data[i].Temperature)
                    {
                        V1 = i;
                        break;
                    }
                }

                // Fixups
                if (V1 == Data.Count() - 1) // if v1=first
                {
                    V2 = V1;
                    V1 = Math.Max(0, V2 - 1);
                }
                else if (V1 == 0)
                {
                    V2 = V1 + 1;
                }
                else
                {
                    // regular case
                    V2 = V1;
                    V1 = V2 - 1;
                }
            };
            return true;
        }

        /* - Description -
         * Duboka kopija liste temperatura-vrednost parova
         */
        /// <summary>
        /// Deep copy of temperature data pairs.
        /// </summary>
        /// <returns>Copy of temperature data</returns>
        public TemperatureData DeepCopy()
        {
            // Create new instance of class
            TemperatureData result = new TemperatureData();

            // Copy all data from old to new instance
            result.Data = new List<DataHolder>();
            for (int i = 0; i < Data.Count; i++)
            {
                result.Data.Add(Data[i].DeepCopy());
            }

            // Return result
            return result;
        }
    }
}
