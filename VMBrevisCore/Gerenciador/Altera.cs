using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using VMBrevisCore.Manipulador;

namespace VMBrevisCore.Gerenciador
{
    public class Altera : Dados
    {
        /// <summary> @ Propriedade ViraMundo
        /// Altera dados do banco comforme parametrizacao
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public T AlteraT<T>(T objeto, int conexao)
        {   
            List<SqlParameter> parametros = MontaParametros(objeto, Operacao.Alterar);
            var conecoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Conecoes.xml");
            //var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
            string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
            var acoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Acoes.xml");
            //var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
            string acao = acoes.XPathSelectElement("Alteracao").Value;
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

