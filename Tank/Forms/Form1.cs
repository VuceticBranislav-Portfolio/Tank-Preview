
using System;
using System.Windows.Forms;

namespace Tank
{
    public partial class Form1 : Form
    {
        /* - Description - 
         * Cisti listview pa ga potom popunjava sa podatcima iz liste shellowa
         */
        /// <summary>
        /// Update listview from shell list data.
        /// </summary>
        private void UpdateList()
        {
            // Prvo se obrisu svi itemi u listview
            listView1.BeginUpdate();
            listView1.Items.Clear();

            int i = 1;
            // Dodavanje teksta u listview
            foreach (ShellCourse item in Program.Calc.Input.DataShells.Shells)
            {
                // Dodavanje redova
                ListViewItem ListItem;
                ListItem = listView1.Items.Add(i.ToString());
                ListItem.SubItems.Add(item.Tag);
                ListItem.SubItems.Add(item.PlateHeight.ToString());
                ListItem.SubItems.Add(item.ProvidedCourseThk.ToString());
                ListItem.SubItems.Add(item.Material.Name);
                ListItem.SubItems.Add(string.Format("{0:0.00}", item.Material.AllowableTemperature));
                ListItem.SubItems.Add(string.Format("{0:0.00}", item.Material.AllowableTest));
                i = i + 1;
            }
            listView1.EndUpdate();
        }

        /* - Description -
         * Updetuje vrednosti global promenjivih na formi.
         */
        /// <summary>
        /// Update all inputs on form.
        /// </summary>
        private void UpdateGlobal()
        {
            textBox1.Text = Program.Calc.Input.DataGlobal.Diameter.ToString();
            textBox2.Text = Program.Calc.Input.DataGlobal.CylinderHeight.ToString();
            textBox3.Text = Program.Calc.Input.DataGlobal.LiquidHeight.ToString();
            textBox4.Text = Program.Calc.Input.DataGlobal.DesignPressureInternal.ToString();
            textBox5.Text = Program.Calc.Input.DataGlobal.DesignPressureExternal.ToString();
            textBox6.Text = Program.Calc.Input.DataGlobal.DensityDesign.ToString();
            textBox7.Text = Program.Calc.Input.DataGlobal.DensityTest.ToString();
            textBox8.Text = Program.Calc.Input.DataGlobal.RoofSlope.ToString();
            textBox9.Text = Program.Calc.Input.DataGlobal.LiquidOverflowHeight.ToString();
            textBox10.Text = "Medium";
            textBox11.Text = "Water";
            textBox12.Text = Program.Calc.Input.DataGlobal.DesignTemperature.ToString();
            textBox13.Text = Program.Calc.Input.DataGlobal.CorrAllCourse1.ToString();
            textBox14.Text = Program.Calc.Input.DataGlobal.CorrAllCourses.ToString();
            textBox15.Text = Program.Calc.Input.DataGlobal.ShellInsThk.ToString();
            textBox16.Text = Program.Calc.Input.DataGlobal.WindVelocity.ToString();
            textBox17.Text = Program.Calc.Input.DataGlobal.DensityOfAir.ToString();
            textBox18.Text = Program.Calc.Input.DataGlobal.ShellSurfaceCoeff.ToString();
            textBox19.Text = Program.Calc.Input.DataGlobal.DensityOfSteel.ToString();
            comboBox1.SelectedIndex = Program.Calc.Input.DataGlobal.RoofType;
            comboBox2.SelectedIndex = Program.Calc.Input.DataGlobal.DesignStaticHead;
            comboBox3.SelectedIndex = Program.Calc.Input.DataGlobal.MaterialOfConstruction;
            comboBox4.SelectedIndex = Program.Calc.Input.DataGlobal.ToleranceClass;
        }

