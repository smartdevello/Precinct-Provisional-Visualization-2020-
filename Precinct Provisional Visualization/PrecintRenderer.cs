using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Precinct_Provisional_Visualization
{
    public class PrecintRenderer
    {

        private int width = 0, height = 0;
        private double totHeight = 1000;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private List<PrecintData> data = null;
        private List<CodeDescription> ycode = null;
        private List<CodeDescription> ncode = null;

        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        public PrecintRenderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public int getDataCount()
        {
            if (this.data == null) return 0;
            else return this.data.Count;
        }
        public List<PrecintData> getData()
        {
            return this.data;
        }
        public void setChatData(List<PrecintData> data, List<CodeDescription> ycode, List<CodeDescription> ncode)
        {
            this.data = data;
            this.ycode = ycode;
            this.ncode = ncode;
        }
        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)((a.X + 20) * px);
            res.Y = (int)((1000 - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)((p.X + 20) * px);
            res.Y = (int)((1000 - p.Y) * px);
            return res;
        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }

        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        private void fillPolygon(Brush brush, PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = convertCoord(points[i]);
            }
            gfx.FillPolygon(brush, points);
        }
        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawString(Font font, Color brushColor, string content, Point o)
        {
            o = convertCoord(o);
            SolidBrush drawBrush = new SolidBrush(brushColor);
            gfx.DrawString(content, font, drawBrush, o.X, o.Y);
        }
        public void drawString(Point o,  string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }
        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }
        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle, string content = "")
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);

            o.X += size.Width / 2;
            o.Y -= size.Height / 2;
            float radius = size.Width * 0.3f;
            o.X += (int)(radius * Math.Cos(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            o.Y -= (int)(radius * Math.Sin(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            content += "\n" + string.Format("{0:F}%", sweepAngle * 100.0f / 360.0f);
            drawString(o, content, 9);
        }
        public void drawFilledCircle(Brush brush, Point o, Size size)
        {
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);
            
            Rectangle rect = new Rectangle(convertCoord(o), size);
            
            gfx.FillEllipse(brush, rect);
        }

        public void draw(string currentPrecinctstring)
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }
            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }
            drawImg(logoImg, new Point(20, 60), new Size(150, 50));



            if (data == null) return;

            PrecintData currentPrecinct =  data.Find(item => item.precinct == currentPrecinctstring);

            if (currentPrecinct == null) return;
            ////////////////////////////////////////First Section//////////////////////////////////////////////////
            ///
            Font textFont = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point);
            Font titleFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Point);


            
            drawString(titleFont, Color.Black, "Precinct Provisional Visualzation", new Point(50, 970));
            drawString(titleFont, Color.Black, currentPrecinct.precinct + " " + currentPrecinct.name, new Point(1300, 970));


            int maxVal = 0, len = 0, totNo = 0, totYes = 0;
            double percent = 0;
            maxVal = Math.Max(currentPrecinct.reg_v, currentPrecinct.actual);
            totNo = currentPrecinct.B10 + currentPrecinct.B11 + currentPrecinct.B12 + currentPrecinct.B13 + currentPrecinct.B14 + currentPrecinct.B17;
            totYes = currentPrecinct.A1 + currentPrecinct.A2 + currentPrecinct.A3 + currentPrecinct.A4 + currentPrecinct.A5 + currentPrecinct.A6 + currentPrecinct.A7 + currentPrecinct.A8;

            drawString(textFont, Color.Black, "REGISTERED", new Point(0, 850));
            drawString(textFont, Color.Black, currentPrecinct.reg_v.ToString(), new Point(750, 850));
            drawString(textFont, Color.Black, "VOTED", new Point(0, 790));
            drawString(textFont, Color.Black, "REJECTED", new Point(0, 700));
            drawString(textFont, Color.Black, currentPrecinct.actual.ToString(), new Point(750, 790));

            len = (int)(500 * currentPrecinct.reg_v / (double)maxVal);
            fillRectangle(Color.DarkSlateBlue, new Rectangle(200, 860, len, 60));
            len = (int)(500 * currentPrecinct.actual / (double)maxVal);
            fillRectangle(Color.LimeGreen, new Rectangle(200, 800, len, 60));


            fillRectangle(Color.LimeGreen, new Rectangle(230, 710, 400, 60));

            Brush redBrush = new SolidBrush(Color.Red);
            Brush greenBrush = new SolidBrush(Color.LimeGreen);
            Brush blackBrush = new SolidBrush(Color.Black);
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush aquaBrush = new SolidBrush(Color.Aqua);

            drawFilledCircle(greenBrush, new Point(200, 710), new Size(60, 60));
            drawFilledCircle(greenBrush, new Point(600, 710), new Size(60, 60));




            percent = Math.Round( currentPrecinct.actual * 100 / (double)currentPrecinct.reg_v , 2);
            drawCenteredString_withBorder(percent.ToString() + "%", new Rectangle(850, 860, 120, 120), blackBrush, textFont, Color.LimeGreen);

            percent = Math.Round(totNo * 100 / (double)currentPrecinct.actual, 2);
            drawCenteredString(percent.ToString() + "%", new Rectangle(850, 710, 120, 60), blackBrush, textFont);
            len = (int)(percent * 400 / 100);
            drawFilledCircle(redBrush, new Point(200 + len, 710), new Size(60, 60));

            ///////////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////Draw D&R Section///////////////////////
            string rRating = "";
            //fillRectangle(Color.Black, new Rectangle(1000, 850, 60, 60));
            //drawCenteredString("08", new Rectangle(1000, 850, 60, 60), whiteBrush, titleFont);
            //fillRectangle(Color.Black, new Rectangle(1000, 780, 60, 60));
            //if (currentPrecinct.r08 == "DEM")
            //{
            //    rRating = "D";
            //    drawCenteredString(rRating, new Rectangle(1000, 780, 60, 60), aquaBrush, titleFont);
            //}
            //else if (currentPrecinct.r08 == "REP")
            //{
            //    rRating = "R";
            //    drawCenteredString(rRating, new Rectangle(1000, 780, 60, 60), redBrush, titleFont);
            //}
            //else
            //{
            //    rRating = "";
            //}


            //fillRectangle(Color.Black, new Rectangle(1070, 850, 60, 60));
            //drawCenteredString("12", new Rectangle(1070, 850, 60, 60), whiteBrush, titleFont);
            //fillRectangle(Color.Black, new Rectangle(1070, 780, 60, 60));
            //if (currentPrecinct.r12 == "DEM")
            //{
            //    rRating = "D";
            //    drawCenteredString(rRating, new Rectangle(1070, 780, 60, 60), aquaBrush, titleFont);
            //}
            //else if (currentPrecinct.r12 == "REP")
            //{
            //    rRating = "R";
            //    drawCenteredString(rRating, new Rectangle(1070, 780, 60, 60), redBrush, titleFont);
            //}
            //else
            //{
            //    rRating = "";
            //}


            //fillRectangle(Color.Black, new Rectangle(1140, 850, 60, 60));
            //drawCenteredString("16", new Rectangle(1140, 850, 60, 60), whiteBrush, titleFont);
            //fillRectangle(Color.Black, new Rectangle(1140, 780, 60, 60));

            //if (currentPrecinct.r16 == "DEM")
            //{
            //    rRating = "D";
            //    drawCenteredString(rRating, new Rectangle(1140, 780, 60, 60), aquaBrush, titleFont);
            //}
            //else if (currentPrecinct.r16 == "REP")
            //{
            //    rRating = "R";
            //    drawCenteredString(rRating, new Rectangle(1140, 780, 60, 60), redBrush, titleFont);
            //}
            //else
            //{
            //    rRating = "";
            //}


            fillRectangle(Color.Black, new Rectangle(1110, 850, 60, 60));
            drawCenteredString("20", new Rectangle(1110, 850, 60, 60), whiteBrush, titleFont);
            fillRectangle(Color.Black, new Rectangle(1110, 780, 60, 60));

            if (currentPrecinct.r20 == "DEM")
            {
                rRating = "D";
                drawCenteredString(rRating, new Rectangle(1110, 780, 60, 60), aquaBrush, titleFont);
            }
            else if (currentPrecinct.r20 == "REP")
            {
                rRating = "R";
                drawCenteredString(rRating, new Rectangle(1110, 780, 60, 60), redBrush, titleFont);
            }
            else
            {
                rRating = "";
            }

            ////////////////////////////////////////Pie Section//////////////////////////////////////////////////
            ///

            float startAngle = 270.0f, sweepAngle = 0;
            sweepAngle = 360 * totYes / (float)(totYes + totNo);
            drawPie(Color.LightGray, new Point(1300, 900), new Size(300, 300), startAngle, sweepAngle, "Accepted");
            startAngle = startAngle + sweepAngle;
            sweepAngle = 360 * totNo / (float)(totYes + totNo);
            drawPie(Color.DarkGray, new Point(1300, 900), new Size(300, 300), startAngle, sweepAngle, "Rejected");
            ///////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////Text Section//////////////////////////////////////////////////
            //LightGray Background
            fillRectangle(Color.LightGray, new Rectangle(800, 570, 850, 600));

            //Draw Border Lines
            fillRectangle(Color.Black, new Rectangle(100, 570, 1400, 10));
            fillRectangle(Color.Black, new Rectangle(100, 590, 50, 50));
            fillRectangle(Color.Black, new Rectangle(1450, 590, 50, 50));

            //Draw Half Line
            drawLine(new Point(800, 570), new Point(800, 0), Color.Black, 3);

            ///////////////////////////////////////////////////////////////////////////////////////////////////


            /////////////////////////////////////  Draw Content /////////////////////////////////////////////////////
            ///
            drawCenteredString("ACCEPTED", new Rectangle(100, 570, 700, 100), blackBrush, textFont);
            drawCenteredString("REJECTED", new Rectangle(800, 570, 700, 100), blackBrush, textFont);

            textFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            titleFont = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point);

            List<CodeDescription> currentYesCode = ycode.FindAll(item => item.precinct == currentPrecinct.precinct);

            int yCood = 500;

            foreach( var code in currentYesCode)
            {
                drawString(textFont, Color.Black, code.description, new Point(100, yCood));
                percent = Math.Round(code.count * 100 / (double)totYes, 2);
                drawString(titleFont, Color.Black, percent.ToString() + "%", new Point(600, yCood));
                int lineCnt = code.description.Count(c => c == '\n') + 1;
                yCood = yCood - 33 * lineCnt;
            }
            //drawString(textFont, Color.Black, "New Resident Ballot Verified\nand Address Updated", new Point(100, 450));
            //drawString(titleFont, Color.Black, "42.30%", new Point(600, 450));

            //drawString(textFont, Color.Black, "Early Ballot Requested\nNot Returned", new Point(100, 370));
            //drawString(titleFont, Color.Black, "15.38%", new Point(600, 370));

            //drawString(textFont, Color.Black, "Office Error Occured\nVerified and Corrected", new Point(100, 290));
            //drawString(titleFont, Color.Black, "03.84%", new Point(600, 290));

            //drawString(textFont, Color.Black, "Registration Received Too Late\nTo Be Included in Roster", new Point(100, 210));
            //drawString(titleFont, Color.Black, "11.53%", new Point(600, 210));

            //drawString(textFont, Color.Black, "ID Address Dosen't Match\nSignature Roster", new Point(100, 130));
            //drawString(titleFont, Color.Black, "26.92%", new Point(600, 130));



            List<CodeDescription> currentNoCode = ncode.FindAll(item => item.precinct == currentPrecinct.precinct);

            yCood = 500;
            foreach(var code in currentNoCode)
            {
                drawString(textFont, Color.Black, code.description, new Point(850, yCood));
                percent = Math.Round(code.count * 100 / (double)totNo, 2);
                drawString(titleFont, Color.Black, percent.ToString() + "%", new Point(1400, yCood));
                int lineCnt = code.description.Count(c => c == '\n') + 1;
                yCood = yCood - 33 * lineCnt;
            }


            textFont = new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Point);
            drawString(textFont, Color.Black, "© 2021 Tesla Laboratories, llc & JHP", new Point(1200, 50));
            drawString(textFont, Color.Black, totYes.ToString(), new Point(750, 50));
            drawString(textFont, Color.Black, totNo.ToString(), new Point(815, 50));
            textFont.Dispose();
            titleFont.Dispose();
            redBrush.Dispose();
            greenBrush.Dispose();
            blackBrush.Dispose();
            whiteBrush.Dispose();
            aquaBrush.Dispose();
        }
    }
}
