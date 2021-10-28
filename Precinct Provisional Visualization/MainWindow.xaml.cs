using AutoMapper;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Precinct_Provisional_Visualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PrecintManager precintManager = null;
        private PrecintRenderer precintRenderer = null;
        private List<string> errPrecinct = null;
        string exportFolderPath = "";
        public MainWindow()
        {
            precintManager = null;
            precintRenderer = null;            
            InitializeComponent();
            pbStatus.Visibility = Visibility.Hidden;
            pbStatus.Value = 0;
            errPrecinct = new List<string>();
        }
        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Render("");
        }
        private void myCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Render();
        }
        private void precinctChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            Render(currentPrecinct.Text);
        }
        void Render(string preceint)
        {
            if (precintRenderer == null)
            {
                precintRenderer = new PrecintRenderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            }
            
            if (precintRenderer.getDataCount() > 0)
            {
                precintRenderer.setRenderSize((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
                precintRenderer.draw(preceint);
                myImage.Source = BmpImageFromBmp(precintRenderer.getBmp());
            }
        }
        private void btnExportAllChart_Click(object sender, RoutedEventArgs e)
        {

            if (precintRenderer == null)
            {
                precintRenderer = new PrecintRenderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            }

            if (precintRenderer.getDataCount() > 0)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {
                        exportFolderPath = dialog.SelectedPath;
                        pbStatus.Visibility = Visibility.Visible;
                        pbStatus.Minimum = 0;
                        pbStatus.Maximum = precintRenderer.getDataCount();
                        pbStatus.Value = 0;
                        errPrecinct.Clear();

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoExport;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerAsync();
                        worker.RunWorkerCompleted += worker_CompletedWork;
                    }
                }
                
            }

        }
        void worker_CompletedWork(object sender, RunWorkerCompletedEventArgs e)
        {
            pbStatus.Visibility = Visibility.Hidden;
            string msg = "Exporting has been done\n";
            if (errPrecinct.Count > 0)
            {                
                foreach (var i in errPrecinct)
                {
                    msg += string.Format("Precinct {0} has a problem\n", i);
                }
            }
            
            MessageBox.Show(msg);
        }

        void worker_DoExport(object sender, DoWorkEventArgs e)
        {
            precintRenderer.setRenderSize((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            List<PrecintData> data = precintRenderer.getData();

            int index = 0;
            foreach (var item in data)
            {
                try
                {
                    precintRenderer.draw(item.precinct);
                    string filename = exportFolderPath + "/" + item.precinct + " " + item.name + ".png";
                    SaveBitmapImagetoFile(BmpImageFromBmp(precintRenderer.getBmp()), filename);
                    index++;
                    (sender as BackgroundWorker).ReportProgress(index);
                } catch (Exception)
                {
                    errPrecinct.Add(item.precinct);
                }

            }

        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void SaveBitmapImagetoFile( BitmapImage image, string filePath)
        {
            //PngBitmapEncoder encoder1 = new PngBitmapEncoder();
            //encoder1.Frames.Add(BitmapFrame.Create(image));

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            try
            {
                using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            } catch(Exception ex)
            {

            }


        }
        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            //openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            List<PrecintData> data = new List<PrecintData>();
            List<CodeDescription> ycode = new List<CodeDescription>();
            List<CodeDescription> ncode = new List<CodeDescription>();

            string errMsg = "";
            if (openFileDialog.ShowDialog() == true)
            {

                    IWorkbook workbook = null;
                    string fileName = openFileDialog.FileName;

                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (fileName.IndexOf(".xlsx") > 0)
                            workbook = new XSSFWorkbook(fs);
                        else if (fileName.IndexOf(".xls") > 0)
                            workbook = new HSSFWorkbook(fs);

                    }

                    ISheet sheet = workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;

                        for (int i = 1; i < rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }

                            if (curRow.Cells.Count == 20)
                            {
                                try
                                {
                                    data.Add(new PrecintData()
                                    {
                                        precinct = curRow.GetCell(0).StringCellValue.Trim(),
                                        name = curRow.GetCell(1).StringCellValue,
                                        reg_v = Convert.ToInt32(curRow.GetCell(2).NumericCellValue),
                                        actual = Convert.ToInt32(curRow.GetCell(3).NumericCellValue),
                                        prov = Convert.ToInt32(curRow.GetCell(4).NumericCellValue),
                                        A1 = Convert.ToInt32(curRow.GetCell(5).NumericCellValue),
                                        A2 = Convert.ToInt32(curRow.GetCell(6).NumericCellValue),
                                        A3 = Convert.ToInt32(curRow.GetCell(7).NumericCellValue),
                                        A4 = Convert.ToInt32(curRow.GetCell(8).NumericCellValue),
                                        A5 = Convert.ToInt32(curRow.GetCell(9).NumericCellValue),
                                        A6 = Convert.ToInt32(curRow.GetCell(10).NumericCellValue),
                                        A7 = Convert.ToInt32(curRow.GetCell(11).NumericCellValue),
                                        A8 = Convert.ToInt32(curRow.GetCell(12).NumericCellValue),
                                        B10 = Convert.ToInt32(curRow.GetCell(13).NumericCellValue),
                                        B11 = Convert.ToInt32(curRow.GetCell(14).NumericCellValue),
                                        B12 = Convert.ToInt32(curRow.GetCell(15).NumericCellValue),
                                        B13 = Convert.ToInt32(curRow.GetCell(16).NumericCellValue),
                                        B14 = Convert.ToInt32(curRow.GetCell(17).NumericCellValue),
                                        B17 = Convert.ToInt32(curRow.GetCell(18).NumericCellValue),
                                        r20 = curRow.GetCell(19).StringCellValue,
                                    });
                                }
                                catch (Exception)
                                {

                                }

                            }
                            else
                            {
                                errMsg += string.Format("The Row {0} has a problem\n", i + 1);
                            }

                        }
                    }

                    sheet = workbook.GetSheetAt(1);
                    if (sheet != null)
                    {

                        int rowCount = sheet.LastRowNum;
                        string last_precint = "";
                        string description = "";

                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }

                            if (curRow.Cells.Count == 4)
                            {
                                try
                                {
                                    last_precint = curRow.GetCell(0).StringCellValue;
                                    if (last_precint.Contains("#")) last_precint = last_precint.Substring(1);

                                    description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                                    ycode.Add(new CodeDescription()
                                    {
                                        precinct = last_precint,
                                        reason_code = curRow.GetCell(1).StringCellValue,
                                        description = description,
                                        count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                                    });
                                }
                                catch (Exception)
                                {

                                }

                            }

                        }

                    }

                    sheet = workbook.GetSheetAt(2);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;
                        string last_precint = "";
                        string description = "";

                            for (int i = 1; i <= rowCount; i++)
                            {
                                IRow curRow = sheet.GetRow(i);
                                if (curRow == null)
                                {
                                    rowCount = i - 1;
                                    break;
                                }

                                if (curRow.Cells.Count == 4)
                                {
                                    try
                                    {
                                        last_precint = curRow.GetCell(0).StringCellValue;
                                        if (last_precint.Contains("#")) last_precint = last_precint.Substring(1);

                                        description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                                        ncode.Add(new CodeDescription()
                                        {
                                            precinct = last_precint,
                                            reason_code = curRow.GetCell(1).StringCellValue,
                                            description = description,
                                            count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                                        });
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }

                            }


                    }




                if (!string.IsNullOrEmpty(errMsg))
                {
                    //MessageBox.Show(errMsg, "Error");
                }

                if (data.Count > 0)
                {
                    //hemoRenderer.setChatData(sorted);
                    precintRenderer.setChatData(data, ycode, ncode);
                    if ( string.IsNullOrEmpty( currentPrecinct.Text))
                    {
                        currentPrecinct.Text = data[0].precinct;
                    }
                    //Render(currentPrecinct.Text);

                }
                else
                {
                    //string msg = hemoManager.getLastException();

                }

            }
        }

        private string AddLineBreaktoString(string desc)
        {
            int newLineIndex = 1;
            for (int i = 0; i< desc.Length; i++)
            {
                if (desc[i] == ' ' && newLineIndex >= 28)
                {
                    int j = i - 1;
                    while (desc[j] != ' ') j--;
                    StringBuilder sb = new StringBuilder(desc);
                    sb[j] = '\n';
                    desc = sb.ToString();
                    newLineIndex = 1; i = j;
                }
                else newLineIndex++;
            }
            return desc;
        }

        private void btnExportCurrentChart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.png)|*.png";
            //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveControlImage(PrecinctChart, saveFileDialog.FileName);
            }
        }
        private void SaveControlImage(FrameworkElement control, string filename)
        {
            RenderTargetBitmap rtb = (RenderTargetBitmap)CreateBitmapFromControl(control);
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }

        public BitmapSource CreateBitmapFromControl(FrameworkElement element)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(element);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }
        
        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
