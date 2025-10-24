using ClosedXML.Excel;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using ShairiStore.Enums;

namespace ShairiStore.Common;

public static class StableExportServiceHelper
{
    private const string DateFormat = "dd-MMM-yyyy hh:mm tt";
    private const string WatermarkText = "CS & Sons";

    /// <summary>
    /// Generic export entry point. 'data' can be a collection (e.g. List<Order>) or a single object.
    /// </summary>
    public static byte[] GenerateExportFile(object data, ExportTypes exportTypes, string title, IEnumerable<string>? columns = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"{title} Export");

        // Add watermark (header + faint repeated on-sheet banners)
        ApplyWatermark(worksheet, WatermarkText);
        ApplyOnSheetFaintBanner(worksheet, WatermarkText);

        int currentRow = 1;

        if (data is IEnumerable enumerable && !(data is string))
        {
            // If top-level is a collection of orders (or other entities), iterate each item and write sections
            foreach (var item in enumerable.Cast<object>())
            {
                var targetId = GetTargetId(item);
                bool titleRequired = true;
                if (exportTypes == ExportTypes.OrderWithDetails)
                {
                    // 1) Order main section written as single-row table
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order - ({targetId})", currentRow, titleRequired ,columns);

                    // 2) OrderDetails (collection)
                    WriteCollectionIfExists(item, "OrderDetails", worksheet, ref currentRow, $"Order Details - ({targetId})", titleRequired , columns);

                    // 3) OrderPayments (collection)
                    WriteCollectionIfExists(item, "OrderPayments", worksheet, ref currentRow, $"Order Payments - ({targetId})", titleRequired, columns);

                    // 4) OrderInvoice (single object)
                    WriteSingleIfExists(item, "OrderInvoice", worksheet, ref currentRow, $"Order Invoice - ({targetId})", titleRequired, columns);

                    // 5) Related info sections: Seller, Broker, Warehouse
                    WriteSingleIfExists(item, "Seller", worksheet, ref currentRow, "Seller Info", titleRequired, null);
                    WriteSingleIfExists(item, "Broker", worksheet, ref currentRow, "Broker Info", titleRequired, null);
                    WriteSingleIfExists(item, "Warehouse", worksheet, ref currentRow, "Warehouse Info", titleRequired, null);

                    // 6) User section - show only required user fields (if any navigation property representing a user exists)
                    WriteSingleIfExists(item, "User" ,worksheet, ref currentRow, "User Info", titleRequired, new[] { "FullName", "UserName", "Email", "PhoneNumber" });
                }
                else if(exportTypes == ExportTypes.OutGoingOrders)
                {
                    // 1) Order main section written as single-row table
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order - ({targetId})", currentRow, titleRequired, columns);

                    // 2) OrderDetails (collection)
                    WriteCollectionIfExists(item, "OrderDetails", worksheet, ref currentRow, $"Order Details - ({targetId})", titleRequired, columns);

                    // 3) OrderPayments (collection)
                    WriteSingleIfExists(item, "OutgoingOrderPayments", worksheet, ref currentRow, $"Order Payments - ({targetId})", titleRequired, columns);

                    // 4) OrderInvoice (single object)
                    WriteSingleIfExists(item, "OrderInvoice", worksheet, ref currentRow, $"Order Invoice - ({targetId})", titleRequired, columns);

                    // 5) Related info sections: Seller, Broker, Warehouse
                    WriteSingleIfExists(item, "Seller", worksheet, ref currentRow, "Seller Info", titleRequired, null);
                    WriteSingleIfExists(item, "Broker", worksheet, ref currentRow, "Broker Info", titleRequired, null);
                    WriteSingleIfExists(item, "Warehouse", worksheet, ref currentRow, "Warehouse Info", titleRequired, null);

                    // 6) User section - show only required user fields (if any navigation property representing a user exists)
                    WriteSingleIfExists(item, "User" ,worksheet, ref currentRow, "User Info", titleRequired, new[] { "FullName", "UserName", "Email", "PhoneNumber" });
                }
                else if(exportTypes == ExportTypes.Orders)
                {
                    // 1) Order main section written as single-row table
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Orders", currentRow, currentRow == 1 ? titleRequired : !titleRequired ,columns);
                    currentRow--;
                }
                else if(exportTypes == ExportTypes.Invoices)
                {
                    // 4) OrderInvoice (single object)
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order Invoices", currentRow, currentRow == 1 ? titleRequired : !titleRequired, columns);
                    currentRow--;
                }
                else if (exportTypes == ExportTypes.Payments)
                {
                    // 3) OrderPayments (collection)
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order Payments", currentRow, currentRow == 1 ? titleRequired : !titleRequired, columns);
                    currentRow--;
                }
                else if(exportTypes == ExportTypes.Credits)
                {
                    // 7) Credits (collection)
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Total Credits", currentRow, currentRow == 1 ? titleRequired : !titleRequired, columns);
                    currentRow--;
                }
                else if(exportTypes == ExportTypes.Expenses)
                {
                    // 8) Exppenses (collection)
                    currentRow = WriteSingleObjectAsRow(worksheet, item, $"Total Expenses", currentRow, currentRow == 1 ? titleRequired : !titleRequired, columns);
                    currentRow--;
                }
                else
                {

                }
            }
        }
        else
        {
            // Single object top-level: write it like a single-row table (if desired) then nested collections/objects
            currentRow = WriteSingleObjectAsRow(worksheet, data, title, currentRow, true ,columns);

            foreach (var prop in data.GetType().GetProperties())
            {
                var val = prop.GetValue(data);
                if (val is IEnumerable subCollection && !(val is string) && subCollection.Cast<object>().Any())
                {
                    currentRow = WriteCollection(worksheet, subCollection, SplitPascalCase(prop.Name), currentRow + 2, true, columns);
                }
                else if (val != null && IsComplexObject(prop.PropertyType))
                {
                    currentRow = WriteSingleObjectAsRow(worksheet, val, SplitPascalCase(prop.Name), currentRow + 2, true, columns);
                }
            }
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // Adds faint repeated banners on-sheet (visible in Excel UI).
    // We place them at a few rows (1, 25, 50...) with very light color and rotation.
    private static void ApplyOnSheetFaintBanner(IXLWorksheet sheet, string text)
    {
        var bmpColor = XLColor.FromArgb(230, 230, 230); // very light gray background
        var fontColor = XLColor.FromArgb(200, 200, 200); // faint text color

        // We'll create a few wide merged ranges with rotated large text.
        // Keep them in safe rows that are unlikely to be overwritten by normal sections.
        // We place them at rows 1, 30, 60, ... but ensure we do not permanently overwrite header/title rows:
        // The code writes watermark earlier, but actual content is written starting at row 1.
        // To avoid overwriting content, we'll write watermark on rows beyond a safe offset; still header watermark is primary.
        int[] watermarkRows = new[] { 150, 300, 450 }; // far rows — they won't collide with usual report sizes

        foreach (var row in watermarkRows)
        {
            var rng = sheet.Range(row, 1, row + 3, 20); // 4 rows x 20 columns area
            try
            {
                rng.Merge();
                rng.Value = text;
                rng.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rng.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                rng.Style.Font.FontSize = 36;
                rng.Style.Font.Bold = true;
                rng.Style.Font.FontColor = fontColor;
                rng.Style.Fill.BackgroundColor = bmpColor;
                // rotate text slightly
                rng.Style.Alignment.TextRotation = 45;
            }
            catch
            {
                // safe ignore if merge/rotation fails in any environment
            }
        }

    }

    private static void ApplyWatermark(IXLWorksheet sheet, string text)
    {
        try
        {
            // Larger bitmap — gives better scaling for Excel
            using var bmp = new Bitmap(1200, 400);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Slightly darker gray, better opacity
                var color = Color.FromArgb(90, 120, 120, 120); // More visible gray
                using var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Point);
                using var brush = new SolidBrush(color);

                // Center text in the bitmap
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                // Rotate around center
                g.TranslateTransform(bmp.Width / 2f, bmp.Height / 2f);
                g.RotateTransform(-35);
                g.DrawString(text, font, brush, 0, 0, format);
            }

            // Convert image to memory stream
            using var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            // Add as centered picture (rough estimate for middle of the sheet)
            var pic = sheet.AddPicture(stream)
                .MoveTo(sheet.Cell("E15")) // roughly middle of first visible page
                .WithSize(1000, 350);      // Larger visible watermark area
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add watermark: {ex.Message}");
        }
    }


    #region Write helpers

    /// <summary>
    /// Writes a single object as a table with one header row and one data row.
    /// Skips bool properties, formats DateTime, and resolves special user-by fields.
    /// </summary>
    private static int WriteSingleObjectAsRow(IXLWorksheet sheet, object data, string title, int startRow, bool isTitleRequired = true, IEnumerable<string>? columns = null)
    {
        if (data is null) return startRow;
        int currentRow = startRow;

        // Section title
        if (isTitleRequired)
        {
            WriteSectionTitle(sheet, title, currentRow);
            currentRow++;
        }
        // Get properties to show (simple types, non-bool)
        var props = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
            .ToList();

        if (columns != null && columns.Any())
            props = props.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

        if (isTitleRequired)
        {
            // Header row
            for (int i = 0; i < props.Count; i++)
            {
                var cell = sheet.Cell(currentRow, i + 1);
                cell.Value = SplitPascalCase(props[i].Name);
                StyleHeaderCell(cell);
            }
            currentRow++;
        }

        // Single data row
        for (int i = 0; i < props.Count; i++)
        {
            var prop = props[i];
            var value = ResolveSpecialFields(data, prop);
            var cell = sheet.Cell(currentRow, i + 1);
            StyleDataCell(cell);

            if (value == null)
                cell.SetValue(string.Empty);
            else if (value is DateTime dt)
                cell.SetValue(dt.ToString(DateFormat));
            else
                cell.SetValue(value.ToString());
        }
        currentRow++;

        // Blank row after section
        currentRow++;
        return currentRow;
    }

    /// <summary>
    /// Writes a collection section (header row + multiple data rows)
    /// </summary>
    private static int WriteCollection(IXLWorksheet sheet, IEnumerable collection, string title, int startRow, bool isTitleRequired = true, IEnumerable<string>? columns = null)
    {
        if (collection == null) return startRow;
        var first = collection.Cast<object>().FirstOrDefault();
        if (first == null) return startRow;

        int currentRow = startRow;
        if (isTitleRequired)
        {
            WriteSectionTitle(sheet, title, currentRow);
            currentRow++;
        }

        var props = first.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
            .ToList();

        if (columns != null && columns.Any())
            props = props.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

        if (isTitleRequired)
        {
            // Header
            for (int i = 0; i < props.Count; i++)
            {
                var cell = sheet.Cell(currentRow, i + 1);
                cell.Value = SplitPascalCase(props[i].Name);
                StyleHeaderCell(cell);
            }
            currentRow++;
        }

        // Rows
        foreach (var item in collection)
        {
            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                var value = ResolveSpecialFields(item, prop);
                var cell = sheet.Cell(currentRow, i + 1);
                StyleDataCell(cell);

                if (value == null)
                    cell.SetValue(string.Empty);
                else if (value is DateTime dt)
                    cell.SetValue(dt.ToString(DateFormat));
                else
                    cell.SetValue(value.ToString());
            }
            currentRow++;
        }

        currentRow++; // blank row
        return currentRow;
    }

    /// <summary>
    /// Writes collection found by property name on parent object.
    /// </summary>
    private static void WriteCollectionIfExists(object parent, string propName, IXLWorksheet sheet, ref int currentRow, string title, bool isTitleRequired = true ,IEnumerable<string>? columns = null)
    {
        var prop = parent.GetType().GetProperty(propName);
        if (prop == null) return;
        var val = prop.GetValue(parent);
        if (val is IEnumerable coll && coll.Cast<object>().Any())
        {
            currentRow = WriteCollection(sheet, coll, title, currentRow + 2, isTitleRequired, columns);
        }
    }

    /// <summary>
    /// Writes single nested object found by property name.
    /// </summary>
    private static void WriteSingleIfExists(object parent, string propName, IXLWorksheet sheet, ref int currentRow, string title, bool isTitleRequired = true, IEnumerable<string>? columns = null)
    {
        var prop = parent.GetType().GetProperty(propName);
        if (prop == null) return;
        var val = prop.GetValue(parent);
        if (val != null)
        {
            currentRow = WriteSingleObjectAsRow(sheet, val, title, currentRow + 2, isTitleRequired ,columns);
        }
    }

    /// <summary>
    /// Writes a User section if a user-like navigation property exists on parent object.
    /// It locates the first navigation property whose type contains FullName (and value not null).
    /// Then writes only FullName, UserName, Email, PhoneNumber.
    /// </summary>
    private static void WriteUserSectionIfExists(object parent, IXLWorksheet sheet, ref int currentRow, string targetId)
    {
        //// Find navigation property that looks like a user (has FullName property)
        //var userProp = parent.GetType().GetProperties()
        //    .FirstOrDefault(p =>
        //    {
        //        var t = p.PropertyType;
        //        return !IsSimpleType(t) && t.GetProperty("FullName") != null;
        //    });

        //if (userProp == null) return;
        //var userVal = userProp.GetValue(parent);
        //if (userVal == null) return;

        //var title = $"User - ({targetId})";
        //WriteSectionTitle(sheet, title, currentRow);
        //currentRow++;

        //var wanted = new[] { "FullName", "UserName", "Email", "PhoneNumber" };
        //foreach (var field in wanted)
        //{
        //    var p = userVal.GetType().GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //    if (p == null) continue;
        //    var value = p.GetValue(userVal);
        //    var label = SplitPascalCase(field);

        //    sheet.Cell(currentRow, 1).SetValue(label);
        //    sheet.Cell(currentRow, 2).SetValue(value?.ToString() ?? "");
        //    sheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //    sheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //    currentRow++;
        //}

        //currentRow++; // blank row
        // Find navigation property that looks like a user (has FullName property)
        var userProp = parent.GetType().GetProperties()
            .FirstOrDefault(p =>
            {
                var t = p.PropertyType;
                return !IsSimpleType(t) && t.GetProperty("FullName") != null;
            });

        if (userProp == null) return;
        var userVal = userProp.GetValue(parent);
        if (userVal == null) return;

        var title = $"User - ({targetId})";

        WriteSectionTitle(sheet, title, currentRow);
        currentRow++;

        var wanted = new[] { "FullName", "UserName", "Email", "PhoneNumber" };

        // Header row
        int col = 1;
        foreach (var field in wanted)
        {
            var label = SplitPascalCase(field);
            sheet.Cell(currentRow, col).SetValue(label);
            sheet.Cell(currentRow, col).Style.Font.Bold = true;
            sheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            col++;
        }

        currentRow++;

        // --- Data Row ---
        col = 1;
        foreach (var field in wanted)
        {
            var p = userVal.GetType().GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) continue;
            var value = p.GetValue(userVal);
            sheet.Cell(currentRow, col).SetValue(value?.ToString() ?? "");
            sheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            col++;
        }

        currentRow += 2; // leave a blank row after user section
    }

    #endregion

    #region Styling & small helpers

    private static void WriteSectionTitle(IXLWorksheet sheet, string title, int row)
    {
        var titleCell = sheet.Cell(row, 1);
        titleCell.Value = SplitPascalCase(title);
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 12;
        titleCell.Style.Fill.BackgroundColor = XLColor.LightBlue;
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        // Merge a reasonable number of columns for visual header; adjust if you expect many columns
        sheet.Range(row, 1, row, 8).Merge();
    }

    private static void StyleHeaderCell(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192); // blue
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    }

    private static void StyleDataCell(IXLCell cell)
    {
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    }

    /// <summary>
    /// Resolve special fields:
    /// - If property ends with 'By', attempt to find a navigation property that looks like a User and return its FullName.
    /// - If property ends with 'Id' and there is a navigation property (Seller, Broker, OrderType, etc.), return that navigation entity's name field (TypeName, SellerName, BrokerName, Name, or first string property).
    /// - Otherwise return the property value.
    /// </summary>
    private static object? ResolveSpecialFields(object obj, PropertyInfo prop)
    {
        var propName = prop.Name;

        // ✅ 0) If obj is not Order and property == OrderId, show Order.OrderName instead of GUID/ID
        if (!obj.GetType().Name.Equals("Order", StringComparison.OrdinalIgnoreCase) &&
            propName.Equals("OrderId", StringComparison.OrdinalIgnoreCase))
        {
            var orderNav = obj.GetType().GetProperty("Order", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if(orderNav == null)
            {
                orderNav = obj.GetType().GetProperty("Orders", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            if (orderNav != null)
            {
                var orderVal = orderNav.GetValue(obj);
                if (orderVal != null)
                {
                    var nameProp = orderVal.GetType().GetProperty("OrderName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (nameProp != null)
                    {
                        var resolved = nameProp.GetValue(orderVal);
                        if (resolved != null)
                            return resolved; // ✅ Return friendly name instead of ID
                    }
                }
            }
            // fallback: return original OrderId
            return prop.GetValue(obj);
        }

        // 1) If property name ends with "By" (CreatedBy, OrderBy, PaidBy, InvoiceBy, UpdatedBy) -> try to get User.FullName
        if (propName.EndsWith("By", StringComparison.OrdinalIgnoreCase))
        {
            var userNav = obj.GetType().GetProperties()
                .Select(p => new { Prop = p, Val = p.GetValue(obj) })
                .FirstOrDefault(x => x.Val != null && x.Prop.PropertyType.GetProperty("FullName") != null);

            if (userNav != null)
            {
                var fullNameProp = userNav.Prop.PropertyType.GetProperty("FullName");
                if (fullNameProp != null)
                    return fullNameProp.GetValue(userNav.Val);
            }
        }

        // 2) If property looks like an FK (ends with Id), try to resolve related navigation name
        if ((propName.EndsWith("Id", StringComparison.OrdinalIgnoreCase) && !(propName.StartsWith("Order",StringComparison.OrdinalIgnoreCase))) || (propName.Contains("TypeId", StringComparison.OrdinalIgnoreCase) || propName.Contains("StatusId")))
        {
            // base name = e.g. "Seller" from "SellerId"
            var baseName = propName.Substring(0, propName.Length - 2);

            if(baseName.StartsWith("SubCategory", StringComparison.OrdinalIgnoreCase) && !baseName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                baseName = "Order" + baseName;
            }


            // Try exact navigation property with that name
            var navProp = obj.GetType().GetProperty(baseName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            // If not found, try some common alternatives (plural/suffix)
            if (navProp == null)
            {
                // try baseName + "Info" (SellerInfo)
                navProp = obj.GetType().GetProperty(baseName + "Info", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }

            if (navProp != null)
            {
                var navVal = navProp.GetValue(obj);
                if (navVal != null)
                {
                    // Preferred candidate names to look for on the navigation object:
                    var preferred = new[] { "Name", "SellerName", "BrokerName", "TypeName", "Title", "FullName", "SubCategoryName", "Status" };

                    foreach (var cand in preferred)
                    {
                        var candProp = navVal.GetType().GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (candProp != null)
                        {
                            var resolved = candProp.GetValue(navVal);
                            if (resolved != null) return resolved;
                        }
                    }

                    // fallback: first string property on navVal
                    var firstString = navVal.GetType().GetProperties()
                        .FirstOrDefault(p => p.PropertyType == typeof(string));
                    if (firstString != null)
                    {
                        var resolved = firstString.GetValue(navVal);
                        if (resolved != null) return resolved;
                    }
                }
            }
        }

        // 3) If the property itself is a navigation object (e.g., property named "Seller") return a friendly string
        var val = prop.GetValue(obj);
        if (val != null && !IsSimpleType(prop.PropertyType))
        {
            var fullNameProp = prop.PropertyType.GetProperty("FullName");
            if (fullNameProp != null)
                return fullNameProp.GetValue(val);

            var preferred = new[] { "Name", "SellerName", "BrokerName", "TypeName", "Title", "FullName", "SubCategoryName", "Status" };
            foreach (var cand in preferred)
            {
                var candProp = prop.PropertyType.GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (candProp != null)
                {
                    var resolved = candProp.GetValue(val);
                    if (resolved != null) return resolved;
                }
            }

            // fallback to first string property
            var firstString = prop.PropertyType.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(string));
            if (firstString != null)
            {
                var resolved = firstString.GetValue(val);
                if (resolved != null) return resolved;
            }
        }

        // default: return raw property value
        return val;
    }

    private static string GetTargetId(object item)
    {
        // prefer OrderId, then Id, then any property ending in Id
        var idProp = item.GetType().GetProperty("OrderId") ??
                     item.GetType().GetProperty("Id") ??
                     item.GetType().GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
        return idProp?.GetValue(item)?.ToString() ?? "N/A";
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(Guid)
            || type == typeof(double)
            || type == typeof(float);
    }

    private static bool IsComplexObject(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsClass && type != typeof(string);
    }

    private static string SplitPascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    }

    #endregion
}
