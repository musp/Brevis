using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using VMBrevis.Manipulador;

namespace VMBrevis.Gerenciador
{
    public class Inclui : Dados 
    {
        public T IncluiT<T>(T objeto, int conexao)
        {
            List<SqlParameter> parametros = MontaParametros(objeto, Operacao.Inserir);
            //var conecoes = XElement.Load(@"C:\inetpub\wwwroot\VmSocial\Conecoes.xml");
            var conecoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Conecoes.xml");
            //            var conecoes = XElement.Load(new StringReader(@"<conexoes><ViraMundoDistribuidor>Data Source=DESKTOP-215201P;Initial Catalog=VMDistribuidor;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoDistribuidor>
            //  <ViraMundoFollowDocs>Data Source=DESKTOP-215201P;Initial Catalog=VMFWD;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoFollowDocs>
            //  <ViraMundoLogin>Data Source=DESKTOP-215201P;Initial Catalog=VMSLogin;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLogin>
            //  <ViraMundoSocial>Data Source=socialroma.database.windows.net;Initial Catalog=SocialRoma;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoSocial>
            //  <!--<ViraMundoValidacao>Data Source=DESKTOP-215201P;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoValidacao>-->
            //  <ViraMundoValidacao>Data Source=socialroma.database.windows.net;Initial Catalog=VMValidacao;Persist Security Info=True;User ID=muspsocialroma;Password=keycode@musp.com9858</ViraMundoValidacao>
            //  <ViraMundoLojaVirtual>Data Source=DESKTOP-215201P;Initial Catalog=VMLojaVirtual;Persist Security Info=True;User ID=sa;Password=keycode</ViraMundoLojaVirtual>
            //</conexoes>"));
            string stringDeConexaoCorrente = conecoes.XPathSelectElement(RetornaConexao(conexao).ToString()).Value;
            //var acoes = XElement.Load(@"C:\inetpub\wwwroot\VmSoc ial\Acoes.xml");
            var acoes = XElement.Load(@"C:\EmFrente\prjViraMundo\VMBrevis\Acoes.xml");
            //            var acoes = XElement.Load(new StringReader(
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
            string acao = acoes.XPathSelectElement("Adicao").Value;
            AbreConexao(new DadosConexao() { nome = stringDeConexaoCorrente, stringDeConexao = stringDeConexaoCorrente });
            char ultimoCaractere = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString().LastOrDefault();
            string nomeTabela = parametros.Where(s => s.ParameterName.Contains("tabela")).FirstOrDefault().Value.ToString();
            if (ultimoCaractere == 'm')
                nomeTabela = nomeTabela.Substring(0, nomeTabela.Count() - 1) + "ns";
            string comando = acao.Replace("ESQUEMA", parametros.Where(s => s.ParameterName.Contains("esquema")).FirstOrDefault().Value.ToString())
                .Replace("TABELA", ultimoCaractere == 'm' ? nomeTabela : nomeTabela + "s").Replace("COLUNAS", parametros.Where(s => s.ParameterName == "Colunas").FirstOrDefault().Value.ToString()).Replace("DADOS", parametros.Where(s => s.ParameterName == "Valores").FirstOrDefault().Value.ToString());
            return ConverteDeSqlDataReaderParaT(CarregaDadosGenericoT<object>(comando), objeto);
        }
    }
}
