using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PaintForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            loadPictureBox();

            // Make the pen selected by default
            selectedShapeButton = PenButton;
            selectedShapeButton.BackColor = Color.Red; 
        }

        Bitmap paintImage;
        Graphics paintGraphics;

        private void loadPictureBox()
        {
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            paintImage = new Bitmap(width, height);

            paintGraphics = Graphics.FromImage(paintImage);

            paintGraphics.FillRectangle(Brushes.White, 0, 0, width, height);
            
            pictureBox1.Image = paintImage;

            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
        }

        Point lastPoint;
        bool isMouseDown = false;

        void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = e.Location;
            isMouseDown = true;
        }
                
        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                if (selectedShapeButton.Text == "Pen")
                {
                    DrawLineInCanvas(e.Location);
                }
                else
                {
                    DrawShapeInWorkingImage(e.Location);
                }
            }
        }

        void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;

            if (selectedShapeButton.Text != "Pen")
            {
                // Draw again to get the shape without border
                DrawShapeInWorkingImage(e.Location);

                paintImage = new Bitmap(workingImage);

                // Re initialize graphics object as the bitmap is new
                paintGraphics = Graphics.FromImage(paintImage); 

                pictureBox1.Image = paintImage;                
            }
        }

        private void DrawLineInCanvas(Point currentPoint)
        {
            // Form pen with the color selected and the size value in tracker
            Pen pen = new Pen(colorPicker.Color, trackBar1.Value);

            paintGraphics.DrawLine(pen, lastPoint, currentPoint);

            lastPoint = currentPoint;

            pictureBox1.Refresh();
        }
        
        ColorDialog colorPicker = new ColorDialog();

        private void ColorPickerButton_Click(object sender, EventArgs e)
        {
            colorPicker.ShowDialog();
        }
        
        Bitmap workingImage;
        Graphics workingGraphics;

        private void DrawShapeInWorkingImage(Point currentPoint)
        {
            Pen pen = new Pen(colorPicker.Color, trackBar1.Value);

            workingImage = new Bitmap(paintImage);
            workingGraphics = Graphics.FromImage(workingImage);

            int startPointX = lastPoint.X < currentPoint.X ? lastPoint.X : currentPoint.X;
            int startPointY = lastPoint.Y < currentPoint.Y ? lastPoint.Y : currentPoint.Y;

            int shapeWidth = (lastPoint.X > currentPoint.X ? lastPoint.X : currentPoint.X) - startPointX;
            int shapeHeight = (lastPoint.Y > currentPoint.Y ? lastPoint.Y : currentPoint.Y) - startPointY;

            switch (selectedShapeButton.Text)
            {
                case "Rectangle":
                    // Check if it is to draw or fill shape
                    if (!FillColorCheckBox.Checked)
                    {
                        // Draw Rectangle
                        workingGraphics.DrawRectangle(pen, startPointX, startPointY, shapeWidth, shapeHeight);
                    }
                    else
                    {
                        // Fill Rectangle
                        workingGraphics.FillRectangle(pen.Brush, startPointX, startPointY, shapeWidth, shapeHeight);
                    }
                    break;

                case "Circle":
                    if (!FillColorCheckBox.Checked)
                    {
                        workingGraphics.DrawEllipse(pen, startPointX, startPointY, shapeWidth, shapeHeight);
                    }
                    else
                    {
                        workingGraphics.FillEllipse(pen.Brush, startPointX, startPointY, shapeWidth, shapeHeight);
                    }
                    break;

                case "Triangle":
                    Point point1 = new Point() { X = startPointX, Y = startPointY + shapeHeight };
                    Point point2 = new Point() { X = startPointX + (shapeWidth / 2), Y = startPointY };
                    Point point3 = new Point() { X = startPointX + shapeWidth, Y = startPointY + shapeHeight };

                    if (!FillColorCheckBox.Checked)
                    {
                        workingGraphics.DrawPolygon(pen, new Point[] { point1, point2, point3 });
                    }
                    else
                    {
                        workingGraphics.FillPolygon(pen.Brush, new Point[] { point1, point2, point3 });
                    }
                    break;

                case "Line":
                    workingGraphics.DrawLine(pen, lastPoint, currentPoint);
                    break;
            }

            // The outline should be shown only if it is not a line and the drawing is on. 
            if (isMouseDown && selectedShapeButton.Text != "Line") 
            {
                // Draw outline while drawing shapes
                Pen outLinePen = new Pen(Color.Black);
                outLinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                workingGraphics.DrawRectangle(outLinePen, startPointX, startPointY, shapeWidth, shapeHeight);
            }

            pictureBox1.Image = workingImage;
        }
        

        Button selectedShapeButton;

        private void PenButtond_Click(object sender, EventArgs e)
        {
            selectedShapeButton.BackColor = SystemColors.Control;

            Button clickedButton = sender as Button;
            clickedButton.BackColor = Color.Red;

            selectedShapeButton = clickedButton;
        }
    }
}
