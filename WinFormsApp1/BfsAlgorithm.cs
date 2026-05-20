using Microsoft.Extensions.Logging;

namespace WinFormsApp1;

public class BfsAlgorithm : IBfsAlgorithm
{
    private readonly ILogger<BfsAlgorithm> _logger;

    public BfsAlgorithm(ILogger<BfsAlgorithm> logger)
    {
        _logger = logger;
    }

    public List<BfsStep> Run(AlgorithmSettings settings)
    {
        if (settings is not BfsSettings bfsSettings)
        {
            _logger.LogError("Переданы настройки неверного типа: {Type}", settings.GetType().Name);
            throw new ArgumentException("Ожидались BfsSettings");
        }

        _logger.LogInformation("Запуск BFS. Вершин: {V}, рёбер: {E}, старт: {S}",
            bfsSettings.VertexIds.Count, bfsSettings.Edges.Count, bfsSettings.StartVertexId);

        var graph = BuildGraph(bfsSettings);
        var steps = graph.BfsSteps(bfsSettings.StartVertexId).ToList();

        _logger.LogInformation("BFS завершён. Шагов: {Steps}", steps.Count);
        return steps;
    }

    public void SaveToFile(string filePath, List<BfsStep> steps, BfsSettings settings)
    {
        try
        {
            _logger.LogInformation("Сохранение в файл: {Path}", filePath);

            using var writer = new StreamWriter(filePath);
            // Вершины
            writer.WriteLine("VERTICES");
            writer.WriteLine(string.Join(",", settings.VertexIds));

            // Рёбра
            writer.WriteLine("EDGES");
            foreach (var (from, to) in settings.Edges)
                writer.WriteLine($"{from};{to}");

            // Стартовая вершина
            writer.WriteLine("START");
            writer.WriteLine(settings.StartVertexId);

            // Шаги BFS
            writer.WriteLine("STEPS");
            foreach (var step in steps)
                writer.WriteLine($"{step.Id};{step.Depth};{string.Join(",", step.Neighbors)}");

            _logger.LogInformation("Сохранение завершено успешно");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении файла: {Path}", filePath);
            throw;
        }
    }

    public BfsSettings LoadFromFile(string filePath)
    {
        try
        {
            _logger.LogInformation("Загрузка из файла: {Path}", filePath);

            var settings = new BfsSettings();
            var lines = File.ReadAllLines(filePath);
            var section = "";

            foreach (var line in lines)
            {
                if (line is "VERTICES" or "EDGES" or "START" or "STEPS")
                {
                    section = line;
                    continue;
                }

                switch (section)
                {
                    case "VERTICES":
                        if (!string.IsNullOrWhiteSpace(line))
                            settings.VertexIds = line.Split(',')
                                .Select(int.Parse).ToList();
                        break;

                    case "EDGES":
                        var parts = line.Split(';');
                        if (parts.Length == 2)
                            settings.Edges.Add((int.Parse(parts[0]), int.Parse(parts[1])));
                        break;

                    case "START":
                        if (int.TryParse(line, out int startId))
                            settings.StartVertexId = startId;
                        break;
                }
            }

            settings.CollectionSize = settings.VertexIds.Count;
            _logger.LogInformation("Загрузка завершена. Вершин: {V}, рёбер: {E}",
                settings.VertexIds.Count, settings.Edges.Count);

            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла: {Path}", filePath);
            throw;
        }
    }

    private static Graph BuildGraph(BfsSettings settings)
    {
        var graph = new Graph();
        foreach (var id in settings.VertexIds)
            graph.AddVertex(id);
        foreach (var (from, to) in settings.Edges)
            graph.AddEdge(from, to);
        return graph;
    }
}