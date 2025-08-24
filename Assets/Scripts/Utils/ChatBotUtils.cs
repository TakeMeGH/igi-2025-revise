using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI;
using Perspective.CameraController;
using Perspective.Character.NPC;
using Perspective.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Perspective.Utils
{
    public sealed class ChatBotUtils : Singleton<ChatBotUtils>
    {
        [SerializeField] private List<EventsClassificationExamples> eventsExamples = new();
        private OpenAIApi _openai;
        private readonly List<ChatMessage> _messages = new();

        private readonly string _encodedKey =
            "c2stcHJvai1PRTVKcEpibDROR0pQWkJZZWU4WDFtS3NuT0U3TFNOR28xbFJrZ1h6dUV2VkFKMGtfT2VqbWw2bXRsYVQwZ2JMU3ZXUlBaeGphLVQzQmxia0ZKZU53ZGdHM2xVaUVPMDlONi1ITE01TlB0LWEzVUFoLUNKX3lMdndONWpRLXFGN3BrVWcxM1BCQVluclVVejFVbzZwTzdGUk10UUE=";

        private string GetApiKey()
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(_encodedKey));
        }

        protected override void OwnAwake()
        {
            _openai = new OpenAIApi(GetApiKey());
        }

        private async Task WaitUntilConnected()
        {
            var pauseController = FindAnyObjectByType<PauseMenuController>();
            if (pauseController)
            {
                pauseController.SetPause(true);
            }

            Debug.Log("⏸ Game paused. Waiting for internet...");

            while (!await HasInternetConnection())
            {
                await Task.Delay(2000);
            }

            if (pauseController)
            {
                pauseController.SetPause(false);
            }

            Debug.Log("▶ Internet restored. Game resumed!");
        }

        private async Task<bool> HasInternetConnection()
        {
            using var request = UnityWebRequest.Head("https://www.google.com");
            request.timeout = 5;
            var asyncOp = request.SendWebRequest();

            while (!asyncOp.isDone)
                await Task.Yield();

            return request.result == UnityWebRequest.Result.Success;
        }

        public async Task<string> AskBot(string prompt, string userMessage)
        {
            if (!await HasInternetConnection())
            {
                await WaitUntilConnected();
            }

            _messages.Clear();
            _messages.Add(new ChatMessage()
            {
                Role = "system",
                Content = prompt
            });

            _messages.Add(new ChatMessage()
            {
                Role = "user",
                Content = userMessage
            });

            var completionResponse = await _openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4.1-nano",
                Messages = _messages
            });

            if (completionResponse.Choices is { Count: > 0 })
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                // Save assistant reply to history
                _messages.Add(message);

                return message.Content;
            }

            Debug.LogWarning("No text was generated from this prompt.");
            return string.Empty;
        }

        public List<string> FormatComments(string rawOutput)
        {
            var comments = new List<string>();

            var lines = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("-"))
                {
                    comments.Add(trimmed[1..].Trim());
                }
            }

            return comments;
        }

        public string BuildCommentsPrompt(string stance)
        {
            return $@"
Anda adalah generator komentar netizen di media sosial. 
Tugas Anda adalah membuat 15 komentar singkat dan alami berdasarkan judul berita yang diberikan.

# Instruksi
1. Semua komentar harus terdengar realistis, seperti ditulis netizen biasa.
2. Semua komentar HARUS cenderung {stance}.
   - Jika stance = ""pro pemerintah"": komentar mendukung pemerintah, kebijakan, atau pejabat.
   - Jika stance = ""pro rakyat"": komentar mendukung masyarakat, menyuarakan keluhan publik, atau mengkritik pemerintah.
   - Jika stance = ""netral"": komentar berupa observasi/pertanyaan tanpa memihak.
3. Panjang komentar 1–2 kalimat dengan bahasa sehari-hari.
4. Jangan menyebut bahwa Anda adalah AI.

# Format Output
- Komentar 1
- Komentar 2
- Komentar 3
- dst.
";
        }

        public string BuildNoneEventCommentsPrompt()
        {
            return @"
Anda adalah generator komentar netizen di media sosial. 
Tugas Anda adalah membuat komentar singkat dan alami berdasarkan judul berita yang diberikan pengguna. 

# Instruksi
1. Abaikan gambar sepenuhnya, jangan menyinggung atau mencoba menjelaskan gambar.
2. Anggap gambar tidak relevan dengan berita, jadi komentar hanya menanggapi judul berita.
3. Buat 15 komentar netizen dengan nada:
   - bingung,
   - nyinyir,
   - bercanda,
   - mempertanyakan relevansi.
4. Jangan ada komentar yang bersifat mendukung pemerintah atau rakyat.
5. Gunakan bahasa sehari-hari, informal khas netizen.
6. Panjang komentar 1–2 kalimat.

# Format Output
- Kembalikan daftar komentar dalam bentuk bullet point.
";
        }


        public string BuildStancePrompt(NpcEvent currentEvent)
        {
            var sb = new StringBuilder();

            sb.AppendLine(
                @"Analisis hubungan antara sebuah gambar dan judul berita yang disediakan oleh pengguna. Tugas Anda adalah mengklasifikasikan dengan tepat salah satu dari empat kategori berikut:

1. tidak relevan
2. netral
3. pro pemerintah
4. pro rakyat

# Instruksi Internal
Lakukan analisis internal dengan tahapan sebagai berikut sebelum memberikan hasil akhir:
1. Perhatikan isi gambar dan teks judul berita.
2. Tentukan secara internal apakah judul berita relevan atau tidak dengan gambar:
   - Jika tidak relevan sama sekali, pilih kategori ""tidak relevan"".
   - Jika relevan, lanjut ke langkah berikutnya.
3. Analisis sikap atau nada dalam teks terhadap konteks gambar:
   - Jika bersifat objektif atau netral, pilih ""netral"".
   - Jika menunjukkan dukungan terhadap pemerintah, pejabat, atau kebijakan pemerintah, pilih ""pro pemerintah"".
   - Jika menunjukkan dukungan atau simpati terhadap masyarakat, warga sipil, atau suara publik, pilih ""pro rakyat"".
4. Output hanya label yang dipilih dari empat kategori di atas, dalam bentuk teks polos.

# Format Output
- Hanya kembalikan satu label dari daftar:
  - tidak relevan
  - netral
  - pro pemerintah
  - pro rakyat
- Jangan tampilkan penjelasan, alasan, atau bentuk teks lainnya.
- Output harus dalam Bahasa Indonesia, persis sesuai dengan salah satu label.
");

            sb.AppendLine("# Contoh");
            foreach (var currentEventsExamples in eventsExamples.Where(currentEventsExamples =>
                         currentEvent == currentEventsExamples.npcEvent))
            {
                sb.AppendLine($"# Contoh untuk konteks gambar: {currentEventsExamples.imageContext}");

                var index = 1;
                foreach (var ex in currentEventsExamples.examples)
                {
                    sb.AppendLine($"**Contoh {index}**");
                    sb.AppendLine($"- Konteks gambar: {currentEventsExamples.imageContext}");
                    sb.AppendLine($"- Judul berita: \"{ex.headline}\"");
                    sb.AppendLine($"- Output: {LabelToString(ex.output)}");
                    sb.AppendLine();
                    index++;
                }
            }

            sb.AppendLine(@"# Catatan
- Analisis harus dilakukan secara internal sebelum memberikan jawaban.
- Output hanya boleh berupa salah satu label yang valid, tanpa tanda baca tambahan atau komentar apapun.
- Sistem harus menghindari bias, dan hanya menilai berdasarkan konten aktual dari gambar dan teks.");

            return sb.ToString();
        }

        private static string LabelToString(ClassificationLabel label)
        {
            return label switch
            {
                ClassificationLabel.TidakRelevan => "tidak relevan",
                ClassificationLabel.Netral => "netral",
                ClassificationLabel.ProPemerintah => "pro pemerintah",
                ClassificationLabel.ProRakyat => "pro rakyat",
                _ => "netral"
            };
        }
    }
}