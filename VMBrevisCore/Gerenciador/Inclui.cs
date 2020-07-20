using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using VMBrevisCore.Manipulador;

namespace VMBrevisCore.Gerenciador
{
    public class Inclui : Dados
    {
        public T IncluiT<T>(T objeto, int conexao)
        {
            List<SqlParameter> parametros = MontaParametros(objeto, Operacao.Inserir);
            var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
            //var conecoes = XElement.Load(@"I:\MEU\VMBrevis\VMBrevisCore\ConecoesLocal.xml");
            string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
            //var acoes = XElement.Load(@"I:\MEU\VMBrevis\VMBrevisCore\Acoes.xml");
            var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
            string acao = acoes.XPathSelectElement("Adicao").Value;
            AbreConexao(new DadosConexao() { nome = stringDeConexaoCorrente, stringDeConexao = stringDeConexaoCorrente });
            string comando = acao
                .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                .Replace("COLUNAS", parametros.Where(s => s.ParameterName == "Colunas").FirstOrDefault().Value.ToString())
                .Replace("DADOS", parametros.Where(s => s.ParameterName == "Valores").FirstOrDefault().Value.ToString());
            return ConverteDeSqlDataReaderParaT(CarregaDadosGenericoT<object>(comando), objeto);
        }
    }
}