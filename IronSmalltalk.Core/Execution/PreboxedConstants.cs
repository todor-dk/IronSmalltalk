using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using IronSmalltalk.Common;

namespace IronSmalltalk.Runtime.Execution
{
    public static class PreboxedConstants
    {
        static PreboxedConstants()
        {
            // At that point the inline initializers should have run, so it's safe to run the rest.
            PreboxedConstants.Char_Objects = PreboxedConstants.Get_Char_Objects();
            PreboxedConstants.Char_Expressions = PreboxedConstants.Get_Char_Expressions();
            PreboxedConstants.Int32_Objects = PreboxedConstants.Get_Int32_Objects();
            PreboxedConstants.Int32_Expressions = PreboxedConstants.Get_Int32_Expressions();
        }

        public static readonly Expression Nil_Expression = Expression.Constant(null, typeof(object));

        #region Boolean

        public static readonly object True = true;
        public static readonly Expression True_Expression = Expression.Field(null, typeof(PreboxedConstants), "True");

        public static readonly object False = false;
        public static readonly Expression False_Expression = Expression.Field(null, typeof(PreboxedConstants), "False");

        #endregion

        #region Single Float

        private const Single Single_PI_Value = (Single)Math.PI;
        private const Single Single_E_Value = (Single)Math.E;

        public static Expression GetConstant(Single value)
        {
            if (value == 0.0F)
                return PreboxedConstants.Single_Zero_Expression;
            if (value == 0.5F)
                return PreboxedConstants.Single_Half_Expression;
            if (value == 1.0F)
                return PreboxedConstants.Single_One_Expression;
            if (value == 2.0F)
                return PreboxedConstants.Single_Two_Expression;
            if (value == -0.5F)
                return PreboxedConstants.Single_MinusHalf_Expression;
            if (value == -1.0F)
                return PreboxedConstants.Single_MinusOne_Expression;
            if (value == -2.0F)
                return PreboxedConstants.Single_MinusTwo_Expression;
            if (value == PreboxedConstants.Single_PI_Value)
                return PreboxedConstants.Single_PI_Expression;
            if (value == PreboxedConstants.Single_E_Value)
                return PreboxedConstants.Single_E_Expression;
            return null;
        }

        public static object GetValue(Single value)
        {
            if (value == 0.0F)
                return PreboxedConstants.Single_Zero;
            if (value == 0.5F)
                return PreboxedConstants.Single_Half;
            if (value == 1.0F)
                return PreboxedConstants.Single_One;
            if (value == 2.0F)
                return PreboxedConstants.Single_Two;
            if (value == -0.5F)
                return PreboxedConstants.Single_MinusHalf;
            if (value == -1.0F)
                return PreboxedConstants.Single_MinusOne;
            if (value == -2.0F)
                return PreboxedConstants.Single_MinusTwo;
            if (value == PreboxedConstants.Single_PI_Value)
                return PreboxedConstants.Single_PI;
            if (value == PreboxedConstants.Single_E_Value)
                return PreboxedConstants.Single_E;
            return null;
        }

        /// <summary>
        /// A singleton boxed single float 0.0.
        /// </summary>
        public static readonly object Single_Zero = 0.0F;

        /// <summary>
        /// Expression to return the singleton boxed single float 0.0.
        /// </summary>
        public static readonly Expression Single_Zero_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_Zero");

        /// <summary>
        /// A singleton boxed single float 0.5.
        /// </summary>
        public static readonly object Single_Half = 0.5F;

        /// <summary>
        /// Expression to return the singleton boxed single float 0.5.
        /// </summary>
        public static readonly Expression Single_Half_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_Half");

        /// <summary>
        /// A singleton boxed single float 1.0.
        /// </summary>
        public static readonly object Single_One = 1.0F;

        /// <summary>
        /// Expression to return the singleton boxed single float 1.0.
        /// </summary>
        public static readonly Expression Single_One_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_One");

        /// <summary>
        /// A singleton boxed single float 2.0.
        /// </summary>
        public static readonly object Single_Two = 2.0F;

        /// <summary>
        /// Expression to return the singleton boxed single float 2.0.
        /// </summary>
        public static readonly Expression Single_Two_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_Two");

        /// <summary>
        /// A singleton boxed single float -0.5.
        /// </summary>
        public static readonly object Single_MinusHalf = -0.5F;

        /// <summary>
        /// Expression to return the singleton boxed single float -0.5.
        /// </summary>
        public static readonly Expression Single_MinusHalf_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_MinusHalf");

        /// <summary>
        /// A singleton boxed single float -1.0.
        /// </summary>
        public static readonly object Single_MinusOne = -1.0F;

        /// <summary>
        /// Expression to return the singleton boxed single float -1.0.
        /// </summary>
        public static readonly Expression Single_MinusOne_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_MinusOne");

        /// <summary>
        /// A singleton boxed single float -2.0.
        /// </summary>
        public static readonly object Single_MinusTwo = -2.0F;

        /// <summary>
        /// Expression to return the singleton boxed single float -2.0.
        /// </summary>
        public static readonly Expression Single_MinusTwo_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_MinusTwo");

        /// <summary>
        /// A singleton boxed single float PI (3.14159...).
        /// </summary>
        public static readonly object Single_PI = PreboxedConstants.Single_PI_Value;

        /// <summary>
        /// Expression to return the singleton boxed single float PI (3.14159...).
        /// </summary>
        public static readonly Expression Single_PI_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_PI");

        /// <summary>
        /// A singleton boxed single float E (2.71828...).
        /// </summary>
        public static readonly object Single_E = PreboxedConstants.Single_E_Value;

        /// <summary>
        /// Expression to return the singleton boxed single float E (2.71828...).
        /// </summary>
        public static readonly Expression Single_E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Single_E");

        #endregion

        #region Dougle Float

        public static Expression GetConstant(Double value)
        {
            if (value == 0.0D)
                return PreboxedConstants.Double_Zero_Expression;
            if (value == 0.5D)
                return PreboxedConstants.Double_Half_Expression;
            if (value == 1.0D)
                return PreboxedConstants.Double_One_Expression;
            if (value == 2.0D)
                return PreboxedConstants.Double_Two_Expression;
            if (value == -0.5D)
                return PreboxedConstants.Double_MinusHalf_Expression;
            if (value == -1.0D)
                return PreboxedConstants.Double_MinusOne_Expression;
            if (value == -2.0D)
                return PreboxedConstants.Double_MinusTwo_Expression;
            if (value == Math.PI)
                return PreboxedConstants.Double_PI_Expression;
            if (value == Math.E)
                return PreboxedConstants.Double_E_Expression;
            return null;
        }

        public static object GetValue(Double value)
        {
            if (value == 0.0D)
                return PreboxedConstants.Double_Zero;
            if (value == 0.5D)
                return PreboxedConstants.Double_Half;
            if (value == 1.0D)
                return PreboxedConstants.Double_One;
            if (value == 2.0D)
                return PreboxedConstants.Double_Two;
            if (value == -0.5D)
                return PreboxedConstants.Double_MinusHalf;
            if (value == -1.0D)
                return PreboxedConstants.Double_MinusOne;
            if (value == -2.0D)
                return PreboxedConstants.Double_MinusTwo;
            if (value == Math.PI)
                return PreboxedConstants.Double_PI;
            if (value == Math.E)
                return PreboxedConstants.Double_E;
            return null;
        }

        /// <summary>
        /// A singleton boxed double float 0.0.
        /// </summary>
        public static readonly object Double_Zero = 0.0D;

        /// <summary>
        /// Expression to return the singleton boxed double float 0.0.
        /// </summary>
        public static readonly Expression Double_Zero_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_Zero");

        /// <summary>
        /// A singleton boxed double float 0.5.
        /// </summary>
        public static readonly object Double_Half = 0.5D;

        /// <summary>
        /// Expression to return the singleton boxed double float 0.5.
        /// </summary>
        public static readonly Expression Double_Half_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_Half");

        /// <summary>
        /// A singleton boxed double float 1.0.
        /// </summary>
        public static readonly object Double_One = 1.0D;

        /// <summary>
        /// Expression to return the singleton boxed double float 1.0.
        /// </summary>
        public static readonly Expression Double_One_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_One");

        /// <summary>
        /// A singleton boxed double float 2.0.
        /// </summary>
        public static readonly object Double_Two = 2.0D;

        /// <summary>
        /// Expression to return the singleton boxed double float 2.0.
        /// </summary>
        public static readonly Expression Double_Two_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_Two");

        /// <summary>
        /// A singleton boxed double float -0.5.
        /// </summary>
        public static readonly object Double_MinusHalf = -0.5D;

        /// <summary>
        /// Expression to return the singleton boxed double float -0.5.
        /// </summary>
        public static readonly Expression Double_MinusHalf_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_MinusHalf");

        /// <summary>
        /// A singleton boxed double float -1.0.
        /// </summary>
        public static readonly object Double_MinusOne = -1.0D;

        /// <summary>
        /// Expression to return the singleton boxed double float -1.0.
        /// </summary>
        public static readonly Expression Double_MinusOne_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_MinusOne");

        /// <summary>
        /// A singleton boxed double float -2.0.
        /// </summary>
        public static readonly object Double_MinusTwo = -2.0D;

        /// <summary>
        /// Expression to return the singleton boxed double float -2.0.
        /// </summary>
        public static readonly Expression Double_MinusTwo_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_MinusTwo");

        /// <summary>
        /// A singleton boxed double float PI (3.14159...).
        /// </summary>
        public static readonly object Double_PI = Math.PI;

        /// <summary>
        /// Expression to return the singleton boxed double float PI (3.14159...).
        /// </summary>
        public static readonly Expression Double_PI_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_PI");

        /// <summary>
        /// A singleton boxed double float E (2.71828...).
        /// </summary>
        public static readonly object Double_E = Math.E;

        /// <summary>
        /// Expression to return the singleton boxed double float E (2.71828...).
        /// </summary>
        public static readonly Expression Double_E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Double_E");

        #endregion

        #region Character

        public static Expression GetConstant(char value)
        {
            if (value <= '\x00FF')
                return PreboxedConstants.Char_Expressions[value];
            return null;
        }

        public static object GetValue(char value)
        {
            if (value <= '\x00FF')
                return PreboxedConstants.Char_Objects[value];
            return null;

        }

        private static readonly object[] Char_Objects;

        private static object[] Get_Char_Objects()
        {
            return new object[] 
            {
                PreboxedConstants.Char_0000,
                PreboxedConstants.Char_0001,
                PreboxedConstants.Char_0002,
                PreboxedConstants.Char_0003,
                PreboxedConstants.Char_0004,
                PreboxedConstants.Char_0005,
                PreboxedConstants.Char_0006,
                PreboxedConstants.Char_0007,
                PreboxedConstants.Char_0008,
                PreboxedConstants.Char_0009,
                PreboxedConstants.Char_000A,
                PreboxedConstants.Char_000B,
                PreboxedConstants.Char_000C,
                PreboxedConstants.Char_000D,
                PreboxedConstants.Char_000E,
                PreboxedConstants.Char_000F,
                PreboxedConstants.Char_0010,
                PreboxedConstants.Char_0011,
                PreboxedConstants.Char_0012,
                PreboxedConstants.Char_0013,
                PreboxedConstants.Char_0014,
                PreboxedConstants.Char_0015,
                PreboxedConstants.Char_0016,
                PreboxedConstants.Char_0017,
                PreboxedConstants.Char_0018,
                PreboxedConstants.Char_0019,
                PreboxedConstants.Char_001A,
                PreboxedConstants.Char_001B,
                PreboxedConstants.Char_001C,
                PreboxedConstants.Char_001D,
                PreboxedConstants.Char_001E,
                PreboxedConstants.Char_001F,
                PreboxedConstants.Char_0020,
                PreboxedConstants.Char_0021,
                PreboxedConstants.Char_0022,
                PreboxedConstants.Char_0023,
                PreboxedConstants.Char_0024,
                PreboxedConstants.Char_0025,
                PreboxedConstants.Char_0026,
                PreboxedConstants.Char_0027,
                PreboxedConstants.Char_0028,
                PreboxedConstants.Char_0029,
                PreboxedConstants.Char_002A,
                PreboxedConstants.Char_002B,
                PreboxedConstants.Char_002C,
                PreboxedConstants.Char_002D,
                PreboxedConstants.Char_002E,
                PreboxedConstants.Char_002F,
                PreboxedConstants.Char_0030,
                PreboxedConstants.Char_0031,
                PreboxedConstants.Char_0032,
                PreboxedConstants.Char_0033,
                PreboxedConstants.Char_0034,
                PreboxedConstants.Char_0035,
                PreboxedConstants.Char_0036,
                PreboxedConstants.Char_0037,
                PreboxedConstants.Char_0038,
                PreboxedConstants.Char_0039,
                PreboxedConstants.Char_003A,
                PreboxedConstants.Char_003B,
                PreboxedConstants.Char_003C,
                PreboxedConstants.Char_003D,
                PreboxedConstants.Char_003E,
                PreboxedConstants.Char_003F,
                PreboxedConstants.Char_0040,
                PreboxedConstants.Char_0041,
                PreboxedConstants.Char_0042,
                PreboxedConstants.Char_0043,
                PreboxedConstants.Char_0044,
                PreboxedConstants.Char_0045,
                PreboxedConstants.Char_0046,
                PreboxedConstants.Char_0047,
                PreboxedConstants.Char_0048,
                PreboxedConstants.Char_0049,
                PreboxedConstants.Char_004A,
                PreboxedConstants.Char_004B,
                PreboxedConstants.Char_004C,
                PreboxedConstants.Char_004D,
                PreboxedConstants.Char_004E,
                PreboxedConstants.Char_004F,
                PreboxedConstants.Char_0050,
                PreboxedConstants.Char_0051,
                PreboxedConstants.Char_0052,
                PreboxedConstants.Char_0053,
                PreboxedConstants.Char_0054,
                PreboxedConstants.Char_0055,
                PreboxedConstants.Char_0056,
                PreboxedConstants.Char_0057,
                PreboxedConstants.Char_0058,
                PreboxedConstants.Char_0059,
                PreboxedConstants.Char_005A,
                PreboxedConstants.Char_005B,
                PreboxedConstants.Char_005C,
                PreboxedConstants.Char_005D,
                PreboxedConstants.Char_005E,
                PreboxedConstants.Char_005F,
                PreboxedConstants.Char_0060,
                PreboxedConstants.Char_0061,
                PreboxedConstants.Char_0062,
                PreboxedConstants.Char_0063,
                PreboxedConstants.Char_0064,
                PreboxedConstants.Char_0065,
                PreboxedConstants.Char_0066,
                PreboxedConstants.Char_0067,
                PreboxedConstants.Char_0068,
                PreboxedConstants.Char_0069,
                PreboxedConstants.Char_006A,
                PreboxedConstants.Char_006B,
                PreboxedConstants.Char_006C,
                PreboxedConstants.Char_006D,
                PreboxedConstants.Char_006E,
                PreboxedConstants.Char_006F,
                PreboxedConstants.Char_0070,
                PreboxedConstants.Char_0071,
                PreboxedConstants.Char_0072,
                PreboxedConstants.Char_0073,
                PreboxedConstants.Char_0074,
                PreboxedConstants.Char_0075,
                PreboxedConstants.Char_0076,
                PreboxedConstants.Char_0077,
                PreboxedConstants.Char_0078,
                PreboxedConstants.Char_0079,
                PreboxedConstants.Char_007A,
                PreboxedConstants.Char_007B,
                PreboxedConstants.Char_007C,
                PreboxedConstants.Char_007D,
                PreboxedConstants.Char_007E,
                PreboxedConstants.Char_007F,
                PreboxedConstants.Char_0080,
                PreboxedConstants.Char_0081,
                PreboxedConstants.Char_0082,
                PreboxedConstants.Char_0083,
                PreboxedConstants.Char_0084,
                PreboxedConstants.Char_0085,
                PreboxedConstants.Char_0086,
                PreboxedConstants.Char_0087,
                PreboxedConstants.Char_0088,
                PreboxedConstants.Char_0089,
                PreboxedConstants.Char_008A,
                PreboxedConstants.Char_008B,
                PreboxedConstants.Char_008C,
                PreboxedConstants.Char_008D,
                PreboxedConstants.Char_008E,
                PreboxedConstants.Char_008F,
                PreboxedConstants.Char_0090,
                PreboxedConstants.Char_0091,
                PreboxedConstants.Char_0092,
                PreboxedConstants.Char_0093,
                PreboxedConstants.Char_0094,
                PreboxedConstants.Char_0095,
                PreboxedConstants.Char_0096,
                PreboxedConstants.Char_0097,
                PreboxedConstants.Char_0098,
                PreboxedConstants.Char_0099,
                PreboxedConstants.Char_009A,
                PreboxedConstants.Char_009B,
                PreboxedConstants.Char_009C,
                PreboxedConstants.Char_009D,
                PreboxedConstants.Char_009E,
                PreboxedConstants.Char_009F,
                PreboxedConstants.Char_00A0,
                PreboxedConstants.Char_00A1,
                PreboxedConstants.Char_00A2,
                PreboxedConstants.Char_00A3,
                PreboxedConstants.Char_00A4,
                PreboxedConstants.Char_00A5,
                PreboxedConstants.Char_00A6,
                PreboxedConstants.Char_00A7,
                PreboxedConstants.Char_00A8,
                PreboxedConstants.Char_00A9,
                PreboxedConstants.Char_00AA,
                PreboxedConstants.Char_00AB,
                PreboxedConstants.Char_00AC,
                PreboxedConstants.Char_00AD,
                PreboxedConstants.Char_00AE,
                PreboxedConstants.Char_00AF,
                PreboxedConstants.Char_00B0,
                PreboxedConstants.Char_00B1,
                PreboxedConstants.Char_00B2,
                PreboxedConstants.Char_00B3,
                PreboxedConstants.Char_00B4,
                PreboxedConstants.Char_00B5,
                PreboxedConstants.Char_00B6,
                PreboxedConstants.Char_00B7,
                PreboxedConstants.Char_00B8,
                PreboxedConstants.Char_00B9,
                PreboxedConstants.Char_00BA,
                PreboxedConstants.Char_00BB,
                PreboxedConstants.Char_00BC,
                PreboxedConstants.Char_00BD,
                PreboxedConstants.Char_00BE,
                PreboxedConstants.Char_00BF,
                PreboxedConstants.Char_00C0,
                PreboxedConstants.Char_00C1,
                PreboxedConstants.Char_00C2,
                PreboxedConstants.Char_00C3,
                PreboxedConstants.Char_00C4,
                PreboxedConstants.Char_00C5,
                PreboxedConstants.Char_00C6,
                PreboxedConstants.Char_00C7,
                PreboxedConstants.Char_00C8,
                PreboxedConstants.Char_00C9,
                PreboxedConstants.Char_00CA,
                PreboxedConstants.Char_00CB,
                PreboxedConstants.Char_00CC,
                PreboxedConstants.Char_00CD,
                PreboxedConstants.Char_00CE,
                PreboxedConstants.Char_00CF,
                PreboxedConstants.Char_00D0,
                PreboxedConstants.Char_00D1,
                PreboxedConstants.Char_00D2,
                PreboxedConstants.Char_00D3,
                PreboxedConstants.Char_00D4,
                PreboxedConstants.Char_00D5,
                PreboxedConstants.Char_00D6,
                PreboxedConstants.Char_00D7,
                PreboxedConstants.Char_00D8,
                PreboxedConstants.Char_00D9,
                PreboxedConstants.Char_00DA,
                PreboxedConstants.Char_00DB,
                PreboxedConstants.Char_00DC,
                PreboxedConstants.Char_00DD,
                PreboxedConstants.Char_00DE,
                PreboxedConstants.Char_00DF,
                PreboxedConstants.Char_00E0,
                PreboxedConstants.Char_00E1,
                PreboxedConstants.Char_00E2,
                PreboxedConstants.Char_00E3,
                PreboxedConstants.Char_00E4,
                PreboxedConstants.Char_00E5,
                PreboxedConstants.Char_00E6,
                PreboxedConstants.Char_00E7,
                PreboxedConstants.Char_00E8,
                PreboxedConstants.Char_00E9,
                PreboxedConstants.Char_00EA,
                PreboxedConstants.Char_00EB,
                PreboxedConstants.Char_00EC,
                PreboxedConstants.Char_00ED,
                PreboxedConstants.Char_00EE,
                PreboxedConstants.Char_00EF,
                PreboxedConstants.Char_00F0,
                PreboxedConstants.Char_00F1,
                PreboxedConstants.Char_00F2,
                PreboxedConstants.Char_00F3,
                PreboxedConstants.Char_00F4,
                PreboxedConstants.Char_00F5,
                PreboxedConstants.Char_00F6,
                PreboxedConstants.Char_00F7,
                PreboxedConstants.Char_00F8,
                PreboxedConstants.Char_00F9,
                PreboxedConstants.Char_00FA,
                PreboxedConstants.Char_00FB,
                PreboxedConstants.Char_00FC,
                PreboxedConstants.Char_00FD,
                PreboxedConstants.Char_00FE,
                PreboxedConstants.Char_00FF        
            };
        }

        private static readonly Expression[] Char_Expressions;

        private static Expression[] Get_Char_Expressions()
        {
            return new Expression[] 
            {
                PreboxedConstants.Char_0000_Expression,
                PreboxedConstants.Char_0001_Expression,
                PreboxedConstants.Char_0002_Expression,
                PreboxedConstants.Char_0003_Expression,
                PreboxedConstants.Char_0004_Expression,
                PreboxedConstants.Char_0005_Expression,
                PreboxedConstants.Char_0006_Expression,
                PreboxedConstants.Char_0007_Expression,
                PreboxedConstants.Char_0008_Expression,
                PreboxedConstants.Char_0009_Expression,
                PreboxedConstants.Char_000A_Expression,
                PreboxedConstants.Char_000B_Expression,
                PreboxedConstants.Char_000C_Expression,
                PreboxedConstants.Char_000D_Expression,
                PreboxedConstants.Char_000E_Expression,
                PreboxedConstants.Char_000F_Expression,
                PreboxedConstants.Char_0010_Expression,
                PreboxedConstants.Char_0011_Expression,
                PreboxedConstants.Char_0012_Expression,
                PreboxedConstants.Char_0013_Expression,
                PreboxedConstants.Char_0014_Expression,
                PreboxedConstants.Char_0015_Expression,
                PreboxedConstants.Char_0016_Expression,
                PreboxedConstants.Char_0017_Expression,
                PreboxedConstants.Char_0018_Expression,
                PreboxedConstants.Char_0019_Expression,
                PreboxedConstants.Char_001A_Expression,
                PreboxedConstants.Char_001B_Expression,
                PreboxedConstants.Char_001C_Expression,
                PreboxedConstants.Char_001D_Expression,
                PreboxedConstants.Char_001E_Expression,
                PreboxedConstants.Char_001F_Expression,
                PreboxedConstants.Char_0020_Expression,
                PreboxedConstants.Char_0021_Expression,
                PreboxedConstants.Char_0022_Expression,
                PreboxedConstants.Char_0023_Expression,
                PreboxedConstants.Char_0024_Expression,
                PreboxedConstants.Char_0025_Expression,
                PreboxedConstants.Char_0026_Expression,
                PreboxedConstants.Char_0027_Expression,
                PreboxedConstants.Char_0028_Expression,
                PreboxedConstants.Char_0029_Expression,
                PreboxedConstants.Char_002A_Expression,
                PreboxedConstants.Char_002B_Expression,
                PreboxedConstants.Char_002C_Expression,
                PreboxedConstants.Char_002D_Expression,
                PreboxedConstants.Char_002E_Expression,
                PreboxedConstants.Char_002F_Expression,
                PreboxedConstants.Char_0030_Expression,
                PreboxedConstants.Char_0031_Expression,
                PreboxedConstants.Char_0032_Expression,
                PreboxedConstants.Char_0033_Expression,
                PreboxedConstants.Char_0034_Expression,
                PreboxedConstants.Char_0035_Expression,
                PreboxedConstants.Char_0036_Expression,
                PreboxedConstants.Char_0037_Expression,
                PreboxedConstants.Char_0038_Expression,
                PreboxedConstants.Char_0039_Expression,
                PreboxedConstants.Char_003A_Expression,
                PreboxedConstants.Char_003B_Expression,
                PreboxedConstants.Char_003C_Expression,
                PreboxedConstants.Char_003D_Expression,
                PreboxedConstants.Char_003E_Expression,
                PreboxedConstants.Char_003F_Expression,
                PreboxedConstants.Char_0040_Expression,
                PreboxedConstants.Char_0041_Expression,
                PreboxedConstants.Char_0042_Expression,
                PreboxedConstants.Char_0043_Expression,
                PreboxedConstants.Char_0044_Expression,
                PreboxedConstants.Char_0045_Expression,
                PreboxedConstants.Char_0046_Expression,
                PreboxedConstants.Char_0047_Expression,
                PreboxedConstants.Char_0048_Expression,
                PreboxedConstants.Char_0049_Expression,
                PreboxedConstants.Char_004A_Expression,
                PreboxedConstants.Char_004B_Expression,
                PreboxedConstants.Char_004C_Expression,
                PreboxedConstants.Char_004D_Expression,
                PreboxedConstants.Char_004E_Expression,
                PreboxedConstants.Char_004F_Expression,
                PreboxedConstants.Char_0050_Expression,
                PreboxedConstants.Char_0051_Expression,
                PreboxedConstants.Char_0052_Expression,
                PreboxedConstants.Char_0053_Expression,
                PreboxedConstants.Char_0054_Expression,
                PreboxedConstants.Char_0055_Expression,
                PreboxedConstants.Char_0056_Expression,
                PreboxedConstants.Char_0057_Expression,
                PreboxedConstants.Char_0058_Expression,
                PreboxedConstants.Char_0059_Expression,
                PreboxedConstants.Char_005A_Expression,
                PreboxedConstants.Char_005B_Expression,
                PreboxedConstants.Char_005C_Expression,
                PreboxedConstants.Char_005D_Expression,
                PreboxedConstants.Char_005E_Expression,
                PreboxedConstants.Char_005F_Expression,
                PreboxedConstants.Char_0060_Expression,
                PreboxedConstants.Char_0061_Expression,
                PreboxedConstants.Char_0062_Expression,
                PreboxedConstants.Char_0063_Expression,
                PreboxedConstants.Char_0064_Expression,
                PreboxedConstants.Char_0065_Expression,
                PreboxedConstants.Char_0066_Expression,
                PreboxedConstants.Char_0067_Expression,
                PreboxedConstants.Char_0068_Expression,
                PreboxedConstants.Char_0069_Expression,
                PreboxedConstants.Char_006A_Expression,
                PreboxedConstants.Char_006B_Expression,
                PreboxedConstants.Char_006C_Expression,
                PreboxedConstants.Char_006D_Expression,
                PreboxedConstants.Char_006E_Expression,
                PreboxedConstants.Char_006F_Expression,
                PreboxedConstants.Char_0070_Expression,
                PreboxedConstants.Char_0071_Expression,
                PreboxedConstants.Char_0072_Expression,
                PreboxedConstants.Char_0073_Expression,
                PreboxedConstants.Char_0074_Expression,
                PreboxedConstants.Char_0075_Expression,
                PreboxedConstants.Char_0076_Expression,
                PreboxedConstants.Char_0077_Expression,
                PreboxedConstants.Char_0078_Expression,
                PreboxedConstants.Char_0079_Expression,
                PreboxedConstants.Char_007A_Expression,
                PreboxedConstants.Char_007B_Expression,
                PreboxedConstants.Char_007C_Expression,
                PreboxedConstants.Char_007D_Expression,
                PreboxedConstants.Char_007E_Expression,
                PreboxedConstants.Char_007F_Expression,
                PreboxedConstants.Char_0080_Expression,
                PreboxedConstants.Char_0081_Expression,
                PreboxedConstants.Char_0082_Expression,
                PreboxedConstants.Char_0083_Expression,
                PreboxedConstants.Char_0084_Expression,
                PreboxedConstants.Char_0085_Expression,
                PreboxedConstants.Char_0086_Expression,
                PreboxedConstants.Char_0087_Expression,
                PreboxedConstants.Char_0088_Expression,
                PreboxedConstants.Char_0089_Expression,
                PreboxedConstants.Char_008A_Expression,
                PreboxedConstants.Char_008B_Expression,
                PreboxedConstants.Char_008C_Expression,
                PreboxedConstants.Char_008D_Expression,
                PreboxedConstants.Char_008E_Expression,
                PreboxedConstants.Char_008F_Expression,
                PreboxedConstants.Char_0090_Expression,
                PreboxedConstants.Char_0091_Expression,
                PreboxedConstants.Char_0092_Expression,
                PreboxedConstants.Char_0093_Expression,
                PreboxedConstants.Char_0094_Expression,
                PreboxedConstants.Char_0095_Expression,
                PreboxedConstants.Char_0096_Expression,
                PreboxedConstants.Char_0097_Expression,
                PreboxedConstants.Char_0098_Expression,
                PreboxedConstants.Char_0099_Expression,
                PreboxedConstants.Char_009A_Expression,
                PreboxedConstants.Char_009B_Expression,
                PreboxedConstants.Char_009C_Expression,
                PreboxedConstants.Char_009D_Expression,
                PreboxedConstants.Char_009E_Expression,
                PreboxedConstants.Char_009F_Expression,
                PreboxedConstants.Char_00A0_Expression,
                PreboxedConstants.Char_00A1_Expression,
                PreboxedConstants.Char_00A2_Expression,
                PreboxedConstants.Char_00A3_Expression,
                PreboxedConstants.Char_00A4_Expression,
                PreboxedConstants.Char_00A5_Expression,
                PreboxedConstants.Char_00A6_Expression,
                PreboxedConstants.Char_00A7_Expression,
                PreboxedConstants.Char_00A8_Expression,
                PreboxedConstants.Char_00A9_Expression,
                PreboxedConstants.Char_00AA_Expression,
                PreboxedConstants.Char_00AB_Expression,
                PreboxedConstants.Char_00AC_Expression,
                PreboxedConstants.Char_00AD_Expression,
                PreboxedConstants.Char_00AE_Expression,
                PreboxedConstants.Char_00AF_Expression,
                PreboxedConstants.Char_00B0_Expression,
                PreboxedConstants.Char_00B1_Expression,
                PreboxedConstants.Char_00B2_Expression,
                PreboxedConstants.Char_00B3_Expression,
                PreboxedConstants.Char_00B4_Expression,
                PreboxedConstants.Char_00B5_Expression,
                PreboxedConstants.Char_00B6_Expression,
                PreboxedConstants.Char_00B7_Expression,
                PreboxedConstants.Char_00B8_Expression,
                PreboxedConstants.Char_00B9_Expression,
                PreboxedConstants.Char_00BA_Expression,
                PreboxedConstants.Char_00BB_Expression,
                PreboxedConstants.Char_00BC_Expression,
                PreboxedConstants.Char_00BD_Expression,
                PreboxedConstants.Char_00BE_Expression,
                PreboxedConstants.Char_00BF_Expression,
                PreboxedConstants.Char_00C0_Expression,
                PreboxedConstants.Char_00C1_Expression,
                PreboxedConstants.Char_00C2_Expression,
                PreboxedConstants.Char_00C3_Expression,
                PreboxedConstants.Char_00C4_Expression,
                PreboxedConstants.Char_00C5_Expression,
                PreboxedConstants.Char_00C6_Expression,
                PreboxedConstants.Char_00C7_Expression,
                PreboxedConstants.Char_00C8_Expression,
                PreboxedConstants.Char_00C9_Expression,
                PreboxedConstants.Char_00CA_Expression,
                PreboxedConstants.Char_00CB_Expression,
                PreboxedConstants.Char_00CC_Expression,
                PreboxedConstants.Char_00CD_Expression,
                PreboxedConstants.Char_00CE_Expression,
                PreboxedConstants.Char_00CF_Expression,
                PreboxedConstants.Char_00D0_Expression,
                PreboxedConstants.Char_00D1_Expression,
                PreboxedConstants.Char_00D2_Expression,
                PreboxedConstants.Char_00D3_Expression,
                PreboxedConstants.Char_00D4_Expression,
                PreboxedConstants.Char_00D5_Expression,
                PreboxedConstants.Char_00D6_Expression,
                PreboxedConstants.Char_00D7_Expression,
                PreboxedConstants.Char_00D8_Expression,
                PreboxedConstants.Char_00D9_Expression,
                PreboxedConstants.Char_00DA_Expression,
                PreboxedConstants.Char_00DB_Expression,
                PreboxedConstants.Char_00DC_Expression,
                PreboxedConstants.Char_00DD_Expression,
                PreboxedConstants.Char_00DE_Expression,
                PreboxedConstants.Char_00DF_Expression,
                PreboxedConstants.Char_00E0_Expression,
                PreboxedConstants.Char_00E1_Expression,
                PreboxedConstants.Char_00E2_Expression,
                PreboxedConstants.Char_00E3_Expression,
                PreboxedConstants.Char_00E4_Expression,
                PreboxedConstants.Char_00E5_Expression,
                PreboxedConstants.Char_00E6_Expression,
                PreboxedConstants.Char_00E7_Expression,
                PreboxedConstants.Char_00E8_Expression,
                PreboxedConstants.Char_00E9_Expression,
                PreboxedConstants.Char_00EA_Expression,
                PreboxedConstants.Char_00EB_Expression,
                PreboxedConstants.Char_00EC_Expression,
                PreboxedConstants.Char_00ED_Expression,
                PreboxedConstants.Char_00EE_Expression,
                PreboxedConstants.Char_00EF_Expression,
                PreboxedConstants.Char_00F0_Expression,
                PreboxedConstants.Char_00F1_Expression,
                PreboxedConstants.Char_00F2_Expression,
                PreboxedConstants.Char_00F3_Expression,
                PreboxedConstants.Char_00F4_Expression,
                PreboxedConstants.Char_00F5_Expression,
                PreboxedConstants.Char_00F6_Expression,
                PreboxedConstants.Char_00F7_Expression,
                PreboxedConstants.Char_00F8_Expression,
                PreboxedConstants.Char_00F9_Expression,
                PreboxedConstants.Char_00FA_Expression,
                PreboxedConstants.Char_00FB_Expression,
                PreboxedConstants.Char_00FC_Expression,
                PreboxedConstants.Char_00FD_Expression,
                PreboxedConstants.Char_00FE_Expression,
                PreboxedConstants.Char_00FF_Expression       
            };
        }

        /// <summary>
        /// A singleton boxed char 0 (0x0000).
        /// </summary>
        public static readonly object Char_0000 = '\x0000';

        /// <summary>
        /// Expression that returns the singleton boxed char 0 (0x0000).
        /// </summary>
        public static readonly Expression Char_0000_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0000");

        /// <summary>
        /// A singleton boxed char 1 (0x0001).
        /// </summary>
        public static readonly object Char_0001 = '\x0001';

        /// <summary>
        /// Expression that returns the singleton boxed char 1 (0x0001).
        /// </summary>
        public static readonly Expression Char_0001_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0001");

        /// <summary>
        /// A singleton boxed char 2 (0x0002).
        /// </summary>
        public static readonly object Char_0002 = '\x0002';

        /// <summary>
        /// Expression that returns the singleton boxed char 2 (0x0002).
        /// </summary>
        public static readonly Expression Char_0002_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0002");

        /// <summary>
        /// A singleton boxed char 3 (0x0003).
        /// </summary>
        public static readonly object Char_0003 = '\x0003';

        /// <summary>
        /// Expression that returns the singleton boxed char 3 (0x0003).
        /// </summary>
        public static readonly Expression Char_0003_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0003");

        /// <summary>
        /// A singleton boxed char 4 (0x0004).
        /// </summary>
        public static readonly object Char_0004 = '\x0004';

        /// <summary>
        /// Expression that returns the singleton boxed char 4 (0x0004).
        /// </summary>
        public static readonly Expression Char_0004_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0004");

        /// <summary>
        /// A singleton boxed char 5 (0x0005).
        /// </summary>
        public static readonly object Char_0005 = '\x0005';

        /// <summary>
        /// Expression that returns the singleton boxed char 5 (0x0005).
        /// </summary>
        public static readonly Expression Char_0005_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0005");

        /// <summary>
        /// A singleton boxed char 6 (0x0006).
        /// </summary>
        public static readonly object Char_0006 = '\x0006';

        /// <summary>
        /// Expression that returns the singleton boxed char 6 (0x0006).
        /// </summary>
        public static readonly Expression Char_0006_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0006");

        /// <summary>
        /// A singleton boxed char 7 (0x0007).
        /// </summary>
        public static readonly object Char_0007 = '\x0007';

        /// <summary>
        /// Expression that returns the singleton boxed char 7 (0x0007).
        /// </summary>
        public static readonly Expression Char_0007_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0007");

        /// <summary>
        /// A singleton boxed char 8 (0x0008).
        /// </summary>
        public static readonly object Char_0008 = '\x0008';

        /// <summary>
        /// Expression that returns the singleton boxed char 8 (0x0008).
        /// </summary>
        public static readonly Expression Char_0008_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0008");

        /// <summary>
        /// A singleton boxed char 9 (0x0009).
        /// </summary>
        public static readonly object Char_0009 = '\x0009';

        /// <summary>
        /// Expression that returns the singleton boxed char 9 (0x0009).
        /// </summary>
        public static readonly Expression Char_0009_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0009");

        /// <summary>
        /// A singleton boxed char 10 (0x000A).
        /// </summary>
        public static readonly object Char_000A = '\x000A';

        /// <summary>
        /// Expression that returns the singleton boxed char 10 (0x000A).
        /// </summary>
        public static readonly Expression Char_000A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000A");

        /// <summary>
        /// A singleton boxed char 11 (0x000B).
        /// </summary>
        public static readonly object Char_000B = '\x000B';

        /// <summary>
        /// Expression that returns the singleton boxed char 11 (0x000B).
        /// </summary>
        public static readonly Expression Char_000B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000B");

