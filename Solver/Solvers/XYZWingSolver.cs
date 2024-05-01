using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class XYZWingSolver : ISolver
{
    public string Name => nameof(XYZWingSolver);

    public bool IsTough => true;

    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        /*
            YWZing is a mix of naked triples (3/2/2 variant), pointed pairs, and 3/4ers of an XWing.
            It requires three cells, one with three candidates (the hinge), and then two others with
            two of those three candidates.
            One cell (the hinge cell) can see the other two cells (same column/row or box).
            The other two cannot see each other (not in the same box, column, or two).

            Whatever is the common candidate in the two ends of the Y, that candidate can be removed
            from every cell all three cells can see.
        */

        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);

        if (cellCandidates.Count != 3)
        {
            return false;
        }

        List<IEnumerable<int>> lines = Puzzle.GetLinesForCell(cell);

        foreach (int lineOne in Enumerable.Range(0, lines.Count))
        {
            foreach (int pincerOne in GetPincersInLine(puzzle, cell, cellCandidates, lines[lineOne]))
            {
                IReadOnlyList<int> pincerOneCandidates = puzzle.GetCellCandidates(pincerOne);
                Cell pincerOneCell = Puzzle.GetCellForIndex(pincerOne);

                // We now have a first pincer cell; need a second
                // The second pincer cell will not be in the same line as the first
                foreach (int lineTwo in Enumerable.Range(0, lines.Count).Where(x => x != lineOne))
                {
                    foreach (int pincerTwo in GetPincersInLine(puzzle, cell, cellCandidates, lines[lineTwo]))
                    {
                        Cell pincerTwoCell = Puzzle.GetCellForIndex(pincerTwo);

                        if (cell.Box != pincerOneCell.Box &&
                            cell.Box != pincerTwoCell.Box)
                            {
                                continue;
                            } 

                        if (pincerOneCell.Box == pincerTwoCell.Box ||
                            pincerOneCell.Row == pincerTwoCell.Row ||
                            pincerOneCell.Column == pincerTwoCell.Column)
                            {
                                continue;
                            }

                        IReadOnlyList<int> pincerTwoCandidates = puzzle.GetCellCandidates(pincerTwo);

                        if (pincerOneCandidates.Intersect(pincerTwoCandidates).Count() is 1 &&
                            cellCandidates.Intersect(pincerTwoCandidates).Count() is 2)
                        {
                            int value = pincerOneCandidates.Intersect(pincerTwoCandidates).Single();
                            List<int> wingCells = [cell, pincerOne, pincerTwo];

                            Cell otherCell = cell.Box == pincerOneCell.Box ? pincerTwoCell : pincerOneCell;
                            bool filter(int x) => cell.Box == Puzzle.GetCellForIndex(x).Box && !wingCells.Contains(x);
                            IEnumerable<int> sharedIndices = cell.Row == otherCell.Row ? Puzzle.GetRowIndices(cell.Row).Where(filter) : Puzzle.GetColumnIndices(cell.Column).Where(filter);

                            // Now search for the shared values in a shared line
                            
                            foreach (int sharedIndex in sharedIndices)
                            {
                                IReadOnlyList<int> candidates = puzzle.GetCellCandidates(sharedIndex);
                                if (candidates.Contains(value))
                                {
                                    Cell c = Puzzle.GetCellForIndex(sharedIndex);
                                    Solution s = new(c, -1, Name)
                                    {
                                        AlignedIndices = wingCells,
                                        AlignedCandidates = cellCandidates,
                                        RemovalCandidates = [value],
                                    };
                                    s.Next = solution;
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
            if (cellCandidates.Intersect(candidates).Count() is 2)
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

        int cellOneBoxColumnStart = Puzzle.BoxFirstCellIndices[cellOne.Box];
        int cellTwoBoxColumnStart = Puzzle.BoxFirstCellIndices[cellTwo.Box];

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
