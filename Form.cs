using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WF1_Tech_Test_ZJWyrd
{
    public partial class Form : System.Windows.Forms.Form
    {
        DataTable Table;

        public Form()
        {
            InitializeComponent();
        }

        private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!e.TabPage.Enabled)
            {
                e.Cancel = true;
                MessageBox.Show("Please load a JSON file first");
            }
        }

        private void LoadClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Table = new DataTable();
                try
                {
                    string extension = Path.GetExtension(openFileDialog.FileName);
                    // CSV Doesnt automatically parse to the correct datatype and my attempt here didnt work so ive moved on to just focus on JSON
                    #region CSV Read/Parse
                    /*if (extension.ToLower() == ".csv") 
                    {
                        using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                        {
                            string[] Headers = sr.ReadLine().Split(','); // grab the first line that has all the headers.

                            foreach (string H in Headers)
                            {
                                Table.Columns.Add(H);
                            }

                            while (!sr.EndOfStream)
                            {
                                string[] line = sr.ReadLine().Split(',');
                                DataRow row = Table.NewRow();
                                for (int i = 0; i < Table.Columns.Count; i++)
                                {
                                    if (line[i][0] != '"')
                                    {
                                        if (line[i].Contains('.'))
                                            row[i] = double.Parse(line[i]);
                                        else 
                                            row[i] = int.Parse(line[i]);
                                    }
                                    else
                                        row[i] = line[i];
                                }
                                Table.Rows.Add(row);
                            }
                        }

                    }
                    else */
                    #endregion
                    if (extension.ToLower() == ".json")
                    {
                        using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                        {
                            Table = (DataTable)JsonConvert.DeserializeObject(sr.ReadToEnd(), (typeof(DataTable)));
                        }
                    }
                    else
                    {
                        throw new Exception("File not handled. Please use JSON.");
                    }

                    DataLoaded();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("uh oh. " + ex.Message);
                }
            }
        }

        private void ChartLoadClick(object sender, EventArgs e)
        {
            string X = (string)listBox1.SelectedValue;
            string Y = (string)listBox1.SelectedValue;

            /// ran out of time to make this work but here is the vision
            /// 1) Select two columns from the two list boxes
            /// 2) get a new DataTable with only the two chosen columns.
            /// 3) If necessary, group the Y values based on X values, though this might work naturally
            /// 4) Based on Things, choose whether bar or scatter graphs are best.
            ///     - example 1) Lap against Lap Time scatter graph
            ///     - example 2) Driver ID against Points / Wins Bar Chart
            ///         - Would need to add multiselect to second listbox 
            ///     - example 3) Scatter graph of Latitude vs Longitude might look nice
            /// 5) Some sort of refining to say, only get top 10 drivers from example 2.
            ///     Another Input for refining in such a way
            /// 6) export graph as [Format] (Image, Data, etc)

            chart1.Update();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
        }

        private void DataLoaded()
        {
            TableTab.Enabled = true;
            ChartTab.Enabled = true;

            if (Table.Rows.Count < 500) // arbitrary number
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            else
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Need to find a library / package for proper sorting, looking for an excel-esque filter row.
            dataGridView1.DataSource = Table;

            listBox1.DataSource = Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            listBox2.DataSource = Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            tabControl1.SelectedIndex = 1;
        }
    }
}
