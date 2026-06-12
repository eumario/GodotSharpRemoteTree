namespace Godot.Sharp.RemoteTree.Inspector.BaseTypes;

public partial class LayersGrid : Control
{
    private static Color FontColor = Color.FromHtml("ffffffbf");
    private static Color FontDisabledColor = Color.FromHtml("ffffff59");
    private static Color FontHoverColor = Color.FromHtml("ffffffd9");
    private static Color HilightColor = Color.FromHtml("569eff46");
    private static Color HilightDisabledColor = Color.FromHtml("2b4f80a3");
    
    private const uint HoveredIndexNone = UInt32.MaxValue;
    private List<Rect2> _flagRects = [];
    private Rect2 _expandRect;
    private bool _expandHovered = false;
    private bool _expanded = false;
    private int _expansionRows = 0;
    private uint _hoveredIndex = HoveredIndexNone;
    private bool _dragging = false;
    private bool _draggingValueToSet = false;
    private bool _readOnly = false;
    // private int _renamedLayerIndex = -1;
    // private PopupMenu _layerRename = null!;
    // private ConfirmationDialog _renameDialog = null!;
    // private LineEdit _renameDialogText = null!;
    
    
    public uint Value { get; set; }
    public int LayerGroupSize { get; set; }
    public uint LayerCount { get; set; }
    public List<string> Names { get; set; } = [];
    public List<string> Tooltips { get; set; } = [];

    private void _RenamePressed(int menu) { }
    private void _RenameOperationConfirm() { }

    private void _UpdateHovered(Vector2 pos)
    {
        var expandWasHovered = _expandHovered;
        _expandHovered = _expandRect.HasPoint(pos);
        if (_expandHovered != expandWasHovered)
        {
            QueueRedraw();
        }

        if (!_expandHovered)
        {
            for (var i = 0; i < _flagRects.Count; i++)
            {
                if (!_flagRects[i].HasPoint(pos)) continue;
                _hoveredIndex = (uint)i;
                QueueRedraw();
                return;
            }
        }
        
        _hoveredIndex = HoveredIndexNone;
        QueueRedraw();
    }

    private void _OnHoverExit()
    {
        if (_expandHovered)
        {
            _expandHovered = false;
            QueueRedraw();
        }

        if (_hoveredIndex != HoveredIndexNone)
        {
            _hoveredIndex = HoveredIndexNone;
            QueueRedraw();
        }

        if (_dragging)
            _dragging = false;
    }


    private void _UpdateFlag(bool replace)
    {
        if (_hoveredIndex != HoveredIndexNone)
        {
            if (replace)
            {
                if (Value == 1u << (int)_hoveredIndex)
                {
                    Value = ~Value;
                }
                else
                {
                    Value = 1u << (int)_hoveredIndex;
                }
            }
        } else if (_expandHovered)
        {
            _expanded = !_expanded;
            UpdateMinimumSize();
            QueueRedraw();
        }
    }

    private Vector2 GetGridSize()
    {
        var font = GetThemeFont("font", "Label");
        var fontSize = GetThemeFontSize("font_size", "Label");
        return new Vector2(0, font.GetHeight(fontSize) * 3);
    }

    public override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationDraw:
                var gridSize = GetGridSize();
                gridSize.X = GetSize().X;

                _flagRects.Clear();
                
                var prevExpansionRows = _expansionRows;
                _expansionRows = 0;

                var bSize = (gridSize.Y * 80 / 100) / 2;
                var h = bSize * 2 + 1;

                var color = _readOnly ? HilightDisabledColor : HilightColor;
                var textColor = _readOnly ? FontDisabledColor : FontColor;
                textColor.A *= 0.5f;
                var textColorOn = _readOnly ? FontDisabledColor : FontHoverColor;
                textColorOn.A *= 0.7f;

                var vofs = (gridSize.Y - h) / 2;
                uint layerIndex = 0;

                var arrowPos = Vector2.Zero;
                var blockOfs = new Vector2(4, vofs);

                var font = GetThemeFont("font", "Label");
                var fontSize = GetThemeFontSize("font_size", "Label");

