using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace CalculaExpressao
{
    public static class ExtensionMethods
    {
        private static Object lockMe = new Object();

        public static bool NumeroPar(this int value)
        {
            if (value % 2 == 0)
                return true;
            else
                return false;
        }

        public static short GetValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());

            object o = fieldInfo.GetValue(value);

            return Convert.ToInt16(o);
        }

        public static string ToShortDateString_ConsultaSQL_BETWEEN_INICIO(this DateTime value)
        {
            DateTime data = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
            return data.ToString();
        }

        public static string ToShortDateString_ConsultaSQL_BETWEEN_FINAL(this DateTime value)
        {
            DateTime data = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
            return data.ToString();
        }

        public static T ToEnum<T>(this int valor)
        {
            return (T)Enum.Parse(typeof(T), valor.ToString());
        }

        #region + Decimal

        public static Decimal Truncar2Casas(this Decimal valor)
        {
            valor *= 100;
            valor = Math.Truncate(valor);
            valor /= 100;

            return valor;
        }

        public static Decimal Truncar3Casas(this Decimal valor)
        {
            valor *= 1000;
            valor = Math.Truncate(valor);
            valor /= 1000;

            return valor;
        }

        public static Decimal Truncar4Casas(this Decimal valor)
        {
            valor *= 10000;
            valor = Math.Truncate(valor);
            valor /= 10000;

            return valor;
        }

        public static Decimal ToDecimal2(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F2"));
        }

        public static Decimal ToDecimal3(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F3"));
        }

        public static Decimal ToDecimal4(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F4"));
        }

        public static Decimal ToDecimal6(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F6"));
        }
        public static Decimal ToDecimal8(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F8"));
        }
        public static Decimal ToDecimal10(this Decimal valor)
        {
            return Decimal.Parse(valor.ToString("F10"));
        }
        public static Decimal ToParteFracionaria(this Decimal valor)
        {
            return (valor - (int)valor);
        }
        /// <summary>
        /// Converte um decimal negativo em positivo
        /// <para>Exemplo</para>
        /// <para>decimal d = -100;</para>
        /// <para>d = d.ToDecimalPositivo()</para>
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static Decimal ToDecimalPositivo(this Decimal valor)
        {
            //if(valor < 0)
            //    valor = (valor * (-1));

            //return valor;
            return Math.Abs(valor);
        }
        public static Decimal ToDecimalNegativo(this Decimal valor)
        {
            if (valor < 0)
                return valor;
            else
                return (valor * (-1));
        }
        public static Decimal ToDecimalCemPorcento(this Decimal valor)
        {
            return (valor * 100);
        }

        public static Double ToDouble(this object valor)
        {         
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Convert.ToDouble(Convert.ToDecimal(valor));
        }
        public static decimal ToUltimoDigito(this decimal campo)
        {
            return Convert.ToDecimal(campo.ToString().Last().ToString());
        }

        /// <summary>
        /// Converte um decimal para string com duas casas decimais
        /// <para>Ex:   2,00</para>
        /// <para>para: 2.00</para>
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string ToMoedaStringDecimal2(this Decimal? valor)
        {
            return valor.GetValueOrDefault().ToString("N2").Replace(".", "").TrocaVirgulaPorPonto();
        }
        public static string ToMoedaStringDecimal3(this Decimal? valor)
        {
            return valor.GetValueOrDefault().ToString("N3").Replace(".", "").TrocaVirgulaPorPonto();
        }
        public static string ToMoedaStringDecimal4(this Decimal? valor)
        {
            return valor.GetValueOrDefault().ToString("N4").Replace(".", "").TrocaVirgulaPorPonto();
        }
        #endregion

        #region + Double

        public static double ToDoublePositivo(this double value)
        {
            return Math.Abs(value);
        }

        #endregion

        #region + Arquivos
        public static byte[] ConvertArquivoEmBinario(this string caminhoLocal)
        {
            return System.IO.File.ReadAllBytes(caminhoLocal);
        }

        public static FileStream ToArquivoGravar(this string caminhoLocal, byte[] _ByteArray, FileMode filemode = FileMode.Create, FileAccess fileacesso = FileAccess.ReadWrite)
        {
            FileStream _FileStream;
            using (_FileStream = new FileStream(caminhoLocal, filemode, fileacesso))
            {
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
                _FileStream.Close();
            }
            return _FileStream;
        }
        public static FileStream ToArquivoAbrir(this string caminhoLocal)
        {
            return new FileStream(caminhoLocal, FileMode.Open, FileAccess.Read);
        }

        public static Stream StringToStream(this string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static StreamReader StreamToStreamReader(this Stream s)
        {
            return new StreamReader(s);
        }

        #endregion

        #region + String

        public static string ConvertStringToHex(this string input)
        {
            var he = string.Empty;
            foreach (char c in input)
                he += ((int)c).ToString("x");

            return he;
        }
        public static bool ValidarEAN13(this string CodigoEAN13)
        {
            if (CodigoEAN13.ENulaVazia()) return false;

            bool result = (CodigoEAN13.Length == 13);
            if (result)
            {
                const string checkSum = "131313131313";

                int digito = int.Parse(CodigoEAN13[CodigoEAN13.Length - 1].ToString());
                string ean = CodigoEAN13.Substring(0, CodigoEAN13.Length - 1);

                int sum = 0;
                for (int i = 0; i <= ean.Length - 1; i++)
                {
                    sum += int.Parse(ean[i].ToString()) * int.Parse(checkSum[i].ToString());
                }
                int calculo = 10 - (sum % 10);
                result = (digito == calculo);
            }
            return result;
        }
        

        public static List<string> QuebraEmTamanhoFixo(this string valor, int tam)
        {
            List<string> quebrados = new List<string>();


            if (valor.Length > tam)
            {
                valor = valor.Replace("\r", "").Replace("\n", "");

                int partes = valor.Length / tam;
                int sobra = valor.Length % tam;
                int inicio = 0;

                for (int i = 0; i < partes; i++)
                {
                    inicio = (i * tam);
                    quebrados.Add(valor.Substring(inicio, tam));
                }

                if (sobra > 0)
                {
                    inicio = (partes * tam);
                    quebrados.Add(valor.Substring(inicio, sobra));
                }
            }
            else
            {
                quebrados.Add(valor);
            }

            return quebrados;
        }

        public static string RemovePIPE(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace("|", "").Trim();

            return valor;
        }
        public static string Formatar(this string valor, params object[] o)
        {
            return string.Format(valor, o);
        }
        
        public static string ToTrimOrEmpty(this string valor)
        {
            return String.IsNullOrEmpty(valor) ? string.Empty : valor.Trim();
        }

        //public static bool TagXMLEDigito(this char valor)
        //{
        //    if (valor.Equals('v')) return true;
        //    else if (valor.Equals('p')) return true;
        //    else if (valor.Equals('q')) return true;
        // //   else if (valor.Equals('v')) return true;
        //    return false;
        //}
        public static bool ENulaVazia(this string valor)
        {
            return String.IsNullOrEmpty(valor);
        }

        public static string TrocaVirgulaPorPonto(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace(",", ".").Trim();

            return valor;
        }
        public static string TrocaPontoPorVirgula(this string valor)
        {
            if (!valor.ENulaVazia() && valor.Contains(".."))
            {
                valor = valor.Replace("..", ",").Trim();
            }
            else
            {
                if (!valor.ENulaVazia())
                    valor = valor.Replace(".", ",").Trim();
            }
            return valor;

        }

        public static string LimparCodigoBarras(this string valor)
        {
            if (valor.ENulaVazia())
                return "";

            return valor.Replace(".", "").Replace("-", "").Replace("/", "").Replace(" ", "").Replace(@"\", "");
        }

        public static string LimparEspacos(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace(" ", "").Trim();

            return valor;
        }

        public static string LimparPontoEVirgula(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace(".", "").Replace(",", "").Trim();

            return valor;
        }
        public static string LimparBarra(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace(@"\", "").Replace(@"/", "").Trim();

            return valor;
        }

        public static string LimparCep(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace(".", "").Replace("-", "").Trim();

            return valor;
        }

        public static string LimparTelefone(this string valor, bool tirarZeroDDD = false)
        {
            if (!valor.ENulaVazia())
            {
                valor = valor.LimparNumero();// valor.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace(" ", "").Trim();
                if (tirarZeroDDD)
                {
                    if (valor.Length > 0)
                    {
                        if (valor[0].Equals('0'))
                            valor = valor.Substring(1, valor.Length - 1);
                    }
                }
            }

            return valor;

        }

        public static string LimparInscricaoEstadual(this string valor)
        {
            if (!valor.ENulaVazia())
                valor = valor.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

            return valor;
        }

        public static DateTime ToDateTime(this string valor, string Formato = "")
        {
            try
            {
                if (Formato == "")
                {
                    if (string.IsNullOrEmpty(valor))
                        return DateTime.MinValue;
                    else
                        return Convert.ToDateTime(valor);
                }
                else
                {
                    if (string.IsNullOrEmpty(valor))
                        return DateTime.MinValue;
                    else
                        return DateTime.ParseExact(valor, Formato, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return DateTime.MinValue;
            }

        }

        public static TimeSpan ToTimeSpan(this string valor)
        {
            try
            {
                if (string.IsNullOrEmpty(valor))
                    return new TimeSpan(0, 0, 0);
                else
                    return TimeSpan.Parse(valor);

            }
            catch
            {
                return new TimeSpan(0, 0, 0);
            }

        }

        public static TimeSpan ToTimeSpan(this double valor)
        {
            try
            {
                if (valor == 0)
                    return new TimeSpan(0, 0, 0);
                else
                    return TimeSpan.FromMinutes(valor);

            }
            catch
            {
                return new TimeSpan(0, 0, 0);
            }

        }

        public static Int32 ToInt32(this string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return 0;
            else
                return Convert.ToInt32(valor);
        }

        public static Decimal ToDecimal(this string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return 0;
            else
                return Convert.ToDecimal(valor);
        }

        public static string LimparAcentuacao(this string campo)
        {
            if (campo == null) return string.Empty;

            try
            {
                campo = (campo.Contains("Ã")) ? campo.Replace("Ã", "A") : campo;
                campo = (campo.Contains("Á")) ? campo.Replace("Á", "A") : campo;
                campo = (campo.Contains("À")) ? campo.Replace("À", "A") : campo;
                campo = (campo.Contains("Â")) ? campo.Replace("Â", "A") : campo;
                campo = (campo.Contains("Õ")) ? campo.Replace("Õ", "O") : campo;
                campo = (campo.Contains("Ó")) ? campo.Replace("Ó", "O") : campo;
                campo = (campo.Contains("Ô")) ? campo.Replace("Ô", "O") : campo;
                campo = (campo.Contains("Ñ")) ? campo.Replace("Ñ", "N") : campo;
                campo = (campo.Contains("É")) ? campo.Replace("É", "E") : campo;
                campo = (campo.Contains("Ê")) ? campo.Replace("Ê", "E") : campo;
                campo = (campo.Contains("Ü")) ? campo.Replace("Ü", "U") : campo;
                campo = (campo.Contains("Ú")) ? campo.Replace("Ú", "U") : campo;
                campo = (campo.Contains("Í")) ? campo.Replace("Í", "I") : campo;
                campo = (campo.Contains("Ç")) ? campo.Replace("Ç", "C") : campo;
                campo = (campo.Contains("ã")) ? campo.Replace("ã", "a") : campo;
                campo = (campo.Contains("á")) ? campo.Replace("á", "a") : campo;
                campo = (campo.Contains("à")) ? campo.Replace("à", "a") : campo;
                campo = (campo.Contains("â")) ? campo.Replace("â", "a") : campo;
                campo = (campo.Contains("õ")) ? campo.Replace("õ", "o") : campo;
                campo = (campo.Contains("ó")) ? campo.Replace("ó", "o") : campo;
                campo = (campo.Contains("ô")) ? campo.Replace("ô", "o") : campo;
                campo = (campo.Contains("ñ")) ? campo.Replace("ñ", "n") : campo;
                campo = (campo.Contains("é")) ? campo.Replace("é", "e") : campo;
                campo = (campo.Contains("ê")) ? campo.Replace("ê", "e") : campo;
                campo = (campo.Contains("ü")) ? campo.Replace("ü", "u") : campo;
                campo = (campo.Contains("ú")) ? campo.Replace("ú", "u") : campo;
                campo = (campo.Contains("í")) ? campo.Replace("í", "i") : campo;
                campo = (campo.Contains("ç")) ? campo.Replace("ç", "c") : campo;
                campo = campo.Replace("*", "").Replace("º", "").Replace("  ", " ").Replace("'", "").Replace("--", "").Replace("–", "-").Replace("'", "").Trim();
            }
            catch
            {
                throw;
            }
            return campo;
        }

        public static string LimparCaracteresEspeciais(this string campo)
        {
            if (!campo.ENulaVazia())
                campo = campo.Replace("--", "");

            campo = Regex.Replace(campo, @"[\u0000-\u001F]", string.Empty);

            return campo;
        }

        public static string LimparCaracteresEspeciaisUTF8(this string campo)
        {
            UTF8Encoding encorder = new UTF8Encoding();
            byte[] utfbytes = Encoding.UTF8.GetBytes(campo);
            campo = encorder.GetString(utfbytes);
            return campo;
        }
       

        public static string GetNomeArquivo(this string Caminho)
        {
            string[] lista = Caminho.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            if (lista.Length == 0)
                return "";
            else
                return lista[lista.Length - 1];
        }
        public static string GetExtensaoArquivo(this string Caminho)
        {
            string[] lista = Caminho.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            string Nome = "";
            if (lista.Length == 0)
                Nome = "";
            else
                Nome = lista[lista.Length - 1];

            string[] lista2 = Nome.Split(new string[] { @"." }, StringSplitOptions.RemoveEmptyEntries);
            if (lista2.Length == 0)
                return "";
            else
                return lista2[lista2.Length - 1];
        }

        /// <summary>
        /// Converte uma string de 6 posições ou 8 posições em uma DateTime (Data)
        /// <code>ddMMyyyy</code>
        /// </summary>
        /// <param name="Valor">String com 6 ou 8 posições que representa uma Data</param>
        /// <returns></returns>
        public static DateTime GetData(this string Valor)
        {
            Valor = LimparNumero(Valor);

            if (Valor.Length != 6 && Valor.Length != 8)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            DateTime.TryParse(Valor.Substring(0, 2) + "/" + Valor.Substring(2, 2) + "/" + Valor.Substring(4, Valor.Length - 4), out dt);

            return dt;

        }

        /// <summary>
        /// Converte uma string de 6 posições ou 8 posições em uma DateTime (Data)
        /// </summary>
        /// <param name="Valor">String com 6 ou 8 posições que representa uma Data</param>
        /// <returns></returns>
        public static DateTime GetData(this StringBuilder ValorSB)
        {

            string Valor = LimparNumero(ValorSB.ToString());

            if (Valor.Length != 6 && Valor.Length != 8)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            DateTime.TryParse(Valor.Substring(0, 2) + "/" + Valor.Substring(2, 2) + "/" + Valor.Substring(4, Valor.Length - 4), out dt);

            return dt;

        }

        /// <summary>
        /// Converte uma string de 6 posições em uma DateTime (Hora)
        /// </summary>
        /// <param name="Valor">String com 6 que representa um Horário</param>
        /// <returns></returns>
        public static DateTime GetTime(this string Valor)
        {
            Valor = LimparNumero(Valor);

            if (Valor.Length != 6)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            DateTime.TryParse(Valor.Substring(0, 2) + ":" + Valor.Substring(2, 2) + ":" + Valor.Substring(4, 2), out dt);

            return dt;

        }

        /// <summary>
        /// Converte uma string de 6 posições em uma DateTime (Hora)
        /// </summary>
        /// <param name="Valor">String com 6 que representa um Horário</param>
        /// <returns></returns>
        public static DateTime GetTime(this StringBuilder ValorSB)
        {
            string Valor = LimparNumero(ValorSB.ToString());

            if (Valor.Length != 6)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            DateTime.TryParse(Valor.Substring(0, 2) + ":" + Valor.Substring(2, 2) + ":" + Valor.Substring(4, 2), out dt);

            return dt;

        }

        public static DayOfWeek[] GetDiasSemana(this string Valor)
        {
            List<DayOfWeek> dias = null;

            foreach (string i in Valor.Split(','))
            {
                switch (i.ToUpper().Trim().Replace("Á", "A"))
                {
                    case "SEG":
                        dias.Add(DayOfWeek.Monday);
                        break;
                    case "TER":
                        dias.Add(DayOfWeek.Tuesday);
                        break;
                    case "QUA":
                        dias.Add(DayOfWeek.Wednesday);
                        break;
                    case "QUI":
                        dias.Add(DayOfWeek.Thursday);
                        break;
                    case "SEX":
                        dias.Add(DayOfWeek.Friday);
                        break;
                    case "SAB":
                        dias.Add(DayOfWeek.Saturday);
                        break;
                    case "DOM":
                        dias.Add(DayOfWeek.Sunday);
                        break;
                }
            }


            return dias.ToArray();

        }

        public static int GetDiaDaSemana(this DateTime Valor)
        {
            return (int)Valor.DayOfWeek;

        }

        public static string ToInsert(this string txt, int posicao, string valor)
        {
            return txt.Insert(posicao, valor);
        }

        public static string LimparNumero(this string campo)
        {
            string temp = string.Empty;
            if (campo == null) return string.Empty;
            try
            {
                var o = campo.ToArray();

                foreach (var caracter in o)
                {
                    if (Char.IsDigit(caracter))
                        temp += caracter;
                }

                return temp;
            }
            catch
            {
                throw;
            }
        }

       
       
        public static string TiraMascaraCnpjCpf(this string Valor)
        {
            try
            {
                string Generico = "";

                foreach (char c in Valor.ToCharArray())
                {
                    if (Numero(c))
                        Generico += c.ToString();
                }

                return Generico.ToString();
            }
            catch
            {
                return Valor;
            }
        }

        private static bool Numero(char c)
        {
            int N = (int)c;

            if (N < 48 || N > 57)
                return false;
            else
                return true;

        }

        public static string FormatarMascara(this string valor, string mascara)
        {
            StringBuilder dado = new StringBuilder();
            // remove caracteres nao numericos
            foreach (char c in valor)
            {
                if (Char.IsNumber(c))
                    dado.Append(c);
            }
            int indMascara = mascara.Length;
            int indCampo = dado.Length;
            for (; indCampo > 0 && indMascara > 0;)
            {
                if (mascara[--indMascara] == '#')
                    indCampo--;
            }
            StringBuilder saida = new StringBuilder();
            for (; indMascara < mascara.Length; indMascara++)
            {
                saida.Append((mascara[indMascara] == '#') ? dado[indCampo++] : mascara[indMascara]);
            }
            return saida.ToString();
        }

       

        

       
       

        /// <summary>
        /// Se retornar true é Válido
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static bool ValidaCPF(this string CPF)
        {

            if (CPF.Length != 11 || CPF == "00000000000" || CPF == "11111111111" ||
                CPF == "22222222222" || CPF == "33333333333" || CPF == "44444444444" ||
                CPF == "55555555555" || CPF == "66666666666" || CPF == "77777777777" ||
                CPF == "88888888888" || CPF == "99999999999")
                return false;
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += Convert.ToInt32(CPF.Substring(i, 1)) * (10 - i);
            int resto = 11 - (soma % 11);
            if (resto == 10 || resto == 11)
                resto = 0;
            if (resto != Convert.ToInt32(CPF.Substring(9, 1)))
                return false;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += Convert.ToInt32(CPF.Substring(i, 1)) * (11 - i);
            resto = 11 - (soma % 11);
            if (resto == 10 || resto == 11)
                resto = 0;
            if (resto != Convert.ToInt32(CPF.Substring(10, 1)))
                return false;

            return true;
        }
        /// <summary>
        /// Se retornar true é Válido
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static bool ValidaCNPJ(this string CNPJ)
        {

            if (CNPJ.Length == 14)
            {
                string CNPJ1, CNPJ2;
                double Soma, Mult;
                double Digito = 0;
                int i, j;
                int ContIni, ContFim;
                string controle;
                CNPJ1 = CNPJ.Substring(0, 12);
                CNPJ2 = CNPJ.Substring(CNPJ.Length - 2);
                controle = "";
                ContIni = 2;
                ContFim = 13;
                for (j = 1; j <= 2; j++)
                {
                    Soma = 0;
                    for (i = ContIni; i <= ContFim; i++)
                    {
                        Mult = (ContFim + 1 + j - i);
                        if (Mult > 9)
                        {
                            Mult = Mult - 8;
                        }
                        Soma = Soma + (Convert.ToInt32(CNPJ1.Substring(i - j - 1, 1)) * Mult);
                    }
                    if (j == 2)
                    {
                        Soma = Soma + (2 * Digito);
                    }
                    Digito = ((Soma * 10) % 11) % 10;
                    controle = controle + Digito;
                    ContIni = 3;
                    ContFim = 14;
                }
                if (controle != CNPJ2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool ValidaCPFouCNPJ(this string Valor)
        {
            if (Valor.Length == 14)
                return Valor.ValidaCNPJ();
            else if (Valor.Length == 11)
                return Valor.ValidaCPF();
            else
                return false;

        }

        public static string ConvertToUTF8(this string o)
        {
            o = o.ENulaVazia() ? "" : o;
            return Encoding.UTF8.GetString(
                    Encoding.Convert(
                    Encoding.Unicode,
                    Encoding.ASCII,
                    Encoding.Unicode.GetBytes(o)));
        }
       
        public static string ToHtml(this string texto)
        {
            return texto.Replace("\n", "<br/>");
        }
        public static string EncriptografarBase64(this string Texto)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Texto);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string DescriptografarBase64(this string Texto)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(Texto);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GetTipoTransacaoGerencial(this string transacao)
        {
            List<string> nota = new List<string>() { "TEN01", "TSN01", "TPC01", "TPV01" };
            List<string> notaDevolucao = new List<string>() { "TDE01", "TDS01" };
            List<string> ecf = new List<string>() { "ECF01" };
            List<string> ordemCarregamento = new List<string>() { "OCR01" };

            string retorno = string.Empty;
            if (nota.Contains(transacao))
            {
                retorno = "Nota";
            }
            else if (notaDevolucao.Contains(transacao))
            {
                retorno = "Nota Dev";
            }
            else if (ordemCarregamento.Contains(transacao))
            {
                retorno = "Nota OC";
            }
            else if (ecf.Contains(transacao))
            {
                retorno = "ECF";
            }
            return retorno;
        }

        public static string PeriodoDescricaoCurta(this string periodo)
        {
            string retorno = string.Empty;
            switch (periodo)
            {
                case "01": retorno = "Jan"; break;
                case "02": retorno = "Fev"; break;
                case "03": retorno = "Mar"; break;
                case "04": retorno = "Abr"; break;
                case "05": retorno = "Mai"; break;
                case "06": retorno = "Jun"; break;
                case "07": retorno = "Jul"; break;
                case "08": retorno = "Ago"; break;
                case "09": retorno = "Set"; break;
                case "10": retorno = "Out"; break;
                case "11": retorno = "Nov"; break;
                case "12": retorno = "Dez"; break;
                default:
                    break;
            }
            return retorno;
        }

        public static string PrimeiraMaiuscula(this string text)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            string Texto = textInfo.ToTitleCase(text.ToLower());

            return Texto;
        }

        public static bool ValidarEmail(this string text)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            if (rg.IsMatch(text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ComecaCom(this string text, string t)
        {
            if (text.Length > 1)
                if (text[0].ToString() == t) return true;

            return false;
        }

        public static string LimpaXML(this string xml)
        {
            //return new string(s.Normalize(System.Text.NormalizationForm.FormD).Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
            //string[] cEspeciais = { "#39", "---", "--", "'", "#", "\r\n", "\n", "\r" };
            //string[] caracter = { "\n", "\t", "\r", "> <", ">  <", ">   <", ">    <", ">     <" };

            List<string> caracteres = new List<string>() { "---", "--", "\r\n", "\n", "\t", "\r" };

            for (int i = 1; i <= 20; i++)
                caracteres.Add(">{0}<".Formatar(string.Empty.PadLeft(i, ' ')));

            string retorno = xml.Trim();

            for (int i = 0; i < caracteres.Count; i++)
            {
                while (retorno.Contains(caracteres[i]))
                {
                    //if (caracter[i] == "> <")
                    //    retorno = retorno.Replace(caracter[i], "><");
                    //else if (caracter[i] == ">  <")
                    //    retorno = retorno.Replace(caracter[i], "><");
                    //else if (caracter[i] == ">   <")
                    //    retorno = retorno.Replace(caracter[i], "><");
                    //else if (caracter[i] == ">    <")
                    //    retorno = retorno.Replace(caracter[i], "><");
                    //else if (caracter[i] == ">     <")
                    //    retorno = retorno.Replace(caracter[i], "><");
                    //else
                    if (caracteres[i].Contains(">") || caracteres[i].Contains("<"))
                        retorno = retorno.Replace(caracteres[i], "><");
                    else
                        retorno = retorno.Replace(caracteres[i], "");
                }
            }
            return retorno;
        }

        #region + Validar GTIN
        /// <summary>
        /// Fonte link:
        /// <para>http://stackoverflow.com/questions/10143547/how-do-i-validate-a-upc-or-ean-code</para>
        /// </summary>
        /// <param name="codigoBarras"></param>
        /// <returns></returns>
        public static bool IsGtinValido(this string codigoBarras)
        {
            //[0-9]{0}|[0-9]{8}|[0-9]{12,14}
            var _gtinRegex = new Regex("^(\\d{8}|\\d{12,14})$");

            if (!(_gtinRegex.IsMatch(codigoBarras))) return false;                                                                  // check if all digits and with 8, 12, 13 or 14 digits
            codigoBarras = codigoBarras.PadLeft(14, '0');                                                                           // stuff zeros at start to garantee 14 digits
            int[] mult = Enumerable.Range(0, 13).Select(i => ((int)(codigoBarras[i] - '0')) * ((i % 2 == 0) ? 3 : 1)).ToArray();    // STEP 1: without check digit, "Multiply value of each position" by 3 or 1
            int sum = mult.Sum();                                                                                                   // STEP 2: "Add results together to create sum"
            return (10 - (sum % 10)) % 10 == int.Parse(codigoBarras[13].ToString());                                                // STEP 3 Equivalent to "Subtract the sum from the nearest equal or higher multiple of ten = CHECK DIGIT"
        }
        #endregion

        #endregion

        #region + List

        public static T ClonarListaSemReferencia<T>(T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        public static List<T> Clonar<T>(this IEnumerable<T> oldList)
        {
            return new List<T>(oldList);
        }
        public static List<T> ToList<T>(this ArrayList arrayList)
        {
            List<T> list = new List<T>(arrayList.Count);
            foreach (T instance in arrayList)
            {
                list.Add(instance);
            }
            return list;
        }

        public static DataTable ListToDataTable<T>(this IEnumerable<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                Type propType = info.PropertyType;
                if (propType.IsGenericType &&
                    propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = Nullable.GetUnderlyingType(propType);
                }

                dt.Columns.Add(new DataColumn(info.Name, propType));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    object _valor = info.GetValue(t, null);
                    if (info.PropertyType == typeof(DateTime))
                    {
                        if (((DateTime)_valor) != DateTime.MinValue)
                            row[info.Name] = _valor;
                    }
                    else
                        row[info.Name] = _valor != null ? _valor : DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> en, Action<T> f)
        {
            foreach (var a in en) f(a);
            return en;
        }
       


       


        /// <summary>
        /// Atualizar um elemento da lista que satisfaz a condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// 
        public static bool Update<T>(this IEnumerable<T> source, Action<T> action)
        {
            bool r = false;
            foreach (var item in source)
            {
                action(item);
                r = true;
            }

            return r;
        }

      

      

    

        public static Collection<T> ToCollection<T>(this List<T> items)
        {
            Collection<T> collection = new Collection<T>();

            for (int i = 0; i < items.Count; i++)
            {
                collection.Add(items[i]);
            }

            return collection;
        }

        #endregion

        #region + DataTable

        /// <summary>
        /// Adiciona uma coluna vazia 
        /// <para>Ordenado por ela</para>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable DataTableAddColunaVazia(this DataTable table)
        {
            try
            {
                var orow = table.NewRow();
                orow["Codigo"] = -1;
                orow["Descricao"] = "";
                table.Rows.Add(orow);
                var view = table.AsDataView();
                view.Sort = "Codigo";

                return view.ToTable();
            }
            catch
            {

                throw;
            }

        }
        public static DataTable DataTableAddPermiteNulo(this DataTable table, string coluna, bool permiteNulos = true)
        {
            try
            {
                table.Columns[coluna].AllowDBNull = permiteNulos;
                return table;
            }
            catch
            {

                throw;
            }
        }

        public static DataTable DataTableAddColunaVazia(this DataTable table, string codigo, string descricao, bool permiteNulos = true)
        {
            try
            {
                table.Columns[codigo].AllowDBNull = permiteNulos;

                var orow = table.NewRow();
                orow[codigo] = permiteNulos ? null : "-1";
                orow[descricao] = "";

                table.Rows.Add(orow);
                return table;
            }
            catch
            {

                throw;
            }
        }
        public static DataTable DataTableAddColunaDados(this DataTable table, string codigo, string descricao, string vCodigo, string vDescricao)
        {
            try
            {
                var orow = table.NewRow();
                orow[codigo] = vCodigo;
                orow[descricao] = vDescricao;

                table.Rows.Add(orow);
                return table;
            }
            catch
            {

                throw;
            }
        }

        public static List<T> DataTableToList<T>(this DataTable table)
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                T item = (T)Activator.CreateInstance(typeof(T));

                for (int c = 0; c < table.Columns.Count; c++)
                {
                    var property = properties.Where(p => p.Name.ToLower().Equals(table.Columns[c].ColumnName.ToLower())).FirstOrDefault();
                    try
                    {
                        if (property != null)
                        {
                            var tipo = table.Rows[i][c].GetType();
                            if (tipo.Equals(typeof(System.DBNull)))
                                property.SetValue(item, null, null);
                            else
                                property.SetValue(item, table.Rows[i][c], null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
                    }

                }
                list.Add(item);
            }
            return list;
        }
        public static ParallelLoopResult ForParaleloOrdenado<TSource>(this ParallelQuery<TSource> colecao, Action<TSource> acao)
        {
            int current = 0;
            object lockCurrent = new object();

            return Parallel.For(0, colecao.Count(), new ParallelOptions { MaxDegreeOfParallelism = 2 }, (ii, loopState) =>
            {
                int thisCurrent = 0;
                lock (lockCurrent)
                {
                    thisCurrent = current;
                    current++;
                }

                var o = colecao.ElementAt(thisCurrent);
                if (o != null) acao(o);
            });
        }

        /// <summary>
        /// http://stackoverflow.com/questions/3639768/parallel-foreach-ordered-execution
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="colecao"></param>
        /// <param name="acao"></param>
        public static ParallelLoopResult ForParalelo<TSource>(this ICollection<TSource> colecao, Action<TSource> acao)
        {
            int current = 0;
            object lockCurrent = new object();

            return Parallel.For(0, colecao.Count, new ParallelOptions { MaxDegreeOfParallelism = 2 }, (ii, loopState) =>
            {
                int thisCurrent = 0;
                lock (lockCurrent)
                {
                    thisCurrent = current;
                    current++;
                }

                lock (colecao)
                {
                    var o = colecao.ElementAt(thisCurrent);
                    if (o != null) acao(o);
                }
            });
        }

        public static Task<List<T>> DataTableToListParallel<T>(this DataTable table)
        {
            var list = new List<T>();

            //return Task.Factory.StartNew(() => {

            //    var properties = typeof(T).GetProperties();

            //    BonzayParallel.For(0, table.Rows.Count, i =>
            //    {
            //        lock (lockMe)
            //        {
            //            T item = (T)Activator.CreateInstance(typeof(T));

            //            for (int c = 0; c < table.Columns.Count; c++)
            //            {
            //                var property = properties.Where(p => p.Name.ToLower().Equals(table.Columns[c].ColumnName.ToLower())).FirstOrDefault();
            //                try
            //                {
            //                    if (property != null)
            //                    {
            //                        var tipo = table.Rows[i][c].GetType();
            //                        if (tipo.Equals(typeof(System.DBNull)))
            //                            property.SetValue(item, null, null);
            //                        else
            //                            property.SetValue(item, table.Rows[i][c], null);
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
            //                }
            //            }
            //            list.Add(item);
            //        }
            //    });

            object lockCurrent = new object();

            return Task.Factory.StartNew(() =>
            {
                var properties = typeof(T).GetProperties();

                Parallel.For(0, table.Rows.Count, new ParallelOptions { MaxDegreeOfParallelism = 2 }, i =>
                {
                    lock (lockCurrent)
                    {
                        T item = (T)Activator.CreateInstance(typeof(T));

                        Parallel.For(0, table.Columns.Count, new ParallelOptions { MaxDegreeOfParallelism = 2 }, c =>
                        //     for (int c = 0; c < table.Columns.Count; c++)
                        {
                            var property = properties.Where(p => p.Name.ToLower().Equals(table.Columns[c].ColumnName.ToLower())).FirstOrDefault();
                            try
                            {
                                if (property != null)
                                {
                                    var tipo = table.Rows[i][c].GetType();
                                    if (tipo.Equals(typeof(System.DBNull)))
                                        property.SetValue(item, null, null);
                                    else
                                        property.SetValue(item, table.Rows[i][c], null);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
                            }
                        });
                        list.Add(item);
                    }
                });
                return list;
            });
        }


        public static ArrayList DataTableToList2(this DataTable table)
        {
            var list = new ArrayList();
            var properties = table.Columns;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var o = new object();

                //foreach (DataColumn col in table.Columns)
                //{
                //    var property = new MemberInfo();
                //    property.Name = col.ColumnName;

                //    var tipo = table.Rows[i][col].GetType();
                //    if (tipo.Equals(typeof(System.DBNull)))
                //        property.SetValue(item, null, null);
                //    else
                //        property.SetValue(item, table.Rows[i][c], null);
                //}

                for (int c = 0; c < table.Columns.Count; c++)
                {
                    //var property = properties.Where(p => p.Name.ToLower().Equals(table.Columns[c].ColumnName.ToLower())).FirstOrDefault();
                    //try
                    //{
                    //    if (property != null)
                    //    {
                    //        var tipo = table.Rows[i][c].GetType();
                    //        if (tipo.Equals(typeof(System.DBNull)))
                    //            property.SetValue(item, null, null);
                    //        else
                    //            property.SetValue(item, table.Rows[i][c], null);
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
                    //}

                }
                // list.Add(item);
            }
            return list;
        }

        public static T DataTableToObject<T>(this DataTable table)
        {
            var properties = typeof(T).GetProperties();
            T item = (T)Activator.CreateInstance(typeof(T));

            if (table.Rows.Count > 0)
            {
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    var property = properties.Where(p => p.Name.ToLower().Equals(table.Columns[c].ColumnName.ToLower())).FirstOrDefault();
                    try
                    {
                        if (property != null)
                        {
                            var tipo = table.Rows[0][c].GetType();
                            if (tipo.Equals(typeof(System.DBNull)))
                                property.SetValue(item, null, null);
                            else
                                property.SetValue(item, table.Rows[0][c], null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
                    }

                }
            }
            else
                item = default(T);

            return item;
        }

        #endregion

        #region + DataReader
        public static T DataReaderToObject<T>(this IDataReader reader)
        {
            try
            {
                var properties = typeof(T).GetProperties();
                T item = (T)Activator.CreateInstance(typeof(T));

                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var property = properties.Where(p => p.Name.ToLower().Equals(reader.GetName(i).ToLower())).FirstOrDefault();
                        try
                        {
                            if (property != null)
                            {
                                var tipo = reader[i].GetType();
                                if (tipo.Equals(typeof(System.DBNull)))
                                    property.SetValue(item, null, null);
                                else
                                    property.SetValue(item, reader[i], null);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + string.Format(" Falha ao percorrer coleção, propriedade {0}", property.Name));
                        }
                    }
                }
                else
                    item = default(T);

                return item;
            }
            catch
            {
                throw;
            }
        }

        public static List<T> DataReaderToList<T>(this IDataReader reader)
        {
            try
            {
                var list = new List<T>();
                var properties = typeof(T).GetProperties();

                while (reader.Read())
                {
                    T item = (T)Activator.CreateInstance(typeof(T));

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var property = properties.Where(p => p.Name.ToLower().Equals(reader.GetName(i).ToLower())).FirstOrDefault();

                        if (property != null)
                        {
                            var tipo = reader[i].GetType();
                            if (tipo.Equals(typeof(System.DBNull)))
                                property.SetValue(item, null, null);
                            else
                                property.SetValue(item, reader[i], null);
                        }
                    }

                    list.Add(item);
                }
                return list;
            }
            catch
            {
                throw;
            }
        }



        #endregion

        #region + DateTime



        public static TimeSpan ToTimeSpan(this Object valor)
        {
            if(valor == null)
                return new TimeSpan(0, 0, 0);

            if (valor is TimeSpan)
                return (TimeSpan)valor;

   
            return new TimeSpan(0, 0, 0);
        }

        public static bool IsFimdeSemana(this DateTime adSource)
        {

            if (adSource.DayOfWeek == DayOfWeek.Saturday || adSource.DayOfWeek == DayOfWeek.Sunday)
                return true;
            else
                return false;

        }
       
        public static bool IsDiaX(this DateTime adSource, DayOfWeek diaSemana)
        {

            if (adSource.DayOfWeek == diaSemana)
                return true;
            else
                return false;

        }

        public static bool IsSegundaFeira(this DateTime adSource)
        {

            if (adSource.DayOfWeek == DayOfWeek.Monday)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Retorna primeiro dia do mês
        /// </summary>
        /// <param name="adSource"></param>
        /// <returns></returns>
        public static DateTime ToPrimeiroDiaMes(this DateTime adSource)
        {
            return new DateTime(adSource.Year, adSource.Month, 1);
        }
        public static DateTime ToPrimeiroDiaMes(this DateTime adSource, int mes, int ano)
        {
            return new DateTime(ano, mes, 1, 0, 0, 0);
        }
        /// <summary>
        /// Retorna último dia do mês
        /// </summary>
        /// <param name="adSource"></param>
        /// <returns></returns>
        //public static DateTime ToUltimoDiaMes(this DateTime adSource)
        //{
        //    int dias = DateTime.DaysInMonth(adSource.Year, adSource.Month);
        //    return new DateTime(adSource.Year, adSource.Month, dias);
        //}
        //public static DateTime ToUltimoDiaMes(this DateTime adSource, int mes, int ano)
        //{
        //    int dias = DateTime.DaysInMonth(ano, mes);
        //    return new DateTime(ano, mes, dias, 23, 59, 59);
        //}

        public static string ToPeriodo(this string periodo, string exercicio)
        {
            var dataInicial = new DateTime(Convert.ToInt32(exercicio), Convert.ToInt32(periodo), 1);
            int dias = DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month);
            var dataFinal = new DateTime(Convert.ToInt32(exercicio), Convert.ToInt32(periodo), dias);

            return string.Format(" '{0}' and '{1}'", dataInicial.ToShortDateString_ConsultaSQL_BETWEEN_INICIO(), dataFinal.ToShortDateString_ConsultaSQL_BETWEEN_FINAL());
        }

        public static string ToPeriodoInicial(this string periodo, string exercicio)
        {
            var dataInicial = new DateTime(Convert.ToInt32(exercicio), Convert.ToInt32(periodo), 1);
            return dataInicial.ToShortDateString_ConsultaSQL_BETWEEN_INICIO();
        }

        public static string ToPeriodoFinal(this string periodo, string exercicio)
        {
            var dataInicial = new DateTime(Convert.ToInt32(exercicio), Convert.ToInt32(periodo), 1);
            int dias = DateTime.DaysInMonth(dataInicial.Year, dataInicial.Month);
            var dataFinal = new DateTime(Convert.ToInt32(exercicio), Convert.ToInt32(periodo), dias);

            return dataFinal.ToShortDateString_ConsultaSQL_BETWEEN_FINAL();
        }

        public static double TotalMinutes(this string valor)
        {
            try
            {
                var aux = valor.Split(':');
                double minutos = 0;

                if (aux.Count() == 2)
                {
                    minutos = aux[0].ToInt32() * 60;
                    minutos += aux[1].ToInt32();

                }
                else
                    return 0;

                return minutos;
            }
            catch
            {
                return 0;
            }
        }

        public static string FromMinutes(this double valor)
        {
            try
            {

                if (valor == 0)
                    return "00:00:00";              
               
                else if(valor < 60)
                    return "00:" +(int)valor+":"+ (valor - (int)valor) * 100;

                else if (valor >= 60)
                {
                    var horas = (int)(valor / 60);
                    var minutos = (int)(valor - (horas * 60));
                    var segundos = (valor - (valor - (horas * 60))) * 100;
                    return horas + ":" + minutos + ":" + segundos;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static bool IsBetween(this DateTime dt, DateTime inicial, DateTime final)
        {
            return dt >= inicial && dt <= final;
        }

        public static bool IsBetweenHora(this TimeSpan hora, TimeSpan horaInicial, TimeSpan horaFinal)
        {
            return (hora >= horaInicial && hora <= horaFinal);
        }

        public static string ToStringHHmm(this TimeSpan valor)
        {
                       
            return string.Format("{0}:{1:00}", (valor.Days * 24 + valor.Hours), valor.Minutes);

            //var str = valor.ToString();
            //var lst = str.Split(':');

            //return lst[0] + ":" + lst[1];
        }

        #endregion

        #region + Object

        /// <summary>
        /// Formata o campo de acordo com o tipo e o tamanho 
        /// </summary>        
        public static string FitStringLength(this object valor, int maxLength, int minLength, char FitChar, int maxStartPosition, bool maxTest, bool minTest, bool isNumber)
        {
            try
            {
                string result = "";
                string SringToBeFit = valor.ToString();

                if (maxTest == true)
                {
                    // max
                    if (SringToBeFit.Length > maxLength)
                    {
                        result += SringToBeFit.Substring(maxStartPosition, maxLength);
                    }
                }

                if (minTest == true)
                {
                    // min
                    if (SringToBeFit.Length <= minLength)
                    {
                        if (isNumber == true)
                        {
                            result += (string)(new string(FitChar, (minLength - SringToBeFit.Length)) + SringToBeFit);
                        }
                        else
                        {
                            result += (string)(SringToBeFit + new string(FitChar, (minLength - SringToBeFit.Length)));
                        }
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        public static Decimal ToDecimal(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Convert.ToDecimal(valor);
        }       

        public static int ToInt32(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(valor);
        }

        public static Int64 ToInt64(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Convert.ToInt64(valor);
        }

        public static int ToInt32ArrendondaParaCima(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Math.Ceiling(valor.ToDecimal()).ToInt32();
        }

        public static int? ToInt32DefaultNull(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(valor);
        }

        public static short ToShort(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return 0;
            else
                return Convert.ToInt16(valor);
        }

        public static string ToStringBRdata(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return null;
            else
                return Convert.ToString(valor);
        }
        public static string ToStringSQLBRdata(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return " like '%'";
            else
                return " ='{0}'".Formatar(valor);
        }
        public static string ToIntSQLBRdata(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return " like '%'";
            else
            {
                if (valor.ToInt32() == 0)
                    return " IS NULL";

                else
                    return " ={0}".Formatar(valor);

            }
        }
        public static short? ToShortDefaultNull(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return null;
            else
                return Convert.ToInt16(valor);
        }

        public static bool ToBool(this object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return false;
            else
                return Convert.ToBoolean(valor.ToShort());
        }

        public static DateTime ToDateTime(this object valor)
        {
            try
            {
                if (valor != null && string.IsNullOrEmpty(valor.ToString()))
                    return DateTime.MinValue;
                else
                    return Convert.ToDateTime(valor);
            }
            catch
            {
                return DateTime.MinValue;
            }

        }
        public static byte ToByte(this object valor)
        {
            try
            {
                if (valor != null && string.IsNullOrEmpty(valor.ToString()))
                    return 0;
                else
                    return Convert.ToByte(valor);
            }
            catch
            {
                return 0;
            }

        }
        public static bool IsNull(this object valor)
        {
            return valor == null;
        }
        #endregion

        #region + GZip
        public static string DescompactarGZip(this string gZip)
        {
            return DescompactarGZip(Convert.FromBase64String(gZip));
        }
        public static string DescompactarGZip(this byte[] gZip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gZip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int count;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memoryStream.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }
        #endregion


        

       

        public static string ToSQL(this List<int> lista, string coluna)
        {
            string SQL = "";

            foreach (int item in lista)
            {
                SQL = AddCondicao(SQL, "({0} = {1})".Formatar(coluna, item));
            }

            return SQL;
        }
        public static string ToSQLString(this List<int> lista, string coluna)
        {
            string SQL = "";

            foreach (int item in lista)
            {
                SQL = AddCondicao(SQL, "({0} = '{1}')".Formatar(coluna, item));
            }

            return SQL;
        }
        public static string ToSQL(this List<string> lista, string coluna)
        {
            string SQL = "";

            foreach (var item in lista)
            {
                SQL = AddCondicao(SQL, "({0} = {1})".Formatar(coluna, item));
            }

            return SQL;
        }


        private static string AddCondicao(string SQL, string Cond)
        {
            if (SQL.Length > 0)
            {
                SQL += " OR ";
            }
            SQL += Cond;
            return SQL;
        }

        public static IEnumerable<TResult> LeftOuterJoin<TSource, TInner, TKey, TResult>(this IEnumerable<TSource> source, IEnumerable<TInner> other, Func<TSource, TKey> func, Func<TInner, TKey> innerkey, Func<TSource, TInner, TResult> res)
        {
            return from f in source
                   join b in other on func.Invoke(f) equals innerkey.Invoke(b) into g
                   from result in g.DefaultIfEmpty()
                   select res.Invoke(f, result);
        }

        public static string GetNomeEnumeracao(this Type value, object index)
        {
            try
            {
                return Enum.ToObject(value, index).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GetValorPropriedadeToString<T>(this T o, string campo)
        {
            PropertyInfo info = o.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == campo.ToLower());

            if (info != null)
                return info.GetValue(o, null).ToString();

            return string.Empty;
        }
        public static object GetValorPropriedadeToObject<T>(this T o, string campo)
        {
            var valor = default(object);
            PropertyInfo info = o.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == campo.ToLower());

            if (info != null)
                valor = info.GetValue(o, null);

            return valor;
        }

        //public static object GetValorPropriedade(this Type value, object objeto, string campo)
        //{
        //    var properties = value.GetProperties();
        //    var propriedade = properties.Where(p => p.Name.ToLower().Equals(campo.ToLower())).FirstOrDefault();
        //    var valor = default(object);

        //    try
        //    {
        //        valor = propriedade.GetValue(objeto, null);
        //    }
        //    catch 
        //    {
        //        valor = null;
        //    }

        //    return valor;
        //}


        #region + Exception



        #endregion

    

    
    }
}
