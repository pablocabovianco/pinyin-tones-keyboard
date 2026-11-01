using UI.Helpers;

namespace UI
{
    /// <summary>
    /// La aplicación no tiene ventana: vive como un ícono en la bandeja del sistema, desde donde
    /// se pausa, se reactiva (también con doble clic) y se cierra.
    /// </summary>
    public class BandejaContext : ApplicationContext
    {
        private readonly ConversorTeclado _conversor;
        private readonly NotifyIcon _icono;
        private readonly ToolStripMenuItem _menuActivo;
        private readonly Icon _iconoActivo = IconoBandeja.Crear(Color.FromArgb(200, 30, 45));
        private readonly Icon _iconoPausa = IconoBandeja.Crear(Color.Gray);

        public BandejaContext(ConversorTeclado conversor)
        {
            _conversor = conversor;

            _menuActivo = new ToolStripMenuItem("Activo", null, (_, _) => Alternar());
            ContextMenuStrip menu = new();
            menu.Items.Add(_menuActivo);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Salir", null, (_, _) => ExitThread());

            _icono = new NotifyIcon { ContextMenuStrip = menu, Visible = true };
            _icono.DoubleClick += (_, _) => Alternar();

            try
            {
                _conversor.Activar();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Pinyin Tonos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            ActualizarEstado();

            _icono.ShowBalloonTip(3000, "Pinyin Tonos",
                "Escribí la sílaba y el número de tono: hao3 → hǎo, lv4 → lǜ.", ToolTipIcon.None);
        }

        private void Alternar()
        {
            try
            {
                if (_conversor.Activo)
                    _conversor.Desactivar();
                else
                    _conversor.Activar();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Pinyin Tonos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ActualizarEstado();
        }

        private void ActualizarEstado()
        {
            _menuActivo.Checked = _conversor.Activo;
            _icono.Icon = _conversor.Activo ? _iconoActivo : _iconoPausa;
            _icono.Text = _conversor.Activo ? "Pinyin Tonos: activo" : "Pinyin Tonos: en pausa";
        }

        protected override void ExitThreadCore()
        {
            _conversor.Desactivar();
            _icono.Visible = false;
            base.ExitThreadCore();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _icono.ContextMenuStrip?.Dispose();
                _icono.Dispose();
                _iconoActivo.Dispose();
                _iconoPausa.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
