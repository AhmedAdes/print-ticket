using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace print_ticket
{
  class Program
  {
    public class PrintItem
    {
      public string ServiceNo;
      public string DeptName;
      public string ImageName;
      public string ArabicHeader;
      public string EnglishHeader;
      public string Waiting;
    }

    static void Main(string[] args)
    {
      // Console.WriteLine("Hello World!");
      PrintReceipt();
      Environment.Exit(0);
    }

    private static void PrintReceipt()
    {
      try
      {
        PrintDocument pdPrint = new PrintDocument();
        pdPrint.PrintPage += new PrintPageEventHandler(pdPrint_PrintPage);

        // string PRINTER_NAME = "EPSON TM-T88IV Receipt";

        // Change the printer to the indicated printer.
        // pdPrint.PrinterSettings.PrinterName = PRINTER_NAME;

        if (pdPrint.PrinterSettings.IsValid)
        {
          pdPrint.DocumentName = "Testing";
          // Start printing.
          pdPrint.Print();
        }
      }
      catch (System.Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private static void pdPrint_PrintPage(object sender, PrintPageEventArgs e)
    {
      e.Graphics.PageUnit = GraphicsUnit.Point;

      float x, y;
      // Instantiate font objects used in printing.
      Font HeaderFont = new Font("Microsoft Sans Serif", (float)14, FontStyle.Regular, GraphicsUnit.Point); // Substituted to FontA Font
      Font NumberFont = new Font("Microsoft Sans Serif", (float)50, FontStyle.Bold, GraphicsUnit.Point); // Substituted to FontA Font
      Font DeptFont = new Font("Microsoft Sans Serif", (float)16, FontStyle.Bold, GraphicsUnit.Point); // Substituted to FontA Font
      Font FooterFont = new Font("Microsoft Sans Serif", (float)10, FontStyle.Regular, GraphicsUnit.Point); // Substituted to FontA Font
      using (StreamReader r = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\print-data.json"))
      {
        string json = r.ReadToEnd();
        PrintItem items = JsonConvert.DeserializeObject<PrintItem>(json);

        string ServiceNo = items.ServiceNo;
        string DeptName = items.DeptName;

        // Image pbImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"\images\CMA_CGM_logo2.png"); 
        Image pbImage = Image.FromFile(items.ImageName);
        // Image pbImage = Properties.Resources.CMA_CGM_logo2;
        StringFormat format = new StringFormat(StringFormatFlags.LineLimit);
        format.Alignment = StringAlignment.Center;

        string ArabicHeader = items.ArabicHeader;
        string EnglishHeader = items.EnglishHeader; // "Welcome To CMA CGM";
        string Waiting = string.Format("waiting customers: {0}", items.Waiting);
        string footer = string.Format("الوقت: {0}       التاريخ: {1}", DateTime.Now.ToString("hh:mm:ss tt"), DateTime.Now.ToString("dd/MM/yyy"));

        // Draw the bitmap
        x = (e.MarginBounds.Width - pbImage.Width * ((float)e.MarginBounds.Width / (float)e.PageBounds.Width)) / 2;
        y = 0;
        e.Graphics.DrawImage(pbImage, x, y, pbImage.Width, pbImage.Height);

        // Print the receipt text
        y = pbImage.Height;
        Rectangle rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(ArabicHeader, HeaderFont, Brushes.Black, rect, format);

        y += HeaderFont.GetHeight(e.Graphics);
        rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(EnglishHeader, HeaderFont, Brushes.Black, rect, format);

        y += HeaderFont.GetHeight(e.Graphics);
        rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(ServiceNo, NumberFont, Brushes.Black, rect, format);

        y += NumberFont.GetHeight(e.Graphics); ;
        rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(DeptName, DeptFont, Brushes.Black, rect, format);

        y += DeptFont.GetHeight(e.Graphics);
        rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(Waiting, FooterFont, Brushes.Black, rect, format);

        y += DeptFont.GetHeight(e.Graphics);
        rect = new Rectangle(0, (int)y, (int)e.Graphics.VisibleClipBounds.Width, e.MarginBounds.Height);
        e.Graphics.DrawString(footer, FooterFont, Brushes.Black, rect, format);

        y += FooterFont.GetHeight(e.Graphics) - (float)3.5;
        e.Graphics.DrawString("               ", FooterFont, Brushes.Black, 0, y);

        // Indicate that no more data to print, and the Print Document can now send the print data to the spooler.
        e.HasMorePages = false;
      }
    }
  }
}
