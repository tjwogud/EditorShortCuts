using EditorShortCuts.Utils;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace EditorShortCuts
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            var filepath = Path.Combine(modEntry.Path, "Settings.xml");
            using (var writer = new StreamWriter(filepath))
                new XmlSerializer(GetType()).Serialize(writer, this);
        }

        [XmlArrayItem("keys")]
        public List<Tuple<int, int>> keys = new List<Tuple<int, int>>();
    }
}
