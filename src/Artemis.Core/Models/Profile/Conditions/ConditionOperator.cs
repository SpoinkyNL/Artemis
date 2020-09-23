﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Artemis.Core
{
    /// <summary>
    ///     A condition operator is used by the conditions system to perform a specific boolean check
    /// </summary>
    public abstract class ConditionOperator
    {
        /// <summary>
        ///     Gets the plugin info this condition operator belongs to
        ///     <para>Note: Not set until after registering</para>
        /// </summary>
        public PluginInfo PluginInfo { get; internal set; }

        /// <summary>
        ///     Gets the types this operator supports
        /// </summary>
        public abstract IReadOnlyCollection<Type> CompatibleTypes { get; }

        /// <summary>
        ///     Gets or sets the description of this logical operator
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Gets or sets the icon of this logical operator
        /// </summary>
        public abstract string Icon { get; }

        /// <summary>
        ///     Gets or sets whether this condition operator supports a right side, defaults to true
        /// </summary>
        public bool SupportsRightSide { get; protected set; } = true;

        /// <summary>
        ///     Returns whether the given type is supported by the operator
        /// </summary>
        public bool SupportsType(Type type)
        {
            if (type == null)
                return true;
            return CompatibleTypes.Any(t => t.IsCastableFrom(type));
        }

        /// <summary>
        ///     Creates a binary expression comparing two types
        /// </summary>
        /// <param name="leftSide">The parameter on the left side of the expression</param>
        /// <param name="rightSide">The parameter on the right side of the expression</param>
        /// <returns></returns>
        public abstract BinaryExpression CreateExpression(Expression leftSide, Expression rightSide);

        /// <summary>
        ///     Wraps the provided expression in null-checks for the left and right side
        ///     <para>
        ///         The resulting expression body looks like:
        ///         <code>(a == null &amp;&amp; b == null) || ((a != null &amp;&amp; b != null) &amp;&amp; &lt;expression&gt;)</code>
        ///     </para>
        /// </summary>
        /// <param name="leftSide">The left side to check for nulls</param>
        /// <param name="rightSide">The right side to check for nulls</param>
        /// <param name="expression">The expression to wrap</param>
        /// <returns>The wrapped expression</returns>
        protected BinaryExpression AddNullChecks(Expression leftSide, Expression rightSide, BinaryExpression expression)
        {
            var nullConst = Expression.Constant(null);
            return Expression.OrElse(
                Expression.AndAlso(
                    Expression.Equal(leftSide, nullConst),
                    Expression.Equal(rightSide, nullConst)
                ),
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.NotEqual(leftSide, nullConst),
                        Expression.NotEqual(rightSide, nullConst)
                    ),
                    expression
                )
            );
        }
    }
}