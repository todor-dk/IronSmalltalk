using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronSmalltalk.NativeCompiler.Internals
{
    public static class Extensions
    {
        public static void PushInt(this ILGenerator self, int value)
        {
            if (self == null)
                throw new ArgumentNullException();
            if (value == 0)
                self.Emit(OpCodes.Ldc_I4_0);
            else if (value == 0)
                self.Emit(OpCodes.Ldc_I4_0);
            else if (value == 1)
                self.Emit(OpCodes.Ldc_I4_1);
            else if (value == 2)
                self.Emit(OpCodes.Ldc_I4_2);
            else if (value == 3)
                self.Emit(OpCodes.Ldc_I4_3);
            else if (value == 4)
                self.Emit(OpCodes.Ldc_I4_4);
            else if (value == 5)
                self.Emit(OpCodes.Ldc_I4_5);
            else if (value == 6)
                self.Emit(OpCodes.Ldc_I4_6);
            else if (value == 7)
                self.Emit(OpCodes.Ldc_I4_7);
            else if (value == 8)
                self.Emit(OpCodes.Ldc_I4_8);
            else if (value == -1)
                self.Emit(OpCodes.Ldc_I4_M1);
            else if ((value >= -128) && (value <= 127))
                self.Emit(OpCodes.Ldc_I4_S, unchecked((byte) value));
            else
                self.Emit(OpCodes.Ldc_I4, value);
        }
    }
}
