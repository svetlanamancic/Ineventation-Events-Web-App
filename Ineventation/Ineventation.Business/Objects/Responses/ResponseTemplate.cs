using System;

namespace Ineventation.Business.Objects.Responses
{
    [Serializable]
    public class ResponseTemplate<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public uint? Code { get; set; }
        public T Target { get; set; }
    }
}
