using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq.Expressions
{
    public static class LambdaExpressionHelper
    {
        public class GetFieldResult
        {
            public MemberExpression MemberExpression { get; internal set; }
            public System.Reflection.FieldInfo FieldInfo { get; internal set; }
        }

        public static GetFieldResult GetField<TObject, TValue>(this Expression<Func<TObject, TValue>> FieldExpression)
        {
            var res = new GetFieldResult();
            res.MemberExpression = FieldExpression.Body as MemberExpression;
            if (res.MemberExpression == null) throw new ArgumentNullException("FieldExpression", "Doit être une expression de lecture d'un membre.");
            res.FieldInfo = res.MemberExpression.Member as System.Reflection.FieldInfo;
            if (res.FieldInfo == null) throw new ArgumentNullException("FieldExpression", "L'expression doit faire référence à un champ.");
            return res;
        }

        public class GetPropertyResult
        {
            public MemberExpression MemberExpression { get; internal set; }
            public System.Reflection.PropertyInfo PropertyInfo { get; internal set; }
        }

        public static GetPropertyResult GetProperty<TObject, TValue>(this Expression<Func<TObject, TValue>> PropertyExpression)
        {
            var res = new GetPropertyResult();
            res.MemberExpression = PropertyExpression.Body as MemberExpression;
            if (res.MemberExpression == null) throw new ArgumentNullException("PropertyExpression", "Doit être une expression de lecture d'une propriété.");
            res.PropertyInfo = res.MemberExpression.Member as System.Reflection.PropertyInfo;
            if (res.PropertyInfo == null) throw new ArgumentNullException("PropertyExpression", "L'expression doit faire référence à une propriété.");
            return res;
        }

        public static Action<TObject, TValue> GetAction<TObject, TValue>(this Expression<Func<TObject, TValue>> FieldExpression)
        {
            var res = GetField<TObject, TValue>(FieldExpression);
            return res.GetAction<TObject, TValue>();
        }

        public static Action<TObject, TValue> GetAction<TObject, TValue>(this GetFieldResult res)
        {
            var prmObj = Expression.Parameter(typeof(TObject), "obj");
            var prmValue = Expression.Parameter(typeof(TValue), "v");
            var fld = Expression.Field(prmObj, res.FieldInfo);

            //---> Compilation de l'expression régulière pour l'affectation
            var bodySet = Expression.Assign(fld, prmValue);
            var lambdaSet = Expression.Lambda<Action<TObject, TValue>>(bodySet, prmObj, prmValue);
            return lambdaSet.Compile();
        }

        public static Func<TObject, TValue> GetFunc<TObject, TValue>(this Expression<Func<TObject, TValue>> FieldExpression)
        {
            var res = GetField<TObject, TValue>(FieldExpression);
            return res.GetFunc<TObject, TValue>();
        }

        public static Func<TObject, TValue> GetFunc<TObject, TValue>(this GetFieldResult res)
        {
            var prmObj = Expression.Parameter(typeof(TObject), "obj");
 
            //---> Compilation de l'expression régulière pour la lecture
            var bodyGet = Expression<TValue>.MakeMemberAccess(prmObj, res.FieldInfo);
            var lambdaGet = Expression.Lambda<Func<TObject, TValue>>(bodyGet, prmObj);
            return lambdaGet.Compile();
        }
    }

    //public static class ExpressionExtension
    //{
    //    public static BinaryExpression Assign(Expression left, Expression right)
    //    {
    //        var assign = typeof(Assigner<>).MakeGenericType(left.Type).GetMethod("Assign");

    //        var assignExpr = Expression.Add(left, right, assign);

    //        return assignExpr;
    //    }

    //    private static class Assigner<T>
    //    {
    //        public static T Assign(ref T left, T right)
    //        {
    //            return (left = right);
    //        }
    //    }
    //}

}
