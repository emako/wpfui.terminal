using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Ui.Terminal.Internals;

internal class DepPropHelper<CONTROL_TYPE> where CONTROL_TYPE : UserControl
{
    protected DepPropHelper() => throw new Exception("Should not be instanced");

    public static DependencyProperty GenerateWriteOnlyProperty<PROP_TYPE>(Expression<Func<CONTROL_TYPE, PROP_TYPE>> PropToSet)
    {
        if (PropToSet.Body is not MemberExpression me)
            throw new ArgumentException(nameof(PropToSet));

        var propName = me.Member.Name;
        var prop = typeof(CONTROL_TYPE).GetProperty(me.Member.Name, BindingFlags.Instance | BindingFlags.Public);

        return prop switch
        {
            null => throw new ArgumentException(nameof(PropToSet)),
            _ => DependencyProperty.Register(propName, typeof(PROP_TYPE), typeof(CONTROL_TYPE), new FrameworkPropertyMetadata(null, (target, value) => CoerceReadOnlyHandle(prop.SetMethod!, target, value)))
        };
    }

    private static object CoerceReadOnlyHandle(MethodInfo SetMethod, DependencyObject target, object value)
    {
        SetMethod.Invoke(target, [value]);
        return null!;
    }
}
