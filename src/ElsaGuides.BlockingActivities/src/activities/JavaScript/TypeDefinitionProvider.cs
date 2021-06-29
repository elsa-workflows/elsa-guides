using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.JavaScript.Services;
using MyActivityLibrary.Models;

namespace MyActivityLibrary.JavaScript
{
    public class MyTypeDefinitionProvider : TypeDefinitionProvider
    {
        public override ValueTask<IEnumerable<Type>> CollectTypesAsync(TypeDefinitionContext context, CancellationToken cancellationToken = default)
        {
            var types = new[] { typeof(FileModel) };
            return new ValueTask<IEnumerable<Type>>(types);
        }
    }
}