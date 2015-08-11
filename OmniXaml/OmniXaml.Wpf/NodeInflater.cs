namespace OmniXaml.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using AppServices.NetCore;
    using Builder;
    using Parsers.ProtoParser;
    using Parsers.XamlNodes;
    using Typing;

    internal class NodeInflater
    {
        private readonly IEnumerable<Type> inflatables;
        private readonly WiringContext wiringContext;
        private readonly XamlNodeBuilder nodeBuilder;

        public NodeInflater(IEnumerable<Type> inflatables, WiringContext wiringContext)
        {
            this.inflatables = inflatables;
            this.wiringContext = wiringContext;
            nodeBuilder = new XamlNodeBuilder(wiringContext.TypeContext);
        }

        public IEnumerable<XamlInstruction> Inflate(IEnumerable<XamlInstruction> nodes)
        {
            var processedNodes = new Collection<XamlInstruction>();
            var skipNext = false;
            foreach (var xamlNode in nodes)
            {
                var matchedInflatable = GetMatchedInflatable(xamlNode);

                if (matchedInflatable != null)
                {
                    var toAdd = ReadNodes(xamlNode.XamlType.UnderlyingType);
                    var croppedNodes = Crop(toAdd, xamlNode.XamlType, wiringContext.TypeContext.GetXamlType((matchedInflatable)));

                    foreach (var croppedNode in croppedNodes)
                    {
                        processedNodes.Add(croppedNode);
                        skipNext = true;
                    }
                }
                else
                {
                    if (skipNext)
                    {
                        skipNext = false;
                    }
                    else
                    {
                        processedNodes.Add(xamlNode);
                    }
                }
            }

            return processedNodes;
        }

        private IEnumerable<XamlInstruction> Crop(IEnumerable<XamlInstruction> original, XamlType newType, XamlType oldType)
        {
            var list = original.ToList();
            var nodeToReplace = list.First(node => NodeHasSameType(oldType, node));
            var id = list.IndexOf(nodeToReplace);
            list[id] = nodeBuilder.StartObject(newType.UnderlyingType);
            return list;
        }

        private static bool NodeHasSameType(XamlType oldType, XamlInstruction instruction)
        {
            var xamlType = instruction.XamlType;
            if (xamlType != null)
            {
                var nodeHasSameType = xamlType.Equals(oldType);
                return nodeHasSameType;
            }

            return false;
        }

        private static IEnumerable<XamlInstruction> ReadNodes(Type underlyingType)
        {
            var resourceProvider = new InflatableTranslator();

            using (var stream = resourceProvider.GetStream(underlyingType))
            {
                var wiringContext = WpfWiringContextFactory.GetContext(new TypeFactory());
                var loader = new XamlInstructionParser(wiringContext);
                var protoParser = new XamlProtoInstructionParser(wiringContext);

                return loader.Parse(protoParser.Parse(stream));
            }
        }

        private Type GetMatchedInflatable(XamlInstruction xamlInstruction)
        {
            if (xamlInstruction.XamlType != null)
            {
                var matches = from inflatable in inflatables
                              where inflatable.IsAssignableFrom(xamlInstruction.XamlType.UnderlyingType)
                              select inflatable;

                return matches.FirstOrDefault();
            }

            return null;
        }
    }
}