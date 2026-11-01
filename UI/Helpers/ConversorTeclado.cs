using System.Runtime.InteropServices;
using System.Text;
using Service;
using static UI.Helpers.NativeMethods;

namespace UI.Helpers
{
    /// <summary>
    /// Método de entrada para pinyin: sigue las letras que se escriben en la ventana activa y,
    /// cuando llega un número de tono (1 a 5), reemplaza la sílaba por su forma acentuada.
    /// Solo recuerda en memoria las últimas letras de la sílaba en curso (como mucho
    /// <see cref="PinyinService.LargoMaximoSilaba"/>); no guarda ni envía nada.
    /// </summary>
    public sealed class ConversorTeclado(PinyinService pinyinService) : IDisposable
    {
        private readonly PinyinService _pinyinService = pinyinService;
        private readonly StringBuilder _silaba = new();

        // Se guardan en campos para que el GC no libere los delegados mientras Windows los usa.
        private HookProc? _procTeclado;
        private HookProc? _procMouse;
        private IntPtr _hookTeclado;
        private IntPtr _hookMouse;
        private IntPtr _ventanaActual;

        public bool Activo => _hookTeclado != IntPtr.Zero;

        public void Activar()
        {
            if (Activo)
                return;

            _procTeclado = ProcesarTecla;
            _procMouse = ProcesarMouse;
            IntPtr modulo = GetModuleHandle(null);

            _hookTeclado = SetWindowsHookEx(WH_KEYBOARD_LL, _procTeclado, modulo, 0);
            if (_hookTeclado == IntPtr.Zero)
                throw new InvalidOperationException($"No se pudo activar la conversión (error {Marshal.GetLastWin32Error()}).");

            // El mouse solo se usa para saber que el cursor pudo cambiar de lugar.
            _hookMouse = SetWindowsHookEx(WH_MOUSE_LL, _procMouse, modulo, 0);
        }

        public void Desactivar()
        {
            if (_hookTeclado != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookTeclado);
                _hookTeclado = IntPtr.Zero;
            }
            if (_hookMouse != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookMouse);
                _hookMouse = IntPtr.Zero;
            }
            _silaba.Clear();
        }

        public void Dispose() => Desactivar();

        private IntPtr ProcesarTecla(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (int)wParam is WM_KEYDOWN or WM_SYSKEYDOWN)
            {
                KBDLLHOOKSTRUCT tecla = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                // Lo que se escribe con SendInput (este programa u otro) no se vuelve a procesar.
                if ((tecla.flags & LLKHF_INJECTED) == 0 && ManejarTecla((Keys)tecla.vkCode))
                    return 1; // se consume el número de tono: no llega a la aplicación
            }
            return CallNextHookEx(_hookTeclado, nCode, wParam, lParam);
        }

        private IntPtr ProcesarMouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (int)wParam is WM_LBUTTONDOWN or WM_RBUTTONDOWN or WM_MBUTTONDOWN)
                _silaba.Clear();
            return CallNextHookEx(_hookMouse, nCode, wParam, lParam);
        }

        /// <summary>Devuelve true si la tecla se reemplazó por la conversión.</summary>
        private bool ManejarTecla(Keys tecla)
        {
            try
            {
                IntPtr ventana = GetForegroundWindow();
                if (ventana != _ventanaActual)
                {
                    _silaba.Clear();
                    _ventanaActual = ventana;
                }

                if (EsModificador(tecla))
                    return false;

                // Atajos (Ctrl+C, Alt+Tab) y AltGr (que Windows informa como Ctrl+Alt) cortan la sílaba.
                if (Presionada(Keys.ControlKey) || Presionada(Keys.Menu))
                {
                    _silaba.Clear();
                    return false;
                }

                bool shift = Presionada(Keys.ShiftKey);

                if (tecla is >= Keys.A and <= Keys.Z)
                {
                    bool mayuscula = shift ^ ((GetKeyState((int)Keys.CapsLock) & 1) != 0);
                    char letra = mayuscula ? (char)tecla : char.ToLowerInvariant((char)tecla);
                    if (_silaba.Length == PinyinService.LargoMaximoSilaba)
                        _silaba.Remove(0, 1);
                    _silaba.Append(letra);
                    return false;
                }

                if (tecla == Keys.Back)
                {
                    if (_silaba.Length > 0)
                        _silaba.Length--;
                    return false;
                }

                int tono = ObtenerTono(tecla, shift);
                if (tono > 0 && _silaba.Length > 0)
                {
                    ResultadoConversion? resultado = _pinyinService.Convertir(_silaba.ToString(), tono);
                    _silaba.Clear();
                    if (resultado == null)
                        return false;

                    Escribir(resultado);
                    return true;
                }

                // Espacio, puntuación, flechas, Enter, etc.: termina la sílaba.
                _silaba.Clear();
                return false;
            }
            catch (Exception)
            {
                // Una excepción acá cerraría el programa; ante cualquier problema la tecla pasa sin tocar.
                _silaba.Clear();
                return false;
            }
        }

        private static int ObtenerTono(Keys tecla, bool shift)
        {
            if (tecla is >= Keys.D1 and <= Keys.D5 && !shift)
                return tecla - Keys.D0;
            if (tecla is >= Keys.NumPad1 and <= Keys.NumPad5)
                return tecla - Keys.NumPad0;
            return 0;
        }

        private static bool EsModificador(Keys tecla) => tecla is
            Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey or
            Keys.ControlKey or Keys.LControlKey or Keys.RControlKey or
            Keys.Menu or Keys.LMenu or Keys.RMenu or Keys.CapsLock;

        private static bool Presionada(Keys tecla) => GetAsyncKeyState((int)tecla) < 0;

        private static void Escribir(ResultadoConversion resultado)
        {
            List<INPUT> entradas = [];
            for (int i = 0; i < resultado.CaracteresABorrar; i++)
            {
                entradas.Add(Tecla((ushort)Keys.Back, '\0', 0));
                entradas.Add(Tecla((ushort)Keys.Back, '\0', KEYEVENTF_KEYUP));
            }
            foreach (char caracter in resultado.TextoAEscribir)
            {
                entradas.Add(Tecla(0, caracter, KEYEVENTF_UNICODE));
                entradas.Add(Tecla(0, caracter, KEYEVENTF_UNICODE | KEYEVENTF_KEYUP));
            }

            if (entradas.Count > 0)
                SendInput((uint)entradas.Count, [.. entradas], Marshal.SizeOf<INPUT>());
        }

        private static INPUT Tecla(ushort virtualKey, char caracter, uint flags) => new()
        {
            type = INPUT_KEYBOARD,
            u = new InputUnion { ki = new KEYBDINPUT { wVk = virtualKey, wScan = caracter, dwFlags = flags } }
        };
    }
}
