using ClosedXML.Excel;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShairiStore.Common;

public static class WatermarkExportServiceHelper
{
    private const string DateFormat = "dd-MMM-yyyy hh:mm tt";
    private const string WatermarkText = "CS & Sons";

    /// <summary>
    /// Generic export entry point. 'data' can be a collection (e.g. List<Order>) or a single object.
    /// </summary>
    public static byte[] GenerateExportFile(object data, string title, IEnumerable<string>? columns = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"{title} Export");

        // Add watermark (header + faint repeated on-sheet banners)
        ApplyWatermark(worksheet, WatermarkText);
        ApplyOnSheetFaintBanner(worksheet, WatermarkText);

        int currentRow = 1;

        if (data is IEnumerable enumerable && !(data is string))
        {
            foreach (var item in enumerable.Cast<object>())
            {
                var targetId = GetTargetId(item);

                // 1) Order main section (single row)
                currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order - ({targetId})", currentRow, columns);

                // 2) OrderDetails
                WriteCollectionIfExists(item, "OrderDetails", worksheet, ref currentRow, $"Order Details - ({targetId})", columns);

                // 3) OrderPayments
                WriteCollectionIfExists(item, "OrderPayments", worksheet, ref currentRow, $"Order Payments - ({targetId})", columns);

                // 4) OrderInvoice
                WriteSingleIfExists(item, "OrderInvoice", worksheet, ref currentRow, $"Order Invoice - ({targetId})", columns);

                // 5) Related Info
                WriteSingleIfExists(item, "Seller", worksheet, ref currentRow, "Seller Info", null);
                WriteSingleIfExists(item, "Broker", worksheet, ref currentRow, "Broker Info", null);
                WriteSingleIfExists(item, "Warehouse", worksheet, ref currentRow, "Warehouse Info", null);

                // 6) User Info
                WriteUserSectionIfExists(item, worksheet, ref currentRow, targetId);
            }
        }
        else
        {
            // Single object top-level
            currentRow = WriteSingleObjectAsRow(worksheet, data, title, currentRow, columns);

            foreach (var prop in data.GetType().GetProperties())
            {
                var val = prop.GetValue(data);
                if (val is IEnumerable subCollection && !(val is string) && subCollection.Cast<object>().Any())
                {
                    currentRow = WriteCollection(worksheet, subCollection, SplitPascalCase(prop.Name), currentRow + 2, columns);
                }
                else if (val != null && IsComplexObject(prop.PropertyType))
                {
                    currentRow = WriteSingleObjectAsRow(worksheet, val, SplitPascalCase(prop.Name), currentRow + 2, columns);
                }
            }
        }

        worksheet.Columns().AdjustToContents();

        // Optionally set print options: center horizontally, landscape, show gridlines off for cleaner look, etc.
        worksheet.PageSetup.CenterHorizontally = true;
        worksheet.PageSetup.CenterVertically = false;
        worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #region Write Helpers

