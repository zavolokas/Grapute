using System;
using System.Collections.Generic;

namespace MapReduce
{
    class TokenComparer : IComparer<Token>
    {
        public int Compare(Token x, Token y)
        {
            return string.Compare(x.Term, y.Term, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}