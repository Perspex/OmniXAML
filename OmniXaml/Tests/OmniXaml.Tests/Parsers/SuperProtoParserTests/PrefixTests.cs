﻿namespace OmniXaml.Tests.Parsers.SuperProtoParserTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Classes;
    using Classes.Another;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OmniXaml.Parsers.ProtoParser;

    [TestClass]
    public class PrefixTests : GivenAWiringContext
    {
        private readonly ProtoNodeBuilder builder;
        private IParser<Stream, IEnumerable<ProtoXamlInstruction>> sut;

        public PrefixTests()
        {
            builder = new ProtoNodeBuilder(WiringContext.TypeContext, WiringContext.FeatureProvider);
        }

        [TestInitialize]
        public void Initialize()
        {
            sut = new XamlProtoInstructionParser(WiringContext);
        }

        [TestMethod]
        public void SingleCollapsed()
        {
            var actualNodes = sut.Parse<IEnumerable<ProtoXamlInstruction>>("<x:Foreigner xmlns:x=\"another\"/>").ToList();
            var expectedNodes = new List<ProtoXamlInstruction>
            {
                builder.NamespacePrefixDeclaration(AnotherNs),
                builder.EmptyElement(typeof (Foreigner), AnotherNs),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void AttachedProperty()
        {
            var actualNodes = sut.Parse<IEnumerable<ProtoXamlInstruction>>(@"<DummyClass xmlns=""root"" xmlns:x=""another"" x:Foreigner.Property=""Value""></DummyClass>").ToList();

            var ns = "root";

            var expectedNodes = new Collection<ProtoXamlInstruction>
            {
                builder.NamespacePrefixDeclaration("", ns),
                builder.NamespacePrefixDeclaration("x", "another"),
                builder.NonEmptyElement(typeof (DummyClass), RootNs),
                builder.AttachableProperty<Foreigner>("Property", "Value", AnotherNs),
                builder.EndTag(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void ElementWithPrefixThatIsDefinedAfterwards()
        {
            var actualNodes = sut.Parse<IEnumerable<ProtoXamlInstruction>>(@"<x:DummyClass xmlns:x=""another""></x:DummyClass>").ToList();

            var ns = "root";

            var expectedNodes = new Collection<ProtoXamlInstruction>
            {
                builder.NamespacePrefixDeclaration(AnotherNs),
                builder.NonEmptyElement(typeof (DummyClass), AnotherNs),
                builder.EndTag(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }
    }
}