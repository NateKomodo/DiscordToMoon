using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DiscordToMoon.Helpers
{
    public sealed class JsonLoader
    {
        private readonly ConcurrentStack<JObject> _jLoaded = new ConcurrentStack<JObject>();
        private readonly ConcurrentStack<string> _sLoaded = new ConcurrentStack<string>();
        
        public JsonLoader(IEnumerable<string> jsonPaths)
        {
            Console.WriteLine("Loading json...");

            Parallel.ForEach(jsonPaths, LoadFile);

            Console.WriteLine($"Loaded {_jLoaded.Count + _sLoaded.Count} files");
        }

        public JsonLoader(string jsonPath)
        {
            Console.WriteLine("Loading json...");

            LoadFile(jsonPath);

            Console.WriteLine("Loaded 1 files");
        }

        private void LoadFile(string path)
        {
            var body = File.ReadAllText(path);
            if (body.StartsWith("{"))
                _jLoaded.Push(JObject.Parse(body));
            else
                _sLoaded.Push(body);
        }

        public Dictionary<string, List<string>> FromDiscord()
        {
            var manifest = new ConcurrentDictionary<string, List<string>>();

            Parallel.ForEach(_jLoaded, j =>
            {
                var name = j.SelectToken("channel.name")?.ToObject<string>();
                var messages = j.SelectToken("messages", false)?.ToObject<JArray>();

                if (string.IsNullOrEmpty(name) || messages == null) throw new NullReferenceException();

                var messageList = new List<string>();

                foreach (var message in messages)
                {
                    var content = message.SelectToken("content", false)?.ToObject<string>();
                    var author = message.SelectToken("author.name", false)?.ToObject<string>();
                    content = content != null ? StripNonAscii(content) : null;
                    author = author != null ? StripNonAscii(author) : null;
                    messageList.Add(string.IsNullOrEmpty(content)
                        ? $"{author}: <image/file/emoji>\n"
                        : $"{author}: {content}\n");
                }

                name = "#" + name;

                if (manifest.ContainsKey(name))
                    manifest[name].AddRange(messageList);
                else
                    manifest.TryAdd(name, messageList);
            });
            
            Console.WriteLine($"Loaded {manifest.Keys.Count} channels and {manifest.Values.Select(m => m.Count).Sum()} messages");
            foreach (var kvp in manifest) Console.WriteLine($" - {kvp.Key}: {kvp.Value.Count}");

            return manifest.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public string FromText()
        {
            return _sLoaded.Aggregate("", (current, s) => current + StripNonAscii(s));
        }
        
        private static string StripNonAscii(string s)
        {
            return s.ToCharArray().Where(c => c < 128).Aggregate("", (current, c) => current + c);
        }
    }
}