        /* - Description -
         * Popunjava privremene podatke u nasu bazu 
         */
        /// <summary>
        /// Make dummy data for calculation.
        /// </summary>
        private void DummyData()
        {
            // Resetovanje Calc promenjive
            Program.Calc = new Calculation();

            // Loadiranje svih resursa iz excela
            Program.Calc.Res = new DataResources(Program.Calc.Options.PathTable);
            if (Program.Calc.Res.LoadAll())
            {
                // MessageBox.Show("All loaded OK", "Message", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Resource load from EXCEL problem !", "Problem here", MessageBoxButtons.OK);
            }

            // Dodavanje par materijala u odabranu listu kao da su seletovani iz baze
            DataMaterial mat;
            mat = new DataMaterial();
            mat.FromBase = true;
            mat.BaseId = 10010;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            mat = new DataMaterial();
            mat.FromBase = true;
            mat.BaseId = 10009;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            mat = new DataMaterial();
            mat.FromBase = true;
            mat.BaseId = 10060;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            mat = new DataMaterial();
            mat.FromBase = false;
            mat.ForTemperature = 200;
            mat.ForThickness = 20;
            mat.Name = "Custom mat #1";
            mat.AllowableTemperature = 123;
            mat.AllowableAmbient = 98;
            mat.AllowableTest = 398;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            mat = new DataMaterial();
            mat.FromBase = false;
            mat.ForTemperature = 180;
            mat.ForThickness = 12;
            mat.Name = "Custom mat #2";
            mat.AllowableTemperature = 223;
            mat.AllowableAmbient = 198;
            mat.AllowableTest = 308;
            Program.Calc.Input.DataMat.AddMaterial(mat);

            // Definisanje default vrednosti za global
            Program.Calc.Input.DataGlobal.Diameter = 3.2;
            Program.Calc.Input.DataGlobal.CylinderHeight = 10.2;
            Program.Calc.Input.DataGlobal.LiquidHeight = 10.2;
            Program.Calc.Input.DataGlobal.RoofType = 1;                 //0 - Dome; 1 - Cone
            Program.Calc.Input.DataGlobal.RoofSlope = 10;
            Program.Calc.Input.DataGlobal.DesignStaticHead = 0;         // 0 - Nominal liquid level; 1 - Full of liquid; 2 - Liquid overflow
            Program.Calc.Input.DataGlobal.LiquidOverflowHeight = 19;
            Program.Calc.Input.DataGlobal.MaterialOfConstruction = 1;   // 0 - Carbon steel; 1 - Stainless steel
            Program.Calc.Input.DataGlobal.DesignPressureInternal = 100;
            Program.Calc.Input.DataGlobal.DesignPressureExternal = 50;
            Program.Calc.Input.DataGlobal.DensityDesign = 1050;
            Program.Calc.Input.DataGlobal.DensityTest = 1000;
            Program.Calc.Input.DataGlobal.DesignTemperature = 110;
            Program.Calc.Input.DataGlobal.CorrAllCourse1 = 0;
            Program.Calc.Input.DataGlobal.CorrAllCourses = 0;
            Program.Calc.Input.DataGlobal.ToleranceClass = 1;           // 0/1/2/3 - Class A/B/C/D
            Program.Calc.Input.DataGlobal.ShellInsThk = 100;
            Program.Calc.Input.DataGlobal.WindVelocity = 45;
            Program.Calc.Input.DataGlobal.DensityOfAir = 1.25;          // Constant
            Program.Calc.Input.DataGlobal.ShellSurfaceCoeff = 0.7;
            Program.Calc.Input.DataGlobal.DensityOfSteel = 8000;

            // Lokalna promenjiva koja ce sadrzati shell koji se treba dodati u listu shelova.
            // Svaki novi shell se prvo mora creirati pa tek onda dodati u listu. 
            ShellCourse shell = null;

            // Adding shell #1 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 1";
            shell.PlateHeight = 2;
            shell.Material = Program.Calc.Input.DataMat.GetById(10009);
            shell.ProvidedCourseThk = 6;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Adding shell #2 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 2";
            shell.PlateHeight = 2;
            shell.Material = Program.Calc.Input.DataMat.GetById(10009);
            shell.ProvidedCourseThk = 5;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Adding shell #3 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 3";
            shell.PlateHeight = 2;
            shell.Material = Program.Calc.Input.DataMat.GetById(10009);
            shell.ProvidedCourseThk = 5;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Adding shell #4 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 4";
            shell.PlateHeight = 2;
            shell.Material = Program.Calc.Input.DataMat.GetById(10009);
            shell.ProvidedCourseThk = 5;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Adding shell #5 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 5";
            shell.PlateHeight = 2;
            shell.Material = Program.Calc.Input.DataMat.GetById(1);
            shell.ProvidedCourseThk = 5;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Adding shell #6 to Calc
            shell = new ShellCourse();
            shell.Tag = "Course 6";
            shell.PlateHeight = 0.2;
            shell.Material = Program.Calc.Input.DataMat.GetById(10009);
            shell.ProvidedCourseThk = 5;
            Program.Calc.Input.DataShells.Shells.Add(shell);

            // Update all
            UpdateGlobal();
            UpdateList();
        }

