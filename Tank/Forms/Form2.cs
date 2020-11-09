
using System.Windows.Forms;

namespace Tank
{
    public partial class Form2 : Form

    {
        public Form2()
        {
            InitializeComponent();
        }

        /* - Description -
         * Popunjava formu za dodavanje shella
         */
        /// <summary>
        /// Prepare form for shell adding.
        /// </summary>
        /// <param name="name">Provide course name.</param>
        /// <param name="h">Provide height.</param>
        /// <param name="t">Provide thickness.</param>
        /// <param name="mat">Provide material for shell.</param>
        /// <param name="type">Must be 1 for adding new shell. Anything else will be edit.</param>
        internal void Prepare(string name, double h, double t, DataMaterial mat, int type)
        {
            // Dodavanje svih selektovanih materijala
            comboBox1.Items.AddRange(Program.Calc.Input.DataMat.GetMaterialList().ToArray());

            textBox1.Text = name;
            textBox2.Text = h.ToString();
            textBox3.Text = t.ToString();
            if (mat == null)
            {
                // Odabrati prvi materijal ako nema ubacenog...
                comboBox1.SelectedIndex = 0;
            }
            else if (comboBox1.Items.Count > 0)
            {
                // ili pronaci postojeci u komboboksu
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (comboBox1.Items[i].ToString() == mat.Name)
                    {
                        comboBox1.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (type == 1)
            {
                this.Text = "Add shell course...";
            }
            else
            {
                this.Text = "Edit shell course...";
            }
        }
    }
}
