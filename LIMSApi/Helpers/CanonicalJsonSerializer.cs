using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LIMSApi.Helpers
{
    public static class CanonicalJsonSerializer
    {
        private static readonly JsonWriterOptions WriterOptions = new()
        {
            Indented = false
        };

        public static string Serialize<T>(T value)
        {
            if (value == null) return "null";

            // Serialize to JsonNode to sort keys recursively
            var node = JsonSerializer.SerializeToNode(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // Invariant case
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            if (node == null) return "null";

            var sortedNode = SortJsonNode(node);

            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, WriterOptions))
            {
                sortedNode?.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static string SerializeCanonical<T>(T value) => Serialize(value);

        public static string ComputeSha256(string canonicalJson)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(canonicalJson);
            byte[] hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static string ComputeSha256Hash(string canonicalJson) => ComputeSha256(canonicalJson);

        private static JsonNode? SortJsonNode(JsonNode? node)
        {
            if (node is JsonObject obj)
            {
                var sortedObj = new JsonObject();
                var sortedProperties = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, JsonNode?>>();
                foreach (var prop in obj)
                {
                    sortedProperties.Add(new System.Collections.Generic.KeyValuePair<string, JsonNode?>(prop.Key, prop.Value));
                }
                sortedProperties.Sort((a, b) => string.CompareOrdinal(a.Key, b.Key));

                foreach (var prop in sortedProperties)
                {
                    sortedObj[prop.Key] = SortJsonNode(prop.Value);
                }
                return sortedObj;
            }

            if (node is JsonArray arr)
            {
                var sortedArr = new JsonArray();
                foreach (var item in arr)
                {
                    sortedArr.Add(SortJsonNode(item));
                }
                return sortedArr;
            }

            return node?.DeepClone();
        }
    }
}