        /* - Description -
         * Vraca double iz textboxa ili default vrednost ukoliko nije validan broj.
         */
        /// <summary>
        /// Return double from given textbox. <br/>
        /// Has default value in case of invalid string.
        /// </summary>
        /// <param name="tb"> TesxtBox to take value from.</param>
        /// <param name="defaul">Default value in case on invalid text input.</param>
        /// <returns></returns>
        private double GetFromTextBox(TextBox tb, double defaul)
        {
            double dummy;
            if (double.TryParse(tb.Text, out dummy))
            {
                return dummy;
            }
            else return defaul;
        }

        /* - Description -
         * Popunjava podatke u Calc promenjivoj sa textBoxova sa forme
         */
        /// <summary>
        /// Input all global variables from form inputs
        /// </summary>
        private void GetDataFromForm()
        {      
            // Default vrednosti bi trebalo da su u skladu sa onima iz DummyData funkcije, za sada...
            Program.Calc.Input.DataGlobal.Diameter = GetFromTextBox(textBox1, 3.2);
            Program.Calc.Input.DataGlobal.CylinderHeight = GetFromTextBox(textBox2, 10.2);
            Program.Calc.Input.DataGlobal.LiquidHeight = GetFromTextBox(textBox3, 10.2);
            Program.Calc.Input.DataGlobal.DesignPressureInternal = GetFromTextBox(textBox4, 100);
            Program.Calc.Input.DataGlobal.DesignPressureExternal = GetFromTextBox(textBox5, 50);
            Program.Calc.Input.DataGlobal.DensityDesign = GetFromTextBox(textBox6, 1050);
            Program.Calc.Input.DataGlobal.DensityTest = GetFromTextBox(textBox7, 1000);
            Program.Calc.Input.DataGlobal.RoofSlope = GetFromTextBox(textBox8, 10);
            Program.Calc.Input.DataGlobal.LiquidOverflowHeight = GetFromTextBox(textBox9, 19);
            Program.Calc.Input.DataGlobal.DesignTemperature = GetFromTextBox(textBox12, 110);
            Program.Calc.Input.DataGlobal.CorrAllCourse1 = GetFromTextBox(textBox13, 0);
            Program.Calc.Input.DataGlobal.CorrAllCourses = GetFromTextBox(textBox14, 0);
            Program.Calc.Input.DataGlobal.ShellInsThk = GetFromTextBox(textBox15, 100);
            Program.Calc.Input.DataGlobal.WindVelocity = GetFromTextBox(textBox16, 45);
            Program.Calc.Input.DataGlobal.DensityOfAir = GetFromTextBox(textBox17, 1.25);
            Program.Calc.Input.DataGlobal.ShellSurfaceCoeff = GetFromTextBox(textBox18, 0.7);
            Program.Calc.Input.DataGlobal.DensityOfSteel = GetFromTextBox(textBox19, 8000);

            Program.Calc.Input.DataGlobal.RoofType = comboBox1.SelectedIndex;               // 0 - Dome; 1 - Cone
            Program.Calc.Input.DataGlobal.DesignStaticHead = comboBox2.SelectedIndex;       // 0 - Nominal liquid level; 1 - Full of liquid; 2 - Liquid overflow
            Program.Calc.Input.DataGlobal.MaterialOfConstruction = comboBox3.SelectedIndex; // 0 - Carbon steel; 1 - Stainless steel
            Program.Calc.Input.DataGlobal.ToleranceClass = comboBox4.SelectedIndex;         // 0/1/2/3 - Class A/B/C/D

            // Nekoriste se za sad: textBox10, textBox11
            // Update all
            UpdateGlobal();
            UpdateList();
        }

        /* - Description -
         * Auto generisana procedura. Tokom pokretanja programa pravi formu iz textualnog fajla. 
         */
        public Form1()
        {
            InitializeComponent();
        }

        /* - Description -
         * Preuzima vrednosti iz editora i ubacuje u Calc promenivu pa ih koristi za dalje.
         */
        private void button1_Click_Update(object sender, EventArgs e)
        {
            GetDataFromForm();
            Program.Calc.DoReport(richTextBox1);
            // Selektovanje prvog itema u listview, ako postoji.
            if (listView1.Items.Count >= 1)
            {
                listView1.Items[0].Selected = true;
            }
        }

