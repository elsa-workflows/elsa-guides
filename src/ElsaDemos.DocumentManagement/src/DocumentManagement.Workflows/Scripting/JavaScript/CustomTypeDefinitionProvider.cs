using System;
using System.Collections.Generic;
using DocumentManagement.Workflows.Activities;
using Elsa.Scripting.JavaScript.Services;

namespace DocumentManagement.Workflows.Scripting.JavaScript
{
    /// <summary>
    /// Register .NET types for which we want to include with JS intellisense.
    /// </summary>
    public class CustomTypeDefinitionProvider : TypeDefinitionProvider
    {
        public override IEnumerable<Type> CollectTypes(TypeDefinitionContext context)
        {
            yield return typeof(DocumentFile);
        }
    }
}