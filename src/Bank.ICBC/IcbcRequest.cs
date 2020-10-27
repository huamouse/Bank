using System;
using System.Collections.Generic;

namespace Icbc
{
    public abstract class IcbcRequest<T> where T : IcbcResponse
    {
        public abstract HttpMethod Method { get; }

        public abstract string ServiceUrl { get; }

        public virtual bool IsNeedEncrypt => false;

        public abstract Type GetResponseClass();

        public abstract Type GetBizContentClass();

        public BizContent Content { get; set; }

        public Dictionary<string, string> ExtraParams { get; set; } = new Dictionary<String, String>();
    }

    public enum HttpMethod
    {
        Get,
        Post,
        Unknown
    }
}
