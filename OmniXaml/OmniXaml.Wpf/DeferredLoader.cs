namespace OmniXaml.Wpf
{
    using System.Collections.Generic;

    public class DeferredLoader : IDeferredLoader
    {
        public object Load(IEnumerable<XamlInstruction> nodes, WiringContext context)
        {
            return new TemplateContent(nodes, context);
        }
    }
}