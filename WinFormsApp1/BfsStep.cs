namespace WinFormsApp1;

public record BfsStep(int Id, int Depth, IReadOnlyList<int> Neighbors);