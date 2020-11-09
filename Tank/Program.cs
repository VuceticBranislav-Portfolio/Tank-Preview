
/* - Description -
 * Ovo je glavni fajl projekta. Ovde se nalazi metoda Tank.TProgram.Main() 
 * koja sadrzi pocetni kod za aplikaciju. Pored toga ovde se nalazi i globalna 
 * promenjiva "Calc" koja sadrzi sve u vezi proracuna.
 */

/* ToDo List
 * List stvari koje bi trebalo uraditi.
 * [ ] Izdvojiti podesavanja od ostatka u zasebnu celinu kako bi program mogao otvoriti vise projekata od jednom...
 * [ ] Revidirati sve exceptionale koji nisu potrebni i izbaciti ih...
 * [ ] Zastita od korisnika na sto vise mesta
 * [ ] Ispeglati koriscenje araya iz excela da uvek budu 0 based
 * Know issues & bugs
 * [ ] Kada se kustom materijal obrise i dalje postoji u shelu. Treba da nije dostupan.
 */

using System;
using System.Windows.Forms;

namespace Tank
{
    static class Program
    {
        /* - Description -
         * Deklarisanje promenjive koja sadrzi sve potrebno za proracun.
         * Treba da se koristi samo jedna instanca ove klase i to ova ovde.
         * Napravljena je kao statik da bi se mogla koristiti svuda.
         */
        /// <summary>
        /// All inputs, results and calculations are located here. <br/>
        /// Report will be generated from this variable too. <br/>
        /// Only this instance should exists.
        /// </summary>
        static public Calculation Calc = new Calculation();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}