        /// <summary>
        /// A singleton boxed char 12 (0x000C).
        /// </summary>
        public static readonly object Char_000C = '\x000C';

        /// <summary>
        /// Expression that returns the singleton boxed char 12 (0x000C).
        /// </summary>
        public static readonly Expression Char_000C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000C");

        /// <summary>
        /// A singleton boxed char 13 (0x000D).
        /// </summary>
        public static readonly object Char_000D = '\x000D';

        /// <summary>
        /// Expression that returns the singleton boxed char 13 (0x000D).
        /// </summary>
        public static readonly Expression Char_000D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000D");

        /// <summary>
        /// A singleton boxed char 14 (0x000E).
        /// </summary>
        public static readonly object Char_000E = '\x000E';

        /// <summary>
        /// Expression that returns the singleton boxed char 14 (0x000E).
        /// </summary>
        public static readonly Expression Char_000E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000E");

        /// <summary>
        /// A singleton boxed char 15 (0x000F).
        /// </summary>
        public static readonly object Char_000F = '\x000F';

        /// <summary>
        /// Expression that returns the singleton boxed char 15 (0x000F).
        /// </summary>
        public static readonly Expression Char_000F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_000F");

        /// <summary>
        /// A singleton boxed char 16 (0x0010).
        /// </summary>
        public static readonly object Char_0010 = '\x0010';

        /// <summary>
        /// Expression that returns the singleton boxed char 16 (0x0010).
        /// </summary>
        public static readonly Expression Char_0010_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0010");

        /// <summary>
        /// A singleton boxed char 17 (0x0011).
        /// </summary>
        public static readonly object Char_0011 = '\x0011';

        /// <summary>
        /// Expression that returns the singleton boxed char 17 (0x0011).
        /// </summary>
        public static readonly Expression Char_0011_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0011");

        /// <summary>
        /// A singleton boxed char 18 (0x0012).
        /// </summary>
        public static readonly object Char_0012 = '\x0012';

        /// <summary>
        /// Expression that returns the singleton boxed char 18 (0x0012).
        /// </summary>
        public static readonly Expression Char_0012_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0012");

        /// <summary>
        /// A singleton boxed char 19 (0x0013).
        /// </summary>
        public static readonly object Char_0013 = '\x0013';

        /// <summary>
        /// Expression that returns the singleton boxed char 19 (0x0013).
        /// </summary>
        public static readonly Expression Char_0013_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0013");

        /// <summary>
        /// A singleton boxed char 20 (0x0014).
        /// </summary>
        public static readonly object Char_0014 = '\x0014';

        /// <summary>
        /// Expression that returns the singleton boxed char 20 (0x0014).
        /// </summary>
        public static readonly Expression Char_0014_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0014");

        /// <summary>
        /// A singleton boxed char 21 (0x0015).
        /// </summary>
        public static readonly object Char_0015 = '\x0015';

        /// <summary>
        /// Expression that returns the singleton boxed char 21 (0x0015).
        /// </summary>
        public static readonly Expression Char_0015_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0015");

        /// <summary>
        /// A singleton boxed char 22 (0x0016).
        /// </summary>
        public static readonly object Char_0016 = '\x0016';

        /// <summary>
        /// Expression that returns the singleton boxed char 22 (0x0016).
        /// </summary>
        public static readonly Expression Char_0016_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0016");

        /// <summary>
        /// A singleton boxed char 23 (0x0017).
        /// </summary>
        public static readonly object Char_0017 = '\x0017';

        /// <summary>
        /// Expression that returns the singleton boxed char 23 (0x0017).
        /// </summary>
        public static readonly Expression Char_0017_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0017");

        /// <summary>
        /// A singleton boxed char 24 (0x0018).
        /// </summary>
        public static readonly object Char_0018 = '\x0018';

        /// <summary>
        /// Expression that returns the singleton boxed char 24 (0x0018).
        /// </summary>
        public static readonly Expression Char_0018_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0018");

        /// <summary>
        /// A singleton boxed char 25 (0x0019).
        /// </summary>
        public static readonly object Char_0019 = '\x0019';

        /// <summary>
        /// Expression that returns the singleton boxed char 25 (0x0019).
        /// </summary>
        public static readonly Expression Char_0019_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0019");

        /// <summary>
        /// A singleton boxed char 26 (0x001A).
        /// </summary>
        public static readonly object Char_001A = '\x001A';

        /// <summary>
        /// Expression that returns the singleton boxed char 26 (0x001A).
        /// </summary>
        public static readonly Expression Char_001A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001A");

        /// <summary>
        /// A singleton boxed char 27 (0x001B).
        /// </summary>
        public static readonly object Char_001B = '\x001B';

        /// <summary>
        /// Expression that returns the singleton boxed char 27 (0x001B).
        /// </summary>
        public static readonly Expression Char_001B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001B");

        /// <summary>
        /// A singleton boxed char 28 (0x001C).
        /// </summary>
        public static readonly object Char_001C = '\x001C';

        /// <summary>
        /// Expression that returns the singleton boxed char 28 (0x001C).
        /// </summary>
        public static readonly Expression Char_001C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001C");

        /// <summary>
        /// A singleton boxed char 29 (0x001D).
        /// </summary>
        public static readonly object Char_001D = '\x001D';

        /// <summary>
        /// Expression that returns the singleton boxed char 29 (0x001D).
        /// </summary>
        public static readonly Expression Char_001D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001D");

        /// <summary>
        /// A singleton boxed char 30 (0x001E).
        /// </summary>
        public static readonly object Char_001E = '\x001E';

        /// <summary>
        /// Expression that returns the singleton boxed char 30 (0x001E).
        /// </summary>
        public static readonly Expression Char_001E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001E");

        /// <summary>
        /// A singleton boxed char 31 (0x001F).
        /// </summary>
        public static readonly object Char_001F = '\x001F';

        /// <summary>
        /// Expression that returns the singleton boxed char 31 (0x001F).
        /// </summary>
        public static readonly Expression Char_001F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_001F");

        /// <summary>
        /// A singleton boxed char 32 (0x0020).
        /// </summary>
        public static readonly object Char_0020 = '\x0020';

        /// <summary>
        /// Expression that returns the singleton boxed char 32 (0x0020).
        /// </summary>
        public static readonly Expression Char_0020_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0020");

        /// <summary>
        /// A singleton boxed char 33 (0x0021).
        /// </summary>
        public static readonly object Char_0021 = '\x0021';

        /// <summary>
        /// Expression that returns the singleton boxed char 33 (0x0021).
        /// </summary>
        public static readonly Expression Char_0021_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0021");

        /// <summary>
        /// A singleton boxed char 34 (0x0022).
        /// </summary>
        public static readonly object Char_0022 = '\x0022';

        /// <summary>
        /// Expression that returns the singleton boxed char 34 (0x0022).
        /// </summary>
        public static readonly Expression Char_0022_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0022");

        /// <summary>
        /// A singleton boxed char 35 (0x0023).
        /// </summary>
        public static readonly object Char_0023 = '\x0023';

        /// <summary>
        /// Expression that returns the singleton boxed char 35 (0x0023).
        /// </summary>
        public static readonly Expression Char_0023_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0023");

        /// <summary>
        /// A singleton boxed char 36 (0x0024).
        /// </summary>
        public static readonly object Char_0024 = '\x0024';

        /// <summary>
        /// Expression that returns the singleton boxed char 36 (0x0024).
        /// </summary>
        public static readonly Expression Char_0024_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0024");

        /// <summary>
        /// A singleton boxed char 37 (0x0025).
        /// </summary>
        public static readonly object Char_0025 = '\x0025';

        /// <summary>
        /// Expression that returns the singleton boxed char 37 (0x0025).
        /// </summary>
        public static readonly Expression Char_0025_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0025");

        /// <summary>
        /// A singleton boxed char 38 (0x0026).
        /// </summary>
        public static readonly object Char_0026 = '\x0026';

        /// <summary>
        /// Expression that returns the singleton boxed char 38 (0x0026).
        /// </summary>
        public static readonly Expression Char_0026_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0026");

        /// <summary>
        /// A singleton boxed char 39 (0x0027).
        /// </summary>
        public static readonly object Char_0027 = '\x0027';

        /// <summary>
        /// Expression that returns the singleton boxed char 39 (0x0027).
        /// </summary>
        public static readonly Expression Char_0027_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0027");

        /// <summary>
        /// A singleton boxed char 40 (0x0028).
        /// </summary>
        public static readonly object Char_0028 = '\x0028';

        /// <summary>
        /// Expression that returns the singleton boxed char 40 (0x0028).
        /// </summary>
        public static readonly Expression Char_0028_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0028");

        /// <summary>
        /// A singleton boxed char 41 (0x0029).
        /// </summary>
        public static readonly object Char_0029 = '\x0029';

        /// <summary>
        /// Expression that returns the singleton boxed char 41 (0x0029).
        /// </summary>
        public static readonly Expression Char_0029_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0029");

        /// <summary>
        /// A singleton boxed char 42 (0x002A).
        /// </summary>
        public static readonly object Char_002A = '\x002A';

        /// <summary>
        /// Expression that returns the singleton boxed char 42 (0x002A).
        /// </summary>
        public static readonly Expression Char_002A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002A");

        /// <summary>
        /// A singleton boxed char 43 (0x002B).
        /// </summary>
        public static readonly object Char_002B = '\x002B';

        /// <summary>
        /// Expression that returns the singleton boxed char 43 (0x002B).
        /// </summary>
        public static readonly Expression Char_002B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002B");

        /// <summary>
        /// A singleton boxed char 44 (0x002C).
        /// </summary>
        public static readonly object Char_002C = '\x002C';

        /// <summary>
        /// Expression that returns the singleton boxed char 44 (0x002C).
        /// </summary>
        public static readonly Expression Char_002C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002C");

        /// <summary>
        /// A singleton boxed char 45 (0x002D).
        /// </summary>
        public static readonly object Char_002D = '\x002D';

        /// <summary>
        /// Expression that returns the singleton boxed char 45 (0x002D).
        /// </summary>
        public static readonly Expression Char_002D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002D");

        /// <summary>
        /// A singleton boxed char 46 (0x002E).
        /// </summary>
        public static readonly object Char_002E = '\x002E';

        /// <summary>
        /// Expression that returns the singleton boxed char 46 (0x002E).
        /// </summary>
        public static readonly Expression Char_002E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002E");

        /// <summary>
        /// A singleton boxed char 47 (0x002F).
        /// </summary>
        public static readonly object Char_002F = '\x002F';

        /// <summary>
        /// Expression that returns the singleton boxed char 47 (0x002F).
        /// </summary>
        public static readonly Expression Char_002F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_002F");

        /// <summary>
        /// A singleton boxed char 48 (0x0030).
        /// </summary>
        public static readonly object Char_0030 = '\x0030';

        /// <summary>
        /// Expression that returns the singleton boxed char 48 (0x0030).
        /// </summary>
        public static readonly Expression Char_0030_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0030");

        /// <summary>
        /// A singleton boxed char 49 (0x0031).
        /// </summary>
        public static readonly object Char_0031 = '\x0031';

        /// <summary>
        /// Expression that returns the singleton boxed char 49 (0x0031).
        /// </summary>
        public static readonly Expression Char_0031_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0031");

        /// <summary>
        /// A singleton boxed char 50 (0x0032).
        /// </summary>
        public static readonly object Char_0032 = '\x0032';

        /// <summary>
        /// Expression that returns the singleton boxed char 50 (0x0032).
        /// </summary>
        public static readonly Expression Char_0032_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0032");

        /// <summary>
        /// A singleton boxed char 51 (0x0033).
        /// </summary>
        public static readonly object Char_0033 = '\x0033';

        /// <summary>
        /// Expression that returns the singleton boxed char 51 (0x0033).
        /// </summary>
        public static readonly Expression Char_0033_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0033");

        /// <summary>
        /// A singleton boxed char 52 (0x0034).
        /// </summary>
        public static readonly object Char_0034 = '\x0034';

        /// <summary>
        /// Expression that returns the singleton boxed char 52 (0x0034).
        /// </summary>
        public static readonly Expression Char_0034_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0034");

        /// <summary>
        /// A singleton boxed char 53 (0x0035).
        /// </summary>
        public static readonly object Char_0035 = '\x0035';

        /// <summary>
        /// Expression that returns the singleton boxed char 53 (0x0035).
        /// </summary>
        public static readonly Expression Char_0035_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0035");

        /// <summary>
        /// A singleton boxed char 54 (0x0036).
        /// </summary>
        public static readonly object Char_0036 = '\x0036';

        /// <summary>
        /// Expression that returns the singleton boxed char 54 (0x0036).
        /// </summary>
        public static readonly Expression Char_0036_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0036");

        /// <summary>
        /// A singleton boxed char 55 (0x0037).
        /// </summary>
        public static readonly object Char_0037 = '\x0037';

        /// <summary>
        /// Expression that returns the singleton boxed char 55 (0x0037).
        /// </summary>
        public static readonly Expression Char_0037_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0037");

        /// <summary>
        /// A singleton boxed char 56 (0x0038).
        /// </summary>
        public static readonly object Char_0038 = '\x0038';

        /// <summary>
        /// Expression that returns the singleton boxed char 56 (0x0038).
        /// </summary>
        public static readonly Expression Char_0038_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0038");

        /// <summary>
        /// A singleton boxed char 57 (0x0039).
        /// </summary>
        public static readonly object Char_0039 = '\x0039';

        /// <summary>
        /// Expression that returns the singleton boxed char 57 (0x0039).
        /// </summary>
        public static readonly Expression Char_0039_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0039");

        /// <summary>
        /// A singleton boxed char 58 (0x003A).
        /// </summary>
        public static readonly object Char_003A = '\x003A';

        /// <summary>
        /// Expression that returns the singleton boxed char 58 (0x003A).
        /// </summary>
        public static readonly Expression Char_003A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003A");

        /// <summary>
        /// A singleton boxed char 59 (0x003B).
        /// </summary>
        public static readonly object Char_003B = '\x003B';

        /// <summary>
        /// Expression that returns the singleton boxed char 59 (0x003B).
        /// </summary>
        public static readonly Expression Char_003B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003B");

        /// <summary>
        /// A singleton boxed char 60 (0x003C).
        /// </summary>
        public static readonly object Char_003C = '\x003C';

        /// <summary>
        /// Expression that returns the singleton boxed char 60 (0x003C).
        /// </summary>
        public static readonly Expression Char_003C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003C");

        /// <summary>
        /// A singleton boxed char 61 (0x003D).
        /// </summary>
        public static readonly object Char_003D = '\x003D';

        /// <summary>
        /// Expression that returns the singleton boxed char 61 (0x003D).
        /// </summary>
        public static readonly Expression Char_003D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003D");

        /// <summary>
        /// A singleton boxed char 62 (0x003E).
        /// </summary>
        public static readonly object Char_003E = '\x003E';

        /// <summary>
        /// Expression that returns the singleton boxed char 62 (0x003E).
        /// </summary>
        public static readonly Expression Char_003E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003E");

        /// <summary>
        /// A singleton boxed char 63 (0x003F).
        /// </summary>
        public static readonly object Char_003F = '\x003F';

        /// <summary>
        /// Expression that returns the singleton boxed char 63 (0x003F).
        /// </summary>
        public static readonly Expression Char_003F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_003F");

        /// <summary>
        /// A singleton boxed char 64 (0x0040).
        /// </summary>
        public static readonly object Char_0040 = '\x0040';

        /// <summary>
        /// Expression that returns the singleton boxed char 64 (0x0040).
        /// </summary>
        public static readonly Expression Char_0040_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0040");

        /// <summary>
        /// A singleton boxed char 65 (0x0041).
        /// </summary>
        public static readonly object Char_0041 = '\x0041';

        /// <summary>
        /// Expression that returns the singleton boxed char 65 (0x0041).
        /// </summary>
        public static readonly Expression Char_0041_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0041");

        /// <summary>
        /// A singleton boxed char 66 (0x0042).
        /// </summary>
        public static readonly object Char_0042 = '\x0042';

        /// <summary>
        /// Expression that returns the singleton boxed char 66 (0x0042).
        /// </summary>
        public static readonly Expression Char_0042_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0042");

        /// <summary>
        /// A singleton boxed char 67 (0x0043).
        /// </summary>
        public static readonly object Char_0043 = '\x0043';

        /// <summary>
        /// Expression that returns the singleton boxed char 67 (0x0043).
        /// </summary>
        public static readonly Expression Char_0043_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0043");

        /// <summary>
        /// A singleton boxed char 68 (0x0044).
        /// </summary>
        public static readonly object Char_0044 = '\x0044';

        /// <summary>
        /// Expression that returns the singleton boxed char 68 (0x0044).
        /// </summary>
        public static readonly Expression Char_0044_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0044");

        /// <summary>
        /// A singleton boxed char 69 (0x0045).
        /// </summary>
        public static readonly object Char_0045 = '\x0045';

        /// <summary>
        /// Expression that returns the singleton boxed char 69 (0x0045).
        /// </summary>
        public static readonly Expression Char_0045_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0045");

        /// <summary>
        /// A singleton boxed char 70 (0x0046).
        /// </summary>
        public static readonly object Char_0046 = '\x0046';

        /// <summary>
        /// Expression that returns the singleton boxed char 70 (0x0046).
        /// </summary>
        public static readonly Expression Char_0046_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0046");

        /// <summary>
        /// A singleton boxed char 71 (0x0047).
        /// </summary>
        public static readonly object Char_0047 = '\x0047';

        /// <summary>
        /// Expression that returns the singleton boxed char 71 (0x0047).
        /// </summary>
        public static readonly Expression Char_0047_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0047");

        /// <summary>
        /// A singleton boxed char 72 (0x0048).
        /// </summary>
        public static readonly object Char_0048 = '\x0048';

        /// <summary>
        /// Expression that returns the singleton boxed char 72 (0x0048).
        /// </summary>
        public static readonly Expression Char_0048_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0048");

        /// <summary>
        /// A singleton boxed char 73 (0x0049).
        /// </summary>
        public static readonly object Char_0049 = '\x0049';

        /// <summary>
        /// Expression that returns the singleton boxed char 73 (0x0049).
        /// </summary>
        public static readonly Expression Char_0049_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0049");

        /// <summary>
        /// A singleton boxed char 74 (0x004A).
        /// </summary>
        public static readonly object Char_004A = '\x004A';

        /// <summary>
        /// Expression that returns the singleton boxed char 74 (0x004A).
        /// </summary>
        public static readonly Expression Char_004A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004A");

        /// <summary>
        /// A singleton boxed char 75 (0x004B).
        /// </summary>
        public static readonly object Char_004B = '\x004B';

        /// <summary>
        /// Expression that returns the singleton boxed char 75 (0x004B).
        /// </summary>
        public static readonly Expression Char_004B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004B");

        /// <summary>
        /// A singleton boxed char 76 (0x004C).
        /// </summary>
        public static readonly object Char_004C = '\x004C';

        /// <summary>
        /// Expression that returns the singleton boxed char 76 (0x004C).
        /// </summary>
        public static readonly Expression Char_004C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004C");

        /// <summary>
        /// A singleton boxed char 77 (0x004D).
        /// </summary>
        public static readonly object Char_004D = '\x004D';

        /// <summary>
        /// Expression that returns the singleton boxed char 77 (0x004D).
        /// </summary>
        public static readonly Expression Char_004D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004D");

        /// <summary>
        /// A singleton boxed char 78 (0x004E).
        /// </summary>
        public static readonly object Char_004E = '\x004E';

        /// <summary>
        /// Expression that returns the singleton boxed char 78 (0x004E).
        /// </summary>
        public static readonly Expression Char_004E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004E");

        /// <summary>
        /// A singleton boxed char 79 (0x004F).
        /// </summary>
        public static readonly object Char_004F = '\x004F';

        /// <summary>
        /// Expression that returns the singleton boxed char 79 (0x004F).
        /// </summary>
        public static readonly Expression Char_004F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_004F");

        /// <summary>
        /// A singleton boxed char 80 (0x0050).
        /// </summary>
        public static readonly object Char_0050 = '\x0050';

        /// <summary>
        /// Expression that returns the singleton boxed char 80 (0x0050).
        /// </summary>
        public static readonly Expression Char_0050_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0050");

        /// <summary>
        /// A singleton boxed char 81 (0x0051).
        /// </summary>
        public static readonly object Char_0051 = '\x0051';

        /// <summary>
        /// Expression that returns the singleton boxed char 81 (0x0051).
        /// </summary>
        public static readonly Expression Char_0051_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0051");

        /// <summary>
        /// A singleton boxed char 82 (0x0052).
        /// </summary>
        public static readonly object Char_0052 = '\x0052';

        /// <summary>
        /// Expression that returns the singleton boxed char 82 (0x0052).
        /// </summary>
        public static readonly Expression Char_0052_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0052");

        /// <summary>
        /// A singleton boxed char 83 (0x0053).
        /// </summary>
        public static readonly object Char_0053 = '\x0053';

        /// <summary>
        /// Expression that returns the singleton boxed char 83 (0x0053).
        /// </summary>
        public static readonly Expression Char_0053_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0053");

        /// <summary>
        /// A singleton boxed char 84 (0x0054).
        /// </summary>
        public static readonly object Char_0054 = '\x0054';

        /// <summary>
        /// Expression that returns the singleton boxed char 84 (0x0054).
        /// </summary>
        public static readonly Expression Char_0054_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0054");

        /// <summary>
        /// A singleton boxed char 85 (0x0055).
        /// </summary>
        public static readonly object Char_0055 = '\x0055';

        /// <summary>
        /// Expression that returns the singleton boxed char 85 (0x0055).
        /// </summary>
        public static readonly Expression Char_0055_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0055");

        /// <summary>
        /// A singleton boxed char 86 (0x0056).
        /// </summary>
        public static readonly object Char_0056 = '\x0056';

        /// <summary>
        /// Expression that returns the singleton boxed char 86 (0x0056).
        /// </summary>
        public static readonly Expression Char_0056_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0056");

        /// <summary>
        /// A singleton boxed char 87 (0x0057).
        /// </summary>
        public static readonly object Char_0057 = '\x0057';

        /// <summary>
        /// Expression that returns the singleton boxed char 87 (0x0057).
        /// </summary>
        public static readonly Expression Char_0057_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0057");

        /// <summary>
        /// A singleton boxed char 88 (0x0058).
        /// </summary>
        public static readonly object Char_0058 = '\x0058';

        /// <summary>
        /// Expression that returns the singleton boxed char 88 (0x0058).
        /// </summary>
        public static readonly Expression Char_0058_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0058");

        /// <summary>
        /// A singleton boxed char 89 (0x0059).
        /// </summary>
        public static readonly object Char_0059 = '\x0059';

        /// <summary>
        /// Expression that returns the singleton boxed char 89 (0x0059).
        /// </summary>
        public static readonly Expression Char_0059_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0059");

        /// <summary>
        /// A singleton boxed char 90 (0x005A).
        /// </summary>
        public static readonly object Char_005A = '\x005A';

        /// <summary>
        /// Expression that returns the singleton boxed char 90 (0x005A).
        /// </summary>
        public static readonly Expression Char_005A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005A");

        /// <summary>
        /// A singleton boxed char 91 (0x005B).
        /// </summary>
        public static readonly object Char_005B = '\x005B';

        /// <summary>
        /// Expression that returns the singleton boxed char 91 (0x005B).
        /// </summary>
        public static readonly Expression Char_005B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005B");

        /// <summary>
        /// A singleton boxed char 92 (0x005C).
        /// </summary>
        public static readonly object Char_005C = '\x005C';

        /// <summary>
        /// Expression that returns the singleton boxed char 92 (0x005C).
        /// </summary>
        public static readonly Expression Char_005C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005C");

        /// <summary>
        /// A singleton boxed char 93 (0x005D).
        /// </summary>
        public static readonly object Char_005D = '\x005D';

        /// <summary>
        /// Expression that returns the singleton boxed char 93 (0x005D).
        /// </summary>
        public static readonly Expression Char_005D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005D");

        /// <summary>
        /// A singleton boxed char 94 (0x005E).
        /// </summary>
        public static readonly object Char_005E = '\x005E';

        /// <summary>
        /// Expression that returns the singleton boxed char 94 (0x005E).
        /// </summary>
        public static readonly Expression Char_005E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005E");

        /// <summary>
        /// A singleton boxed char 95 (0x005F).
        /// </summary>
        public static readonly object Char_005F = '\x005F';

        /// <summary>
        /// Expression that returns the singleton boxed char 95 (0x005F).
        /// </summary>
        public static readonly Expression Char_005F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_005F");

        /// <summary>
        /// A singleton boxed char 96 (0x0060).
        /// </summary>
        public static readonly object Char_0060 = '\x0060';

        /// <summary>
        /// Expression that returns the singleton boxed char 96 (0x0060).
        /// </summary>
        public static readonly Expression Char_0060_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0060");

        /// <summary>
        /// A singleton boxed char 97 (0x0061).
        /// </summary>
        public static readonly object Char_0061 = '\x0061';

        /// <summary>
        /// Expression that returns the singleton boxed char 97 (0x0061).
        /// </summary>
        public static readonly Expression Char_0061_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0061");

        /// <summary>
        /// A singleton boxed char 98 (0x0062).
        /// </summary>
        public static readonly object Char_0062 = '\x0062';

        /// <summary>
        /// Expression that returns the singleton boxed char 98 (0x0062).
        /// </summary>
        public static readonly Expression Char_0062_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0062");

        /// <summary>
        /// A singleton boxed char 99 (0x0063).
        /// </summary>
        public static readonly object Char_0063 = '\x0063';

        /// <summary>
        /// Expression that returns the singleton boxed char 99 (0x0063).
        /// </summary>
        public static readonly Expression Char_0063_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0063");

        /// <summary>
        /// A singleton boxed char 100 (0x0064).
        /// </summary>
        public static readonly object Char_0064 = '\x0064';

        /// <summary>
        /// Expression that returns the singleton boxed char 100 (0x0064).
        /// </summary>
        public static readonly Expression Char_0064_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0064");

        /// <summary>
        /// A singleton boxed char 101 (0x0065).
        /// </summary>
        public static readonly object Char_0065 = '\x0065';

        /// <summary>
        /// Expression that returns the singleton boxed char 101 (0x0065).
        /// </summary>
        public static readonly Expression Char_0065_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0065");

        /// <summary>
        /// A singleton boxed char 102 (0x0066).
        /// </summary>
        public static readonly object Char_0066 = '\x0066';

        /// <summary>
        /// Expression that returns the singleton boxed char 102 (0x0066).
        /// </summary>
        public static readonly Expression Char_0066_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0066");

        /// <summary>
        /// A singleton boxed char 103 (0x0067).
        /// </summary>
        public static readonly object Char_0067 = '\x0067';

        /// <summary>
        /// Expression that returns the singleton boxed char 103 (0x0067).
        /// </summary>
        public static readonly Expression Char_0067_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0067");

        /// <summary>
        /// A singleton boxed char 104 (0x0068).
        /// </summary>
        public static readonly object Char_0068 = '\x0068';

        /// <summary>
        /// Expression that returns the singleton boxed char 104 (0x0068).
        /// </summary>
        public static readonly Expression Char_0068_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0068");

        /// <summary>
        /// A singleton boxed char 105 (0x0069).
        /// </summary>
        public static readonly object Char_0069 = '\x0069';

        /// <summary>
        /// Expression that returns the singleton boxed char 105 (0x0069).
        /// </summary>
        public static readonly Expression Char_0069_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0069");

        /// <summary>
        /// A singleton boxed char 106 (0x006A).
        /// </summary>
        public static readonly object Char_006A = '\x006A';

        /// <summary>
        /// Expression that returns the singleton boxed char 106 (0x006A).
        /// </summary>
        public static readonly Expression Char_006A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006A");

        /// <summary>
        /// A singleton boxed char 107 (0x006B).
        /// </summary>
        public static readonly object Char_006B = '\x006B';

        /// <summary>
        /// Expression that returns the singleton boxed char 107 (0x006B).
        /// </summary>
        public static readonly Expression Char_006B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006B");

        /// <summary>
        /// A singleton boxed char 108 (0x006C).
        /// </summary>
        public static readonly object Char_006C = '\x006C';

        /// <summary>
        /// Expression that returns the singleton boxed char 108 (0x006C).
        /// </summary>
        public static readonly Expression Char_006C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006C");

        /// <summary>
        /// A singleton boxed char 109 (0x006D).
        /// </summary>
        public static readonly object Char_006D = '\x006D';

        /// <summary>
        /// Expression that returns the singleton boxed char 109 (0x006D).
        /// </summary>
        public static readonly Expression Char_006D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006D");

        /// <summary>
        /// A singleton boxed char 110 (0x006E).
        /// </summary>
        public static readonly object Char_006E = '\x006E';

        /// <summary>
        /// Expression that returns the singleton boxed char 110 (0x006E).
        /// </summary>
        public static readonly Expression Char_006E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006E");

        /// <summary>
        /// A singleton boxed char 111 (0x006F).
        /// </summary>
        public static readonly object Char_006F = '\x006F';

        /// <summary>
        /// Expression that returns the singleton boxed char 111 (0x006F).
        /// </summary>
        public static readonly Expression Char_006F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_006F");

        /// <summary>
        /// A singleton boxed char 112 (0x0070).
        /// </summary>
        public static readonly object Char_0070 = '\x0070';

        /// <summary>
        /// Expression that returns the singleton boxed char 112 (0x0070).
        /// </summary>
        public static readonly Expression Char_0070_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0070");

        /// <summary>
        /// A singleton boxed char 113 (0x0071).
        /// </summary>
        public static readonly object Char_0071 = '\x0071';

        /// <summary>
        /// Expression that returns the singleton boxed char 113 (0x0071).
        /// </summary>
        public static readonly Expression Char_0071_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0071");

        /// <summary>
        /// A singleton boxed char 114 (0x0072).
        /// </summary>
        public static readonly object Char_0072 = '\x0072';

        /// <summary>
        /// Expression that returns the singleton boxed char 114 (0x0072).
        /// </summary>
        public static readonly Expression Char_0072_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0072");

        /// <summary>
        /// A singleton boxed char 115 (0x0073).
        /// </summary>
        public static readonly object Char_0073 = '\x0073';

        /// <summary>
        /// Expression that returns the singleton boxed char 115 (0x0073).
        /// </summary>
        public static readonly Expression Char_0073_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0073");

        /// <summary>
        /// A singleton boxed char 116 (0x0074).
        /// </summary>
        public static readonly object Char_0074 = '\x0074';

        /// <summary>
        /// Expression that returns the singleton boxed char 116 (0x0074).
        /// </summary>
        public static readonly Expression Char_0074_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0074");

        /// <summary>
        /// A singleton boxed char 117 (0x0075).
        /// </summary>
        public static readonly object Char_0075 = '\x0075';

        /// <summary>
        /// Expression that returns the singleton boxed char 117 (0x0075).
        /// </summary>
        public static readonly Expression Char_0075_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0075");

        /// <summary>
        /// A singleton boxed char 118 (0x0076).
        /// </summary>
        public static readonly object Char_0076 = '\x0076';

        /// <summary>
        /// Expression that returns the singleton boxed char 118 (0x0076).
        /// </summary>
        public static readonly Expression Char_0076_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0076");

        /// <summary>
        /// A singleton boxed char 119 (0x0077).
        /// </summary>
        public static readonly object Char_0077 = '\x0077';

        /// <summary>
        /// Expression that returns the singleton boxed char 119 (0x0077).
        /// </summary>
        public static readonly Expression Char_0077_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0077");

        /// <summary>
        /// A singleton boxed char 120 (0x0078).
        /// </summary>
        public static readonly object Char_0078 = '\x0078';

        /// <summary>
        /// Expression that returns the singleton boxed char 120 (0x0078).
        /// </summary>
        public static readonly Expression Char_0078_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0078");

        /// <summary>
        /// A singleton boxed char 121 (0x0079).
        /// </summary>
        public static readonly object Char_0079 = '\x0079';

        /// <summary>
        /// Expression that returns the singleton boxed char 121 (0x0079).
        /// </summary>
        public static readonly Expression Char_0079_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0079");

        /// <summary>
        /// A singleton boxed char 122 (0x007A).
        /// </summary>
        public static readonly object Char_007A = '\x007A';

        /// <summary>
        /// Expression that returns the singleton boxed char 122 (0x007A).
        /// </summary>
        public static readonly Expression Char_007A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007A");

        /// <summary>
        /// A singleton boxed char 123 (0x007B).
        /// </summary>
        public static readonly object Char_007B = '\x007B';

        /// <summary>
        /// Expression that returns the singleton boxed char 123 (0x007B).
        /// </summary>
        public static readonly Expression Char_007B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007B");

        /// <summary>
        /// A singleton boxed char 124 (0x007C).
        /// </summary>
        public static readonly object Char_007C = '\x007C';

        /// <summary>
        /// Expression that returns the singleton boxed char 124 (0x007C).
        /// </summary>
        public static readonly Expression Char_007C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007C");

        /// <summary>
        /// A singleton boxed char 125 (0x007D).
        /// </summary>
        public static readonly object Char_007D = '\x007D';

        /// <summary>
        /// Expression that returns the singleton boxed char 125 (0x007D).
        /// </summary>
        public static readonly Expression Char_007D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007D");

        /// <summary>
        /// A singleton boxed char 126 (0x007E).
        /// </summary>
        public static readonly object Char_007E = '\x007E';

        /// <summary>
        /// Expression that returns the singleton boxed char 126 (0x007E).
        /// </summary>
        public static readonly Expression Char_007E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007E");

        /// <summary>
        /// A singleton boxed char 127 (0x007F).
        /// </summary>
        public static readonly object Char_007F = '\x007F';

        /// <summary>
        /// Expression that returns the singleton boxed char 127 (0x007F).
        /// </summary>
        public static readonly Expression Char_007F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_007F");

        /// <summary>
        /// A singleton boxed char 128 (0x0080).
        /// </summary>
        public static readonly object Char_0080 = '\x0080';

        /// <summary>
        /// Expression that returns the singleton boxed char 128 (0x0080).
        /// </summary>
        public static readonly Expression Char_0080_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0080");

        /// <summary>
        /// A singleton boxed char 129 (0x0081).
        /// </summary>
        public static readonly object Char_0081 = '\x0081';

        /// <summary>
        /// Expression that returns the singleton boxed char 129 (0x0081).
        /// </summary>
        public static readonly Expression Char_0081_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0081");

        /// <summary>
        /// A singleton boxed char 130 (0x0082).
        /// </summary>
        public static readonly object Char_0082 = '\x0082';

        /// <summary>
        /// Expression that returns the singleton boxed char 130 (0x0082).
        /// </summary>
        public static readonly Expression Char_0082_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0082");

        /// <summary>
        /// A singleton boxed char 131 (0x0083).
        /// </summary>
        public static readonly object Char_0083 = '\x0083';

        /// <summary>
        /// Expression that returns the singleton boxed char 131 (0x0083).
        /// </summary>
        public static readonly Expression Char_0083_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0083");

        /// <summary>
        /// A singleton boxed char 132 (0x0084).
        /// </summary>
        public static readonly object Char_0084 = '\x0084';

        /// <summary>
        /// Expression that returns the singleton boxed char 132 (0x0084).
        /// </summary>
        public static readonly Expression Char_0084_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0084");

        /// <summary>
        /// A singleton boxed char 133 (0x0085).
        /// </summary>
        public static readonly object Char_0085 = '\x0085';

        /// <summary>
        /// Expression that returns the singleton boxed char 133 (0x0085).
        /// </summary>
        public static readonly Expression Char_0085_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0085");

        /// <summary>
        /// A singleton boxed char 134 (0x0086).
        /// </summary>
        public static readonly object Char_0086 = '\x0086';

        /// <summary>
        /// Expression that returns the singleton boxed char 134 (0x0086).
        /// </summary>
        public static readonly Expression Char_0086_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0086");

        /// <summary>
        /// A singleton boxed char 135 (0x0087).
        /// </summary>
        public static readonly object Char_0087 = '\x0087';

        /// <summary>
        /// Expression that returns the singleton boxed char 135 (0x0087).
        /// </summary>
        public static readonly Expression Char_0087_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0087");

        /// <summary>
        /// A singleton boxed char 136 (0x0088).
        /// </summary>
        public static readonly object Char_0088 = '\x0088';

        /// <summary>
        /// Expression that returns the singleton boxed char 136 (0x0088).
        /// </summary>
        public static readonly Expression Char_0088_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0088");

        /// <summary>
        /// A singleton boxed char 137 (0x0089).
        /// </summary>
        public static readonly object Char_0089 = '\x0089';

        /// <summary>
        /// Expression that returns the singleton boxed char 137 (0x0089).
        /// </summary>
        public static readonly Expression Char_0089_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0089");

        /// <summary>
        /// A singleton boxed char 138 (0x008A).
        /// </summary>
        public static readonly object Char_008A = '\x008A';

        /// <summary>
        /// Expression that returns the singleton boxed char 138 (0x008A).
        /// </summary>
        public static readonly Expression Char_008A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008A");

        /// <summary>
        /// A singleton boxed char 139 (0x008B).
        /// </summary>
        public static readonly object Char_008B = '\x008B';

        /// <summary>
        /// Expression that returns the singleton boxed char 139 (0x008B).
        /// </summary>
        public static readonly Expression Char_008B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008B");

        /// <summary>
        /// A singleton boxed char 140 (0x008C).
        /// </summary>
        public static readonly object Char_008C = '\x008C';

        /// <summary>
        /// Expression that returns the singleton boxed char 140 (0x008C).
        /// </summary>
        public static readonly Expression Char_008C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008C");

        /// <summary>
        /// A singleton boxed char 141 (0x008D).
        /// </summary>
        public static readonly object Char_008D = '\x008D';

        /// <summary>
        /// Expression that returns the singleton boxed char 141 (0x008D).
        /// </summary>
        public static readonly Expression Char_008D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008D");

        /// <summary>
        /// A singleton boxed char 142 (0x008E).
        /// </summary>
        public static readonly object Char_008E = '\x008E';

