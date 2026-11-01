using System.Drawing.Drawing2D;

namespace UI.Helpers
{
    /// <summary>
    /// Dibuja el ícono de la bandeja: una "ǎ" blanca sobre un cuadrado redondeado del color
    /// indicado. Se dibuja al tamaño real de la bandeja (no se achica uno más grande) y la letra
    /// se escala para ocupar todo el cuadrado, así se distingue aun a 16 px.
    /// </summary>
    internal static class IconoBandeja
    {
        public static Icon Crear(Color fondo)
        {
            int lado = SystemInformation.SmallIconSize.Width;
            using Bitmap imagen = new(lado, lado);
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using GraphicsPath cuadrado = CuadradoRedondeado(lado, lado / 4f);
                using SolidBrush pincelFondo = new(fondo);
                g.FillPath(pincelFondo, cuadrado);

                using GraphicsPath letra = new();
                using FontFamily familia = new("Segoe UI");
                letra.AddString("ǎ", familia, (int)FontStyle.Bold, 100, PointF.Empty, StringFormat.GenericTypographic);

                // Centra la letra y la agranda hasta dejar un margen chico alrededor.
                RectangleF limites = letra.GetBounds();
                float espacio = lado * 0.76f;
                float escala = Math.Min(espacio / limites.Width, espacio / limites.Height);
                using Matrix ajuste = new();
                ajuste.Translate(lado / 2f, lado / 2f);
                ajuste.Scale(escala, escala);
                ajuste.Translate(-(limites.X + limites.Width / 2), -(limites.Y + limites.Height / 2));
                letra.Transform(ajuste);
                g.FillPath(Brushes.White, letra);
            }

            IntPtr handle = imagen.GetHicon();
            try
            {
                using Icon temporal = Icon.FromHandle(handle);
                return (Icon)temporal.Clone();
            }
            finally
            {
                NativeMethods.DestroyIcon(handle);
            }
        }

        private static GraphicsPath CuadradoRedondeado(int lado, float radio)
        {
            float diametro = radio * 2;
            float fin = lado - 1;
            GraphicsPath camino = new();
            camino.AddArc(0, 0, diametro, diametro, 180, 90);
            camino.AddArc(fin - diametro, 0, diametro, diametro, 270, 90);
            camino.AddArc(fin - diametro, fin - diametro, diametro, diametro, 0, 90);
            camino.AddArc(0, fin - diametro, diametro, diametro, 90, 90);
            camino.CloseFigure();
            return camino;
        }
    }
}
