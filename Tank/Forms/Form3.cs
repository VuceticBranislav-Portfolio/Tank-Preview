
using System;
using System.Windows.Forms;

namespace Tank
{
    public partial class Form3 : Form
    {
        // Lokalne promenjive 
        private double Thk;
        private double Temp;

        public Form3()
        {
            InitializeComponent();
        }

        /* - Description -
         * Updejtuje listview sa custom materijalima
         */
        /// <summary>
        /// Update custom materijal listview.
        /// </summary>
        private void UpdateMatSelect()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            int i = 0;
            foreach (var item in Program.Calc.Input.DataMat.Materials)
            {
                ListViewItem ListItem;
                ListItem = listView1.Items.Add((i + 1).ToString());
                if (item.Value.FromBase == true)
                {   // check ID < 0 then print
                    if (item.Value.BaseId > -1)
                    {
                        Material m;
                        m = Program.Calc.Res.Mat.GetMaterialFromID(item.Value.BaseId);
                        ListItem.SubItems.Add(m.ID.ToString());
                        ListItem.SubItems.Add(m.Source);
                        ListItem.SubItems.Add(m.NameSymbolic);
                        ListItem.SubItems.Add(m.NameNumeric);
                        ListItem.SubItems.Add(m.Product);
                        ListItem.SubItems.Add(m.Year.ToString());
                    }
                    else
                    {
                        ListItem.SubItems.Add("x");
                        ListItem.SubItems.Add("Error");
                        ListItem.SubItems.Add("x");
                        ListItem.SubItems.Add("x");
                        ListItem.SubItems.Add("x");
                        ListItem.SubItems.Add("x");
                    }
                }
                else
                {
                    ListItem.SubItems.Add(item.Value.BaseId.ToString());
                    ListItem.SubItems.Add("Custom");
                    ListItem.SubItems.Add(item.Value.Name);
                    ListItem.SubItems.Add("-");
                    ListItem.SubItems.Add("-");
                    ListItem.SubItems.Add("-");
                }
                i = i + 1;
            }
            listView1.EndUpdate();
        }

        /* - Description -
         * Updejtuje listview sa materijalima iz baze
         */
        /// <summary>
        /// Update materijal from base in listview.
        /// </summary>
        void UpdateMatBase()
        {
            listView2.BeginUpdate();
            listView2.Items.Clear();
            int i = 1;
            foreach (Material item in Program.Calc.Res.Mat.materials.Values)
            {
                ListViewItem ListItem;
                ListItem = listView2.Items.Add(i.ToString());
                ListItem.SubItems.Add(item.ID.ToString());
                ListItem.SubItems.Add(item.Source);
                ListItem.SubItems.Add(item.NameSymbolic);
                ListItem.SubItems.Add(item.NameNumeric);
                ListItem.SubItems.Add(item.Product);
                ListItem.SubItems.Add(item.Year.ToString());
                i = i + 1;
            }
            listView2.EndUpdate();
        }

        /* - Description -
         * Updejtuje lokalne promenjive
         */
        /// <summary>
        /// Update local variables
        /// </summary>
        private void UpdateEdits()
        {
            textBox1.Text = Temp.ToString();
            textBox2.Text = Thk.ToString();
        }

