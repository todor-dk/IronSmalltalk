/*
 * **************************************************************************
 *
 * Copyright (c) The IronSmalltalk Project. 
 *
 * This source code is subject to terms and conditions of the 
 * license agreement found in the solution directory. 
 * See: $(SolutionDir)\License.htm ... in the root of this distribution.
 * By using this source code in any fashion, you are agreeing 
 * to be bound by the terms of the license agreement.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * **************************************************************************
*/

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using IronSmalltalk.Compiler.SemanticAnalysis;
using IronSmalltalk.ExpressionCompiler.Bindings;
using IronSmalltalk.ExpressionCompiler.Internals;
using IronSmalltalk.ExpressionCompiler.Primitives;
using IronSmalltalk.Runtime.Execution;
using IronSmalltalk.Runtime.Execution.CallSiteBinders;

namespace IronSmalltalk.ExpressionCompiler.Visiting
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// == 
    /// basicAt: 
    /// basicAt:put:
    /// basicSize 
    /// basicNew:
    /// </remarks>
    public class MessageVisitor : NestedEncoderVisitor<Expression>
    {
        public Expression Receiver { get; private set; }
        public bool IsSuperSend { get; private set; }
        public bool IsConstantReceiver { get; private set; }
        public ParameterExpression CascadeReceiver { get; private set; }
        public bool CascadeSuperSend { get; private set; }
        public bool CascadeConstantReceiver { get; private set; }

        public MessageVisitor(EncoderVisitor parentVisitor, Expression receiver, bool superSend, bool constantReceiver, bool hasCascadeMessages)
            : base(parentVisitor)
        {
            if (receiver == null)
                throw new ArgumentNullException();
            this.Receiver = receiver;
            this.IsSuperSend = superSend;
            this.IsConstantReceiver = constantReceiver;
            this.CascadeConstantReceiver = false;
            if (hasCascadeMessages)
                this.CascadeReceiver = Expression.Variable(typeof(object), "_cascadeSelf");
            else
                this.CascadeReceiver = null;
            this.CascadeSuperSend = false;
        }

        #region Message Sequences

        #region MessageSequenceNode implementations

        public override Expression VisitBinaryKeywordMessageSequence(Compiler.SemanticNodes.BinaryKeywordMessageSequenceNode node)
        {
            if (node.NextMessage == null)
                this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            if (node.NextMessage != null)
                return node.NextMessage.Accept(this);
            else
                return this.Receiver;
        }

        public override Expression VisitKeywordMessageSequence(Compiler.SemanticNodes.KeywordMessageSequenceNode node)
        {
            Expression expr = this.InlineMessageSend(node);
            if (expr != null)
                return expr;
            this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            return this.Receiver;
        }

        public override Expression VisitUnaryBinaryKeywordMessageSequence(Compiler.SemanticNodes.UnaryBinaryKeywordMessageSequenceNode node)
        {
            if (node.NextMessage == null)
                this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            if (node.NextMessage != null)
                return node.NextMessage.Accept(this);
            else
                return this.Receiver;
        }

        #endregion

        #region Argument message sequences

        public override Expression VisitUnaryMessageSequence(Compiler.SemanticNodes.UnaryMessageSequenceNode node)
        {
            if (node.NextMessage == null)
                this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            if (node.NextMessage != null)
                return node.NextMessage.Accept(this);
            else
                return this.Receiver;
        }

        public override Expression VisitBinaryMessageSequence(Compiler.SemanticNodes.BinaryMessageSequenceNode node)
        {
            if (node.NextMessage == null)
                this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            if (node.NextMessage != null)
                return node.NextMessage.Accept(this);
            else
                return this.Receiver;
        }

        public override Expression VisitUnaryBinaryMessageSequence(Compiler.SemanticNodes.UnaryBinaryMessageSequenceNode node)
        {
            if (node.NextMessage == null)
                this.SetCascadeReceiver();
            this.SetResult(node.Message.Accept(this));
            if (node.NextMessage != null)
                return node.NextMessage.Accept(this);
            else
                return this.Receiver;
        }

        #endregion

        #endregion

        #region Message Nodes

        public override Expression VisitUnaryMessage(Compiler.SemanticNodes.UnaryMessageNode node)
        {
            return this.Context.CompileDynamicCall(this, node.SelectorToken.Value, node.SelectorToken.Value,
                this.IsSuperSend, this.IsConstantReceiver, this.Receiver);
        }

        public override Expression VisitBinaryMessage(Compiler.SemanticNodes.BinaryMessageNode node)
        {
            Expression argument = node.Argument.Accept(this);
            if (node.SelectorToken.Value == "==")
                return this.InlineIdentityTest(this.Receiver, argument);
            else
                return this.Context.CompileDynamicCall(this, node.SelectorToken.Value, node.SelectorToken.Value,
                    this.IsSuperSend, this.IsConstantReceiver, this.Receiver, argument);
        }

        public override Expression VisitKeywordMessage(Compiler.SemanticNodes.KeywordMessageNode node)
        {
            StringBuilder selector = new StringBuilder();
            foreach (var token in node.SelectorTokens)
                selector.Append(token.Value);

            Expression[] arguments = new Expression[node.Arguments.Count];
            for (int i = 0; i < node.Arguments.Count; i++)
                arguments[i] = node.Arguments[i].Accept(this);

            return this.Context.CompileDynamicCall(
                this,
                selector.ToString(), 
                node.SelectorTokens[0].Value.Substring(0, node.SelectorTokens[0].Value.Length - 1),
                node.Arguments.Count,
                this.IsSuperSend, 
                this.IsConstantReceiver, 
                this.Receiver,
                arguments);
        }

        #endregion

        #region Message Arguments

        public override Expression VisitBinaryArgument(Compiler.SemanticNodes.BinaryArgumentNode node)
        {
            PrimaryVisitor visitor = new PrimaryVisitor(this);
            Expression result = node.Primary.Accept(visitor);

            if (node.Messages != null)
                result = node.Messages.Accept(new MessageVisitor(this, result, visitor.IsSuperSend, visitor.IsConstant, false));
            else if (visitor.IsSuperSend)
                throw (new SemanticCodeGenerationException(CodeGenerationErrors.SuperNotFollowedByMessage)).SetErrorLocation(node);

            return result;
        }

        public override Expression VisitKeywordArgument(Compiler.SemanticNodes.KeywordArgumentNode node)
        {
            PrimaryVisitor visitor = new PrimaryVisitor(this);
            Expression result = node.Primary.Accept(visitor);

            if (node.Messages != null)
                result = node.Messages.Accept(new MessageVisitor(this, result, visitor.IsSuperSend, visitor.IsConstant, false));
            else if (visitor.IsSuperSend)
                throw (new SemanticCodeGenerationException(CodeGenerationErrors.SuperNotFollowedByMessage)).SetErrorLocation(node);

            return result;
        }

        #endregion

        #region Helpers





        private void SetResult(Expression receiver)
        {
            this.Receiver = receiver;
            this.IsSuperSend = false; // Only the first message may be sent to super
            this.IsConstantReceiver = false; //  Only the first message can be concidered as having constant receiver
        }

        private void SetCascadeReceiver()
        {
            if (this.CascadeReceiver == null)
                return;
            this.Receiver = Expression.Assign(this.CascadeReceiver, this.Receiver);
            this.CascadeConstantReceiver = this.IsConstantReceiver;
            this.CascadeSuperSend = this.IsSuperSend;
        }

        #endregion

        #region Inlining

        private Expression InlineIdentityTest(Expression a, Expression b)
        {
            return PrimitiveEncoder.EncodeReferenceEquals(a, b);
        }

        private Expression InlineMessageSend(Compiler.SemanticNodes.KeywordMessageSequenceNode node)
        {
            if (this.CascadeReceiver != null)
                return null;
            if (this.IsSuperSend)
                return null;
            if ((node.Message.SelectorTokens.Count == 1) && (node.Message.Arguments.Count == 1))
            {
                Compiler.SemanticNodes.BlockNode block = node.Message.Arguments[0].Primary as Compiler.SemanticNodes.BlockNode;
                if ((block != null) && (block.Arguments.Count == 0) && (node.Message.Arguments[0].Messages == null))
                {
                    if (node.Message.SelectorTokens[0].Value == "ifTrue:")
                        return this.InlineIfTrueIfFalse(block, null);
                    if (node.Message.SelectorTokens[0].Value == "ifFalse:")
                        return this.InlineIfTrueIfFalse(null, block);
                    if (node.Message.SelectorTokens[0].Value == "and:")
                        return this.InlineAnd(block);
                    if (node.Message.SelectorTokens[0].Value == "or:")
                        return this.InlineOr(block);
                    if (node.Message.SelectorTokens[0].Value == "timesRepeat:")
                        return this.InlineTimesRepeat(block);
                }
            }
            else if ((node.Message.SelectorTokens.Count == 2) && (node.Message.Arguments.Count == 2))
            {
                Compiler.SemanticNodes.BlockNode block1 = node.Message.Arguments[0].Primary as Compiler.SemanticNodes.BlockNode;
                Compiler.SemanticNodes.BlockNode block2 = node.Message.Arguments[1].Primary as Compiler.SemanticNodes.BlockNode;
                if ((block1 != null) && (block1.Arguments.Count == 0) && (node.Message.Arguments[0].Messages == null))
                {
                    if ((block2 != null) && (block2.Arguments.Count == 0) && (node.Message.Arguments[1].Messages == null))
                    {
                        if ((node.Message.SelectorTokens[0].Value == "ifTrue:") && (node.Message.SelectorTokens[1].Value == "ifFalse:"))
                            return this.InlineIfTrueIfFalse(block1, block2);
                        if ((node.Message.SelectorTokens[0].Value == "ifFalse:") && (node.Message.SelectorTokens[1].Value == "ifTrue:"))
                            return this.InlineIfTrueIfFalse(block2, block1);
                    }
                }
                if ((block2 != null) && (block2.Arguments.Count == 1) && (node.Message.Arguments[1].Messages == null))
                {
                    if ((node.Message.SelectorTokens[0].Value == "to:") && (node.Message.SelectorTokens[1].Value == "do:"))
                        return this.InlineToDo(node.Message.Arguments[0], block2);
                }
            }
            else if ((node.Message.SelectorTokens.Count == 3) && (node.Message.Arguments.Count == 3))
            {
                Compiler.SemanticNodes.BlockNode block = node.Message.Arguments[2].Primary as Compiler.SemanticNodes.BlockNode;
                if ((block != null) && (block.Arguments.Count == 1) && (node.Message.Arguments[2].Messages == null))
                {
                    if ((node.Message.SelectorTokens[0].Value == "to:") && (node.Message.SelectorTokens[1].Value == "by:") && (node.Message.SelectorTokens[2].Value == "do:"))
                        return this.InlineToByDo(node.Message.Arguments[0], node.Message.Arguments[1], block);
                }
            }
            return null;
        }

        private Expression InlineIfTrueIfFalse(Compiler.SemanticNodes.BlockNode trueBlock, Compiler.SemanticNodes.BlockNode falseBlock)
        {
            NameBinding nilBinding = this.Context.GetBinding(SemanticConstants.Nil);
            Expression testExpression = Expression.Convert(this.Receiver, typeof(bool));
            Expression trueExpression;
            if (trueBlock != null)
                trueExpression = trueBlock.Accept(new InlineBlockVisitor(this));
            else
                trueExpression = nilBinding.GenerateReadExpression(this);
            Expression falseExpression;
            if (falseBlock != null)
                falseExpression = falseBlock.Accept(new InlineBlockVisitor(this));
            else
                falseExpression = nilBinding.GenerateReadExpression(this);

            Expression result = Expression.Condition(testExpression, trueExpression, falseExpression, typeof(object));
            this.SetResult(result);
            return result;
        }

        private Expression InlineAnd(Compiler.SemanticNodes.BlockNode block)
        {
            NameBinding falseBinding = this.Context.GetBinding(SemanticConstants.False);
            Expression testExpression = Expression.Convert(this.Receiver, typeof(bool));
            Expression blockExpression = block.Accept(new InlineBlockVisitor(this));
            Expression result = Expression.Condition(testExpression, blockExpression, falseBinding.GenerateReadExpression(this), typeof(object));
            this.SetResult(result);
            return result;
        }

        private Expression InlineOr(Compiler.SemanticNodes.BlockNode block)
        {
            NameBinding trueBinding = this.Context.GetBinding(SemanticConstants.True);
            Expression testExpression = Expression.Convert(this.Receiver, typeof(bool));
            Expression blockExpression = block.Accept(new InlineBlockVisitor(this));
            Expression result = Expression.Condition(testExpression, trueBinding.GenerateReadExpression(this), blockExpression, typeof(object));
            this.SetResult(result);
            return result;

        }

        private Expression InlineTimesRepeat(Compiler.SemanticNodes.BlockNode block)
        {
            /// timesRepeat: aBlock
            /// Inlines to:
            ///     1 to: self by: 1 do: aBlock
            return this.InlineToByDo(
                Expression.Constant(1, typeof(int)),
                this.Receiver,
                Expression.Constant(1, typeof(int)),
                block);
        }

        private Expression InlineToDo(Compiler.SemanticNodes.KeywordArgumentNode stop, Compiler.SemanticNodes.BlockNode block)
        {
            /// to: stop do: aBlock
            /// Inlines to:
            ///     self to: stop by: 1 do: aBlock
            return this.InlineToByDo(this.Receiver, stop.Accept(this), Expression.Constant(1, typeof(int)), block);
        }

        private Expression InlineToByDo(Compiler.SemanticNodes.KeywordArgumentNode stop, Compiler.SemanticNodes.KeywordArgumentNode by, Compiler.SemanticNodes.BlockNode block)
        {
            return this.InlineToByDo(this.Receiver, stop.Accept(this), by.Accept(this), block);
        }

        private Expression InlineToByDo(Expression start, Expression stop, Expression step, Compiler.SemanticNodes.BlockNode block)
        {
            /// to:by:do:
            /* C# semantics of the inlining
            if ((start is int) && (stop is int) && (step is int))
            {
                int iStart, iStop, iStep;
                try
                {
                    iStart = checked((int)start);
                    iStop = checked((int)stop);
                    iStep = checked((int)step);
                    // This is to ensure the calculation below do not fail running unchecked
                    int na = checked(iStart + iStep);
                    na = checked(iStop + iStep);
                }
                catch (OverflowException)
                {
                    goto Int32Overwlow;
                }

                if (iStep == 0)
                    throw new ArgumentOutOfRangeException();

                if (iStep > 0)
                {
                    do
                    {
                        if (iStart > iStop)
                            goto Exit;
                        block(iStart);
                        iStart = iStart + iStep;
                    } while (true);
                }
                else
                {
                    do
                    {
                        if (iStop > iStart)
                            goto Exit;
                        block(iStart);
                        iStart = iStart + iStep;
                    } while (true);
                }
            }
        Int32Overwlow:
            // We could implement BigInteger optimization here, but too much work and probably not much to gain
            // Fallback to dynamic invocation
            dynamic dStart = start;
            dynamic dStep = step;
            if (dStep == 0)
                throw new ArgumentOutOfRangeException();

            if (dStep > 0)
            {
                do
                {
                    if (dStart > stop)
                        goto Exit;
                    block(dStart);
                    dStart = dStart + step;
                } while (true);
            }
            else
            {
                do
                {
                    if (stop > dStart)
                        goto Exit;
                    block(dStart);
                    dStart = dStart + step;
                } while (true);
            }
        Exit:
             */

            NameBinding nilBinding = this.Context.GetBinding(SemanticConstants.Nil);
            LabelTarget exitLabel = Expression.Label(typeof(void));
            LabelTarget overflowLabel = Expression.Label(typeof(void));

            ParameterExpression iStart = Expression.Variable(typeof(int), "iStart");
            ParameterExpression iStop = Expression.Variable(typeof(int), "iStop");
            ParameterExpression iStep = Expression.Variable(typeof(int), "iStep");
            ParameterExpression iNA = Expression.Variable(typeof(int), "iNA");
            InlineBlockVisitor visitor = new InlineBlockVisitor(this);
            if (block.Arguments.Count > 0)
                visitor.DefineExternalArgument(block.Arguments[0].Token.Value, Expression.Convert(iStart, typeof(object)));
            Expression integerOperation = block.Accept(visitor);

            Expression integerTest = Expression.AndAlso(Expression.AndAlso(
                Expression.TypeIs(start, typeof(int)), Expression.TypeIs(stop, typeof(int))), Expression.TypeIs(step, typeof(int)));

            Expression integerBlock = Expression.Block(
                Expression.TryCatch(
                    Expression.Block(
                        Expression.Assign(iStart, Expression.ConvertChecked(start, typeof(int))),
                        Expression.Assign(iStop, Expression.ConvertChecked(stop, typeof(int))),
                        Expression.Assign(iStep, Expression.ConvertChecked(step, typeof(int))),
                        Expression.Assign(iNA, Expression.AddChecked(iStart, iStep)),
                        Expression.Assign(iNA, Expression.AddChecked(iStop, iStep))),
                    Expression.Catch(typeof(OverflowException), Expression.Goto(overflowLabel, typeof(int))),
                    Expression.Catch(typeof(InvalidCastException), Expression.Goto(overflowLabel, typeof(int)))),
                Expression.IfThen(
                    Expression.Equal(iStep, Expression.Constant(0)),
                    Expression.Throw(Expression.New(typeof(ArgumentOutOfRangeException)), typeof(object))),
                Expression.IfThenElse(
                    Expression.GreaterThan(iStep, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.GreaterThan(iStart, iStop),
                            Expression.Break(exitLabel, nilBinding.GenerateReadExpression(this)),
                            Expression.Block(
                                integerOperation,
                                Expression.AddAssign(iStart, iStep)))),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.GreaterThan(iStop, iStart),
                            Expression.Break(exitLabel, nilBinding.GenerateReadExpression(this)),
                            Expression.Block(
                                integerOperation,
                                Expression.AddAssign(iStart, iStep))))));

            ParameterExpression dStart = Expression.Variable(typeof(object), "dStart");
            ParameterExpression dStep = Expression.Variable(typeof(object), "dStep");
            visitor = new InlineBlockVisitor(this);
            if (block.Arguments.Count > 0)
                visitor.DefineExternalArgument(block.Arguments[0].Token.Value, dStart);
            Expression dynamicOperation = block.Accept(visitor);
            
            Expression dynamicBlock = Expression.Block(
                Expression.Assign(dStart, start),
                Expression.Assign(dStep, Expression.Convert(step, typeof(object))),
                Expression.IfThen(
                    Expression.IsTrue(Expression.Convert(this.Context.CompileDynamicCall(this, "=", dStep, PreboxedConstants.Int32_00000000_Expression), typeof(bool))),
                    Expression.Throw(Expression.New(typeof(ArgumentOutOfRangeException)), typeof(object))),
                Expression.IfThenElse(
                    Expression.IsTrue(Expression.Convert(this.Context.CompileDynamicCall(this, ">", dStep, PreboxedConstants.Int32_00000000_Expression), typeof(bool))),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.IsTrue(Expression.Convert(this.Context.CompileDynamicCall(this, ">", dStart, stop), typeof(bool))),
                            Expression.Break(exitLabel, nilBinding.GenerateReadExpression(this)),
                            Expression.Block(
                                dynamicOperation,
                                Expression.Assign(dStart, this.Context.CompileDynamicCall(this, "+", dStart, dStep))))),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.IsTrue(Expression.Convert(this.Context.CompileDynamicCall(this, ">", stop, dStart), typeof(bool))),
                            Expression.Break(exitLabel, nilBinding.GenerateReadExpression(this)),
                            Expression.Block(
                                dynamicOperation,
                                Expression.Assign(dStart, this.Context.CompileDynamicCall(this, "+", dStart, dStep)))))));

            return Expression.Block(
                new ParameterExpression[] { iStart, iStop, iStep, iNA, dStart, dStep },
                integerBlock,
                Expression.Label(overflowLabel),
                dynamicBlock,
                Expression.Label(exitLabel),
                nilBinding.GenerateReadExpression(this));
        }

        #endregion
    }
}
