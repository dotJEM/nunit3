using System;
using System.Linq.Expressions;
using DotJEM.NUnit3.Constraints;
using DotJEM.NUnit3.Constraints.Objects;
using DotJEM.NUnit3.Util;
using NUnit.Framework.Constraints;
using NUnitHas = NUnit.Framework.Has;

namespace DotJEM.NUnit3;

public class Has : NUnitHas
{
    public static IPropertiesConstraintsFactory Properties { get; } = new PropertiesConstraintsFactory();

    public static ResolvableConstraintExpression Property<T>(Expression<Func<T, object>> property)
        => new ConstraintExpression().Property(property.GetPropertyInfo().Name);

    public static ResolvableConstraintExpression Property<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        => new ConstraintExpression().Property(expression.GetPropertyInfo().Name);
}