        /* - Description -
         * Automatsko klikanje na PrepareAll button cim se forma prikaze
         */
        private void Form1_Shown(object sender, EventArgs e)
        {
            // Posle klika za popunjavanje vrednosti odma prebaciti fokus na dugme za proracun.
            // Ovo je zgodno da bi se sve radilo na space-space-space :)
            DummyData();
            button1.Focus();
            // Selektovanje prvog itema u listview, ako postoji.
            if (listView1.Items.Count >= 1)
            {
                listView1.Items[0].Selected = true;
            }
        }

        /* - Description - Add Button
         * Add shell button
         */
        private void button2_Click_AddShell(object sender, EventArgs e)
        {
            // Kreiranje promenjive tipa Form2 sa nazivom forma2 
            Form2 form2 = new Form2();

            // Trazenje indeksa za ubacit item...
            int index;
            if (listView1.SelectedItems.Count < 1)
            {
                index = listView1.Items.Count - 1;
            }
            else
            {
                index = listView1.SelectedItems[0].Index;
            }

            // Pripremanje forme2 za prikazivanje
            if (index > 0)
            {
                ShellCourse shl;
                shl = Program.Calc.Input.DataShells.Shells[index];
                form2.Prepare("Course " + (Program.Calc.Input.DataShells.Shells.Count + 1).ToString(), shl.PlateHeight, shl.ProvidedCourseThk, shl.Material, 1);
            }
            else
            {
                form2.Prepare("Course " + (Program.Calc.Input.DataShells.Shells.Count + 1).ToString(), 2, 6, null, 1);
            }

            // Prikazivanje forme i cekanje OK rezultata
            form2.ShowDialog();
            if (form2.DialogResult == DialogResult.OK)
            {
                // Kreiranje TShell klase sa imenom shell
                ShellCourse shell = new ShellCourse();

                // Popunjavanje shella podatcima sa form2
                shell.Tag = form2.textBox1.Text;
                shell.PlateHeight = double.Parse(form2.textBox2.Text);
                shell.ProvidedCourseThk = double.Parse(form2.textBox3.Text);
                shell.Material = Program.Calc.Input.DataMat.GetByName(form2.comboBox1.Text);

                // Dodavanje popunjenog shella u listu shellova
                Program.Calc.Input.DataShells.Shells.Insert(index + 1, shell);
            }

            // Updejtovanje list
            UpdateList();

            // Selektovanje dodatog itema
            listView1.Items[index + 1].Selected = true;
        }

        /* - Description -
         * Promena selektovanog shela
         */
        private void button10_Click_Edit(object sender, EventArgs e)
        {
            // Kreiranje promenjive tipa Form2 sa nazivom forma2 
            Form2 form2 = new Form2();

            // Trazenje indeksa za edit...
            int index;
            if (listView1.SelectedItems.Count < 1) return;
            index = listView1.SelectedItems[0].Index;

            // Pripremanje forme2 za prikazivanje
            if (index > -1)
            {
                ShellCourse shl;
                shl = Program.Calc.Input.DataShells.Shells[index];
                form2.Prepare(shl.Tag, shl.PlateHeight, shl.ProvidedCourseThk, shl.Material, 0);
            }

            // Prikazivanje forme i cekanje OK rezultata
            form2.ShowDialog();
            if (form2.DialogResult == DialogResult.OK)
            {
                // Popunjavanje shella podatcima sa form2
                Program.Calc.Input.DataShells.Shells[index].Tag = form2.textBox1.Text;
                Program.Calc.Input.DataShells.Shells[index].PlateHeight = double.Parse(form2.textBox2.Text);
                Program.Calc.Input.DataShells.Shells[index].ProvidedCourseThk = double.Parse(form2.textBox3.Text);
                Program.Calc.Input.DataShells.Shells[index].Material = Program.Calc.Input.DataMat.GetByName(form2.comboBox1.Text);
            }

            // Updejtovanje list
            UpdateList();

            // Reselektovanje indexa u listi
            if (index > -1)
            {
                listView1.Items[index].Selected = true;
            }
        }

        /* - Description -
         * Simulacija edit dugmeta na dvoklik
         */
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            button10_Click_Edit(null, null);
        }

