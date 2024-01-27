// using System.Diagnostics;
// using System.Diagnostics.CodeAnalysis;
// using System.Runtime.CompilerServices;

// namespace Sudoku;

// // Hidden singles: One logically absent value in a row or column, based on 
// // values being present in an adjacent row or column.
// // Example:
// // Solved cell: r3:c1; 8
// // Solved by: HiddenSinglesSolver:RowSolver
// //  *
// //  0 0 2 | 0 3 0 | 0 0 8
// //  0 0 0 | 0 0 8 | 0 0 0
// //  8 3 1 | 0 2 0 | 0 0 0*

// public class HiddenSinglesSolver : ISolver
// {
//     public bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         for (int i = 0; i < 9; i++)
//         {
//             if (TrySolveBox(i, puzzle, out solution))
//             {
//                 return true;
//             }
//         }
        
//         solution = default;
//         return false;
//     }

//     private bool TrySolveBox(int index, Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
//     {
//         Box box = puzzle.GetBox(index);
//         // get adjacent neighboring boxes
//         Box ahnb1 = puzzle.GetBox(box.FirstHorizontalNeighbor);
//         Box ahnb2 = puzzle.GetBox(box.SecondHorizontalNeighbor);
//         Box avnb1 = puzzle.GetBox(box.FirstVerticalNeighbor);
//         Box avnb2 = puzzle.GetBox(box.SecondVerticalNeighbor);

//         NeighborBoxes neighbors = new([ ahnb1, ahnb2 ], [ avnb1, avnb2 ]);

//         if (TrySolveRowOneCellUnsolved(puzzle, box, neighbors, out solution))
//         {
//             return true;
//         }

//         if (TrySolveColumnOneCellUnsolved(puzzle, box, neighbors, out solution))
//         {
//             Console.WriteLine("TrySolveColumnOneCellUnsolved");
//             return true;
//         }

//         for (int i = 0; i < 9; i++)
//         {
//             if (TrySolveCell(puzzle, box, i, out solution ))
//             {
//                 return true;
//             }
//         }

//         solution = default;
//         return false;
//     }

//     private bool TrySolveRowOneCellUnsolved(Puzzle puzzle, Box box, NeighborBoxes neighbors, [NotNullWhen(true)] out Solution? solution)
//     {
//         // Try for three rows
//         for (int i = 0; i < 3; i++)
//         {
//             // get (3) row values
//             int[] row = box.GetRowValues(i, puzzle).ToArray();

//             // test for one cell unsolved in row
//             bool oneUnsolved = row.Count(num => num is 0) is 1;

//             if (oneUnsolved)
//             {
//                 // find which cell is unsolved within the row
//                 int unsolvedIndex = Array.IndexOf(row, 0);
//                 // find within the box
//                 int boxCellIndex = (i * 3) + unsolvedIndex;
//                 // find within the puzzle
//                 int cellIndex = box.CellsForCells[boxCellIndex];
//                 // find candidates for that cell
//                 List<int> candidates = puzzle.Candidates[cellIndex];

//                 // find neighbors
//                 int two = (i + 1) % 3;
//                 int three = (i + 2) % 3;

//                 NeighborRows neighborRows = new(neighbors.Horizontal[0].GetRowValues(two, puzzle),
//                                                 neighbors.Horizontal[0].GetRowValues(three,puzzle),
//                                                 neighbors.Horizontal[1].GetRowValues(two,puzzle),
//                                                 neighbors.Horizontal[1].GetRowValues(three,puzzle));

//                 if (TrySolveRowOneCellUnsolvedForRow(candidates, row, neighborRows, out int value))
//                 {
//                     puzzle.Candidates[cellIndex] = [];
//                     solution = Box.GetSolutionForBox(box.Index, boxCellIndex, value, nameof(HiddenSinglesSolver));
//                     return true;
//                 }
//             }
//         }

//         solution = default;
//         return false;
//     }

//     private bool TrySolveColumnOneCellUnsolved(Puzzle puzzle, Box box, NeighborBoxes neighbors, [NotNullWhen(true)] out Solution? solution)
//     {
//         // Try for three columns
//         for (int i = 0; i < 3; i++)
//         {
//             // get (3) row values
//             int[] column = box.GetColumnValues(i, puzzle).ToArray();

//             // test for one cell unsolved in row
//             bool oneUnsolved = column.Count(num => num is 0) is 1;

