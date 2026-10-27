using System;
using mEditor.Models.Buffer;
using Xunit;

namespace PieceTableLib.Tests;

public class PieceTableTests
{
    [Fact]
    public void Constructor_StoresInitialText()
    {
        var table = new PieceTable("Hello world!");

        Assert.Equal("Hello world!", table.GetText());
        Assert.Equal(12, table.Length);
        Assert.Equal(1, table.LineCount);
    }

    [Fact]
    public void Constructor_WithEmptyText_CreatesEmptyDocument()
    {
        var table = new PieceTable();

        Assert.Equal("", table.GetText());
        Assert.Equal(0, table.Length);
        Assert.Equal(1, table.LineCount);
        Assert.Equal("", table.GetLine(0));
    }

    [Fact]
    public void Constructor_NullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new PieceTable(null!));
    }

    // Insert tests

    [Fact]
    public void Insert_AtBeginning_InsertsText()
    {
        var table = new PieceTable("world");

        table.Insert(0, "Hello ");

        Assert.Equal("Hello world", table.GetText());
        Assert.Equal(11, table.Length);
    }

    [Fact]
    public void Insert_InMiddle_InsertsText()
    {
        var table = new PieceTable("Hello world!");

        table.Insert(6, "beautiful ");

        Assert.Equal("Hello beautiful world!", table.GetText());
        Assert.Equal(22, table.Length);
    }

    [Fact]
    public void Insert_AtEnd_AppendsText()
    {
        var table = new PieceTable("Hello");

        table.Insert(5, " world!");

        Assert.Equal("Hello world!", table.GetText());
    }

    [Fact]
    public void Insert_IntoEmptyDocument_Works()
    {
        var table = new PieceTable();

        table.Insert(0, "Hello");

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Insert_EmptyText_DoesNothing()
    {
        var table = new PieceTable("Hello");

        table.Insert(2, "");

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Insert_MultipleTimes_PreservesOrder()
    {
        var table = new PieceTable("ac");

        table.Insert(1, "b");
        table.Insert(3, "d");
        table.Insert(4, "e");

        Assert.Equal("abcde", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Insert_Newline_UpdatesLineIndex()
    {
        var table = new PieceTable("HelloWorld");

        table.Insert(5, "\n");

        Assert.Equal("Hello\nWorld", table.GetText());
        Assert.Equal(2, table.LineCount);
        Assert.Equal(6, table.GetLineStart(1));
        Assert.Equal("World", table.GetLine(1));
    }

    [Fact]
    public void Insert_InvalidPosition_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Insert(-1, "X"));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Insert(6, "X"));
    }

    [Fact]
    public void Insert_NullText_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentNullException>(
            () => table.Insert(0, null!));
    }

    // Delete tests

    [Fact]
    public void Delete_FromBeginning_RemovesText()
    {
        var table = new PieceTable("Hello world!");

        table.Delete(0, 6);

        Assert.Equal("world!", table.GetText());
        Assert.Equal(6, table.Length);
    }

    [Fact]
    public void Delete_FromEnd_RemovesText()
    {
        var table = new PieceTable("Hello world!");

        table.Delete(5, 7);

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Delete_FromMiddle_RemovesText()
    {
        var table = new PieceTable("Hello beautiful world!");

        table.Delete(6, 10);

        Assert.Equal("Hello world!", table.GetText());
        Assert.Equal(12, table.Length);
    }

    [Fact]
    public void Delete_EntireDocument_LeavesEmptyDocument()
    {
        var table = new PieceTable("Hello");

        table.Delete(0, 5);

        Assert.Equal("", table.GetText());
        Assert.Equal(0, table.Length);
        Assert.Equal(1, table.LineCount);
    }

    [Fact]
    public void Delete_ZeroLength_DoesNothing()
    {
        var table = new PieceTable("Hello");

        table.Delete(2, 0);

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Delete_AcrossMultiplePieces_Works()
    {
        var table = new PieceTable("abcdef");

        table.Insert(2, "XYZ");
        table.Delete(1, 6);

        Assert.Equal("aef", table.GetText());
        Assert.Equal(3, table.Length);
    }

    [Fact]
    public void Delete_Newline_UpdatesLineIndex()
    {
        var table = new PieceTable("Hello\nWorld");

        table.Delete(5, 1);

        Assert.Equal("HelloWorld", table.GetText());
        Assert.Equal(1, table.LineCount);
        Assert.Equal("HelloWorld", table.GetLine(0));
    }

    [Fact]
    public void Delete_InvalidPosition_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Delete(-1, 1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Delete(6, 0));
    }

    [Fact]
    public void Delete_InvalidLength_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Delete(0, -1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Delete(3, 3));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Delete(0, 6));
    }

    // Range tests

    [Fact]
    public void GetRange_ReturnsRequestedText()
    {
        var table = new PieceTable("Hello beautiful world!");

        Assert.Equal("beautiful", table.GetRange(6, 9));
    }

    [Fact]
    public void GetRange_AcrossMultiplePieces_Works()
    {
        var table = new PieceTable("abcdef");

        table.Insert(3, "XYZ");

        Assert.Equal("bcXYZde", table.GetRange(1, 7));
    }

    [Fact]
    public void GetRange_ZeroLength_ReturnsEmptyString()
    {
        var table = new PieceTable("Hello");

        Assert.Equal("", table.GetRange(2, 0));
        Assert.Equal("", table.GetRange(5, 0));
    }

    [Fact]
    public void GetRange_EntireDocument_ReturnsAllText()
    {
        var table = new PieceTable("Hello");

        Assert.Equal("Hello", table.GetRange(0, 5));
    }

    [Fact]
    public void GetRange_InvalidRange_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetRange(-1, 1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetRange(0, -1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetRange(3, 3));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetRange(6, 0));
    }

    // Replace tests

    [Fact]
    public void Replace_InMiddle_ReplacesText()
    {
        var table = new PieceTable("Hello beautiful world!");

        table.Replace(6, 9, "amazing");

        Assert.Equal("Hello amazing world!", table.GetText());
        Assert.Equal(20, table.Length);
    }

    [Fact]
    public void Replace_WithEmptyText_DeletesRange()
    {
        var table = new PieceTable("Hello world!");

        table.Replace(5, 7, "");

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Replace_ZeroLength_InsertsText()
    {
        var table = new PieceTable("Hello world!");

        table.Replace(6, 0, "beautiful ");

        Assert.Equal("Hello beautiful world!", table.GetText());
    }

    [Fact]
    public void Replace_WithEmptyRangeAndText_DoesNothing()
    {
        var table = new PieceTable("Hello");

        table.Replace(2, 0, "");

        Assert.Equal("Hello", table.GetText());
        Assert.Equal(5, table.Length);
    }

    [Fact]
    public void Replace_InvalidRange_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Replace(-1, 1, "X"));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.Replace(3, 3, "X"));
    }

    [Fact]
    public void Replace_NullText_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentNullException>(
            () => table.Replace(0, 1, null!));
    }

    [Fact]
    public void Replace_Newline_UpdatesLineIndex()
    {
        var table = new PieceTable("Hello\nWorld");

        table.Replace(5, 1, "\nNew\n");

        Assert.Equal("Hello\nNew\nWorld", table.GetText());
        Assert.Equal(3, table.LineCount);
        Assert.Equal("New", table.GetLine(1));
        Assert.Equal("World", table.GetLine(2));
    }

    // Line-index tests

    [Fact]
    public void LineCount_EmptyDocument_IsOne()
    {
        var table = new PieceTable();

        Assert.Equal(1, table.LineCount);
    }

    [Fact]
    public void LineCount_MultilineDocument_ReturnsCorrectCount()
    {
        var table = new PieceTable("One\nTwo\nThree");

        Assert.Equal(3, table.LineCount);
    }

    [Fact]
    public void LineCount_TrailingNewline_IncludesEmptyFinalLine()
    {
        var table = new PieceTable("One\nTwo\n");

        Assert.Equal(3, table.LineCount);
        Assert.Equal("", table.GetLine(2));
        Assert.Equal(8, table.GetLineStart(2));
    }

    [Fact]
    public void GetLineStart_ReturnsCorrectOffsets()
    {
        var table = new PieceTable("One\nTwo\nThree");

        Assert.Equal(0, table.GetLineStart(0));
        Assert.Equal(4, table.GetLineStart(1));
        Assert.Equal(8, table.GetLineStart(2));
    }

    [Fact]
    public void GetLineStart_InvalidLine_Throws()
    {
        var table = new PieceTable("One\nTwo");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLineStart(-1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLineStart(2));
    }

    [Fact]
    public void GetLineFromPosition_ReturnsCorrectLine()
    {
        var table = new PieceTable("One\nTwo\nThree");

        Assert.Equal(0, table.GetLineFromPosition(0));
        Assert.Equal(0, table.GetLineFromPosition(2));
        Assert.Equal(0, table.GetLineFromPosition(3));
        Assert.Equal(1, table.GetLineFromPosition(4));
        Assert.Equal(1, table.GetLineFromPosition(7));
        Assert.Equal(2, table.GetLineFromPosition(8));
        Assert.Equal(2, table.GetLineFromPosition(13));
    }

    [Fact]
    public void GetLineFromPosition_AtDocumentEnd_ReturnsLastLine()
    {
        var table = new PieceTable("One\nTwo");

        Assert.Equal(1, table.GetLineFromPosition(table.Length));
    }

    [Fact]
    public void GetLineFromPosition_InvalidPosition_Throws()
    {
        var table = new PieceTable("Hello");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLineFromPosition(-1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLineFromPosition(6));
    }

    [Fact]
    public void GetLineColumn_ReturnsCorrectLineAndColumn()
    {
        var table = new PieceTable("One\nTwo\nThree");

        Assert.Equal((0, 0), table.GetLineColumn(0));
        Assert.Equal((0, 2), table.GetLineColumn(2));
        Assert.Equal((0, 3), table.GetLineColumn(3));
        Assert.Equal((1, 0), table.GetLineColumn(4));
        Assert.Equal((1, 2), table.GetLineColumn(6));
        Assert.Equal((2, 2), table.GetLineColumn(10));
    }

    [Fact]
    public void GetLine_ReturnsLineWithoutNewline()
    {
        var table = new PieceTable("One\nTwo\nThree");

        Assert.Equal("One", table.GetLine(0));
        Assert.Equal("Two", table.GetLine(1));
        Assert.Equal("Three", table.GetLine(2));
    }

    [Fact]
    public void GetLine_CRLF_ExcludesBothNewlineCharacters()
    {
        var table = new PieceTable("One\r\nTwo\r\nThree");

        Assert.Equal(3, table.LineCount);
        Assert.Equal("One", table.GetLine(0));
        Assert.Equal("Two", table.GetLine(1));
        Assert.Equal("Three", table.GetLine(2));
    }

    [Fact]
    public void GetLine_EmptyLines_ReturnsEmptyStrings()
    {
        var table = new PieceTable("One\n\nThree");

        Assert.Equal(3, table.LineCount);
        Assert.Equal("One", table.GetLine(0));
        Assert.Equal("", table.GetLine(1));
        Assert.Equal("Three", table.GetLine(2));
    }

    [Fact]
    public void GetLine_InvalidLine_Throws()
    {
        var table = new PieceTable("One\nTwo");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLine(-1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => table.GetLine(2));
    }

    [Fact]
    public void InsertAndDelete_MixedOperations_Work()
    {
        var table = new PieceTable("Hello world!");

        table.Insert(6, "beautiful ");
        table.Delete(6, 10);
        table.Insert(6, "piece ");

        Assert.Equal("Hello piece world!", table.GetText());
        Assert.Equal(18, table.Length);
    }

    [Fact]
    public void MixedEdits_KeepLineIndexConsistent()
    {
        var table = new PieceTable("Alpha\nBeta");

        table.Insert(5, "\nNew");
        Assert.Equal(3, table.LineCount);
        Assert.Equal("New", table.GetLine(1));
        Assert.Equal("Beta", table.GetLine(2));

        table.Delete(5, 4);
        Assert.Equal(2, table.LineCount);
        Assert.Equal("Alpha", table.GetLine(0));
        Assert.Equal("Beta", table.GetLine(1));

        table.Replace(0, 5, "First\nSecond");
        Assert.Equal(3, table.LineCount);
        Assert.Equal("First", table.GetLine(0));
        Assert.Equal("Second", table.GetLine(1));
        Assert.Equal("Beta", table.GetLine(2));
    }
}
