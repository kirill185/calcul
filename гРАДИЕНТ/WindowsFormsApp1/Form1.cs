using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace CssGradientGenerator
{
    public partial class Form1 : Form
    {
        private GradientData currentGradient;
        private ColorDialog colorDialog;
        private int selectedStopIndex = -1;

        private Panel panelPreview;
        private TextBox txtCSS;
        private ListBox lstStops;
        private ComboBox cboType;
        private NumericUpDown nudAngle;
        private Button btnAddStop;
        private Button btnRemoveStop;
        private Button btnPickColor;
        private NumericUpDown nudPosition;
        private Button btnExport;
        private Label lblStatus;

        public Form1()
        {
            InitializeComponents();
            currentGradient = new GradientData();
            colorDialog = new ColorDialog();
            SetupEvents();
            UpdateUI();
        }

        private void InitializeComponents()
        {
            this.Text = "CSS Генератор Градиентов";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            panelPreview = new Panel()
            {
                Location = new Point(20, 20),
                Size = new Size(450, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            txtCSS = new TextBox()
            {
                Location = new Point(20, 340),
                Size = new Size(450, 150),
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.LightYellow,
                ScrollBars = ScrollBars.Vertical
            };

            GroupBox groupBox = new GroupBox()
            {
                Text = "Настройки градиента",
                Location = new Point(490, 20),
                Size = new Size(430, 320)
            };

            Label lblType = new Label() { Text = "Тип:", Location = new Point(15, 30), Size = new Size(40, 25) };
            cboType = new ComboBox()
            {
                Location = new Point(60, 28),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboType.Items.AddRange(new object[] { "Линейный", "Радиальный" });
            cboType.SelectedIndex = 0;

            Label lblAngle = new Label() { Text = "Угол:", Location = new Point(200, 30), Size = new Size(40, 25) };
            nudAngle = new NumericUpDown()
            {
                Location = new Point(240, 28),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 360,
                Value = 135
            };

            Label lblStops = new Label() { Text = "Цветовые остановки:", Location = new Point(15, 70), Size = new Size(130, 25) };
            lstStops = new ListBox()
            {
                Location = new Point(15, 95),
                Size = new Size(250, 120)
            };

            btnAddStop = new Button() { Text = "Добавить", Location = new Point(280, 95), Size = new Size(130, 30), BackColor = Color.LightGreen };
            btnRemoveStop = new Button() { Text = "Удалить", Location = new Point(280, 130), Size = new Size(130, 30), BackColor = Color.LightCoral };

            Label lblColor = new Label() { Text = "Цвет:", Location = new Point(15, 230), Size = new Size(40, 25) };
            btnPickColor = new Button() { Text = "Выбрать цвет", Location = new Point(60, 228), Size = new Size(100, 25) };

            Label lblPos = new Label() { Text = "Позиция %:", Location = new Point(170, 230), Size = new Size(70, 25) };
            nudPosition = new NumericUpDown()
            {
                Location = new Point(240, 228),
                Size = new Size(80, 25),
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 1,
                Increment = 5
            };

            btnExport = new Button()
            {
                Text = "Экспорт в PNG",
                Location = new Point(490, 360),
                Size = new Size(200, 40),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            lblStatus = new Label()
            {
                Text = "Готов",
                Location = new Point(20, 510),
                Size = new Size(900, 25),
                ForeColor = Color.DarkGreen,
                Font = new Font("Arial", 9)
            };

            groupBox.Controls.AddRange(new Control[] {
                lblType, cboType, lblAngle, nudAngle,
                lblStops, lstStops, btnAddStop, btnRemoveStop,
                lblColor, btnPickColor, lblPos, nudPosition
            });

            this.Controls.AddRange(new Control[] {
                panelPreview, txtCSS, groupBox, btnExport, lblStatus
            });
        }

        private void SetupEvents()
        {
            cboType.SelectedIndexChanged += (s, e) => {
                currentGradient.Type = cboType.SelectedIndex == 0 ? GradientType.Linear : GradientType.Radial;
                UpdateUI();
                SetStatus($"Тип изменен на {cboType.Text}");
            };

            nudAngle.ValueChanged += (s, e) => {
                currentGradient.Angle = (double)nudAngle.Value;
                UpdateUI();
                SetStatus($"Угол изменен на {nudAngle.Value}°");
            };

            lstStops.SelectedIndexChanged += (s, e) => {
                if (lstStops.SelectedIndex >= 0 && lstStops.SelectedIndex < currentGradient.Stops.Count)
                {
                    selectedStopIndex = lstStops.SelectedIndex;
                    nudPosition.Value = (decimal)currentGradient.Stops[selectedStopIndex].Position;
                    SetStatus($"Выбрана остановка {selectedStopIndex + 1}");
                }
            };

            btnAddStop.Click += (s, e) => AddStop();
            btnRemoveStop.Click += (s, e) => RemoveStop();
            btnPickColor.Click += (s, e) => ChangeStopColor();
            nudPosition.ValueChanged += (s, e) => ChangeStopPosition();
            btnExport.Click += (s, e) => ExportToPNG();
            panelPreview.Paint += (s, e) => DrawGradient(e.Graphics);
        }

        private void AddStop()
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                double newPos = 50.0;
                if (currentGradient.Stops.Count > 0)
                {
                    var positions = currentGradient.Stops.Select(s => s.Position).OrderBy(p => p).ToList();
                    if (positions.Count >= 2)
                        newPos = (positions[0] + positions[positions.Count - 1]) / 2;
                }

                currentGradient.AddStop(colorDialog.Color, newPos);
                UpdateUI();
                SetStatus($"Добавлена остановка: цвет {colorDialog.Color.Name}, позиция {newPos}%");
            }
        }

        private void RemoveStop()
        {
            if (lstStops.SelectedIndex >= 0)
            {
                if (currentGradient.RemoveStop(lstStops.SelectedIndex))
                {
                    UpdateUI();
                    SetStatus("Остановка удалена");
                }
                else
                {
                    SetStatus("Нельзя удалить: нужно минимум 2 остановки", true);
                }
            }
            else
            {
                SetStatus("Сначала выберите остановку для удаления", true);
            }
        }

        private void ChangeStopColor()
        {
            if (selectedStopIndex >= 0 && selectedStopIndex < currentGradient.Stops.Count)
            {
                colorDialog.Color = currentGradient.Stops[selectedStopIndex].Color;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentGradient.Stops[selectedStopIndex].Color = colorDialog.Color;
                    UpdateUI();
                    SetStatus($"Цвет остановки изменен на {colorDialog.Color.Name}");
                }
            }
            else
            {
                SetStatus("Сначала выберите остановку из списка", true);
            }
        }

        private void ChangeStopPosition()
        {
            if (selectedStopIndex >= 0 && selectedStopIndex < currentGradient.Stops.Count)
            {
                double newPos = (double)nudPosition.Value;
                currentGradient.Stops[selectedStopIndex].Position = newPos;
                currentGradient.SortStops();
                UpdateUI();

                for (int i = 0; i < currentGradient.Stops.Count; i++)
                {
                    if (Math.Abs(currentGradient.Stops[i].Position - newPos) < 0.1)
                    {
                        lstStops.SelectedIndex = i;
                        selectedStopIndex = i;
                        break;
                    }
                }
                SetStatus($"Позиция изменена на {newPos}%");
            }
        }

        private void ExportToPNG()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG файлы|*.png";
            sfd.Title = "Сохранить градиент как изображение";
            sfd.FileName = "gradient.png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int width = 800;
                    int height = 400;
                    Bitmap bmp = new Bitmap(width, height);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                        DrawGradientToGraphics(g, new Rectangle(0, 0, width, height));
                        string cssText = currentGradient.GenerateCSS();
                        g.DrawString(cssText, new Font("Consolas", 10), Brushes.Black, 10, 10);
                    }

                    bmp.Save(sfd.FileName, ImageFormat.Png);
                    bmp.Dispose();
                    SetStatus($"Экспорт успешен: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    SetStatus($"Ошибка экспорта: {ex.Message}", true);
                }
            }
        }

        private void DrawGradient(Graphics g)
        {
            DrawGradientToGraphics(g, panelPreview.ClientRectangle);
        }

        private void DrawGradientToGraphics(Graphics g, Rectangle rect)
        {
            if (currentGradient.Stops.Count < 2)
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 12))
                {
                    g.DrawString("Недостаточно цветовых остановок", font, Brushes.Red, 50, rect.Height / 2 - 20);
                    g.DrawString("Нажмите 'Добавить' для создания градиента", font, Brushes.Black, 50, rect.Height / 2 + 10);
                }
                return;
            }

            var sortedStops = currentGradient.Stops.OrderBy(s => s.Position).ToList();

            if (currentGradient.Type == GradientType.Linear)
            {
                double angleRad = currentGradient.Angle * Math.PI / 180.0;
                PointF center = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                PointF start = new PointF(
                    center.X + (float)(Math.Cos(angleRad + Math.PI) * rect.Width / 2),
                    center.Y + (float)(Math.Sin(angleRad + Math.PI) * rect.Height / 2)
                );
                PointF end = new PointF(
                    center.X + (float)(Math.Cos(angleRad) * rect.Width / 2),
                    center.Y + (float)(Math.Sin(angleRad) * rect.Height / 2)
                );

                using (LinearGradientBrush brush = new LinearGradientBrush(start, end, Color.Black, Color.Black))
                {
                    ColorBlend blend = new ColorBlend();
                    blend.Positions = sortedStops.Select(s => (float)(s.Position / 100.0)).ToArray();
                    blend.Colors = sortedStops.Select(s => s.Color).ToArray();
                    brush.InterpolationColors = blend;
                    g.FillRectangle(brush, rect);
                }
            }
            else
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(rect);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterPoint = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                        brush.SurroundColors = new Color[] { sortedStops.Last().Color };
                        brush.CenterColor = sortedStops.First().Color;
                        g.FillRectangle(brush, rect);
                    }
                }
            }

            using (Pen pen = new Pen(Color.Gray, 2))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }

        private void UpdateUI()
        {
            lstStops.Items.Clear();
            foreach (var stop in currentGradient.Stops.OrderBy(s => s.Position))
            {
                lstStops.Items.Add(stop.ToString());
            }

            txtCSS.Text = currentGradient.GenerateCSS();
            cboType.SelectedIndex = currentGradient.Type == GradientType.Linear ? 0 : 1;
            nudAngle.Value = (decimal)currentGradient.Angle;
            nudAngle.Enabled = currentGradient.Type == GradientType.Linear;
            panelPreview.Invalidate();
        }

        private void SetStatus(string message, bool isError = false)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isError ? Color.Red : Color.DarkGreen;

            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) => {
                lblStatus.Text = "Готов";
                lblStatus.ForeColor = Color.DarkGreen;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }

    public enum GradientType { Linear, Radial }

    public class ColorStop
    {
        public Color Color { get; set; }
        public double Position { get; set; }

        public ColorStop(Color color, double position)
        {
            Color = color;
            Position = Math.Max(0, Math.Min(100, position));
        }

        public override string ToString()
        {
            return $"{Color.Name} ({Color.R},{Color.G},{Color.B}) - {Position:F1}%";
        }
    }

    public class GradientData
    {
        public GradientType Type { get; set; }
        public double Angle { get; set; }
        public List<ColorStop> Stops { get; private set; }

        public GradientData()
        {
            Type = GradientType.Linear;
            Angle = 135;
            Stops = new List<ColorStop>();
            Stops.Add(new ColorStop(Color.Red, 0));
            Stops.Add(new ColorStop(Color.Blue, 100));
        }

        public void AddStop(Color color, double position)
        {
            Stops.Add(new ColorStop(color, position));
            SortStops();
        }

        public bool RemoveStop(int index)
        {
            if (Stops.Count > 2 && index >= 0 && index < Stops.Count)
            {
                Stops.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void SortStops()
        {
            Stops = Stops.OrderBy(s => s.Position).ToList();
        }

        public string GenerateCSS()
        {
            if (Stops.Count < 2)
                return "background: transparent; /* добавьте минимум 2 цвета */";

            var sortedStops = Stops.OrderBy(s => s.Position).ToList();
            string stopsCSS = string.Join(", ", sortedStops.Select(s =>
                $"rgba({s.Color.R},{s.Color.G},{s.Color.B},{s.Color.A / 255.0:F2}) {s.Position:F1}%"));

            if (Type == GradientType.Linear)
                return $"background: linear-gradient({Angle:F0}deg, {stopsCSS});";
            else
                return $"background: radial-gradient(circle, {stopsCSS});";
        }
    }
}