using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using VMBrevisCore.Manipulador;

namespace VMBrevisCore.Gerenciador
{
    public class Remove : Dados
    {
        string caminhoBanco = "";
        public Remove(string caminho)
        {
            caminhoBanco = caminho;
        }
        /// <summary>
        /// Remoção de dados conforme parametros, cuidado é sempre bom na hora da remoção de infomações.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T RemoveT<T>(T objeto, int conexao)
        {
            List<SqlParameter> parametros = MontaParametros(objeto, Operacao.Alterar);
            //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
            var conexoes = XElement.Load(@caminhoBanco + @"\conexoesLocal.xml");
            string stringDeConexaoCorrente = "";//conexoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
            XNode noCorrente = conexoes.FirstNode;
            for (int indice = 1; indice <= conexao; indice++)
            {
                if (indice == conexao)
                {
                    stringDeConexaoCorrente = ((System.Xml.Linq.XElement)noCorrente).Value.ToString();
                    break;
                }
                noCorrente = noCorrente.NextNode;
                if (noCorrente.ToString().Equals(string.Empty))
                    break;
            }
            //var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
            var acoes = XElement.Load(@caminhoBanco + @"\Acoes.xml");
            string acao = acoes.XPathSelectElement("RemocaoFisica").Value;
            AbreConexao(new DadosConexao() { nome = stringDeConexaoCorrente, stringDeConexao = stringDeConexaoCorrente });
            string comando = acao
                .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                .Replace("COLUNASDADOS", parametros.Where(s => s.ParameterName == "ColunasValores").FirstOrDefault().Value.ToString())
                .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
            return ConverteDeSqlDataReaderParaT(CarregaDadosGenericoT<object>(comando), objeto);
        }
    }
}