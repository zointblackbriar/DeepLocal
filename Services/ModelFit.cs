using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DeepLocal.Services
{
    /// <summary>
    /// Scelta del modello in base alla VRAM della GPU. Ollama mette sulla GPU solo ciò che
    /// entra: il resto gira su CPU e rallenta molto. Se il modello scelto non entra si cerca
    /// una variante più piccola della stessa famiglia (stesso nome prima di ":").
    /// </summary>
    public static class ModelFit
    {
        /// <summary>Frazione della VRAM totale usabile dal modello: il resto va a KV cache e desktop.</summary>
        public const double UsableFraction = 0.85;

        /// <summary>Sotto questa VRAM la 12B non entra: ha senso proporre la 4B (3.3 GB).</summary>
        public const long SuggestSmallerBelowBytes = 9_500_000_000L;

        public static long Budget(long vramBytes) => (long)(vramBytes * UsableFraction);

        /// <summary>true se non ci sono informazioni sicure (GPU o dimensione ignote): non si avvisa.</summary>
        public static bool Fits(bool isCloud, long modelSizeBytes, long? vramBytes) =>
            isCloud || modelSizeBytes <= 0 || vramBytes is null || modelSizeBytes <= Budget(vramBytes.Value);

        /// <summary>Nome del modello senza il tag, es. "translategemma" da "translategemma:12b".</summary>
        public static string BaseName(string name)
        {
            var i = name.IndexOf(':');
            return i > 0 ? name[..i] : name;
        }

        /// <summary>
        /// La variante più grande della stessa famiglia che entra nella GPU, o null se il modello
        /// scelto entra già o nessuna variante più piccola è adatta.
        /// </summary>
        public static ModelItem? SmallerThatFits(ModelItem current, IEnumerable<ModelItem> candidates, long? vramBytes)
        {
            if (Fits(current.IsCloud, current.Size, vramBytes)) return null;

            var baseName = BaseName(current.Name);
            return candidates
                .Where(m => !m.IsCloud && m.Size > 0 && Fits(m.IsCloud, m.Size, vramBytes) &&
                            string.Equals(BaseName(m.Name), baseName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m => m.Size)
                .FirstOrDefault();
        }

        /// <summary>VRAM in GB come stringa per i messaggi ("6"), "?" se sconosciuta.</summary>
        public static string FormatVramGb(long? vramBytes) =>
            vramBytes is long v ? (v / 1_073_741_824.0).ToString("0.#", CultureInfo.InvariantCulture) : "?";
    }
}
