using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace AtomicCore
{
    /// <summary>
    /// 表达式访问基础抽象类
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/zh-cn/library/bb882521(v=vs.90).aspx
    /// </remarks>
    public abstract class ExpressionVisitorBase
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        protected ExpressionVisitorBase()
        {
            this.ExpressionChains = new Stack<Expression>();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 表达式解析链（后进先出）
        /// </summary>
        public Stack<Expression> ExpressionChains { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 基础访问方法
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        /// <summary>
        /// 访问到位置表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitUnknown(Expression expression, bool isStackPush = true)
        {
            throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        }

        /// <summary>
        /// 成员数据绑定
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding, bool isStackPush = true)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// 成员访问初始化
        /// </summary>
        /// <param name="initializer"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer, bool isStackPush = true)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        /// <summary>
        /// 访问一元表达式
        /// </summary>
        /// <param name="unaryExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitUnary(UnaryExpression unaryExp, bool isStackPush = true)
        {
            this.StackPush(unaryExp, isStackPush);

            Expression operand = this.Visit(unaryExp.Operand);
            if (operand != unaryExp.Operand)
            {
                return Expression.MakeUnary(unaryExp.NodeType, operand, unaryExp.Type, unaryExp.Method);
            }

            this.StackPop(isStackPush);
            return unaryExp;
        }

        /// <summary>
        /// 访问二元表达式
        /// </summary>
        /// <param name="binaryExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitBinary(BinaryExpression binaryExp, bool isStackPush = true)
        {
            this.StackPush(binaryExp, isStackPush);

            Expression left = this.Visit(binaryExp.Left);
            Expression right = this.Visit(binaryExp.Right);
            Expression conversion = this.Visit(binaryExp.Conversion);
            if (left != binaryExp.Left || right != binaryExp.Right || conversion != binaryExp.Conversion)
            {
                if (binaryExp.NodeType == ExpressionType.Coalesce && binaryExp.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(binaryExp.NodeType, left, right, binaryExp.IsLiftedToNull, binaryExp.Method);
            }

            this.StackPop(isStackPush);
            return binaryExp;
        }

        /// <summary>
        /// 访问类型判断
        /// </summary>
        /// <param name="typeBinaryExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression typeBinaryExp, bool isStackPush = true)
        {
            this.StackPush(typeBinaryExp, isStackPush);

            Expression expr = this.Visit(typeBinaryExp.Expression);
            if (expr != typeBinaryExp.Expression)
            {
                return Expression.TypeIs(expr, typeBinaryExp.TypeOperand);
            }

            this.StackPop(isStackPush);
            return typeBinaryExp;
        }

        /// <summary>
        /// 访问常量表达式
        /// </summary>
        /// <param name="constantExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitConstant(ConstantExpression constantExp, bool isStackPush = true)
        {
            return constantExp;
        }

        /// <summary>
        /// 访问条件表达式（类似三元表达式）
        /// </summary>
        /// <param name="conditionalExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitConditional(ConditionalExpression conditionalExp, bool isStackPush = true)
        {
            this.StackPush(conditionalExp, isStackPush);

            Expression test = this.Visit(conditionalExp.Test);
            Expression ifTrue = this.Visit(conditionalExp.IfTrue);
            Expression ifFalse = this.Visit(conditionalExp.IfFalse);
            if (test != conditionalExp.Test || ifTrue != conditionalExp.IfTrue || ifFalse != conditionalExp.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }

            this.StackPop(isStackPush);
            return conditionalExp;
        }

        /// <summary>
        /// 访问参数
        /// </summary>
        /// <param name="paramExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitParameter(ParameterExpression paramExp, bool isStackPush = true)
        {
            return paramExp;
        }

        /// <summary>
        /// 访问成员表达式 例如 propertys.memberA 或 字段成员
        /// </summary>
        /// <param name="memberExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAccess(MemberExpression memberExp, bool isStackPush = true)
        {
            this.StackPush(memberExp, isStackPush);

            Expression exp = this.Visit(memberExp.Expression);
            if (exp != memberExp.Expression)
            {
                return Expression.MakeMemberAccess(exp, memberExp.Member);
            }

            this.StackPop(isStackPush);
            return memberExp;
        }

        /// <summary>
        /// 访问方法調用
        /// </summary>
        /// <param name="methodCallExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression methodCallExp, bool isStackPush = true)
        {
            this.StackPush(methodCallExp, isStackPush);

            Expression obj = this.Visit(methodCallExp.Object);//method host instance
            IEnumerable<Expression> args = this.VisitExpressionList(methodCallExp.Arguments);
            if (obj != methodCallExp.Object || args != methodCallExp.Arguments)
            {
                return Expression.Call(obj, methodCallExp.Method, args);
            }

            this.StackPop(isStackPush);
            return methodCallExp;
        }

        /// <summary>
        /// 访问表达式集合 例如：方法的参数表达式数组
        /// </summary>
        /// <param name="original"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original, bool isStackPush = true)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.StackPush(original[i], isStackPush);

                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }

                this.StackPop(isStackPush);
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }

            return original;
        }

        /// <summary>
        /// 访问成员值指定(未压栈)
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment, bool isStackPush = true)
        {
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }

            return assignment;
        }

        /// <summary>
        /// 访问成员绑定
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding, bool isStackPush = true)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        /// <summary>
        /// 访问成员集合绑定
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding, bool isStackPush = true)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        /// <summary>
        /// 访问绑定集合
        /// </summary>
        /// <param name="original"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original, bool isStackPush = true)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        /// <summary>
        /// 访问集合成员初始化
        /// </summary>
        /// <param name="original"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original, bool isStackPush = true)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        /// <summary>
        /// 访问表达式
        /// </summary>
        /// <param name="lambda"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda, bool isStackPush = true)
        {
            this.StackPush(lambda, isStackPush);

            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }

            this.StackPop(isStackPush);
            return lambda;
        }

        /// <summary>
        /// 访问构造表达式
        /// </summary>
        /// <param name="nex"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual NewExpression VisitNew(NewExpression nex, bool isStackPush = true)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }
            return nex;
        }

        /// <summary>
        /// 访问成员初始化
        /// </summary>
        /// <param name="init"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init, bool isStackPush = true)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        /// <summary>
        /// 访问集合初始化
        /// </summary>
        /// <param name="init"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitListInit(ListInitExpression init, bool isStackPush = true)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }

        /// <summary>
        /// 访问构造数组
        /// </summary>
        /// <param name="na"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na, bool isStackPush = true)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;
        }

        /// <summary>
        /// 访问調用
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv, bool isStackPush = true)
        {
            this.StackPush(iv, isStackPush);

            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }

            this.StackPop(isStackPush);
            return iv;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 是否表达式压栈
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="isStackPush"></param>
        private void StackPush(Expression exp, bool isStackPush)
        {
            if (isStackPush)
                this.ExpressionChains.Push(exp);
        }

        /// <summary>
        /// 是否表达式出栈
        /// </summary>
        /// <param name="isStackPush"></param>
        private void StackPop(bool isStackPush)
        {
            if (isStackPush)
                this.ExpressionChains.Pop();
        }

        #endregion
    }
}
