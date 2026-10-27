using System;
using System.Collections.Generic;

namespace mEditor.Models.Buffer;

using System.Text;

public sealed class PieceTable
{
    private enum BufferKind
    {
        Original,
        Add
    }

    private readonly record struct Piece(
        BufferKind Buffer,
        int Start,
        int Length);

    private readonly string _originalBuffer;
    private readonly StringBuilder _addBuffer = new();
    private readonly List<Piece> _pieces = new();
    private readonly LineIndex _lineIndex = new();

    public int Length { get; private set; }

    public int LineCount => _lineIndex.LineCount;

    public PieceTable(string initialText = "")
    {
        _originalBuffer = initialText
            ?? throw new ArgumentNullException(nameof(initialText));

        Length = initialText.Length;

        if (Length > 0)
        {
            _pieces.Add(
                new Piece(BufferKind.Original, 0, Length));
        }

        _lineIndex.Rebuild(initialText);
    }

    public string GetText()
    {
        var result = new StringBuilder(Length);
        string addText = _addBuffer.ToString();

        foreach (var piece in _pieces)
        {
            string buffer = piece.Buffer == BufferKind.Original
                ? _originalBuffer
                : addText;

            result.Append(buffer, piece.Start, piece.Length);
        }

        return result.ToString();
    }

    public string GetRange(int position, int length)
    {
        ValidateRange(position, length);

        if (length == 0)
            return string.Empty;

        var result = new StringBuilder(length);
        string addText = _addBuffer.ToString();

        int rangeEnd = position + length;
        int cursor = 0;

        foreach (var piece in _pieces)
        {
            int pieceStart = cursor;
            int pieceEnd = cursor + piece.Length;

            int overlapStart = Math.Max(position, pieceStart);
            int overlapEnd = Math.Min(rangeEnd, pieceEnd);

            if (overlapStart < overlapEnd)
            {
                string buffer = piece.Buffer == BufferKind.Original
                    ? _originalBuffer
                    : addText;

                int bufferStart =
                    piece.Start + overlapStart - pieceStart;

                int overlapLength = overlapEnd - overlapStart;

                result.Append(buffer, bufferStart, overlapLength);
            }

            cursor = pieceEnd;

            if (cursor >= rangeEnd)
                break;
        }

        return result.ToString();
    }

    public void Insert(int position, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (position < 0 || position > Length)
            throw new ArgumentOutOfRangeException(nameof(position));

        if (text.Length == 0)
            return;

        int addStart = _addBuffer.Length;
        _addBuffer.Append(text);

        var inserted = new Piece(
            BufferKind.Add,
            addStart,
            text.Length);

        int index = FindInsertionPoint(position, out int offset);

        if (index == _pieces.Count)
        {
            _pieces.Add(inserted);
        }
        else if (offset == 0)
        {
            _pieces.Insert(index, inserted);
        }
        else
        {
            Piece old = _pieces[index];

            var left = new Piece(
                old.Buffer,
                old.Start,
                offset);

            var right = new Piece(
                old.Buffer,
                old.Start + offset,
                old.Length - offset);

            _pieces[index] = left;
            _pieces.Insert(index + 1, inserted);
            _pieces.Insert(index + 2, right);
        }

        Length += text.Length;

        MergePieces();
        UpdateLineIndex();
    }

    public void Delete(int position, int length)
    {
        ValidateRange(position, length);

        if (length == 0)
            return;

        int end = position + length;
        int cursor = 0;

        var result = new List<Piece>(_pieces.Count);

        foreach (var piece in _pieces)
        {
            int pieceStart = cursor;
            int pieceEnd = cursor + piece.Length;

            if (pieceEnd <= position || pieceStart >= end)
            {
                result.Add(piece);
            }
            else
            {
                int leftLength = Math.Max(
                    0,
                    position - pieceStart);

                if (leftLength > 0)
                {
                    result.Add(new Piece(
                        piece.Buffer,
                        piece.Start,
                        leftLength));
                }

                int rightLength = Math.Max(
                    0,
                    pieceEnd - end);

                if (rightLength > 0)
                {
                    result.Add(new Piece(
                        piece.Buffer,
                        piece.Start + piece.Length - rightLength,
                        rightLength));
                }
            }

            cursor = pieceEnd;
        }

        _pieces.Clear();
        _pieces.AddRange(result);

        Length -= length;

        MergePieces();
        UpdateLineIndex();
    }

    public void Replace(int position, int length, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        ValidateRange(position, length);

        if (length == 0 && text.Length == 0)
            return;

        Delete(position, length);
        Insert(position, text);
    }

    public int GetLineStart(int line)
    {
        return _lineIndex.GetLineStart(line);
    }

    public int GetLineFromPosition(int position)
    {
        return _lineIndex.GetLineFromPosition(position, Length);
    }

    public (int Line, int Column) GetLineColumn(int position)
    {
        return _lineIndex.GetLineColumn(position, Length);
    }

    public string GetLine(int line)
    {
        string text = GetText();
        var (start, length) = _lineIndex.GetLineRange(line, text);

        return text.Substring(start, length);
    }

    private void UpdateLineIndex()
    {
        _lineIndex.Rebuild(GetText());
    }

    private void ValidateRange(int position, int length)
    {
        if (position < 0 || position > Length)
            throw new ArgumentOutOfRangeException(nameof(position));

        if (length < 0 || length > Length - position)
            throw new ArgumentOutOfRangeException(nameof(length));
    }

    private int FindInsertionPoint(int position, out int offset)
    {
        int cursor = 0;

        for (int i = 0; i < _pieces.Count; i++)
        {
            Piece piece = _pieces[i];
            int next = cursor + piece.Length;

            if (position < next)
            {
                offset = position - cursor;
                return i;
            }

            cursor = next;

            if (position == cursor)
            {
                offset = 0;
                return i + 1;
            }
        }

        offset = 0;
        return _pieces.Count;
    }

    private void MergePieces()
    {
        for (int i = 1; i < _pieces.Count;)
        {
            Piece a = _pieces[i - 1];
            Piece b = _pieces[i];

            if (a.Buffer == b.Buffer &&
                a.Start + a.Length == b.Start)
            {
                _pieces[i - 1] = new Piece(
                    a.Buffer,
                    a.Start,
                    a.Length + b.Length);

                _pieces.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
