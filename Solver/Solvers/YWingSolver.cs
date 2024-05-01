using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class YWingSolver : ISolver
{
    public string Name => nameof(YWingSolver);

    public bool IsTough => true;

    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        /*
            YWing is a mix of naked triples (2/2/2 variant), pointed pairs, and 3/4ers of an XWing.
            It requires three cells, each with a different two candidates, that union to three candidates.
            One cell (the trunk of the Y) can see the other two cells (same column or box).
            The other two cannot see each other (not in the same box, column, or two).

            Whatever is the common candidate in the two ends of the Y, that candidate can be removed
            from every cell those two cells can both see.
            This candidate will not be in the trunk Y cell (by construction), so no issues there.
        */

        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);

        if (cellCandidates.Count != 2)
        {
            return false;
        }

        List<IEnumerable<int>> lines = Puzzle.GetLinesForCell(cell);

        foreach (int lineOne in Enumerable.Range(0, lines.Count))
        {
            foreach (int pincerOne in GetPincersInLine(puzzle, cell, cellCandidates, lines[lineOne]))
            {
                IReadOnlyList<int> pincerOneCandidates = puzzle.GetCellCandidates(pincerOne);
                List<int> yWingCandidates = cellCandidates.Union(pincerOneCandidates).ToList();
                Cell pincerOneCell = Puzzle.GetCellForIndex(pincerOne);

                // We now have a first pincer cell; need a second
                // The second pincer cell will not be in the same line as the first
                foreach (int lineTwo in Enumerable.Range(0, lines.Count).Where(x => x != lineOne))
                {
                    foreach (int pincerTwo in GetPincersInLine(puzzle, cell, cellCandidates, lines[lineTwo]))
                    {
                        Cell pincerTwoCell = Puzzle.GetCellForIndex(pincerTwo);

                        if (pincerOneCell.Box == pincerTwoCell.Box ||
                            pincerOneCell.Row == pincerTwoCell.Row ||
                            pincerOneCell.Column == pincerTwoCell.Column)
                            {
                                continue;
                            }

                        IReadOnlyList<int> pincerTwoCandidates = puzzle.GetCellCandidates(pincerTwo);

                        if (pincerOneCandidates.Intersect(pincerTwoCandidates).Count() is 1 &&
                            yWingCandidates.Intersect(pincerTwoCandidates).Count() is 2)
                        {
                            int value = pincerOneCandidates.Intersect(pincerTwoCandidates).Single();
                            // Now search for the shared values in a shared line
                            IEnumerable<int> sharedIndices = GetSharedIndices(puzzle, cell, Puzzle.GetCellForIndex(pincerOne), Puzzle.GetCellForIndex(pincerTwo));
                            
                            foreach (int sharedIndex in sharedIndices)
                            {
                                IReadOnlyList<int> candidates = puzzle.GetCellCandidates(sharedIndex);
                                if (candidates.Contains(value))
                                {
                                    Cell c = Puzzle.GetCellForIndex(sharedIndex);
                                    Solution s = new(c, -1, Name)
                                    {
                                        AlignedIndices = [cell, pincerOne, pincerTwo],
                                        AlignedCandidates = yWingCandidates,
                                        RemovalCandidates = [value],
                                        Next = solution
                                    };
                                    solution = s;
                                }
                            }

                            if (solution is not null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private static IEnumerable<int> GetPincersInLine(Puzzle puzzle, Cell cell, IReadOnlyList<int> cellCandidates, IEnumerable<int> line)
    {
        foreach (int index in line.Where(x => x != cell && puzzle.GetCellCandidates(x).Count is 2))
        {
            IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);
            if (cellCandidates.Intersect(candidates).Count() is 1)
            {
                yield return index;
            }
        }
    }

    private static IEnumerable<int> GetSharedIndices(Puzzle puzzle, Cell cell, Cell cellOne, Cell cellTwo)
    {
        /*
            Cases:
            - Cells row/column intersect, providing a single cell of intersection
            - Cell one row/column intersects with cell two box, providing up to three unsolved cells of intersection
            - Same as above but starting with cell two.
            - There are three possibilites: 1, 3, or 6 cells

            The key for the second case if if the the cells are in the same row or column of boxes.
            The pincer that doesn't match the
        */

        bool boxMatch = cell.Box == cellOne.Box || cell.Box == cellTwo.Box;

        bool cellOneSameRow = cell.Row == cellOne.Row;
        bool cellOneSameColumn = cell.Column == cellOne.Column;

        var (higher, lower) = cellOne > cellTwo ? (cellOne, cellTwo) : (cellTwo, cellOne);

        // case for row/column intersection (one cell)
        if (!boxMatch)
        {
            int cellIndex = Puzzle.GetIndexForRowColumn(higher.Row, lower.Column);
            yield return cellIndex;
            yield break;
        }

        int cellOneBoxColumn = cellOne.Box % 3;
        int cellOneBoxRow = cellOne.Box / 3;
        int cellTwoBoxColumn = cellTwo.Box % 3;
        int cellTwoBoxRow = cellTwo.Box / 3;

        bool boxRowsMatch = cellOneBoxRow == cellTwoBoxRow;
        bool boxColumnsMatch = cellOneBoxColumn == cellTwoBoxColumn;

        // case where a pincer cell is in same box as root cell
        if (boxRowsMatch)
        {
            IEnumerable<int> rowOneIndices = Puzzle.GetRowIndices(cellOne.Row);
            IEnumerable<int> cellOneRowMatching = rowOneIndices.Where(x => Puzzle.BoxByIndices[x] == cellTwo.Box && !(puzzle.IsCellSolved(x) || x == cell));

            foreach (int rowOneIndex in cellOneRowMatching)
            {
                yield return rowOneIndex;
            }

            IEnumerable<int> rowTwoIndices = Puzzle.GetRowIndices(cellTwo.Row);
            IEnumerable<int> cellTwoRowMatching = rowTwoIndices.Where(x => Puzzle.BoxByIndices[x] == cellOne.Box && !(puzzle.IsCellSolved(x) || x == cell));

            foreach (int cellTwoIndex in cellTwoRowMatching)
            {
                yield return cellTwoIndex;
            }
        }
        else if (boxColumnsMatch)
        {
            IEnumerable<int> columnOneIndices = Puzzle.GetColumnIndices(cellOne.Column);
            IEnumerable<int> cellOneColumnMatching = columnOneIndices.Where(x => Puzzle.BoxByIndices[x] == cellTwo.Box && !(puzzle.IsCellSolved(x) || x == cell) );

            foreach (int columnOneIndex in cellOneColumnMatching)
            {
                yield return columnOneIndex;
            }

            IEnumerable<int> columnTwoIndices = Puzzle.GetColumnIndices(cellTwo.Column);
            IEnumerable<int> cellTwoColumnMatching = columnTwoIndices.Where(x => Puzzle.BoxByIndices[x] == cellOne.Box && !(puzzle.IsCellSolved(x) || x == cell));

            foreach (int cellTwoIndex in cellTwoColumnMatching)
            {
                yield return cellTwoIndex;
            }
        }

    }
}
