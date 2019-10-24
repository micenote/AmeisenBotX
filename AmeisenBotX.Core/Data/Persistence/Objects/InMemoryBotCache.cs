﻿using AmeisenBotX.Core.Data.Objects.WowObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AmeisenBotX.Core.Data.Persistence.Objects
{
    [Serializable]
    public class InMemoryBotCache : IAmeisenBotCache
    {
        public InMemoryBotCache(string path)
        {
            Path = path;
            NameCache = new Dictionary<ulong, string>();
            ReactionCache = new Dictionary<(int, int), WowUnitReaction>();
        }

        public string Path { get; }

        public Dictionary<ulong, string> NameCache { get; private set; }

        public Dictionary<(int, int), WowUnitReaction> ReactionCache { get; private set; }

        public void Save()
        {
            using (Stream stream = File.Open(Path, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, this);
            }
        }

        public void Load()
        {
            using (Stream stream = File.Open(Path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                InMemoryBotCache loadedCache = (InMemoryBotCache)binaryFormatter.Deserialize(stream);

                NameCache = loadedCache.NameCache;
                ReactionCache = loadedCache.ReactionCache;
            }
        }

        public bool TryGetName(ulong guid, out string name)
        {
            if (NameCache.ContainsKey(guid))
            {
                name = NameCache[guid];
                return true;
            }

            name = "";
            return false;
        }

        public void CacheName(ulong guid, string name)
        {
            if (!NameCache.ContainsKey(guid))
            {
                NameCache.Add(guid,name);
            }
        }

        public bool TryGetReaction(int a, int b, out WowUnitReaction reaction)
        {
            if (ReactionCache.ContainsKey((a, b)))
            {
                reaction = ReactionCache[(a, b)];
                return true;
            }
            else if (ReactionCache.ContainsKey((b, a)))
            {
                reaction = ReactionCache[(b, a)];
                return true;
            }

            reaction = WowUnitReaction.Unknown;
            return false;
        }

        public void CacheReaction(int a, int b, WowUnitReaction reaction)
        {
            if (!ReactionCache.ContainsKey((a, b)))
            {
                ReactionCache.Add((a, b), reaction);
            }
        }
    }
}
