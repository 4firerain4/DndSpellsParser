using Shared;
using System.Text.Json;
using System.Text.Json.Nodes;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TTGClub
{
    internal class JsongSpellParser
    {
        public static List<string> ParseLinks(List<string> jsonSpells)
        {
            JsonNode node = JsonNode.Parse(jsonSpells.First());

            List<string> urls = node.AsArray().Select(p => p["url"].ToString().Substring(7)).ToList();

            return urls;
        }



        public static List<Spell> ParseInfo(List<string> jsonFullSpells)
        {
            List<Spell> spells = new();

            foreach (var json in jsonFullSpells)
            {
                Spell spell = new();
                JsonNode node = JsonNode.Parse(json);
                spell.Title = node["name"]["rus"].ToString();
                spell.TitleEn = node["name"]["eng"].ToString();
                spell.Level = node["level"].ToString();
                spell.School = node["school"].ToString();
                spell.ComponentV = bool.Parse(node["components"]["v"]?.ToString()); //TODO: поменяй строковые типы на булево.
                spell.ComponentS = bool.Parse(node["components"]["s"]?.ToString());
                spell.ComponentM = node["components"]["m"]?.ToString() ?? null;
                spell.Url = node["url"].ToString();
                spell.Duration = node["duration"].ToString();
                spell.Distance = node["range"].ToString();
                spell.Description = node["description"].ToString();
                spell.CastingTime = node["time"].ToString();
                spell.Sources = new [] {node["source"]["name"]?.ToString() ?? null};

                spell.UnitClasses = ParseJsonMassive(node["classes"]?.AsArray() ?? null);
                spell.SubClasses = ParseJsonMassive(node["subclasses"]?.AsArray() ?? null);
                spell.Races = ParseJsonMassive(node["races"]?.AsArray() ?? null);
                


                spells.Add(spell);

                Console.WriteLine($"Spell {spell.Title} parsed");

            }

            return spells;
        }

        private static string[] ParseJsonMassive(JsonArray jsonArray)
        {
            if (jsonArray == null) return Array.Empty<string>();

            List<string> result = new();

            foreach (JsonNode item in jsonArray)
            {
                result.Add(item["name"].ToString());
            }

            return result.ToArray();
        }
    }
}
