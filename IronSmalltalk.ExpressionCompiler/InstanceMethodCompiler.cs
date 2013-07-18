using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.ExpressionCompiler.BindingScopes;
using IronSmalltalk.ExpressionCompiler.Visiting;
using IronSmalltalk.Runtime;
using IronSmalltalk.Runtime.Bindings;

namespace IronSmalltalk.ExpressionCompiler
{
    public sealed class InstanceMethodCompiler : MethodCompiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runtime">Smalltalk runtime responsible for running the code.</param>
        /// <param name="compilerOptions">Options that control the workings of the compiler.</param>
        public InstanceMethodCompiler(SmalltalkRuntime runtime, CompilerOptions compilerOptions)
            : base(runtime, compilerOptions)
        {
        }

        protected override VisitingContext GetVisitingContext(SmalltalkClass cls, Expression self, Expression[] arguments)
        {
            SmalltalkNameScope globalNameScope = this.CompilerOptions.GlobalNameScope ?? this.Runtime.GlobalScope;

            BindingScope globalScope = BindingScope.ForInstanceMethod(cls, globalNameScope);
            BindingScope reservedScope = (cls.Superclass == null) ?
                ReservedScope.ForRootClassInstanceMethod() :
                ReservedScope.ForInstanceMethod();

            return new VisitingContext(this, globalScope, reservedScope, self, arguments[0], arguments.Skip(1), cls.Name);
        }
    }
}
