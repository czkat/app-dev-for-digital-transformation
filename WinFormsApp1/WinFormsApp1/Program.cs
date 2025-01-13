using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TrafficLightsGrid
{
    public class TrafficLightForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private int currentLightIndex = 0; // 0 = Red, 1 = Yellow, 2 = Green
        private bool showChartPage = false; // Determines the current "page"

        public TrafficLightForm()
        {
            this.Text = "Traffic Lights Grid";
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;

            // Initialize timer
            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // 1 second
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            this.MouseClick += TrafficLightForm_MouseClick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!showChartPage)
            {
                currentLightIndex = (currentLightIndex + 1) % 3; // Cycle through 0, 1, 2
                this.Invalidate(); // Redraw the form
            }
        }

        private void TrafficLightForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!showChartPage)
            {
                // Bounding box of the first traffic light
                Rectangle firstTrafficLightBounds = new Rectangle(150, 50, 80, 240);

                if (firstTrafficLightBounds.Contains(e.Location))
                {
                    // Switch to chart page
                    ShowChartPage();
                }
            }
        }


        private void ShowChartPage()
        {
            showChartPage = true;

            // Clear all existing controls
            this.Controls.Clear();

            // Create a TableLayoutPanel to organize the charts
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };

            // Adjust column and row styles to evenly distribute space
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Create and add four charts to the tableLayoutPanel
            for (int i = 0; i < 4; i++)
            {
                Control chartWithDescriptionPanel = CreateChartWithDescription(i); // Create chart with description
                tableLayoutPanel.Controls.Add(chartWithDescriptionPanel, i % 2, i / 2); // Add the panel to the grid
            }

            // Add the TableLayoutPanel to the form
            this.Controls.Add(tableLayoutPanel);

            // Add a "Back" button
            Button backButton = new Button
            {
                Text = "Back",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            backButton.Click += BackButton_Click;

            this.Controls.Add(backButton);
        }


        
        private Control CreateChartWithDescription(int index)
        {
            // Create the Chart control
            Chart chart = new Chart
            {
                Dock = DockStyle.Top,
                Height = 300 // Adjust height as needed for chart
            };

            // Create a chart area
            ChartArea chartArea = new ChartArea($"ChartArea{index}");
            chart.ChartAreas.Add(chartArea);

            // Set the Y-axis title to "Hours"
            chartArea.AxisY.Title = "Hours";
            chartArea.AxisY.TitleFont = new Font("Arial", 12, FontStyle.Bold); // Optional: Customize font

            // Create a series and add data points
            Series series = new Series($"Series{index}");

            switch (index)
            {
                case 0: // Column Chart
                    series.ChartType = SeriesChartType.Column;
                    series.Points.AddXY("Red", 10);
                    series.Points.AddXY("Yellow", 5);
                    series.Points.AddXY("Green", 8);
                    break;

                case 1: // Pie Chart (Red, Yellow, Green)
                    series.ChartType = SeriesChartType.Pie;
                    series.Points.AddXY("Red", 40);
                    series.Points.AddXY("Yellow", 30);
                    series.Points.AddXY("Green", 30);

                    series.Points[0].Color = Color.Red;
                    series.Points[1].Color = Color.Yellow;
                    series.Points[2].Color = Color.Green;
                    series.Label = "#PERCENT";
                    break;

                case 2: // Line Chart
                    series.ChartType = SeriesChartType.Line;
                    series.Points.AddXY("Day 1", 10);
                    series.Points.AddXY("Day 2", 15);
                    series.Points.AddXY("Day 3", 8);
                    series.Points.AddXY("Day 4", 12);
                    break;

                case 3: // Area Chart
                    series.ChartType = SeriesChartType.Area;
                    series.Points.AddXY("January", 30);
                    series.Points.AddXY("February", 25);
                    series.Points.AddXY("March", 35);
                    series.Points.AddXY("April", 40);
                    break;
            }

            chart.Series.Add(series);

            // Create the description label
            Label descriptionLabel = new Label
            {
                Text = GetChartDescription(index), // Get the description for this chart
                Dock = DockStyle.Bottom,
                Height = 40, // Adjust the height as needed for description
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = Color.Black
            };

            // Create a panel to hold the chart and description
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill, // Fill the panel to make use of available space
                Padding = new Padding(10)
            };
            panel.Controls.Add(chart);
            panel.Controls.Add(descriptionLabel);

            return panel;
        }

        private string GetChartDescription(int index)
        {
            switch (index)
            {
                case 0:
                    return "This is a column chart showing traffic light cycles in red, yellow, and green.";
                case 1:
                    return "This pie chart shows the distribution of red, yellow, and green light cycles.";
                case 2:
                    return "This is a line chart tracking traffic light data over time.";
                case 3:
                    return "This area chart represents traffic light data trends over several months.";
                default:
                    return string.Empty;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            // Go back to the traffic lights page
            showChartPage = false;

            // Clear all existing controls and refresh
            this.Controls.Clear();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!showChartPage)
            {
                Graphics g = e.Graphics;

                // Load the smaller image for the grid (ensure the path is correct)
                Image gridImage = Image.FromFile(@"C:\Users\admin\Desktop\net\winforms\WinFormsApp1\cnc.png");

                // Define grid parameters
                int cols = 4; // Number of columns
                int rows = 2; // Number of rows
                int cellWidth = this.ClientSize.Width / cols; // Calculate width of each grid cell
                int cellHeight = this.ClientSize.Height / rows; // Calculate height of each grid cell

                // Draw the image in a grid
                for (int col = 0; col < cols; col++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        // Calculate the top-left corner of the current grid cell
                        int x = col * cellWidth;
                        int y = row * cellHeight;

                        // Draw the image scaled to fit the cell
                        g.DrawImage(gridImage, x, y, cellWidth, cellHeight);
                    }
                }

                // Dispose the image to free resources
                gridImage.Dispose();

                // Optionally draw the traffic lights on top
                DrawTrafficLights(g);
            }
        }

        private void DrawTrafficLights(Graphics g)
        {
            int cols = 4; // Number of columns
            int rows = 2; // Number of rows

            // Define grid cell size
            int cellWidth = this.ClientSize.Width / cols;
            int cellHeight = this.ClientSize.Height / rows;

            // Traffic light size
            int lightWidth = 80;
            int lightHeight = 240;

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    // Calculate the top-left corner of the current grid cell
                    int cellX = col * cellWidth;
                    int cellY = row * cellHeight;

                    // Center the traffic light within the grid cell
                    int x = cellX + (cellWidth - lightWidth) / 2;
                    int y = cellY + (cellHeight - lightHeight) / 2;

                    // For the first traffic light, use the changing light index
                    int lightState = (col == 0 && row == 0) ? currentLightIndex : 0; // Other lights are always red

                    // Draw a single traffic light
                    DrawTrafficLight(g, x, y, lightState);
                }
            }
        }


        private void DrawTrafficLight(Graphics g, int x, int y, int lightState)
        {
            // Traffic light rectangle (outer boundary)
            int trafficLightWidth = 80;
            int trafficLightHeight = 240;

            // Draw black boundary
            g.FillRectangle(Brushes.Black, x, y, trafficLightWidth, trafficLightHeight);

            // Draw red light
            DrawLight(g, x, y, lightState == 0 ? Brushes.Red : Brushes.Gray);

            // Draw yellow light
            DrawLight(g, x, y + 80, lightState == 1 ? Brushes.Yellow : Brushes.Gray);

            // Draw green light
            DrawLight(g, x, y + 160, lightState == 2 ? Brushes.Green : Brushes.Gray);
        }

        private void DrawLight(Graphics g, int x, int y, Brush color)
        {
            int lightDiameter = 60; // Diameter of each light
            int lightPadding = 10;  // Padding inside the black boundary

            // Draw circular light
            g.FillEllipse(color, x + lightPadding, y + lightPadding, lightDiameter, lightDiameter);

            // Draw black boundary around the light
            g.DrawEllipse(Pens.Black, x + lightPadding, y + lightPadding, lightDiameter, lightDiameter);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrafficLightForm());
        }
    }
}