        /// <summary>
        /// Expression that returns the singleton boxed char 142 (0x008E).
        /// </summary>
        public static readonly Expression Char_008E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008E");

        /// <summary>
        /// A singleton boxed char 143 (0x008F).
        /// </summary>
        public static readonly object Char_008F = '\x008F';

        /// <summary>
        /// Expression that returns the singleton boxed char 143 (0x008F).
        /// </summary>
        public static readonly Expression Char_008F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_008F");

        /// <summary>
        /// A singleton boxed char 144 (0x0090).
        /// </summary>
        public static readonly object Char_0090 = '\x0090';

        /// <summary>
        /// Expression that returns the singleton boxed char 144 (0x0090).
        /// </summary>
        public static readonly Expression Char_0090_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0090");

        /// <summary>
        /// A singleton boxed char 145 (0x0091).
        /// </summary>
        public static readonly object Char_0091 = '\x0091';

        /// <summary>
        /// Expression that returns the singleton boxed char 145 (0x0091).
        /// </summary>
        public static readonly Expression Char_0091_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0091");

        /// <summary>
        /// A singleton boxed char 146 (0x0092).
        /// </summary>
        public static readonly object Char_0092 = '\x0092';

        /// <summary>
        /// Expression that returns the singleton boxed char 146 (0x0092).
        /// </summary>
        public static readonly Expression Char_0092_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0092");

        /// <summary>
        /// A singleton boxed char 147 (0x0093).
        /// </summary>
        public static readonly object Char_0093 = '\x0093';

        /// <summary>
        /// Expression that returns the singleton boxed char 147 (0x0093).
        /// </summary>
        public static readonly Expression Char_0093_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0093");

        /// <summary>
        /// A singleton boxed char 148 (0x0094).
        /// </summary>
        public static readonly object Char_0094 = '\x0094';

        /// <summary>
        /// Expression that returns the singleton boxed char 148 (0x0094).
        /// </summary>
        public static readonly Expression Char_0094_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0094");

        /// <summary>
        /// A singleton boxed char 149 (0x0095).
        /// </summary>
        public static readonly object Char_0095 = '\x0095';

        /// <summary>
        /// Expression that returns the singleton boxed char 149 (0x0095).
        /// </summary>
        public static readonly Expression Char_0095_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0095");

        /// <summary>
        /// A singleton boxed char 150 (0x0096).
        /// </summary>
        public static readonly object Char_0096 = '\x0096';

        /// <summary>
        /// Expression that returns the singleton boxed char 150 (0x0096).
        /// </summary>
        public static readonly Expression Char_0096_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0096");

        /// <summary>
        /// A singleton boxed char 151 (0x0097).
        /// </summary>
        public static readonly object Char_0097 = '\x0097';

        /// <summary>
        /// Expression that returns the singleton boxed char 151 (0x0097).
        /// </summary>
        public static readonly Expression Char_0097_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0097");

        /// <summary>
        /// A singleton boxed char 152 (0x0098).
        /// </summary>
        public static readonly object Char_0098 = '\x0098';

        /// <summary>
        /// Expression that returns the singleton boxed char 152 (0x0098).
        /// </summary>
        public static readonly Expression Char_0098_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0098");

        /// <summary>
        /// A singleton boxed char 153 (0x0099).
        /// </summary>
        public static readonly object Char_0099 = '\x0099';

        /// <summary>
        /// Expression that returns the singleton boxed char 153 (0x0099).
        /// </summary>
        public static readonly Expression Char_0099_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_0099");

        /// <summary>
        /// A singleton boxed char 154 (0x009A).
        /// </summary>
        public static readonly object Char_009A = '\x009A';

        /// <summary>
        /// Expression that returns the singleton boxed char 154 (0x009A).
        /// </summary>
        public static readonly Expression Char_009A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009A");

        /// <summary>
        /// A singleton boxed char 155 (0x009B).
        /// </summary>
        public static readonly object Char_009B = '\x009B';

        /// <summary>
        /// Expression that returns the singleton boxed char 155 (0x009B).
        /// </summary>
        public static readonly Expression Char_009B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009B");

        /// <summary>
        /// A singleton boxed char 156 (0x009C).
        /// </summary>
        public static readonly object Char_009C = '\x009C';

        /// <summary>
        /// Expression that returns the singleton boxed char 156 (0x009C).
        /// </summary>
        public static readonly Expression Char_009C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009C");

        /// <summary>
        /// A singleton boxed char 157 (0x009D).
        /// </summary>
        public static readonly object Char_009D = '\x009D';

        /// <summary>
        /// Expression that returns the singleton boxed char 157 (0x009D).
        /// </summary>
        public static readonly Expression Char_009D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009D");

        /// <summary>
        /// A singleton boxed char 158 (0x009E).
        /// </summary>
        public static readonly object Char_009E = '\x009E';

        /// <summary>
        /// Expression that returns the singleton boxed char 158 (0x009E).
        /// </summary>
        public static readonly Expression Char_009E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009E");

        /// <summary>
        /// A singleton boxed char 159 (0x009F).
        /// </summary>
        public static readonly object Char_009F = '\x009F';

        /// <summary>
        /// Expression that returns the singleton boxed char 159 (0x009F).
        /// </summary>
        public static readonly Expression Char_009F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_009F");

        /// <summary>
        /// A singleton boxed char 160 (0x00A0).
        /// </summary>
        public static readonly object Char_00A0 = '\x00A0';

        /// <summary>
        /// Expression that returns the singleton boxed char 160 (0x00A0).
        /// </summary>
        public static readonly Expression Char_00A0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A0");

        /// <summary>
        /// A singleton boxed char 161 (0x00A1).
        /// </summary>
        public static readonly object Char_00A1 = '\x00A1';

        /// <summary>
        /// Expression that returns the singleton boxed char 161 (0x00A1).
        /// </summary>
        public static readonly Expression Char_00A1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A1");

        /// <summary>
        /// A singleton boxed char 162 (0x00A2).
        /// </summary>
        public static readonly object Char_00A2 = '\x00A2';

        /// <summary>
        /// Expression that returns the singleton boxed char 162 (0x00A2).
        /// </summary>
        public static readonly Expression Char_00A2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A2");

        /// <summary>
        /// A singleton boxed char 163 (0x00A3).
        /// </summary>
        public static readonly object Char_00A3 = '\x00A3';

        /// <summary>
        /// Expression that returns the singleton boxed char 163 (0x00A3).
        /// </summary>
        public static readonly Expression Char_00A3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A3");

        /// <summary>
        /// A singleton boxed char 164 (0x00A4).
        /// </summary>
        public static readonly object Char_00A4 = '\x00A4';

        /// <summary>
        /// Expression that returns the singleton boxed char 164 (0x00A4).
        /// </summary>
        public static readonly Expression Char_00A4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A4");

        /// <summary>
        /// A singleton boxed char 165 (0x00A5).
        /// </summary>
        public static readonly object Char_00A5 = '\x00A5';

        /// <summary>
        /// Expression that returns the singleton boxed char 165 (0x00A5).
        /// </summary>
        public static readonly Expression Char_00A5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A5");

        /// <summary>
        /// A singleton boxed char 166 (0x00A6).
        /// </summary>
        public static readonly object Char_00A6 = '\x00A6';

        /// <summary>
        /// Expression that returns the singleton boxed char 166 (0x00A6).
        /// </summary>
        public static readonly Expression Char_00A6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A6");

        /// <summary>
        /// A singleton boxed char 167 (0x00A7).
        /// </summary>
        public static readonly object Char_00A7 = '\x00A7';

        /// <summary>
        /// Expression that returns the singleton boxed char 167 (0x00A7).
        /// </summary>
        public static readonly Expression Char_00A7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A7");

        /// <summary>
        /// A singleton boxed char 168 (0x00A8).
        /// </summary>
        public static readonly object Char_00A8 = '\x00A8';

        /// <summary>
        /// Expression that returns the singleton boxed char 168 (0x00A8).
        /// </summary>
        public static readonly Expression Char_00A8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A8");

        /// <summary>
        /// A singleton boxed char 169 (0x00A9).
        /// </summary>
        public static readonly object Char_00A9 = '\x00A9';

        /// <summary>
        /// Expression that returns the singleton boxed char 169 (0x00A9).
        /// </summary>
        public static readonly Expression Char_00A9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00A9");

        /// <summary>
        /// A singleton boxed char 170 (0x00AA).
        /// </summary>
        public static readonly object Char_00AA = '\x00AA';

        /// <summary>
        /// Expression that returns the singleton boxed char 170 (0x00AA).
        /// </summary>
        public static readonly Expression Char_00AA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AA");

        /// <summary>
        /// A singleton boxed char 171 (0x00AB).
        /// </summary>
        public static readonly object Char_00AB = '\x00AB';

        /// <summary>
        /// Expression that returns the singleton boxed char 171 (0x00AB).
        /// </summary>
        public static readonly Expression Char_00AB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AB");

        /// <summary>
        /// A singleton boxed char 172 (0x00AC).
        /// </summary>
        public static readonly object Char_00AC = '\x00AC';

        /// <summary>
        /// Expression that returns the singleton boxed char 172 (0x00AC).
        /// </summary>
        public static readonly Expression Char_00AC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AC");

        /// <summary>
        /// A singleton boxed char 173 (0x00AD).
        /// </summary>
        public static readonly object Char_00AD = '\x00AD';

        /// <summary>
        /// Expression that returns the singleton boxed char 173 (0x00AD).
        /// </summary>
        public static readonly Expression Char_00AD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AD");

        /// <summary>
        /// A singleton boxed char 174 (0x00AE).
        /// </summary>
        public static readonly object Char_00AE = '\x00AE';

        /// <summary>
        /// Expression that returns the singleton boxed char 174 (0x00AE).
        /// </summary>
        public static readonly Expression Char_00AE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AE");

        /// <summary>
        /// A singleton boxed char 175 (0x00AF).
        /// </summary>
        public static readonly object Char_00AF = '\x00AF';

        /// <summary>
        /// Expression that returns the singleton boxed char 175 (0x00AF).
        /// </summary>
        public static readonly Expression Char_00AF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00AF");

        /// <summary>
        /// A singleton boxed char 176 (0x00B0).
        /// </summary>
        public static readonly object Char_00B0 = '\x00B0';

        /// <summary>
        /// Expression that returns the singleton boxed char 176 (0x00B0).
        /// </summary>
        public static readonly Expression Char_00B0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B0");

        /// <summary>
        /// A singleton boxed char 177 (0x00B1).
        /// </summary>
        public static readonly object Char_00B1 = '\x00B1';

        /// <summary>
        /// Expression that returns the singleton boxed char 177 (0x00B1).
        /// </summary>
        public static readonly Expression Char_00B1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B1");

        /// <summary>
        /// A singleton boxed char 178 (0x00B2).
        /// </summary>
        public static readonly object Char_00B2 = '\x00B2';

        /// <summary>
        /// Expression that returns the singleton boxed char 178 (0x00B2).
        /// </summary>
        public static readonly Expression Char_00B2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B2");

        /// <summary>
        /// A singleton boxed char 179 (0x00B3).
        /// </summary>
        public static readonly object Char_00B3 = '\x00B3';

        /// <summary>
        /// Expression that returns the singleton boxed char 179 (0x00B3).
        /// </summary>
        public static readonly Expression Char_00B3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B3");

        /// <summary>
        /// A singleton boxed char 180 (0x00B4).
        /// </summary>
        public static readonly object Char_00B4 = '\x00B4';

        /// <summary>
        /// Expression that returns the singleton boxed char 180 (0x00B4).
        /// </summary>
        public static readonly Expression Char_00B4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B4");

        /// <summary>
        /// A singleton boxed char 181 (0x00B5).
        /// </summary>
        public static readonly object Char_00B5 = '\x00B5';

        /// <summary>
        /// Expression that returns the singleton boxed char 181 (0x00B5).
        /// </summary>
        public static readonly Expression Char_00B5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B5");

        /// <summary>
        /// A singleton boxed char 182 (0x00B6).
        /// </summary>
        public static readonly object Char_00B6 = '\x00B6';

        /// <summary>
        /// Expression that returns the singleton boxed char 182 (0x00B6).
        /// </summary>
        public static readonly Expression Char_00B6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B6");

        /// <summary>
        /// A singleton boxed char 183 (0x00B7).
        /// </summary>
        public static readonly object Char_00B7 = '\x00B7';

        /// <summary>
        /// Expression that returns the singleton boxed char 183 (0x00B7).
        /// </summary>
        public static readonly Expression Char_00B7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B7");

        /// <summary>
        /// A singleton boxed char 184 (0x00B8).
        /// </summary>
        public static readonly object Char_00B8 = '\x00B8';

        /// <summary>
        /// Expression that returns the singleton boxed char 184 (0x00B8).
        /// </summary>
        public static readonly Expression Char_00B8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B8");

        /// <summary>
        /// A singleton boxed char 185 (0x00B9).
        /// </summary>
        public static readonly object Char_00B9 = '\x00B9';

        /// <summary>
        /// Expression that returns the singleton boxed char 185 (0x00B9).
        /// </summary>
        public static readonly Expression Char_00B9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00B9");

        /// <summary>
        /// A singleton boxed char 186 (0x00BA).
        /// </summary>
        public static readonly object Char_00BA = '\x00BA';

        /// <summary>
        /// Expression that returns the singleton boxed char 186 (0x00BA).
        /// </summary>
        public static readonly Expression Char_00BA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BA");

        /// <summary>
        /// A singleton boxed char 187 (0x00BB).
        /// </summary>
        public static readonly object Char_00BB = '\x00BB';

        /// <summary>
        /// Expression that returns the singleton boxed char 187 (0x00BB).
        /// </summary>
        public static readonly Expression Char_00BB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BB");

        /// <summary>
        /// A singleton boxed char 188 (0x00BC).
        /// </summary>
        public static readonly object Char_00BC = '\x00BC';

        /// <summary>
        /// Expression that returns the singleton boxed char 188 (0x00BC).
        /// </summary>
        public static readonly Expression Char_00BC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BC");

        /// <summary>
        /// A singleton boxed char 189 (0x00BD).
        /// </summary>
        public static readonly object Char_00BD = '\x00BD';

        /// <summary>
        /// Expression that returns the singleton boxed char 189 (0x00BD).
        /// </summary>
        public static readonly Expression Char_00BD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BD");

        /// <summary>
        /// A singleton boxed char 190 (0x00BE).
        /// </summary>
        public static readonly object Char_00BE = '\x00BE';

        /// <summary>
        /// Expression that returns the singleton boxed char 190 (0x00BE).
        /// </summary>
        public static readonly Expression Char_00BE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BE");

        /// <summary>
        /// A singleton boxed char 191 (0x00BF).
        /// </summary>
        public static readonly object Char_00BF = '\x00BF';

        /// <summary>
        /// Expression that returns the singleton boxed char 191 (0x00BF).
        /// </summary>
        public static readonly Expression Char_00BF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00BF");

        /// <summary>
        /// A singleton boxed char 192 (0x00C0).
        /// </summary>
        public static readonly object Char_00C0 = '\x00C0';

        /// <summary>
        /// Expression that returns the singleton boxed char 192 (0x00C0).
        /// </summary>
        public static readonly Expression Char_00C0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C0");

        /// <summary>
        /// A singleton boxed char 193 (0x00C1).
        /// </summary>
        public static readonly object Char_00C1 = '\x00C1';

        /// <summary>
        /// Expression that returns the singleton boxed char 193 (0x00C1).
        /// </summary>
        public static readonly Expression Char_00C1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C1");

        /// <summary>
        /// A singleton boxed char 194 (0x00C2).
        /// </summary>
        public static readonly object Char_00C2 = '\x00C2';

        /// <summary>
        /// Expression that returns the singleton boxed char 194 (0x00C2).
        /// </summary>
        public static readonly Expression Char_00C2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C2");

        /// <summary>
        /// A singleton boxed char 195 (0x00C3).
        /// </summary>
        public static readonly object Char_00C3 = '\x00C3';

        /// <summary>
        /// Expression that returns the singleton boxed char 195 (0x00C3).
        /// </summary>
        public static readonly Expression Char_00C3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C3");

        /// <summary>
        /// A singleton boxed char 196 (0x00C4).
        /// </summary>
        public static readonly object Char_00C4 = '\x00C4';

        /// <summary>
        /// Expression that returns the singleton boxed char 196 (0x00C4).
        /// </summary>
        public static readonly Expression Char_00C4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C4");

        /// <summary>
        /// A singleton boxed char 197 (0x00C5).
        /// </summary>
        public static readonly object Char_00C5 = '\x00C5';

        /// <summary>
        /// Expression that returns the singleton boxed char 197 (0x00C5).
        /// </summary>
        public static readonly Expression Char_00C5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C5");

        /// <summary>
        /// A singleton boxed char 198 (0x00C6).
        /// </summary>
        public static readonly object Char_00C6 = '\x00C6';

        /// <summary>
        /// Expression that returns the singleton boxed char 198 (0x00C6).
        /// </summary>
        public static readonly Expression Char_00C6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C6");

        /// <summary>
        /// A singleton boxed char 199 (0x00C7).
        /// </summary>
        public static readonly object Char_00C7 = '\x00C7';

        /// <summary>
        /// Expression that returns the singleton boxed char 199 (0x00C7).
        /// </summary>
        public static readonly Expression Char_00C7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C7");

        /// <summary>
        /// A singleton boxed char 200 (0x00C8).
        /// </summary>
        public static readonly object Char_00C8 = '\x00C8';

        /// <summary>
        /// Expression that returns the singleton boxed char 200 (0x00C8).
        /// </summary>
        public static readonly Expression Char_00C8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C8");

        /// <summary>
        /// A singleton boxed char 201 (0x00C9).
        /// </summary>
        public static readonly object Char_00C9 = '\x00C9';

        /// <summary>
        /// Expression that returns the singleton boxed char 201 (0x00C9).
        /// </summary>
        public static readonly Expression Char_00C9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00C9");

        /// <summary>
        /// A singleton boxed char 202 (0x00CA).
        /// </summary>
        public static readonly object Char_00CA = '\x00CA';

        /// <summary>
        /// Expression that returns the singleton boxed char 202 (0x00CA).
        /// </summary>
        public static readonly Expression Char_00CA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CA");

        /// <summary>
        /// A singleton boxed char 203 (0x00CB).
        /// </summary>
        public static readonly object Char_00CB = '\x00CB';

        /// <summary>
        /// Expression that returns the singleton boxed char 203 (0x00CB).
        /// </summary>
        public static readonly Expression Char_00CB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CB");

        /// <summary>
        /// A singleton boxed char 204 (0x00CC).
        /// </summary>
        public static readonly object Char_00CC = '\x00CC';

        /// <summary>
        /// Expression that returns the singleton boxed char 204 (0x00CC).
        /// </summary>
        public static readonly Expression Char_00CC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CC");

        /// <summary>
        /// A singleton boxed char 205 (0x00CD).
        /// </summary>
        public static readonly object Char_00CD = '\x00CD';

        /// <summary>
        /// Expression that returns the singleton boxed char 205 (0x00CD).
        /// </summary>
        public static readonly Expression Char_00CD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CD");

        /// <summary>
        /// A singleton boxed char 206 (0x00CE).
        /// </summary>
        public static readonly object Char_00CE = '\x00CE';

        /// <summary>
        /// Expression that returns the singleton boxed char 206 (0x00CE).
        /// </summary>
        public static readonly Expression Char_00CE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CE");

        /// <summary>
        /// A singleton boxed char 207 (0x00CF).
        /// </summary>
        public static readonly object Char_00CF = '\x00CF';

        /// <summary>
        /// Expression that returns the singleton boxed char 207 (0x00CF).
        /// </summary>
        public static readonly Expression Char_00CF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00CF");

        /// <summary>
        /// A singleton boxed char 208 (0x00D0).
        /// </summary>
        public static readonly object Char_00D0 = '\x00D0';

        /// <summary>
        /// Expression that returns the singleton boxed char 208 (0x00D0).
        /// </summary>
        public static readonly Expression Char_00D0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D0");

        /// <summary>
        /// A singleton boxed char 209 (0x00D1).
        /// </summary>
        public static readonly object Char_00D1 = '\x00D1';

        /// <summary>
        /// Expression that returns the singleton boxed char 209 (0x00D1).
        /// </summary>
        public static readonly Expression Char_00D1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D1");

        /// <summary>
        /// A singleton boxed char 210 (0x00D2).
        /// </summary>
        public static readonly object Char_00D2 = '\x00D2';

        /// <summary>
        /// Expression that returns the singleton boxed char 210 (0x00D2).
        /// </summary>
        public static readonly Expression Char_00D2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D2");

        /// <summary>
        /// A singleton boxed char 211 (0x00D3).
        /// </summary>
        public static readonly object Char_00D3 = '\x00D3';

        /// <summary>
        /// Expression that returns the singleton boxed char 211 (0x00D3).
        /// </summary>
        public static readonly Expression Char_00D3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D3");

        /// <summary>
        /// A singleton boxed char 212 (0x00D4).
        /// </summary>
        public static readonly object Char_00D4 = '\x00D4';

        /// <summary>
        /// Expression that returns the singleton boxed char 212 (0x00D4).
        /// </summary>
        public static readonly Expression Char_00D4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D4");

        /// <summary>
        /// A singleton boxed char 213 (0x00D5).
        /// </summary>
        public static readonly object Char_00D5 = '\x00D5';

        /// <summary>
        /// Expression that returns the singleton boxed char 213 (0x00D5).
        /// </summary>
        public static readonly Expression Char_00D5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D5");

        /// <summary>
        /// A singleton boxed char 214 (0x00D6).
        /// </summary>
        public static readonly object Char_00D6 = '\x00D6';

        /// <summary>
        /// Expression that returns the singleton boxed char 214 (0x00D6).
        /// </summary>
        public static readonly Expression Char_00D6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D6");

        /// <summary>
        /// A singleton boxed char 215 (0x00D7).
        /// </summary>
        public static readonly object Char_00D7 = '\x00D7';

        /// <summary>
        /// Expression that returns the singleton boxed char 215 (0x00D7).
        /// </summary>
        public static readonly Expression Char_00D7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D7");

        /// <summary>
        /// A singleton boxed char 216 (0x00D8).
        /// </summary>
        public static readonly object Char_00D8 = '\x00D8';

        /// <summary>
        /// Expression that returns the singleton boxed char 216 (0x00D8).
        /// </summary>
        public static readonly Expression Char_00D8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D8");

        /// <summary>
        /// A singleton boxed char 217 (0x00D9).
        /// </summary>
        public static readonly object Char_00D9 = '\x00D9';

        /// <summary>
        /// Expression that returns the singleton boxed char 217 (0x00D9).
        /// </summary>
        public static readonly Expression Char_00D9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00D9");

        /// <summary>
        /// A singleton boxed char 218 (0x00DA).
        /// </summary>
        public static readonly object Char_00DA = '\x00DA';

        /// <summary>
        /// Expression that returns the singleton boxed char 218 (0x00DA).
        /// </summary>
        public static readonly Expression Char_00DA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DA");

        /// <summary>
        /// A singleton boxed char 219 (0x00DB).
        /// </summary>
        public static readonly object Char_00DB = '\x00DB';

        /// <summary>
        /// Expression that returns the singleton boxed char 219 (0x00DB).
        /// </summary>
        public static readonly Expression Char_00DB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DB");

        /// <summary>
        /// A singleton boxed char 220 (0x00DC).
        /// </summary>
        public static readonly object Char_00DC = '\x00DC';

        /// <summary>
        /// Expression that returns the singleton boxed char 220 (0x00DC).
        /// </summary>
        public static readonly Expression Char_00DC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DC");

        /// <summary>
        /// A singleton boxed char 221 (0x00DD).
        /// </summary>
        public static readonly object Char_00DD = '\x00DD';

        /// <summary>
        /// Expression that returns the singleton boxed char 221 (0x00DD).
        /// </summary>
        public static readonly Expression Char_00DD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DD");

        /// <summary>
        /// A singleton boxed char 222 (0x00DE).
        /// </summary>
        public static readonly object Char_00DE = '\x00DE';

        /// <summary>
        /// Expression that returns the singleton boxed char 222 (0x00DE).
        /// </summary>
        public static readonly Expression Char_00DE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DE");

        /// <summary>
        /// A singleton boxed char 223 (0x00DF).
        /// </summary>
        public static readonly object Char_00DF = '\x00DF';

        /// <summary>
        /// Expression that returns the singleton boxed char 223 (0x00DF).
        /// </summary>
        public static readonly Expression Char_00DF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00DF");

        /// <summary>
        /// A singleton boxed char 224 (0x00E0).
        /// </summary>
        public static readonly object Char_00E0 = '\x00E0';

        /// <summary>
        /// Expression that returns the singleton boxed char 224 (0x00E0).
        /// </summary>
        public static readonly Expression Char_00E0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E0");

        /// <summary>
        /// A singleton boxed char 225 (0x00E1).
        /// </summary>
        public static readonly object Char_00E1 = '\x00E1';

        /// <summary>
        /// Expression that returns the singleton boxed char 225 (0x00E1).
        /// </summary>
        public static readonly Expression Char_00E1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E1");

        /// <summary>
        /// A singleton boxed char 226 (0x00E2).
        /// </summary>
        public static readonly object Char_00E2 = '\x00E2';

        /// <summary>
        /// Expression that returns the singleton boxed char 226 (0x00E2).
        /// </summary>
        public static readonly Expression Char_00E2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E2");

        /// <summary>
        /// A singleton boxed char 227 (0x00E3).
        /// </summary>
        public static readonly object Char_00E3 = '\x00E3';

        /// <summary>
        /// Expression that returns the singleton boxed char 227 (0x00E3).
        /// </summary>
        public static readonly Expression Char_00E3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E3");

        /// <summary>
        /// A singleton boxed char 228 (0x00E4).
        /// </summary>
        public static readonly object Char_00E4 = '\x00E4';

        /// <summary>
        /// Expression that returns the singleton boxed char 228 (0x00E4).
        /// </summary>
        public static readonly Expression Char_00E4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E4");

        /// <summary>
        /// A singleton boxed char 229 (0x00E5).
        /// </summary>
        public static readonly object Char_00E5 = '\x00E5';

        /// <summary>
        /// Expression that returns the singleton boxed char 229 (0x00E5).
        /// </summary>
        public static readonly Expression Char_00E5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E5");

        /// <summary>
        /// A singleton boxed char 230 (0x00E6).
        /// </summary>
        public static readonly object Char_00E6 = '\x00E6';

        /// <summary>
        /// Expression that returns the singleton boxed char 230 (0x00E6).
        /// </summary>
        public static readonly Expression Char_00E6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E6");

        /// <summary>
        /// A singleton boxed char 231 (0x00E7).
        /// </summary>
        public static readonly object Char_00E7 = '\x00E7';

        /// <summary>
        /// Expression that returns the singleton boxed char 231 (0x00E7).
        /// </summary>
        public static readonly Expression Char_00E7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E7");

        /// <summary>
        /// A singleton boxed char 232 (0x00E8).
        /// </summary>
        public static readonly object Char_00E8 = '\x00E8';

        /// <summary>
        /// Expression that returns the singleton boxed char 232 (0x00E8).
        /// </summary>
        public static readonly Expression Char_00E8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E8");

        /// <summary>
        /// A singleton boxed char 233 (0x00E9).
        /// </summary>
        public static readonly object Char_00E9 = '\x00E9';

        /// <summary>
        /// Expression that returns the singleton boxed char 233 (0x00E9).
        /// </summary>
        public static readonly Expression Char_00E9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00E9");

        /// <summary>
        /// A singleton boxed char 234 (0x00EA).
        /// </summary>
        public static readonly object Char_00EA = '\x00EA';

        /// <summary>
        /// Expression that returns the singleton boxed char 234 (0x00EA).
        /// </summary>
        public static readonly Expression Char_00EA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00EA");

        /// <summary>
        /// A singleton boxed char 235 (0x00EB).
        /// </summary>
        public static readonly object Char_00EB = '\x00EB';

        /// <summary>
        /// Expression that returns the singleton boxed char 235 (0x00EB).
        /// </summary>
        public static readonly Expression Char_00EB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00EB");

        /// <summary>
        /// A singleton boxed char 236 (0x00EC).
        /// </summary>
        public static readonly object Char_00EC = '\x00EC';

        /// <summary>
        /// Expression that returns the singleton boxed char 236 (0x00EC).
        /// </summary>
        public static readonly Expression Char_00EC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00EC");

        /// <summary>
        /// A singleton boxed char 237 (0x00ED).
        /// </summary>
        public static readonly object Char_00ED = '\x00ED';

        /// <summary>
        /// Expression that returns the singleton boxed char 237 (0x00ED).
        /// </summary>
        public static readonly Expression Char_00ED_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00ED");

        /// <summary>
        /// A singleton boxed char 238 (0x00EE).
        /// </summary>
        public static readonly object Char_00EE = '\x00EE';

        /// <summary>
        /// Expression that returns the singleton boxed char 238 (0x00EE).
        /// </summary>
        public static readonly Expression Char_00EE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00EE");

        /// <summary>
        /// A singleton boxed char 239 (0x00EF).
        /// </summary>
        public static readonly object Char_00EF = '\x00EF';

        /// <summary>
        /// Expression that returns the singleton boxed char 239 (0x00EF).
        /// </summary>
        public static readonly Expression Char_00EF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00EF");

        /// <summary>
        /// A singleton boxed char 240 (0x00F0).
        /// </summary>
        public static readonly object Char_00F0 = '\x00F0';

        /// <summary>
        /// Expression that returns the singleton boxed char 240 (0x00F0).
        /// </summary>
        public static readonly Expression Char_00F0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F0");

        /// <summary>
        /// A singleton boxed char 241 (0x00F1).
        /// </summary>
        public static readonly object Char_00F1 = '\x00F1';

        /// <summary>
        /// Expression that returns the singleton boxed char 241 (0x00F1).
        /// </summary>
        public static readonly Expression Char_00F1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F1");

        /// <summary>
        /// A singleton boxed char 242 (0x00F2).
        /// </summary>
        public static readonly object Char_00F2 = '\x00F2';

        /// <summary>
        /// Expression that returns the singleton boxed char 242 (0x00F2).
        /// </summary>
        public static readonly Expression Char_00F2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F2");

        /// <summary>
        /// A singleton boxed char 243 (0x00F3).
        /// </summary>
        public static readonly object Char_00F3 = '\x00F3';

        /// <summary>
        /// Expression that returns the singleton boxed char 243 (0x00F3).
        /// </summary>
        public static readonly Expression Char_00F3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F3");

        /// <summary>
        /// A singleton boxed char 244 (0x00F4).
        /// </summary>
        public static readonly object Char_00F4 = '\x00F4';

        /// <summary>
        /// Expression that returns the singleton boxed char 244 (0x00F4).
        /// </summary>
        public static readonly Expression Char_00F4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F4");

        /// <summary>
        /// A singleton boxed char 245 (0x00F5).
        /// </summary>
        public static readonly object Char_00F5 = '\x00F5';

        /// <summary>
        /// Expression that returns the singleton boxed char 245 (0x00F5).
        /// </summary>
        public static readonly Expression Char_00F5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F5");

        /// <summary>
        /// A singleton boxed char 246 (0x00F6).
        /// </summary>
        public static readonly object Char_00F6 = '\x00F6';

        /// <summary>
        /// Expression that returns the singleton boxed char 246 (0x00F6).
        /// </summary>
        public static readonly Expression Char_00F6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F6");

        /// <summary>
        /// A singleton boxed char 247 (0x00F7).
        /// </summary>
        public static readonly object Char_00F7 = '\x00F7';

        /// <summary>
        /// Expression that returns the singleton boxed char 247 (0x00F7).
        /// </summary>
        public static readonly Expression Char_00F7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F7");

        /// <summary>
        /// A singleton boxed char 248 (0x00F8).
        /// </summary>
        public static readonly object Char_00F8 = '\x00F8';

        /// <summary>
        /// Expression that returns the singleton boxed char 248 (0x00F8).
        /// </summary>
        public static readonly Expression Char_00F8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F8");

        /// <summary>
        /// A singleton boxed char 249 (0x00F9).
        /// </summary>
        public static readonly object Char_00F9 = '\x00F9';

        /// <summary>
        /// Expression that returns the singleton boxed char 249 (0x00F9).
        /// </summary>
        public static readonly Expression Char_00F9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00F9");

        /// <summary>
        /// A singleton boxed char 250 (0x00FA).
        /// </summary>
        public static readonly object Char_00FA = '\x00FA';

        /// <summary>
        /// Expression that returns the singleton boxed char 250 (0x00FA).
        /// </summary>
        public static readonly Expression Char_00FA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FA");

        /// <summary>
        /// A singleton boxed char 251 (0x00FB).
        /// </summary>
        public static readonly object Char_00FB = '\x00FB';

        /// <summary>
        /// Expression that returns the singleton boxed char 251 (0x00FB).
        /// </summary>
        public static readonly Expression Char_00FB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FB");

        /// <summary>
        /// A singleton boxed char 252 (0x00FC).
        /// </summary>
        public static readonly object Char_00FC = '\x00FC';

        /// <summary>
        /// Expression that returns the singleton boxed char 252 (0x00FC).
        /// </summary>
        public static readonly Expression Char_00FC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FC");

        /// <summary>
        /// A singleton boxed char 253 (0x00FD).
        /// </summary>
        public static readonly object Char_00FD = '\x00FD';

        /// <summary>
        /// Expression that returns the singleton boxed char 253 (0x00FD).
        /// </summary>
        public static readonly Expression Char_00FD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FD");

        /// <summary>
        /// A singleton boxed char 254 (0x00FE).
        /// </summary>
        public static readonly object Char_00FE = '\x00FE';

        /// <summary>
        /// Expression that returns the singleton boxed char 254 (0x00FE).
        /// </summary>
        public static readonly Expression Char_00FE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FE");

        /// <summary>
        /// A singleton boxed char 255 (0x00FF).
        /// </summary>
        public static readonly object Char_00FF = '\x00FF';

        /// <summary>
        /// Expression that returns the singleton boxed char 255 (0x00FF).
        /// </summary>
        public static readonly Expression Char_00FF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Char_00FF");



        #endregion

        #region 32-bit Integer

        public static Expression GetConstant(int value)
        {
            if ((-255 <= value) && (value <= 255))
                return PreboxedConstants.Int32_Expressions[value+255];
            return null;
        }

        public static object GetValue(int value)
        {
            if ((-255 <= value) && (value <= 255))
                return PreboxedConstants.Int32_Objects[value+255];
            return null;
        }

        private static readonly object[] Int32_Objects;

