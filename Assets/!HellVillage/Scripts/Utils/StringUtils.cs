using System;
using UnityEngine;

namespace HellVillage
{
    public class StringUtils
    {
        /// <summary>
        /// Menambahkan suffix dan prefix sebagai padding terhadap teks yang diberikan.
        /// </summary>
        /// <param name="text">Isi Teks</param>
        /// <param name="n">Jumlah padding</param>
        /// <param name="padding">Teks padding. Default = " "</param>
        /// <returns>string text</returns>
        public static string AddPaddingSpaces(string text, int n, string padding = " ")
        {
            for (int i = 0; i < n; i++)
            {
                text = padding + text + padding;
            }
            return text;
        }
    }
}
