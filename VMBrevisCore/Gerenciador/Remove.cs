
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
            //var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
            var conecoes = XElement.Load(@"I:\MEU\VMBrevis\VMBrevisCore\ConecoesLocal.xml");
            string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
            var acoes = XElement.Load(@"I:\MEU\VMBrevis\VMBrevisCore\Acoes.xml");
            //var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
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
