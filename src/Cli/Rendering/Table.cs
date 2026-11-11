using CliInterface.Cli.Engine;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Plain console table: header plus rows, columns sized by content.
    /// Deliberately text only, so it works everywhere including scripts.
    /// </summary>
    public sealed class Table
    {
        private readonly List<string> _headers;
        private readonly List<List<string>> _rows = new();

        /// <summary>
        /// Initializes a table with the given column headers.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        public Table(params string[] headers)
        {
            _headers = headers.ToList();
        }

        /// <summary>
        /// Appends one row. Missing cells become empty.
        /// </summary>
        /// <param name="cells">The row cells in column order.</param>
        public void AddRow(params string?[] cells)
        {
            List<string> row = new(_headers.Count);
            for (int i = 0; i < _headers.Count; i++)
            {
                row.Add(i < cells.Length && cells[i] is not null ? cells[i]! : string.Empty);
            }
            _rows.Add(row);
        }

        /// <summary>
        /// Draws the table with aligned columns.
        /// </summary>
        /// <param name="console">The console for output.</param>
        public void Draw(IConsole console)
        {
            int[] widths = new int[_headers.Count];
            for (int i = 0; i < _headers.Count; i++)
            {
                widths[i] = _headers[i].Length;
            }
            foreach (List<string> row in _rows)
            {
                for (int i = 0; i < _headers.Count; i++)
                {
                    if (i < row.Count && row[i].Length > widths[i])
                        widths[i] = row[i].Length;
                }
            }

            DrawRow(console, _headers, widths);
            console.WriteLine(string.Join("  ", widths.Select(w => new string('-', w))));
            foreach (List<string> row in _rows)
            {
                DrawRow(console, row, widths);
            }
        }

        /// <summary>
        /// Prints one padded line of the table.
        /// </summary>
        /// <param name="console">The console for output.</param>
        /// <param name="cells">The cells to print.</param>
        /// <param name="widths">The column widths.</param>
        private static void DrawRow(IConsole console, List<string> cells, int[] widths)
        {
            var parts = new List<string>();
            for (int i = 0; i < widths.Length; i++)
            {
                string cell = i < cells.Count ? cells[i] : string.Empty;
                parts.Add(cell.PadRight(widths[i]));
            }
            console.WriteLine(string.Join("  ", parts).TrimEnd());
        }
    }
}
