using System;
using System.Collections.Generic;

namespace mEditor.Models.Buffer;

public sealed class LineIndex
{
    private readonly List<int> _lineStarts = new() { 0 };

    public int LineCount => _lineStarts.Count;

    public void Rebuild(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lineStarts.Clear();
        _lineStarts.Add(0);

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
                _lineStarts.Add(i + 1);
        }
    }

    public int GetLineStart(int line)
    {
        if ((uint)line >= (uint)_lineStarts.Count)
            throw new ArgumentOutOfRangeException(nameof(line));

        return _lineStarts[line];
    }

    public int GetLineFromPosition(int position, int textLength)
    {
        if (position < 0 || position > textLength)
            throw new ArgumentOutOfRangeException(nameof(position));

        int index = _lineStarts.BinarySearch(position);

        if (index >= 0)
            return index;

        return ~index - 1;
    }

    public (int Line, int Column) GetLineColumn(
        int position,
        int textLength)
    {
        int line = GetLineFromPosition(position, textLength);

        return (line, position - _lineStarts[line]);
    }

    
    public (int Start, int Length) GetLineRange(
        int line,
        string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if ((uint)line >= (uint)_lineStarts.Count)
            throw new ArgumentOutOfRangeException(nameof(line));

        int start = _lineStarts[line];

        int end = line + 1 < _lineStarts.Count
            ? _lineStarts[line + 1]
            : text.Length;

        if (end > start && text[end - 1] == '\n')
        {
            end--;

            if (end > start && text[end - 1] == '\r')
                end--;
        }

        return (start, end - start);
    }

}
