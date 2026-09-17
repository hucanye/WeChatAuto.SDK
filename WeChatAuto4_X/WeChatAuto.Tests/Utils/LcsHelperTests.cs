using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;
using OneOf;
using Xunit.Abstractions;
using NAudio.CoreAudioApi;
using WeChatAuto.Utils;


namespace WeChatAuto.Tests.Utils;

public class LcsHelperTest
{
    private readonly ITestOutputHelper _output;

    public LcsHelperTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "测试Diff")]
    public void TestDiff()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F"
        };

        var newList = new[]
        {
            "E",
            "G",
            "I",
            "G",
            "xxx撤消一条消息"
        };

        var diff = LcsHelper.Diff(oldList, newList);

        foreach (var item in diff)
        {
            _output.WriteLine($"{item.Type,-6}  {item.Value}");
        }
    }

    /// <summary>
    /// 测试Diff转换为DiffBlock。
    /// </summary>
    [Fact(DisplayName = "测试DiffBlock")]
    public void TestToDiffBlocks()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
        };

        var newList = new[]
        {
            "A",
            "B",
            "C",
            "X",
            "Y",
            "E",
            "F",
            "G",
            "H",
            "I"
        };

        var diff = LcsHelper.Diff(oldList, newList);
        var blocks = LcsHelper.ToDiffBlocks(diff);

        foreach (var block in blocks)
        {
            _output.WriteLine(
                $"{block.Type,-6} Count={block.Count}  [{string.Join(", ", block.Items)}]");
        }

        // Assert.Equal(3, blocks.Count);

        // Assert.Equal(DiffType.Equal, blocks[0].Type);
        // Assert.Equal(3, blocks[0].Count);
        // Assert.Equal(
        //     new[] { "A", "B", "C" },
        //     blocks[0].Items);

        // Assert.Equal(DiffType.Delete, blocks[1].Type);
        // Assert.Equal(2, blocks[1].Count);
        // Assert.Equal(
        //     new[] { "D", "E" },
        //     blocks[1].Items);

        // Assert.Equal(DiffType.Insert, blocks[2].Type);
        // Assert.Equal(2, blocks[2].Count);
        // Assert.Equal(
        //     new[] { "X", "Y" },
        //     blocks[2].Items);
    }

    /// <summary>
    /// 测试正常情况下获取新增消息。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-正常新增")]
    public void TestGetNewItems_Normal()
    {
        var oldList = new[]
        {
            // "A",
            // "B",
            // "C",
            // "D",
            // "E"
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
        };

        var newList = new[]
        {
            // "A",
            // "B",
            // "C",
            // "D",
            // "E",
            // "F",
            // "G"
            "A",
            "B",
            "C",
            "X",
            "Y",
            "E",
            "F",
            "G",
            "H",
            "I",
            "X"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        _output.WriteLine(
            $"新增消息：[{string.Join(", ", newItems)}]");

        // Assert.Equal(
        //     new[] { "F", "G" },
        //     newItems);
    }

    /// <summary>
    /// 测试旧快照前面的消息已经从窗口中消失，
    /// 但中间仍然存在连续的Equal消息。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-窗口滚动")]
    public void TestGetNewItems_ScrollWindow()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F"
        };

        var newList = new[]
        {
            "C",
            "D",
            "E",
            "F",
            "G",
            "H"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        _output.WriteLine(
            $"新增消息：[{string.Join(", ", newItems)}]");

        Assert.Equal(
            new[] { "G", "H" },
            newItems);
    }

    /// <summary>
    /// 测试没有可靠Equal锚点时，
    /// 整个newList都应该作为新消息返回。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-无可靠锚点")]
    public void TestGetNewItems_NoReliableAnchor()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };

        var newList = new[]
        {
            "X",
            "Y",
            "Z",
            "M",
            "N"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        _output.WriteLine(
            $"无可靠锚点，全部作为新消息：[{string.Join(", ", newItems)}]");

        Assert.Equal(
            newList,
            newItems);
    }

    /// <summary>
    /// 测试虽然Equal总数量达到3，
    /// 但是不存在连续3个Equal，因此不能建立可靠锚点。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-Equal不连续")]
    public void TestGetNewItems_NonConsecutiveEqual()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C"
        };

        var newList = new[]
        {
            "A",
            "X",
            "B",
            "Y",
            "C"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        _output.WriteLine(
            $"Equal虽然总数为3，但不连续：[{string.Join(", ", newItems)}]");

        // 最大连续Equal只有1，
        // 所以没有可靠锚点，整个newList都认为是新的。
        Assert.Equal(
            newList,
            newItems);
    }

    /// <summary>
    /// 测试存在多个可靠Equal Block时，
    /// 使用最后一个可靠Equal Block作为锚点。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-使用最后一个可靠锚点")]
    public void TestGetNewItems_LastReliableAnchor()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F"
        };

        var newList = new[]
        {
            "A",
            "B",
            "C",
            "X",
            "D",
            "E",
            "F",
            "G",
            "H"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        _output.WriteLine(
            $"使用最后一个可靠锚点后的新增消息：[{string.Join(", ", newItems)}]");

        // Equal [A,B,C]
        // Insert [X]
        // Equal [D,E,F]
        // Insert [G,H]
        //
        // 最后一个可靠Equal是 [D,E,F]
        // 所以最终只返回 [G,H]。
        Assert.Equal(
            new[] { "G", "H" },
            newItems);
    }

    /// <summary>
    /// 测试没有旧快照时，整个新快照作为新消息。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-旧快照为空")]
    public void TestGetNewItems_EmptyOldList()
    {
        var oldList = Array.Empty<string>();

        var newList = new[]
        {
            "A",
            "B",
            "C"
        };

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        Assert.Equal(
            newList,
            newItems);
    }

    /// <summary>
    /// 测试新快照为空时，不存在新增消息。
    /// </summary>
    [Fact(DisplayName = "测试GetNewItems-新快照为空")]
    public void TestGetNewItems_EmptyNewList()
    {
        var oldList = new[]
        {
            "A",
            "B",
            "C"
        };

        var newList = Array.Empty<string>();

        var newItems = LcsHelper.GetNewItems(
            oldList,
            newList);

        Assert.Empty(newItems);
    }
}