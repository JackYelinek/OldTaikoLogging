using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging.Emulator
{
    class EmulatorSongData
    {
        public string TjaFilePath { get; set; }
        // The Ini file may not exist if I didn't play the song previously
        public string IniFilePath { get; set; }
        // In Emulator, Ura is a part of the song title
        public string SongTitle { get; set; }
        public string Genre { get; set; }
        public int Level { get; set; }
        public float BPM { get; set; }

        public EmulatorSongData(string filePath)
        {
            // filePath == TjaFilePath
            TjaFilePath = filePath;
            IniFilePath = filePath + ".score.ini";

            var splitFilePath = filePath.Split('\\');
            SongTitle = splitFilePath[splitFilePath.Length - 1].Remove(splitFilePath[splitFilePath.Length - 1].IndexOf(".tja"));
            InitializeSongData();
        }

        private void InitializeSongData()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var lines = File.ReadAllLines(TjaFilePath, Encoding.GetEncoding(932));
            bool gotLevel = false;
            bool gotBPM = false;
            bool gotGenre = false;

            for (int l = 0; l < lines.Length; l++)
            {
                if (lines[l].IndexOf("LEVEL") == 0 && gotLevel == false)
                {
                    gotLevel = true;
                    Level = int.Parse(lines[l].Remove(0, 6));
                }
                else if (lines[l].IndexOf("BPM") == 0 && gotBPM == false)
                {
                    gotBPM = true;
                    BPM = float.Parse(lines[l].Remove(0, 4));
                }
                else if (lines[l].IndexOf("GENRE") == 0 && gotGenre == false)
                {
                    gotGenre = true;
                    string genre = lines[l].Remove(0, 6);
                    if (genre == "�o���G�e�B" || genre == "バラエティ")
                    {
                        genre = "VA";
                    }
                    else if (genre == "�i���R�I���W�i��" || genre == "ナムコオリジナル")
                    {
                        genre = "NO";
                    }
                    else if (genre == "�{�[�J���C�h" || genre == "ボーカロイド")
                    {
                        genre = "VC";
                    }
                    else if (genre == "�Q�[���~���[�W�b�N" || genre == "ゲームミュージック")
                    {
                        genre = "GM";
                    }
                    else if (genre == "�N���V�b�N" || genre == "クラシック")
                    {
                        genre = "CL";
                    }
                    else if (genre == "J-POP")
                    {
                        genre = "JP";
                    }
                    else if (genre == "アニメ")
                    {
                        genre = "AN";
                    }
                    else
                    {
                        genre = null;
                    }
                    Genre = genre;
                }
                if (gotLevel == true && gotBPM == true && gotGenre == true)
                {
                    break;
                }
            }
        }

    }
}
