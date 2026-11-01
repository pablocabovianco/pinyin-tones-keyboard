using Service;
using UI.Helpers;

namespace UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Una sola instancia: con dos abiertas cada sílaba se convertiría dos veces.
            using Mutex mutex = new(true, @"Local\PinyinTonos", out bool primeraInstancia);
            if (!primeraInstancia)
                return;

            ApplicationConfiguration.Initialize();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => MostrarError(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => MostrarError(e.ExceptionObject as Exception);

            using ConversorTeclado conversor = new(new PinyinService());
            using BandejaContext contexto = new(conversor);
            Application.Run(contexto);
        }

        private static void MostrarError(Exception? ex)
        {
            MessageBox.Show(ex?.Message ?? "Error inesperado.", "Pinyin Tonos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
