
/* - Description -
 * Ovaj fajl sadrzi klase koje se koriste za rad sa opcijama i podesavanjima aplikacije.
 * Takodje sadrzi lokacije externih fajlova.
 * Sva podesavanja vezana za samu aplikaciju treba da se nalaze ovde.
 */

using System.Windows.Forms;

namespace Tank
{
    /* - Description -
     * Sve opcije vezane za aplikaciju treba da se nalaze ovde.
     * Podesavanja aplikacije, folderi...
     */
    /// <summary>
    /// All application options are located here. <br/>
    /// Stuff like default folders, application behavior and stuff like this...
    /// </summary>
    class DataOptions
    {
        public string PathApplication;  // Application root folder
        public string PathFiles;        // Application "Files" folder should be located in application root
        public string PathTable;        // "Tables.xlsx" file with all tables should be in "Files" folder

        /// <summary>
        /// Default constructor. <br/>
        /// Should be call once at startup, so it be prepared for future use.
        /// </summary>
        public DataOptions()
        {
            PathApplication = Application.StartupPath + @"\";
            PathFiles = PathApplication + @"Files\";
            PathTable = PathFiles + @"Tables.xlsx";
        }
    }
}