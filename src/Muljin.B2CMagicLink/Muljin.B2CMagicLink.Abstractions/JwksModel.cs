using System;
namespace Muljin.B2CMagicLink
{
    public record JwksModel
    {
        public ICollection<JwksKeyModel> Keys { get; set; } = new List<JwksKeyModel>();
    }
}

