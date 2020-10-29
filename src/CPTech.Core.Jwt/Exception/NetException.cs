using System;

namespace CPTech.Core
{
    public class NetException : Exception
    {
        public int Code { get; set; } = -1;

        public NetException(string message) : base(message)
        {
        }

        public NetException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
