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
        string caminhoBanco = "";
        public Busca(string caminho)
        {
            caminhoBanco = caminho;
        }

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
                //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
                var conexoes = XElement.Load(@caminhoBanco + @"\conexoes.xml");
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
                var acoes = XElement.Load(@caminhoBanco + @"\acoes.xml");
                //var acoes = XElement.Load(@"D:\home\site\wwwroot\Acoes.xml");
                string acao = acoes.XPathSelectElement("Selecao").Value;
                AbreConexao(new DadosConexao() { nome = conexao.ToString(), stringDeConexao = stringDeConexaoCorrente });
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
                //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
                var conexoes = XElement.Load(@caminhoBanco + @"\conexoes.xml");
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
                var acoes = XElement.Load(@caminhoBanco + @"\acoes.xml");
                string acao = acoes.XPathSelectElement("SelecaoComJuncao").Value;
                AbreConexao(new DadosConexao() { nome = "", stringDeConexao = stringDeConexaoCorrente });
                acao = acao
                    .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                    .Replace("TABELA", parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString())
                    .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
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
        public List<T> PesquisaTodosT<T>(T dados, int conexao)
        {
            try
            {
                List<SqlParameter> parametros = MontaParametros(dados, Operacao.CarregarTodos);
                //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
                var conexoes = XElement.Load(@caminhoBanco + @"\Conexoes.xml");
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
                //var conexoes = XElement.Load(@"D:\home\site\wwwroot\conexoes.xml");
                var conexoes = XElement.Load(@caminhoBanco + @"\conexoes.xml");
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
                var acoes = XElement.Load(@caminhoBanco + @"\acoes.xml");
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