using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using VMBrevis.DadosAcesso;
using VMBrevis.Manipulador;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.IO;

namespace VMBrevis.Gerenciador
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
                //var conecoes = XElement.Load(@"C:\inetpub\wwwroot\VmSocial\Conecoes.xml");
                var conecoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Conecoes.xml");
                //                var conecoes = XElement.Load(new StringReader(@"<conexoes><ViraMundoDistribuidor>Data Source=DESKTOP-215201P;Initial Catalog=VMDistribuidor;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoDistribuidor>
                //  <ViraMundoFollowDocs>Data Source=DESKTOP-215201P;Initial Catalog=VMFWD;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoFollowDocs>
                //  <ViraMundoLogin>Data Source=DESKTOP-215201P;Initial Catalog=VMSLogin;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLogin>
                //  <ViraMundoSocial>Data Source=socialroma.database.windows.net;Initial Catalog=SocialRoma;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoSocial>
                //  <!--<ViraMundoValidacao>Data Source=DESKTOP-215201P;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoValidacao>-->
                //  <ViraMundoValidacao>Data Source=socialroma.database.windows.net;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoValidacao>
                //  <ViraMundoLojaVirtual>Data Source=DESKTOP-215201P;Initial Catalog=VMLojaVirtual;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLojaVirtual>
                //</conexoes>"));
                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                //var acoes = XElement.Load(@"C:\inetpub\wwwroot\VmSocial\Acoes.xml");
                var acoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Acoes.xml");
                //                var acoes = XElement.Load(new StringReader(
                //           @"<acoes>
                //  <Selecao>SELECT * FROM ESQUEMA.TABELA WHERE 1=1 CLAUSULAS</Selecao>
                //  <Alteracao>UPDATE ESQUEMA.TABELA SET COLUNASDADOS WHERE 1=1 CLAUSULAS</Alteracao>
                //  <Adicao>INSERT INTO ESQUEMA.TABELA (COLUNAS) VALUES (DADOS) SELECT SCOPE_IDENTITY() as Id</Adicao>
                //  <RemocaoFisica>DELETE FROM ESQUEMA.TABELA WHERE CLAUSULAS</RemocaoFisica>
                //  <RemocaoLogica>UPDATE ESQUEMA.TABELA SET COLUNASDADOS WHERE 1=1 CLAUSULAS</RemocaoLogica>
                //  <SelecaoComJuncao>SELECT * FROM ESQUEMA.TABELA JUNCAO WHERE 1=1 CLAUSULAS</SelecaoComJuncao>
                //  <Juncao>ESQUEMA.TABELA ON (ColunasJuncao)</Juncao>
                //  <ColunasJuncao>COLUNA0 = COLUNA1</ColunasJuncao>
                //</acoes>"));
                string acao = acoes.XPathSelectElement("Selecao").Value;
                AbreConexao(new DadosConexao() { nome = stringDeConexaoCorrente, stringDeConexao = stringDeConexaoCorrente });
                char ultimoCaractere = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString().LastOrDefault();
                string nomeTabela = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString();
                if (ultimoCaractere == 'm')
                    nomeTabela = nomeTabela.Substring(0, nomeTabela.Count() - 1) + "ns";
                string comando = acao.Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                                     .Replace("TABELA", ultimoCaractere == 'm' ? nomeTabela : nomeTabela + "s")
                                     .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
                return ConverteDeSqlDataReaderParaT(CarregaDadosGenericoT<object>(comando), dados);
            }
            catch (Exception ex)
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
                //var conecoes = XElement.Load(@"C:\inetpub\wwwroot\VmSocial\Conecoes.xml");
                var conecoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Conecoes.xml");
                //                var conecoes = XElement.Load(new StringReader(@"<conexoes><ViraMundoDistribuidor>Data Source=DESKTOP-215201P;Initial Catalog=VMDistribuidor;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoDistribuidor>
                //  <ViraMundoFollowDocs>Data Source=DESKTOP-215201P;Initial Catalog=VMFWD;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoFollowDocs>
                //  <ViraMundoLogin>Data Source=DESKTOP-215201P;Initial Catalog=VMSLogin;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLogin>
                //  <ViraMundoSocial>Data Source=socialroma.database.windows.net;Initial Catalog=SocialRoma;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoSocial>
                //  <!--<ViraMundoValidacao>Data Source=DESKTOP-215201P;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoValidacao>-->
                //  <ViraMundoValidacao>Data Source=socialroma.database.windows.net;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoValidacao>
                //  <ViraMundoLojaVirtual>Data Source=DESKTOP-215201P;Initial Catalog=VMLojaVirtual;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLojaVirtual>
                //</conexoes>"));
                string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
                //var acoes = XElement.Load(@"C:\inetpub\wwwroot\VmSocial\Acoes.xml");
                var acoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Acoes.xml");
                //                var acoes = XElement.Load(new StringReader(
                //           @"<acoes>
                //  <Selecao>SELECT * FROM ESQUEMA.TABELA WHERE 1=1 CLAUSULAS</Selecao>
                //  <Alteracao>UPDATE ESQUEMA.TABELA SET COLUNASDADOS WHERE 1=1 CLAUSULAS</Alteracao>
                //  <Adicao>INSERT INTO ESQUEMA.TABELA (COLUNAS) VALUES (DADOS) SELECT SCOPE_IDENTITY() as Id</Adicao>
                //  <RemocaoFisica>DELETE FROM ESQUEMA.TABELA WHERE CLAUSULAS</RemocaoFisica>
                //  <RemocaoLogica>UPDATE ESQUEMA.TABELA SET COLUNASDADOS WHERE 1=1 CLAUSULAS</RemocaoLogica>
                //  <SelecaoComJuncao>SELECT * FROM ESQUEMA.TABELA JUNCAO WHERE 1=1 CLAUSULAS</SelecaoComJuncao>
                //  <Juncao>ESQUEMA.TABELA ON (ColunasJuncao)</Juncao>
                //  <ColunasJuncao>COLUNA0 = COLUNA1</ColunasJuncao>
                //</acoes>"));
                string acao = acoes.XPathSelectElement("SelecaoComJuncao").Value;
                AbreConexao(new DadosConexao() { nome = "", stringDeConexao = stringDeConexaoCorrente });
                char ultimoCaractere = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString().LastOrDefault();
                string nomeTabela = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString();
                if (ultimoCaractere == 'm')
                    nomeTabela = nomeTabela.Substring(0, nomeTabela.Count() - 1) + "ns";
                acao = acao
                    .Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                    .Replace("TABELA", ultimoCaractere == 'm' ? nomeTabela : nomeTabela + "s")
                    .Replace("CLAUSULAS", parametros.Where(s => s.ParameterName == "Parametros").FirstOrDefault().Value.ToString());
                //string juncao = acoes.XPathSelectElement("Juncao").Value;
                //string comando = "";
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

        //public List<T> BuscaListaT<T>(T dados)
        //{
        //    try
        //    {
        //        List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);
        //        var doc = XElement.Load("C:\EmFrente\prjViraMundo\VMBrevis\Conecoes.xml");
        //        string stringDeConexaoCorrente = doc.XPathSelectElement(ConexoesAcessos.ViraMundoLojaVirtual.ToString()).Value.Remove(0, 6).Remove(114);
        //        AbreConexao(new DadosConexao() { nome = ConexoesAcessos.ViraMundoBrevis.ToString(), stringDeConexao = stringDeConexaoCorrente });
        //        SqlDataReader datareader = ExecucaoGenerica<object>(parametros, Esquemas.VMP);
        //        return ConverteDeSqlListaDataReaderParaT<T>(datareader);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<T>();

        //        throw;
        //    }
        //    finally
        //    {
        //        FechaConexao();
        //    }
        //}
    }


}
