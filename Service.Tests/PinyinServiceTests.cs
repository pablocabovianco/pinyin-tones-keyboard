using Service;

namespace Service.Tests
{
    public class PinyinServiceTests
    {
        private readonly PinyinService _service = new();

        [Theory]
        [InlineData("hao", 3, 2, "ǎo")]
        [InlineData("ni", 3, 1, "ǐ")]
        [InlineData("ma", 1, 1, "ā")]
        [InlineData("xian", 1, 2, "ān")]
        [InlineData("zhuang", 4, 3, "àng")]
        [InlineData("er", 4, 2, "èr")]
        [InlineData("lv", 4, 1, "ǜ")]
        [InlineData("lve", 4, 2, "üè")]
        public void Convertir_PoneLaMarcaYBorraSoloDesdeElPrimerCambio(string escrito, int tono, int borrar, string texto)
        {
            Assert.Equal(new ResultadoConversion(borrar, texto), _service.Convertir(escrito, tono));
        }

        [Theory]
        [InlineData("dou", 1, "ōu")]  // en "ou" va en la o
        [InlineData("liu", 2, "ú")]   // si no hay a, e ni ou: la última vocal
        [InlineData("gui", 4, "ì")]
        [InlineData("xue", 2, "é")]
        public void Convertir_UbicaElTonoSegunLasReglas(string escrito, int tono, string texto)
        {
            Assert.Equal(texto, _service.Convertir(escrito, tono)?.TextoAEscribir);
        }

        [Fact]
        public void Convertir_ConVariasSilabasJuntas_TomaLaUltima()
        {
            Assert.Equal(new ResultadoConversion(2, "ǎo"), _service.Convertir("nihao", 3));
        }

        [Theory]
        [InlineData("nar", 3, 2, "ǎr")]
        [InlineData("zher", 4, 2, "èr")]
        [InlineData("huir", 4, 2, "ìr")]
        [InlineData("dianr", 3, 3, "ǎnr")]
        [InlineData("zhuangr", 4, 4, "àngr")]
        [InlineData("NAR", 3, 2, "ǍR")]
        [InlineData("nar", 5, 0, "")]
        public void Convertir_ConErhua_PoneElTonoEnLaSilabaBase(string escrito, int tono, int borrar, string texto)
        {
            Assert.Equal(new ResultadoConversion(borrar, texto), _service.Convertir(escrito, tono));
        }

        [Fact]
        public void Convertir_ErNoSeTomaComoErhua()
        {
            Assert.Null(_service.Convertir("err", 4));
        }

        [Theory]
        [InlineData("Hao", 3, "ǎo")]
        [InlineData("HAO", 3, "ǍO")]
        [InlineData("LV", 4, "Ǜ")]
        public void Convertir_RespetaMayusculas(string escrito, int tono, string texto)
        {
            Assert.Equal(texto, _service.Convertir(escrito, tono)?.TextoAEscribir);
        }

        [Fact]
        public void Convertir_TonoNeutro_SoloConsumeElNumero()
        {
            Assert.Equal(new ResultadoConversion(0, ""), _service.Convertir("ma", 5));
            Assert.Equal(new ResultadoConversion(1, "ü"), _service.Convertir("lv", 5));
        }

        [Theory]
        [InlineData("mp", 3)]    // sin sílaba de pinyin: "mp3" queda igual
        [InlineData("win", 1)]
        [InlineData("", 1)]
        [InlineData("hao", 6)]
        [InlineData("hao", 0)]
        public void Convertir_SinSilabaOTonoValido_DevuelveNull(string escrito, int tono)
        {
            Assert.Null(_service.Convertir(escrito, tono));
        }
    }
}
