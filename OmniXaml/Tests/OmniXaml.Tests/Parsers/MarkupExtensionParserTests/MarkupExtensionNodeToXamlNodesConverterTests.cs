﻿namespace OmniXaml.Tests.Parsers.MarkupExtensionParserTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Builder;
    using Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OmniXaml.Parsers.MarkupExtensions;

    [TestClass]
    public class MarkupExtensionNodeToXamlNodesConverterTests : GivenAWiringContextWithNodeBuilders
    {
        private XamlNodeBuilder x;

        public MarkupExtensionNodeToXamlNodesConverterTests()
        {
            x = new XamlNodeBuilder(WiringContext.TypeContext);
        }

        [TestMethod]
        public void NameOnly()
        {
            var tree = new MarkupExtensionNode(new IdentifierNode("DummyExtension"));
            var sut = new MarkupExtensionNodeToXamlNodesConverter(WiringContext);
            var actualNodes = sut.Convert(tree).ToList();
            var expectedNodes = new List<XamlInstruction>
            {
                X.StartObject<DummyExtension>(),
                X.EndObject(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void NameAndAttribute()
        {
            var tree = new MarkupExtensionNode(new IdentifierNode("DummyExtension"), new OptionsCollection {new PropertyOption("Property", new StringNode("Value"))});
            var sut = new MarkupExtensionNodeToXamlNodesConverter(WiringContext);
            var actualNodes = sut.Convert(tree).ToList();

            var expectedNodes = new List<XamlInstruction>
            {
                X.StartObject<DummyExtension>(),
                X.StartMember<DummyExtension>(d => d.Property),
                X.Value("Value"),
                X.EndMember(),
                X.EndObject(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void NameAndTwoAttributes()
        {
            var tree = new MarkupExtensionNode(new IdentifierNode("DummyExtension"), new OptionsCollection
            {
                new PropertyOption("Property", new StringNode("Value")),
                new PropertyOption("AnotherProperty", new StringNode("AnotherValue")),
            });
            var sut = new MarkupExtensionNodeToXamlNodesConverter(WiringContext);
            var actualNodes = sut.Convert(tree).ToList();

            var expectedNodes = new List<XamlInstruction>
            {
                X.StartObject<DummyExtension>(),
                X.StartMember<DummyExtension>(d => d.Property),
                X.Value("Value"),
                X.EndMember(),
                X.StartMember<DummyExtension>(d => d.AnotherProperty),
                X.Value("AnotherValue"),
                X.EndMember(),
                X.EndObject(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }

        [TestMethod]
        public void PositionalOption()
        {
            var tree = new MarkupExtensionNode(new IdentifierNode("DummyExtension"), new OptionsCollection
            {
               new PositionalOption("Option")
            });
            var sut = new MarkupExtensionNodeToXamlNodesConverter(WiringContext);
            var actualNodes = sut.Convert(tree).ToList();

            var expectedNodes = new Collection<XamlInstruction>
            {
                X.StartObject<DummyExtension>(),
                X.MarkupExtensionArguments(),
                X.Value("Option"),
                X.EndMember(),
                X.EndObject(),
            };

            CollectionAssert.AreEqual(expectedNodes, actualNodes);
        }
    }

   
}
