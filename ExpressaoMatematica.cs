using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculaExpressao
{
    public class ExpressaoMatematica
    {

        string[] Operadores = new string[4] { "/","*","-","+" };
        char[] OperadoresChar = new char[4] { '/', '*', '-', '+' };

        public decimal Execulta(string _Expressao)
        {

            return CalculaProcedencia2(_Expressao);     
            
        }

        private decimal CalculaProcedencia2(string _Expressao)
        {
            try
            {
                string str;
                decimal valor = 0;

                int aux1 = -1;
                int aux2 = _Expressao.Count();
                for (int i = 0; i < _Expressao.Length; i++)
                {
                    if (_Expressao[i].ToString().Equals("(") && (i >= aux1 && i < aux2))
                        aux1 = i;
                    else if (_Expressao[i].ToString().Equals(")") && (i <= aux2 && aux2 > aux1))
                        aux2 = i;
                }

                if (aux1 != -1 && aux2 != _Expressao.Count())
                {
                    str = _Expressao.Substring(aux1+1, aux2 - aux1 - 1);
                    decimal aux3 = Calcula(str);
                    _Expressao = _Expressao.Replace("("+str+")", aux3.ToString());
                    valor += CalculaProcedencia2(_Expressao);
                }
                else
                {
                    valor += Calcula(_Expressao);
                }
                
                return valor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private decimal CalculaProcedencia(string _Expressao)
        {
            try
            {
                string str;
                decimal valor = 0;
                for (int i = 0; i < _Expressao.Length;)
                {
                    if (_Expressao.Contains("("))
                    {
                        str = _Expressao.Substring(_Expressao.IndexOf("(") + 1, (_Expressao.IndexOf(")") - _Expressao.IndexOf("(")) - 1);                        
                        valor += CalculaProcedencia(str);
                        i = _Expressao.IndexOf(")");
                    }
                    else
                    {
                        return Calcula(_Expressao);
                    }
                }

                return valor;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private decimal Calcula(string _Expressao)
        {
            try
            {
                List<string> tmp = new List<string>();
                tmp.Add(_Expressao);
                List<string> exp = DesmenbraExpressao2(_Expressao);

                decimal valor = 0;
                bool segundoValor = false;
                for (int i = 1; i < exp.Count(); i++)
                {
                    if (exp[i].Equals("/"))
                    {
                        if(segundoValor)
                            valor = (valor / exp[i + 1].ToDecimal());
                        else
                            valor += (exp[i - 1].ToDecimal() / exp[i + 1].ToDecimal());

                        segundoValor = true;
                    }                        
                    if (exp[i].Equals("*"))
                    {
                        if (segundoValor)
                            valor = (valor * exp[i + 1].ToDecimal());
                        else
                            valor += (exp[i - 1].ToDecimal() * exp[i + 1].ToDecimal());
                        segundoValor = true;
                    }                        
                    if (exp[i].Equals("-"))
                    {
                        if (segundoValor)
                            valor = (valor - exp[i + 1].ToDecimal());
                        else
                            valor += (exp[i - 1].ToDecimal() - exp[i + 1].ToDecimal());
                        segundoValor = true;
                    }                        
                    if (exp[i].Equals("+"))
                    {
                        if (segundoValor)
                            valor = (valor + exp[i + 1].ToDecimal());
                        else
                            valor += (exp[i - 1].ToDecimal() + exp[i + 1].ToDecimal());
                        segundoValor = true;
                    }
                        
                }
                return valor;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<string> DesmenbraExpressao(List<string> _Expressao)
        {
            try
            {
                List<string> retorno = new List<string>();

                foreach (var str in _Expressao)
                {
                    foreach (var ope in Operadores)
                    {
                        if (str.Count() > 1 && str.Contains(ope))
                        {
                            string[] tmp = str.Split(Convert.ToChar(ope));
                            for (int i = 0; i < tmp.Count(); i++)
                            {
                                if (i == 0 || (i % 2 != 0 && i != 1))
                                    retorno.Add(tmp[i]);
                                else
                                {
                                    retorno.Add(ope);
                                    retorno.Add(tmp[i]);
                                }
                            }
                            // retorno = DesmenbraExpressao(retorno);
                        }

                    }


                }
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<string> DesmenbraExpressao2(string _Expressao)
        {
            try
            {

                var aux6 = _Expressao.Split(OperadoresChar);
                if (aux6.Length == 1 ? aux6[0].ToString().Length == _Expressao.Length : false)
                {
                    var aux5 = new List<string>();
                    aux5.Add(_Expressao);
                    return aux5;
                }

                List<string> retorno = new List<string>();
                int indice = 0;
                foreach (var ope in Operadores)
                {
                    indice++;
                    string[] tmp = _Expressao.Split(Convert.ToChar(ope));
                    if (tmp.Count() > 1)
                    {
                        for (int i = 0; i < tmp.Count(); i++)
                        {
                            if (i == 0 || (i % 2 != 0 && i != 1))
                                retorno.Add(tmp[i]);
                            else
                            {
                                retorno.Add(ope);
                                //retorno.Add(tmp[i]);

                                var aux4 = DesmenbraExpressao2(tmp[i]);
                                foreach (var y in aux4)
                                    retorno.Add(y);

                                return retorno;
                            }
                        }
                    }
                }

                return retorno;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