        private static object[] Get_Int32_Objects()
        {
            return new object[]
            {
                PreboxedConstants.Int32_FFFFFF01,
                PreboxedConstants.Int32_FFFFFF02,
                PreboxedConstants.Int32_FFFFFF03,
                PreboxedConstants.Int32_FFFFFF04,
                PreboxedConstants.Int32_FFFFFF05,
                PreboxedConstants.Int32_FFFFFF06,
                PreboxedConstants.Int32_FFFFFF07,
                PreboxedConstants.Int32_FFFFFF08,
                PreboxedConstants.Int32_FFFFFF09,
                PreboxedConstants.Int32_FFFFFF0A,
                PreboxedConstants.Int32_FFFFFF0B,
                PreboxedConstants.Int32_FFFFFF0C,
                PreboxedConstants.Int32_FFFFFF0D,
                PreboxedConstants.Int32_FFFFFF0E,
                PreboxedConstants.Int32_FFFFFF0F,
                PreboxedConstants.Int32_FFFFFF10,
                PreboxedConstants.Int32_FFFFFF11,
                PreboxedConstants.Int32_FFFFFF12,
                PreboxedConstants.Int32_FFFFFF13,
                PreboxedConstants.Int32_FFFFFF14,
                PreboxedConstants.Int32_FFFFFF15,
                PreboxedConstants.Int32_FFFFFF16,
                PreboxedConstants.Int32_FFFFFF17,
                PreboxedConstants.Int32_FFFFFF18,
                PreboxedConstants.Int32_FFFFFF19,
                PreboxedConstants.Int32_FFFFFF1A,
                PreboxedConstants.Int32_FFFFFF1B,
                PreboxedConstants.Int32_FFFFFF1C,
                PreboxedConstants.Int32_FFFFFF1D,
                PreboxedConstants.Int32_FFFFFF1E,
                PreboxedConstants.Int32_FFFFFF1F,
                PreboxedConstants.Int32_FFFFFF20,
                PreboxedConstants.Int32_FFFFFF21,
                PreboxedConstants.Int32_FFFFFF22,
                PreboxedConstants.Int32_FFFFFF23,
                PreboxedConstants.Int32_FFFFFF24,
                PreboxedConstants.Int32_FFFFFF25,
                PreboxedConstants.Int32_FFFFFF26,
                PreboxedConstants.Int32_FFFFFF27,
                PreboxedConstants.Int32_FFFFFF28,
                PreboxedConstants.Int32_FFFFFF29,
                PreboxedConstants.Int32_FFFFFF2A,
                PreboxedConstants.Int32_FFFFFF2B,
                PreboxedConstants.Int32_FFFFFF2C,
                PreboxedConstants.Int32_FFFFFF2D,
                PreboxedConstants.Int32_FFFFFF2E,
                PreboxedConstants.Int32_FFFFFF2F,
                PreboxedConstants.Int32_FFFFFF30,
                PreboxedConstants.Int32_FFFFFF31,
                PreboxedConstants.Int32_FFFFFF32,
                PreboxedConstants.Int32_FFFFFF33,
                PreboxedConstants.Int32_FFFFFF34,
                PreboxedConstants.Int32_FFFFFF35,
                PreboxedConstants.Int32_FFFFFF36,
                PreboxedConstants.Int32_FFFFFF37,
                PreboxedConstants.Int32_FFFFFF38,
                PreboxedConstants.Int32_FFFFFF39,
                PreboxedConstants.Int32_FFFFFF3A,
                PreboxedConstants.Int32_FFFFFF3B,
                PreboxedConstants.Int32_FFFFFF3C,
                PreboxedConstants.Int32_FFFFFF3D,
                PreboxedConstants.Int32_FFFFFF3E,
                PreboxedConstants.Int32_FFFFFF3F,
                PreboxedConstants.Int32_FFFFFF40,
                PreboxedConstants.Int32_FFFFFF41,
                PreboxedConstants.Int32_FFFFFF42,
                PreboxedConstants.Int32_FFFFFF43,
                PreboxedConstants.Int32_FFFFFF44,
                PreboxedConstants.Int32_FFFFFF45,
                PreboxedConstants.Int32_FFFFFF46,
                PreboxedConstants.Int32_FFFFFF47,
                PreboxedConstants.Int32_FFFFFF48,
                PreboxedConstants.Int32_FFFFFF49,
                PreboxedConstants.Int32_FFFFFF4A,
                PreboxedConstants.Int32_FFFFFF4B,
                PreboxedConstants.Int32_FFFFFF4C,
                PreboxedConstants.Int32_FFFFFF4D,
                PreboxedConstants.Int32_FFFFFF4E,
                PreboxedConstants.Int32_FFFFFF4F,
                PreboxedConstants.Int32_FFFFFF50,
                PreboxedConstants.Int32_FFFFFF51,
                PreboxedConstants.Int32_FFFFFF52,
                PreboxedConstants.Int32_FFFFFF53,
                PreboxedConstants.Int32_FFFFFF54,
                PreboxedConstants.Int32_FFFFFF55,
                PreboxedConstants.Int32_FFFFFF56,
                PreboxedConstants.Int32_FFFFFF57,
                PreboxedConstants.Int32_FFFFFF58,
                PreboxedConstants.Int32_FFFFFF59,
                PreboxedConstants.Int32_FFFFFF5A,
                PreboxedConstants.Int32_FFFFFF5B,
                PreboxedConstants.Int32_FFFFFF5C,
                PreboxedConstants.Int32_FFFFFF5D,
                PreboxedConstants.Int32_FFFFFF5E,
                PreboxedConstants.Int32_FFFFFF5F,
                PreboxedConstants.Int32_FFFFFF60,
                PreboxedConstants.Int32_FFFFFF61,
                PreboxedConstants.Int32_FFFFFF62,
                PreboxedConstants.Int32_FFFFFF63,
                PreboxedConstants.Int32_FFFFFF64,
                PreboxedConstants.Int32_FFFFFF65,
                PreboxedConstants.Int32_FFFFFF66,
                PreboxedConstants.Int32_FFFFFF67,
                PreboxedConstants.Int32_FFFFFF68,
                PreboxedConstants.Int32_FFFFFF69,
                PreboxedConstants.Int32_FFFFFF6A,
                PreboxedConstants.Int32_FFFFFF6B,
                PreboxedConstants.Int32_FFFFFF6C,
                PreboxedConstants.Int32_FFFFFF6D,
                PreboxedConstants.Int32_FFFFFF6E,
                PreboxedConstants.Int32_FFFFFF6F,
                PreboxedConstants.Int32_FFFFFF70,
                PreboxedConstants.Int32_FFFFFF71,
                PreboxedConstants.Int32_FFFFFF72,
                PreboxedConstants.Int32_FFFFFF73,
                PreboxedConstants.Int32_FFFFFF74,
                PreboxedConstants.Int32_FFFFFF75,
                PreboxedConstants.Int32_FFFFFF76,
                PreboxedConstants.Int32_FFFFFF77,
                PreboxedConstants.Int32_FFFFFF78,
                PreboxedConstants.Int32_FFFFFF79,
                PreboxedConstants.Int32_FFFFFF7A,
                PreboxedConstants.Int32_FFFFFF7B,
                PreboxedConstants.Int32_FFFFFF7C,
                PreboxedConstants.Int32_FFFFFF7D,
                PreboxedConstants.Int32_FFFFFF7E,
                PreboxedConstants.Int32_FFFFFF7F,
                PreboxedConstants.Int32_FFFFFF80,
                PreboxedConstants.Int32_FFFFFF81,
                PreboxedConstants.Int32_FFFFFF82,
                PreboxedConstants.Int32_FFFFFF83,
                PreboxedConstants.Int32_FFFFFF84,
                PreboxedConstants.Int32_FFFFFF85,
                PreboxedConstants.Int32_FFFFFF86,
                PreboxedConstants.Int32_FFFFFF87,
                PreboxedConstants.Int32_FFFFFF88,
                PreboxedConstants.Int32_FFFFFF89,
                PreboxedConstants.Int32_FFFFFF8A,
                PreboxedConstants.Int32_FFFFFF8B,
                PreboxedConstants.Int32_FFFFFF8C,
                PreboxedConstants.Int32_FFFFFF8D,
                PreboxedConstants.Int32_FFFFFF8E,
                PreboxedConstants.Int32_FFFFFF8F,
                PreboxedConstants.Int32_FFFFFF90,
                PreboxedConstants.Int32_FFFFFF91,
                PreboxedConstants.Int32_FFFFFF92,
                PreboxedConstants.Int32_FFFFFF93,
                PreboxedConstants.Int32_FFFFFF94,
                PreboxedConstants.Int32_FFFFFF95,
                PreboxedConstants.Int32_FFFFFF96,
                PreboxedConstants.Int32_FFFFFF97,
                PreboxedConstants.Int32_FFFFFF98,
                PreboxedConstants.Int32_FFFFFF99,
                PreboxedConstants.Int32_FFFFFF9A,
                PreboxedConstants.Int32_FFFFFF9B,
                PreboxedConstants.Int32_FFFFFF9C,
                PreboxedConstants.Int32_FFFFFF9D,
                PreboxedConstants.Int32_FFFFFF9E,
                PreboxedConstants.Int32_FFFFFF9F,
                PreboxedConstants.Int32_FFFFFFA0,
                PreboxedConstants.Int32_FFFFFFA1,
                PreboxedConstants.Int32_FFFFFFA2,
                PreboxedConstants.Int32_FFFFFFA3,
                PreboxedConstants.Int32_FFFFFFA4,
                PreboxedConstants.Int32_FFFFFFA5,
                PreboxedConstants.Int32_FFFFFFA6,
                PreboxedConstants.Int32_FFFFFFA7,
                PreboxedConstants.Int32_FFFFFFA8,
                PreboxedConstants.Int32_FFFFFFA9,
                PreboxedConstants.Int32_FFFFFFAA,
                PreboxedConstants.Int32_FFFFFFAB,
                PreboxedConstants.Int32_FFFFFFAC,
                PreboxedConstants.Int32_FFFFFFAD,
                PreboxedConstants.Int32_FFFFFFAE,
                PreboxedConstants.Int32_FFFFFFAF,
                PreboxedConstants.Int32_FFFFFFB0,
                PreboxedConstants.Int32_FFFFFFB1,
                PreboxedConstants.Int32_FFFFFFB2,
                PreboxedConstants.Int32_FFFFFFB3,
                PreboxedConstants.Int32_FFFFFFB4,
                PreboxedConstants.Int32_FFFFFFB5,
                PreboxedConstants.Int32_FFFFFFB6,
                PreboxedConstants.Int32_FFFFFFB7,
                PreboxedConstants.Int32_FFFFFFB8,
                PreboxedConstants.Int32_FFFFFFB9,
                PreboxedConstants.Int32_FFFFFFBA,
                PreboxedConstants.Int32_FFFFFFBB,
                PreboxedConstants.Int32_FFFFFFBC,
                PreboxedConstants.Int32_FFFFFFBD,
                PreboxedConstants.Int32_FFFFFFBE,
                PreboxedConstants.Int32_FFFFFFBF,
                PreboxedConstants.Int32_FFFFFFC0,
                PreboxedConstants.Int32_FFFFFFC1,
                PreboxedConstants.Int32_FFFFFFC2,
                PreboxedConstants.Int32_FFFFFFC3,
                PreboxedConstants.Int32_FFFFFFC4,
                PreboxedConstants.Int32_FFFFFFC5,
                PreboxedConstants.Int32_FFFFFFC6,
                PreboxedConstants.Int32_FFFFFFC7,
                PreboxedConstants.Int32_FFFFFFC8,
                PreboxedConstants.Int32_FFFFFFC9,
                PreboxedConstants.Int32_FFFFFFCA,
                PreboxedConstants.Int32_FFFFFFCB,
                PreboxedConstants.Int32_FFFFFFCC,
                PreboxedConstants.Int32_FFFFFFCD,
                PreboxedConstants.Int32_FFFFFFCE,
                PreboxedConstants.Int32_FFFFFFCF,
                PreboxedConstants.Int32_FFFFFFD0,
                PreboxedConstants.Int32_FFFFFFD1,
                PreboxedConstants.Int32_FFFFFFD2,
                PreboxedConstants.Int32_FFFFFFD3,
                PreboxedConstants.Int32_FFFFFFD4,
                PreboxedConstants.Int32_FFFFFFD5,
                PreboxedConstants.Int32_FFFFFFD6,
                PreboxedConstants.Int32_FFFFFFD7,
                PreboxedConstants.Int32_FFFFFFD8,
                PreboxedConstants.Int32_FFFFFFD9,
                PreboxedConstants.Int32_FFFFFFDA,
                PreboxedConstants.Int32_FFFFFFDB,
                PreboxedConstants.Int32_FFFFFFDC,
                PreboxedConstants.Int32_FFFFFFDD,
                PreboxedConstants.Int32_FFFFFFDE,
                PreboxedConstants.Int32_FFFFFFDF,
                PreboxedConstants.Int32_FFFFFFE0,
                PreboxedConstants.Int32_FFFFFFE1,
                PreboxedConstants.Int32_FFFFFFE2,
                PreboxedConstants.Int32_FFFFFFE3,
                PreboxedConstants.Int32_FFFFFFE4,
                PreboxedConstants.Int32_FFFFFFE5,
                PreboxedConstants.Int32_FFFFFFE6,
                PreboxedConstants.Int32_FFFFFFE7,
                PreboxedConstants.Int32_FFFFFFE8,
                PreboxedConstants.Int32_FFFFFFE9,
                PreboxedConstants.Int32_FFFFFFEA,
                PreboxedConstants.Int32_FFFFFFEB,
                PreboxedConstants.Int32_FFFFFFEC,
                PreboxedConstants.Int32_FFFFFFED,
                PreboxedConstants.Int32_FFFFFFEE,
                PreboxedConstants.Int32_FFFFFFEF,
                PreboxedConstants.Int32_FFFFFFF0,
                PreboxedConstants.Int32_FFFFFFF1,
                PreboxedConstants.Int32_FFFFFFF2,
                PreboxedConstants.Int32_FFFFFFF3,
                PreboxedConstants.Int32_FFFFFFF4,
                PreboxedConstants.Int32_FFFFFFF5,
                PreboxedConstants.Int32_FFFFFFF6,
                PreboxedConstants.Int32_FFFFFFF7,
                PreboxedConstants.Int32_FFFFFFF8,
                PreboxedConstants.Int32_FFFFFFF9,
                PreboxedConstants.Int32_FFFFFFFA,
                PreboxedConstants.Int32_FFFFFFFB,
                PreboxedConstants.Int32_FFFFFFFC,
                PreboxedConstants.Int32_FFFFFFFD,
                PreboxedConstants.Int32_FFFFFFFE,
                PreboxedConstants.Int32_FFFFFFFF,
                PreboxedConstants.Int32_00000000,
                PreboxedConstants.Int32_00000001,
                PreboxedConstants.Int32_00000002,
                PreboxedConstants.Int32_00000003,
                PreboxedConstants.Int32_00000004,
                PreboxedConstants.Int32_00000005,
                PreboxedConstants.Int32_00000006,
                PreboxedConstants.Int32_00000007,
                PreboxedConstants.Int32_00000008,
                PreboxedConstants.Int32_00000009,
                PreboxedConstants.Int32_0000000A,
                PreboxedConstants.Int32_0000000B,
                PreboxedConstants.Int32_0000000C,
                PreboxedConstants.Int32_0000000D,
                PreboxedConstants.Int32_0000000E,
                PreboxedConstants.Int32_0000000F,
                PreboxedConstants.Int32_00000010,
                PreboxedConstants.Int32_00000011,
                PreboxedConstants.Int32_00000012,
                PreboxedConstants.Int32_00000013,
                PreboxedConstants.Int32_00000014,
                PreboxedConstants.Int32_00000015,
                PreboxedConstants.Int32_00000016,
                PreboxedConstants.Int32_00000017,
                PreboxedConstants.Int32_00000018,
                PreboxedConstants.Int32_00000019,
                PreboxedConstants.Int32_0000001A,
                PreboxedConstants.Int32_0000001B,
                PreboxedConstants.Int32_0000001C,
                PreboxedConstants.Int32_0000001D,
                PreboxedConstants.Int32_0000001E,
                PreboxedConstants.Int32_0000001F,
                PreboxedConstants.Int32_00000020,
                PreboxedConstants.Int32_00000021,
                PreboxedConstants.Int32_00000022,
                PreboxedConstants.Int32_00000023,
                PreboxedConstants.Int32_00000024,
                PreboxedConstants.Int32_00000025,
                PreboxedConstants.Int32_00000026,
                PreboxedConstants.Int32_00000027,
                PreboxedConstants.Int32_00000028,
                PreboxedConstants.Int32_00000029,
                PreboxedConstants.Int32_0000002A,
                PreboxedConstants.Int32_0000002B,
                PreboxedConstants.Int32_0000002C,
                PreboxedConstants.Int32_0000002D,
                PreboxedConstants.Int32_0000002E,
                PreboxedConstants.Int32_0000002F,
                PreboxedConstants.Int32_00000030,
                PreboxedConstants.Int32_00000031,
                PreboxedConstants.Int32_00000032,
                PreboxedConstants.Int32_00000033,
                PreboxedConstants.Int32_00000034,
                PreboxedConstants.Int32_00000035,
                PreboxedConstants.Int32_00000036,
                PreboxedConstants.Int32_00000037,
                PreboxedConstants.Int32_00000038,
                PreboxedConstants.Int32_00000039,
                PreboxedConstants.Int32_0000003A,
                PreboxedConstants.Int32_0000003B,
                PreboxedConstants.Int32_0000003C,
                PreboxedConstants.Int32_0000003D,
                PreboxedConstants.Int32_0000003E,
                PreboxedConstants.Int32_0000003F,
                PreboxedConstants.Int32_00000040,
                PreboxedConstants.Int32_00000041,
                PreboxedConstants.Int32_00000042,
                PreboxedConstants.Int32_00000043,
                PreboxedConstants.Int32_00000044,
                PreboxedConstants.Int32_00000045,
                PreboxedConstants.Int32_00000046,
                PreboxedConstants.Int32_00000047,
                PreboxedConstants.Int32_00000048,
                PreboxedConstants.Int32_00000049,
                PreboxedConstants.Int32_0000004A,
                PreboxedConstants.Int32_0000004B,
                PreboxedConstants.Int32_0000004C,
                PreboxedConstants.Int32_0000004D,
                PreboxedConstants.Int32_0000004E,
                PreboxedConstants.Int32_0000004F,
                PreboxedConstants.Int32_00000050,
                PreboxedConstants.Int32_00000051,
                PreboxedConstants.Int32_00000052,
                PreboxedConstants.Int32_00000053,
                PreboxedConstants.Int32_00000054,
                PreboxedConstants.Int32_00000055,
                PreboxedConstants.Int32_00000056,
                PreboxedConstants.Int32_00000057,
                PreboxedConstants.Int32_00000058,
                PreboxedConstants.Int32_00000059,
                PreboxedConstants.Int32_0000005A,
                PreboxedConstants.Int32_0000005B,
                PreboxedConstants.Int32_0000005C,
                PreboxedConstants.Int32_0000005D,
                PreboxedConstants.Int32_0000005E,
                PreboxedConstants.Int32_0000005F,
                PreboxedConstants.Int32_00000060,
                PreboxedConstants.Int32_00000061,
                PreboxedConstants.Int32_00000062,
                PreboxedConstants.Int32_00000063,
                PreboxedConstants.Int32_00000064,
                PreboxedConstants.Int32_00000065,
                PreboxedConstants.Int32_00000066,
                PreboxedConstants.Int32_00000067,
                PreboxedConstants.Int32_00000068,
                PreboxedConstants.Int32_00000069,
                PreboxedConstants.Int32_0000006A,
                PreboxedConstants.Int32_0000006B,
                PreboxedConstants.Int32_0000006C,
                PreboxedConstants.Int32_0000006D,
                PreboxedConstants.Int32_0000006E,
                PreboxedConstants.Int32_0000006F,
                PreboxedConstants.Int32_00000070,
                PreboxedConstants.Int32_00000071,
                PreboxedConstants.Int32_00000072,
                PreboxedConstants.Int32_00000073,
                PreboxedConstants.Int32_00000074,
                PreboxedConstants.Int32_00000075,
                PreboxedConstants.Int32_00000076,
                PreboxedConstants.Int32_00000077,
                PreboxedConstants.Int32_00000078,
                PreboxedConstants.Int32_00000079,
                PreboxedConstants.Int32_0000007A,
                PreboxedConstants.Int32_0000007B,
                PreboxedConstants.Int32_0000007C,
                PreboxedConstants.Int32_0000007D,
                PreboxedConstants.Int32_0000007E,
                PreboxedConstants.Int32_0000007F,
                PreboxedConstants.Int32_00000080,
                PreboxedConstants.Int32_00000081,
                PreboxedConstants.Int32_00000082,
                PreboxedConstants.Int32_00000083,
                PreboxedConstants.Int32_00000084,
                PreboxedConstants.Int32_00000085,
                PreboxedConstants.Int32_00000086,
                PreboxedConstants.Int32_00000087,
                PreboxedConstants.Int32_00000088,
                PreboxedConstants.Int32_00000089,
                PreboxedConstants.Int32_0000008A,
                PreboxedConstants.Int32_0000008B,
                PreboxedConstants.Int32_0000008C,
                PreboxedConstants.Int32_0000008D,
                PreboxedConstants.Int32_0000008E,
                PreboxedConstants.Int32_0000008F,
                PreboxedConstants.Int32_00000090,
                PreboxedConstants.Int32_00000091,
                PreboxedConstants.Int32_00000092,
                PreboxedConstants.Int32_00000093,
                PreboxedConstants.Int32_00000094,
                PreboxedConstants.Int32_00000095,
                PreboxedConstants.Int32_00000096,
                PreboxedConstants.Int32_00000097,
                PreboxedConstants.Int32_00000098,
                PreboxedConstants.Int32_00000099,
                PreboxedConstants.Int32_0000009A,
                PreboxedConstants.Int32_0000009B,
                PreboxedConstants.Int32_0000009C,
                PreboxedConstants.Int32_0000009D,
                PreboxedConstants.Int32_0000009E,
                PreboxedConstants.Int32_0000009F,
                PreboxedConstants.Int32_000000A0,
                PreboxedConstants.Int32_000000A1,
                PreboxedConstants.Int32_000000A2,
                PreboxedConstants.Int32_000000A3,
                PreboxedConstants.Int32_000000A4,
                PreboxedConstants.Int32_000000A5,
                PreboxedConstants.Int32_000000A6,
                PreboxedConstants.Int32_000000A7,
                PreboxedConstants.Int32_000000A8,
                PreboxedConstants.Int32_000000A9,
                PreboxedConstants.Int32_000000AA,
                PreboxedConstants.Int32_000000AB,
                PreboxedConstants.Int32_000000AC,
                PreboxedConstants.Int32_000000AD,
                PreboxedConstants.Int32_000000AE,
                PreboxedConstants.Int32_000000AF,
                PreboxedConstants.Int32_000000B0,
                PreboxedConstants.Int32_000000B1,
                PreboxedConstants.Int32_000000B2,
                PreboxedConstants.Int32_000000B3,
                PreboxedConstants.Int32_000000B4,
                PreboxedConstants.Int32_000000B5,
                PreboxedConstants.Int32_000000B6,
                PreboxedConstants.Int32_000000B7,
                PreboxedConstants.Int32_000000B8,
                PreboxedConstants.Int32_000000B9,
                PreboxedConstants.Int32_000000BA,
                PreboxedConstants.Int32_000000BB,
                PreboxedConstants.Int32_000000BC,
                PreboxedConstants.Int32_000000BD,
                PreboxedConstants.Int32_000000BE,
                PreboxedConstants.Int32_000000BF,
                PreboxedConstants.Int32_000000C0,
                PreboxedConstants.Int32_000000C1,
                PreboxedConstants.Int32_000000C2,
                PreboxedConstants.Int32_000000C3,
                PreboxedConstants.Int32_000000C4,
                PreboxedConstants.Int32_000000C5,
                PreboxedConstants.Int32_000000C6,
                PreboxedConstants.Int32_000000C7,
                PreboxedConstants.Int32_000000C8,
                PreboxedConstants.Int32_000000C9,
                PreboxedConstants.Int32_000000CA,
                PreboxedConstants.Int32_000000CB,
                PreboxedConstants.Int32_000000CC,
                PreboxedConstants.Int32_000000CD,
                PreboxedConstants.Int32_000000CE,
                PreboxedConstants.Int32_000000CF,
                PreboxedConstants.Int32_000000D0,
                PreboxedConstants.Int32_000000D1,
                PreboxedConstants.Int32_000000D2,
                PreboxedConstants.Int32_000000D3,
                PreboxedConstants.Int32_000000D4,
                PreboxedConstants.Int32_000000D5,
                PreboxedConstants.Int32_000000D6,
                PreboxedConstants.Int32_000000D7,
                PreboxedConstants.Int32_000000D8,
                PreboxedConstants.Int32_000000D9,
                PreboxedConstants.Int32_000000DA,
                PreboxedConstants.Int32_000000DB,
                PreboxedConstants.Int32_000000DC,
                PreboxedConstants.Int32_000000DD,
                PreboxedConstants.Int32_000000DE,
                PreboxedConstants.Int32_000000DF,
                PreboxedConstants.Int32_000000E0,
                PreboxedConstants.Int32_000000E1,
                PreboxedConstants.Int32_000000E2,
                PreboxedConstants.Int32_000000E3,
                PreboxedConstants.Int32_000000E4,
                PreboxedConstants.Int32_000000E5,
                PreboxedConstants.Int32_000000E6,
                PreboxedConstants.Int32_000000E7,
                PreboxedConstants.Int32_000000E8,
                PreboxedConstants.Int32_000000E9,
                PreboxedConstants.Int32_000000EA,
                PreboxedConstants.Int32_000000EB,
                PreboxedConstants.Int32_000000EC,
                PreboxedConstants.Int32_000000ED,
                PreboxedConstants.Int32_000000EE,
                PreboxedConstants.Int32_000000EF,
                PreboxedConstants.Int32_000000F0,
                PreboxedConstants.Int32_000000F1,
                PreboxedConstants.Int32_000000F2,
                PreboxedConstants.Int32_000000F3,
                PreboxedConstants.Int32_000000F4,
                PreboxedConstants.Int32_000000F5,
                PreboxedConstants.Int32_000000F6,
                PreboxedConstants.Int32_000000F7,
                PreboxedConstants.Int32_000000F8,
                PreboxedConstants.Int32_000000F9,
                PreboxedConstants.Int32_000000FA,
                PreboxedConstants.Int32_000000FB,
                PreboxedConstants.Int32_000000FC,
                PreboxedConstants.Int32_000000FD,
                PreboxedConstants.Int32_000000FE,
                PreboxedConstants.Int32_000000FF        
            };
        }

        private static readonly Expression[] Int32_Expressions;