        /* - Description -
         * Updejt podataka na listview klik
         */
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInfoData(GetSelectedID(listView2), Program.Calc.Res.Mat, Program.Calc.Input.DataMat);
            UpdateAddMaterial(GetSelectedID(listView2), Program.Calc.Res.Mat, Program.Calc.Input.DataMat);
        }

        /* - Description -
         * Cisti podatke za kliknuti materijal
         */
        /// <summary>
        /// Clear all material data.
        /// </summary>
        private void ClearInfoData()
        {
            label1.Text = "";
            label3.Text = "";
            label5.Text = "";
            label7.Text = "";
            label9.Text = "";
            label11.Text = "";
            label13.Text = "";
            label15.Text = "";
            label17.Text = "";
            label19.Text = "";
            label23.Text = "";
        }

        /* - Description -
         * Updejtuje podatke o selektovanom materijalu
         */
        /// <summary>
        /// Update selected material data in info panel
        /// </summary>
        private void UpdateInfoData(int index, Materials materials, DataMaterials mats)
        {
            if (index > -1)
            {
                label1.Text = index.ToString();
                if (index < 10000)
                {
                    DataMaterial mat2;
                    if (mats.Materials.TryGetValue(index, out mat2))
                    {
                        if (mat2 != null)
                        {
                            label3.Text = "Custom";
                            label5.Text = mat2.Name;
                            label7.Text = "-";
                            label9.Text = "-";
                            label11.Text = "-";
                            label13.Text = mat2.ForTemperature.ToString();
                            label15.Text = mat2.AllowableTemperature.ToString();
                            label17.Text = string.Format("{0:0.00}", mat2.AllowableAmbient);
                            label19.Text = string.Format("{0:0.00}", mat2.AllowableTest);
                            label23.Text = string.Format("{0:0.00}", mat2.ForThickness);
                        }
                    }
                    else
                    {
                        label3.Text = "-";
                        label5.Text = "-";
                        label7.Text = "-";
                        label9.Text = "-";
                        label11.Text = "-";
                        label13.Text = "-";
                        label15.Text = "-";
                        label17.Text = "-";
                        label19.Text = "-";
                        label23.Text = "-";
                    }
                }
                else
                {
                    Material mat;
                    mat = materials.GetMaterialFromID(index);
                    if (mat != null)
                    {
                        label3.Text = mat.Source;
                        label5.Text = mat.NameSymbolic;
                        label7.Text = mat.NameNumeric;
                        label9.Text = mat.Product;
                        label11.Text = mat.Year.ToString();
                        label13.Text = Temp.ToString();
                        label15.Text = string.Format("{0:0.00}", mat.GetAllowable(Temp, Thk, false));  //TProgram.Calc.Data.DataGlobal.DesignTemperature 
                        label17.Text = string.Format("{0:0.00}", mat.GetAllowable(20, Thk, false));
                        label19.Text = string.Format("{0:0.00}", mat.GetAllowable(20, Thk, true));
                        label23.Text = string.Format("{0:0.00}", Thk);
                    }
                    else
                    {
                        label3.Text = "-";
                        label5.Text = "-";
                        label7.Text = "-";
                        label9.Text = "-";
                        label11.Text = "-";
                        label13.Text = "-";
                        label15.Text = "-";
                        label17.Text = "-";
                        label19.Text = "-";
                        label23.Text = "-";
                    }
                }
            }
            else ClearInfoData();
        }

        /* - Description -
         * Dvoklik na materijal iz baze ubacuje isti u listu kustom materijala
         */
        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count < 1) return;

            int index;
            if (int.TryParse(listView2.SelectedItems[0].SubItems[1].Text, out index))
            {
                Material mat;
                mat = Program.Calc.Res.Mat.GetMaterialFromID(index);
                if (mat != null)
                {
                    DataMaterial m;
                    m = new DataMaterial();
                    m.FromBase = true;
                    m.BaseId = mat.ID;
                    Program.Calc.Input.DataMat.AddMaterial(m);
                    UpdateMatSelect();
                }
            }
        }

        /* - Description -
         * Priprema forme na onshow event
         */
        private void Form3_Shown(object sender, EventArgs e)
        {
            Temp = Program.Calc.Input.DataGlobal.DesignTemperature;
            Thk = 15; // todo softcode this
            UpdateEdits();
            UpdateMatBase();
            UpdateMatSelect();
        }

        /* - Description -
         * Dvoklik na kustom materijal izbacuje ga iz liste
         */
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1) return;

            int index;
            if (int.TryParse(listView1.SelectedItems[0].SubItems[1].Text, out index))
            {
                if (Program.Calc.Input.DataMat.Materials.ContainsKey(index))
                {
                    Program.Calc.Input.DataMat.Materials.Remove(index);
                    UpdateMatSelect();
                }
            }
        }

        /* - Description -
         * Klik na listview updejtuje info o materijalu i priprema podatke za ubacivanje novog custom materijala
         */
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInfoData(GetSelectedID(listView1), Program.Calc.Res.Mat, Program.Calc.Input.DataMat);
            UpdateAddMaterial(GetSelectedID(listView1), Program.Calc.Res.Mat, Program.Calc.Input.DataMat);
        }

        /* - Description -
         * Updejtuje podatke za dodavanje custom materijala.
         */
        /// <summary>
        /// Prepare data for custom material
        /// </summary>
        private void UpdateAddMaterial(int index, Materials materials, DataMaterials mats)
        {
            // Ako postoji index...
            if (index > -1)
            {
                // ... kada je index manji od 10000 onda je to kustom materijal ...
                if (index < 10000)
                {
                    // Privremeni kustom materijal i pokusaj cupanja istog iz baze
                    DataMaterial mat2;
                    if (mats.Materials.TryGetValue(index, out mat2))
                    {
                        // Ako je pronaden materijal popuniti podatke...
                        if (mat2 != null)
                        {
                            textBox3.Text = mat2.Name;
                            textBox4.Text = mat2.ForTemperature.ToString();
                            textBox8.Text = string.Format("{0:0.00}", mat2.ForThickness);
                            textBox5.Text = string.Format("{0:0.00}", mat2.AllowableTemperature);
                            textBox6.Text = string.Format("{0:0.00}", mat2.AllowableAmbient);
                            textBox7.Text = string.Format("{0:0.00}", mat2.AllowableTest);
                        }
                    }
                    else
                    {
                        // ... ako nije nadjen onda crta-crta-crtica.
                        textBox3.Text = "-";
                        textBox4.Text = "-";
                        textBox5.Text = "-";
                        textBox6.Text = "-";
                        textBox7.Text = "-";
                        textBox8.Text = "-";
                    }
                }
                else
                {
                    // Za materijal iz baze...
                    Material mat;
                    mat = materials.GetMaterialFromID(index);
                    if (mat != null)
                    {
                        // Ako je pronaden materijal popuniti podatke...
                        textBox3.Text = mat.NameSymbolic;
                        textBox4.Text = Temp.ToString();
                        textBox8.Text = string.Format("{0:0.00}", Thk);
                        textBox5.Text = string.Format("{0:0.00}", mat.GetAllowable(Temp, Thk, false));
                        textBox6.Text = string.Format("{0:0.00}", mat.GetAllowable(20, Thk, false));
                        textBox7.Text = string.Format("{0:0.00}", mat.GetAllowable(20, Thk, true));
                    }
                    else
                    {
                        // ... ako nije nadjen onda crta-crta-crtica.
                        textBox3.Text = "-";
                        textBox4.Text = "-";
                        textBox5.Text = "-";
                        textBox6.Text = "-";
                        textBox7.Text = "-";
                        textBox8.Text = "-";
                    }
                }
            }
            else ClearAddMaterialData();
        }

        /* - Description -
         * Cisti podatke za dodavanje novog custom materijala
         */
        /// <summary>
        /// Clear all new custom material data.
        /// </summary>
        private void ClearAddMaterialData()
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
        }

        private void textBox2_Update(object sender, EventArgs e)
        {
            double t;
            if (double.TryParse(textBox2.Text, out t))
            {
                Thk = t;
            }
            else
            {
                Thk = 15; // TODO Make this softcoded // TODO Make same as in form show
            }
            UpdateEdits();
            //listView1_SelectedIndexChanged(null, null);
            listView2_SelectedIndexChanged(null, null);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                textBox2_Update(sender, null);
            }
        }

        private void textBox1_Update(object sender, EventArgs e)
        {
            double t;
            if (double.TryParse(textBox1.Text, out t))
            {
                Temp = t;
            }
            else
            {
                Temp = Program.Calc.Input.DataGlobal.DesignTemperature; // TODO Make same as in form show
            }
            UpdateEdits();
            UpdateInfoData(GetSelectedID(listView2), Program.Calc.Res.Mat, Program.Calc.Input.DataMat);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                textBox1_Update(sender, null);
            }
        }

        /* - Description -
         * Enumeracija kolona u listview
         */
        /// <summary>
        /// Names of columns in listview table.
        /// </summary>
        enum ColumnNames : int
        {
            /// <summary>
            /// Number.
            /// </summary>
            Index = 0,
            /// <summary>
            /// Material index. Must be unique. For material from base should start from 10000.
            /// </summary>
            ID = 1,
            /// <summary>
            /// Material standard.
            /// </summary>
            Standard = 2,
            /// <summary>
            /// Material symbolic name.
            /// </summary>
            Symbolic = 3,
            /// <summary>
            /// Material numeric name.
            /// </summary>
            Numeric = 4,
            /// <summary>
            /// Material product form.
            /// </summary>
            Product = 5,
            /// <summary>
            /// Year of material standard.
            /// </summary>
            Year = 6
        }

        /* - Description -
         * Vraca material index iz listview
         */
        /// <summary>
        /// Return material id from listview table.
        /// </summary>
        /// <param name="lv">List view.</param>
        /// <returns>Return material index or -1.</returns>
        public static int GetSelectedID(ListView lv)
        {
            if (lv == null) return -1;
            if (lv.SelectedItems.Count < 1) return -1;
            int index;
            if (int.TryParse(lv.SelectedItems[0].SubItems[(int)ColumnNames.ID].Text, out index))
            {
                return index;
            }
            else return -1;
        }

        /* - Description -
         * Dodaje novi custom material.
         */
        private void button2_Click_AddMaterial(object sender, EventArgs e)
        {
            // Materijal se dodaje samo ako su svi unosi validni.
            string name = textBox3.Text;
            double Temp;
            if (double.TryParse(textBox4.Text, out Temp) == false) return;
            double Thk;
            if (double.TryParse(textBox8.Text, out Thk) == false) return;
            double AllD;
            if (double.TryParse(textBox5.Text, out AllD) == false) return;
            double AllA;
            if (double.TryParse(textBox6.Text, out AllA) == false) return;
            double AllT;
            if (double.TryParse(textBox7.Text, out AllT) == false) return;

            // Dodavanje materijala
            DataMaterial mat;
            mat = new DataMaterial();
            mat.FromBase = false;
            mat.ForTemperature = Temp;
            mat.ForThickness = Thk;
            mat.Name = name;
            mat.AllowableTemperature = AllD;
            mat.AllowableAmbient = AllA;
            mat.AllowableTest = AllT;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            UpdateMatSelect();
        }
    }
}