                while (true)
                {
                    var ofs = blockOfs;

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < LayerGroupSize; j++)
                        {
                            var on = (Value & (1u << (int)layerIndex)) >= 1;
                            var rect2 = new Rect2(ofs, new Vector2(bSize, bSize));
                            color.A = on ? 0.6f : 0.2f;
                            if (layerIndex == _hoveredIndex)
                            {
                                color.A += 0.15f;
                            }

                            DrawRect(rect2, color);
                            _flagRects.Add(rect2);

                            var offset = Vector2.Zero;
                            offset.Y = rect2.Size.Y * 0.75f;
                            
                            DrawString(font, rect2.Position + offset,$"{layerIndex + 1}", HorizontalAlignment.Center, rect2.Size.X, fontSize, on ? FontHoverColor : FontColor);

                            ofs.X += bSize + 1;

                            ++layerIndex;
                        }

                        ofs.X = blockOfs.X;
                        ofs.Y += bSize + 1;
                    }

                    if (layerIndex > LayerCount)
                    {
                        if (_flagRects.Count != 0 && (_expansionRows == 0))
                        {
                            var lastRect = _flagRects[^1];
                            arrowPos = lastRect.End;
                        }

                        break;
                    }

                    var blockSizeX = LayerGroupSize * (bSize + 1);
                    blockOfs.X += blockSizeX + 3;

                    if (blockOfs.X + blockSizeX + 12 > gridSize.X)
                    {
                        if (_flagRects.Count != 0 && _expansionRows == 0)
                        {
                            var lastRect = _flagRects[^1];
                            arrowPos = lastRect.End;
                        }

                        ++_expansionRows;

                        if (_expanded)
                        {
                            blockOfs.X = 4;
                            blockOfs.Y += 2 * (bSize + 1) + 3;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if ((_expansionRows != prevExpansionRows) && _expanded)
                {
                    UpdateMinimumSize();
                }

                if (_expansionRows == 0 && layerIndex == LayerCount)
                    break;

                var arrow = GetThemeIcon("arrow", "Tree");

                var arrowColor = HilightColor;
                arrowColor.A = _expandHovered ? 1.0f : 0.6f;

                arrowPos.X += 2.0f;
                arrowPos.Y -= arrow.GetHeight();

                var arrowSize = arrow.GetSize();
                if (_expanded)
                    arrowSize.Y *= -1.0f;
                
                var arrowDrawRect = new Rect2(arrowPos, arrowSize);
                _expandRect = arrowDrawRect;

                var ci = GetCanvasItem();
                arrow.DrawRect(ci, arrowDrawRect, false, arrowColor);
                
                break;
            case NotificationMouseExit:
                _OnHoverExit();
                break;
        }
    }
    
    public void SetReadOnly(bool readOnly) { _readOnly = readOnly; }

    public Vector2 GetMinimumSize()
    {
        var minSize = GetGridSize();

        if (_expanded)
        {
            var bsize = (minSize.Y * 80 / 100) / 2;
            for (var i = 0; i < _expansionRows; ++i)
            {
                minSize.Y += 2 * (bsize + 1) + 3;
            }
        }

        return minSize;
    }


    public string GetTooltip(Vector2 pos)
    {
        for (var i = 0; i < _flagRects.Count; i++)
        {
            if (i < Tooltips.Count && _flagRects[i].HasPoint(pos))
            {
                return Tooltips[i];
            }
        }

        return string.Empty;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (_readOnly)
            return;

        switch (@event)
        {
            case InputEventMouseMotion mm:
            {
                _UpdateHovered(mm.Position);
                if (_dragging && _hoveredIndex != HoveredIndexNone &&
                    _draggingValueToSet != (Value & (1u << (int)_hoveredIndex)) >= 1)
                {
                    Value ^= 1u << (int)_hoveredIndex;
                    QueueRedraw();
                }

                return;
            }
            case InputEventMouseButton mb when mb.GetButtonIndex() == MouseButton.Left && mb.IsPressed():
            {
                _UpdateHovered(mb.Position);
                bool replaceMode = mb.IsCommandOrControlPressed();
                _UpdateFlag(replaceMode);
                if (!replaceMode && _hoveredIndex != HoveredIndexNone)
                {
                    _dragging = true;
                    _draggingValueToSet = (Value & (1u << (int)_hoveredIndex)) >= 1;
                }

                break;
            }
            case InputEventMouseButton mb when mb.GetButtonIndex() == MouseButton.Left && !mb.IsPressed():
            {
                _dragging = false;
                break;
            }
            case InputEventMouseButton mb when mb.GetButtonIndex() == MouseButton.Right && mb.IsPressed():
            {
                break;
            }
        }
    }
    
    public void SetFlag(uint flag) { }

    public LayersGrid()
    {
        
    }
}