        private static Expression[] Get_Int32_Expressions()
        {
            return new Expression[]
            {
                PreboxedConstants.Int32_FFFFFF01_Expression,
                PreboxedConstants.Int32_FFFFFF02_Expression,
                PreboxedConstants.Int32_FFFFFF03_Expression,
                PreboxedConstants.Int32_FFFFFF04_Expression,
                PreboxedConstants.Int32_FFFFFF05_Expression,
                PreboxedConstants.Int32_FFFFFF06_Expression,
                PreboxedConstants.Int32_FFFFFF07_Expression,
                PreboxedConstants.Int32_FFFFFF08_Expression,
                PreboxedConstants.Int32_FFFFFF09_Expression,
                PreboxedConstants.Int32_FFFFFF0A_Expression,
                PreboxedConstants.Int32_FFFFFF0B_Expression,
                PreboxedConstants.Int32_FFFFFF0C_Expression,
                PreboxedConstants.Int32_FFFFFF0D_Expression,
                PreboxedConstants.Int32_FFFFFF0E_Expression,
                PreboxedConstants.Int32_FFFFFF0F_Expression,
                PreboxedConstants.Int32_FFFFFF10_Expression,
                PreboxedConstants.Int32_FFFFFF11_Expression,
                PreboxedConstants.Int32_FFFFFF12_Expression,
                PreboxedConstants.Int32_FFFFFF13_Expression,
                PreboxedConstants.Int32_FFFFFF14_Expression,
                PreboxedConstants.Int32_FFFFFF15_Expression,
                PreboxedConstants.Int32_FFFFFF16_Expression,
                PreboxedConstants.Int32_FFFFFF17_Expression,
                PreboxedConstants.Int32_FFFFFF18_Expression,
                PreboxedConstants.Int32_FFFFFF19_Expression,
                PreboxedConstants.Int32_FFFFFF1A_Expression,
                PreboxedConstants.Int32_FFFFFF1B_Expression,
                PreboxedConstants.Int32_FFFFFF1C_Expression,
                PreboxedConstants.Int32_FFFFFF1D_Expression,
                PreboxedConstants.Int32_FFFFFF1E_Expression,
                PreboxedConstants.Int32_FFFFFF1F_Expression,
                PreboxedConstants.Int32_FFFFFF20_Expression,
                PreboxedConstants.Int32_FFFFFF21_Expression,
                PreboxedConstants.Int32_FFFFFF22_Expression,
                PreboxedConstants.Int32_FFFFFF23_Expression,
                PreboxedConstants.Int32_FFFFFF24_Expression,
                PreboxedConstants.Int32_FFFFFF25_Expression,
                PreboxedConstants.Int32_FFFFFF26_Expression,
                PreboxedConstants.Int32_FFFFFF27_Expression,
                PreboxedConstants.Int32_FFFFFF28_Expression,
                PreboxedConstants.Int32_FFFFFF29_Expression,
                PreboxedConstants.Int32_FFFFFF2A_Expression,
                PreboxedConstants.Int32_FFFFFF2B_Expression,
                PreboxedConstants.Int32_FFFFFF2C_Expression,
                PreboxedConstants.Int32_FFFFFF2D_Expression,
                PreboxedConstants.Int32_FFFFFF2E_Expression,
                PreboxedConstants.Int32_FFFFFF2F_Expression,
                PreboxedConstants.Int32_FFFFFF30_Expression,
                PreboxedConstants.Int32_FFFFFF31_Expression,
                PreboxedConstants.Int32_FFFFFF32_Expression,
                PreboxedConstants.Int32_FFFFFF33_Expression,
                PreboxedConstants.Int32_FFFFFF34_Expression,
                PreboxedConstants.Int32_FFFFFF35_Expression,
                PreboxedConstants.Int32_FFFFFF36_Expression,
                PreboxedConstants.Int32_FFFFFF37_Expression,
                PreboxedConstants.Int32_FFFFFF38_Expression,
                PreboxedConstants.Int32_FFFFFF39_Expression,
                PreboxedConstants.Int32_FFFFFF3A_Expression,
                PreboxedConstants.Int32_FFFFFF3B_Expression,
                PreboxedConstants.Int32_FFFFFF3C_Expression,
                PreboxedConstants.Int32_FFFFFF3D_Expression,
                PreboxedConstants.Int32_FFFFFF3E_Expression,
                PreboxedConstants.Int32_FFFFFF3F_Expression,
                PreboxedConstants.Int32_FFFFFF40_Expression,
                PreboxedConstants.Int32_FFFFFF41_Expression,
                PreboxedConstants.Int32_FFFFFF42_Expression,
                PreboxedConstants.Int32_FFFFFF43_Expression,
                PreboxedConstants.Int32_FFFFFF44_Expression,
                PreboxedConstants.Int32_FFFFFF45_Expression,
                PreboxedConstants.Int32_FFFFFF46_Expression,
                PreboxedConstants.Int32_FFFFFF47_Expression,
                PreboxedConstants.Int32_FFFFFF48_Expression,
                PreboxedConstants.Int32_FFFFFF49_Expression,
                PreboxedConstants.Int32_FFFFFF4A_Expression,
                PreboxedConstants.Int32_FFFFFF4B_Expression,
                PreboxedConstants.Int32_FFFFFF4C_Expression,
                PreboxedConstants.Int32_FFFFFF4D_Expression,
                PreboxedConstants.Int32_FFFFFF4E_Expression,
                PreboxedConstants.Int32_FFFFFF4F_Expression,
                PreboxedConstants.Int32_FFFFFF50_Expression,
                PreboxedConstants.Int32_FFFFFF51_Expression,
                PreboxedConstants.Int32_FFFFFF52_Expression,
                PreboxedConstants.Int32_FFFFFF53_Expression,
                PreboxedConstants.Int32_FFFFFF54_Expression,
                PreboxedConstants.Int32_FFFFFF55_Expression,
                PreboxedConstants.Int32_FFFFFF56_Expression,
                PreboxedConstants.Int32_FFFFFF57_Expression,
                PreboxedConstants.Int32_FFFFFF58_Expression,
                PreboxedConstants.Int32_FFFFFF59_Expression,
                PreboxedConstants.Int32_FFFFFF5A_Expression,
                PreboxedConstants.Int32_FFFFFF5B_Expression,
                PreboxedConstants.Int32_FFFFFF5C_Expression,
                PreboxedConstants.Int32_FFFFFF5D_Expression,
                PreboxedConstants.Int32_FFFFFF5E_Expression,
                PreboxedConstants.Int32_FFFFFF5F_Expression,
                PreboxedConstants.Int32_FFFFFF60_Expression,
                PreboxedConstants.Int32_FFFFFF61_Expression,
                PreboxedConstants.Int32_FFFFFF62_Expression,
                PreboxedConstants.Int32_FFFFFF63_Expression,
                PreboxedConstants.Int32_FFFFFF64_Expression,
                PreboxedConstants.Int32_FFFFFF65_Expression,
                PreboxedConstants.Int32_FFFFFF66_Expression,
                PreboxedConstants.Int32_FFFFFF67_Expression,
                PreboxedConstants.Int32_FFFFFF68_Expression,
                PreboxedConstants.Int32_FFFFFF69_Expression,
                PreboxedConstants.Int32_FFFFFF6A_Expression,
                PreboxedConstants.Int32_FFFFFF6B_Expression,
                PreboxedConstants.Int32_FFFFFF6C_Expression,
                PreboxedConstants.Int32_FFFFFF6D_Expression,
                PreboxedConstants.Int32_FFFFFF6E_Expression,
                PreboxedConstants.Int32_FFFFFF6F_Expression,
                PreboxedConstants.Int32_FFFFFF70_Expression,
                PreboxedConstants.Int32_FFFFFF71_Expression,
                PreboxedConstants.Int32_FFFFFF72_Expression,
                PreboxedConstants.Int32_FFFFFF73_Expression,
                PreboxedConstants.Int32_FFFFFF74_Expression,
                PreboxedConstants.Int32_FFFFFF75_Expression,
                PreboxedConstants.Int32_FFFFFF76_Expression,
                PreboxedConstants.Int32_FFFFFF77_Expression,
                PreboxedConstants.Int32_FFFFFF78_Expression,
                PreboxedConstants.Int32_FFFFFF79_Expression,
                PreboxedConstants.Int32_FFFFFF7A_Expression,
                PreboxedConstants.Int32_FFFFFF7B_Expression,
                PreboxedConstants.Int32_FFFFFF7C_Expression,
                PreboxedConstants.Int32_FFFFFF7D_Expression,
                PreboxedConstants.Int32_FFFFFF7E_Expression,
                PreboxedConstants.Int32_FFFFFF7F_Expression,
                PreboxedConstants.Int32_FFFFFF80_Expression,
                PreboxedConstants.Int32_FFFFFF81_Expression,
                PreboxedConstants.Int32_FFFFFF82_Expression,
                PreboxedConstants.Int32_FFFFFF83_Expression,
                PreboxedConstants.Int32_FFFFFF84_Expression,
                PreboxedConstants.Int32_FFFFFF85_Expression,
                PreboxedConstants.Int32_FFFFFF86_Expression,
                PreboxedConstants.Int32_FFFFFF87_Expression,
                PreboxedConstants.Int32_FFFFFF88_Expression,
                PreboxedConstants.Int32_FFFFFF89_Expression,
                PreboxedConstants.Int32_FFFFFF8A_Expression,
                PreboxedConstants.Int32_FFFFFF8B_Expression,
                PreboxedConstants.Int32_FFFFFF8C_Expression,
                PreboxedConstants.Int32_FFFFFF8D_Expression,
                PreboxedConstants.Int32_FFFFFF8E_Expression,
                PreboxedConstants.Int32_FFFFFF8F_Expression,
                PreboxedConstants.Int32_FFFFFF90_Expression,
                PreboxedConstants.Int32_FFFFFF91_Expression,
                PreboxedConstants.Int32_FFFFFF92_Expression,
                PreboxedConstants.Int32_FFFFFF93_Expression,
                PreboxedConstants.Int32_FFFFFF94_Expression,
                PreboxedConstants.Int32_FFFFFF95_Expression,
                PreboxedConstants.Int32_FFFFFF96_Expression,
                PreboxedConstants.Int32_FFFFFF97_Expression,
                PreboxedConstants.Int32_FFFFFF98_Expression,
                PreboxedConstants.Int32_FFFFFF99_Expression,
                PreboxedConstants.Int32_FFFFFF9A_Expression,
                PreboxedConstants.Int32_FFFFFF9B_Expression,
                PreboxedConstants.Int32_FFFFFF9C_Expression,
                PreboxedConstants.Int32_FFFFFF9D_Expression,
                PreboxedConstants.Int32_FFFFFF9E_Expression,
                PreboxedConstants.Int32_FFFFFF9F_Expression,
                PreboxedConstants.Int32_FFFFFFA0_Expression,
                PreboxedConstants.Int32_FFFFFFA1_Expression,
                PreboxedConstants.Int32_FFFFFFA2_Expression,
                PreboxedConstants.Int32_FFFFFFA3_Expression,
                PreboxedConstants.Int32_FFFFFFA4_Expression,
                PreboxedConstants.Int32_FFFFFFA5_Expression,
                PreboxedConstants.Int32_FFFFFFA6_Expression,
                PreboxedConstants.Int32_FFFFFFA7_Expression,
                PreboxedConstants.Int32_FFFFFFA8_Expression,
                PreboxedConstants.Int32_FFFFFFA9_Expression,
                PreboxedConstants.Int32_FFFFFFAA_Expression,
                PreboxedConstants.Int32_FFFFFFAB_Expression,
                PreboxedConstants.Int32_FFFFFFAC_Expression,
                PreboxedConstants.Int32_FFFFFFAD_Expression,
                PreboxedConstants.Int32_FFFFFFAE_Expression,
                PreboxedConstants.Int32_FFFFFFAF_Expression,
                PreboxedConstants.Int32_FFFFFFB0_Expression,
                PreboxedConstants.Int32_FFFFFFB1_Expression,
                PreboxedConstants.Int32_FFFFFFB2_Expression,
                PreboxedConstants.Int32_FFFFFFB3_Expression,
                PreboxedConstants.Int32_FFFFFFB4_Expression,
                PreboxedConstants.Int32_FFFFFFB5_Expression,
                PreboxedConstants.Int32_FFFFFFB6_Expression,
                PreboxedConstants.Int32_FFFFFFB7_Expression,
                PreboxedConstants.Int32_FFFFFFB8_Expression,
                PreboxedConstants.Int32_FFFFFFB9_Expression,
                PreboxedConstants.Int32_FFFFFFBA_Expression,
                PreboxedConstants.Int32_FFFFFFBB_Expression,
                PreboxedConstants.Int32_FFFFFFBC_Expression,
                PreboxedConstants.Int32_FFFFFFBD_Expression,
                PreboxedConstants.Int32_FFFFFFBE_Expression,
                PreboxedConstants.Int32_FFFFFFBF_Expression,
                PreboxedConstants.Int32_FFFFFFC0_Expression,
                PreboxedConstants.Int32_FFFFFFC1_Expression,
                PreboxedConstants.Int32_FFFFFFC2_Expression,
                PreboxedConstants.Int32_FFFFFFC3_Expression,
                PreboxedConstants.Int32_FFFFFFC4_Expression,
                PreboxedConstants.Int32_FFFFFFC5_Expression,
                PreboxedConstants.Int32_FFFFFFC6_Expression,
                PreboxedConstants.Int32_FFFFFFC7_Expression,
                PreboxedConstants.Int32_FFFFFFC8_Expression,
                PreboxedConstants.Int32_FFFFFFC9_Expression,
                PreboxedConstants.Int32_FFFFFFCA_Expression,
                PreboxedConstants.Int32_FFFFFFCB_Expression,
                PreboxedConstants.Int32_FFFFFFCC_Expression,
                PreboxedConstants.Int32_FFFFFFCD_Expression,
                PreboxedConstants.Int32_FFFFFFCE_Expression,
                PreboxedConstants.Int32_FFFFFFCF_Expression,
                PreboxedConstants.Int32_FFFFFFD0_Expression,
                PreboxedConstants.Int32_FFFFFFD1_Expression,
                PreboxedConstants.Int32_FFFFFFD2_Expression,
                PreboxedConstants.Int32_FFFFFFD3_Expression,
                PreboxedConstants.Int32_FFFFFFD4_Expression,
                PreboxedConstants.Int32_FFFFFFD5_Expression,
                PreboxedConstants.Int32_FFFFFFD6_Expression,
                PreboxedConstants.Int32_FFFFFFD7_Expression,
                PreboxedConstants.Int32_FFFFFFD8_Expression,
                PreboxedConstants.Int32_FFFFFFD9_Expression,
                PreboxedConstants.Int32_FFFFFFDA_Expression,
                PreboxedConstants.Int32_FFFFFFDB_Expression,
                PreboxedConstants.Int32_FFFFFFDC_Expression,
                PreboxedConstants.Int32_FFFFFFDD_Expression,
                PreboxedConstants.Int32_FFFFFFDE_Expression,
                PreboxedConstants.Int32_FFFFFFDF_Expression,
                PreboxedConstants.Int32_FFFFFFE0_Expression,
                PreboxedConstants.Int32_FFFFFFE1_Expression,
                PreboxedConstants.Int32_FFFFFFE2_Expression,
                PreboxedConstants.Int32_FFFFFFE3_Expression,
                PreboxedConstants.Int32_FFFFFFE4_Expression,
                PreboxedConstants.Int32_FFFFFFE5_Expression,
                PreboxedConstants.Int32_FFFFFFE6_Expression,
                PreboxedConstants.Int32_FFFFFFE7_Expression,
                PreboxedConstants.Int32_FFFFFFE8_Expression,
                PreboxedConstants.Int32_FFFFFFE9_Expression,
                PreboxedConstants.Int32_FFFFFFEA_Expression,
                PreboxedConstants.Int32_FFFFFFEB_Expression,
                PreboxedConstants.Int32_FFFFFFEC_Expression,
                PreboxedConstants.Int32_FFFFFFED_Expression,
                PreboxedConstants.Int32_FFFFFFEE_Expression,
                PreboxedConstants.Int32_FFFFFFEF_Expression,
                PreboxedConstants.Int32_FFFFFFF0_Expression,
                PreboxedConstants.Int32_FFFFFFF1_Expression,
                PreboxedConstants.Int32_FFFFFFF2_Expression,
                PreboxedConstants.Int32_FFFFFFF3_Expression,
                PreboxedConstants.Int32_FFFFFFF4_Expression,
                PreboxedConstants.Int32_FFFFFFF5_Expression,
                PreboxedConstants.Int32_FFFFFFF6_Expression,
                PreboxedConstants.Int32_FFFFFFF7_Expression,
                PreboxedConstants.Int32_FFFFFFF8_Expression,
                PreboxedConstants.Int32_FFFFFFF9_Expression,
                PreboxedConstants.Int32_FFFFFFFA_Expression,
                PreboxedConstants.Int32_FFFFFFFB_Expression,
                PreboxedConstants.Int32_FFFFFFFC_Expression,
                PreboxedConstants.Int32_FFFFFFFD_Expression,
                PreboxedConstants.Int32_FFFFFFFE_Expression,
                PreboxedConstants.Int32_FFFFFFFF_Expression,
                PreboxedConstants.Int32_00000000_Expression,
                PreboxedConstants.Int32_00000001_Expression,
                PreboxedConstants.Int32_00000002_Expression,
                PreboxedConstants.Int32_00000003_Expression,
                PreboxedConstants.Int32_00000004_Expression,
                PreboxedConstants.Int32_00000005_Expression,
                PreboxedConstants.Int32_00000006_Expression,
                PreboxedConstants.Int32_00000007_Expression,
                PreboxedConstants.Int32_00000008_Expression,
                PreboxedConstants.Int32_00000009_Expression,
                PreboxedConstants.Int32_0000000A_Expression,
                PreboxedConstants.Int32_0000000B_Expression,
                PreboxedConstants.Int32_0000000C_Expression,
                PreboxedConstants.Int32_0000000D_Expression,
                PreboxedConstants.Int32_0000000E_Expression,
                PreboxedConstants.Int32_0000000F_Expression,
                PreboxedConstants.Int32_00000010_Expression,
                PreboxedConstants.Int32_00000011_Expression,
                PreboxedConstants.Int32_00000012_Expression,
                PreboxedConstants.Int32_00000013_Expression,
                PreboxedConstants.Int32_00000014_Expression,
                PreboxedConstants.Int32_00000015_Expression,
                PreboxedConstants.Int32_00000016_Expression,
                PreboxedConstants.Int32_00000017_Expression,
                PreboxedConstants.Int32_00000018_Expression,
                PreboxedConstants.Int32_00000019_Expression,
                PreboxedConstants.Int32_0000001A_Expression,
                PreboxedConstants.Int32_0000001B_Expression,
                PreboxedConstants.Int32_0000001C_Expression,
                PreboxedConstants.Int32_0000001D_Expression,
                PreboxedConstants.Int32_0000001E_Expression,
                PreboxedConstants.Int32_0000001F_Expression,
                PreboxedConstants.Int32_00000020_Expression,
                PreboxedConstants.Int32_00000021_Expression,
                PreboxedConstants.Int32_00000022_Expression,
                PreboxedConstants.Int32_00000023_Expression,
                PreboxedConstants.Int32_00000024_Expression,
                PreboxedConstants.Int32_00000025_Expression,
                PreboxedConstants.Int32_00000026_Expression,
                PreboxedConstants.Int32_00000027_Expression,
                PreboxedConstants.Int32_00000028_Expression,
                PreboxedConstants.Int32_00000029_Expression,
                PreboxedConstants.Int32_0000002A_Expression,
                PreboxedConstants.Int32_0000002B_Expression,
                PreboxedConstants.Int32_0000002C_Expression,
                PreboxedConstants.Int32_0000002D_Expression,
                PreboxedConstants.Int32_0000002E_Expression,
                PreboxedConstants.Int32_0000002F_Expression,
                PreboxedConstants.Int32_00000030_Expression,
                PreboxedConstants.Int32_00000031_Expression,
                PreboxedConstants.Int32_00000032_Expression,
                PreboxedConstants.Int32_00000033_Expression,
                PreboxedConstants.Int32_00000034_Expression,
                PreboxedConstants.Int32_00000035_Expression,
                PreboxedConstants.Int32_00000036_Expression,
                PreboxedConstants.Int32_00000037_Expression,
                PreboxedConstants.Int32_00000038_Expression,
                PreboxedConstants.Int32_00000039_Expression,
                PreboxedConstants.Int32_0000003A_Expression,
                PreboxedConstants.Int32_0000003B_Expression,
                PreboxedConstants.Int32_0000003C_Expression,
                PreboxedConstants.Int32_0000003D_Expression,
                PreboxedConstants.Int32_0000003E_Expression,
                PreboxedConstants.Int32_0000003F_Expression,
                PreboxedConstants.Int32_00000040_Expression,
                PreboxedConstants.Int32_00000041_Expression,
                PreboxedConstants.Int32_00000042_Expression,
                PreboxedConstants.Int32_00000043_Expression,
                PreboxedConstants.Int32_00000044_Expression,
                PreboxedConstants.Int32_00000045_Expression,
                PreboxedConstants.Int32_00000046_Expression,
                PreboxedConstants.Int32_00000047_Expression,
                PreboxedConstants.Int32_00000048_Expression,
                PreboxedConstants.Int32_00000049_Expression,
                PreboxedConstants.Int32_0000004A_Expression,
                PreboxedConstants.Int32_0000004B_Expression,
                PreboxedConstants.Int32_0000004C_Expression,
                PreboxedConstants.Int32_0000004D_Expression,
                PreboxedConstants.Int32_0000004E_Expression,
                PreboxedConstants.Int32_0000004F_Expression,
                PreboxedConstants.Int32_00000050_Expression,
                PreboxedConstants.Int32_00000051_Expression,
                PreboxedConstants.Int32_00000052_Expression,
                PreboxedConstants.Int32_00000053_Expression,
                PreboxedConstants.Int32_00000054_Expression,
                PreboxedConstants.Int32_00000055_Expression,
                PreboxedConstants.Int32_00000056_Expression,
                PreboxedConstants.Int32_00000057_Expression,
                PreboxedConstants.Int32_00000058_Expression,
                PreboxedConstants.Int32_00000059_Expression,
                PreboxedConstants.Int32_0000005A_Expression,
                PreboxedConstants.Int32_0000005B_Expression,
                PreboxedConstants.Int32_0000005C_Expression,
                PreboxedConstants.Int32_0000005D_Expression,
                PreboxedConstants.Int32_0000005E_Expression,
                PreboxedConstants.Int32_0000005F_Expression,
                PreboxedConstants.Int32_00000060_Expression,
                PreboxedConstants.Int32_00000061_Expression,
                PreboxedConstants.Int32_00000062_Expression,
                PreboxedConstants.Int32_00000063_Expression,
                PreboxedConstants.Int32_00000064_Expression,
                PreboxedConstants.Int32_00000065_Expression,
                PreboxedConstants.Int32_00000066_Expression,
                PreboxedConstants.Int32_00000067_Expression,
                PreboxedConstants.Int32_00000068_Expression,
                PreboxedConstants.Int32_00000069_Expression,
                PreboxedConstants.Int32_0000006A_Expression,
                PreboxedConstants.Int32_0000006B_Expression,
                PreboxedConstants.Int32_0000006C_Expression,
                PreboxedConstants.Int32_0000006D_Expression,
                PreboxedConstants.Int32_0000006E_Expression,
                PreboxedConstants.Int32_0000006F_Expression,
                PreboxedConstants.Int32_00000070_Expression,
                PreboxedConstants.Int32_00000071_Expression,
                PreboxedConstants.Int32_00000072_Expression,
                PreboxedConstants.Int32_00000073_Expression,
                PreboxedConstants.Int32_00000074_Expression,
                PreboxedConstants.Int32_00000075_Expression,
                PreboxedConstants.Int32_00000076_Expression,
                PreboxedConstants.Int32_00000077_Expression,
                PreboxedConstants.Int32_00000078_Expression,
                PreboxedConstants.Int32_00000079_Expression,
                PreboxedConstants.Int32_0000007A_Expression,
                PreboxedConstants.Int32_0000007B_Expression,
                PreboxedConstants.Int32_0000007C_Expression,
                PreboxedConstants.Int32_0000007D_Expression,
                PreboxedConstants.Int32_0000007E_Expression,
                PreboxedConstants.Int32_0000007F_Expression,
                PreboxedConstants.Int32_00000080_Expression,
                PreboxedConstants.Int32_00000081_Expression,
                PreboxedConstants.Int32_00000082_Expression,
                PreboxedConstants.Int32_00000083_Expression,
                PreboxedConstants.Int32_00000084_Expression,
                PreboxedConstants.Int32_00000085_Expression,
                PreboxedConstants.Int32_00000086_Expression,
                PreboxedConstants.Int32_00000087_Expression,
                PreboxedConstants.Int32_00000088_Expression,
                PreboxedConstants.Int32_00000089_Expression,
                PreboxedConstants.Int32_0000008A_Expression,
                PreboxedConstants.Int32_0000008B_Expression,
                PreboxedConstants.Int32_0000008C_Expression,
                PreboxedConstants.Int32_0000008D_Expression,
                PreboxedConstants.Int32_0000008E_Expression,
                PreboxedConstants.Int32_0000008F_Expression,
                PreboxedConstants.Int32_00000090_Expression,
                PreboxedConstants.Int32_00000091_Expression,
                PreboxedConstants.Int32_00000092_Expression,
                PreboxedConstants.Int32_00000093_Expression,
                PreboxedConstants.Int32_00000094_Expression,
                PreboxedConstants.Int32_00000095_Expression,
                PreboxedConstants.Int32_00000096_Expression,
                PreboxedConstants.Int32_00000097_Expression,
                PreboxedConstants.Int32_00000098_Expression,
                PreboxedConstants.Int32_00000099_Expression,
                PreboxedConstants.Int32_0000009A_Expression,
                PreboxedConstants.Int32_0000009B_Expression,
                PreboxedConstants.Int32_0000009C_Expression,
                PreboxedConstants.Int32_0000009D_Expression,
                PreboxedConstants.Int32_0000009E_Expression,
                PreboxedConstants.Int32_0000009F_Expression,
                PreboxedConstants.Int32_000000A0_Expression,
                PreboxedConstants.Int32_000000A1_Expression,
                PreboxedConstants.Int32_000000A2_Expression,
                PreboxedConstants.Int32_000000A3_Expression,
                PreboxedConstants.Int32_000000A4_Expression,
                PreboxedConstants.Int32_000000A5_Expression,
                PreboxedConstants.Int32_000000A6_Expression,
                PreboxedConstants.Int32_000000A7_Expression,
                PreboxedConstants.Int32_000000A8_Expression,
                PreboxedConstants.Int32_000000A9_Expression,
                PreboxedConstants.Int32_000000AA_Expression,
                PreboxedConstants.Int32_000000AB_Expression,
                PreboxedConstants.Int32_000000AC_Expression,
                PreboxedConstants.Int32_000000AD_Expression,
                PreboxedConstants.Int32_000000AE_Expression,
                PreboxedConstants.Int32_000000AF_Expression,
                PreboxedConstants.Int32_000000B0_Expression,
                PreboxedConstants.Int32_000000B1_Expression,
                PreboxedConstants.Int32_000000B2_Expression,
                PreboxedConstants.Int32_000000B3_Expression,
                PreboxedConstants.Int32_000000B4_Expression,
                PreboxedConstants.Int32_000000B5_Expression,
                PreboxedConstants.Int32_000000B6_Expression,
                PreboxedConstants.Int32_000000B7_Expression,
                PreboxedConstants.Int32_000000B8_Expression,
                PreboxedConstants.Int32_000000B9_Expression,
                PreboxedConstants.Int32_000000BA_Expression,
                PreboxedConstants.Int32_000000BB_Expression,
                PreboxedConstants.Int32_000000BC_Expression,
                PreboxedConstants.Int32_000000BD_Expression,
                PreboxedConstants.Int32_000000BE_Expression,
                PreboxedConstants.Int32_000000BF_Expression,
                PreboxedConstants.Int32_000000C0_Expression,
                PreboxedConstants.Int32_000000C1_Expression,
                PreboxedConstants.Int32_000000C2_Expression,
                PreboxedConstants.Int32_000000C3_Expression,
                PreboxedConstants.Int32_000000C4_Expression,
                PreboxedConstants.Int32_000000C5_Expression,
                PreboxedConstants.Int32_000000C6_Expression,
                PreboxedConstants.Int32_000000C7_Expression,
                PreboxedConstants.Int32_000000C8_Expression,
                PreboxedConstants.Int32_000000C9_Expression,
                PreboxedConstants.Int32_000000CA_Expression,
                PreboxedConstants.Int32_000000CB_Expression,
                PreboxedConstants.Int32_000000CC_Expression,
                PreboxedConstants.Int32_000000CD_Expression,
                PreboxedConstants.Int32_000000CE_Expression,
                PreboxedConstants.Int32_000000CF_Expression,
                PreboxedConstants.Int32_000000D0_Expression,
                PreboxedConstants.Int32_000000D1_Expression,
                PreboxedConstants.Int32_000000D2_Expression,
                PreboxedConstants.Int32_000000D3_Expression,
                PreboxedConstants.Int32_000000D4_Expression,
                PreboxedConstants.Int32_000000D5_Expression,
                PreboxedConstants.Int32_000000D6_Expression,
                PreboxedConstants.Int32_000000D7_Expression,
                PreboxedConstants.Int32_000000D8_Expression,
                PreboxedConstants.Int32_000000D9_Expression,
                PreboxedConstants.Int32_000000DA_Expression,
                PreboxedConstants.Int32_000000DB_Expression,
                PreboxedConstants.Int32_000000DC_Expression,
                PreboxedConstants.Int32_000000DD_Expression,
                PreboxedConstants.Int32_000000DE_Expression,
                PreboxedConstants.Int32_000000DF_Expression,
                PreboxedConstants.Int32_000000E0_Expression,
                PreboxedConstants.Int32_000000E1_Expression,
                PreboxedConstants.Int32_000000E2_Expression,
                PreboxedConstants.Int32_000000E3_Expression,
                PreboxedConstants.Int32_000000E4_Expression,
                PreboxedConstants.Int32_000000E5_Expression,
                PreboxedConstants.Int32_000000E6_Expression,
                PreboxedConstants.Int32_000000E7_Expression,
                PreboxedConstants.Int32_000000E8_Expression,
                PreboxedConstants.Int32_000000E9_Expression,
                PreboxedConstants.Int32_000000EA_Expression,
                PreboxedConstants.Int32_000000EB_Expression,
                PreboxedConstants.Int32_000000EC_Expression,
                PreboxedConstants.Int32_000000ED_Expression,
                PreboxedConstants.Int32_000000EE_Expression,
                PreboxedConstants.Int32_000000EF_Expression,
                PreboxedConstants.Int32_000000F0_Expression,
                PreboxedConstants.Int32_000000F1_Expression,
                PreboxedConstants.Int32_000000F2_Expression,
                PreboxedConstants.Int32_000000F3_Expression,
                PreboxedConstants.Int32_000000F4_Expression,
                PreboxedConstants.Int32_000000F5_Expression,
                PreboxedConstants.Int32_000000F6_Expression,
                PreboxedConstants.Int32_000000F7_Expression,
                PreboxedConstants.Int32_000000F8_Expression,
                PreboxedConstants.Int32_000000F9_Expression,
                PreboxedConstants.Int32_000000FA_Expression,
                PreboxedConstants.Int32_000000FB_Expression,
                PreboxedConstants.Int32_000000FC_Expression,
                PreboxedConstants.Int32_000000FD_Expression,
                PreboxedConstants.Int32_000000FE_Expression,
                PreboxedConstants.Int32_000000FF_Expression      
            };
        }

        /// <summary>
        /// A singleton boxed 32-bin integer -255 (0xFFFFFF01).
        /// </summary>
        public static readonly object Int32_FFFFFF01 = -255;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -255 (0xFFFFFF01).
        /// </summary>
        public static readonly Expression Int32_FFFFFF01_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF01");

        /// <summary>
        /// A singleton boxed 32-bin integer -254 (0xFFFFFF02).
        /// </summary>
        public static readonly object Int32_FFFFFF02 = -254;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -254 (0xFFFFFF02).
        /// </summary>
        public static readonly Expression Int32_FFFFFF02_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF02");

        /// <summary>
        /// A singleton boxed 32-bin integer -253 (0xFFFFFF03).
        /// </summary>
        public static readonly object Int32_FFFFFF03 = -253;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -253 (0xFFFFFF03).
        /// </summary>
        public static readonly Expression Int32_FFFFFF03_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF03");

        /// <summary>
        /// A singleton boxed 32-bin integer -252 (0xFFFFFF04).
        /// </summary>
        public static readonly object Int32_FFFFFF04 = -252;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -252 (0xFFFFFF04).
        /// </summary>
        public static readonly Expression Int32_FFFFFF04_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF04");

        /// <summary>
        /// A singleton boxed 32-bin integer -251 (0xFFFFFF05).
        /// </summary>
        public static readonly object Int32_FFFFFF05 = -251;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -251 (0xFFFFFF05).
        /// </summary>
        public static readonly Expression Int32_FFFFFF05_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF05");

        /// <summary>
        /// A singleton boxed 32-bin integer -250 (0xFFFFFF06).
        /// </summary>
        public static readonly object Int32_FFFFFF06 = -250;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -250 (0xFFFFFF06).
        /// </summary>
        public static readonly Expression Int32_FFFFFF06_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF06");

        /// <summary>
        /// A singleton boxed 32-bin integer -249 (0xFFFFFF07).
        /// </summary>
        public static readonly object Int32_FFFFFF07 = -249;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -249 (0xFFFFFF07).
        /// </summary>
        public static readonly Expression Int32_FFFFFF07_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF07");

        /// <summary>
        /// A singleton boxed 32-bin integer -248 (0xFFFFFF08).
        /// </summary>
        public static readonly object Int32_FFFFFF08 = -248;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -248 (0xFFFFFF08).
        /// </summary>
        public static readonly Expression Int32_FFFFFF08_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF08");

        /// <summary>
        /// A singleton boxed 32-bin integer -247 (0xFFFFFF09).
        /// </summary>
        public static readonly object Int32_FFFFFF09 = -247;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -247 (0xFFFFFF09).
        /// </summary>
        public static readonly Expression Int32_FFFFFF09_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF09");

