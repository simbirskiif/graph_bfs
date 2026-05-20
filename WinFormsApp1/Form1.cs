using System.Numerics;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private readonly List<Vertex> _vertices = [];
    private readonly int _radius = 30;

    private bool _isDragging = false;
    private Vertex? _startSelected = null;
    private int _startSelectedIndex = -1;
    private Vertex? _selectedVertex = null;
    private int _selectedIndex = -1;
    private Vector2 _thisMousePosition = Vector2.Zero;

    private readonly Graph _graph = new Graph();

    private List<BfsStep> _bfsSteps = [];

    private int _currentBfsStep = -1;


    public Form1()
    {
        InitializeComponent();
        this.KeyPreview = true;
        this.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Space)
            {
                if (_selectedVertex != null)
                {
                    _bfsSteps = _graph.BfsSteps(_selectedIndex!).ToList();
                    _currentBfsStep = 0;
                    picture.Invalidate();
                }
            }

            else if (e.KeyCode == Keys.Right)
            {
                if (_currentBfsStep < _bfsSteps.Count - 1)
                {
                    _currentBfsStep++;
                }

                picture.Invalidate();
            }

            else if (e.KeyCode == Keys.Left)
            {
                if (_currentBfsStep > 0)
                {
                    _currentBfsStep--;
                }

                picture.Invalidate();
            }
        };
    }

    private void picture_MouseClick(object sender, MouseEventArgs e)
    {
        var collusion = Collusion(e.X, e.Y);
        if (collusion != null)
        {
            UnselectAll();
            var index = _vertices.FindIndex(x => x.Id == collusion.Id);
            if (index != -1)
            {
                var v = _vertices[index];
                v.Selected = true;
                _vertices[index] = v;
            }
        }
        else
        {
            _vertices.Add(new Vertex(e.X, e.Y, _vertices.Count));
        }

        picture.Invalidate();
    }

    private void OnClickInCollusion(int index)
    {
        UnselectAll();
        if (index != -1)
        {
            _vertices[index].Selected = true;
            _currentBfsStep = -1;
        }
        picture.Invalidate();
    }

    private void OnClickOutCollusion(int x, int y)
    {
        UnselectAll();
        var graphVertex = _graph.AddVertex();
        _vertices.Add(new Vertex(x, y, graphVertex.Id));
    }

    private void picture_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var pen = new Pen(Color.DarkViolet, 2);
        var format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        DrawEdges(g);
        DrawVertices(g, format, pen);

        if (_isDragging && _startSelected != null)
        {
            using var p = new Pen(Color.Blue, 2);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawLine(p, _startSelected.X, _startSelected.Y,
                _thisMousePosition.X, _thisMousePosition.Y);
        }
    }

    private void DrawVertices(Graphics g, StringFormat format, Pen pen)
    {
        // Что видно на текущем шаге BFS
        var visited = _bfsSteps.Take(_currentBfsStep + 1).Select(s => s.Id).ToHashSet();
        var current = _currentBfsStep >= 0 ? _bfsSteps[_currentBfsStep].Id : -1;
        var frontier = _currentBfsStep >= 0
            ? _bfsSteps[_currentBfsStep].Neighbors.ToHashSet()
            : new HashSet<int>();

        foreach (var v in _vertices)
        {
            Brush fill;

            if (_currentBfsStep < 0)
            {
                // BFS не запущен — обычная отрисовка
                fill = !_isDragging
                    ? v.Selected ? Brushes.Red : Brushes.Gray
                    : v.Id == _startSelected?.Id
                        ? Brushes.Blue
                        : Brushes.Gray;
            }
            else
            {
                fill = v.Id == current ? Brushes.Orange // текущая
                    : frontier.Contains(v.Id) ? Brushes.Yellow // соседи текущей
                    : visited.Contains(v.Id) ? Brushes.LightGreen // уже посещённые
                    : Brushes.Gray; // ещё не дошли
            }

            g.FillEllipse(fill, v.X - _radius / 2, v.Y - _radius / 2, _radius, _radius);
            g.DrawEllipse(pen, v.X - _radius / 2, v.Y - _radius / 2, _radius, _radius);
            g.DrawString(v.Id.ToString(), this.Font, Brushes.Black, v.X, v.Y, format);
        }
    }

    private void DrawEdges(Graphics g)
    {
        var current = _currentBfsStep >= 0 ? _bfsSteps[_currentBfsStep].Id : -1;
        var frontier = _currentBfsStep >= 0
            ? _bfsSteps[_currentBfsStep].Neighbors.ToHashSet()
            : new HashSet<int>();

        using var penNormal = new Pen(Color.DarkSlateGray, 2);
        using var penFrontier = new Pen(Color.Orange, 3);

        foreach (var edge in _graph.Edges)
        {
            var a = _vertices.FirstOrDefault(v => v.Id == edge.From);
            var b = _vertices.FirstOrDefault(v => v.Id == edge.To);
            if (a is null || b is null) continue;

            // Подсветить рёбра от текущей вершины к её соседям
            bool isActive = (edge.From == current && frontier.Contains(edge.To)) ||
                            (edge.To == current && frontier.Contains(edge.From));

            g.DrawLine(isActive ? penFrontier : penNormal, a.X, a.Y, b.X, b.Y);
        }
    }

    private Vertex? Collusion(int x, int y)
    {
        int i = CollusionIndex(x, y);
        return i >= 0 ? _vertices[i] : null;
    }

    private int CollusionIndex(int x, int y)
    {
        for (int i = 0; i < _vertices.Count; i++)
        {
            var v = _vertices[i];
            double d = Math.Pow(v.X - x, 2) + Math.Pow(v.Y - y, 2);
            if (d <= _radius * _radius) return i;
        }

        return -1;
    }

    private void UnselectAll()
    {
        _vertices.ForEach(v => v.Selected = false);
    }

    private void picture_MouseDown(object sender, MouseEventArgs e)
    {
        _isDragging = true;
        _startSelectedIndex = CollusionIndex(e.X, e.Y);
        _startSelected = _startSelectedIndex >= 0 ? _vertices[_startSelectedIndex] : null;
    }

    private void picture_MouseMove(object sender, MouseEventArgs e)
    {
        _thisMousePosition = new Vector2(e.X, e.Y);
        picture.Invalidate();
    }

    private void picture_MouseUp(object sender, MouseEventArgs e)
    {
        _isDragging = false;
        _selectedIndex = CollusionIndex(e.X, e.Y);
        _selectedVertex = _selectedIndex >= 0 ? _vertices[_selectedIndex] : null;

        if (_selectedVertex != null && _startSelected != null)
        {
            if (_selectedVertex == _startSelected)
            {
                //выделить
                OnClickInCollusion(_selectedIndex);
            }
            else
            {
                // создать ребро
                _graph.AddEdge(_startSelected.Id, _selectedVertex.Id);
            }
        }
        else
        {
            if (_selectedVertex == null && _startSelected != null)
            {
                //новая вершина + ребро
                var graphVertex = _graph.AddVertex();
                _vertices.Add(new Vertex(e.X, e.Y, graphVertex.Id));
                _graph.AddEdge(_startSelected.Id, graphVertex.Id);
            }
            else if (_startSelected == _selectedVertex) // оба null
            {
                //  новая вершина без рёбер
                OnClickOutCollusion(e.X, e.Y);
            }
        }

        picture.Invalidate();
    }
}