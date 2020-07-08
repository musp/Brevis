using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using VMBrevisCore.Manipulador;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;

namespace VMBrevisCore.Gerenciador
{
    public class Busca : Dados
    {

        /// <summary> @ Propriedade ViraMundo
        /// Busca dados genericos por parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <returns></returns>
        /// 
        public T BuscaT<T>(T dados, int conexao)
        {
            try
            {
                List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);

                var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
                //var conecoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Conecoes.xml");
                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                //var acoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Acoes.xml");
                var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");

                string acao = acoes.XPathSelectElement("Selecao").Value;
                AbreConexao(new DadosConexao() { nome = stringDeConexaoCorrente, stringDeConexao = stringDeConexaoCorrente });
                char ultimoCaractere = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString().LastOrDefault();
                string comando = acao.Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                                     .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                                     .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
                return ConverteDeSqlDataReaderParaT(CarregaDadosGenericoT<object>(comando), dados);
            }
            catch
            {
                FechaConexao();
                throw;
            }
            finally
            {
                FechaConexao();
            }
        }

        public List<T> PesquisaT<T>(T dados, int conexao)
        {
            try
            {
                List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);

                var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
                //var conecoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Conecoes.xml");

                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
                //var acoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Acoes.xml");
                string acao = acoes.XPathSelectElement("SelecaoComJuncao").Value;
                AbreConexao(new DadosConexao() { nome = "", stringDeConexao = stringDeConexaoCorrente });
                acao = acao
                    .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                    .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                    .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
                return ConverteDeSqlListaDataReaderParaT<T>(CarregaDadosGenericoT<T>(acao.Replace("JUNCAO", "")));
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                FechaConexao();
            }
        }
        public List<T> PesquisaTodosT<T>(T dados, int conexao)
        {
            try
            {
                List<SqlParameter> parametros = MontaParametros(dados, Operacao.CarregarTodos);
                var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
                //var conecoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Conecoes.xml");
                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
                //var acoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Acoes.xml");
                string acao = acoes.XPathSelectElement("SelecaoComJuncao").Value;
                AbreConexao(new DadosConexao() { nome = "", stringDeConexao = stringDeConexaoCorrente });
                acao = acao
                    .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                    .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                    .Replace("CLAUSULAS", "");
                return ConverteDeSqlListaDataReaderParaT<T>(CarregaDadosGenericoT<T>(acao.Replace("JUNCAO", "")));
            }
            catch
            {
                throw;
            }
            finally
            {
                FechaConexao();
            }
        }
        public T UltimoId<T>(T dado, int conexao)
        {
            try
            {
                List<SqlParameter> parametros = MontaParametros(dado, Operacao.UltimoId);
                var conecoes = XElement.Load(@"D:\home\site\wwwroot\Conecoes.xml");
                //var conecoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Conecoes.xml");
                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
                //var acoes = XElement.Load(@"K:\ViraMundo\eventosvm\ProjetoEventoViraMundo\VMBrevisCore\Acoes.xml");
                AbreConexao(new DadosConexao() { nome = "", stringDeConexao = stringDeConexaoCorrente });
                return ConverteDeSqlListaDataReaderParaT<T>(CarregaDadosGenericoT<T>("SELECT top 1 * FROM " 
                    + parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString() + "." 
                    + parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString() + " order by Id desc")).FirstOrDefault();
            }
            catch
            {

                throw;
            }
            finally
            {
                FechaConexao();
            }
        }
    }
}