    private static int WriteSingleObjectAsRow(IXLWorksheet sheet, object data, string title, int startRow, IEnumerable<string>? columns = null)
    {
        if (data == null) return startRow;
        int currentRow = startRow;

        WriteSectionTitle(sheet, title, currentRow);
        currentRow++;

        var props = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
            .ToList();

        if (columns != null && columns.Any())
            props = props.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

        // Header row
        for (int i = 0; i < props.Count; i++)
        {
            var cell = sheet.Cell(currentRow, i + 1);
            cell.Value = SplitPascalCase(props[i].Name);
            StyleHeaderCell(cell);
        }
        currentRow++;

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

    private static int WriteCollection(IXLWorksheet sheet, IEnumerable collection, string title, int startRow, IEnumerable<string>? columns = null)
    {
        if (collection == null) return startRow;
        var first = collection.Cast<object>().FirstOrDefault();
        if (first == null) return startRow;

        int currentRow = startRow;
        WriteSectionTitle(sheet, title, currentRow);
        currentRow++;

        var props = first.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
            .ToList();

        if (columns != null && columns.Any())
            props = props.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

        // Header
        for (int i = 0; i < props.Count; i++)
        {
            var cell = sheet.Cell(currentRow, i + 1);
            cell.Value = SplitPascalCase(props[i].Name);
            StyleHeaderCell(cell);
        }
        currentRow++;

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

    private static void WriteCollectionIfExists(object parent, string propName, IXLWorksheet sheet, ref int currentRow, string title, IEnumerable<string>? columns = null)
    {
        var prop = parent.GetType().GetProperty(propName);
        if (prop == null) return;
        var val = prop.GetValue(parent);
        if (val is IEnumerable coll && coll.Cast<object>().Any())
        {
            currentRow = WriteCollection(sheet, coll, title, currentRow + 2, columns);
        }
    }

    private static void WriteSingleIfExists(object parent, string propName, IXLWorksheet sheet, ref int currentRow, string title, IEnumerable<string>? columns = null)
    {
        var prop = parent.GetType().GetProperty(propName);
        if (prop == null) return;
        var val = prop.GetValue(parent);
        if (val != null)
        {
            currentRow = WriteSingleObjectAsRow(sheet, val, title, currentRow + 2, columns);
        }
    }

    private static void WriteUserSectionIfExists(object parent, IXLWorksheet sheet, ref int currentRow, string targetId)
    {
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
        foreach (var field in wanted)
        {
            var p = userVal.GetType().GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) continue;
            var value = p.GetValue(userVal);
            var label = SplitPascalCase(field);

            sheet.Cell(currentRow, 1).SetValue(label);
            sheet.Cell(currentRow, 2).SetValue(value?.ToString() ?? "");
            sheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            sheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            currentRow++;
        }

        currentRow++; // blank row
    }

    #endregion

    #region Styling & Helpers

    private static void WriteSectionTitle(IXLWorksheet sheet, string title, int row)
    {
        var titleCell = sheet.Cell(row, 1);
        titleCell.Value = SplitPascalCase(title);
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 12;
        titleCell.Style.Fill.BackgroundColor = XLColor.LightBlue;
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        sheet.Range(row, 1, row, 12).Merge();
    }

    private static void StyleHeaderCell(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192);
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static void StyleDataCell(IXLCell cell)
    {
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        // wrap text for longer fields
        cell.Style.Alignment.WrapText = true;
    }

    ///// <summary>
    ///// Resolve special fields:
    ///// - If property ends with 'By', attempt to find a navigation property that looks like a User and return its FullName.
    ///// - If property ends with 'Id' and there is a navigation property (Seller, Broker, OrderType, Category, SubCategory, Brand, PaymentMethod, Status), return that navigation entity's name field (TypeName, SellerName, BrokerName, CategoryName, SubCategoryName, BrandName, PaymentMode, Status, or first string property).
    ///// - Otherwise return the property value.
    ///// </summary>
    //private static object? ResolveSpecialFields(object obj, PropertyInfo prop)
    //{
    //    var propName = prop.Name;
    //    var val = prop.GetValue(obj);

    //    // 1) If property name ends with "By" (CreatedBy, OrderBy, PaidBy, InvoiceBy, UpdatedBy) -> try to get User.FullName
    //    if (propName.EndsWith("By", StringComparison.OrdinalIgnoreCase))
    //    {
    //        var userNav = obj.GetType().GetProperties()
    //            .Select(p => new { Prop = p, Val = p.GetValue(obj) })
    //            .FirstOrDefault(x => x.Val != null && x.Prop.PropertyType.GetProperty("FullName") != null);

    //        if (userNav != null)
    //        {
    //            var fullNameProp = userNav.Prop.PropertyType.GetProperty("FullName");
    //            if (fullNameProp != null)
    //                return fullNameProp.GetValue(userNav.Val);
    //        }
    //    }

    //    // 2) If property looks like an FK (ends with Id), try to resolve related navigation name
    //    if (propName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
    //    {
    //        var baseName = propName.Substring(0, propName.Length - 2); // e.g., SellerId -> Seller

    //        // Potential navigation property names to try
    //        var candidates = new[]
    //        {
    //            baseName,                   // Seller
    //            baseName + "Info",          // SellerInfo
    //            baseName + "s",             // Sellers
    //            baseName + "Ref",           // SellerRef
    //            baseName + "Entity"         // SellerEntity
    //        };

    //        PropertyInfo? navProp = null;
    //        foreach (var c in candidates)
    //        {
    //            navProp = obj.GetType().GetProperty(c, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    //            if (navProp != null) break;
    //        }

    //        if (navProp != null)
    //        {
    //            var navVal = navProp.GetValue(obj);
    //            if (navVal != null)
    //            {
    //                // Try known preferred fields for name resolution
    //                var preferred = new[]
    //                {
    //                    "TypeName", "SellerName", "BrokerName", "CategoryName",
    //                    "SubCategoryName", "BrandName", "PaymentMode", "Status",
    //                    "Name", "Title", "FullName"
    //                };

    //                foreach (var cand in preferred)
    //                {
    //                    var candProp = navVal.GetType().GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    //                    if (candProp != null)
    //                    {
    //                        var resolved = candProp.GetValue(navVal);
    //                        if (resolved != null) return resolved;
    //                    }
    //                }

    //                // fallback: first string property
    //                var firstString = navVal.GetType().GetProperties()
    //                    .FirstOrDefault(p => p.PropertyType == typeof(string));
    //                if (firstString != null)
    //                {
    //                    var resolved = firstString.GetValue(navVal);
    //                    if (resolved != null) return resolved;
    //                }
    //            }
    //        }
    //    }

    //    // 3) If the property itself is a navigation object (e.g., property named "Seller") return a friendly string
    //    if (val != null && !IsSimpleType(prop.PropertyType))
    //    {
    //        var preferred = new[]
    //        {
    //            "TypeName", "SellerName", "BrokerName", "CategoryName",
    //            "SubCategoryName", "BrandName", "PaymentMode", "Status",
    //            "Name", "Title", "FullName"
    //        };

    //        foreach (var cand in preferred)
    //        {
    //            var candProp = prop.PropertyType.GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    //            if (candProp != null)
    //            {
    //                var resolved = candProp.GetValue(val);
    //                if (resolved != null) return resolved;
    //            }
    //        }

    //        // fallback to first string property
    //        var firstString = prop.PropertyType.GetProperties()
    //            .FirstOrDefault(p => p.PropertyType == typeof(string));
    //        if (firstString != null)
    //        {
    //            var resolved = firstString.GetValue(val);
    //            if (resolved != null) return resolved;
    //        }
    //    }

    //    // 4) Format DateTime
    //    if (val is DateTime dt)
    //        return dt.ToString(DateFormat);

    //    // default: return raw property value
    //    return val;
    //}


    private static object? ResolveSpecialFields(object obj, PropertyInfo prop)
    {
        var propName = prop.Name;
        var val = prop.GetValue(obj);

        // 1) Handle "...By" pattern (CreatedBy, PaidBy, etc.)
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

        // 2) Handle foreign keys ending with Id
        if (propName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
        {
            var baseName = propName[..^2]; // Remove 'Id', e.g., SellerId -> Seller

            var candidates = new[]
            {
            baseName,
            baseName + "Info",
            baseName + "s",
            baseName + "Ref",
            baseName + "Entity"
        };

            PropertyInfo? navProp = candidates
                .Select(c => obj.GetType().GetProperty(c, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase))
                .FirstOrDefault(p => p != null);

            if (navProp != null)
            {
                var navVal = navProp.GetValue(obj);
                if (navVal != null)
                {
                    // ✅ Handle SubCategory explicitly
                    if (navProp.Name.Equals("SubCategory", StringComparison.OrdinalIgnoreCase) ||
                        navProp.PropertyType.Name.Equals("SubCategory", StringComparison.OrdinalIgnoreCase) ||
                        navProp.PropertyType.Name.Equals("SubCategoryInfo", StringComparison.OrdinalIgnoreCase))
                    {
                        var subCatNameProp = navVal.GetType().GetProperty("SubCategoryName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (subCatNameProp != null)
                        {
                            var resolved = subCatNameProp.GetValue(navVal);
                            if (resolved != null)
                                return resolved;
                        }
                    }

                    // Generic preferred name resolution
                    var preferred = new[]
                    {
                    "TypeName", "SellerName", "BrokerName", "CategoryName",
                    "SubCategoryName", "BrandName", "PaymentMode", "Status",
                    "Name", "Title", "FullName"
                };

                    foreach (var cand in preferred)
                    {
                        var candProp = navVal.GetType().GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (candProp != null)
                        {
                            var resolved = candProp.GetValue(navVal);
                            if (resolved != null)
                                return resolved;
                        }
                    }

                    // Fallback: first string property
                    var firstString = navVal.GetType().GetProperties()
                        .FirstOrDefault(p => p.PropertyType == typeof(string));
                    if (firstString != null)
                    {
                        var resolved = firstString.GetValue(navVal);
                        if (resolved != null)
                            return resolved;
                    }
                }
            }
        }

        // 3) Handle direct navigation objects (e.g., property = SubCategory)
        if (val != null && !IsSimpleType(prop.PropertyType))
        {
            // ✅ Handle SubCategory directly
            if (propName.Equals("SubCategory", StringComparison.OrdinalIgnoreCase) ||
                prop.PropertyType.Name.Equals("SubCategory", StringComparison.OrdinalIgnoreCase) ||
                prop.PropertyType.Name.Equals("SubCategoryInfo", StringComparison.OrdinalIgnoreCase))
            {
                var subCatNameProp = prop.PropertyType.GetProperty("SubCategoryName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (subCatNameProp != null)
                {
                    var resolved = subCatNameProp.GetValue(val);
                    if (resolved != null)
                        return resolved;
                }
            }

            // Generic resolution for other navigation objects
            var preferred = new[]
            {
            "TypeName", "SellerName", "BrokerName", "CategoryName",
            "SubCategoryName", "BrandName", "PaymentMode", "Status",
            "Name", "Title", "FullName"
        };

            foreach (var cand in preferred)
            {
                var candProp = prop.PropertyType.GetProperty(cand, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (candProp != null)
                {
                    var resolved = candProp.GetValue(val);
                    if (resolved != null)
                        return resolved;
                }
            }

            // fallback: first string property
            var firstString = prop.PropertyType.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(string));
            if (firstString != null)
            {
                var resolved = firstString.GetValue(val);
                if (resolved != null)
                    return resolved;
            }
        }

        // 4) Format DateTime
        if (val is DateTime dt)
            return dt.ToString(DateFormat);

        // Default
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

    #region Watermark Helpers

    // Adds a header watermark (visible in print / PDFs)
    private static void ApplyHeaderWatermark(IXLWorksheet sheet, string text)
    {
        try
        {
            // Modern ClosedXML API (v0.97+)
            var header = sheet.PageSetup.Header;
            if (header != null)
            {
                header.Center.AddText(text)
                    .SetFontName("Arial")
                    .SetFontSize(16)
                    .SetFontColor(XLColor.Gray);
            }

            // Optional: Add to footer as well if you want it repeated there
            // var footer = sheet.PageSetup.Footer;
            // footer.Center.AddText(text).SetFontName("Arial").SetFontSize(16).SetFontColor(XLColor.Gray);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to apply header watermark: {ex.Message}");
        }
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
            // 1️⃣ Create a transparent bitmap with watermark text
            using var bmp = new Bitmap(400, 100);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                using var font = new Font("Arial", 36, FontStyle.Bold, GraphicsUnit.Point);
                var color = Color.FromArgb(65, 150, 150, 150); // Very light gray with transparency
                using var brush = new SolidBrush(color);

                // Rotate text diagonally
                g.TranslateTransform(200, 50);
                g.RotateTransform(-25);
                g.DrawString(text, font, brush, new PointF(-150, -25));
            }

            // 2️⃣ Convert image to stream
            using var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            // 3️⃣ Add as semi-transparent picture (acts like a watermark)
            var pic = sheet.AddPicture(stream)
                .MoveTo(sheet.Cell("A1"), 10, 10)
                .WithSize(600, 200);

            // No MoveToBack() needed — transparency handles the “behind” effect
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add watermark: {ex.Message}");
        }
    }

    #endregion
}
