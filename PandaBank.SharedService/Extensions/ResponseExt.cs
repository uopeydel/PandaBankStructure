using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PandaBank.SharedService.Extensions
{
    public static class PandaResponse
    {
        public static Results<TResult> CreateSuccessResponse<TResult>(TResult tresult, PagingInfo pagingInfo = null)
        {
            var result = new Results<TResult>();
            result.Data = tresult;
            result.PageInfo = pagingInfo;
            return result;
        }

        public static Results<TResult> CreateErrorResponse<TResult>(string error, Exception exception = null)
        {
            var result = new Results<TResult>();
            if (result.Errors == null)
            {
                result.Errors = new List<string>
                {
                    exception?.Message
                };
            }
            result.Errors.AddRange(GetInnerExceptionMessage(exception));
            result.Errors.Insert(0, error);
            result.Errors = result?.Errors?.Where(w => !string.IsNullOrEmpty(w)).Distinct().ToList();
            return result;
        }

        public static Results<TResult> CreateErrorResponse<TResult>(string[] errors, Exception exception = null)
        {
            var result = new Results<TResult>();
            result.Errors = GetInnerExceptionMessage(exception);
            foreach (var error in errors)
            {
                result.Errors.Insert(0, error);
            }
            result.Errors = result?.Errors?.Where(w => !string.IsNullOrEmpty(w)).Distinct().ToList();
            return result;
        }

        private static List<string> GetInnerExceptionMessage(Exception exception)
        {
            var errorTextList = new List<string>();
            var error = GetSubInnerExceptionMessage(exception);
            while (!string.IsNullOrEmpty(error?.Trim()))
            {
                errorTextList.Add(error);
                error = GetSubInnerExceptionMessage(exception);
            }
            return errorTextList;
        }

        private static string GetSubInnerExceptionMessage(Exception exception)
        {
            return exception?.InnerException?.Message;
        }




        public static Expression<Func<T, bool>> AndAlso<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor
            : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }

    }
}
