using InstagrammPasper.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using static InstagrammPasper.Classes.AppMessages;

namespace InstagrammPasper.Classes
{
    public class ConvertToExcelFunctionality
    {
        public void ConvertToExcel(TextBox resultTextBox)
        {
            ConstantPaths cp = new ConstantPaths();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var dataListToConvert = GetDataFromJson(resultTextBox);

            if (dataListToConvert == null)
            {
                SetMessage("List is empty! Please, press to Show Data button first!", false, resultTextBox);
                return;
            }

            SetMessage("Create the paths ...", false, resultTextBox);

            // Check, if directory not exists, create directory
            if (!Directory.Exists(cp.PathToExcelFolder))
                Directory.CreateDirectory(cp.PathToExcelFolder);

            var fullPath = new FileInfo(cp.GetFullPathToResult("Result.xlsx"));

            if (fullPath.Exists)
            {
                fullPath.Delete();
            }

            using (ExcelPackage excelPackage = new ExcelPackage(fullPath))
            {
                // Get handle to the existing worksheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Follows");
                var namedStyle = excelPackage.Workbook.Styles.CreateNamedStyle("HyperLink");   //This one is language dependent
                namedStyle.Style.Font.UnderLine = true;
                namedStyle.Style.Font.Color.SetColor(Color.Blue);

                SetMessage("Create the table object, Please, Wait ...", true, resultTextBox);

                if (worksheet != null)
                {
                    const int startRow = 5;
                    int row = startRow;

                    //Create Headers and format them
                    worksheet.Cells["A1"].Value = "Demonick Games";
                    using (ExcelRange r = worksheet.Cells["A1:D1"])
                    {
                        r.Merge = true;
                        r.Style.Font.SetFromFont(new Font("Britannic Bold", 22, FontStyle.Italic));
                        r.Style.Font.Color.SetColor(Color.White);
                        r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(23, 55, 93));
                    }

                    worksheet.Cells["A2"].Value = "Same follows report";

                    using (ExcelRange r = worksheet.Cells["A2:D2"])
                    {
                        r.Merge = true;
                        r.Style.Font.SetFromFont(new Font("Britannic Bold", 18, FontStyle.Italic));
                        r.Style.Font.Color.SetColor(Color.Black);
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    }

                    worksheet.Cells["A4"].Value = cp.ColumNames[0];
                    worksheet.Cells["B4"].Value = cp.ColumNames[1];
                    worksheet.Cells["C4"].Value = cp.ColumNames[2];
                    worksheet.Cells["D4"].Value = cp.ColumNames[3];

                    worksheet.Cells["A4:D4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["A4:D4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells["A4:D4"].Style.Font.Bold = true;

                    List<FollowData> rowData = new List<FollowData>();

                    foreach (var item in dataListToConvert)
                    {
                        rowData.AddRange(item.FollowsData);
                    }

                    rowData = rowData.OrderByDescending(x => x.SameFollowCount).ToList();

                    while ((row - 5) < rowData.Count)
                    {
                        int col = 1;

                        // our query has the columns in the right order, so simply
                        // iterate through the columns
                        for (int i = 1; i <= cp.ColumNames.Length; i++)
                        {
                            // use the email address as a hyperlink for column 1
                            if (i == 2)
                            {
                                // insert the email address as a hyperlink for the name
                                string hyperlink = rowData[row - 5].FollowPageAddress;
                                worksheet.Cells[row, i].Hyperlink = new Uri(hyperlink, UriKind.Absolute);
                            }
                            else
                            {
                                // do not bother filling cell with blank data (also useful if we have a formula in a cell)
                                worksheet.Cells[row, i].Value = GetRowData(rowData[row - 5], i);
                                //col++;
                            }
                        }
                        row++;
                    }

                    worksheet.Cells[startRow, 2, row - 1, 2].StyleName = "HyperLink";

                    //Set column width
                    worksheet.Column(1).Width = 34;
                    worksheet.Column(2).Width = 45;
                    worksheet.Column(3).Width = 6;
                    worksheet.Column(4).Width = 40;

                    // lets set the header text
                    worksheet.HeaderFooter.OddHeader.CenteredText = "Demonick Games. Twitter Following Page Report";
                    // add the page number to the footer plus the total number of pages
                    worksheet.HeaderFooter.OddFooter.RightAlignedText =
                        string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                    // add the sheet name to the footer
                    worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                    // add the file path to the footer
                    worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                    excelPackage.Save();

                    SetMessage("Save in file ...", true, resultTextBox);
                }
            }

            SetMessage($"Save data success to convert and save in Excel file!\nFile path is: {fullPath}", true, resultTextBox);
        }

        private string GetRowData(FollowData model, int counter)
        {
            switch (counter)
            {
                case 1:
                    {
                        var cleanedName = CleanInput(model.FollowName);
                        return cleanedName;
                    }
                case 2: return model.FollowPageAddress;
                case 3: return model.SameFollowCount.ToString();
                case 4:
                    {
                        string followerNames = string.Empty;
                        foreach (var item in model.SameFollowPeople)
                        {
                            followerNames += $"{item}, ";
                        }

                        return followerNames;
                    }
            }
            return null;
        }

        private List<FollowModel> GetDataFromJson(TextBox resultTextBox)
        {
            ConstantPaths cp = new ConstantPaths();

            string fileName = "resultList.json";
            string fullPath = cp.GetFullPath(fileName);

            UniversalSerializeDataClass<List<FollowModel>> deserializer = new UniversalSerializeDataClass<List<FollowModel>>();
            var result = deserializer.DeserializeData(fullPath);

            if (result != null)
            {
                SetMessage("Load data from JSON complete.", false, resultTextBox);
                return result;
            }
            else
            {
                SetMessage("Load data from JSON failed. Please, parse the data and push Show result button first.", false, resultTextBox);
                return null;
            }
        }
    }
}
