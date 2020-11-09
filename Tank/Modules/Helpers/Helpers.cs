
/* - Description -
 * Da bi se kod drzao sto cistijim sve pomocne metode se nalaze u ovom fajlu.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tank
{
    /* - Description -
     * Sve pomocne stvari vezane za program treba da se nalaze ovde da bi 
     * ocistili program od raznoraznih pomocnih funkcija i grupisali ih na jedno mesto.
     */
    /// <summary>
    /// Helper class <br/>
    /// Contains all stuff that may come handy...
    /// </summary>
    public static class Helpers
    {
        /* - Description -
         * Ovo je specijalana funkcija. Ona dodaje bilo kojoj string promenjivoj novu funkcijonalnost ".ToLength()"
         * Ona obezbeduje da se string persira uvek u tacan broj karaktera. Visak ce biti odsecen a manjak dopunjen spejsovima
         * Ovo je potrebno radi lakseg formatiranja reporta
         */
        /// <summary>
        /// Ensure that string has exact length by trimming or appending spaces to end.
        /// </summary>
        /// <param name="self">String that make request for method call.</param>
        /// <param name="length">String length that need to be ensured.</param>
        /// <returns>Return string with exact length.</returns>
        public static string ToLength(this string self, int length)
        {
            // Ako string nepostoji ni rezultat nece postojati ili ako je trazena duzina < 1
            if ((self == null) || (length < 1)) return null;

            // U zavisnosti da li je duzina stringa veca od trazene duzine... 
            if (self.Length > length)
            {   // odseca se visak karaktera...
                return self.Substring(0, length);
            }
            else
            {   // ili se dodaje potreban broj spejsova na kraj da string bude trazene duzine.
                return self.PadRight(length);
            };
        }

        /* - Description -
         * Ispisuje text u log.
         */
        /// <summary>
        /// Write text to trace log.
        /// </summary>
        public class MyException : Exception
        {
            public MyException(String Msg)
            {
                Trace.Write(Msg);
            }
        }

        /* - Description -
         * Vraca red iz dvodimenzionalne matrice kao 0 based niz.
         */
        /// <summary>
        /// Return row as 0-based array
        /// </summary>
        public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        /* - Description -
         * Convert string to double 
         */
        /// <summary>
        /// Convert string to double.
        /// </summary>
        /// <param name="Obj">String for convert</param>
        /// <returns>Return double or 0 in case of null input</returns>
        public static double ObjToDouble(string Obj)
        {
            double I;
            if (double.TryParse(Obj, out I))
            {
                // Return persed value
                return I;
            }
            else
            {
                // Return 0 if can not parse
                return 0;
            }
        }

        /* - Description -
         * Convert string to nulable double  
         */
        /// <summary>
        /// Convert string to nulable double.
        /// </summary>
        /// <param name="Obj">String for convert</param>
        /// <returns>Return double or null</returns>
        public static double? ToDouble(string Obj)
        {
            double I;
            if (double.TryParse(Obj, out I))
            {
                return I;
            }
            else return null;
        }

        /* - Description -
         * Convert object to string
         */
        /// <summary>
        /// Convert object to string.
        /// </summary>
        /// <param name="Obj">Object for convert</param>
        /// <returns>Return string or 0 in case of null input</returns>
        public static string ObjToStr(object Obj)
        {
            if (Obj != null)
            {
                return Obj.ToString();
            }
            else
            {
                return null;
            }
        }

        /* - Description -
         * Convert string to int 
         */
        /// <summary>
        /// Convert int to double.
        /// </summary>
        /// <param name="Obj">String for convert</param>
        /// <returns>Return int or 0 in case of null input</returns>
        public static int ObjToInt(string Obj)
        {
            int I;
            if (int.TryParse(Obj, out I))
            {
                return I;
            }
            else
            {
                return 0;
            }
        }
    }
}
