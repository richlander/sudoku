namespace Sudoku;
public partial class Puzzle
{

    // Board navigation
    public static IEnumerable<int> GetRowIndices(int index) => IndicesByRow.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetColumnIndices(int index) => IndicesByColumn.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetBoxIndices(int index) => IndicesByBox.Skip(index * 9).Take(9);

    public static Cell GetCellForIndex(int index) => new(
        index,
        index / 9,
        index % 9,
        BoxByIndices[index],
        BoxRowByIndices[index],
        BoxColumnByIndices[index],
        BoxIndices[index]
    );

    // Solution handling
    public static void AttachToLastSolution(Solution solution, Solution nextSolution)
    {
        Solution? s = solution;
        while (s.Next is not null)
        {
            s = s.Next;
        }

        solution.Next = nextSolution;
    }

    // Board solving
    public bool TryFindValueAppearsOnce(Cell cell, IEnumerable<int> line, int uniqueValue, out int uniqueIndex)
    {
        uniqueIndex = -1;
        int matchIndex = -1;
        foreach (int neighborIndex in line.Where(x => x !=cell))
        {
            IEnumerable<int> neighborCandidates = GetCellCandidates(neighborIndex);
            if (!neighborCandidates.Contains(uniqueValue))
            {
                continue;
            }
            
            if (matchIndex > -1)
            {
                return false;
            }

            matchIndex = neighborIndex;
        }

        uniqueIndex = matchIndex;
        return true;
    }

    public bool TryFindCellCandidatesAppearOnce(Cell cell, IEnumerable<int> line, out int uniqueIndex)
    {
        IReadOnlyList<int> cellCandidates = GetCellCandidates(cell);
        uniqueIndex = -1;
        int matchIndex = -1;
        foreach (int neighborIndex in line.Where(x => x !=cell))
        {
            IReadOnlyList<int> neighborCandidates = GetCellCandidates(neighborIndex);
            if (neighborCandidates.Count is not 2 || 
                neighborCandidates.Intersect(cellCandidates).Count() is not 2)
            {
                continue;
            }
            
            if (matchIndex > -1)
            {
                return false;
            }

            matchIndex = neighborIndex;
        }

        if (matchIndex is -1)
        {
            return false;
        }

        uniqueIndex = matchIndex;
        return true;
    }
}