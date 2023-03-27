using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace RsaRepro
{
    public static class Artifacts
    {
        public static readonly string AccessToken;

        public static readonly byte[] PayloadBytes;
        public static readonly string SignatureString;
        public static readonly byte[] SignatureBytes;

        public static readonly X509Certificate2 ValidationCert;
        public static readonly RSA Rsa;
        public static readonly RSACryptoServiceProvider CryptoServiceProvider;

        static Artifacts()
        {
            var x5c = "MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP";
            AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiI4YjY5YmU1Yy1kNTYxLTQ3YWMtYTg4Mi1mMGZiYjlhM2E0NmYiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8zOTRmMjI1Zi03YWM5LTQ3N2UtOWI0Yy05ODkzZjRlMWY1MDIvIiwiaWF0IjoxNjc1NTQ4NDcyLCJuYmYiOjE2NzU1NDg0NzIsImV4cCI6MTY3NTU1MzIzMCwiYWNyIjoiMSIsImFpbyI6IkUyWmdZT2lZNisxWnVwWHBnT0Y2NS9oSGoxZXlibGtsb0tXdXVldVA4UCt5WGlaSnpma0EiLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiNjcxNzE4ZTAtMDljNS00NmQyLTlhMmItM2NhMTk0ZTYyMjgwIiwiYXBwaWRhY3IiOiIwIiwiYXV0aF90aW1lIjoxNjc1NTQ4NzcyLCJmYW1pbHlfbmFtZSI6Ilwi44KS44Gyc8ynzIzMgXPMp8yMzIHjgbvjgo_jgozjgojjgYblrZjlnKjjgZnjgovwn5q08J-PvvCfmrTwn4--8J-kvfCfj77igI3imYDvuI_wn6S98J-PvuKAjeKZgO-4jyIsImdpdmVuX25hbWUiOiLDv-WnpuWnpuWnpuWnpuWnpiIsImlwYWRkciI6Ijg4LjEwMy4xMzQuMTQzIiwibmFtZSI6IlRlc3QgQWNjb3VudCIsIm9pZCI6ImFlMmZjZGVmLWM2YjUtNDhlOC05MTVkLTIxZjAyZjc3MDdmMiIsInJoIjoiMC5BUjhBWHlKUE9jbDZma2ViVEppVDlPSDFBbHktYVl0aDFheEhxSUx3LTdtanBHLUZBR2cuIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic2lkIjoiOGFlNjhiOWMtNDI5Zi00OGVlLTg4YTItNzI4NTY3YTIxODQ1Iiwic3ViIjoidVUtSnk1NllTYTlpeE5tVGFOR21RMDVqa2V0WHVOV0d0NkU5TnJaZmNJayIsInRpZCI6IjM5NGYyMjVmLTdhYzktNDc3ZS05YjRjLTk4OTNmNGUxZjUwMiIsInVuaXF1ZV9uYW1lIjoidGVzdGFjY291bnRAc2t5cGVhYWR0ZXN0aW5nLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6InRlc3RhY2NvdW50QHNreXBlYWFkdGVzdGluZy5vbm1pY3Jvc29mdC5jb20iLCJ1dGkiOiJUQnBoMEl1SUtFbU5QVXNrdktVMkFBIiwidmVyIjoiMS4wIn0.lpRd1jxLcCl7hjgBTm6nVcZm1-tMLZ2qWt-LUpOZKRQwxPyUQzXx2SfZf5IYCErZ20g3sy2tyZz7x63Nk2zOixVn_RdHMO0qtKCG6XsIgBmFKlmlGWKRRd8DRu3uE9KjfqanVMO1MGmEel0nxNpmiSvVmagaY6Dqp99n8sDX7v5Oiykidn3NVB31HPjWru8-R14Hvc6FK4I25qNh_Ub05clg33l4nYcuA861iD0X_FUN-7BNLlycAu5usH1TEhgAqQM4uigftA1NzxXHqr6aIJINXVG2n0lq9Qj7k41HSXVlUdqHbnI2b1WRX9hyUcAsP-kaKJrHDJOK5vexwQAd0w";


            var tokenSpan = AccessToken.AsSpan();
            var firstDot = tokenSpan.IndexOf('.');
            var lastDot = tokenSpan.LastIndexOf('.');
            var signatureSection = tokenSpan.Slice(lastDot + 1);
            var bodySection = tokenSpan.Slice(0, lastDot);

            SignatureString = signatureSection.ToString();
            SignatureBytes = Base64UrlEncoder.DecodeBytes(SignatureString);

            PayloadBytes = Encoding.UTF8.GetBytes(bodySection.ToString());

            ValidationCert = new X509Certificate2(Convert.FromBase64String(x5c));
            Rsa = ValidationCert.GetRSAPublicKey();
            CryptoServiceProvider = new RSACryptoServiceProvider();
            CryptoServiceProvider.ImportParameters(Artifacts.Rsa.ExportParameters(false));
        }
    }
}
