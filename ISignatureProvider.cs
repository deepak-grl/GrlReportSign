using System;

namespace GrlReportSign
{
    public interface ISignatureProvider
    {
        string GenerateSignature(string content);
        bool VerifySignature(string content, string signature);
    }
}
