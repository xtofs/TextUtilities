namespace Xtofs.TextUtilities;

/// <summary>
/// Provides extension methods for writing formatted tables to a <see cref="TextWriter"/>.
/// </summary>
public static class TableFormatter
{
    extension(TextWriter writer)
    {
        /// <summary>
        /// Writes a formatted table to the <see cref="TextWriter"/> asynchronously using the specified rows and table format.
        /// </summary>
        /// <typeparam name="T">The type of the data row.</typeparam>
        /// <param name="rows">The rows to write to the table.</param>
        /// <param name="format">The format of the table, including headers and value selectors.</param>
        /// <returns>A task representing the asynchronous write operation.</returns>
        public async Task WriteTableAsync<T>(IReadOnlyList<T> rows, TableFormat<T> format)
        {
            var (headers, valueSelectors) = format;

            if (headers.Length != valueSelectors.Count)
            {
                throw new ArgumentException("Number of headers must match number of value selectors.");
            }

            string[][] cells = [
                headers,
            .. rows.Select(FormatCells)
            ];
            var widths = new int[valueSelectors.Count];
            for (int i = 0; i < valueSelectors.Count; i++)
            {
                widths[i] = cells.Select(row => row[i].Length).Max();
            }

            for (int rowIndex = 0; rowIndex < cells.Length; rowIndex++)
            {
                var row = cells[rowIndex];
                for (int colIndex = 0; colIndex < row.Length; colIndex++)
                {
                    var cell = row[colIndex];
                    await writer.WriteAsync(cell.PadRight(widths[colIndex] + 2));
                }

                await writer.WriteLineAsync();

                if (rowIndex == 0)
                {
                    // after printing headers, print a separator
                    for (int colIndex = 0; colIndex < row.Length; colIndex++)
                    {
                        await writer.WriteAsync(new string('-', widths[colIndex]) + "  ");
                    }

                    await writer.WriteLineAsync();
                }
            }

            // create a row of cells, one cell per value selector
            // use this instead of selectors.Select(..).ToArray()
            string[] FormatCells(T item)
            {
                string[] values = new string[valueSelectors.Count];
                for (int i = 0; i < valueSelectors.Count; i++)
                {
                    values[i] = valueSelectors[i](item);
                }

                return values;
            }
        }
    }
}


