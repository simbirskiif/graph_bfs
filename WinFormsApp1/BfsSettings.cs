namespace WinFormsApp1;

public class BfsSettings : AlgorithmSettings
{
    public List<(int From, int To)> Edges { get; set; } = [];

    public List<(int Id, int X, int Y)> Vertices { get; set; } = [];

    public int StartVertexId { get; set; } = -1;

    public BfsSettings()
    {
        Name = "Обход графа в ширину (BFS)";
        Description =
            "BFS обходит граф уровень за уровнем, начиная от стартовой вершины.\n" +
            "Использует очередь. Гарантирует кратчайший путь в невзвешенном графе.\n\n" +
            "Управление:\n" +
            "  ЛКМ на пустом месте — создать вершину\n" +
            "  Drag вершина → вершина — создать ребро\n" +
            "  Drag вершина → пустота — создать вершину и ребро\n" +
            "  ЛКМ на вершине — выделить\n" +
            "  Space — запустить BFS от выделенной вершины\n" +
            "  → / ← — следующий / предыдущий шаг\n" +
            "  Escape — сбросить BFS";
    }
}