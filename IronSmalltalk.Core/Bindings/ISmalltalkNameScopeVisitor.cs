﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.Runtime.Bindings
{
    public interface ISmalltalkNameScopeVisitor
    {
        void Visit(Symbol protectedName);
        void Visit(ClassBinding binding);
        void Visit(PoolBinding binding);
        void Visit(GlobalVariableBinding binding);
        void Visit(GlobalConstantBinding binding);
    }
}