        /// <summary>
        /// A singleton boxed 32-bin integer -246 (0xFFFFFF0A).
        /// </summary>
        public static readonly object Int32_FFFFFF0A = -246;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -246 (0xFFFFFF0A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0A");

        /// <summary>
        /// A singleton boxed 32-bin integer -245 (0xFFFFFF0B).
        /// </summary>
        public static readonly object Int32_FFFFFF0B = -245;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -245 (0xFFFFFF0B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0B");

        /// <summary>
        /// A singleton boxed 32-bin integer -244 (0xFFFFFF0C).
        /// </summary>
        public static readonly object Int32_FFFFFF0C = -244;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -244 (0xFFFFFF0C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0C");

        /// <summary>
        /// A singleton boxed 32-bin integer -243 (0xFFFFFF0D).
        /// </summary>
        public static readonly object Int32_FFFFFF0D = -243;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -243 (0xFFFFFF0D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0D");

        /// <summary>
        /// A singleton boxed 32-bin integer -242 (0xFFFFFF0E).
        /// </summary>
        public static readonly object Int32_FFFFFF0E = -242;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -242 (0xFFFFFF0E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0E");

        /// <summary>
        /// A singleton boxed 32-bin integer -241 (0xFFFFFF0F).
        /// </summary>
        public static readonly object Int32_FFFFFF0F = -241;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -241 (0xFFFFFF0F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF0F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF0F");

        /// <summary>
        /// A singleton boxed 32-bin integer -240 (0xFFFFFF10).
        /// </summary>
        public static readonly object Int32_FFFFFF10 = -240;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -240 (0xFFFFFF10).
        /// </summary>
        public static readonly Expression Int32_FFFFFF10_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF10");

        /// <summary>
        /// A singleton boxed 32-bin integer -239 (0xFFFFFF11).
        /// </summary>
        public static readonly object Int32_FFFFFF11 = -239;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -239 (0xFFFFFF11).
        /// </summary>
        public static readonly Expression Int32_FFFFFF11_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF11");

        /// <summary>
        /// A singleton boxed 32-bin integer -238 (0xFFFFFF12).
        /// </summary>
        public static readonly object Int32_FFFFFF12 = -238;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -238 (0xFFFFFF12).
        /// </summary>
        public static readonly Expression Int32_FFFFFF12_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF12");

        /// <summary>
        /// A singleton boxed 32-bin integer -237 (0xFFFFFF13).
        /// </summary>
        public static readonly object Int32_FFFFFF13 = -237;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -237 (0xFFFFFF13).
        /// </summary>
        public static readonly Expression Int32_FFFFFF13_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF13");

        /// <summary>
        /// A singleton boxed 32-bin integer -236 (0xFFFFFF14).
        /// </summary>
        public static readonly object Int32_FFFFFF14 = -236;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -236 (0xFFFFFF14).
        /// </summary>
        public static readonly Expression Int32_FFFFFF14_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF14");

        /// <summary>
        /// A singleton boxed 32-bin integer -235 (0xFFFFFF15).
        /// </summary>
        public static readonly object Int32_FFFFFF15 = -235;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -235 (0xFFFFFF15).
        /// </summary>
        public static readonly Expression Int32_FFFFFF15_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF15");

        /// <summary>
        /// A singleton boxed 32-bin integer -234 (0xFFFFFF16).
        /// </summary>
        public static readonly object Int32_FFFFFF16 = -234;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -234 (0xFFFFFF16).
        /// </summary>
        public static readonly Expression Int32_FFFFFF16_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF16");

        /// <summary>
        /// A singleton boxed 32-bin integer -233 (0xFFFFFF17).
        /// </summary>
        public static readonly object Int32_FFFFFF17 = -233;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -233 (0xFFFFFF17).
        /// </summary>
        public static readonly Expression Int32_FFFFFF17_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF17");

        /// <summary>
        /// A singleton boxed 32-bin integer -232 (0xFFFFFF18).
        /// </summary>
        public static readonly object Int32_FFFFFF18 = -232;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -232 (0xFFFFFF18).
        /// </summary>
        public static readonly Expression Int32_FFFFFF18_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF18");

        /// <summary>
        /// A singleton boxed 32-bin integer -231 (0xFFFFFF19).
        /// </summary>
        public static readonly object Int32_FFFFFF19 = -231;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -231 (0xFFFFFF19).
        /// </summary>
        public static readonly Expression Int32_FFFFFF19_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF19");

        /// <summary>
        /// A singleton boxed 32-bin integer -230 (0xFFFFFF1A).
        /// </summary>
        public static readonly object Int32_FFFFFF1A = -230;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -230 (0xFFFFFF1A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1A");

        /// <summary>
        /// A singleton boxed 32-bin integer -229 (0xFFFFFF1B).
        /// </summary>
        public static readonly object Int32_FFFFFF1B = -229;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -229 (0xFFFFFF1B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1B");

        /// <summary>
        /// A singleton boxed 32-bin integer -228 (0xFFFFFF1C).
        /// </summary>
        public static readonly object Int32_FFFFFF1C = -228;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -228 (0xFFFFFF1C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1C");

        /// <summary>
        /// A singleton boxed 32-bin integer -227 (0xFFFFFF1D).
        /// </summary>
        public static readonly object Int32_FFFFFF1D = -227;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -227 (0xFFFFFF1D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1D");

        /// <summary>
        /// A singleton boxed 32-bin integer -226 (0xFFFFFF1E).
        /// </summary>
        public static readonly object Int32_FFFFFF1E = -226;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -226 (0xFFFFFF1E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1E");

        /// <summary>
        /// A singleton boxed 32-bin integer -225 (0xFFFFFF1F).
        /// </summary>
        public static readonly object Int32_FFFFFF1F = -225;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -225 (0xFFFFFF1F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF1F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF1F");

        /// <summary>
        /// A singleton boxed 32-bin integer -224 (0xFFFFFF20).
        /// </summary>
        public static readonly object Int32_FFFFFF20 = -224;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -224 (0xFFFFFF20).
        /// </summary>
        public static readonly Expression Int32_FFFFFF20_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF20");

        /// <summary>
        /// A singleton boxed 32-bin integer -223 (0xFFFFFF21).
        /// </summary>
        public static readonly object Int32_FFFFFF21 = -223;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -223 (0xFFFFFF21).
        /// </summary>
        public static readonly Expression Int32_FFFFFF21_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF21");

        /// <summary>
        /// A singleton boxed 32-bin integer -222 (0xFFFFFF22).
        /// </summary>
        public static readonly object Int32_FFFFFF22 = -222;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -222 (0xFFFFFF22).
        /// </summary>
        public static readonly Expression Int32_FFFFFF22_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF22");

        /// <summary>
        /// A singleton boxed 32-bin integer -221 (0xFFFFFF23).
        /// </summary>
        public static readonly object Int32_FFFFFF23 = -221;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -221 (0xFFFFFF23).
        /// </summary>
        public static readonly Expression Int32_FFFFFF23_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF23");

        /// <summary>
        /// A singleton boxed 32-bin integer -220 (0xFFFFFF24).
        /// </summary>
        public static readonly object Int32_FFFFFF24 = -220;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -220 (0xFFFFFF24).
        /// </summary>
        public static readonly Expression Int32_FFFFFF24_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF24");

        /// <summary>
        /// A singleton boxed 32-bin integer -219 (0xFFFFFF25).
        /// </summary>
        public static readonly object Int32_FFFFFF25 = -219;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -219 (0xFFFFFF25).
        /// </summary>
        public static readonly Expression Int32_FFFFFF25_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF25");

        /// <summary>
        /// A singleton boxed 32-bin integer -218 (0xFFFFFF26).
        /// </summary>
        public static readonly object Int32_FFFFFF26 = -218;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -218 (0xFFFFFF26).
        /// </summary>
        public static readonly Expression Int32_FFFFFF26_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF26");

        /// <summary>
        /// A singleton boxed 32-bin integer -217 (0xFFFFFF27).
        /// </summary>
        public static readonly object Int32_FFFFFF27 = -217;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -217 (0xFFFFFF27).
        /// </summary>
        public static readonly Expression Int32_FFFFFF27_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF27");

        /// <summary>
        /// A singleton boxed 32-bin integer -216 (0xFFFFFF28).
        /// </summary>
        public static readonly object Int32_FFFFFF28 = -216;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -216 (0xFFFFFF28).
        /// </summary>
        public static readonly Expression Int32_FFFFFF28_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF28");

        /// <summary>
        /// A singleton boxed 32-bin integer -215 (0xFFFFFF29).
        /// </summary>
        public static readonly object Int32_FFFFFF29 = -215;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -215 (0xFFFFFF29).
        /// </summary>
        public static readonly Expression Int32_FFFFFF29_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF29");

        /// <summary>
        /// A singleton boxed 32-bin integer -214 (0xFFFFFF2A).
        /// </summary>
        public static readonly object Int32_FFFFFF2A = -214;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -214 (0xFFFFFF2A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2A");

        /// <summary>
        /// A singleton boxed 32-bin integer -213 (0xFFFFFF2B).
        /// </summary>
        public static readonly object Int32_FFFFFF2B = -213;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -213 (0xFFFFFF2B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2B");

        /// <summary>
        /// A singleton boxed 32-bin integer -212 (0xFFFFFF2C).
        /// </summary>
        public static readonly object Int32_FFFFFF2C = -212;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -212 (0xFFFFFF2C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2C");

        /// <summary>
        /// A singleton boxed 32-bin integer -211 (0xFFFFFF2D).
        /// </summary>
        public static readonly object Int32_FFFFFF2D = -211;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -211 (0xFFFFFF2D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2D");

        /// <summary>
        /// A singleton boxed 32-bin integer -210 (0xFFFFFF2E).
        /// </summary>
        public static readonly object Int32_FFFFFF2E = -210;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -210 (0xFFFFFF2E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2E");

        /// <summary>
        /// A singleton boxed 32-bin integer -209 (0xFFFFFF2F).
        /// </summary>
        public static readonly object Int32_FFFFFF2F = -209;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -209 (0xFFFFFF2F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF2F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF2F");

        /// <summary>
        /// A singleton boxed 32-bin integer -208 (0xFFFFFF30).
        /// </summary>
        public static readonly object Int32_FFFFFF30 = -208;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -208 (0xFFFFFF30).
        /// </summary>
        public static readonly Expression Int32_FFFFFF30_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF30");

        /// <summary>
        /// A singleton boxed 32-bin integer -207 (0xFFFFFF31).
        /// </summary>
        public static readonly object Int32_FFFFFF31 = -207;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -207 (0xFFFFFF31).
        /// </summary>
        public static readonly Expression Int32_FFFFFF31_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF31");

        /// <summary>
        /// A singleton boxed 32-bin integer -206 (0xFFFFFF32).
        /// </summary>
        public static readonly object Int32_FFFFFF32 = -206;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -206 (0xFFFFFF32).
        /// </summary>
        public static readonly Expression Int32_FFFFFF32_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF32");

        /// <summary>
        /// A singleton boxed 32-bin integer -205 (0xFFFFFF33).
        /// </summary>
        public static readonly object Int32_FFFFFF33 = -205;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -205 (0xFFFFFF33).
        /// </summary>
        public static readonly Expression Int32_FFFFFF33_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF33");

        /// <summary>
        /// A singleton boxed 32-bin integer -204 (0xFFFFFF34).
        /// </summary>
        public static readonly object Int32_FFFFFF34 = -204;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -204 (0xFFFFFF34).
        /// </summary>
        public static readonly Expression Int32_FFFFFF34_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF34");

        /// <summary>
        /// A singleton boxed 32-bin integer -203 (0xFFFFFF35).
        /// </summary>
        public static readonly object Int32_FFFFFF35 = -203;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -203 (0xFFFFFF35).
        /// </summary>
        public static readonly Expression Int32_FFFFFF35_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF35");

        /// <summary>
        /// A singleton boxed 32-bin integer -202 (0xFFFFFF36).
        /// </summary>
        public static readonly object Int32_FFFFFF36 = -202;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -202 (0xFFFFFF36).
        /// </summary>
        public static readonly Expression Int32_FFFFFF36_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF36");

        /// <summary>
        /// A singleton boxed 32-bin integer -201 (0xFFFFFF37).
        /// </summary>
        public static readonly object Int32_FFFFFF37 = -201;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -201 (0xFFFFFF37).
        /// </summary>
        public static readonly Expression Int32_FFFFFF37_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF37");

        /// <summary>
        /// A singleton boxed 32-bin integer -200 (0xFFFFFF38).
        /// </summary>
        public static readonly object Int32_FFFFFF38 = -200;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -200 (0xFFFFFF38).
        /// </summary>
        public static readonly Expression Int32_FFFFFF38_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF38");

        /// <summary>
        /// A singleton boxed 32-bin integer -199 (0xFFFFFF39).
        /// </summary>
        public static readonly object Int32_FFFFFF39 = -199;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -199 (0xFFFFFF39).
        /// </summary>
        public static readonly Expression Int32_FFFFFF39_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF39");

        /// <summary>
        /// A singleton boxed 32-bin integer -198 (0xFFFFFF3A).
        /// </summary>
        public static readonly object Int32_FFFFFF3A = -198;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -198 (0xFFFFFF3A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3A");

        /// <summary>
        /// A singleton boxed 32-bin integer -197 (0xFFFFFF3B).
        /// </summary>
        public static readonly object Int32_FFFFFF3B = -197;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -197 (0xFFFFFF3B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3B");

        /// <summary>
        /// A singleton boxed 32-bin integer -196 (0xFFFFFF3C).
        /// </summary>
        public static readonly object Int32_FFFFFF3C = -196;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -196 (0xFFFFFF3C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3C");

        /// <summary>
        /// A singleton boxed 32-bin integer -195 (0xFFFFFF3D).
        /// </summary>
        public static readonly object Int32_FFFFFF3D = -195;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -195 (0xFFFFFF3D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3D");

        /// <summary>
        /// A singleton boxed 32-bin integer -194 (0xFFFFFF3E).
        /// </summary>
        public static readonly object Int32_FFFFFF3E = -194;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -194 (0xFFFFFF3E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3E");

        /// <summary>
        /// A singleton boxed 32-bin integer -193 (0xFFFFFF3F).
        /// </summary>
        public static readonly object Int32_FFFFFF3F = -193;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -193 (0xFFFFFF3F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF3F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF3F");

        /// <summary>
        /// A singleton boxed 32-bin integer -192 (0xFFFFFF40).
        /// </summary>
        public static readonly object Int32_FFFFFF40 = -192;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -192 (0xFFFFFF40).
        /// </summary>
        public static readonly Expression Int32_FFFFFF40_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF40");

        /// <summary>
        /// A singleton boxed 32-bin integer -191 (0xFFFFFF41).
        /// </summary>
        public static readonly object Int32_FFFFFF41 = -191;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -191 (0xFFFFFF41).
        /// </summary>
        public static readonly Expression Int32_FFFFFF41_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF41");

        /// <summary>
        /// A singleton boxed 32-bin integer -190 (0xFFFFFF42).
        /// </summary>
        public static readonly object Int32_FFFFFF42 = -190;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -190 (0xFFFFFF42).
        /// </summary>
        public static readonly Expression Int32_FFFFFF42_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF42");

        /// <summary>
        /// A singleton boxed 32-bin integer -189 (0xFFFFFF43).
        /// </summary>
        public static readonly object Int32_FFFFFF43 = -189;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -189 (0xFFFFFF43).
        /// </summary>
        public static readonly Expression Int32_FFFFFF43_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF43");

        /// <summary>
        /// A singleton boxed 32-bin integer -188 (0xFFFFFF44).
        /// </summary>
        public static readonly object Int32_FFFFFF44 = -188;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -188 (0xFFFFFF44).
        /// </summary>
        public static readonly Expression Int32_FFFFFF44_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF44");

        /// <summary>
        /// A singleton boxed 32-bin integer -187 (0xFFFFFF45).
        /// </summary>
        public static readonly object Int32_FFFFFF45 = -187;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -187 (0xFFFFFF45).
        /// </summary>
        public static readonly Expression Int32_FFFFFF45_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF45");

        /// <summary>
        /// A singleton boxed 32-bin integer -186 (0xFFFFFF46).
        /// </summary>
        public static readonly object Int32_FFFFFF46 = -186;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -186 (0xFFFFFF46).
        /// </summary>
        public static readonly Expression Int32_FFFFFF46_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF46");

        /// <summary>
        /// A singleton boxed 32-bin integer -185 (0xFFFFFF47).
        /// </summary>
        public static readonly object Int32_FFFFFF47 = -185;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -185 (0xFFFFFF47).
        /// </summary>
        public static readonly Expression Int32_FFFFFF47_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF47");

        /// <summary>
        /// A singleton boxed 32-bin integer -184 (0xFFFFFF48).
        /// </summary>
        public static readonly object Int32_FFFFFF48 = -184;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -184 (0xFFFFFF48).
        /// </summary>
        public static readonly Expression Int32_FFFFFF48_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF48");

        /// <summary>
        /// A singleton boxed 32-bin integer -183 (0xFFFFFF49).
        /// </summary>
        public static readonly object Int32_FFFFFF49 = -183;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -183 (0xFFFFFF49).
        /// </summary>
        public static readonly Expression Int32_FFFFFF49_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF49");

        /// <summary>
        /// A singleton boxed 32-bin integer -182 (0xFFFFFF4A).
        /// </summary>
        public static readonly object Int32_FFFFFF4A = -182;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -182 (0xFFFFFF4A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4A");

        /// <summary>
        /// A singleton boxed 32-bin integer -181 (0xFFFFFF4B).
        /// </summary>
        public static readonly object Int32_FFFFFF4B = -181;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -181 (0xFFFFFF4B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4B");

        /// <summary>
        /// A singleton boxed 32-bin integer -180 (0xFFFFFF4C).
        /// </summary>
        public static readonly object Int32_FFFFFF4C = -180;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -180 (0xFFFFFF4C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4C");

        /// <summary>
        /// A singleton boxed 32-bin integer -179 (0xFFFFFF4D).
        /// </summary>
        public static readonly object Int32_FFFFFF4D = -179;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -179 (0xFFFFFF4D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4D");

        /// <summary>
        /// A singleton boxed 32-bin integer -178 (0xFFFFFF4E).
        /// </summary>
        public static readonly object Int32_FFFFFF4E = -178;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -178 (0xFFFFFF4E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4E");

        /// <summary>
        /// A singleton boxed 32-bin integer -177 (0xFFFFFF4F).
        /// </summary>
        public static readonly object Int32_FFFFFF4F = -177;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -177 (0xFFFFFF4F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF4F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF4F");

        /// <summary>
        /// A singleton boxed 32-bin integer -176 (0xFFFFFF50).
        /// </summary>
        public static readonly object Int32_FFFFFF50 = -176;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -176 (0xFFFFFF50).
        /// </summary>
        public static readonly Expression Int32_FFFFFF50_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF50");

        /// <summary>
        /// A singleton boxed 32-bin integer -175 (0xFFFFFF51).
        /// </summary>
        public static readonly object Int32_FFFFFF51 = -175;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -175 (0xFFFFFF51).
        /// </summary>
        public static readonly Expression Int32_FFFFFF51_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF51");

        /// <summary>
        /// A singleton boxed 32-bin integer -174 (0xFFFFFF52).
        /// </summary>
        public static readonly object Int32_FFFFFF52 = -174;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -174 (0xFFFFFF52).
        /// </summary>
        public static readonly Expression Int32_FFFFFF52_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF52");

        /// <summary>
        /// A singleton boxed 32-bin integer -173 (0xFFFFFF53).
        /// </summary>
        public static readonly object Int32_FFFFFF53 = -173;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -173 (0xFFFFFF53).
        /// </summary>
        public static readonly Expression Int32_FFFFFF53_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF53");

        /// <summary>
        /// A singleton boxed 32-bin integer -172 (0xFFFFFF54).
        /// </summary>
        public static readonly object Int32_FFFFFF54 = -172;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -172 (0xFFFFFF54).
        /// </summary>
        public static readonly Expression Int32_FFFFFF54_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF54");

        /// <summary>
        /// A singleton boxed 32-bin integer -171 (0xFFFFFF55).
        /// </summary>
        public static readonly object Int32_FFFFFF55 = -171;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -171 (0xFFFFFF55).
        /// </summary>
        public static readonly Expression Int32_FFFFFF55_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF55");

        /// <summary>
        /// A singleton boxed 32-bin integer -170 (0xFFFFFF56).
        /// </summary>
        public static readonly object Int32_FFFFFF56 = -170;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -170 (0xFFFFFF56).
        /// </summary>
        public static readonly Expression Int32_FFFFFF56_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF56");

        /// <summary>
        /// A singleton boxed 32-bin integer -169 (0xFFFFFF57).
        /// </summary>
        public static readonly object Int32_FFFFFF57 = -169;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -169 (0xFFFFFF57).
        /// </summary>
        public static readonly Expression Int32_FFFFFF57_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF57");

        /// <summary>
        /// A singleton boxed 32-bin integer -168 (0xFFFFFF58).
        /// </summary>
        public static readonly object Int32_FFFFFF58 = -168;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -168 (0xFFFFFF58).
        /// </summary>
        public static readonly Expression Int32_FFFFFF58_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF58");

        /// <summary>
        /// A singleton boxed 32-bin integer -167 (0xFFFFFF59).
        /// </summary>
        public static readonly object Int32_FFFFFF59 = -167;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -167 (0xFFFFFF59).
        /// </summary>
        public static readonly Expression Int32_FFFFFF59_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF59");

        /// <summary>
        /// A singleton boxed 32-bin integer -166 (0xFFFFFF5A).
        /// </summary>
        public static readonly object Int32_FFFFFF5A = -166;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -166 (0xFFFFFF5A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5A");

        /// <summary>
        /// A singleton boxed 32-bin integer -165 (0xFFFFFF5B).
        /// </summary>
        public static readonly object Int32_FFFFFF5B = -165;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -165 (0xFFFFFF5B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5B");

        /// <summary>
        /// A singleton boxed 32-bin integer -164 (0xFFFFFF5C).
        /// </summary>
        public static readonly object Int32_FFFFFF5C = -164;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -164 (0xFFFFFF5C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5C");

        /// <summary>
        /// A singleton boxed 32-bin integer -163 (0xFFFFFF5D).
        /// </summary>
        public static readonly object Int32_FFFFFF5D = -163;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -163 (0xFFFFFF5D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5D");

        /// <summary>
        /// A singleton boxed 32-bin integer -162 (0xFFFFFF5E).
        /// </summary>
        public static readonly object Int32_FFFFFF5E = -162;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -162 (0xFFFFFF5E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5E");

        /// <summary>
        /// A singleton boxed 32-bin integer -161 (0xFFFFFF5F).
        /// </summary>
        public static readonly object Int32_FFFFFF5F = -161;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -161 (0xFFFFFF5F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF5F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF5F");

        /// <summary>
        /// A singleton boxed 32-bin integer -160 (0xFFFFFF60).
        /// </summary>
        public static readonly object Int32_FFFFFF60 = -160;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -160 (0xFFFFFF60).
        /// </summary>
        public static readonly Expression Int32_FFFFFF60_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF60");

        /// <summary>
        /// A singleton boxed 32-bin integer -159 (0xFFFFFF61).
        /// </summary>
        public static readonly object Int32_FFFFFF61 = -159;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -159 (0xFFFFFF61).
        /// </summary>
        public static readonly Expression Int32_FFFFFF61_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF61");

        /// <summary>
        /// A singleton boxed 32-bin integer -158 (0xFFFFFF62).
        /// </summary>
        public static readonly object Int32_FFFFFF62 = -158;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -158 (0xFFFFFF62).
        /// </summary>
        public static readonly Expression Int32_FFFFFF62_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF62");

        /// <summary>
        /// A singleton boxed 32-bin integer -157 (0xFFFFFF63).
        /// </summary>
        public static readonly object Int32_FFFFFF63 = -157;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -157 (0xFFFFFF63).
        /// </summary>
        public static readonly Expression Int32_FFFFFF63_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF63");

        /// <summary>
        /// A singleton boxed 32-bin integer -156 (0xFFFFFF64).
        /// </summary>
        public static readonly object Int32_FFFFFF64 = -156;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -156 (0xFFFFFF64).
        /// </summary>
        public static readonly Expression Int32_FFFFFF64_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF64");

        /// <summary>
        /// A singleton boxed 32-bin integer -155 (0xFFFFFF65).
        /// </summary>
        public static readonly object Int32_FFFFFF65 = -155;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -155 (0xFFFFFF65).
        /// </summary>
        public static readonly Expression Int32_FFFFFF65_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF65");

        /// <summary>
        /// A singleton boxed 32-bin integer -154 (0xFFFFFF66).
        /// </summary>
        public static readonly object Int32_FFFFFF66 = -154;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -154 (0xFFFFFF66).
        /// </summary>
        public static readonly Expression Int32_FFFFFF66_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF66");

        /// <summary>
        /// A singleton boxed 32-bin integer -153 (0xFFFFFF67).
        /// </summary>
        public static readonly object Int32_FFFFFF67 = -153;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -153 (0xFFFFFF67).
        /// </summary>
        public static readonly Expression Int32_FFFFFF67_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF67");

        /// <summary>
        /// A singleton boxed 32-bin integer -152 (0xFFFFFF68).
        /// </summary>
        public static readonly object Int32_FFFFFF68 = -152;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -152 (0xFFFFFF68).
        /// </summary>
        public static readonly Expression Int32_FFFFFF68_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF68");

        /// <summary>
        /// A singleton boxed 32-bin integer -151 (0xFFFFFF69).
        /// </summary>
        public static readonly object Int32_FFFFFF69 = -151;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -151 (0xFFFFFF69).
        /// </summary>
        public static readonly Expression Int32_FFFFFF69_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF69");

        /// <summary>
        /// A singleton boxed 32-bin integer -150 (0xFFFFFF6A).
        /// </summary>
        public static readonly object Int32_FFFFFF6A = -150;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -150 (0xFFFFFF6A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6A");

        /// <summary>
        /// A singleton boxed 32-bin integer -149 (0xFFFFFF6B).
        /// </summary>
        public static readonly object Int32_FFFFFF6B = -149;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -149 (0xFFFFFF6B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6B");

        /// <summary>
        /// A singleton boxed 32-bin integer -148 (0xFFFFFF6C).
        /// </summary>
        public static readonly object Int32_FFFFFF6C = -148;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -148 (0xFFFFFF6C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6C");

        /// <summary>
        /// A singleton boxed 32-bin integer -147 (0xFFFFFF6D).
        /// </summary>
        public static readonly object Int32_FFFFFF6D = -147;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -147 (0xFFFFFF6D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6D");

        /// <summary>
        /// A singleton boxed 32-bin integer -146 (0xFFFFFF6E).
        /// </summary>
        public static readonly object Int32_FFFFFF6E = -146;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -146 (0xFFFFFF6E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6E");

        /// <summary>
        /// A singleton boxed 32-bin integer -145 (0xFFFFFF6F).
        /// </summary>
        public static readonly object Int32_FFFFFF6F = -145;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -145 (0xFFFFFF6F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF6F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF6F");

        /// <summary>
        /// A singleton boxed 32-bin integer -144 (0xFFFFFF70).
        /// </summary>
        public static readonly object Int32_FFFFFF70 = -144;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -144 (0xFFFFFF70).
        /// </summary>
        public static readonly Expression Int32_FFFFFF70_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF70");

        /// <summary>
        /// A singleton boxed 32-bin integer -143 (0xFFFFFF71).
        /// </summary>
        public static readonly object Int32_FFFFFF71 = -143;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -143 (0xFFFFFF71).
        /// </summary>
        public static readonly Expression Int32_FFFFFF71_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF71");

        /// <summary>
        /// A singleton boxed 32-bin integer -142 (0xFFFFFF72).
        /// </summary>
        public static readonly object Int32_FFFFFF72 = -142;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -142 (0xFFFFFF72).
        /// </summary>
        public static readonly Expression Int32_FFFFFF72_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF72");

        /// <summary>
        /// A singleton boxed 32-bin integer -141 (0xFFFFFF73).
        /// </summary>
        public static readonly object Int32_FFFFFF73 = -141;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -141 (0xFFFFFF73).
        /// </summary>
        public static readonly Expression Int32_FFFFFF73_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF73");

        /// <summary>
        /// A singleton boxed 32-bin integer -140 (0xFFFFFF74).
        /// </summary>
        public static readonly object Int32_FFFFFF74 = -140;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -140 (0xFFFFFF74).
        /// </summary>
        public static readonly Expression Int32_FFFFFF74_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF74");

        /// <summary>
        /// A singleton boxed 32-bin integer -139 (0xFFFFFF75).
        /// </summary>
        public static readonly object Int32_FFFFFF75 = -139;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -139 (0xFFFFFF75).
        /// </summary>
        public static readonly Expression Int32_FFFFFF75_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF75");

        /// <summary>
        /// A singleton boxed 32-bin integer -138 (0xFFFFFF76).
        /// </summary>
        public static readonly object Int32_FFFFFF76 = -138;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -138 (0xFFFFFF76).
        /// </summary>
        public static readonly Expression Int32_FFFFFF76_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF76");

        /// <summary>
        /// A singleton boxed 32-bin integer -137 (0xFFFFFF77).
        /// </summary>
        public static readonly object Int32_FFFFFF77 = -137;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -137 (0xFFFFFF77).
        /// </summary>
        public static readonly Expression Int32_FFFFFF77_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF77");

        /// <summary>
        /// A singleton boxed 32-bin integer -136 (0xFFFFFF78).
        /// </summary>
        public static readonly object Int32_FFFFFF78 = -136;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -136 (0xFFFFFF78).
        /// </summary>
        public static readonly Expression Int32_FFFFFF78_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF78");

        /// <summary>
        /// A singleton boxed 32-bin integer -135 (0xFFFFFF79).
        /// </summary>
        public static readonly object Int32_FFFFFF79 = -135;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -135 (0xFFFFFF79).
        /// </summary>
        public static readonly Expression Int32_FFFFFF79_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF79");

        /// <summary>
        /// A singleton boxed 32-bin integer -134 (0xFFFFFF7A).
        /// </summary>
        public static readonly object Int32_FFFFFF7A = -134;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -134 (0xFFFFFF7A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7A");

        /// <summary>
        /// A singleton boxed 32-bin integer -133 (0xFFFFFF7B).
        /// </summary>
        public static readonly object Int32_FFFFFF7B = -133;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -133 (0xFFFFFF7B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7B");

        /// <summary>
        /// A singleton boxed 32-bin integer -132 (0xFFFFFF7C).
        /// </summary>
        public static readonly object Int32_FFFFFF7C = -132;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -132 (0xFFFFFF7C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7C");

        /// <summary>
        /// A singleton boxed 32-bin integer -131 (0xFFFFFF7D).
        /// </summary>
        public static readonly object Int32_FFFFFF7D = -131;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -131 (0xFFFFFF7D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7D");

        /// <summary>
        /// A singleton boxed 32-bin integer -130 (0xFFFFFF7E).
        /// </summary>
        public static readonly object Int32_FFFFFF7E = -130;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -130 (0xFFFFFF7E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7E");

        /// <summary>
        /// A singleton boxed 32-bin integer -129 (0xFFFFFF7F).
        /// </summary>
        public static readonly object Int32_FFFFFF7F = -129;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -129 (0xFFFFFF7F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF7F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF7F");

        /// <summary>
        /// A singleton boxed 32-bin integer -128 (0xFFFFFF80).
        /// </summary>
        public static readonly object Int32_FFFFFF80 = -128;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -128 (0xFFFFFF80).
        /// </summary>
        public static readonly Expression Int32_FFFFFF80_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF80");

        /// <summary>
        /// A singleton boxed 32-bin integer -127 (0xFFFFFF81).
        /// </summary>
        public static readonly object Int32_FFFFFF81 = -127;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -127 (0xFFFFFF81).
        /// </summary>
        public static readonly Expression Int32_FFFFFF81_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF81");

        /// <summary>
        /// A singleton boxed 32-bin integer -126 (0xFFFFFF82).
        /// </summary>
        public static readonly object Int32_FFFFFF82 = -126;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -126 (0xFFFFFF82).
        /// </summary>
        public static readonly Expression Int32_FFFFFF82_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF82");

        /// <summary>
        /// A singleton boxed 32-bin integer -125 (0xFFFFFF83).
        /// </summary>
        public static readonly object Int32_FFFFFF83 = -125;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -125 (0xFFFFFF83).
        /// </summary>
        public static readonly Expression Int32_FFFFFF83_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF83");

        /// <summary>
        /// A singleton boxed 32-bin integer -124 (0xFFFFFF84).
        /// </summary>
        public static readonly object Int32_FFFFFF84 = -124;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -124 (0xFFFFFF84).
        /// </summary>
        public static readonly Expression Int32_FFFFFF84_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF84");

        /// <summary>
        /// A singleton boxed 32-bin integer -123 (0xFFFFFF85).
        /// </summary>
        public static readonly object Int32_FFFFFF85 = -123;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -123 (0xFFFFFF85).
        /// </summary>
        public static readonly Expression Int32_FFFFFF85_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF85");

        /// <summary>
        /// A singleton boxed 32-bin integer -122 (0xFFFFFF86).
        /// </summary>
        public static readonly object Int32_FFFFFF86 = -122;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -122 (0xFFFFFF86).
        /// </summary>
        public static readonly Expression Int32_FFFFFF86_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF86");

        /// <summary>
        /// A singleton boxed 32-bin integer -121 (0xFFFFFF87).
        /// </summary>
        public static readonly object Int32_FFFFFF87 = -121;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -121 (0xFFFFFF87).
        /// </summary>
        public static readonly Expression Int32_FFFFFF87_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF87");

        /// <summary>
        /// A singleton boxed 32-bin integer -120 (0xFFFFFF88).
        /// </summary>
        public static readonly object Int32_FFFFFF88 = -120;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -120 (0xFFFFFF88).
        /// </summary>
        public static readonly Expression Int32_FFFFFF88_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF88");

        /// <summary>
        /// A singleton boxed 32-bin integer -119 (0xFFFFFF89).
        /// </summary>
        public static readonly object Int32_FFFFFF89 = -119;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -119 (0xFFFFFF89).
        /// </summary>
        public static readonly Expression Int32_FFFFFF89_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF89");

        /// <summary>
        /// A singleton boxed 32-bin integer -118 (0xFFFFFF8A).
        /// </summary>
        public static readonly object Int32_FFFFFF8A = -118;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -118 (0xFFFFFF8A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8A");

        /// <summary>
        /// A singleton boxed 32-bin integer -117 (0xFFFFFF8B).
        /// </summary>
        public static readonly object Int32_FFFFFF8B = -117;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -117 (0xFFFFFF8B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8B");

        /// <summary>
        /// A singleton boxed 32-bin integer -116 (0xFFFFFF8C).
        /// </summary>
        public static readonly object Int32_FFFFFF8C = -116;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -116 (0xFFFFFF8C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8C");

        /// <summary>
        /// A singleton boxed 32-bin integer -115 (0xFFFFFF8D).
        /// </summary>
        public static readonly object Int32_FFFFFF8D = -115;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -115 (0xFFFFFF8D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8D");

        /// <summary>
        /// A singleton boxed 32-bin integer -114 (0xFFFFFF8E).
        /// </summary>
        public static readonly object Int32_FFFFFF8E = -114;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -114 (0xFFFFFF8E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8E");

        /// <summary>
        /// A singleton boxed 32-bin integer -113 (0xFFFFFF8F).
        /// </summary>
        public static readonly object Int32_FFFFFF8F = -113;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -113 (0xFFFFFF8F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF8F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF8F");

        /// <summary>
        /// A singleton boxed 32-bin integer -112 (0xFFFFFF90).
        /// </summary>
        public static readonly object Int32_FFFFFF90 = -112;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -112 (0xFFFFFF90).
        /// </summary>
        public static readonly Expression Int32_FFFFFF90_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF90");

        /// <summary>
        /// A singleton boxed 32-bin integer -111 (0xFFFFFF91).
        /// </summary>
        public static readonly object Int32_FFFFFF91 = -111;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -111 (0xFFFFFF91).
        /// </summary>
        public static readonly Expression Int32_FFFFFF91_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF91");

        /// <summary>
        /// A singleton boxed 32-bin integer -110 (0xFFFFFF92).
        /// </summary>
        public static readonly object Int32_FFFFFF92 = -110;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -110 (0xFFFFFF92).
        /// </summary>
        public static readonly Expression Int32_FFFFFF92_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF92");

        /// <summary>
        /// A singleton boxed 32-bin integer -109 (0xFFFFFF93).
        /// </summary>
        public static readonly object Int32_FFFFFF93 = -109;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -109 (0xFFFFFF93).
        /// </summary>
        public static readonly Expression Int32_FFFFFF93_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF93");

        /// <summary>
        /// A singleton boxed 32-bin integer -108 (0xFFFFFF94).
        /// </summary>
        public static readonly object Int32_FFFFFF94 = -108;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -108 (0xFFFFFF94).
        /// </summary>
        public static readonly Expression Int32_FFFFFF94_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF94");

        /// <summary>
        /// A singleton boxed 32-bin integer -107 (0xFFFFFF95).
        /// </summary>
        public static readonly object Int32_FFFFFF95 = -107;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -107 (0xFFFFFF95).
        /// </summary>
        public static readonly Expression Int32_FFFFFF95_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF95");

        /// <summary>
        /// A singleton boxed 32-bin integer -106 (0xFFFFFF96).
        /// </summary>
        public static readonly object Int32_FFFFFF96 = -106;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -106 (0xFFFFFF96).
        /// </summary>
        public static readonly Expression Int32_FFFFFF96_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF96");

        /// <summary>
        /// A singleton boxed 32-bin integer -105 (0xFFFFFF97).
        /// </summary>
        public static readonly object Int32_FFFFFF97 = -105;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -105 (0xFFFFFF97).
        /// </summary>
        public static readonly Expression Int32_FFFFFF97_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF97");

        /// <summary>
        /// A singleton boxed 32-bin integer -104 (0xFFFFFF98).
        /// </summary>
        public static readonly object Int32_FFFFFF98 = -104;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -104 (0xFFFFFF98).
        /// </summary>
        public static readonly Expression Int32_FFFFFF98_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF98");

        /// <summary>
        /// A singleton boxed 32-bin integer -103 (0xFFFFFF99).
        /// </summary>
        public static readonly object Int32_FFFFFF99 = -103;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -103 (0xFFFFFF99).
        /// </summary>
        public static readonly Expression Int32_FFFFFF99_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF99");

        /// <summary>
        /// A singleton boxed 32-bin integer -102 (0xFFFFFF9A).
        /// </summary>
        public static readonly object Int32_FFFFFF9A = -102;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -102 (0xFFFFFF9A).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9A");

        /// <summary>
        /// A singleton boxed 32-bin integer -101 (0xFFFFFF9B).
        /// </summary>
        public static readonly object Int32_FFFFFF9B = -101;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -101 (0xFFFFFF9B).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9B");

        /// <summary>
        /// A singleton boxed 32-bin integer -100 (0xFFFFFF9C).
        /// </summary>
        public static readonly object Int32_FFFFFF9C = -100;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -100 (0xFFFFFF9C).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9C");

        /// <summary>
        /// A singleton boxed 32-bin integer -99 (0xFFFFFF9D).
        /// </summary>
        public static readonly object Int32_FFFFFF9D = -99;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -99 (0xFFFFFF9D).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9D");

        /// <summary>
        /// A singleton boxed 32-bin integer -98 (0xFFFFFF9E).
        /// </summary>
        public static readonly object Int32_FFFFFF9E = -98;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -98 (0xFFFFFF9E).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9E");

        /// <summary>
        /// A singleton boxed 32-bin integer -97 (0xFFFFFF9F).
        /// </summary>
        public static readonly object Int32_FFFFFF9F = -97;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -97 (0xFFFFFF9F).
        /// </summary>
        public static readonly Expression Int32_FFFFFF9F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFF9F");

        /// <summary>
        /// A singleton boxed 32-bin integer -96 (0xFFFFFFA0).
        /// </summary>
        public static readonly object Int32_FFFFFFA0 = -96;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -96 (0xFFFFFFA0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA0");

        /// <summary>
        /// A singleton boxed 32-bin integer -95 (0xFFFFFFA1).
        /// </summary>
        public static readonly object Int32_FFFFFFA1 = -95;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -95 (0xFFFFFFA1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA1");

        /// <summary>
        /// A singleton boxed 32-bin integer -94 (0xFFFFFFA2).
        /// </summary>
        public static readonly object Int32_FFFFFFA2 = -94;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -94 (0xFFFFFFA2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA2");

        /// <summary>
        /// A singleton boxed 32-bin integer -93 (0xFFFFFFA3).
        /// </summary>
        public static readonly object Int32_FFFFFFA3 = -93;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -93 (0xFFFFFFA3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA3");

        /// <summary>
        /// A singleton boxed 32-bin integer -92 (0xFFFFFFA4).
        /// </summary>
        public static readonly object Int32_FFFFFFA4 = -92;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -92 (0xFFFFFFA4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA4");

        /// <summary>
        /// A singleton boxed 32-bin integer -91 (0xFFFFFFA5).
        /// </summary>
        public static readonly object Int32_FFFFFFA5 = -91;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -91 (0xFFFFFFA5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA5");

        /// <summary>
        /// A singleton boxed 32-bin integer -90 (0xFFFFFFA6).
        /// </summary>
        public static readonly object Int32_FFFFFFA6 = -90;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -90 (0xFFFFFFA6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA6");

        /// <summary>
        /// A singleton boxed 32-bin integer -89 (0xFFFFFFA7).
        /// </summary>
        public static readonly object Int32_FFFFFFA7 = -89;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -89 (0xFFFFFFA7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA7");

        /// <summary>
        /// A singleton boxed 32-bin integer -88 (0xFFFFFFA8).
        /// </summary>
        public static readonly object Int32_FFFFFFA8 = -88;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -88 (0xFFFFFFA8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA8");

        /// <summary>
        /// A singleton boxed 32-bin integer -87 (0xFFFFFFA9).
        /// </summary>
        public static readonly object Int32_FFFFFFA9 = -87;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -87 (0xFFFFFFA9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFA9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFA9");

        /// <summary>
        /// A singleton boxed 32-bin integer -86 (0xFFFFFFAA).
        /// </summary>
        public static readonly object Int32_FFFFFFAA = -86;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -86 (0xFFFFFFAA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAA");

        /// <summary>
        /// A singleton boxed 32-bin integer -85 (0xFFFFFFAB).
        /// </summary>
        public static readonly object Int32_FFFFFFAB = -85;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -85 (0xFFFFFFAB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAB");

        /// <summary>
        /// A singleton boxed 32-bin integer -84 (0xFFFFFFAC).
        /// </summary>
        public static readonly object Int32_FFFFFFAC = -84;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -84 (0xFFFFFFAC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAC");

        /// <summary>
        /// A singleton boxed 32-bin integer -83 (0xFFFFFFAD).
        /// </summary>
        public static readonly object Int32_FFFFFFAD = -83;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -83 (0xFFFFFFAD).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAD");

        /// <summary>
        /// A singleton boxed 32-bin integer -82 (0xFFFFFFAE).
        /// </summary>
        public static readonly object Int32_FFFFFFAE = -82;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -82 (0xFFFFFFAE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAE");

        /// <summary>
        /// A singleton boxed 32-bin integer -81 (0xFFFFFFAF).
        /// </summary>
        public static readonly object Int32_FFFFFFAF = -81;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -81 (0xFFFFFFAF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFAF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFAF");

        /// <summary>
        /// A singleton boxed 32-bin integer -80 (0xFFFFFFB0).
        /// </summary>
        public static readonly object Int32_FFFFFFB0 = -80;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -80 (0xFFFFFFB0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB0");

        /// <summary>
        /// A singleton boxed 32-bin integer -79 (0xFFFFFFB1).
        /// </summary>
        public static readonly object Int32_FFFFFFB1 = -79;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -79 (0xFFFFFFB1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB1");

        /// <summary>
        /// A singleton boxed 32-bin integer -78 (0xFFFFFFB2).
        /// </summary>
        public static readonly object Int32_FFFFFFB2 = -78;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -78 (0xFFFFFFB2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB2");

        /// <summary>
        /// A singleton boxed 32-bin integer -77 (0xFFFFFFB3).
        /// </summary>
        public static readonly object Int32_FFFFFFB3 = -77;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -77 (0xFFFFFFB3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB3");

        /// <summary>
        /// A singleton boxed 32-bin integer -76 (0xFFFFFFB4).
        /// </summary>
        public static readonly object Int32_FFFFFFB4 = -76;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -76 (0xFFFFFFB4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB4");

        /// <summary>
        /// A singleton boxed 32-bin integer -75 (0xFFFFFFB5).
        /// </summary>
        public static readonly object Int32_FFFFFFB5 = -75;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -75 (0xFFFFFFB5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB5");

        /// <summary>
        /// A singleton boxed 32-bin integer -74 (0xFFFFFFB6).
        /// </summary>
        public static readonly object Int32_FFFFFFB6 = -74;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -74 (0xFFFFFFB6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB6");

        /// <summary>
        /// A singleton boxed 32-bin integer -73 (0xFFFFFFB7).
        /// </summary>
        public static readonly object Int32_FFFFFFB7 = -73;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -73 (0xFFFFFFB7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB7");

        /// <summary>
        /// A singleton boxed 32-bin integer -72 (0xFFFFFFB8).
        /// </summary>
        public static readonly object Int32_FFFFFFB8 = -72;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -72 (0xFFFFFFB8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB8");

        /// <summary>
        /// A singleton boxed 32-bin integer -71 (0xFFFFFFB9).
        /// </summary>
        public static readonly object Int32_FFFFFFB9 = -71;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -71 (0xFFFFFFB9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFB9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFB9");

        /// <summary>
        /// A singleton boxed 32-bin integer -70 (0xFFFFFFBA).
        /// </summary>
        public static readonly object Int32_FFFFFFBA = -70;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -70 (0xFFFFFFBA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBA");

        /// <summary>
        /// A singleton boxed 32-bin integer -69 (0xFFFFFFBB).
        /// </summary>
        public static readonly object Int32_FFFFFFBB = -69;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -69 (0xFFFFFFBB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBB");

        /// <summary>
        /// A singleton boxed 32-bin integer -68 (0xFFFFFFBC).
        /// </summary>
        public static readonly object Int32_FFFFFFBC = -68;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -68 (0xFFFFFFBC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBC");

        /// <summary>
        /// A singleton boxed 32-bin integer -67 (0xFFFFFFBD).
        /// </summary>
        public static readonly object Int32_FFFFFFBD = -67;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -67 (0xFFFFFFBD).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBD");

        /// <summary>
        /// A singleton boxed 32-bin integer -66 (0xFFFFFFBE).
        /// </summary>
        public static readonly object Int32_FFFFFFBE = -66;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -66 (0xFFFFFFBE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBE");

        /// <summary>
        /// A singleton boxed 32-bin integer -65 (0xFFFFFFBF).
        /// </summary>
        public static readonly object Int32_FFFFFFBF = -65;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -65 (0xFFFFFFBF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFBF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFBF");

        /// <summary>
        /// A singleton boxed 32-bin integer -64 (0xFFFFFFC0).
        /// </summary>
        public static readonly object Int32_FFFFFFC0 = -64;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -64 (0xFFFFFFC0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC0");

        /// <summary>
        /// A singleton boxed 32-bin integer -63 (0xFFFFFFC1).
        /// </summary>
        public static readonly object Int32_FFFFFFC1 = -63;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -63 (0xFFFFFFC1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC1");

        /// <summary>
        /// A singleton boxed 32-bin integer -62 (0xFFFFFFC2).
        /// </summary>
        public static readonly object Int32_FFFFFFC2 = -62;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -62 (0xFFFFFFC2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC2");

        /// <summary>
        /// A singleton boxed 32-bin integer -61 (0xFFFFFFC3).
        /// </summary>
        public static readonly object Int32_FFFFFFC3 = -61;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -61 (0xFFFFFFC3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC3");

        /// <summary>
        /// A singleton boxed 32-bin integer -60 (0xFFFFFFC4).
        /// </summary>
        public static readonly object Int32_FFFFFFC4 = -60;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -60 (0xFFFFFFC4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC4");

        /// <summary>
        /// A singleton boxed 32-bin integer -59 (0xFFFFFFC5).
        /// </summary>
        public static readonly object Int32_FFFFFFC5 = -59;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -59 (0xFFFFFFC5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC5");

        /// <summary>
        /// A singleton boxed 32-bin integer -58 (0xFFFFFFC6).
        /// </summary>
        public static readonly object Int32_FFFFFFC6 = -58;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -58 (0xFFFFFFC6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC6");

        /// <summary>
        /// A singleton boxed 32-bin integer -57 (0xFFFFFFC7).
        /// </summary>
        public static readonly object Int32_FFFFFFC7 = -57;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -57 (0xFFFFFFC7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC7");

        /// <summary>
        /// A singleton boxed 32-bin integer -56 (0xFFFFFFC8).
        /// </summary>
        public static readonly object Int32_FFFFFFC8 = -56;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -56 (0xFFFFFFC8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC8");

        /// <summary>
        /// A singleton boxed 32-bin integer -55 (0xFFFFFFC9).
        /// </summary>
        public static readonly object Int32_FFFFFFC9 = -55;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -55 (0xFFFFFFC9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFC9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFC9");

        /// <summary>
        /// A singleton boxed 32-bin integer -54 (0xFFFFFFCA).
        /// </summary>
        public static readonly object Int32_FFFFFFCA = -54;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -54 (0xFFFFFFCA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCA");

        /// <summary>
        /// A singleton boxed 32-bin integer -53 (0xFFFFFFCB).
        /// </summary>
        public static readonly object Int32_FFFFFFCB = -53;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -53 (0xFFFFFFCB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCB");

        /// <summary>
        /// A singleton boxed 32-bin integer -52 (0xFFFFFFCC).
        /// </summary>
        public static readonly object Int32_FFFFFFCC = -52;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -52 (0xFFFFFFCC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCC");

        /// <summary>
        /// A singleton boxed 32-bin integer -51 (0xFFFFFFCD).
        /// </summary>
        public static readonly object Int32_FFFFFFCD = -51;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -51 (0xFFFFFFCD).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCD");

        /// <summary>
        /// A singleton boxed 32-bin integer -50 (0xFFFFFFCE).
        /// </summary>
        public static readonly object Int32_FFFFFFCE = -50;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -50 (0xFFFFFFCE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCE");

        /// <summary>
        /// A singleton boxed 32-bin integer -49 (0xFFFFFFCF).
        /// </summary>
        public static readonly object Int32_FFFFFFCF = -49;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -49 (0xFFFFFFCF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFCF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFCF");

        /// <summary>
        /// A singleton boxed 32-bin integer -48 (0xFFFFFFD0).
        /// </summary>
        public static readonly object Int32_FFFFFFD0 = -48;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -48 (0xFFFFFFD0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD0");

        /// <summary>
        /// A singleton boxed 32-bin integer -47 (0xFFFFFFD1).
        /// </summary>
        public static readonly object Int32_FFFFFFD1 = -47;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -47 (0xFFFFFFD1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD1");

        /// <summary>
        /// A singleton boxed 32-bin integer -46 (0xFFFFFFD2).
        /// </summary>
        public static readonly object Int32_FFFFFFD2 = -46;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -46 (0xFFFFFFD2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD2");

        /// <summary>
        /// A singleton boxed 32-bin integer -45 (0xFFFFFFD3).
        /// </summary>
        public static readonly object Int32_FFFFFFD3 = -45;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -45 (0xFFFFFFD3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD3");

        /// <summary>
        /// A singleton boxed 32-bin integer -44 (0xFFFFFFD4).
        /// </summary>
        public static readonly object Int32_FFFFFFD4 = -44;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -44 (0xFFFFFFD4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD4");

        /// <summary>
        /// A singleton boxed 32-bin integer -43 (0xFFFFFFD5).
        /// </summary>
        public static readonly object Int32_FFFFFFD5 = -43;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -43 (0xFFFFFFD5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD5");

        /// <summary>
        /// A singleton boxed 32-bin integer -42 (0xFFFFFFD6).
        /// </summary>
        public static readonly object Int32_FFFFFFD6 = -42;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -42 (0xFFFFFFD6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD6");

        /// <summary>
        /// A singleton boxed 32-bin integer -41 (0xFFFFFFD7).
        /// </summary>
        public static readonly object Int32_FFFFFFD7 = -41;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -41 (0xFFFFFFD7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD7");

        /// <summary>
        /// A singleton boxed 32-bin integer -40 (0xFFFFFFD8).
        /// </summary>
        public static readonly object Int32_FFFFFFD8 = -40;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -40 (0xFFFFFFD8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD8");

        /// <summary>
        /// A singleton boxed 32-bin integer -39 (0xFFFFFFD9).
        /// </summary>
        public static readonly object Int32_FFFFFFD9 = -39;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -39 (0xFFFFFFD9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFD9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFD9");

        /// <summary>
        /// A singleton boxed 32-bin integer -38 (0xFFFFFFDA).
        /// </summary>
        public static readonly object Int32_FFFFFFDA = -38;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -38 (0xFFFFFFDA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDA");

        /// <summary>
        /// A singleton boxed 32-bin integer -37 (0xFFFFFFDB).
        /// </summary>
        public static readonly object Int32_FFFFFFDB = -37;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -37 (0xFFFFFFDB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDB");

        /// <summary>
        /// A singleton boxed 32-bin integer -36 (0xFFFFFFDC).
        /// </summary>
        public static readonly object Int32_FFFFFFDC = -36;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -36 (0xFFFFFFDC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDC");

        /// <summary>
        /// A singleton boxed 32-bin integer -35 (0xFFFFFFDD).
        /// </summary>
        public static readonly object Int32_FFFFFFDD = -35;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -35 (0xFFFFFFDD).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDD");

        /// <summary>
        /// A singleton boxed 32-bin integer -34 (0xFFFFFFDE).
        /// </summary>
        public static readonly object Int32_FFFFFFDE = -34;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -34 (0xFFFFFFDE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDE");

        /// <summary>
        /// A singleton boxed 32-bin integer -33 (0xFFFFFFDF).
        /// </summary>
        public static readonly object Int32_FFFFFFDF = -33;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -33 (0xFFFFFFDF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFDF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFDF");

        /// <summary>
        /// A singleton boxed 32-bin integer -32 (0xFFFFFFE0).
        /// </summary>
        public static readonly object Int32_FFFFFFE0 = -32;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -32 (0xFFFFFFE0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE0");

        /// <summary>
        /// A singleton boxed 32-bin integer -31 (0xFFFFFFE1).
        /// </summary>
        public static readonly object Int32_FFFFFFE1 = -31;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -31 (0xFFFFFFE1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE1");

        /// <summary>
        /// A singleton boxed 32-bin integer -30 (0xFFFFFFE2).
        /// </summary>
        public static readonly object Int32_FFFFFFE2 = -30;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -30 (0xFFFFFFE2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE2");

        /// <summary>
        /// A singleton boxed 32-bin integer -29 (0xFFFFFFE3).
        /// </summary>
        public static readonly object Int32_FFFFFFE3 = -29;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -29 (0xFFFFFFE3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE3");

        /// <summary>
        /// A singleton boxed 32-bin integer -28 (0xFFFFFFE4).
        /// </summary>
        public static readonly object Int32_FFFFFFE4 = -28;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -28 (0xFFFFFFE4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE4");

        /// <summary>
        /// A singleton boxed 32-bin integer -27 (0xFFFFFFE5).
        /// </summary>
        public static readonly object Int32_FFFFFFE5 = -27;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -27 (0xFFFFFFE5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE5");

        /// <summary>
        /// A singleton boxed 32-bin integer -26 (0xFFFFFFE6).
        /// </summary>
        public static readonly object Int32_FFFFFFE6 = -26;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -26 (0xFFFFFFE6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE6");

        /// <summary>
        /// A singleton boxed 32-bin integer -25 (0xFFFFFFE7).
        /// </summary>
        public static readonly object Int32_FFFFFFE7 = -25;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -25 (0xFFFFFFE7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE7");

        /// <summary>
        /// A singleton boxed 32-bin integer -24 (0xFFFFFFE8).
        /// </summary>
        public static readonly object Int32_FFFFFFE8 = -24;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -24 (0xFFFFFFE8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE8");

        /// <summary>
        /// A singleton boxed 32-bin integer -23 (0xFFFFFFE9).
        /// </summary>
        public static readonly object Int32_FFFFFFE9 = -23;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -23 (0xFFFFFFE9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFE9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFE9");

        /// <summary>
        /// A singleton boxed 32-bin integer -22 (0xFFFFFFEA).
        /// </summary>
        public static readonly object Int32_FFFFFFEA = -22;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -22 (0xFFFFFFEA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFEA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFEA");

        /// <summary>
        /// A singleton boxed 32-bin integer -21 (0xFFFFFFEB).
        /// </summary>
        public static readonly object Int32_FFFFFFEB = -21;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -21 (0xFFFFFFEB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFEB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFEB");

        /// <summary>
        /// A singleton boxed 32-bin integer -20 (0xFFFFFFEC).
        /// </summary>
        public static readonly object Int32_FFFFFFEC = -20;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -20 (0xFFFFFFEC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFEC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFEC");

        /// <summary>
        /// A singleton boxed 32-bin integer -19 (0xFFFFFFED).
        /// </summary>
        public static readonly object Int32_FFFFFFED = -19;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -19 (0xFFFFFFED).
        /// </summary>
        public static readonly Expression Int32_FFFFFFED_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFED");

        /// <summary>
        /// A singleton boxed 32-bin integer -18 (0xFFFFFFEE).
        /// </summary>
        public static readonly object Int32_FFFFFFEE = -18;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -18 (0xFFFFFFEE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFEE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFEE");

        /// <summary>
        /// A singleton boxed 32-bin integer -17 (0xFFFFFFEF).
        /// </summary>
        public static readonly object Int32_FFFFFFEF = -17;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -17 (0xFFFFFFEF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFEF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFEF");

        /// <summary>
        /// A singleton boxed 32-bin integer -16 (0xFFFFFFF0).
        /// </summary>
        public static readonly object Int32_FFFFFFF0 = -16;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -16 (0xFFFFFFF0).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF0");

        /// <summary>
        /// A singleton boxed 32-bin integer -15 (0xFFFFFFF1).
        /// </summary>
        public static readonly object Int32_FFFFFFF1 = -15;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -15 (0xFFFFFFF1).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF1");

        /// <summary>
        /// A singleton boxed 32-bin integer -14 (0xFFFFFFF2).
        /// </summary>
        public static readonly object Int32_FFFFFFF2 = -14;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -14 (0xFFFFFFF2).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF2");

        /// <summary>
        /// A singleton boxed 32-bin integer -13 (0xFFFFFFF3).
        /// </summary>
        public static readonly object Int32_FFFFFFF3 = -13;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -13 (0xFFFFFFF3).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF3");

        /// <summary>
        /// A singleton boxed 32-bin integer -12 (0xFFFFFFF4).
        /// </summary>
        public static readonly object Int32_FFFFFFF4 = -12;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -12 (0xFFFFFFF4).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF4");

        /// <summary>
        /// A singleton boxed 32-bin integer -11 (0xFFFFFFF5).
        /// </summary>
        public static readonly object Int32_FFFFFFF5 = -11;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -11 (0xFFFFFFF5).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF5");

        /// <summary>
        /// A singleton boxed 32-bin integer -10 (0xFFFFFFF6).
        /// </summary>
        public static readonly object Int32_FFFFFFF6 = -10;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -10 (0xFFFFFFF6).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF6");

        /// <summary>
        /// A singleton boxed 32-bin integer -9 (0xFFFFFFF7).
        /// </summary>
        public static readonly object Int32_FFFFFFF7 = -9;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -9 (0xFFFFFFF7).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF7");

        /// <summary>
        /// A singleton boxed 32-bin integer -8 (0xFFFFFFF8).
        /// </summary>
        public static readonly object Int32_FFFFFFF8 = -8;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -8 (0xFFFFFFF8).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF8");

        /// <summary>
        /// A singleton boxed 32-bin integer -7 (0xFFFFFFF9).
        /// </summary>
        public static readonly object Int32_FFFFFFF9 = -7;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -7 (0xFFFFFFF9).
        /// </summary>
        public static readonly Expression Int32_FFFFFFF9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFF9");

        /// <summary>
        /// A singleton boxed 32-bin integer -6 (0xFFFFFFFA).
        /// </summary>
        public static readonly object Int32_FFFFFFFA = -6;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -6 (0xFFFFFFFA).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFA");

        /// <summary>
        /// A singleton boxed 32-bin integer -5 (0xFFFFFFFB).
        /// </summary>
        public static readonly object Int32_FFFFFFFB = -5;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -5 (0xFFFFFFFB).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFB");

        /// <summary>
        /// A singleton boxed 32-bin integer -4 (0xFFFFFFFC).
        /// </summary>
        public static readonly object Int32_FFFFFFFC = -4;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -4 (0xFFFFFFFC).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFC");

        /// <summary>
        /// A singleton boxed 32-bin integer -3 (0xFFFFFFFD).
        /// </summary>
        public static readonly object Int32_FFFFFFFD = -3;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -3 (0xFFFFFFFD).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFD");

        /// <summary>
        /// A singleton boxed 32-bin integer -2 (0xFFFFFFFE).
        /// </summary>
        public static readonly object Int32_FFFFFFFE = -2;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -2 (0xFFFFFFFE).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFE");

        /// <summary>
        /// A singleton boxed 32-bin integer -1 (0xFFFFFFFF).
        /// </summary>
        public static readonly object Int32_FFFFFFFF = -1;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer -1 (0xFFFFFFFF).
        /// </summary>
        public static readonly Expression Int32_FFFFFFFF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_FFFFFFFF");

        /// <summary>
        /// A singleton boxed 32-bin integer 0 (0x00000000).
        /// </summary>
        public static readonly object Int32_00000000 = 0;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 0 (0x00000000).
        /// </summary>
        public static readonly Expression Int32_00000000_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000000");

        /// <summary>
        /// A singleton boxed 32-bin integer 1 (0x00000001).
        /// </summary>
        public static readonly object Int32_00000001 = 1;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 1 (0x00000001).
        /// </summary>
        public static readonly Expression Int32_00000001_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000001");

        /// <summary>
        /// A singleton boxed 32-bin integer 2 (0x00000002).
        /// </summary>
        public static readonly object Int32_00000002 = 2;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 2 (0x00000002).
        /// </summary>
        public static readonly Expression Int32_00000002_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000002");

        /// <summary>
        /// A singleton boxed 32-bin integer 3 (0x00000003).
        /// </summary>
        public static readonly object Int32_00000003 = 3;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 3 (0x00000003).
        /// </summary>
        public static readonly Expression Int32_00000003_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000003");

        /// <summary>
        /// A singleton boxed 32-bin integer 4 (0x00000004).
        /// </summary>
        public static readonly object Int32_00000004 = 4;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 4 (0x00000004).
        /// </summary>
        public static readonly Expression Int32_00000004_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000004");

        /// <summary>
        /// A singleton boxed 32-bin integer 5 (0x00000005).
        /// </summary>
        public static readonly object Int32_00000005 = 5;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 5 (0x00000005).
        /// </summary>
        public static readonly Expression Int32_00000005_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000005");

        /// <summary>
        /// A singleton boxed 32-bin integer 6 (0x00000006).
        /// </summary>
        public static readonly object Int32_00000006 = 6;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 6 (0x00000006).
        /// </summary>
        public static readonly Expression Int32_00000006_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000006");

        /// <summary>
        /// A singleton boxed 32-bin integer 7 (0x00000007).
        /// </summary>
        public static readonly object Int32_00000007 = 7;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 7 (0x00000007).
        /// </summary>
        public static readonly Expression Int32_00000007_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000007");

        /// <summary>
        /// A singleton boxed 32-bin integer 8 (0x00000008).
        /// </summary>
        public static readonly object Int32_00000008 = 8;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 8 (0x00000008).
        /// </summary>
        public static readonly Expression Int32_00000008_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000008");

        /// <summary>
        /// A singleton boxed 32-bin integer 9 (0x00000009).
        /// </summary>
        public static readonly object Int32_00000009 = 9;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 9 (0x00000009).
        /// </summary>
        public static readonly Expression Int32_00000009_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000009");

        /// <summary>
        /// A singleton boxed 32-bin integer 10 (0x0000000A).
        /// </summary>
        public static readonly object Int32_0000000A = 10;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 10 (0x0000000A).
        /// </summary>
        public static readonly Expression Int32_0000000A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000A");

        /// <summary>
        /// A singleton boxed 32-bin integer 11 (0x0000000B).
        /// </summary>
        public static readonly object Int32_0000000B = 11;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 11 (0x0000000B).
        /// </summary>
        public static readonly Expression Int32_0000000B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000B");

        /// <summary>
        /// A singleton boxed 32-bin integer 12 (0x0000000C).
        /// </summary>
        public static readonly object Int32_0000000C = 12;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 12 (0x0000000C).
        /// </summary>
        public static readonly Expression Int32_0000000C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000C");

        /// <summary>
        /// A singleton boxed 32-bin integer 13 (0x0000000D).
        /// </summary>
        public static readonly object Int32_0000000D = 13;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 13 (0x0000000D).
        /// </summary>
        public static readonly Expression Int32_0000000D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000D");

        /// <summary>
        /// A singleton boxed 32-bin integer 14 (0x0000000E).
        /// </summary>
        public static readonly object Int32_0000000E = 14;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 14 (0x0000000E).
        /// </summary>
        public static readonly Expression Int32_0000000E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000E");

        /// <summary>
        /// A singleton boxed 32-bin integer 15 (0x0000000F).
        /// </summary>
        public static readonly object Int32_0000000F = 15;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 15 (0x0000000F).
        /// </summary>
        public static readonly Expression Int32_0000000F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000000F");

        /// <summary>
        /// A singleton boxed 32-bin integer 16 (0x00000010).
        /// </summary>
        public static readonly object Int32_00000010 = 16;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 16 (0x00000010).
        /// </summary>
        public static readonly Expression Int32_00000010_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000010");

        /// <summary>
        /// A singleton boxed 32-bin integer 17 (0x00000011).
        /// </summary>
        public static readonly object Int32_00000011 = 17;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 17 (0x00000011).
        /// </summary>
        public static readonly Expression Int32_00000011_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000011");

        /// <summary>
        /// A singleton boxed 32-bin integer 18 (0x00000012).
        /// </summary>
        public static readonly object Int32_00000012 = 18;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 18 (0x00000012).
        /// </summary>
        public static readonly Expression Int32_00000012_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000012");

        /// <summary>
        /// A singleton boxed 32-bin integer 19 (0x00000013).
        /// </summary>
        public static readonly object Int32_00000013 = 19;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 19 (0x00000013).
        /// </summary>
        public static readonly Expression Int32_00000013_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000013");

        /// <summary>
        /// A singleton boxed 32-bin integer 20 (0x00000014).
        /// </summary>
        public static readonly object Int32_00000014 = 20;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 20 (0x00000014).
        /// </summary>
        public static readonly Expression Int32_00000014_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000014");

        /// <summary>
        /// A singleton boxed 32-bin integer 21 (0x00000015).
        /// </summary>
        public static readonly object Int32_00000015 = 21;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 21 (0x00000015).
        /// </summary>
        public static readonly Expression Int32_00000015_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000015");

        /// <summary>
        /// A singleton boxed 32-bin integer 22 (0x00000016).
        /// </summary>
        public static readonly object Int32_00000016 = 22;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 22 (0x00000016).
        /// </summary>
        public static readonly Expression Int32_00000016_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000016");

        /// <summary>
        /// A singleton boxed 32-bin integer 23 (0x00000017).
        /// </summary>
        public static readonly object Int32_00000017 = 23;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 23 (0x00000017).
        /// </summary>
        public static readonly Expression Int32_00000017_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000017");

        /// <summary>
        /// A singleton boxed 32-bin integer 24 (0x00000018).
        /// </summary>
        public static readonly object Int32_00000018 = 24;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 24 (0x00000018).
        /// </summary>
        public static readonly Expression Int32_00000018_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000018");

        /// <summary>
        /// A singleton boxed 32-bin integer 25 (0x00000019).
        /// </summary>
        public static readonly object Int32_00000019 = 25;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 25 (0x00000019).
        /// </summary>
        public static readonly Expression Int32_00000019_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000019");

        /// <summary>
        /// A singleton boxed 32-bin integer 26 (0x0000001A).
        /// </summary>
        public static readonly object Int32_0000001A = 26;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 26 (0x0000001A).
        /// </summary>
        public static readonly Expression Int32_0000001A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001A");

        /// <summary>
        /// A singleton boxed 32-bin integer 27 (0x0000001B).
        /// </summary>
        public static readonly object Int32_0000001B = 27;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 27 (0x0000001B).
        /// </summary>
        public static readonly Expression Int32_0000001B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001B");

        /// <summary>
        /// A singleton boxed 32-bin integer 28 (0x0000001C).
        /// </summary>
        public static readonly object Int32_0000001C = 28;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 28 (0x0000001C).
        /// </summary>
        public static readonly Expression Int32_0000001C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001C");

        /// <summary>
        /// A singleton boxed 32-bin integer 29 (0x0000001D).
        /// </summary>
        public static readonly object Int32_0000001D = 29;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 29 (0x0000001D).
        /// </summary>
        public static readonly Expression Int32_0000001D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001D");

        /// <summary>
        /// A singleton boxed 32-bin integer 30 (0x0000001E).
        /// </summary>
        public static readonly object Int32_0000001E = 30;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 30 (0x0000001E).
        /// </summary>
        public static readonly Expression Int32_0000001E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001E");

        /// <summary>
        /// A singleton boxed 32-bin integer 31 (0x0000001F).
        /// </summary>
        public static readonly object Int32_0000001F = 31;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 31 (0x0000001F).
        /// </summary>
        public static readonly Expression Int32_0000001F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000001F");

        /// <summary>
        /// A singleton boxed 32-bin integer 32 (0x00000020).
        /// </summary>
        public static readonly object Int32_00000020 = 32;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 32 (0x00000020).
        /// </summary>
        public static readonly Expression Int32_00000020_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000020");

        /// <summary>
        /// A singleton boxed 32-bin integer 33 (0x00000021).
        /// </summary>
        public static readonly object Int32_00000021 = 33;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 33 (0x00000021).
        /// </summary>
        public static readonly Expression Int32_00000021_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000021");

        /// <summary>
        /// A singleton boxed 32-bin integer 34 (0x00000022).
        /// </summary>
        public static readonly object Int32_00000022 = 34;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 34 (0x00000022).
        /// </summary>
        public static readonly Expression Int32_00000022_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000022");

        /// <summary>
        /// A singleton boxed 32-bin integer 35 (0x00000023).
        /// </summary>
        public static readonly object Int32_00000023 = 35;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 35 (0x00000023).
        /// </summary>
        public static readonly Expression Int32_00000023_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000023");

        /// <summary>
        /// A singleton boxed 32-bin integer 36 (0x00000024).
        /// </summary>
        public static readonly object Int32_00000024 = 36;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 36 (0x00000024).
        /// </summary>
        public static readonly Expression Int32_00000024_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000024");

        /// <summary>
        /// A singleton boxed 32-bin integer 37 (0x00000025).
        /// </summary>
        public static readonly object Int32_00000025 = 37;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 37 (0x00000025).
        /// </summary>
        public static readonly Expression Int32_00000025_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000025");

        /// <summary>
        /// A singleton boxed 32-bin integer 38 (0x00000026).
        /// </summary>
        public static readonly object Int32_00000026 = 38;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 38 (0x00000026).
        /// </summary>
        public static readonly Expression Int32_00000026_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000026");

        /// <summary>
        /// A singleton boxed 32-bin integer 39 (0x00000027).
        /// </summary>
        public static readonly object Int32_00000027 = 39;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 39 (0x00000027).
        /// </summary>
        public static readonly Expression Int32_00000027_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000027");

        /// <summary>
        /// A singleton boxed 32-bin integer 40 (0x00000028).
        /// </summary>
        public static readonly object Int32_00000028 = 40;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 40 (0x00000028).
        /// </summary>
        public static readonly Expression Int32_00000028_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000028");

        /// <summary>
        /// A singleton boxed 32-bin integer 41 (0x00000029).
        /// </summary>
        public static readonly object Int32_00000029 = 41;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 41 (0x00000029).
        /// </summary>
        public static readonly Expression Int32_00000029_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000029");

        /// <summary>
        /// A singleton boxed 32-bin integer 42 (0x0000002A).
        /// </summary>
        public static readonly object Int32_0000002A = 42;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 42 (0x0000002A).
        /// </summary>
        public static readonly Expression Int32_0000002A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002A");

        /// <summary>
        /// A singleton boxed 32-bin integer 43 (0x0000002B).
        /// </summary>
        public static readonly object Int32_0000002B = 43;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 43 (0x0000002B).
        /// </summary>
        public static readonly Expression Int32_0000002B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002B");

        /// <summary>
        /// A singleton boxed 32-bin integer 44 (0x0000002C).
        /// </summary>
        public static readonly object Int32_0000002C = 44;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 44 (0x0000002C).
        /// </summary>
        public static readonly Expression Int32_0000002C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002C");

        /// <summary>
        /// A singleton boxed 32-bin integer 45 (0x0000002D).
        /// </summary>
        public static readonly object Int32_0000002D = 45;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 45 (0x0000002D).
        /// </summary>
        public static readonly Expression Int32_0000002D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002D");

        /// <summary>
        /// A singleton boxed 32-bin integer 46 (0x0000002E).
        /// </summary>
        public static readonly object Int32_0000002E = 46;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 46 (0x0000002E).
        /// </summary>
        public static readonly Expression Int32_0000002E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002E");

        /// <summary>
        /// A singleton boxed 32-bin integer 47 (0x0000002F).
        /// </summary>
        public static readonly object Int32_0000002F = 47;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 47 (0x0000002F).
        /// </summary>
        public static readonly Expression Int32_0000002F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000002F");

        /// <summary>
        /// A singleton boxed 32-bin integer 48 (0x00000030).
        /// </summary>
        public static readonly object Int32_00000030 = 48;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 48 (0x00000030).
        /// </summary>
        public static readonly Expression Int32_00000030_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000030");

        /// <summary>
        /// A singleton boxed 32-bin integer 49 (0x00000031).
        /// </summary>
        public static readonly object Int32_00000031 = 49;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 49 (0x00000031).
        /// </summary>
        public static readonly Expression Int32_00000031_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000031");

        /// <summary>
        /// A singleton boxed 32-bin integer 50 (0x00000032).
        /// </summary>
        public static readonly object Int32_00000032 = 50;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 50 (0x00000032).
        /// </summary>
        public static readonly Expression Int32_00000032_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000032");

        /// <summary>
        /// A singleton boxed 32-bin integer 51 (0x00000033).
        /// </summary>
        public static readonly object Int32_00000033 = 51;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 51 (0x00000033).
        /// </summary>
        public static readonly Expression Int32_00000033_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000033");

        /// <summary>
        /// A singleton boxed 32-bin integer 52 (0x00000034).
        /// </summary>
        public static readonly object Int32_00000034 = 52;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 52 (0x00000034).
        /// </summary>
        public static readonly Expression Int32_00000034_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000034");

        /// <summary>
        /// A singleton boxed 32-bin integer 53 (0x00000035).
        /// </summary>
        public static readonly object Int32_00000035 = 53;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 53 (0x00000035).
        /// </summary>
        public static readonly Expression Int32_00000035_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000035");

        /// <summary>
        /// A singleton boxed 32-bin integer 54 (0x00000036).
        /// </summary>
        public static readonly object Int32_00000036 = 54;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 54 (0x00000036).
        /// </summary>
        public static readonly Expression Int32_00000036_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000036");

        /// <summary>
        /// A singleton boxed 32-bin integer 55 (0x00000037).
        /// </summary>
        public static readonly object Int32_00000037 = 55;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 55 (0x00000037).
        /// </summary>
        public static readonly Expression Int32_00000037_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000037");

        /// <summary>
        /// A singleton boxed 32-bin integer 56 (0x00000038).
        /// </summary>
        public static readonly object Int32_00000038 = 56;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 56 (0x00000038).
        /// </summary>
        public static readonly Expression Int32_00000038_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000038");

        /// <summary>
        /// A singleton boxed 32-bin integer 57 (0x00000039).
        /// </summary>
        public static readonly object Int32_00000039 = 57;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 57 (0x00000039).
        /// </summary>
        public static readonly Expression Int32_00000039_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000039");

        /// <summary>
        /// A singleton boxed 32-bin integer 58 (0x0000003A).
        /// </summary>
        public static readonly object Int32_0000003A = 58;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 58 (0x0000003A).
        /// </summary>
        public static readonly Expression Int32_0000003A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003A");

        /// <summary>
        /// A singleton boxed 32-bin integer 59 (0x0000003B).
        /// </summary>
        public static readonly object Int32_0000003B = 59;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 59 (0x0000003B).
        /// </summary>
        public static readonly Expression Int32_0000003B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003B");

        /// <summary>
        /// A singleton boxed 32-bin integer 60 (0x0000003C).
        /// </summary>
        public static readonly object Int32_0000003C = 60;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 60 (0x0000003C).
        /// </summary>
        public static readonly Expression Int32_0000003C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003C");

        /// <summary>
        /// A singleton boxed 32-bin integer 61 (0x0000003D).
        /// </summary>
        public static readonly object Int32_0000003D = 61;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 61 (0x0000003D).
        /// </summary>
        public static readonly Expression Int32_0000003D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003D");

        /// <summary>
        /// A singleton boxed 32-bin integer 62 (0x0000003E).
        /// </summary>
        public static readonly object Int32_0000003E = 62;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 62 (0x0000003E).
        /// </summary>
        public static readonly Expression Int32_0000003E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003E");

        /// <summary>
        /// A singleton boxed 32-bin integer 63 (0x0000003F).
        /// </summary>
        public static readonly object Int32_0000003F = 63;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 63 (0x0000003F).
        /// </summary>
        public static readonly Expression Int32_0000003F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000003F");

        /// <summary>
        /// A singleton boxed 32-bin integer 64 (0x00000040).
        /// </summary>
        public static readonly object Int32_00000040 = 64;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 64 (0x00000040).
        /// </summary>
        public static readonly Expression Int32_00000040_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000040");

        /// <summary>
        /// A singleton boxed 32-bin integer 65 (0x00000041).
        /// </summary>
        public static readonly object Int32_00000041 = 65;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 65 (0x00000041).
        /// </summary>
        public static readonly Expression Int32_00000041_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000041");

        /// <summary>
        /// A singleton boxed 32-bin integer 66 (0x00000042).
        /// </summary>
        public static readonly object Int32_00000042 = 66;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 66 (0x00000042).
        /// </summary>
        public static readonly Expression Int32_00000042_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000042");

        /// <summary>
        /// A singleton boxed 32-bin integer 67 (0x00000043).
        /// </summary>
        public static readonly object Int32_00000043 = 67;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 67 (0x00000043).
        /// </summary>
        public static readonly Expression Int32_00000043_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000043");

        /// <summary>
        /// A singleton boxed 32-bin integer 68 (0x00000044).
        /// </summary>
        public static readonly object Int32_00000044 = 68;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 68 (0x00000044).
        /// </summary>
        public static readonly Expression Int32_00000044_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000044");

        /// <summary>
        /// A singleton boxed 32-bin integer 69 (0x00000045).
        /// </summary>
        public static readonly object Int32_00000045 = 69;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 69 (0x00000045).
        /// </summary>
        public static readonly Expression Int32_00000045_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000045");

        /// <summary>
        /// A singleton boxed 32-bin integer 70 (0x00000046).
        /// </summary>
        public static readonly object Int32_00000046 = 70;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 70 (0x00000046).
        /// </summary>
        public static readonly Expression Int32_00000046_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000046");

        /// <summary>
        /// A singleton boxed 32-bin integer 71 (0x00000047).
        /// </summary>
        public static readonly object Int32_00000047 = 71;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 71 (0x00000047).
        /// </summary>
        public static readonly Expression Int32_00000047_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000047");

        /// <summary>
        /// A singleton boxed 32-bin integer 72 (0x00000048).
        /// </summary>
        public static readonly object Int32_00000048 = 72;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 72 (0x00000048).
        /// </summary>
        public static readonly Expression Int32_00000048_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000048");

        /// <summary>
        /// A singleton boxed 32-bin integer 73 (0x00000049).
        /// </summary>
        public static readonly object Int32_00000049 = 73;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 73 (0x00000049).
        /// </summary>
        public static readonly Expression Int32_00000049_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000049");

        /// <summary>
        /// A singleton boxed 32-bin integer 74 (0x0000004A).
        /// </summary>
        public static readonly object Int32_0000004A = 74;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 74 (0x0000004A).
        /// </summary>
        public static readonly Expression Int32_0000004A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004A");

        /// <summary>
        /// A singleton boxed 32-bin integer 75 (0x0000004B).
        /// </summary>
        public static readonly object Int32_0000004B = 75;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 75 (0x0000004B).
        /// </summary>
        public static readonly Expression Int32_0000004B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004B");

        /// <summary>
        /// A singleton boxed 32-bin integer 76 (0x0000004C).
        /// </summary>
        public static readonly object Int32_0000004C = 76;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 76 (0x0000004C).
        /// </summary>
        public static readonly Expression Int32_0000004C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004C");

        /// <summary>
        /// A singleton boxed 32-bin integer 77 (0x0000004D).
        /// </summary>
        public static readonly object Int32_0000004D = 77;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 77 (0x0000004D).
        /// </summary>
        public static readonly Expression Int32_0000004D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004D");

        /// <summary>
        /// A singleton boxed 32-bin integer 78 (0x0000004E).
        /// </summary>
        public static readonly object Int32_0000004E = 78;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 78 (0x0000004E).
        /// </summary>
        public static readonly Expression Int32_0000004E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004E");

        /// <summary>
        /// A singleton boxed 32-bin integer 79 (0x0000004F).
        /// </summary>
        public static readonly object Int32_0000004F = 79;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 79 (0x0000004F).
        /// </summary>
        public static readonly Expression Int32_0000004F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000004F");

        /// <summary>
        /// A singleton boxed 32-bin integer 80 (0x00000050).
        /// </summary>
        public static readonly object Int32_00000050 = 80;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 80 (0x00000050).
        /// </summary>
        public static readonly Expression Int32_00000050_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000050");

        /// <summary>
        /// A singleton boxed 32-bin integer 81 (0x00000051).
        /// </summary>
        public static readonly object Int32_00000051 = 81;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 81 (0x00000051).
        /// </summary>
        public static readonly Expression Int32_00000051_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000051");

        /// <summary>
        /// A singleton boxed 32-bin integer 82 (0x00000052).
        /// </summary>
        public static readonly object Int32_00000052 = 82;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 82 (0x00000052).
        /// </summary>
        public static readonly Expression Int32_00000052_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000052");

        /// <summary>
        /// A singleton boxed 32-bin integer 83 (0x00000053).
        /// </summary>
        public static readonly object Int32_00000053 = 83;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 83 (0x00000053).
        /// </summary>
        public static readonly Expression Int32_00000053_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000053");

        /// <summary>
        /// A singleton boxed 32-bin integer 84 (0x00000054).
        /// </summary>
        public static readonly object Int32_00000054 = 84;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 84 (0x00000054).
        /// </summary>
        public static readonly Expression Int32_00000054_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000054");

        /// <summary>
        /// A singleton boxed 32-bin integer 85 (0x00000055).
        /// </summary>
        public static readonly object Int32_00000055 = 85;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 85 (0x00000055).
        /// </summary>
        public static readonly Expression Int32_00000055_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000055");

        /// <summary>
        /// A singleton boxed 32-bin integer 86 (0x00000056).
        /// </summary>
        public static readonly object Int32_00000056 = 86;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 86 (0x00000056).
        /// </summary>
        public static readonly Expression Int32_00000056_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000056");

        /// <summary>
        /// A singleton boxed 32-bin integer 87 (0x00000057).
        /// </summary>
        public static readonly object Int32_00000057 = 87;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 87 (0x00000057).
        /// </summary>
        public static readonly Expression Int32_00000057_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000057");

        /// <summary>
        /// A singleton boxed 32-bin integer 88 (0x00000058).
        /// </summary>
        public static readonly object Int32_00000058 = 88;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 88 (0x00000058).
        /// </summary>
        public static readonly Expression Int32_00000058_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000058");

        /// <summary>
        /// A singleton boxed 32-bin integer 89 (0x00000059).
        /// </summary>
        public static readonly object Int32_00000059 = 89;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 89 (0x00000059).
        /// </summary>
        public static readonly Expression Int32_00000059_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000059");

        /// <summary>
        /// A singleton boxed 32-bin integer 90 (0x0000005A).
        /// </summary>
        public static readonly object Int32_0000005A = 90;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 90 (0x0000005A).
        /// </summary>
        public static readonly Expression Int32_0000005A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005A");

        /// <summary>
        /// A singleton boxed 32-bin integer 91 (0x0000005B).
        /// </summary>
        public static readonly object Int32_0000005B = 91;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 91 (0x0000005B).
        /// </summary>
        public static readonly Expression Int32_0000005B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005B");

        /// <summary>
        /// A singleton boxed 32-bin integer 92 (0x0000005C).
        /// </summary>
        public static readonly object Int32_0000005C = 92;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 92 (0x0000005C).
        /// </summary>
        public static readonly Expression Int32_0000005C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005C");

        /// <summary>
        /// A singleton boxed 32-bin integer 93 (0x0000005D).
        /// </summary>
        public static readonly object Int32_0000005D = 93;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 93 (0x0000005D).
        /// </summary>
        public static readonly Expression Int32_0000005D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005D");

        /// <summary>
        /// A singleton boxed 32-bin integer 94 (0x0000005E).
        /// </summary>
        public static readonly object Int32_0000005E = 94;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 94 (0x0000005E).
        /// </summary>
        public static readonly Expression Int32_0000005E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005E");

        /// <summary>
        /// A singleton boxed 32-bin integer 95 (0x0000005F).
        /// </summary>
        public static readonly object Int32_0000005F = 95;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 95 (0x0000005F).
        /// </summary>
        public static readonly Expression Int32_0000005F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000005F");

        /// <summary>
        /// A singleton boxed 32-bin integer 96 (0x00000060).
        /// </summary>
        public static readonly object Int32_00000060 = 96;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 96 (0x00000060).
        /// </summary>
        public static readonly Expression Int32_00000060_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000060");

        /// <summary>
        /// A singleton boxed 32-bin integer 97 (0x00000061).
        /// </summary>
        public static readonly object Int32_00000061 = 97;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 97 (0x00000061).
        /// </summary>
        public static readonly Expression Int32_00000061_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000061");

        /// <summary>
        /// A singleton boxed 32-bin integer 98 (0x00000062).
        /// </summary>
        public static readonly object Int32_00000062 = 98;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 98 (0x00000062).
        /// </summary>
        public static readonly Expression Int32_00000062_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000062");

        /// <summary>
        /// A singleton boxed 32-bin integer 99 (0x00000063).
        /// </summary>
        public static readonly object Int32_00000063 = 99;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 99 (0x00000063).
        /// </summary>
        public static readonly Expression Int32_00000063_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000063");

        /// <summary>
        /// A singleton boxed 32-bin integer 100 (0x00000064).
        /// </summary>
        public static readonly object Int32_00000064 = 100;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 100 (0x00000064).
        /// </summary>
        public static readonly Expression Int32_00000064_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000064");

        /// <summary>
        /// A singleton boxed 32-bin integer 101 (0x00000065).
        /// </summary>
        public static readonly object Int32_00000065 = 101;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 101 (0x00000065).
        /// </summary>
        public static readonly Expression Int32_00000065_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000065");

        /// <summary>
        /// A singleton boxed 32-bin integer 102 (0x00000066).
        /// </summary>
        public static readonly object Int32_00000066 = 102;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 102 (0x00000066).
        /// </summary>
        public static readonly Expression Int32_00000066_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000066");

        /// <summary>
        /// A singleton boxed 32-bin integer 103 (0x00000067).
        /// </summary>
        public static readonly object Int32_00000067 = 103;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 103 (0x00000067).
        /// </summary>
        public static readonly Expression Int32_00000067_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000067");

        /// <summary>
        /// A singleton boxed 32-bin integer 104 (0x00000068).
        /// </summary>
        public static readonly object Int32_00000068 = 104;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 104 (0x00000068).
        /// </summary>
        public static readonly Expression Int32_00000068_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000068");

        /// <summary>
        /// A singleton boxed 32-bin integer 105 (0x00000069).
        /// </summary>
        public static readonly object Int32_00000069 = 105;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 105 (0x00000069).
        /// </summary>
        public static readonly Expression Int32_00000069_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000069");

        /// <summary>
        /// A singleton boxed 32-bin integer 106 (0x0000006A).
        /// </summary>
        public static readonly object Int32_0000006A = 106;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 106 (0x0000006A).
        /// </summary>
        public static readonly Expression Int32_0000006A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006A");

        /// <summary>
        /// A singleton boxed 32-bin integer 107 (0x0000006B).
        /// </summary>
        public static readonly object Int32_0000006B = 107;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 107 (0x0000006B).
        /// </summary>
        public static readonly Expression Int32_0000006B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006B");

        /// <summary>
        /// A singleton boxed 32-bin integer 108 (0x0000006C).
        /// </summary>
        public static readonly object Int32_0000006C = 108;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 108 (0x0000006C).
        /// </summary>
        public static readonly Expression Int32_0000006C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006C");

        /// <summary>
        /// A singleton boxed 32-bin integer 109 (0x0000006D).
        /// </summary>
        public static readonly object Int32_0000006D = 109;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 109 (0x0000006D).
        /// </summary>
        public static readonly Expression Int32_0000006D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006D");

        /// <summary>
        /// A singleton boxed 32-bin integer 110 (0x0000006E).
        /// </summary>
        public static readonly object Int32_0000006E = 110;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 110 (0x0000006E).
        /// </summary>
        public static readonly Expression Int32_0000006E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006E");

        /// <summary>
        /// A singleton boxed 32-bin integer 111 (0x0000006F).
        /// </summary>
        public static readonly object Int32_0000006F = 111;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 111 (0x0000006F).
        /// </summary>
        public static readonly Expression Int32_0000006F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000006F");

        /// <summary>
        /// A singleton boxed 32-bin integer 112 (0x00000070).
        /// </summary>
        public static readonly object Int32_00000070 = 112;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 112 (0x00000070).
        /// </summary>
        public static readonly Expression Int32_00000070_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000070");

        /// <summary>
        /// A singleton boxed 32-bin integer 113 (0x00000071).
        /// </summary>
        public static readonly object Int32_00000071 = 113;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 113 (0x00000071).
        /// </summary>
        public static readonly Expression Int32_00000071_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000071");

        /// <summary>
        /// A singleton boxed 32-bin integer 114 (0x00000072).
        /// </summary>
        public static readonly object Int32_00000072 = 114;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 114 (0x00000072).
        /// </summary>
        public static readonly Expression Int32_00000072_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000072");

        /// <summary>
        /// A singleton boxed 32-bin integer 115 (0x00000073).
        /// </summary>
        public static readonly object Int32_00000073 = 115;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 115 (0x00000073).
        /// </summary>
        public static readonly Expression Int32_00000073_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000073");

        /// <summary>
        /// A singleton boxed 32-bin integer 116 (0x00000074).
        /// </summary>
        public static readonly object Int32_00000074 = 116;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 116 (0x00000074).
        /// </summary>
        public static readonly Expression Int32_00000074_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000074");

        /// <summary>
        /// A singleton boxed 32-bin integer 117 (0x00000075).
        /// </summary>
        public static readonly object Int32_00000075 = 117;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 117 (0x00000075).
        /// </summary>
        public static readonly Expression Int32_00000075_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000075");

        /// <summary>
        /// A singleton boxed 32-bin integer 118 (0x00000076).
        /// </summary>
        public static readonly object Int32_00000076 = 118;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 118 (0x00000076).
        /// </summary>
        public static readonly Expression Int32_00000076_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000076");

        /// <summary>
        /// A singleton boxed 32-bin integer 119 (0x00000077).
        /// </summary>
        public static readonly object Int32_00000077 = 119;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 119 (0x00000077).
        /// </summary>
        public static readonly Expression Int32_00000077_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000077");

        /// <summary>
        /// A singleton boxed 32-bin integer 120 (0x00000078).
        /// </summary>
        public static readonly object Int32_00000078 = 120;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 120 (0x00000078).
        /// </summary>
        public static readonly Expression Int32_00000078_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000078");

        /// <summary>
        /// A singleton boxed 32-bin integer 121 (0x00000079).
        /// </summary>
        public static readonly object Int32_00000079 = 121;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 121 (0x00000079).
        /// </summary>
        public static readonly Expression Int32_00000079_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000079");

        /// <summary>
        /// A singleton boxed 32-bin integer 122 (0x0000007A).
        /// </summary>
        public static readonly object Int32_0000007A = 122;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 122 (0x0000007A).
        /// </summary>
        public static readonly Expression Int32_0000007A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007A");

        /// <summary>
        /// A singleton boxed 32-bin integer 123 (0x0000007B).
        /// </summary>
        public static readonly object Int32_0000007B = 123;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 123 (0x0000007B).
        /// </summary>
        public static readonly Expression Int32_0000007B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007B");

        /// <summary>
        /// A singleton boxed 32-bin integer 124 (0x0000007C).
        /// </summary>
        public static readonly object Int32_0000007C = 124;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 124 (0x0000007C).
        /// </summary>
        public static readonly Expression Int32_0000007C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007C");

        /// <summary>
        /// A singleton boxed 32-bin integer 125 (0x0000007D).
        /// </summary>
        public static readonly object Int32_0000007D = 125;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 125 (0x0000007D).
        /// </summary>
        public static readonly Expression Int32_0000007D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007D");

        /// <summary>
        /// A singleton boxed 32-bin integer 126 (0x0000007E).
        /// </summary>
        public static readonly object Int32_0000007E = 126;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 126 (0x0000007E).
        /// </summary>
        public static readonly Expression Int32_0000007E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007E");

        /// <summary>
        /// A singleton boxed 32-bin integer 127 (0x0000007F).
        /// </summary>
        public static readonly object Int32_0000007F = 127;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 127 (0x0000007F).
        /// </summary>
        public static readonly Expression Int32_0000007F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000007F");

        /// <summary>
        /// A singleton boxed 32-bin integer 128 (0x00000080).
        /// </summary>
        public static readonly object Int32_00000080 = 128;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 128 (0x00000080).
        /// </summary>
        public static readonly Expression Int32_00000080_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000080");

        /// <summary>
        /// A singleton boxed 32-bin integer 129 (0x00000081).
        /// </summary>
        public static readonly object Int32_00000081 = 129;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 129 (0x00000081).
        /// </summary>
        public static readonly Expression Int32_00000081_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000081");

        /// <summary>
        /// A singleton boxed 32-bin integer 130 (0x00000082).
        /// </summary>
        public static readonly object Int32_00000082 = 130;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 130 (0x00000082).
        /// </summary>
        public static readonly Expression Int32_00000082_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000082");

        /// <summary>
        /// A singleton boxed 32-bin integer 131 (0x00000083).
        /// </summary>
        public static readonly object Int32_00000083 = 131;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 131 (0x00000083).
        /// </summary>
        public static readonly Expression Int32_00000083_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000083");

        /// <summary>
        /// A singleton boxed 32-bin integer 132 (0x00000084).
        /// </summary>
        public static readonly object Int32_00000084 = 132;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 132 (0x00000084).
        /// </summary>
        public static readonly Expression Int32_00000084_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000084");

        /// <summary>
        /// A singleton boxed 32-bin integer 133 (0x00000085).
        /// </summary>
        public static readonly object Int32_00000085 = 133;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 133 (0x00000085).
        /// </summary>
        public static readonly Expression Int32_00000085_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000085");

        /// <summary>
        /// A singleton boxed 32-bin integer 134 (0x00000086).
        /// </summary>
        public static readonly object Int32_00000086 = 134;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 134 (0x00000086).
        /// </summary>
        public static readonly Expression Int32_00000086_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000086");

        /// <summary>
        /// A singleton boxed 32-bin integer 135 (0x00000087).
        /// </summary>
        public static readonly object Int32_00000087 = 135;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 135 (0x00000087).
        /// </summary>
        public static readonly Expression Int32_00000087_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000087");

        /// <summary>
        /// A singleton boxed 32-bin integer 136 (0x00000088).
        /// </summary>
        public static readonly object Int32_00000088 = 136;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 136 (0x00000088).
        /// </summary>
        public static readonly Expression Int32_00000088_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000088");

        /// <summary>
        /// A singleton boxed 32-bin integer 137 (0x00000089).
        /// </summary>
        public static readonly object Int32_00000089 = 137;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 137 (0x00000089).
        /// </summary>
        public static readonly Expression Int32_00000089_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000089");

        /// <summary>
        /// A singleton boxed 32-bin integer 138 (0x0000008A).
        /// </summary>
        public static readonly object Int32_0000008A = 138;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 138 (0x0000008A).
        /// </summary>
        public static readonly Expression Int32_0000008A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008A");

        /// <summary>
        /// A singleton boxed 32-bin integer 139 (0x0000008B).
        /// </summary>
        public static readonly object Int32_0000008B = 139;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 139 (0x0000008B).
        /// </summary>
        public static readonly Expression Int32_0000008B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008B");

        /// <summary>
        /// A singleton boxed 32-bin integer 140 (0x0000008C).
        /// </summary>
        public static readonly object Int32_0000008C = 140;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 140 (0x0000008C).
        /// </summary>
        public static readonly Expression Int32_0000008C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008C");

        /// <summary>
        /// A singleton boxed 32-bin integer 141 (0x0000008D).
        /// </summary>
        public static readonly object Int32_0000008D = 141;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 141 (0x0000008D).
        /// </summary>
        public static readonly Expression Int32_0000008D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008D");

        /// <summary>
        /// A singleton boxed 32-bin integer 142 (0x0000008E).
        /// </summary>
        public static readonly object Int32_0000008E = 142;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 142 (0x0000008E).
        /// </summary>
        public static readonly Expression Int32_0000008E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008E");

        /// <summary>
        /// A singleton boxed 32-bin integer 143 (0x0000008F).
        /// </summary>
        public static readonly object Int32_0000008F = 143;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 143 (0x0000008F).
        /// </summary>
        public static readonly Expression Int32_0000008F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000008F");

        /// <summary>
        /// A singleton boxed 32-bin integer 144 (0x00000090).
        /// </summary>
        public static readonly object Int32_00000090 = 144;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 144 (0x00000090).
        /// </summary>
        public static readonly Expression Int32_00000090_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000090");

        /// <summary>
        /// A singleton boxed 32-bin integer 145 (0x00000091).
        /// </summary>
        public static readonly object Int32_00000091 = 145;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 145 (0x00000091).
        /// </summary>
        public static readonly Expression Int32_00000091_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000091");

        /// <summary>
        /// A singleton boxed 32-bin integer 146 (0x00000092).
        /// </summary>
        public static readonly object Int32_00000092 = 146;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 146 (0x00000092).
        /// </summary>
        public static readonly Expression Int32_00000092_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000092");

        /// <summary>
        /// A singleton boxed 32-bin integer 147 (0x00000093).
        /// </summary>
        public static readonly object Int32_00000093 = 147;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 147 (0x00000093).
        /// </summary>
        public static readonly Expression Int32_00000093_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000093");

        /// <summary>
        /// A singleton boxed 32-bin integer 148 (0x00000094).
        /// </summary>
        public static readonly object Int32_00000094 = 148;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 148 (0x00000094).
        /// </summary>
        public static readonly Expression Int32_00000094_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000094");

        /// <summary>
        /// A singleton boxed 32-bin integer 149 (0x00000095).
        /// </summary>
        public static readonly object Int32_00000095 = 149;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 149 (0x00000095).
        /// </summary>
        public static readonly Expression Int32_00000095_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000095");

        /// <summary>
        /// A singleton boxed 32-bin integer 150 (0x00000096).
        /// </summary>
        public static readonly object Int32_00000096 = 150;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 150 (0x00000096).
        /// </summary>
        public static readonly Expression Int32_00000096_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000096");

        /// <summary>
        /// A singleton boxed 32-bin integer 151 (0x00000097).
        /// </summary>
        public static readonly object Int32_00000097 = 151;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 151 (0x00000097).
        /// </summary>
        public static readonly Expression Int32_00000097_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000097");

        /// <summary>
        /// A singleton boxed 32-bin integer 152 (0x00000098).
        /// </summary>
        public static readonly object Int32_00000098 = 152;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 152 (0x00000098).
        /// </summary>
        public static readonly Expression Int32_00000098_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000098");

        /// <summary>
        /// A singleton boxed 32-bin integer 153 (0x00000099).
        /// </summary>
        public static readonly object Int32_00000099 = 153;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 153 (0x00000099).
        /// </summary>
        public static readonly Expression Int32_00000099_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_00000099");

        /// <summary>
        /// A singleton boxed 32-bin integer 154 (0x0000009A).
        /// </summary>
        public static readonly object Int32_0000009A = 154;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 154 (0x0000009A).
        /// </summary>
        public static readonly Expression Int32_0000009A_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009A");

        /// <summary>
        /// A singleton boxed 32-bin integer 155 (0x0000009B).
        /// </summary>
        public static readonly object Int32_0000009B = 155;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 155 (0x0000009B).
        /// </summary>
        public static readonly Expression Int32_0000009B_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009B");

        /// <summary>
        /// A singleton boxed 32-bin integer 156 (0x0000009C).
        /// </summary>
        public static readonly object Int32_0000009C = 156;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 156 (0x0000009C).
        /// </summary>
        public static readonly Expression Int32_0000009C_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009C");

        /// <summary>
        /// A singleton boxed 32-bin integer 157 (0x0000009D).
        /// </summary>
        public static readonly object Int32_0000009D = 157;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 157 (0x0000009D).
        /// </summary>
        public static readonly Expression Int32_0000009D_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009D");

        /// <summary>
        /// A singleton boxed 32-bin integer 158 (0x0000009E).
        /// </summary>
        public static readonly object Int32_0000009E = 158;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 158 (0x0000009E).
        /// </summary>
        public static readonly Expression Int32_0000009E_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009E");

        /// <summary>
        /// A singleton boxed 32-bin integer 159 (0x0000009F).
        /// </summary>
        public static readonly object Int32_0000009F = 159;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 159 (0x0000009F).
        /// </summary>
        public static readonly Expression Int32_0000009F_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_0000009F");

        /// <summary>
        /// A singleton boxed 32-bin integer 160 (0x000000A0).
        /// </summary>
        public static readonly object Int32_000000A0 = 160;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 160 (0x000000A0).
        /// </summary>
        public static readonly Expression Int32_000000A0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A0");

        /// <summary>
        /// A singleton boxed 32-bin integer 161 (0x000000A1).
        /// </summary>
        public static readonly object Int32_000000A1 = 161;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 161 (0x000000A1).
        /// </summary>
        public static readonly Expression Int32_000000A1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A1");

        /// <summary>
        /// A singleton boxed 32-bin integer 162 (0x000000A2).
        /// </summary>
        public static readonly object Int32_000000A2 = 162;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 162 (0x000000A2).
        /// </summary>
        public static readonly Expression Int32_000000A2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A2");

        /// <summary>
        /// A singleton boxed 32-bin integer 163 (0x000000A3).
        /// </summary>
        public static readonly object Int32_000000A3 = 163;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 163 (0x000000A3).
        /// </summary>
        public static readonly Expression Int32_000000A3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A3");

        /// <summary>
        /// A singleton boxed 32-bin integer 164 (0x000000A4).
        /// </summary>
        public static readonly object Int32_000000A4 = 164;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 164 (0x000000A4).
        /// </summary>
        public static readonly Expression Int32_000000A4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A4");

        /// <summary>
        /// A singleton boxed 32-bin integer 165 (0x000000A5).
        /// </summary>
        public static readonly object Int32_000000A5 = 165;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 165 (0x000000A5).
        /// </summary>
        public static readonly Expression Int32_000000A5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A5");

        /// <summary>
        /// A singleton boxed 32-bin integer 166 (0x000000A6).
        /// </summary>
        public static readonly object Int32_000000A6 = 166;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 166 (0x000000A6).
        /// </summary>
        public static readonly Expression Int32_000000A6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A6");

        /// <summary>
        /// A singleton boxed 32-bin integer 167 (0x000000A7).
        /// </summary>
        public static readonly object Int32_000000A7 = 167;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 167 (0x000000A7).
        /// </summary>
        public static readonly Expression Int32_000000A7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A7");

        /// <summary>
        /// A singleton boxed 32-bin integer 168 (0x000000A8).
        /// </summary>
        public static readonly object Int32_000000A8 = 168;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 168 (0x000000A8).
        /// </summary>
        public static readonly Expression Int32_000000A8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A8");

        /// <summary>
        /// A singleton boxed 32-bin integer 169 (0x000000A9).
        /// </summary>
        public static readonly object Int32_000000A9 = 169;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 169 (0x000000A9).
        /// </summary>
        public static readonly Expression Int32_000000A9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000A9");

        /// <summary>
        /// A singleton boxed 32-bin integer 170 (0x000000AA).
        /// </summary>
        public static readonly object Int32_000000AA = 170;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 170 (0x000000AA).
        /// </summary>
        public static readonly Expression Int32_000000AA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AA");

        /// <summary>
        /// A singleton boxed 32-bin integer 171 (0x000000AB).
        /// </summary>
        public static readonly object Int32_000000AB = 171;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 171 (0x000000AB).
        /// </summary>
        public static readonly Expression Int32_000000AB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AB");

        /// <summary>
        /// A singleton boxed 32-bin integer 172 (0x000000AC).
        /// </summary>
        public static readonly object Int32_000000AC = 172;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 172 (0x000000AC).
        /// </summary>
        public static readonly Expression Int32_000000AC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AC");

        /// <summary>
        /// A singleton boxed 32-bin integer 173 (0x000000AD).
        /// </summary>
        public static readonly object Int32_000000AD = 173;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 173 (0x000000AD).
        /// </summary>
        public static readonly Expression Int32_000000AD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AD");

        /// <summary>
        /// A singleton boxed 32-bin integer 174 (0x000000AE).
        /// </summary>
        public static readonly object Int32_000000AE = 174;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 174 (0x000000AE).
        /// </summary>
        public static readonly Expression Int32_000000AE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AE");

        /// <summary>
        /// A singleton boxed 32-bin integer 175 (0x000000AF).
        /// </summary>
        public static readonly object Int32_000000AF = 175;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 175 (0x000000AF).
        /// </summary>
        public static readonly Expression Int32_000000AF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000AF");

        /// <summary>
        /// A singleton boxed 32-bin integer 176 (0x000000B0).
        /// </summary>
        public static readonly object Int32_000000B0 = 176;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 176 (0x000000B0).
        /// </summary>
        public static readonly Expression Int32_000000B0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B0");

        /// <summary>
        /// A singleton boxed 32-bin integer 177 (0x000000B1).
        /// </summary>
        public static readonly object Int32_000000B1 = 177;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 177 (0x000000B1).
        /// </summary>
        public static readonly Expression Int32_000000B1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B1");

        /// <summary>
        /// A singleton boxed 32-bin integer 178 (0x000000B2).
        /// </summary>
        public static readonly object Int32_000000B2 = 178;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 178 (0x000000B2).
        /// </summary>
        public static readonly Expression Int32_000000B2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B2");

        /// <summary>
        /// A singleton boxed 32-bin integer 179 (0x000000B3).
        /// </summary>
        public static readonly object Int32_000000B3 = 179;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 179 (0x000000B3).
        /// </summary>
        public static readonly Expression Int32_000000B3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B3");

        /// <summary>
        /// A singleton boxed 32-bin integer 180 (0x000000B4).
        /// </summary>
        public static readonly object Int32_000000B4 = 180;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 180 (0x000000B4).
        /// </summary>
        public static readonly Expression Int32_000000B4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B4");

        /// <summary>
        /// A singleton boxed 32-bin integer 181 (0x000000B5).
        /// </summary>
        public static readonly object Int32_000000B5 = 181;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 181 (0x000000B5).
        /// </summary>
        public static readonly Expression Int32_000000B5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B5");

        /// <summary>
        /// A singleton boxed 32-bin integer 182 (0x000000B6).
        /// </summary>
        public static readonly object Int32_000000B6 = 182;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 182 (0x000000B6).
        /// </summary>
        public static readonly Expression Int32_000000B6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B6");

        /// <summary>
        /// A singleton boxed 32-bin integer 183 (0x000000B7).
        /// </summary>
        public static readonly object Int32_000000B7 = 183;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 183 (0x000000B7).
        /// </summary>
        public static readonly Expression Int32_000000B7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B7");

        /// <summary>
        /// A singleton boxed 32-bin integer 184 (0x000000B8).
        /// </summary>
        public static readonly object Int32_000000B8 = 184;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 184 (0x000000B8).
        /// </summary>
        public static readonly Expression Int32_000000B8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B8");

        /// <summary>
        /// A singleton boxed 32-bin integer 185 (0x000000B9).
        /// </summary>
        public static readonly object Int32_000000B9 = 185;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 185 (0x000000B9).
        /// </summary>
        public static readonly Expression Int32_000000B9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000B9");

        /// <summary>
        /// A singleton boxed 32-bin integer 186 (0x000000BA).
        /// </summary>
        public static readonly object Int32_000000BA = 186;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 186 (0x000000BA).
        /// </summary>
        public static readonly Expression Int32_000000BA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BA");

        /// <summary>
        /// A singleton boxed 32-bin integer 187 (0x000000BB).
        /// </summary>
        public static readonly object Int32_000000BB = 187;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 187 (0x000000BB).
        /// </summary>
        public static readonly Expression Int32_000000BB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BB");

        /// <summary>
        /// A singleton boxed 32-bin integer 188 (0x000000BC).
        /// </summary>
        public static readonly object Int32_000000BC = 188;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 188 (0x000000BC).
        /// </summary>
        public static readonly Expression Int32_000000BC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BC");

        /// <summary>
        /// A singleton boxed 32-bin integer 189 (0x000000BD).
        /// </summary>
        public static readonly object Int32_000000BD = 189;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 189 (0x000000BD).
        /// </summary>
        public static readonly Expression Int32_000000BD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BD");

        /// <summary>
        /// A singleton boxed 32-bin integer 190 (0x000000BE).
        /// </summary>
        public static readonly object Int32_000000BE = 190;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 190 (0x000000BE).
        /// </summary>
        public static readonly Expression Int32_000000BE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BE");

        /// <summary>
        /// A singleton boxed 32-bin integer 191 (0x000000BF).
        /// </summary>
        public static readonly object Int32_000000BF = 191;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 191 (0x000000BF).
        /// </summary>
        public static readonly Expression Int32_000000BF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000BF");

        /// <summary>
        /// A singleton boxed 32-bin integer 192 (0x000000C0).
        /// </summary>
        public static readonly object Int32_000000C0 = 192;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 192 (0x000000C0).
        /// </summary>
        public static readonly Expression Int32_000000C0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C0");

        /// <summary>
        /// A singleton boxed 32-bin integer 193 (0x000000C1).
        /// </summary>
        public static readonly object Int32_000000C1 = 193;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 193 (0x000000C1).
        /// </summary>
        public static readonly Expression Int32_000000C1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C1");

        /// <summary>
        /// A singleton boxed 32-bin integer 194 (0x000000C2).
        /// </summary>
        public static readonly object Int32_000000C2 = 194;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 194 (0x000000C2).
        /// </summary>
        public static readonly Expression Int32_000000C2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C2");

        /// <summary>
        /// A singleton boxed 32-bin integer 195 (0x000000C3).
        /// </summary>
        public static readonly object Int32_000000C3 = 195;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 195 (0x000000C3).
        /// </summary>
        public static readonly Expression Int32_000000C3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C3");

        /// <summary>
        /// A singleton boxed 32-bin integer 196 (0x000000C4).
        /// </summary>
        public static readonly object Int32_000000C4 = 196;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 196 (0x000000C4).
        /// </summary>
        public static readonly Expression Int32_000000C4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C4");

        /// <summary>
        /// A singleton boxed 32-bin integer 197 (0x000000C5).
        /// </summary>
        public static readonly object Int32_000000C5 = 197;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 197 (0x000000C5).
        /// </summary>
        public static readonly Expression Int32_000000C5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C5");

        /// <summary>
        /// A singleton boxed 32-bin integer 198 (0x000000C6).
        /// </summary>
        public static readonly object Int32_000000C6 = 198;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 198 (0x000000C6).
        /// </summary>
        public static readonly Expression Int32_000000C6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C6");

        /// <summary>
        /// A singleton boxed 32-bin integer 199 (0x000000C7).
        /// </summary>
        public static readonly object Int32_000000C7 = 199;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 199 (0x000000C7).
        /// </summary>
        public static readonly Expression Int32_000000C7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C7");

        /// <summary>
        /// A singleton boxed 32-bin integer 200 (0x000000C8).
        /// </summary>
        public static readonly object Int32_000000C8 = 200;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 200 (0x000000C8).
        /// </summary>
        public static readonly Expression Int32_000000C8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C8");

        /// <summary>
        /// A singleton boxed 32-bin integer 201 (0x000000C9).
        /// </summary>
        public static readonly object Int32_000000C9 = 201;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 201 (0x000000C9).
        /// </summary>
        public static readonly Expression Int32_000000C9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000C9");

        /// <summary>
        /// A singleton boxed 32-bin integer 202 (0x000000CA).
        /// </summary>
        public static readonly object Int32_000000CA = 202;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 202 (0x000000CA).
        /// </summary>
        public static readonly Expression Int32_000000CA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CA");

        /// <summary>
        /// A singleton boxed 32-bin integer 203 (0x000000CB).
        /// </summary>
        public static readonly object Int32_000000CB = 203;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 203 (0x000000CB).
        /// </summary>
        public static readonly Expression Int32_000000CB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CB");

        /// <summary>
        /// A singleton boxed 32-bin integer 204 (0x000000CC).
        /// </summary>
        public static readonly object Int32_000000CC = 204;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 204 (0x000000CC).
        /// </summary>
        public static readonly Expression Int32_000000CC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CC");

        /// <summary>
        /// A singleton boxed 32-bin integer 205 (0x000000CD).
        /// </summary>
        public static readonly object Int32_000000CD = 205;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 205 (0x000000CD).
        /// </summary>
        public static readonly Expression Int32_000000CD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CD");

        /// <summary>
        /// A singleton boxed 32-bin integer 206 (0x000000CE).
        /// </summary>
        public static readonly object Int32_000000CE = 206;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 206 (0x000000CE).
        /// </summary>
        public static readonly Expression Int32_000000CE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CE");

        /// <summary>
        /// A singleton boxed 32-bin integer 207 (0x000000CF).
        /// </summary>
        public static readonly object Int32_000000CF = 207;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 207 (0x000000CF).
        /// </summary>
        public static readonly Expression Int32_000000CF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000CF");

        /// <summary>
        /// A singleton boxed 32-bin integer 208 (0x000000D0).
        /// </summary>
        public static readonly object Int32_000000D0 = 208;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 208 (0x000000D0).
        /// </summary>
        public static readonly Expression Int32_000000D0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D0");

        /// <summary>
        /// A singleton boxed 32-bin integer 209 (0x000000D1).
        /// </summary>
        public static readonly object Int32_000000D1 = 209;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 209 (0x000000D1).
        /// </summary>
        public static readonly Expression Int32_000000D1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D1");

        /// <summary>
        /// A singleton boxed 32-bin integer 210 (0x000000D2).
        /// </summary>
        public static readonly object Int32_000000D2 = 210;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 210 (0x000000D2).
        /// </summary>
        public static readonly Expression Int32_000000D2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D2");

        /// <summary>
        /// A singleton boxed 32-bin integer 211 (0x000000D3).
        /// </summary>
        public static readonly object Int32_000000D3 = 211;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 211 (0x000000D3).
        /// </summary>
        public static readonly Expression Int32_000000D3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D3");

        /// <summary>
        /// A singleton boxed 32-bin integer 212 (0x000000D4).
        /// </summary>
        public static readonly object Int32_000000D4 = 212;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 212 (0x000000D4).
        /// </summary>
        public static readonly Expression Int32_000000D4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D4");

        /// <summary>
        /// A singleton boxed 32-bin integer 213 (0x000000D5).
        /// </summary>
        public static readonly object Int32_000000D5 = 213;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 213 (0x000000D5).
        /// </summary>
        public static readonly Expression Int32_000000D5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D5");

        /// <summary>
        /// A singleton boxed 32-bin integer 214 (0x000000D6).
        /// </summary>
        public static readonly object Int32_000000D6 = 214;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 214 (0x000000D6).
        /// </summary>
        public static readonly Expression Int32_000000D6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D6");

        /// <summary>
        /// A singleton boxed 32-bin integer 215 (0x000000D7).
        /// </summary>
        public static readonly object Int32_000000D7 = 215;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 215 (0x000000D7).
        /// </summary>
        public static readonly Expression Int32_000000D7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D7");

        /// <summary>
        /// A singleton boxed 32-bin integer 216 (0x000000D8).
        /// </summary>
        public static readonly object Int32_000000D8 = 216;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 216 (0x000000D8).
        /// </summary>
        public static readonly Expression Int32_000000D8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D8");

        /// <summary>
        /// A singleton boxed 32-bin integer 217 (0x000000D9).
        /// </summary>
        public static readonly object Int32_000000D9 = 217;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 217 (0x000000D9).
        /// </summary>
        public static readonly Expression Int32_000000D9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000D9");

        /// <summary>
        /// A singleton boxed 32-bin integer 218 (0x000000DA).
        /// </summary>
        public static readonly object Int32_000000DA = 218;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 218 (0x000000DA).
        /// </summary>
        public static readonly Expression Int32_000000DA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DA");

        /// <summary>
        /// A singleton boxed 32-bin integer 219 (0x000000DB).
        /// </summary>
        public static readonly object Int32_000000DB = 219;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 219 (0x000000DB).
        /// </summary>
        public static readonly Expression Int32_000000DB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DB");

        /// <summary>
        /// A singleton boxed 32-bin integer 220 (0x000000DC).
        /// </summary>
        public static readonly object Int32_000000DC = 220;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 220 (0x000000DC).
        /// </summary>
        public static readonly Expression Int32_000000DC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DC");

        /// <summary>
        /// A singleton boxed 32-bin integer 221 (0x000000DD).
        /// </summary>
        public static readonly object Int32_000000DD = 221;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 221 (0x000000DD).
        /// </summary>
        public static readonly Expression Int32_000000DD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DD");

        /// <summary>
        /// A singleton boxed 32-bin integer 222 (0x000000DE).
        /// </summary>
        public static readonly object Int32_000000DE = 222;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 222 (0x000000DE).
        /// </summary>
        public static readonly Expression Int32_000000DE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DE");

        /// <summary>
        /// A singleton boxed 32-bin integer 223 (0x000000DF).
        /// </summary>
        public static readonly object Int32_000000DF = 223;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 223 (0x000000DF).
        /// </summary>
        public static readonly Expression Int32_000000DF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000DF");

        /// <summary>
        /// A singleton boxed 32-bin integer 224 (0x000000E0).
        /// </summary>
        public static readonly object Int32_000000E0 = 224;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 224 (0x000000E0).
        /// </summary>
        public static readonly Expression Int32_000000E0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E0");

        /// <summary>
        /// A singleton boxed 32-bin integer 225 (0x000000E1).
        /// </summary>
        public static readonly object Int32_000000E1 = 225;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 225 (0x000000E1).
        /// </summary>
        public static readonly Expression Int32_000000E1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E1");

        /// <summary>
        /// A singleton boxed 32-bin integer 226 (0x000000E2).
        /// </summary>
        public static readonly object Int32_000000E2 = 226;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 226 (0x000000E2).
        /// </summary>
        public static readonly Expression Int32_000000E2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E2");

        /// <summary>
        /// A singleton boxed 32-bin integer 227 (0x000000E3).
        /// </summary>
        public static readonly object Int32_000000E3 = 227;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 227 (0x000000E3).
        /// </summary>
        public static readonly Expression Int32_000000E3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E3");

        /// <summary>
        /// A singleton boxed 32-bin integer 228 (0x000000E4).
        /// </summary>
        public static readonly object Int32_000000E4 = 228;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 228 (0x000000E4).
        /// </summary>
        public static readonly Expression Int32_000000E4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E4");

        /// <summary>
        /// A singleton boxed 32-bin integer 229 (0x000000E5).
        /// </summary>
        public static readonly object Int32_000000E5 = 229;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 229 (0x000000E5).
        /// </summary>
        public static readonly Expression Int32_000000E5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E5");

        /// <summary>
        /// A singleton boxed 32-bin integer 230 (0x000000E6).
        /// </summary>
        public static readonly object Int32_000000E6 = 230;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 230 (0x000000E6).
        /// </summary>
        public static readonly Expression Int32_000000E6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E6");

        /// <summary>
        /// A singleton boxed 32-bin integer 231 (0x000000E7).
        /// </summary>
        public static readonly object Int32_000000E7 = 231;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 231 (0x000000E7).
        /// </summary>
        public static readonly Expression Int32_000000E7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E7");

        /// <summary>
        /// A singleton boxed 32-bin integer 232 (0x000000E8).
        /// </summary>
        public static readonly object Int32_000000E8 = 232;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 232 (0x000000E8).
        /// </summary>
        public static readonly Expression Int32_000000E8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E8");

        /// <summary>
        /// A singleton boxed 32-bin integer 233 (0x000000E9).
        /// </summary>
        public static readonly object Int32_000000E9 = 233;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 233 (0x000000E9).
        /// </summary>
        public static readonly Expression Int32_000000E9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000E9");

        /// <summary>
        /// A singleton boxed 32-bin integer 234 (0x000000EA).
        /// </summary>
        public static readonly object Int32_000000EA = 234;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 234 (0x000000EA).
        /// </summary>
        public static readonly Expression Int32_000000EA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000EA");

        /// <summary>
        /// A singleton boxed 32-bin integer 235 (0x000000EB).
        /// </summary>
        public static readonly object Int32_000000EB = 235;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 235 (0x000000EB).
        /// </summary>
        public static readonly Expression Int32_000000EB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000EB");

        /// <summary>
        /// A singleton boxed 32-bin integer 236 (0x000000EC).
        /// </summary>
        public static readonly object Int32_000000EC = 236;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 236 (0x000000EC).
        /// </summary>
        public static readonly Expression Int32_000000EC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000EC");

        /// <summary>
        /// A singleton boxed 32-bin integer 237 (0x000000ED).
        /// </summary>
        public static readonly object Int32_000000ED = 237;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 237 (0x000000ED).
        /// </summary>
        public static readonly Expression Int32_000000ED_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000ED");

        /// <summary>
        /// A singleton boxed 32-bin integer 238 (0x000000EE).
        /// </summary>
        public static readonly object Int32_000000EE = 238;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 238 (0x000000EE).
        /// </summary>
        public static readonly Expression Int32_000000EE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000EE");

        /// <summary>
        /// A singleton boxed 32-bin integer 239 (0x000000EF).
        /// </summary>
        public static readonly object Int32_000000EF = 239;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 239 (0x000000EF).
        /// </summary>
        public static readonly Expression Int32_000000EF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000EF");

        /// <summary>
        /// A singleton boxed 32-bin integer 240 (0x000000F0).
        /// </summary>
        public static readonly object Int32_000000F0 = 240;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 240 (0x000000F0).
        /// </summary>
        public static readonly Expression Int32_000000F0_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F0");

        /// <summary>
        /// A singleton boxed 32-bin integer 241 (0x000000F1).
        /// </summary>
        public static readonly object Int32_000000F1 = 241;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 241 (0x000000F1).
        /// </summary>
        public static readonly Expression Int32_000000F1_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F1");

        /// <summary>
        /// A singleton boxed 32-bin integer 242 (0x000000F2).
        /// </summary>
        public static readonly object Int32_000000F2 = 242;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 242 (0x000000F2).
        /// </summary>
        public static readonly Expression Int32_000000F2_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F2");

        /// <summary>
        /// A singleton boxed 32-bin integer 243 (0x000000F3).
        /// </summary>
        public static readonly object Int32_000000F3 = 243;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 243 (0x000000F3).
        /// </summary>
        public static readonly Expression Int32_000000F3_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F3");

        /// <summary>
        /// A singleton boxed 32-bin integer 244 (0x000000F4).
        /// </summary>
        public static readonly object Int32_000000F4 = 244;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 244 (0x000000F4).
        /// </summary>
        public static readonly Expression Int32_000000F4_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F4");

        /// <summary>
        /// A singleton boxed 32-bin integer 245 (0x000000F5).
        /// </summary>
        public static readonly object Int32_000000F5 = 245;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 245 (0x000000F5).
        /// </summary>
        public static readonly Expression Int32_000000F5_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F5");

        /// <summary>
        /// A singleton boxed 32-bin integer 246 (0x000000F6).
        /// </summary>
        public static readonly object Int32_000000F6 = 246;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 246 (0x000000F6).
        /// </summary>
        public static readonly Expression Int32_000000F6_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F6");

        /// <summary>
        /// A singleton boxed 32-bin integer 247 (0x000000F7).
        /// </summary>
        public static readonly object Int32_000000F7 = 247;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 247 (0x000000F7).
        /// </summary>
        public static readonly Expression Int32_000000F7_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F7");

        /// <summary>
        /// A singleton boxed 32-bin integer 248 (0x000000F8).
        /// </summary>
        public static readonly object Int32_000000F8 = 248;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 248 (0x000000F8).
        /// </summary>
        public static readonly Expression Int32_000000F8_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F8");

        /// <summary>
        /// A singleton boxed 32-bin integer 249 (0x000000F9).
        /// </summary>
        public static readonly object Int32_000000F9 = 249;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 249 (0x000000F9).
        /// </summary>
        public static readonly Expression Int32_000000F9_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000F9");

        /// <summary>
        /// A singleton boxed 32-bin integer 250 (0x000000FA).
        /// </summary>
        public static readonly object Int32_000000FA = 250;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 250 (0x000000FA).
        /// </summary>
        public static readonly Expression Int32_000000FA_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FA");

        /// <summary>
        /// A singleton boxed 32-bin integer 251 (0x000000FB).
        /// </summary>
        public static readonly object Int32_000000FB = 251;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 251 (0x000000FB).
        /// </summary>
        public static readonly Expression Int32_000000FB_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FB");

        /// <summary>
        /// A singleton boxed 32-bin integer 252 (0x000000FC).
        /// </summary>
        public static readonly object Int32_000000FC = 252;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 252 (0x000000FC).
        /// </summary>
        public static readonly Expression Int32_000000FC_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FC");

        /// <summary>
        /// A singleton boxed 32-bin integer 253 (0x000000FD).
        /// </summary>
        public static readonly object Int32_000000FD = 253;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 253 (0x000000FD).
        /// </summary>
        public static readonly Expression Int32_000000FD_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FD");

        /// <summary>
        /// A singleton boxed 32-bin integer 254 (0x000000FE).
        /// </summary>
        public static readonly object Int32_000000FE = 254;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 254 (0x000000FE).
        /// </summary>
        public static readonly Expression Int32_000000FE_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FE");

        /// <summary>
        /// A singleton boxed 32-bin integer 255 (0x000000FF).
        /// </summary>
        public static readonly object Int32_000000FF = 255;

        /// <summary>
        /// Expression that returns the singleton boxed 32-bin integer 255 (0x000000FF).
        /// </summary>
        public static readonly Expression Int32_000000FF_Expression = Expression.Field(null, typeof(PreboxedConstants), "Int32_000000FF");


        #endregion

        #region BigInteger

        public static Expression GetConstant(BigInteger value)
        {
            if (value == BigInteger.Zero)
                return PreboxedConstants.BigInteger_Zero_Expression;
            if (value == BigInteger.One)
                return PreboxedConstants.BigInteger_One_Expression;
            if (value == BigInteger.MinusOne)
                return PreboxedConstants.BigInteger_MinusOne_Expression;
            return null;
        }

        public static object GetValue(BigInteger value)
        {
            if (value == BigInteger.Zero)
                return PreboxedConstants.BigInteger_Zero;
            if (value == BigInteger.One)
                return PreboxedConstants.BigInteger_One;
            if (value == BigInteger.MinusOne)
                return PreboxedConstants.BigInteger_MinusOne;
            return null;
        }

        /// <summary>
        /// A singleton boxed BigInteger 0.
        /// </summary>
        public static readonly object BigInteger_Zero = BigInteger.Zero;

        /// <summary>
        /// Expression to return the singleton boxed BigInteger 0.
        /// </summary>
        public static readonly Expression BigInteger_Zero_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigInteger_Zero");

        /// <summary>
        /// A singleton boxed BigInteger 1.
        /// </summary>
        public static readonly object BigInteger_One = BigInteger.One;

        /// <summary>
        /// Expression to return the singleton boxed BigInteger 1.
        /// </summary>
        public static readonly Expression BigInteger_One_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigInteger_One");

        /// <summary>
        /// A singleton boxed BigInteger -1.
        /// </summary>
        public static readonly object BigInteger_MinusOne = BigInteger.MinusOne;

        /// <summary>
        /// Expression to return the singleton boxed BigInteger -1.
        /// </summary>
        public static readonly Expression BigInteger_MinusOne_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigInteger_MinusOne");

        #endregion

        #region BigDecimal

        public static Expression GetConstant(BigDecimal value)
        {
            if (value == BigDecimal.Zero)
                return PreboxedConstants.BigDecimal_Zero_Expression;
            if (value == BigDecimal.One)
                return PreboxedConstants.BigDecimal_One_Expression;
            if (value == BigDecimal.MinusOne)
                return PreboxedConstants.BigDecimal_MinusOne_Expression;
            return null;
        }

        public static object GetValue(BigDecimal value)
        {
            if (value == BigDecimal.Zero)
                return PreboxedConstants.BigDecimal_Zero;
            if (value == BigDecimal.One)
                return PreboxedConstants.BigDecimal_One;
            if (value == BigDecimal.MinusOne)
                return PreboxedConstants.BigDecimal_MinusOne;
            return null;
        }

        /// <summary>
        /// A singleton boxed BigDecimal 0.
        /// </summary>
        public static readonly object BigDecimal_Zero = BigDecimal.Zero;

        /// <summary>
        /// Expression to return the singleton boxed BigDecimal 0.
        /// </summary>
        public static readonly Expression BigDecimal_Zero_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigDecimal_Zero");

        /// <summary>
        /// A singleton boxed BigDecimal 1.
        /// </summary>
        public static readonly object BigDecimal_One = BigDecimal.One;

        /// <summary>
        /// Expression to return the singleton boxed BigDecimal 1.
        /// </summary>
        public static readonly Expression BigDecimal_One_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigDecimal_One");

        /// <summary>
        /// A singleton boxed BigDecimal -1.
        /// </summary>
        public static readonly object BigDecimal_MinusOne = BigDecimal.MinusOne;

        /// <summary>
        /// Expression to return the singleton boxed BigDecimal -1.
        /// </summary>
        public static readonly Expression BigDecimal_MinusOne_Expression = Expression.Field(null, typeof(PreboxedConstants), "BigDecimal_MinusOne");

        #endregion
    }
}
