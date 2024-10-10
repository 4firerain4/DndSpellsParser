using Shared;
using System.Text.Json.Nodes;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TTGClub
{
    internal class JsonSpellParser
    {
        public static List<string> ParseLinks(string jsonSpells)
        {
            JsonArray array = JsonNode.Parse(jsonSpells).AsArray();
            return array.Select(x => $"https://ttg.club/api/v1/spells{x["url"].ToString().Substring(7)}").ToList();
        }
        public static Spell ParseData(string json)
        {
            Spell spell = new();

            JsonNode node = JsonNode.Parse(json);
            spell.Title = node["name"]?["rus"]?.ToString() ?? "N/A";
            spell.TitleEn = node["name"]?["eng"]?.ToString() ?? "N/A";
            spell.Level = node["level"]?.ToString() ?? "N/A";
            spell.School = node["school"]?.ToString() ?? "N/A";

            spell.ComponentV = bool.Parse(node["components"]?["v"]?.ToString() ?? "false");
            spell.ComponentS = bool.Parse(node["components"]?["s"]?.ToString() ?? "false");
            spell.ComponentM = node["components"]?["m"]?.ToString() ?? "N/A";

            spell.Url = node["url"]?.ToString()?? "N/A";
            spell.Duration = node["duration"]?.ToString()?? "N/A";
            spell.Distance = node["range"]?.ToString()?? "N/A";
            spell.Description = node["description"]?.ToString()?? "N/A";
            spell.CastingTime = node["time"]?.ToString() ?? "N/A";
            spell.Sources = new[] { node["source"]?["name"]?.ToString() ?? null}; // TODO: парсить всё данные из источников

            spell.UnitClasses = ParseJsonMassive(node["classes"]?.AsArray() ?? null);
            spell.SubClasses = ParseJsonMassive(node["subclasses"]?.AsArray() ?? null);
            spell.Races = ParseJsonMassive(node["races"]?.AsArray() ?? null);

            return spell;
        }

        private static string[] ParseJsonMassive(JsonArray? jsonArray)
        {
            if (jsonArray is null) return Array.Empty<string>();

            List<string> result = new();

            foreach (JsonNode item in jsonArray)
            {
                result.Add(item["name"].ToString());
            }

            return result.ToArray();
        }
    }
}
