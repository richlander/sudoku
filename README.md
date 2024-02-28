# Sudoku puzzle solver

This project is intended to solve relatively simple Sudoku puzzles using well understood logical strategies. Given a correctly defined Sudoku puzzle, these strategies always produce valid results.

Supported [strategies](https://www.sudokuwiki.org/Strategy_Families):

- [Solved cells](Solver/Solvers/SolvedCellsSolver.cs)
- [Hidden singles](Solver/Solvers/HiddenSinglesSolver.cs)
- [Hidden pairs](Solver/Solvers/HiddenPairsSolver.cs)
- [Naked pairs](Solver/Solvers/NakedPairsSolver.cs)
- [Pointed pairs](Solver/Solvers/PointedPairsSolver.cs)
- [Box/line reduction](Solver/Solvers/BoxLineReductionSolver.cs)
- [X-Wing](Solver/Solvers/XWingSolver.cs)

The project may be expanded over time, however, the primary motivation of the project has been to explore various C# features.

The [solver](https://www.sudokuwiki.org/sudoku.htm) at [SudokuWiki](https://www.sudokuwiki.org/) was used as a baseline and (invaluable) puzzle debugger for these implementations. The implemented solvers are intended to produce the exact same results as SudokuWiki. This is done by adopting the same cell by cell search (left to right, up to down) and solver order.

## Puzzle resources

The following resources were found to be useful.

- https://www.sudokuwiki.org/
- https://www.printable-sudoku-puzzles.com/wfiles/
- https://www.reddit.com/r/sudoku/wiki/index/
- https://github.com/dimitri/sudoku
- http://sudopedia.enjoysudoku.com/Valid_Test_Cases.html
