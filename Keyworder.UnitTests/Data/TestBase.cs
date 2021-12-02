﻿using Keyworder.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Keyworder.UnitTests.Data;

[TestClass]
public class TestBase
{
    protected static string path = null!;
    protected static KeywordService keywordService = null!;

    protected static string GetNewPath() => $"Keywords-{Guid.NewGuid()}.json";

    [TestInitialize]
    public void TestInitialize()
    {
        path = GetNewPath();
        TestData.Create(path);
        keywordService = new KeywordService(path);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}