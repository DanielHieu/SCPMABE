namespace ScpmaBe.WebApi.Helpers
{
    using System;
    using System.Collections;
    using System.Security.Cryptography;
    using System.Text;

    public static class AccessTokenGenerator
    {
        private static string secretKey = "4e06e108c535120193591e8be40fd3779acc9835bb468bbd6251f301ae39d3160c63bce031a3253e9f25e034ebe818ec7c59ae0c476f2330f75b0c9a2fbdb3c422c05db759a704e70c5471bcd7e983440a2b7ac30d180201f60ebcd1f27bc1c574039cc9831d2aecc88b23892c986bb288c9cad12405dff15a9e463bf0a4981fdde797940da745c5214538f9bdd331658a272a597514a4e49dfda40d7061e0ac65c88d623aab2d443e00cc4459b863d9bc677fd72048690ce6b43bf45bd1d605b9d4c20e5ccd6eea2a54c641456e3380594c1b77caf11f49b85b618cc171a2937fdd4b246c9c6ff45657178d51fc40d6ee10a6ed95e6637e789b515753e10901";

        //Example with a expiration time included in the token. (JWT Like, but much simpler)
        public static string GenerateExpiringAccessToken(DateTime expiration, int randomBytesLength = 16)
        {
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
            }

            long expirationTimestamp = ((DateTimeOffset)expiration).ToUnixTimeSeconds();

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[randomBytesLength];
                rng.GetBytes(randomBytes);

                byte[] expirationBytes = BitConverter.GetBytes(expirationTimestamp);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(expirationBytes); //Ensure Big endian.
                }

                byte[] combinedBytes = new byte[randomBytes.Length + expirationBytes.Length];
                Buffer.BlockCopy(randomBytes, 0, combinedBytes, 0, randomBytes.Length);
                Buffer.BlockCopy(expirationBytes, 0, combinedBytes, randomBytes.Length, expirationBytes.Length);

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                {
                    byte[] hash = hmac.ComputeHash(combinedBytes);
                    return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").Replace("=", ""); // URL-safe base64
                }
            }
        }

        public static bool ValidateExpiringAccessToken(string token, out DateTime expiration)
        {
            expiration = DateTime.MinValue; // Default value if validation fails.
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secretKey))
            {
                return false;
            }

            try
            {
                byte[] decodedToken = Convert.FromBase64String(token.Replace("-", "+").Replace("_", "/"));
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                {
                    int hashLength = hmac.HashSize / 8; // Hash size in bytes
                    if (decodedToken.Length < hashLength + 8) // 8 bytes for expiration
                    {
                        return false;
                    }

                    byte[] hash = new byte[hashLength];
                    byte[] combinedBytes = new byte[decodedToken.Length - hashLength];

                    Buffer.BlockCopy(decodedToken, 0, hash, 0, hashLength);
                    Buffer.BlockCopy(decodedToken, hashLength, combinedBytes, 0, combinedBytes.Length);

                    byte[] computedHash = hmac.ComputeHash(combinedBytes);

                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(hash, computedHash))
                    {
                        return false;
                    }

                    byte[] expirationBytes = new byte[8];
                    Buffer.BlockCopy(combinedBytes, combinedBytes.Length - 8, expirationBytes, 0, 8);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(expirationBytes);
                    }

                    long expirationTimestamp = BitConverter.ToInt64(expirationBytes, 0);
                    expiration = DateTimeOffset.FromUnixTimeSeconds(expirationTimestamp).DateTime;

                    return DateTime.UtcNow < expiration;

                }
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
