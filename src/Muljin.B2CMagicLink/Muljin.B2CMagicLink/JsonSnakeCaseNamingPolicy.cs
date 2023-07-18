using System.Text.Json;

namespace Muljin.B2CMagicLink.AzureKeyVault
{
    internal class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }
}

