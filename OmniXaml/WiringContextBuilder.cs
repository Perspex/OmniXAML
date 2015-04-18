namespace OmniXaml
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Catalogs;
    using TypeConversion;

    public class WiringContextBuilder
    {
        private readonly IContentPropertyProvider contentPropertyProvider;
        private readonly ITypeConverterProvider converterProvider;

        private readonly TypeContextBuilder typingCoreBuilder = new TypeContextBuilder();

        public WiringContextBuilder()
        {            
            converterProvider = new TypeConverterProvider();
            contentPropertyProvider = new ContentPropertyProvider();            
        }

        public WiringContextBuilder AddNsForThisType(string prefix, string xamlNs, Type referenceType)
        {
            typingCoreBuilder.AddNsForThisType(prefix, xamlNs, referenceType);
            return this;
        }

        public WiringContextBuilder WithXamlNs(string xamlNs, Assembly assembly, string clrNs)
        {
            typingCoreBuilder.WithXamlNs(xamlNs, assembly, clrNs);
            return this;
        }

        public WiringContextBuilder WithNsPrefix(string prefix, string ns)
        {
            typingCoreBuilder.WithNsPrefix(prefix, ns);
            return this;
        }


        public WiringContextBuilder WithContentPropertiesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            contentPropertyProvider.AddCatalog(new AttributeBasedContentPropertyCatalog(assemblies));
            return this;
        }

        public WiringContext Build()
        {
            var typingCore = typingCoreBuilder.Build();
            return new WiringContext(typingCore, contentPropertyProvider, converterProvider);
        }
    }
}