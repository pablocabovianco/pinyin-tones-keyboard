namespace Service
{
    /// <summary>
    /// Convierte una sílaba escrita con número de tono ("hao" + 3) a su forma acentuada ("hǎo").
    /// La "v" se usa para escribir "ü" (lv4 → lǜ). El tono 5 es el neutro: no lleva marca.
    /// </summary>
    public class PinyinService
    {
        /// <summary>Largo de las sílabas más largas con erhua (zhuangr, chuangr, shuangr).</summary>
        public const int LargoMaximoSilaba = 7;

        private const string Vocales = "aeiouü";

        // Para cada vocal de Vocales, su forma en los tonos 1 a 4.
        private static readonly string[] VocalesConTono = ["āáǎà", "ēéěè", "īíǐì", "ōóǒò", "ūúǔù", "ǖǘǚǜ"];
        private static readonly string[] VocalesConTonoMayuscula = ["ĀÁǍÀ", "ĒÉĚÈ", "ĪÍǏÌ", "ŌÓǑÒ", "ŪÚǓÙ", "ǕǗǙǛ"];

        private static readonly HashSet<string> Silabas = new(
            """
            a ai an ang ao e ei en eng er o ou
            yi ya yao yan yang ye you yin ying yong yu yue yuan yun
            wu wa wo wai wei wan wen wang weng
            ba bo bai bei bao ban ben bang beng bi bie biao bian bin bing bu
            pa po pai pei pao pou pan pen pang peng pi pie piao pian pin ping pu
            ma mo me mai mei mao mou man men mang meng mi mie miao miu mian min ming mu
            fa fo fei fou fan fen fang feng fu
            da de dai dei dao dou dan den dang deng dong di die diao diu dian ding du duo dui duan dun
            ta te tai tao tou tan tang teng tong ti tie tiao tian ting tu tuo tui tuan tun
            na ne nai nei nao nou nan nen nang neng nong ni nie niao niu nian nin niang ning nu nuo nuan nv nve
            la le lai lei lao lou lan lang leng long li lia lie liao liu lian lin liang ling lu luo luan lun lv lve
            ga ge gai gei gao gou gan gen gang geng gong gu gua guo guai gui guan gun guang
            ka ke kai kei kao kou kan ken kang keng kong ku kua kuo kuai kui kuan kun kuang
            ha he hai hei hao hou han hen hang heng hong hu hua huo huai hui huan hun huang
            ji jia jie jiao jiu jian jin jiang jing jiong ju jue juan jun
            qi qia qie qiao qiu qian qin qiang qing qiong qu que quan qun
            xi xia xie xiao xiu xian xin xiang xing xiong xu xue xuan xun
            zha zhe zhi zhai zhei zhao zhou zhan zhen zhang zheng zhong zhu zhua zhuo zhuai zhui zhuan zhun zhuang
            cha che chi chai chao chou chan chen chang cheng chong chu chua chuo chuai chui chuan chun chuang
            sha she shi shai shei shao shou shan shen shang sheng shu shua shuo shuai shui shuan shun shuang
            re ri rao rou ran ren rang reng rong ru rua ruo rui ruan run
            za ze zi zai zei zao zou zan zen zang zeng zong zu zuo zui zuan zun
            ca ce ci cai cao cou can cen cang ceng cong cu cuo cui cuan cun
            sa se si sai sao sou san sen sang seng song su suo sui suan sun
            """.Split([' ', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries));

        /// <summary>
        /// Convierte la sílaba con la que termina <paramref name="escrito"/> (lo último que se
        /// tipeó, sin el número). Si se escribieron varias sílabas juntas ("nihao"), toma la
        /// más larga posible desde el final ("hao"). Devuelve null si no termina en una sílaba
        /// de pinyin o si el tono no es de 1 a 5: en ese caso el número se escribe normalmente.
        /// </summary>
        public ResultadoConversion? Convertir(string escrito, int tono)
        {
            if (tono < 1 || tono > 5)
                return null;

            string? silaba = BuscarSilabaFinal(escrito);
            if (silaba == null)
                return null;

            string convertida = AplicarTono(silaba, tono);
            int iguales = 0;
            while (iguales < silaba.Length && silaba[iguales] == convertida[iguales])
                iguales++;

            return new ResultadoConversion(silaba.Length - iguales, convertida[iguales..]);
        }

        private static string? BuscarSilabaFinal(string escrito)
        {
            for (int largo = Math.Min(LargoMaximoSilaba, escrito.Length); largo > 0; largo--)
            {
                string candidata = escrito[^largo..];
                if (Silabas.Contains(candidata.ToLowerInvariant()) || EsErhua(candidata))
                    return candidata;
            }
            return null;
        }

        // Sílaba con la -r del erhua (nar → nǎr). La r no es vocal, así que el tono queda en la
        // sílaba base sin tratarla aparte. "er" no lleva otra r.
        private static bool EsErhua(string candidata)
        {
            string minuscula = candidata.ToLowerInvariant();
            if (minuscula.Length < 2 || minuscula[^1] != 'r')
                return false;

            string basica = minuscula[..^1];
            return basica != "er" && Silabas.Contains(basica);
        }

        private static string AplicarTono(string silaba, int tono)
        {
            string minuscula = silaba.ToLowerInvariant().Replace('v', 'ü');

            // Reglas de ubicación del tono: a o e si están; en "ou" va en la o; si no, en la última vocal.
            int posicion = minuscula.IndexOf('a');
            if (posicion < 0)
                posicion = minuscula.IndexOf('e');
            if (posicion < 0)
                posicion = minuscula.IndexOf("ou", StringComparison.Ordinal);
            if (posicion < 0)
                posicion = minuscula.LastIndexOfAny(Vocales.ToCharArray());

            char[] resultado = silaba.ToCharArray();
            for (int i = 0; i < resultado.Length; i++)
            {
                bool mayuscula = char.IsUpper(silaba[i]);
                char letra = minuscula[i];

                if (i == posicion && tono < 5)
                {
                    int vocal = Vocales.IndexOf(letra);
                    letra = (mayuscula ? VocalesConTonoMayuscula : VocalesConTono)[vocal][tono - 1];
                }
                else if (mayuscula)
                {
                    letra = char.ToUpperInvariant(letra);
                }

                resultado[i] = letra;
            }
            return new string(resultado);
        }
    }
}
