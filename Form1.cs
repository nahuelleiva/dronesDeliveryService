using System.ComponentModel;
using System.Data;

namespace DronesDeliveryService
{
    public partial class Form1 : Form
    {
        private static Dictionary<string, int> locations = new();
        private static Dictionary<string, int> dronesAndWeights = new();
        private static Dictionary<string, List<List<int>>> possibleRoutes = new();

        public Form1()
        {
            InitializeComponent();
            LoadAppInitialState();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            DialogResult = openFileDialog1.ShowDialog();

            if (DialogResult == DialogResult.OK)
            {
                label1.Text = Path.GetFileName(openFileDialog1.FileName);
                foreach (string line in File.ReadLines(openFileDialog1.FileName))
                {
                    if (line.Contains("Drone"))
                    {
                        dronesAndWeights = Utils.Utils.GetDronesAndWeights(line);
                    } else
                    {
                        var (key, value) = Utils.Utils.GetLocations(line);
                        locations.Add(key, value);
                    }
                }
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                label2.Text = "Computing best routes. The results will be ready shortly...";
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DataTable dt = new();
            dt.Columns.Add("Results");

            var weights = locations.Select(x => x.Value).ToArray();


            foreach (var item in dronesAndWeights)
            {
                var bestRoutes = Utils.Utils.GetBestRoutes(item.Value, weights);

                if (bestRoutes.Any())
                {
                    possibleRoutes.Add(item.Key, bestRoutes);
                }
            }

            if (possibleRoutes.Count > 0)
            {
                foreach (var item in possibleRoutes)
                {
                    int index = 1;

                    // Adding the drone name
                    dt.Rows.Add(item.Key);

                    var routeWeights = item.Value;
                    var groupedWeights = routeWeights.ToLookup(x => x.Count);

                    foreach (var weight in groupedWeights.ToList())
                    {
                        // Adding trip #
                        dt.Rows.Add($"Trip #{index}");

                        // Getting drones best routes
                        var routes = Utils.Utils.GetDronesRoutes(locations, weight.ToList()[0]);

                        // Adding the routes in a new row
                        dt.Rows.Add(string.Join(", ", routes));
                        index++;
                    }

                    // Adding an empty row as separator
                    dt.Rows.Add();
                }
            }

            UpdateDataGridView(dt);
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // We clear our variables in any case, so we can restart the process of
            // calculating the routes if the user clicks again on the button
            LoadAppInitialState();

            if (e.Cancelled == true)
            {
                label2.Text = "Process has been canceled!";
            }
            else if (e.Error != null)
            {
                label2.Text = "Error: " + e.Error.Message;
            } else
            {
                label2.Text = "Done!";
            }
        }

        // Action to update the data grid view once the backgroundworker has finished its process
        private void UpdateDataGridView(DataTable dt)
        {
            if (dataGridView1.InvokeRequired)
            { 
                dataGridView1.Invoke(new Action(() => UpdateDataGridView(dt)));
            } else
            {
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void LoadAppInitialState()
        {
            dronesAndWeights.Clear();
            locations.Clear();
            possibleRoutes.Clear();
            button2.Enabled = false;
        }
    }
}