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

    public Form1()
    {
        InitializeComponent();
    }

    private void picture_MouseClick(object sender, MouseEventArgs e)
    {
        var collusion = Collusion(e.X, e.Y);
        if (collusion != null)
        {
            UnselectAll();
            var index = _vertices.FindIndex(x => x.Name == collusion.Name);
            if (index != -1)
            {
                var v = _vertices[index];
                v.Selected = true;
                _vertices[index] = v;
            }
        }
        else
        {
            _vertices.Add(new Vertex(e.X, e.Y, _vertices.Count.ToString()));
        }

        picture.Invalidate();
    }

    private void OnClickInCollusion(int index)
    {
        UnselectAll();
        if (index != -1)
        {
            var v = _vertices[index];
            v.Selected = true;
            _vertices[index] = v;
        }
    }

    private void OnClickOutCollusion(int x, int y)
    {
        
        _vertices.Add(new Vertex(x, y, _vertices.Count.ToString()));
    }

    private void picture_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        var pen = new Pen(Color.DarkViolet, 2);
        var format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        for (var index = 0; index < _vertices.Count; index++)
        {
            var v = _vertices[index];
            var brush = !_isDragging ? v.Selected ? Brushes.Red : Brushes.Gray :
                int.Parse(v.Name) == _startSelectedIndex ? Brushes.Blue : Brushes.Gray;
            g.FillEllipse(brush, v.X - _radius / 2, v.Y - _radius / 2, _radius, _radius);
            g.DrawEllipse(pen, v.X - _radius / 2, v.Y - _radius / 2, _radius, _radius);

            g.DrawString(v.Name, this.Font, Brushes.Black, v.X, v.Y, format);
        }

        if (_isDragging)
        {
            if (_startSelected != null)
            {
                using (var p = new Pen(Color.Blue, 2))
                {
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawLine(p, _startSelected.X, _startSelected.Y, _thisMousePosition.X, _thisMousePosition.Y);
                }
            }
        }
    }

    private Vertex? Collusion(int x, int y)
    {
        foreach (var v in _vertices)
        {
            double distance = Math.Pow(v.X - x, 2) + Math.Pow(v.Y - y, 2);
            if (distance <= (_radius * _radius))
            {
                return v;
            }
        }

        return null;
    }

    private int CollusionIndex(int x, int y)
    {
        int index = 0;
        foreach (var v in _vertices)
        {
            double distance = Math.Pow(v.X - x, 2) + Math.Pow(v.Y - y, 2);
            if (distance <= (_radius * _radius))
            {
                return index;
            }

            index++;
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
                OnClickInCollusion(_selectedIndex);
            }
        }
        else
        {
            if (_selectedVertex == null && _startSelected != null)
            {
                //TODO тут создать уже соедененное
                Console.WriteLine("Создать соедененное");
            }
            else if (_startSelected == _selectedVertex)
            
            {
                OnClickOutCollusion(e.X, e.Y);
            }
        }

        picture.Invalidate();
    }
}