        /* - Description - Del Button
         * Brise item iz liste shelova dvoklikom
         */
        private void button3_Click_DeleteShell(object sender, EventArgs e)
        {
            // Mora da postoji bar jedan selektovan item u listview...
            if (listView1.SelectedItems.Count < 1) return;

            // Nadje se index tog itema...
            int index = listView1.SelectedItems[0].Index;

            if (index > -1)
            {
                // Brisanje shell elementa sa indexom x
                Program.Calc.Input.DataShells.Shells.RemoveAt(index);

                // Updejtovanje listbox itema
                UpdateList();

                // Selekcija itema ako postoji...
                if (listView1.Items.Count < 1) return;
                if (index < listView1.Items.Count)
                {
                    // Selekcija itema na istom indexu kao pre...
                    listView1.Items[index].Selected = true;
                }
                else
                {
                    // ... ili poslednjeg itema ako nepostoji na starom indexu
                    listView1.Items[listView1.Items.Count - 1].Selected = true;
                }
            }
        }

        /* - Description -
         * Brise sve shelove iz liste
         */
        private void button5_Click_DeleteAll(object sender, EventArgs e)
        {
            Program.Calc.Input.DataShells.Shells.Clear();
            UpdateList();
        }

        /* - Description -
         * Pomera shell za 1 mesto na gore u listi
         */
        private void button6_Click_MoveUp(object sender, EventArgs e)
        {
            // Mora da postoji bar jedan selektovan item u listview...
            if (listView1.SelectedItems.Count < 1) return;

            // Nadje se index tog itema...
            int index = listView1.SelectedItems[0].Index;
            if (index > 0)
            {
                // Zamena itema...
                ShellCourse sc;
                sc = Program.Calc.Input.DataShells.Shells[index];
                Program.Calc.Input.DataShells.Shells[index] = Program.Calc.Input.DataShells.Shells[index - 1];
                Program.Calc.Input.DataShells.Shells[index - 1] = sc;

                // Updejtovanje listbox itema
                UpdateList();

                // Selekcija itema...
                if (index > 0)
                {
                    listView1.Items[index - 1].Selected = true;
                }
            }
        }

        /* - Description -
         * Pomera shell za 1 mesto na gore u listi
         */
        private void button9_Click_MoveDown(object sender, EventArgs e)
        {
            // Mora da postoji bar jedan selektovan item u listview...
            if (listView1.SelectedItems.Count < 1) return;

            // Nadje se index tog itema...
            int index = listView1.SelectedItems[0].Index;
            if (index < Program.Calc.Input.DataShells.Shells.Count - 1)
            {
                // Zamena itema...
                ShellCourse sc;
                sc = Program.Calc.Input.DataShells.Shells[index];
                Program.Calc.Input.DataShells.Shells[index] = Program.Calc.Input.DataShells.Shells[index + 1];
                Program.Calc.Input.DataShells.Shells[index + 1] = sc;

                // Updejtovanje listbox itema
                UpdateList();

                // Selekcija itema...
                if (index < Program.Calc.Input.DataShells.Shells.Count - 1)
                {
                    listView1.Items[index + 1].Selected = true;
                }
            }
        }

        /* - Description -
         * Pokrece material manager dijalog
         */
        private void button7_Click_MaterialManager(object sender, EventArgs e)
        {
            // Prikazivanje forme
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        /* - Description
         * Funkcija za laksi pocetak programiranja.
         * Ove se moze igrati. 
         * Sadrzi kraci primer koriscenja. 
         */
        private void SandBoxFunction(object sender, EventArgs e)
        {
            // Ovde mozes da se vezbas
            // Kratak primer sa sabiranjem i prikazivanjem vrednosti
            // a je 10, b je dizajn pritisak iz Calc klase, s je string za prikazivanje.
            // Prikazivanje je preko dijaloga posto nemas komandnu liniju
            // Note: Dugme "Prepare All" mora biti stisnuto pre ove da bi mogao da se koristi "Calc"

            int a;    // neko a
            double b; // neko b
            string s; // pomocni string

            a = 10;   // a = 10
            b = Program.Calc.Input.DataGlobal.DesignPressureInternal; // b = design pressure
            s = String.Format("Test rezultat\n a={0}, b={1}", a + b, b / a);

            MessageBox.Show(s); // Prikaza rezultata u dijalogu posto nema komandne linije
        }
    }
}
