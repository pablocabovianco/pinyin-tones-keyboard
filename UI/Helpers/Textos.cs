using System.Globalization;

namespace UI.Helpers
{
    /// <summary>
    /// Textos de la interfaz: en español si Windows está en español y en inglés en cualquier
    /// otro idioma.
    /// </summary>
    internal static class Textos
    {
        private static readonly bool Espanol = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "es";

        public static string Activo => Espanol ? "Activo" : "Active";
        public static string Salir => Espanol ? "Salir" : "Exit";
        public static string EstadoActivo => Espanol ? "Pinyin Tones: activo" : "Pinyin Tones: active";
        public static string EstadoPausa => Espanol ? "Pinyin Tones: en pausa" : "Pinyin Tones: paused";
        public static string ErrorInesperado => Espanol ? "Error inesperado." : "Unexpected error.";

        public static string Bienvenida => Espanol
            ? "Escribí la sílaba y el número de tono: hao3 → hǎo, lv4 → lǜ."
            : "Type the syllable and its tone number: hao3 → hǎo, lv4 → lǜ.";

        public static string NoSePudoActivar(int error) => Espanol
            ? $"No se pudo activar la conversión (error {error})."
            : $"Could not turn on the conversion (error {error}).";
    }
}
