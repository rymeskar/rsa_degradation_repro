using BenchmarkDotNet.Attributes;
using System;
using System.Security.Cryptography;

namespace RsaRepro
{
    /// <summary>
    /// This benchmark shows funny performance characteristics of RSA. Most likely due to differences in managed and CAPI code.
    /// </summary>
    public class RsaCryptoDegradations
    {
#if !NETFRAMEWORK
        [Benchmark]
        public bool RsaSpan()
        {
            return Artifacts.Rsa.VerifyData(Artifacts.PayloadBytes.AsSpan(), Artifacts.SignatureBytes.AsSpan(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
#endif
        [Benchmark]
        public bool RsaCryptoServiceProvider()
        {
            return Artifacts.CryptoServiceProvider.VerifyData(Artifacts.PayloadBytes, SHA256.Create(), Artifacts.SignatureBytes);
        }

        [Benchmark(Baseline = true)]
        public bool RsaString()
        {
            return Artifacts.Rsa.VerifyData(Artifacts.PayloadBytes, Artifacts.SignatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
