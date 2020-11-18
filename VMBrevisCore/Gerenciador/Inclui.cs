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
        string caminhoBanco = "";
        public Inclui(string caminho)
        {
            caminhoBanco = caminho;
        }
        public T IncluiT<T>(T objeto, int conexao)
        {
            List<SqlParameter> parametros = MontaParametros(objeto, Operacao.Inserir);
            //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
            XElement conexoes = XElement.Load(@caminhoBanco+ @"\conexoes.xml");
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
            XElement acoes = XElement.Load(@caminhoBanco +@"\acoes.xml");
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