using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CPTech.ModelBinding
{
    public class TupleModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            Type modelType = context.Metadata.ModelType;
            if (typeof(ITuple).IsAssignableFrom(modelType) && modelType.Name.StartsWith("ValueTuple"))
                return new BinderTypeModelBinder(typeof(TupleModelBinder));

            return null;
        }
    }
}
