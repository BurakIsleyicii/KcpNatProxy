using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KcpNatProxy.Server
{
    public class KnpServerOptions
    {
        public KnpServerListenOptions? Listen { get; set; }
        public string? Credential { get; set; }
        public KnpServiceDescription[]? Services { get; set; }

        [MemberNotNullWhen(true, nameof(Listen))]
        public bool Validate([NotNullWhen(false)] out string? errorMessage)
        {
            if (Listen is null)
            {
                errorMessage = "Server listen endpoint is not configured.";
                return false;
            }
            if (!string.IsNullOrEmpty(Credential) && Encoding.UTF8.GetByteCount(Credential) > 64)
            {
                errorMessage = "Credential is too long.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

}