//             if (oneUnsolved)
//             {
//                 // find which cell is unsolved within the row
//                 int unsolvedIndex = Array.IndexOf(column, 0);
//                 // find within the box
//                 int boxCellIndex = (i * 3) + unsolvedIndex;
//                 // find within the puzzle
//                 int cellIndex = box.CellsForCells[boxCellIndex];
//                 // find candidates for that cell
//                 List<int> candidates = puzzle.Candidates[cellIndex];

//                 // find neighbors
//                 int two = (i + 1) % 3;
//                 int three = (i + 2) % 3;

//                 NeighborRows neighborRows = new(neighbors.Vertical[0].GetColumnValues(two, puzzle),
//                                                 neighbors.Vertical[0].GetColumnValues(three,puzzle),
//                                                 neighbors.Vertical[1].GetColumnValues(two,puzzle),
//                                                 neighbors.Vertical[1].GetColumnValues(three,puzzle));

//                 if (TrySolveRowOneCellUnsolvedForRow(candidates, column, neighborRows, out int value))
//                 {
//                     puzzle.Candidates[cellIndex] = [];
//                     solution = Box.GetSolutionForBox(box.Index, boxCellIndex, value, nameof(HiddenSinglesSolver));
//                     return true;
//                 }
//             }
//         }

//         solution = default;
//         return false;
//     }    

//     // One cell unsolved in row
//     private static bool TrySolveRowOneCellUnsolvedForRow(IEnumerable<int> candidates, IEnumerable<int> row, NeighborRows neighbors, out int value)
//     {
//         // Union each row -- all the values that cannot be in those rows within the box
//         IEnumerable<int> neighborRow2Union = neighbors.Box1Row2.Union(neighbors.Box2Row2);
//         IEnumerable<int> neighborRow3Union = neighbors.Box1Row3.Union(neighbors.Box2Row3);
        
//         // Determine matching candidates
//         IEnumerable<int> row2Candidates = neighborRow2Union.Intersect(candidates);
//         IEnumerable<int> row3Candidates = neighborRow3Union.Intersect(candidates);

//         // If there is one matching candidate, then we have a solution
//         IEnumerable<int> row1Candidates = row2Candidates.Intersect(row3Candidates);

//         if (row1Candidates.ValueIfCountExact(1, out value))
//         {
//             return true;
//         }

//         return false;
//     }

//     private static bool TrySolveCell(Puzzle puzzle, Box box, int cell, [NotNullWhen(true)] out Solution? solution)
//     {
//         // Get cell in puzzle
//         int column = box.ColumnsForCells[cell];
//         int cellInPuzzle = box.CellsForCells[cell];
//         int cellInColumn = Puzzle.GetCellForColumn(cellInPuzzle);
//         IEnumerable<int> columnCells = puzzle.GetCellIndicesForColumn(column);

//         if (box.Index is 8 && cell > 6)
//         {

//         }

//         // Candidate unique in column
//         if (TrySolveCandidateUniqueInLine(puzzle, cellInPuzzle, cellInColumn, columnCells, out int value))
//         {
//             solution = Box.GetSolutionForBox(box.Index, cell, value, nameof(HiddenSinglesSolver));
//             return true;
//         }
        
//         solution = default;
//         return false;
//     }

//     private static bool TrySolveCandidateUniqueInLine(Puzzle puzzle, int index, int indexInLine, IEnumerable<int> cells, out int value)
//     {
//         List<int> candidates = puzzle.Candidates[index];
//         IEnumerable<int> candidates2 = candidates;
//         int count = 0;
//         value = 0;

//         foreach (int cell in cells)
//         {
//             if (indexInLine == count)
//             {
//                 count++;
//                 continue;
//             }

//             List<int> cellCandidates = puzzle.Candidates[cell];
//             candidates2 = candidates2.Except(cellCandidates);

//             if (candidates2.Count() is 0)
//             {
//                 return false;
//             }

//             count++;
//         }

//         if (candidates2.Count() is 1)
//         {
//             value = candidates2.Single();
//             return true;
//         }

//         return false;
//     }

// }

// record NeighborRows(IEnumerable<int> Box1Row2, IEnumerable<int> Box1Row3, IEnumerable<int> Box2Row2, IEnumerable<int> Box2Row3);

// record NeighborBoxes(Box[] Horizontal, Box[] Vertical);
