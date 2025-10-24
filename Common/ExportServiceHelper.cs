using ClosedXML.Excel;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShairiStore.Common;
public static class ExportServiceHelper
{
    private const string DateFormat = "dd-MMM-yyyy hh:mm tt";
    public static byte[] GenerateExcelFileSimple<T>(IEnumerable<T> data, IEnumerable<string> columns)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data");

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // Header
        for (int i = 0; i < properties.Count; i++)
            worksheet.Cell(1, i + 1).Value = properties[i].Name;

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                var value = properties[i].GetValue(item);

                // Explicitly convert to supported types
                if (value is null)
                    worksheet.Cell(row, i + 1).SetValue(string.Empty);
                else if (value is DateTime dt)
                    worksheet.Cell(row, i + 1).SetValue(dt);
                else if (value is bool b)
                    worksheet.Cell(row, i + 1).SetValue(b);
                else if (value is int or long or double or decimal or float)
                    worksheet.Cell(row, i + 1).SetValue(Convert.ToDouble(value));
                else
                    worksheet.Cell(row, i + 1).SetValue(value.ToString());
            }
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #region Dead Code
    //public static byte[] GenerateExportFile(object data, string title, IEnumerable<string>? columns = null)
    //{
    //    using var workbook = new XLWorkbook();
    //    var worksheet = workbook.Worksheets.Add($"{title} Export");

    //    int currentRow = 1;

    //    if (data is IEnumerable enumerable)
    //    {
    //        // Write top-level collection
    //        currentRow = WriteSection(worksheet, enumerable, title, currentRow, columns);
    //    }
    //    else
    //    {
    //        // Write single entity + its navigation collections
    //        currentRow = WriteSection(worksheet, data, title, currentRow, columns);

    //        // handle nested collections (e.g. OrderDetails, Payments)
    //        foreach (var prop in data.GetType().GetProperties())
    //        {
    //            var value = prop.GetValue(data);
    //            if (value is IEnumerable subCollection && !(value is string))
    //            {
    //                currentRow = WriteSection(worksheet, subCollection, prop.Name, currentRow + 2);
    //            }
    //        }
    //    }

    //    worksheet.Columns().AdjustToContents();

    //    using var stream = new MemoryStream();
    //    workbook.SaveAs(stream);
    //    return stream.ToArray();
    //}

    //private static int WriteSection(IXLWorksheet sheet, object data, string title, int startRow, IEnumerable<string>? columns = null)
    //{
    //    if (data is null) return startRow;
    //    int currentRow = startRow;

    //    // Section Header
    //    var titleCell = sheet.Cell(currentRow, 1);
    //    titleCell.Value = SplitPascalCase(title);
    //    titleCell.Style.Font.Bold = true;
    //    titleCell.Style.Font.FontSize = 14;
    //    titleCell.Style.Fill.BackgroundColor = XLColor.LightBlue;
    //    sheet.Range(currentRow, 1, currentRow, 6).Merge();
    //    currentRow++;

    //    // Handle collection
    //    if (data is IEnumerable enumerable && !(data is string))
    //    {
    //        var firstItem = enumerable.Cast<object>().FirstOrDefault();
    //        if (firstItem == null) return currentRow;

    //        var properties = firstItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //            .Where(p => IsSimpleType(p.PropertyType))
    //            .ToList();

    //        // Apply column filter if provided
    //        if (columns != null && columns.Any())
    //            properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

    //        // Header Row
    //        for (int i = 0; i < properties.Count; i++)
    //        {
    //            var cell = sheet.Cell(currentRow, i + 1);
    //            cell.Value = SplitPascalCase(properties[i].Name);
    //            cell.Style.Font.Bold = true;
    //            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192);
    //            cell.Style.Font.FontColor = XLColor.White;
    //        }
    //        currentRow++;

    //        // Data Rows
    //        foreach (var item in enumerable)
    //        {
    //            for (int i = 0; i < properties.Count; i++)
    //            {
    //                var value = properties[i].GetValue(item);

    //                // Explicitly convert to supported types
    //                if (value is null)
    //                    sheet.Cell(currentRow, i + 1).SetValue(string.Empty);
    //                else if (value is DateTime dt)
    //                    sheet.Cell(currentRow, i + 1).SetValue(dt);
    //                else if (value is bool b)
    //                    sheet.Cell(currentRow, i + 1).SetValue(b);
    //                else if (value is int or long or double or decimal or float)
    //                    sheet.Cell(currentRow, i + 1).SetValue(Convert.ToDouble(value));
    //                else
    //                    sheet.Cell(currentRow, i + 1).SetValue(value.ToString());
    //            }
    //            currentRow++;
    //        }

    //        currentRow++;
    //    }
    //    else
    //    {
    //        // Single Object (used for nested)
    //        var properties = data.GetType().GetProperties()
    //            .Where(p => IsSimpleType(p.PropertyType))
    //            .ToList();

    //        foreach (var prop in properties)
    //        {
    //            sheet.Cell(currentRow, 1).Value = SplitPascalCase(prop.Name);
    //            sheet.Cell(currentRow, 2).Value = prop.GetValue(data)?.ToString() ?? "";
    //            currentRow++;
    //        }

    //        currentRow++;
    //    }

    //    return currentRow;
    //}

    //private static bool IsSimpleType(Type type)
    //{
    //    return type.IsPrimitive || type.IsEnum || type == typeof(string) ||
    //           type == typeof(decimal) || type == typeof(DateTime) ||
    //           type == typeof(Guid) || Nullable.GetUnderlyingType(type) != null;
    //}

    //private static string SplitPascalCase(string input)
    //{
    //    return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    //}

    //public static byte[] GenerateExportFile(object data, string title, IEnumerable<string>? columns = null)
    //{
    //    using var workbook = new XLWorkbook();
    //    var worksheet = workbook.Worksheets.Add($"{title} Export");

    //    int currentRow = 1;

    //    if (data is IEnumerable enumerable)
    //    {
    //        // Loop through each item (Order with details etc.)
    //        foreach (var item in enumerable)
    //        {
    //            var targetId = GetTargetId(item);
    //            currentRow = WriteSection(worksheet, item, $"{title} - ({targetId})", currentRow, columns);

    //            // --- Nested Collections (OrderDetails, Payments, Invoice, etc.) ---
    //            foreach (var prop in item.GetType().GetProperties())
    //            {
    //                var value = prop.GetValue(item);
    //                if (value is IEnumerable subCollection && !(value is string))
    //                {
    //                    currentRow = WriteSection(worksheet, subCollection, $"{prop.Name} - ({targetId})", currentRow + 2);
    //                }
    //            }

    //            // --- Related Single Entities (Seller, Broker, Warehouse) ---
    //            WriteIfExists(item, "Seller", worksheet, ref currentRow);
    //            WriteIfExists(item, "Broker", worksheet, ref currentRow);
    //            WriteIfExists(item, "Warehouse", worksheet, ref currentRow);
    //        }
    //    }
    //    else
    //    {
    //        // Single record export
    //        currentRow = WriteSection(worksheet, data, title, currentRow, columns);
    //    }

    //    worksheet.Columns().AdjustToContents();

    //    using var stream = new MemoryStream();
    //    workbook.SaveAs(stream);
    //    return stream.ToArray();
    //}

    //private static void WriteIfExists(object parent, string propName, IXLWorksheet sheet, ref int currentRow)
    //{
    //    var prop = parent.GetType().GetProperty(propName);
    //    if (prop == null) return;

    //    var value = prop.GetValue(parent);
    //    if (value == null) return;

    //    currentRow = WriteSection(sheet, value, $"{propName} Info", currentRow + 2);
    //}

    //private static int WriteSection(IXLWorksheet sheet, object data, string title, int startRow, IEnumerable<string>? columns = null)
    //{
    //    if (data is null) return startRow;
    //    int currentRow = startRow;

    //    // --- Section Title ---
    //    var titleCell = sheet.Cell(currentRow, 1);
    //    titleCell.Value = SplitPascalCase(title);
    //    titleCell.Style.Font.Bold = true;
    //    titleCell.Style.Font.FontSize = 14;
    //    titleCell.Style.Fill.BackgroundColor = XLColor.LightBlue;
    //    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    //    sheet.Range(currentRow, 1, currentRow, 8).Merge();
    //    currentRow++;

    //    // --- Handle Collection ---
    //    if (data is IEnumerable enumerable && !(data is string))
    //    {
    //        var firstItem = enumerable.Cast<object>().FirstOrDefault();
    //        if (firstItem == null) return currentRow;

    //        var properties = firstItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
    //            .ToList();

    //        // Apply column filter if provided
    //        if (columns != null && columns.Any())
    //            properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToList();

    //        // Header Row
    //        for (int i = 0; i < properties.Count; i++)
    //        {
    //            var cell = sheet.Cell(currentRow, i + 1);
    //            cell.Value = SplitPascalCase(properties[i].Name);
    //            cell.Style.Font.Bold = true;
    //            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 112, 192);
    //            cell.Style.Font.FontColor = XLColor.White;
    //            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    //        }
    //        currentRow++;

    //        // Data Rows
    //        foreach (var item in enumerable)
    //        {
    //            for (int i = 0; i < properties.Count; i++)
    //            {
    //                var prop = properties[i];
    //                var value = ResolveSpecialFields(item, prop);

    //                var cell = sheet.Cell(currentRow, i + 1);
    //                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

    //                if (value == null)
    //                    cell.SetValue(string.Empty);
    //                else if (value is DateTime dt)
    //                    cell.Value = dt.ToString("dd-MMM-yyyy hh:mm tt");
    //                else
    //                    cell.Value = value.ToString();
    //            }
    //            currentRow++;
    //        }

    //        currentRow++;
    //    }
    //    else
    //    {
    //        // --- Single Object (e.g. Seller, Warehouse, User) ---
    //        var properties = data.GetType().GetProperties()
    //            .Where(p => IsSimpleType(p.PropertyType) && p.PropertyType != typeof(bool))
    //            .ToList();

    //        // Special case: User Info section → only specific fields
    //        if (title.Contains("User", StringComparison.OrdinalIgnoreCase))
    //        {
    //            properties = properties.Where(p =>
    //                new[] { "FullName", "UserName", "Email", "PhoneNumber" }
    //                .Contains(p.Name, StringComparer.OrdinalIgnoreCase)
    //            ).ToList();
    //        }

    //        foreach (var prop in properties)
    //        {
    //            var value = ResolveSpecialFields(data, prop);
    //            var label = SplitPascalCase(prop.Name);

    //            sheet.Cell(currentRow, 1).Value = label;
    //            sheet.Cell(currentRow, 2).Value = value is DateTime dt
    //                ? dt.ToString("dd-MMM-yyyy hh:mm tt")
    //                : value?.ToString() ?? "";

    //            sheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    //            sheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

    //            currentRow++;
    //        }

    //        currentRow++;
    //    }

    //    return currentRow;
    //}

    //#region Helpers

    //private static object? ResolveSpecialFields(object data, PropertyInfo prop)
    //{
    //    // Handle User-related GUIDs (CreatedBy, PaidBy, etc.)
    //    if (prop.Name.EndsWith("By", StringComparison.OrdinalIgnoreCase))
    //    {
    //        var userProp = data.GetType().GetProperty("User");
    //        var userValue = userProp?.GetValue(data);
    //        if (userValue != null)
    //        {
    //            var fullNameProp = userValue.GetType().GetProperty("FullName");
    //            return fullNameProp?.GetValue(userValue);
    //        }
    //    }

    //    return prop.GetValue(data);
    //}

    //private static string GetTargetId(object item)
    //{
    //    var idProp = item.GetType().GetProperty("Id") ?? item.GetType().GetProperty("OrderId") ??
    //                 item.GetType().GetProperty("TargetId");
    //    return idProp?.GetValue(item)?.ToString() ?? "N/A";
    //}

    //private static bool IsSimpleType(Type type)
    //{
    //    return type.IsPrimitive || type.IsEnum || type == typeof(string) ||
    //           type == typeof(decimal) || type == typeof(DateTime) ||
    //           type == typeof(Guid) || Nullable.GetUnderlyingType(type) != null;
    //}

    //private static string SplitPascalCase(string input)
    //{
    //    return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    //}

    //#endregion
    #endregion

    /// <summary>
    /// Generic export entry point. 'data' can be a collection (e.g. List<Order>) or a single object.
    /// </summary>
    public static byte[] GenerateExportFile(object data, string title, IEnumerable<string>? columns = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add($"{title} Export");
        int currentRow = 1;

        if (data is IEnumerable enumerable && !(data is string))
        {
            // If top-level is a collection of orders (or other entities), iterate each item and write sections
            foreach (var item in enumerable.Cast<object>())
            {
                var targetId = GetTargetId(item);

                // 1) Order main section written as single-row table
                currentRow = WriteSingleObjectAsRow(worksheet, item, $"Order - ({targetId})", currentRow, columns);

                // 2) OrderDetails (collection)
                WriteCollectionIfExists(item, "OrderDetails", worksheet, ref currentRow, $"Order Details - ({targetId})", columns);

                // 3) OrderPayments (collection)
                WriteCollectionIfExists(item, "OrderPayments", worksheet, ref currentRow, $"Order Payments - ({targetId})", columns);

                // 4) OrderInvoice (single object)
                WriteSingleIfExists(item, "OrderInvoice", worksheet, ref currentRow, $"Order Invoice - ({targetId})", columns);

                // 5) Related info sections: Seller, Broker, Warehouse
                WriteSingleIfExists(item, "Seller", worksheet, ref currentRow, "Seller Info", null);
                WriteSingleIfExists(item, "Broker", worksheet, ref currentRow, "Broker Info", null);
                WriteSingleIfExists(item, "Warehouse", worksheet, ref currentRow, "Warehouse Info", null);

                // 6) User section - show only required user fields (if any navigation property representing a user exists)
                WriteUserSectionIfExists(item, worksheet, ref currentRow, targetId);

                // User Info (limited fields)
                //var user = item.GetType().GetProperty("User")?.GetValue(user);
                //if (user != null)
                //    currentRow = WriteSection(worksheet, user, "User Info", currentRow + 2, new List<string> { "FullName", "UserName", "Email", "PhoneNumber" }, isCollection: false);
            }
        }
        else
        {
            // Single object top-level: write it like a single-row table (if desired) then nested collections/objects
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
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #region Write helpers

    /// <summary>
    /// Writes a single object as a table with one header row and one data row.
    /// Skips bool properties, formats DateTime, and resolves special user-by fields.
    /// </summary>
    private static int WriteSingleObjectAsRow(IXLWorksheet sheet, object data, string title, int startRow, IEnumerable<string>? columns = null)
    {
        if (data is null) return startRow;
        int currentRow = startRow;

        // Section title
        WriteSectionTitle(sheet, title, currentRow);
        currentRow++;

        // Get properties to show (simple types, non-bool)
        var props = data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
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

    /// <summary>
    /// Writes a collection section (header row + multiple data rows)
    /// </summary>
    private static int WriteCollection(IXLWorksheet sheet, IEnumerable collection, string title, int startRow, IEnumerable<string>? columns = null)
    {
        if (collection == null) return startRow;
        var first = collection.Cast<object>().FirstOrDefault();
        if (first == null) return startRow;

        int currentRow = startRow;
        WriteSectionTitle(sheet, title, currentRow);
        currentRow++;

        var props = first.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
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

    /// <summary>
    /// Writes collection found by property name on parent object.
    /// </summary>
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

    /// <summary>
    /// Writes single nested object found by property name.
    /// </summary>
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

    /// <summary>
    /// Writes a User section if a user-like navigation property exists on parent object.
    /// It locates the first navigation property whose type contains FullName (and value not null).
    /// Then writes only FullName, UserName, Email, PhoneNumber.
    /// </summary>
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
    /// - Otherwise return the property value.
    /// </summary>
    private static object? ResolveSpecialFields(object obj, PropertyInfo prop)
    {
        var propName = prop.Name;

        // If property name ends with "By" (CreatedBy, OrderBy, PaidBy, InvoiceBy, UpdatedBy)
        if (propName.EndsWith("By", StringComparison.OrdinalIgnoreCase))
        {
            // Find any navigation property on obj that appears to be a user (has FullName)
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

        // Normal value:
        var val = prop.GetValue(obj);

        // If property itself is a reference to a user (e.g., property named 'User'), handle if asked
        if (val != null && !IsSimpleType(prop.PropertyType))
        {
            // if it's a user-like object, prefer FullName for quick display
            var fullNameProp = prop.PropertyType.GetProperty("FullName");
            if (fullNameProp != null)
                return fullNameProp.GetValue(val);
        }

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
